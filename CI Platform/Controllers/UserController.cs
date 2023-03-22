
using CI_Platform.Controllers;
using CI_Platform_Entity.CIDbContext;
using CI_Platform_Entity.Models;
using CI_Platform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace CI.Controllers
{
    public class UserController : Controller
    {

        private readonly CipContext _CipContext;

        public UserController(CipContext CipContext)
        {
            _CipContext = CipContext;


        }
        public IActionResult Index()
        {
            List<User> Users = _CipContext.Users.ToList();
            return View(Users);
        }



        public IActionResult Register()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            var obj = _CipContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (obj == null)
            {

                _CipContext.Users.Add(user);
                _CipContext.SaveChanges();
                return RedirectToAction("Login", "Login");
                //return RedirectToAction("Register", "Home");  
            }
            else
            {
                ViewBag.RegError = "Email already exist";

            }
            return View();
        }


        [HttpPost]
        public IActionResult RecomandUser(string EmailId, int MissionId)
        {
            // Generate a password reset token for the user
            //var token = Guid.NewGuid().ToString();

            // Store the token in the password resets table with the user's email
            //var passwordReset = new PasswordReset
            //{
            //    Email = model.Email,
            //    Token = token
            //};

            //_CipContext.PasswordResets.Add(passwordReset);
            //_CipContext.SaveChanges();

            // Send an email with the password reset link to the user's email address
            var resetLink = Url.Action("SingleMission", "Home", new { MissionId = MissionId }, Request.Scheme);
            // Send email to user with reset password link
            // ...
            var fromAddress = new MailAddress("officehl1882@gmail.com", "CI Platform");
            var toAddress = new MailAddress(EmailId);
            var subject = "Someone Recomand you to join Mission";
            var body = $"Hi,<br /><br />Please click on the following link to see mission details:<br /><br /><a href='{resetLink}'>{resetLink}</a>";
            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("officehl1882@gmail.com", "yedkuuhuklkqfzwx"),
                EnableSsl = true
            };
            smtpClient.Send(message);


            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult AddComment(int MissionId, long UserId, string CommentText)
        {
            //Comment commentExist = _CipContext.Comments.FirstOrDefault(CM => CM.UserId == UserId && CM.MissionId == MissionId);

            //if (commentExist != null)
            //{
            //    commentExist.CommentText = CommentText;
            //    _CipContext.Update(commentExist);
            //    _CipContext.SaveChanges();
            //    return Json(new { success = true, commentExist });
            //}
            //else
            //{
            var newComment = new Comment();
            newComment.MissionId = MissionId;
            newComment.UserId = UserId;
            newComment.CommetText = CommentText;
            _CipContext.Add(newComment);
            _CipContext.SaveChanges();


            VolunteeringMission model = new()
            {

                Comments = _CipContext.Comments.Where(CM => CM.MissionId == MissionId).ToList(),
                AllUsers = _CipContext.Users.ToList(),




            };





            return PartialView("_Comments", model);
            //}
        }












        //public IActionResult Edit(int? id)
        //{
        //    Users user = _testingContext.Users.Where(x => x.Id == id).FirstOrDefault();
        //    return View(user);
        //}

        //[HttpPost]
        //public IActionResult Edit(Users user)
        //{
        //    _testingContext.Users.Update(user);
        //    _testingContext.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //public IActionResult Delete(int? id)
        //{
        //    Users user = _testingContext.Users.Where(x => x.Id == id).FirstOrDefault();
        //    return View(user);
        //}

        //[HttpPost]
        //public IActionResult Delete(Users user)
        //{
        //    _testingContext.Users.Remove(user);
        //    _testingContext.SaveChanges();
        //    return RedirectToAction("index");
        //}








    }
}


