using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDjProject.Models;

namespace WebDjProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            StripeConfiguration.SetApiKey("sk_test_dFDizp08z3Cyn2klQBvAMvOc");

            var options = new ChargeCreateOptions
            {
                Amount = 999,
                Currency = "usd",
                SourceId = "tok_visa",
                ReceiptEmail = "ethan.straub@gmail.com",
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public FileContentResult UserPhotos()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();

                if (userId == null || User.IsInRole("Admin"))
                {
                    string fileName = HttpContext.Server.MapPath(@"~/Images/noImg.png");

                    byte[] imageData = null;
                    FileInfo fileInfo = new FileInfo(fileName);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    imageData = br.ReadBytes((int)imageFileLength);

                    return File(imageData, "image/png");

                }
                // to get the user details to load user Image
                var bdUsers = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var userImage = bdUsers.Users.Where(x => x.Id == userId).FirstOrDefault();

                return new FileContentResult(userImage.UserPhoto, "image/jpeg");
            }
            else
            {
                string fileName = HttpContext.Server.MapPath(@"~/Images/noImg.png");

                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");

            }
        }

    }
}