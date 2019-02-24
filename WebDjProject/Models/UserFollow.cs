using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace WebDjProject.Models
{
    public class UserFollow
    {
        [Key]
        public int UserFollowPairID { get; set; }
        [Required]
        public string followedID { get; set; }
        [Required]
        public string followedUserName { get; set; }
        [Required]
        public string followerID { get; set; }
        [Required]
        public string followerUserName { get; set; }
    }
}