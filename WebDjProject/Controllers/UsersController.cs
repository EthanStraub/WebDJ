using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebDjProject.Models;

namespace WebDjProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;

                ViewBag.displayMenu = "No";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();

        }

        
        public ActionResult GetSongs()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var adminUserID = User.Identity.GetUserId();

            var premiumUsersList = db.Users.Where(u => u.Roles.Count >= 1 && u.Id != adminUserID).ToList();

            List<Playlist> premiumPlaylistsList = new List<Playlist>();

            foreach (var premiumUser in premiumUsersList)
            {
                var checkedPlaylist = db.Playlists.Where(p => p.ApplicationUserId == premiumUser.Id).First();
                
                premiumPlaylistsList.Add(checkedPlaylist);
            }

            return View(premiumPlaylistsList);
        }

        [HttpPost]
        public ActionResult SendEmails(List<int> PlaylistIDs, List<string> SongsLists)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            for (int i = 0; i < PlaylistIDs.Count; i++)
            {
                var currentPlaylistID = PlaylistIDs[i];
                Playlist currentPlaylist = db.Playlists.Where(p => p.playlistId == currentPlaylistID).Single();

                var currentUser = db.Users.Where(u => u.Id == currentPlaylist.ApplicationUserId).Single();

                var userEmail = currentUser.Email;
                var userName = currentUser.UserName;
                List<string> userSongList = new List<string>(SongsLists[i].Split(",".ToCharArray()));


                SendPremiumMail(userEmail, userName, userSongList, currentPlaylist.playlistName);
            }

            Thread.Sleep(2000);
            return RedirectToAction("Index", "Home");
        }
        public void SendPremiumMail(string address, string name, List<string> Songlist, string playlistName)
        {
            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("webdjmessaging@gmail.com", "Carbuncle#9");

            MailMessage mm = new MailMessage(address, address, "WebDJ -- Premium Account Recommendations", "Hello " + name + "! Based on the playlist '" + playlistName + "' you made using our website, here are some songs we think you might like. \n https://open.spotify.com/track/" + Songlist[0] + " \n https://open.spotify.com/track/" + Songlist[1] + " \n https://open.spotify.com/track/" + Songlist[2] + " \n https://open.spotify.com/track/" + Songlist[3] + " \n https://open.spotify.com/track/" + Songlist[4]);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
        }

        public bool isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}