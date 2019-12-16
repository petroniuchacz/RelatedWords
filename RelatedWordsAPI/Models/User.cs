using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.AspNetCore.Identity;

namespace RelatedWordsAPI.Models
{
    public class User //: IdentityUser
    {
        public int UserId { get; set; }
        public ICollection<Project> Projects { get; }

    }
}
