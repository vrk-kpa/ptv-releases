using System;
using System.ComponentModel.DataAnnotations;

namespace PTV.Database.Model.Models
{
    internal partial class RestrictedType
    {
        public Guid Id { get; set; }

        [Required]
        public string TypeName { get; set; }

        [Required]
        public Guid Value { get; set; }
    }
}