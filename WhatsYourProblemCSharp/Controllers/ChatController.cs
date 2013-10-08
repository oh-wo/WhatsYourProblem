using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatsYourProblemCSharp.Models;
using WhatsYourProblemCSharp.Helpers;

namespace WhatsYourProblemCSharp.Controllers
{
    public class ChatController : Controller
    {
        //
        // GET: /Problem/

        public ActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(Guid problemid, string content)
        {
            Guid? id = null;
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    ChatComment chat = new ChatComment()
                    {
                        ID=Guid.NewGuid(),
                        Content = content,
                        PostedDate = DateTime.UtcNow,
                        PUserID=new Guid(User.Identity.Name),
                        ProblemID = problemid,
                    };
                    db.ChatComments.Add(chat);
                    db.SaveChanges();
                    id = chat.ID;
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
        public ActionResult GetChat(Guid problemID)
        {
            List<ChatComment> chats = new List<ChatComment>();
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    chats = db.ChatComments.Include("PUser").Where(c => c.ProblemID == problemID).OrderBy(p => p.PostedDate).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return View(chats);
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
                    if (prob != null)
                    {
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
