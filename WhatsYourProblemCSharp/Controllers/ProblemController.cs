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
            Guid? id = null;
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
                    id = prob.ID;
                }

            }
            catch (Exception ex)
            {

            }
            return Json(id);
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

        [HttpPost]
        public JsonResult GetContent(Guid problemID)
        {
            string content = "failure";
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    Problem prob = db.Problems.FirstOrDefault(p => p.ID == problemID);
                    if (prob != null)
                    {
                        content = prob.Content;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(content);
        }

        [HttpPost]
        public JsonResult SaveContent(Guid problemID, string content)
        {
            bool success = false;
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    Problem prob = db.Problems.FirstOrDefault(p => p.ID == problemID);
                    if (prob != null)
                    {
                        prob.Content = content;
                        db.SaveChanges();
                        success = true;
                    }
                    
                }
            }
            catch (Exception ex)
            {

            }
            return Json(success);
        }

        [HttpPost]
        public JsonResult DeleteProblem(Guid problemID)
        {
            bool success = false;
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    Problem prob = db.Problems.FirstOrDefault(p => p.ID == problemID);
                    List<ChatComment> chatComments = db.ChatComments.Where(c => c.ProblemID == problemID).ToList();
                    if (prob != null)
                    {
                        foreach (ChatComment chat in chatComments)
                        {
                            db.ChatComments.Remove(chat);
                        }
                        db.Problems.Remove(prob);
                        db.SaveChanges();
                        success = true;
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return Json(success);
        }

    }
}
