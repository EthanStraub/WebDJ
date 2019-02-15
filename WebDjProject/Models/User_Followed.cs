using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDjProject.Models
{
    public class User_Followed
    {
        // Might rework later
        // This is supposed to be a junction table that stores two user Ids when one user 'follows' the other.

        [Key]
        public int userFollowedJuncId { get; set; }
        //[ForeignKey("ApplicationUser")]
        //public string ApplicationUserId { get; set; }
        //public ApplicationUser ApplicationUser { get; set; }
    }
}