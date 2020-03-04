using DynamicServerSide.Models.Metadata.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServerSide.Models.Metadata
{
    public class DynamicMetadataContext : DbContext
    {
        public DynamicMetadataContext()
            : base("name=DynamicDbContext")
        {

        }
        public virtual DbSet<DynamicTemplate> DynamicTemplates { get; set; }
        public virtual DbSet<DynamicField> DynamicFields { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }

    }
}
