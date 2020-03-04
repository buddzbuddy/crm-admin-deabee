using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServerSide.Models.Metadata.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Route { get; set; }

        public string IconName { get; set; }

        public int? OrderIndex { get; set; }

        [ForeignKey("ParentMenuItem")]
        public int? ParentMenuItemId { get; set; }
        public virtual MenuItem ParentMenuItem { get; set; }

        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }
}