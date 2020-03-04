using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class InsulinReadingResource
    {
        [Key]
        public int Id { get; set; }

        public double reading { get; set; }

        public int insulinType { get; set; }//1-short for a food, 2-long for the day

        public DateTime created { get; set; }

        [ForeignKey("MealTimeResource")]
        public int? MealTimeResourceId { get; set; }
        public virtual MealTimeResource MealTimeResource { get; set; }

        [ForeignKey("UserResource")]
        public int? UserResourceId { get; set; }
        public virtual UserResource UserResource { get; set; }
    }
}
