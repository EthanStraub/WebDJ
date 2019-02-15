using System;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDjProject.Models
{
    public class User_Genres
    {
        [Key]
        public int userGenresJuncId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string genreName { get; set; }
    }
}