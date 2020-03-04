using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DynamicFieldViewModel
    {
        public int TemplateId { get; set; }
        public ColumnDescription ColumnDescription { get; set; }
    }
}