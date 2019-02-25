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
    public class PlaylistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Playlists
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var playlists = db.Playlists;
                return View(playlists.ToList());
            }
            else
            {
                string userId = User.Identity.GetUserId();
                var playlists = db.Playlists.Where(p => p.ApplicationUserId == userId);
                return View(playlists.ToList());
            }
            
        }

        // GET: Playlists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = db.Playlists.Find(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            return View(playlist);
        }

        // GET: Playlists/Create
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();
            var playlists = db.Playlists.Where(p => p.ApplicationUserId == userId);
            var playlistSequence = playlists.ToList();
            if (User.IsInRole("PremiumUser") == false && playlistSequence.Count >= 3)
            {
                return RedirectToAction("Index", "Playlists");
            }
            else if(User.IsInRole("PremiumUser") == true && playlistSequence.Count >= 5)
            {
                return RedirectToAction("Index", "Playlists");
            }
            else
            {
                ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email");
                return View();
            }
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "playlistId,ApplicationUserId,playlistName,songCount,spotifyPlaylistID")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                ViewBag.playlistName = playlist.playlistName;
                
                db.Playlists.Add(playlist);
                
                playlist.ApplicationUserId = User.Identity.GetUserId();
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", playlist.ApplicationUserId);
            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = db.Playlists.Find(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", playlist.ApplicationUserId);
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "playlistId,ApplicationUserId,playlistName,songCount,spotifyPlaylistID")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playlist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", playlist.ApplicationUserId);
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = db.Playlists.Find(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Playlist playlist = db.Playlists.Find(id);
            db.Playlists.Remove(playlist);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Search()
        {
            string userId = User.Identity.GetUserId();
            var playlists = db.Playlists.Where(p => p.ApplicationUserId == userId);
            return View(playlists.ToList());
        }

        public ActionResult Remove()
        {
            if (User.IsInRole("Admin"))
            {
                var playlists = db.Playlists;
                return View(playlists.ToList());
            }
            else
            {
                string userId = User.Identity.GetUserId();
                var playlists = db.Playlists.Where(p => p.ApplicationUserId == userId);
                return View(playlists.ToList());
            }
            
        }

        
        [HttpPost, ActionName("UpdateSongCount")]
        public ActionResult UpdateSongCount(List<int> songCountSet, List<int> playlistIdSet)
        {
            if (playlistIdSet != null)
            {
                for (int k = 0; k < playlistIdSet.Count; k++)
                {
                    var currentId = playlistIdSet[k];

                    Playlist oldPlaylist = db.Playlists.Where(p => p.playlistId == currentId).Single();

                    oldPlaylist.songCount = songCountSet[k];
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
