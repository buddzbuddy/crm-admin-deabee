using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class DrugResource
    {
        [Key]
        public int Id { get; set; }

        public string full_name { get; set; }
        public string NoPatentName { get; set; }

        [ForeignKey("DrugTypeResource")]
        public int? DrugTypeResourceId { get; set; }
        public virtual DrugTypeResource DrugTypeResource { get; set; }
    }
}
