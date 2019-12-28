using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models {
    public class Post {
        public int Id { get; set; }
        [Required]
        public PostType PostType { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
