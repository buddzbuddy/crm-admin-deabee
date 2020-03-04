using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class KetoneReadingResource
    {
        [Key]
        public int Id { get; set; }

        public double reading { get; set; }

        public DateTime created { get; set; }

        [ForeignKey("UserResource")]
        public int? UserResourceId { get; set; }
        public virtual UserResource UserResource { get; set; }
    }
}
