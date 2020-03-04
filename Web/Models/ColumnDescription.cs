using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ColumnDescription
    {
        [Required]
        public string Label { get; set; }
        [Required]
        public string Name { get; set; }

        [Display(Name = "SqlDataType")]
        public SqlDataType? Type { get; set; }
        [Required]
        [Display(Name = "ElementType")]
        public ElementType EType { get; set; }

        [Display(Name = "InputType")]
        public InputType? IType { get; set; }

        [Display(Name = "Is Nullable?")]
        public bool IsNull { get; set; } = true;

        public string DefaultValue { get; set; }

        public int? OrderIndex { get; set; }

        public enum SqlDataType
        {
            Integer = 10,
            Boolean = 3,
            Datetime = 6,
            Decimal = 7,
            Text = 15
        }
        public enum ElementType
        {
            input,
            radiobutton,
            select,
            checkbox,
            date,
            button,
            image
        }
        public enum InputType
        {
            text,
            number,
            email,
            password
        }
    }
}