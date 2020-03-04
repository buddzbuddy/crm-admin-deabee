using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class TextMessageResource
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserResource")]
        public int? UserResourceId { get; set; }
        public virtual UserResource UserResource { get; set; }

        [ForeignKey("DoctorUserResource")]
        public int? DoctorUserResourceId { get; set; }
        public virtual DoctorUserResource DoctorUserResource { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }
    }
}
