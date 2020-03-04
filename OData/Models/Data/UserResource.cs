using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class UserResource
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Preferred_language { get; set; }

        public string Fullname { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        [ForeignKey("ICDResource")]
        public int? ICDResourceId { get; set; }
        public virtual ICDResource ICDResource { get; set; }

        public string InsulinName { get; set; }

        public string InsulinCompany { get; set; }

        public string Preferred_unit { get; set; }

        public string Preferred_unit_a1c { get; set; }

        public string Preferred_unit_weight { get; set; }

        public string Preferred_range { get; set; }

        public double Custom_range_min { get; set; }
        public double Custom_range_max { get; set; }

        [ForeignKey("UserTypeResource")]
        public int? UserTypeResourceId { get; set; }
        public virtual UserTypeResource UserTypeResource { get; set; }

        public int D_type { get; set; }


        public virtual ICollection<GlucoseReadingResource> GlucoseReadingResources { get; set; }
        public virtual ICollection<FoodReadingResource> FoodReadingResources { get; set; }
        public virtual ICollection<CholesterolReadingResource> CholesterolReadingResources { get; set; }
        public virtual ICollection<HB1ACReadingResource> HB1ACReadingResources { get; set; }
        public virtual ICollection<KetoneReadingResource> KetoneReadingResources { get; set; }
        public virtual ICollection<PressureReadingResource> PressureReadingResources { get; set; }
        public virtual ICollection<WeightReadingResource> WeightReadingResources { get; set; }
        public virtual ICollection<InsulinReadingResource> InsulinReadingResources { get; set; }
    }
}
