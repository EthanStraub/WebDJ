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
    public class Playlist_Song
    {
        [Key]
        public int playlistSongJuncId { get; set; }
        [ForeignKey("playlistId")]
        public string playlistId { get; set; }
        [ForeignKey("songId")]
        public string songId { get; set; }
    }
}