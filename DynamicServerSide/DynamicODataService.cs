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
using DynamicServerSide.Models.Metadata;
using DynamicServerSide.Models.Metadata.Entities;
using DynamicServerSide.Models.OData;
using Microsoft.SqlServer.Management.Smo;

namespace DynamicServerSide
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DynamicODataService : EntityFrameworkDataService<DynamicODataContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.AcceptProjectionRequests = true;
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }

        protected override DynamicODataContext CreateDataSource()
        {
            var result = base.CreateDataSource();
            var dcf = new DynamicClassFactory();

            var context = new DynamicMetadataContext();
            var templates = (from t in context.DynamicTemplates.Include("DynamicFields")
                             select t).ToList();

            foreach (var dynamicTemplate in templates.Where(x => x.IsExist == 1))
            {
                var cols = dynamicTemplate.DynamicFields.Where(x => x.SqlTypeEnumCode.HasValue).OrderBy(x => x.OrderIndex).ToList();
                if (cols.Count > 0)
                {
                    var type = CreateType(dcf, dynamicTemplate.Name, cols);
                    result.AddTable(type);
                }
            }
            
            return result;
        }

        private readonly Dictionary<int, Type> ColumnTypeByCode = new Dictionary<int, Type>
        {
            {10, typeof(int) },
            {1000, typeof(int?) },
            {3, typeof(bool) },
            {300, typeof(bool?) },
            {6, typeof(DateTime) },
            {600, typeof(DateTime?) },
            {7, typeof(decimal) },
            {700, typeof(decimal?) },
            {15, typeof(string) },
            {1500, typeof(string) }
        };

        //TODO: Create Nullable props
        private Type CreateType(DynamicClassFactory dcf, string name, ICollection<DynamicField> dynamicFields)
        {
            var props = dynamicFields.ToDictionary(da => da.Name, da => ColumnTypeByCode[!da.IsNullable ? da.SqlTypeEnumCode.Value : (da.SqlTypeEnumCode.Value * 100)]);
            var t = dcf.CreateDynamicType<BaseDynamicEntity>(name, props);
            return t;
        }

        [WebInvoke(Method = "POST")]
        public string DeleteTemplate(int templateId)
        {
            try
            {
                var context = new DynamicMetadataContext();
                var tmpl = context.DynamicTemplates.Find(templateId);
                if (tmpl != null)
                {
                    if(tmpl.IsExist == 0)
                    {
                        context.DynamicTemplates.Remove(tmpl);
                        context.SaveChanges();
                    }
                    else
                        throw new ApplicationException("Template of " + tmpl.Name + " has existing table in database. First drop that table!");
                }
                else
                    throw new ApplicationException("Template not found by Id: " + templateId);
            }
            catch (Exception e)
            {
                return string.Format("Error message: {0}, trace: {1}", e.Message, e.StackTrace);
            }
            return "OK";
        }
        
        [WebInvoke(Method = "POST")]
        public string CreateTableSmo(int templateId, string f)
        {
            try
            {
                if (string.IsNullOrEmpty(f)) f = "T";
                var context = new DynamicMetadataContext();
                var templateObj = context.DynamicTemplates.Include("DynamicFields").FirstOrDefault(x => x.Id == templateId);
                if (templateObj == null)
                {
                    throw new ApplicationException("Template not found by Id: " + templateId);
                }

                string tableName = templateObj.Name;

                //SMO Create table
                String sqlServerLogin = "sa";
                String password = "QQQwww123";
                Server srv = default(Server);
                srv = new Server();

                srv.ConnectionContext.LoginSecure = false;
                srv.ConnectionContext.Login = sqlServerLogin;
                srv.ConnectionContext.Password = password;
                if (srv.Databases[context.Database.Connection.Database].Tables.Contains(tableName))
                    return "Table " + tableName + " already exist in \"" + context.Database.Connection.Database + "\" database";

                var newTable = new Table(srv.Databases[context.Database.Connection.Database], tableName);
                //Create Id by default
                var id = new Column(newTable, "Id")
                {
                    DataType = new DataType(SqlDataType.Int),
                    Identity = true,
                    IdentityIncrement = 1
                };

                newTable.Columns.Add(id);

                Index index = new Index(newTable, "PK_" + tableName)
                {
                    IndexKeyType = IndexKeyType.DriPrimaryKey
                };
                index.IndexedColumns.Add(new IndexedColumn(index, "Id"));
                newTable.Indexes.Add(index);

                //Create columns
                //1: Parse columns from URL
                var fields = templateObj.DynamicFields.Where(x => x.SqlTypeEnumCode.HasValue).ToList();
                if (fields.Count == 0)
                {
                    return "columns are empty!";
                }
                foreach (var field in fields)
                {
                    var c = new Column(newTable, field.Name)
                    {
                        DataType = new DataType((SqlDataType)field.SqlTypeEnumCode),
                        Nullable = field.IsNullable
                    };
                    if (!string.IsNullOrEmpty(field.DefaultValue)) c.Default = field.DefaultValue;

                    newTable.Columns.Add(c);
                }

                if (f == "T")
                {
                    string t_sql = "T-SQL:\n" + string.Join("\n", newTable.EnumScript());
                    return t_sql;
                }
                else if (f == "Y")
                {
                    newTable.Create();
                    return "Table is created successful! Is reloading service: " + ReloadService();
                }
                else
                    return "Parameter 'f' is not recognized! f=" + f;
            }
            catch (Exception e)
            {
                return e.Message + ", trace: " + e.StackTrace;
            }
        }
        
        [WebInvoke(Method = "POST")]
        public string DropTableSmo(int templateId, string f)
        {
            try
            {
                if (string.IsNullOrEmpty(f)) f = "T";
                var context = new DynamicMetadataContext();
                var templateObj = context.DynamicTemplates.Find(templateId);
                if (templateObj == null)
                {
                    throw new ApplicationException("Template not found by Id: " + templateId);
                }

                string tableName = templateObj.Name;

                String sqlServerLogin = "sa";
                String password = "QQQwww123";
                Server srv = default(Server);
                srv = new Server();

                srv.ConnectionContext.LoginSecure = false;
                srv.ConnectionContext.Login = sqlServerLogin;
                srv.ConnectionContext.Password = password;

                if (f == "T")
                {
                    if (!srv.Databases[context.Database.Connection.Database].Tables.Contains(tableName))
                        return "Table " + tableName + " doesn't exist in database";
                    var tbl = srv.Databases[context.Database.Connection.Database].Tables[tableName];
                    if (tbl.RowCount > 0)
                    {
                        return "This table has " + tbl.RowCount + " rows! Delete rows first!";
                    }
                    string t_sql = "T-SQL:\n" + "DROP TABLE [dbo].[" + tableName + "]";
                    return t_sql;
                }
                else if (f == "Y")
                {
                    if (!srv.Databases[context.Database.Connection.Database].Tables.Contains(tableName))
                        return "Table " + tableName + " doesn't exist in database";
                    var tbl = srv.Databases[context.Database.Connection.Database].Tables[tableName];
                    if (tbl.RowCount > 0)
                    {
                        return "This table has " + tbl.RowCount + " rows! Delete rows first!";
                    }
                    srv.Databases[context.Database.Connection.Database].Tables[tableName].Drop();
                    return "Table is dropped successful! Is reloading service: " + ReloadService();
                }
                else
                    return "Parameter 'f' is not recognized! f=" + f;
            }
            catch (Exception e)
            {
                return e.Message + ", trace: " + e.StackTrace;
            }
        }

        private bool IsExistingTable(string templateName)
        {
            String sqlServerLogin = "sa";
            String password = "QQQwww123";
            Server srv = default(Server);
            srv = new Server();

            srv.ConnectionContext.LoginSecure = false;
            srv.ConnectionContext.Login = sqlServerLogin;
            srv.ConnectionContext.Password = password;
            var context = new DynamicMetadataContext();
            return srv.Databases[context.Database.Connection.Database].Tables.Contains(templateName);
        }

        [WebInvoke(Method = "POST")]
        public string ReloadService()
        {
            try
            {
                return "OK";
            }
            finally
            {
                HttpRuntime.UnloadAppDomain();
            }
        }

    }
}
