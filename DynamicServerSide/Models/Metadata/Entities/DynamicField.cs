using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServerSide.Models.Metadata.Entities
{
    public class DynamicField
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public string Name { get; set; }

        public string ElementType { get; set; }

        public string InputType { get; set; }

        public int? SqlTypeEnumCode { get; set; }

        public bool IsNullable { get; set; } = true;

        public string DefaultValue { get; set; }

        public int? OrderIndex { get; set; }

        [ForeignKey("Template")]
        public int? TemplateId { get; set; }
        public virtual DynamicTemplate Template { get; set; }
    }
}
