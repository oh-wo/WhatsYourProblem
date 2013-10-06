using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsYourProblemCSharp.Models;
using WhatsYourProblemCSharp.Helpers;

namespace WhatsYourProblemCSharp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (AuthenticationHelper.IsloggedIn)
            {
                return RedirectToAction("Browse");
            }
            else
            {
                return View();
            }
        }
        [AuthenticationHelper.IsUser]
        public ActionResult Browse()
        {
            PUser user = new PUser();
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                if(Db.PUsers.Any(u => u.ID == new Guid(User.Identity.Name)))
                {
                    user = Db.PUsers.First(u => u.ID == new Guid(User.Identity.Name));
                }
                else
                {

                }
            }
            return View(user);
        }

    }
}
