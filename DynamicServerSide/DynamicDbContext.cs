using System;
using System.Collections.Generic;
using System.Data.Entity;
using MagicDbModelBuilder;

namespace DynamicServerSide
{
    public partial class DynamicDbContext : DbContext
    {
        public DynamicDbContext()
            : base("name=DynamicDbContext")
        {
            Database.SetInitializer(new NullDatabaseInitializer<DynamicDbContext>());
        }

        public void AddTable(Type type)
        {
            _tables.Add(type.Name, type);
        }

        private Dictionary<string, Type> _tables = new Dictionary<string, Type>();

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var entityMethod = modelBuilder.GetType().GetMethod("Entity");

            foreach (var table in _tables)
            {
                entityMethod.MakeGenericMethod(table.Value).Invoke(modelBuilder, new object[] { });
                foreach (var pi in (table.Value).GetProperties())
                {
                    if (pi.Name == "Id")
                        modelBuilder.Entity(table.Value).HasKey(typeof(int), "Id");
                    else
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            modelBuilder.Entity(table.Value).StringProperty(pi.Name);
                        }
                        else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(DateTime))
                        {
                            modelBuilder.Entity(table.Value).PrimitiveProperty(pi.PropertyType, pi.Name);
                        }
                        else
                            throw new ApplicationException("Cannot create propertyType for " + pi.Name + " of entity " + table.Key);
                    }
                }
            }
        }
    }
}
