using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web.Mvc;
using WhatsYourProblemCSharp.Models;
using WhatsYourProblemCSharp.Helpers;


namespace WhatsYourProblemCSharp
{
    [HubName("chatHub")]
    public class LetsChat : Hub
    {
        public void send(Guid chatid)
        {
            using (PhotonFactoryEntities db = new PhotonFactoryEntities())
            {
                ChatComment chat = db.ChatComments.Include("PUser").FirstOrDefault(c => c.ID == chatid);
                Clients.Group(chat.ProblemID.ToString()).addMessage(chat.PUserID, chat.PUser.NameFirst, chat.Content, String.Format("{0},{1}",chat.PostedDate.Value.AddHours(13).ToShortDateString(),chat.PostedDate.Value.AddHours(12).ToShortTimeString()));
            }
            
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }


    }

    [HubName("problemHub")]
    public class ProblemTime : Hub
    {
        public void addProblem(Guid id)
        {
            Problem prob = new Problem();
            using (PhotonFactoryEntities db = new PhotonFactoryEntities())
            {
                prob = db.Problems.Include("PUser").FirstOrDefault(p => p.ID == id);
            }
            if (prob != null)
            {
                Clients.All.addProblem(id, prob.Title, prob.PostedDate.Value.AddHours(13).ToShortDateString(), prob.PUser.NameFirst, prob.PUserID);
            }
        }

        public void startEditing(Guid userid, Guid problemid)
        {
            Clients.All.someoneHasStartedEditing(userid, problemid);
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    Problem prob = db.Problems.FirstOrDefault(p => p.ID == problemid);
                    prob.EditedBy = userid;
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                Clients.All.someoneHasStoppedEditing(userid, problemid);
            }
        }

        public void stopEditing(Guid userid, Guid problemid)
        {
            Clients.All.someoneHasStoppedEditing(userid, problemid);
            try
            {
                using (PhotonFactoryEntities db = new PhotonFactoryEntities())
                {
                    Problem prob = db.Problems.FirstOrDefault(p => p.ID == problemid);
                    prob.EditedBy = null;
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                Clients.All.someoneHasStoppedEditing(userid, problemid);
            }
        }

        public void aProblemHasBeenDeleted(Guid problemid)
        {
            Clients.All.removeProblemFromView(problemid);
        }

<<<<<<< HEAD
<<<<<<< HEAD
        public void aRelationshipHasBeenRemoved(Guid parentid, Guid childid)
        {
            Clients.All.removeRelationship(parentid, childid);
        }
        public void aRelationshipHasBeenCreated(Guid originalid, Guid newid,string relationship)
        {
            Problem problem = new Problem();
            using (PhotonFactoryEntities db = new PhotonFactoryEntities())
            {
                problem = db.Problems.FirstOrDefault(p => p.ID == newid);
            }
            Clients.All.addRelationship(originalid, newid, relationship, problem.Title);
        }
=======
>>>>>>> 17c5e93a2a76b87b85764032e52f423e32740d09
=======
>>>>>>> 17c5e93a2a76b87b85764032e52f423e32740d09
        
    }
}