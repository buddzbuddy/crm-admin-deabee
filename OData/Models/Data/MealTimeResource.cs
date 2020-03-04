using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class MealTimeResource
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }

        public virtual ICollection<FoodReadingResource> FoodReadingResources { get; set; }
    }
}
