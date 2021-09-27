using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiToMongo.Models
{
    public class RegistrationViewModel
    {
        [Required]
        [MaxLength(100)]
        public String FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public String LastName { get; set; }
        [Required]
        [MaxLength(350)]
        public String Email { get; set; }
        [Required]
        [MaxLength(100)]
        public String LoginId { get; set; }
        [Required]
        [MaxLength(50)]
        public String Password { get; set; }
        [Required]
        [MaxLength(50)]
        public String ConfirmPassword { get; set; }
        [Required]
        [MaxLength(15)]
        public String ContactNumber { get; set; }
    }
}
