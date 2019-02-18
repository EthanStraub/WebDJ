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
    public class Playlist
    {
        [Key]
        public int playlistId { get; set; }
        [ForeignKey("ApplicationUser")]
        //playlist.ApplicationUserId = User.Identity.GetUserId();
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Display(Name = "Playlist name")]
        public string playlistName { get; set; }
        [Display(Name = "Number of songs in playlist")]
        public int songCount { get; set; }
        [Display(Name = "Spotify playlist ID")]
        public string spotifyPlaylistID { get; set; }
    }
}