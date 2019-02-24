using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Stripe;
using WebDjProject.Models;

namespace WebDjProject.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public ProfileController()
        {
        }

        public ProfileController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Profile/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.PrivatizeSuccess ? "Your account has now been made private."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
            };

            model.ApplicationUserId = userId;
            return View(model);
        }

        //
        // GET: /Profile/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Profile/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Profile/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Profile/Privatize
        public ActionResult Privatize()
        {
            var id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Profile/Privatize
        [HttpPost, ActionName("Privatize")]
        [ValidateAntiForgeryToken]
        public ActionResult PrivatizeConfirmed()
        {
            var id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            user.PrivateStatus = true;
            db.SaveChanges();
            return RedirectToAction("Index", new { Message = ManageMessageId.PrivatizeSuccess });
        }

        [ActionName("StripePayment")]
        public ActionResult StripePayment()
        {
            StripeConfiguration.SetApiKey("sk_test_dFDizp08z3Cyn2klQBvAMvOc");

            //var token = model.Token; 

            var options = new ChargeCreateOptions
            {
                Amount = 999,
                Currency = "usd",
                SourceId = "tok_visa",
                ReceiptEmail = "ethan.straub@gmail.com",
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            var user = User.Identity;
            ApplicationDbContext db = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var userID = user.GetUserId();
            var loggedInUser = db.Users.Where(u => u.Id == userID).Single();

            var userName = loggedInUser.UserName;
            var userAddress = loggedInUser.Email;

            //check for user roles
            //if user is not in role, add him to it
            if (User.IsInRole("PremiumUser")==false)
            {
                UserManager.AddToRole(userID, "PremiumUser");
                SendMail(userAddress, userName);
                Thread.Sleep(2000);
                //FormsAuthentication.SignOut();
            }

            db.SaveChanges();
            return View(charge);
        }

        public void SendMail(string address, string name)
        {
            MailMessage mail = new MailMessage("webdjmessaging@gmail.com", address);
            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            mail.Subject = "WebDJ -- " + name + "'s account has been approved.";
            mail.Body = "Your account has now been approved for our premium services. For this change to take effect, please log out and then log back into your WebDJ account.";
            client.Send(mail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            PrivatizeSuccess,
            Error
        }
    }
}