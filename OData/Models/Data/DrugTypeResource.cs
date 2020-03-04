using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class DrugTypeResource
    {
        [Key]
        public int Id { get; set; }

        [Column("full_name")]
        public string Name { get; set; }
        public string code { get; set; }

        public virtual ICollection<DrugResource> DrugResources { get; set; }
    }
}
