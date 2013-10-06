using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsYourProblemCSharp.Models;
using WhatsYourProblemCSharp.Helpers;

namespace WhatsYourProblemCSharp.Controllers
{
    public class ProblemController : Controller
    {
        //
        // GET: /Problem/

        public ActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(string title)
        {
            bool success = false;
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    Problem prob = new Problem()
                    {
                        ID = Guid.NewGuid(),
                        Title = title,
                        PostedDate = DateTime.UtcNow,
                        PUserID = new Guid(User.Identity.Name),
                        Rating = 0,
                    };
                    db.Problems.Add(prob);
                    db.SaveChanges();
                    success = true;
                }

            }
            catch (Exception ex)
            {

            }
            return Json(success);
        }

        [AuthenticationHelper.IsUser]
        public ActionResult ProblemContainer()
        {
            List<Problem> problems = new List<Problem>();
            using (PhotonFactoryEntities db = new PhotonFactoryEntities())
            {
                problems = db.Problems.Include("PUser").Take(10).ToList();
            }
            return View(problems);
        }

    }
}
