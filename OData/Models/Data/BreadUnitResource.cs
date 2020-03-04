using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class BreadUnitResource
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ProductResource")]
        public int? ProductResourceId { get; set; }
        public virtual ProductResource ProductResource { get; set; }

        public double Gram { get; set; }

        public double Kkal { get; set; }

        public double Protein { get; set; }

        public double Fat { get; set; }

        public double Carb { get; set; }

        public double BU { get; set; }

        public double GI { get; set; }
    }
}
