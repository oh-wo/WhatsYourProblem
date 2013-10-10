using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
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

        [HttpPost]
        public JsonResult CreateRelated(string title, Guid relatedid, string relationship)
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
                    Problem relatedProblem = db.Problems.FirstOrDefault(p => p.ID == relatedid);
                    if (relatedProblem != null)
                    {
                        switch (relationship)
                        {
                            case "subproblem":
                                prob.Problems.Add(relatedProblem);
                                break;
                            case "parent":
                                relatedProblem.Problems.Add(prob);
                                break;
                        }
                    }
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
                problems = db.Problems.Include("PUser").Take(10).OrderByDescending(p=>p.PostedDate).ToList();
            }
            return View(problems);
        }

        [HttpPost]
        public JsonResult GetContent(Guid problemID)
        {
            List<GotContent> subProblems = new List<GotContent>();
            List<GotContent> parentProblems = new List<GotContent>();
            Problem prob = new Problem();
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    prob = db.Problems.AsNoTracking().Include("Problems").AsNoTracking().FirstOrDefault(p => p.ID == problemID);
                    List<Problem> ParentProblems = db.Problems.Where(p => p.Problems.Any(sp => sp.ID == problemID)).ToList();
                    foreach (Problem problem in prob.Problems)
                    {
                        GotContent gC = new GotContent(){
                            Title = problem.Title,
                            ID = problem.ID,
                        };
                        subProblems.Add(gC);

                    }
                    foreach (Problem problem in ParentProblems)
                    {
                        GotContent gC = new GotContent(){
                            Title = problem.Title,
                            ID = problem.ID,
                        };
                        parentProblems.Add(gC);

                    }
                    
                }
            }
            catch (Exception ex)
            {

            }

            return Json(new { Content = prob.Content, SubProblems = subProblems, ParentProblems = parentProblems});
        }
        public class GotContent
        {
            public string Title { get; set; }
            public Guid ID { get; set; }
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
                        foreach (ChatComment chat in chatComments.ToList())
                        {
                            db.ChatComments.Remove(chat);
                        }
                        foreach (Problem subproblem in prob.Problems.ToList())
                        {
                            prob.Problems.Remove(subproblem);
                        }
                        foreach (Problem parentProblem in db.Problems.Where(p => p.Problems.Any(sp => sp.ID == problemID)).ToList())
                        {
                            parentProblem.Problems.Remove(prob);
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

        [HttpPost]
        public JsonResult WhosOnline()
        {
            List<PUser> onlineUsers = new List<PUser>();
            List<string> onlineNames = new List<string>();
            using (PhotonFactoryEntities db = new PhotonFactoryEntities())
            {
                try
                {
                    onlineUsers = db.PUsers.Where(u => (u.Online ?? false)).ToList();
                    foreach (PUser user in onlineUsers)
                    {
                        onlineNames.Add(user.NameFirst);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Json(onlineNames);
        }

        public ActionResult OnlineUsers()
        {
            List<PUser> onlineUsers = new List<PUser>();
            List<string> onlineNames = new List<string>();
            using (PhotonFactoryEntities db = new PhotonFactoryEntities())
            {
                try
                {
                    onlineUsers = db.PUsers.Where(u => (u.Online ?? false)).ToList();
                    foreach (PUser user in onlineUsers)
                    {
                        onlineNames.Add(user.NameFirst);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return View(onlineNames);
        }
    }
}
