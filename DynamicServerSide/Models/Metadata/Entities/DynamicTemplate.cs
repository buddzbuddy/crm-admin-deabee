﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServerSide.Models.Metadata.Entities
{
    public class DynamicTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int IsExist { get; set; }
        public virtual ICollection<DynamicField> DynamicFields { get; set; }
    }
}
