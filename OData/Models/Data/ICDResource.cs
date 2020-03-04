using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class ICDResource
    {
        [Key]
        public int Id { get; set; }

        public string code { get; set; }

        [Column("disease")]
        public string Name
        {
            get;
            set;
        }
    }
}
