using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DynamicClassGenerator;
using Microsoft.SqlServer.Management.Smo;

namespace DynamicServerSide
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DynamicODataService : EntityFrameworkDataService<DynamicDbContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            //config.SetServiceOperationAccessRule("CreateTable", ServiceOperationRights.All);
            config.SetServiceOperationAccessRule("CreateTableSmo", ServiceOperationRights.All);
            //config.SetServiceActionAccessRule("CreateTable", ServiceActionRights.Invoke);
            config.DataServiceBehavior.AcceptProjectionRequests = true;
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }

        protected override DynamicDbContext CreateDataSource()
        {
            var result = base.CreateDataSource();
            var dcf = new DynamicClassFactory();

            var context = new DynamicEntities();
            var templates = (from t in context.DynamicTemplates
                                             .Include("DynamicTemplateAttributes")
                                             .Include("DynamicTemplateAttributes.DynamicAttribute")
                                             .Include("DynamicTemplateAttributes.DynamicType")
                             select t);

            foreach (var dynamicTemplate in templates)
            {
                var type = CreateType(dcf, dynamicTemplate.Name, dynamicTemplate.DynamicTemplateAttributes);
                result.AddTable(type);
            }
            
            return result;
        }

        private Dictionary<int, Type> ColumnTypeByCode = new Dictionary<int, Type>
        {
            {1, typeof(Int64) },
            {2, typeof(Byte[]) },
            {3, typeof(Boolean) },
            {6, typeof(DateTime) },
            {7, typeof(Decimal) },
            {8, typeof(Double) },
            {10, typeof(Int32) },
            {11, typeof(Decimal) },
            {12, typeof(Char[]) },
            {14, typeof(String) },
            {15, typeof(String) }
        };

        private Type CreateType(DynamicClassFactory dcf, string name, ICollection<DynamicTemplateAttribute> dynamicAttributes)
        {
            //TODO: Define all types
            var props = dynamicAttributes.ToDictionary(da => da.DynamicAttribute.Name, da => ColumnTypeByCode[da.DynamicType.SqlTypeEnumCode]);
            var t = dcf.CreateDynamicType<BaseDynamicEntity>(name, props);
            return t;
        }
        
        private bool AddToMetadata(string tableName, List<Column> columns, out string errorMessage)
        {
            errorMessage = "";
            var context = new DynamicEntities();
            var newTemplate = new DynamicTemplate
            {
                Name = tableName,
                Description = tableName + " Desc"
            };
            try
            {
                context.Database.BeginTransaction();
                context.DynamicTemplates.Add(newTemplate);
                context.SaveChanges();
                foreach (var c in columns)
                {
                    var field = new DynamicAttribute
                    {
                        Name = c.Name
                    };
                    context.DynamicAttributes.Add(field);
                    context.SaveChanges();
                    var typeCode = (int)c.DataType.SqlDataType;
                    var sqlTypeId = context.DynamicTypes.FirstOrDefault(x => x.SqlTypeEnumCode == typeCode)?.Id ?? 0;
                    if (sqlTypeId == 0) throw new ApplicationException("SqlTypeId not found in DB for code: " + typeCode);
                    var templateAttr = new DynamicTemplateAttribute
                    {
                        DisplayName = c.Name,
                        TypeId = sqlTypeId,
                        TemplateId = newTemplate.Id,
                        AttributeId = field.Id,
                        Idx = 0,
                        DynamicAttribute = field
                    };
                    context.DynamicTemplateAttributes.Add(templateAttr);
                    context.SaveChanges();
                }
                context.Database.CurrentTransaction.Commit();
            }
            catch (Exception e)
            {
                context.Database.CurrentTransaction.Rollback();
                errorMessage = e.GetBaseException().Message;
                return false;
            }
            return true;
        }

        private bool DeleteFromMetadata(string templateName, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                var context = new DynamicEntities();
                var newTemplate = context.DynamicTemplates.FirstOrDefault(x => x.Name == templateName);
                context.DynamicTemplates.Remove(newTemplate);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                errorMessage = e.GetBaseException().Message;
                return false;
            }
            return true;
        }

        //[WebInvoke(Method = "POST")]
        //public string CreateTable(string template)
        //{
        //    try
        //    {
        //        var context = new DynamicEntities();
        //        var qry = (from dt in context.DynamicTemplates.Include("DynamicTemplateAttributes")
        //            .Include("DynamicTemplateAttributes.DynamicAttribute")
        //                   select dt).FirstOrDefault(dt => dt.Name == template);
        //        if (qry == null)
        //            throw new ArgumentException(string.Format("The template {0} does not exist", template));
        //        var ct = new StringBuilder();
        //        ct.AppendFormat("CREATE TABLE {0} (Id int IDENTITY(1,1) NOT NULL, ", qry.Name);
        //        foreach (var dta in qry.DynamicTemplateAttributes)
        //        {
        //            ct.AppendFormat("{0} nvarchar(255) NULL, ", dta.DynamicAttribute.Name);
        //        }
        //        ct.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED", qry.Name);
        //        ct.AppendFormat("(Id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF," +
        //                        "ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]");
        //        var ts = ct.ToString();
        //        CurrentDataSource.Database.ExecuteSqlCommand(ts);
        //        return "OK";
        //    }
        //    catch(Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        [WebInvoke(Method = "POST")]
        public string CreateTableSmo(string template, string operation, string tableName, string f, string columns)
        {
            try
            {
                //SMO Create table
                String sqlServerLogin = "sa";
                String password = "QQQwww123";
                Server srv = default(Server);
                srv = new Server();

                srv.ConnectionContext.LoginSecure = false;
                srv.ConnectionContext.Login = sqlServerLogin;
                srv.ConnectionContext.Password = password;
                
                if (operation == "create")
                {
                    if (srv.Databases["Dynamic"].Tables.Contains(tableName))
                        return "Table " + tableName + " already exist in database";
                    
                    var newTable = new Table(srv.Databases["Dynamic"], tableName);
                    //Create Id by default
                    var id = new Column(newTable, "Id");
                    id.DataType = new DataType(SqlDataType.Int);
                    id.Identity = true;
                    id.IdentityIncrement = 1;
                    
                    newTable.Columns.Add(id);

                    Index index = new Index(newTable, "PK_" + tableName);
                    index.IndexKeyType = IndexKeyType.DriPrimaryKey;
                    index.IndexedColumns.Add(new IndexedColumn(index, "Id"));
                    newTable.Indexes.Add(index);
                    
                    //Create columns
                    //1: Parse columns from URL
                    if (string.IsNullOrEmpty(columns))
                    {
                        return "columns are empty!";
                    }
                    var cols = columns.Split(',');
                    var colList = new List<Column>();
                    foreach(var col in cols)
                    {
                        var colProps = col.Split(':');
                        if (string.IsNullOrEmpty(colProps[0]))
                            return "colProps[0] is empty";
                        var colName = colProps[0].Trim();
                        if (string.IsNullOrEmpty(colProps[1]))
                            return "colProps[1] is empty";
                        var colType = (SqlDataType)int.Parse(colProps[1]);
                        if (string.IsNullOrEmpty(colProps[2]))
                            return "colProps[2] is empty";
                        var colIsNull = bool.Parse(colProps[2]);
                        var colDefaultValue = colProps.ElementAtOrDefault(3);
                        var c = new Column(newTable, colName);
                        c.DataType = new DataType(colType);
                        if (string.IsNullOrEmpty(colDefaultValue)) c.Default = colDefaultValue;
                        c.Nullable = colIsNull;
                        
                        newTable.Columns.Add(c);
                        colList.Add(c);
                    }
                    
                    if (f == "T")
                    {
                        string t_sql = "T-SQL:\n" + string.Join("\n", newTable.EnumScript());
                        return t_sql;
                    }
                    else if (f == "Y")
                    {
                        newTable.Create();
                        if (!AddToMetadata(tableName, colList, out string errorMessage))
                        {
                            return "Table was created, but metadata had an error, and " + CreateTableSmo(template, "drop", tableName, "Y", columns) + " back! errorMessage: " + errorMessage;
                        }
                        HttpRuntime.UnloadAppDomain();
                        return "Table and metadata was created!";
                    }
                    else
                        return "Parameter 'f' is not recognized!";
                }
                else
                {
                    if (f == "T")
                    {
                        if (!srv.Databases["Dynamic"].Tables.Contains(tableName))
                            return "Table " + tableName + " doesn't exist in database";
                        string t_sql = "T-SQL:\n" + "DROP TABLE [dbo].[" + tableName + "]";
                        return t_sql;
                    }
                    else if (f == "Y")
                    {
                        if (!srv.Databases["Dynamic"].Tables.Contains(tableName))
                            return "Table " + tableName + " doesn't exist in database";
                        var tbl = srv.Databases["Dynamic"].Tables[tableName];
                        if(tbl.RowCount > 0)
                        {
                            return "This table has " + tbl.RowCount + " rows! Delete rows first!";
                        }
                        if(!DeleteFromMetadata(tableName, out string errorMessage))
                        {
                            return "Table wasn't dropped, cause by metadata errorMessage: " + errorMessage;
                        }
                        srv.Databases["Dynamic"].Tables[tableName].Drop();
                        HttpRuntime.UnloadAppDomain();
                        return "Table and metadata was dropped!";
                    }
                    else
                        return "Parameter 'f' is not recognized!";
                }
                //return tb.Columns.Count.ToString() + " columns; T-SQL:" + newTable.Script()[0];
                //return "OK";
            }
            catch (Exception e)
            {
                return e.Message + ", trace: " + e.StackTrace;
            }
        }
    }
}
