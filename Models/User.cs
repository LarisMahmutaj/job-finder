using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models {
    public class User {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required, DataType(DataType.Password)]
        [MinLength(6)]
        [MaxLength(18)]
        public string Password { get; set; }
        
        public DateTime DateRegistered { get; set; }
    }
}
