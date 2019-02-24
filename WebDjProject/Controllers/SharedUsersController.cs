using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebDjProject.Models;

namespace WebDjProject.Controllers
{
    public class SharedUsersController : Controller
    {
        // GET: SharedUsers
        public ActionResult Index()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (User.IsInRole("Admin"))
            {
                var users = db.Users;
                return View(users.ToList());
            }
            else
            {
                string userId = User.Identity.GetUserId();
                var usersList = db.Users.Where(u => u.PrivateStatus == false && u.Id != userId && u.UserName != "ethan" ).ToList();
                return View(usersList);
            }


        }

        public ActionResult PlaylistsIndex(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (User.IsInRole("Admin"))
            {
                var playlists = db.Playlists;
                return View(playlists.ToList());
            }
            else
            {
                string userId = User.Identity.GetUserId();
                var user = db.Users.Find(id);
                List<Playlist> playlistsList = new List<Playlist>();
                
                var addedPlaylists = db.Playlists.Where(p => p.ApplicationUserId == user.Id).ToList();
                foreach (var playlist in addedPlaylists)
                {
                    playlistsList.Add(playlist);
                }

                return View(playlistsList);
            }
        }



    }
}