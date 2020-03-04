using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class ProductPhotoResource
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ProductResource")]
        public int? ProductResourceId { get; set; }
        public virtual ProductResource ProductResource { get; set; }

        public string FileId { get; set; }
    }
}
