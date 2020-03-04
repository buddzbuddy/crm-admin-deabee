using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class ProductResource
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [ForeignKey("QualityLevelResource")]
        public int? QualityLevelResourceId { get; set; }
        public virtual QualityLevelResource QualityLevelResource { get; set; }


        public virtual ICollection<ProductPhotoResource> ProductPhotoResources { get; set; }
        public virtual ICollection<FoodReadingResource> FoodReadingResources { get; set; }
        public virtual ICollection<BreadUnitResource> BreadUnitResources { get; set; }
    }
}
