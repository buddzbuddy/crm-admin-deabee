using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicClassGenerator;

namespace Web
{
    public partial class ProxyODataService : DataServiceContext
    {
        public ProxyODataService(Uri serviceRoot)
            : base(serviceRoot, global::System.Data.Services.Common.DataServiceProtocolVersion.V3)
        {
            this.ResolveName = new global::System.Func<global::System.Type, string>(this.ResolveNameFromType);
            this.ResolveType = new global::System.Func<string, global::System.Type>(this.ResolveTypeFromName);
            this.OnContextCreated();
        }

        partial void OnContextCreated();
        protected string ResolveNameFromType(global::System.Type clientType)
        {
            if (clientType.Namespace.Equals("Dynamic.Objects", global::System.StringComparison.Ordinal))
            {
                return string.Concat("DynamicServerSide.", clientType.Name);
            }
            return null;
        }
        protected global::System.Type ResolveTypeFromName(string typeName)
        {
            global::System.Type resolvedType = this.DefaultResolveType(typeName, "DynamicServerSide", "Dynamic.Objects");
            if ((resolvedType != null))
            {
                return resolvedType;
            }
            return null;
        }
        public void LoadTypes(IEnumerable<DynamicTemplate> templates)
        {
            if (templates != null && templates.Count() > 0)
            {
                var dcf = new DynamicClassFactory();
                //Types = new Dictionary<string, Type>();

                foreach (var dynamicTemplate in templates)
                {
                    if(!Types.ContainsKey(dynamicTemplate.Name))
                    {
                        var cols = dynamicTemplate.DynamicFields.Where(x => x.SqlTypeEnumCode.HasValue).ToList();
                        if(cols.Count > 0)
                        {
                            var type = CreateType(dcf, dynamicTemplate.Name, cols);
                            Types.Add(type.Name, type);
                        }
                    }
                }

                //dcf.SaveAssembly();
            }
        }

        public DataServiceQuery GetServiceQuery(string name)
        {
            var odd = typeof(ProxyODataService).GetMethod("CreateQuery");
            var mi = odd.MakeGenericMethod(Types[name]);
            var qr = mi.Invoke(this, new[] { name + "s" }) as DataServiceQuery; //assumes plural
            return qr;
        }
        private Dictionary<int, Type> ColumnTypeByCode = new Dictionary<int, Type>
        {
            {10, typeof(int) },
            {3, typeof(bool) },
            {6, typeof(DateTime) },
            {7, typeof(decimal) },
            {15, typeof(string) }
        };
        private Type CreateType(DynamicClassFactory dcf, string name, ICollection<DynamicField> dynamicAttributes)
        {
            var props = dynamicAttributes.ToDictionary(da => da.Name, da => ColumnTypeByCode[da.SqlTypeEnumCode.Value]);
            var type = dcf.CreateDynamicType<BaseDynamicEntity>(name, props);
            return type;
        }

        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();//{ get; private set; }

    }
}
