using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
//using Microsoft.AspNetCore.Identity;

namespace RelatedWordsAPI.Models
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static List<string> Roles()
        {
            return new List<string> { $"{User}", $"{Admin}" };
        }
    }

    public class User 
    {
        public int UserId { get; set; }
        [Required]
        public string Email { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        [NotMapped]
        public string Token { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public ICollection<Project> Projects { get; }

        public User() { }
        public User(string email, string password, string role)
        {
            Email = email;
            Password = password;
            Role = role;
        }

        public static User GenerateWithoutSensitive( User user)
        {
            User newUser = new User(user.Email, null, user.Role);
            newUser.Token = user.Token; newUser.UserId = user.UserId;
            return newUser;
        }


    }

}
