using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models {
    public class SavedPost {
        public int SavedPostId { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
