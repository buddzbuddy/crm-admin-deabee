﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Models.Data
{
    public class UserTypeResource
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<UserResource> UserResources { get; set; }
    }
}
