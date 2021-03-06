﻿using Microsoft.AspNetCore.Identity;
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
        [MaxLength(5000)]
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
