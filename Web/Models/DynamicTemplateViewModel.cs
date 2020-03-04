using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Web.OData.Models.Meta;

namespace Web.Models
{
    public class DynamicTemplateViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }

        public DynamicTemplate Template { get; set; }
        public List<DynamicFieldViewModel> DynamicFieldViewModels { get; set; }
    }
}