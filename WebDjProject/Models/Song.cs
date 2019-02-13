using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDjProject.Models
{
    public class Song
    {
        [Key]
        public int songId { get; set; }
        [Display(Name = "Song name")]
        public string songName { get; set; }
        [Display(Name = "Artist name")]
        public string artistName { get; set; }
        [Display(Name = "Genre")]
        public int songCount { get; set; }
        [Display(Name = "Spotify song ID")]
        public string spotifyPlaylistID { get; set; }
    }
}