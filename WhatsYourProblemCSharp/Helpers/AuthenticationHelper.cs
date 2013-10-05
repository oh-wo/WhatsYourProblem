using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WhatsYourProblemCSharp.Models;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Mail;

namespace WhatsYourProblemCSharp.Helpers
{
    public class AuthenticationHelper //:BaseModel
    {

        /* Specifically related to logging in/out */
        private static readonly string GuestUser = "GuestUser";

        public static bool LogInUser(PUser user)//, int timeout
        {
            bool success = false;
            try
            {
                AuthenticationHelper authHelper = new AuthenticationHelper();
                //var userName = (user == null ? GuestUser : user.ID.ToString());
                var ticket = new FormsAuthenticationTicket(1, user.ID.ToString(), DateTime.Now, DateTime.Now.AddYears(1), true, "", FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                success = true;
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        public static PUser GetUser(string email)
        {
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                return Db.PUsers.FirstOrDefault(u => u.Email == email);
            }
        }

        public static PUser GetUser(Guid id)
        {
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                return Db.PUsers.First(u => u.ID == id);
            }
        }
        public static PUser GetUserByStringID(string IdString)
        {
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                return Db.PUsers.First(u => u.ID == new Guid(IdString));
            }
        }


        public static PUser GetCurrentUser()
        {
            AuthenticationHelper authHelper = new AuthenticationHelper();
            return GetUserByStringID(CurrentSessionUserName);
        }




        private static string CurrentSessionUserName
        {
            get
            {
                using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
                {
                    if (!HttpContext.Current.User.Identity.IsAuthenticated)
                        return null;
                    return HttpContext.Current.User.Identity.Name;
                };
            }
        }

        public static bool IsUnknownUser
        {
            get { return CurrentSessionUserName == null; }
        }

        public static bool IsGuestUser
        {
            get { return CurrentSessionUserName == GuestUser; }
        }

        public static bool IsloggedIn
        {
            get
            {
                AuthenticationHelper authHelper = new AuthenticationHelper();
                return CurrentSessionUserName != null && CurrentSessionUserName != GuestUser && CurrentSessionUserName.ToString() != new Guid().ToString();
            }
        }


        //Action filter to check that the MUser is logged in (prohibit use of user section and business section)
        public class IsUserAttribute : AuthorizeAttribute
        {
            public IsUserAttribute()
            {
            }

            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                return (IsloggedIn);
            }
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                if (IsloggedIn)
                {
                    filterContext.Result = new RedirectResult("/Account/NotAllowedUser");
                }
                else
                {
                    base.HandleUnauthorizedRequest(filterContext);
                };
            }
        }

        /*General account functions */
        public static bool UpdateFailedPasswordAttemptCount(Guid id)
        {
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                bool success = false;
                try
                {
                    PUser user = Db.PUsers.First(u => u.ID == id);
                    user.FailedPasswordCount++;
                    Db.SaveChanges();
                    success = true;
                }
                catch (Exception ex)
                {
                }
                return success;
            }
        }

        private static readonly Int32 MinRequiredPasswordLength = 6;

        public static PUser ValidateUser(string email, string password)
        {
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(email))
                {

                    PUser user = GetUser(email.Trim()) ?? null;

                    if (user != null)
                    {

                        /*if (user.FailedPasswordAttemptCount == null)
                        {
                            user.FailedPasswordAttemptCount = 1;
                            Db.SaveChanges();
                        }
                        else
                        {
                            UpdateFailedPasswordAttemptCount(user.ID);
                            Db.SaveChanges();
                        }*/
                    }
                    else
                    {

                        return null;
                    }

                    string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(password.Trim(), "SHA1");
                    return Db.PUsers.FirstOrDefault(u => u.Email == email && u.Password == hash) ?? null;
                }
                return null;
            }
        }

        public static Guid? CreateUser(string email, string password, string lastName, string firstName)
        {
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                bool success = false;

                //dummy variable - only created so can be set to null so null can be returned if this fails 
                Guid? userID = new Guid();
                userID = null;

                string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(password.Trim(), "SHA1");

                Random rand = new Random();
                PUser user = new PUser()
                {
                    ID = Guid.NewGuid(),
                    Email = email,
                    NameFirst = firstName,
                    NameLast = lastName,
                    EmailCode = Guid.NewGuid(),
                    EmailVerified = false,
                    Password = hash,
                    Created = DateTime.UtcNow,
                    LastActivityDate = DateTime.UtcNow,
                };

                try
                {
                    Db.PUsers.Add(user);
                    Db.SaveChanges();
                    success = true;
                }
                catch (Exception ex)
                {
                }
                return (success == true) ? user.ID : userID;
            }
        }

        public static bool ChangePassword(string email, string oldPassword, string newPassword)
        {
            using (PhotonFactoryEntities Db = new PhotonFactoryEntities())
            {
                if ((ValidateUser(email, oldPassword) == null) || string.IsNullOrEmpty(newPassword.Trim()))
                    return false;
                PUser user = GetUser(email);
                string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword.Trim(), "SHA1");

                user.Password = hash;
                Db.SaveChanges();
                return true;
            }
        }

        public static bool SendVerificationEmail(PUser user)
        {
            bool success = false;
            try
            {

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 465);
                smtp.UseDefaultCredentials = false;
                var credentials = new System.Net.NetworkCredential("hookedup@hookedup.co.nz", "M0nk3yS4nctu4ry");
                smtp.Credentials = credentials;
                smtp.EnableSsl = true;
                smtp.Port = 587;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("hookedup@hookedup.co.nz");
                mail.To.Add(user.Email);
                mail.Subject = "Confirm your email address";
                mail.Body = "Click this link to confirm your address: http://theproblem.org/Account/VerifyEmail/?emailCode=" + user.EmailCode;

                smtp.Send(mail);
                success = true;
            }
            catch (Exception ex)
            {
            }
            return success;
        }
    }
}