using CI_Platform.Models;
using CI_Platform_Entity.CIDbContext;
using CI_Platform_Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace CI_Platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CipContext _CipContext;

        public HomeController(CipContext CipContext)
        {
            _CipContext = CipContext;


        }


        public IActionResult Landingpage()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Resetpass()
        {
            return View();
        }
        public IActionResult ForgetPass()
        {
            return View();
        }
        public IActionResult Nomission()
        {
            return View();
        }
        public IActionResult SingleMission(int MissionId)
        {

            //List<VolunteeringMission> missionsVM = new List<VolunteeringMission>();
            List<Mission> missionlist = _CipContext.Missions.ToList();


            List<User> AllUsers = new List<User>();
            AllUsers = _CipContext.Users.ToList();
            ViewBag.AllUsers = AllUsers;

            //List<Comment> comments = _CipContext.Comments.ToList();
           var comments = from U in _CipContext.Users join CM in _CipContext.Comments on U.UserId equals CM.UserId where CM.MissionId == MissionId select new { U.FirstName, CM.CommetText, CM.CreatedAt };
            ViewBag.Comments = comments;

            var userid = HttpContext.Session.GetString("userid");

            var mission = _CipContext.Missions.FirstOrDefault(m => m.MissionId == MissionId);
            var favmission = _CipContext.FavoriteMissions.FirstOrDefault(FM => FM.MissionId == MissionId);
            missionlist = missionlist.Where(t => t.ThemeId == mission.ThemeId && t.MissionId != mission.MissionId).Take(3).ToList();
            var theme = _CipContext.MissionThemes.FirstOrDefault(t => t.MissionThemeId == mission.ThemeId);
            var goaltxt = _CipContext.GoalMissions.FirstOrDefault(g => g.MissionId == mission.MissionId);
            var city = _CipContext.Cities.FirstOrDefault(s => s.CityId == mission.CityId);
            var ratings = _CipContext.MissionRatings.FirstOrDefault(MR => MR.MissionId == MissionId && MR.UserId == long.Parse(userid));
            //var recvoldet = from U in CipContext.Users join MA in CipContext.MissionApplications on U.UserId equals MA.UserId where MA.MissionId == mission.MissionId select U;
            GoalMission goalMission = _CipContext.GoalMissions.Where(gm => gm.MissionId == mission.MissionId).FirstOrDefault();
            string[] startDateNtime = mission.StartDate.ToString().Split(' ');
            string[] endDateNtime = mission.EndDate.ToString().Split(' ');

            //List<Mission> missionlist1 = _CipContext.Missions.ToList();
            //missionlist1 = missionlist.Where(t => t.ThemeId == mission.ThemeId && t.MissionId != mission.MissionId).Take(3).ToList();
            //ViewBag.related = missionlist1;


            //recent volunteers
            var recvoldetails = from U in _CipContext.Users join MA in _CipContext.MissionApplications on U.UserId equals MA.UserId where MA.MissionId == mission.MissionId select U;
            ViewBag.recentvoldetails = recvoldetails;


            //Related Mission List
            List<VolunteeringMission> relatedMissionList = new List<VolunteeringMission>();
            var relatedmission = _CipContext.Missions.Where(m => m.ThemeId == mission.ThemeId && m.MissionId != mission.MissionId).ToList();
            foreach (var item in relatedmission.Take(3))
            {
                var relcity = _CipContext.Cities.FirstOrDefault(m => m.CityId == item.CityId);
                var reltheme = _CipContext.MissionThemes.FirstOrDefault(m => m.MissionThemeId == item.ThemeId);
                var relgoalobj = _CipContext.GoalMissions.FirstOrDefault(m => m.MissionId == item.MissionId);
                var Startdate = item.StartDate;
                var Enddate = item.EndDate;

                relatedMissionList.Add(new VolunteeringMission
                {
                    MissionId = item.MissionId,
                    City = relcity.Name,
                    Theme = reltheme.Title,
                    SingleTitle = item.Title,
                    Description = item.ShortDescription,
                    StartDate = Startdate,
                    EndDate = Enddate,

                    Organization = item.OrganizationName,

                    GoalText = relgoalobj != null ? relgoalobj.GoalObjectiveText : "null",
                    missionType = item.MissionType,


                });
            }

            ViewBag.relatedmission = relatedMissionList.Take(3);


            //Single Mission Page - Volunteering Mission Page

            var ratingTotal = _CipContext.MissionRatings.Where(MR => MR.MissionId == mission.MissionId).ToList();
            var prevRating = _CipContext.MissionRatings.FirstOrDefault(pr => pr.MissionId == mission.MissionId && pr.UserId == long.Parse(userid));

            var rat = 0;
            var sum = 0;
            foreach (var r in ratingTotal)
            {
                sum = (int)(sum + (r.Rating));
            }
            if (ratingTotal.Count() == 0)
            {
                rat = 0;
            }
            else
            {
                rat = sum / ratingTotal.Count();

            }
            if (prevRating != null)
            {
                ViewBag.avgrating = rat;
            }
            else
            {
                ViewBag.avgrating = 0;
            }
            var TotalRatedByUser = ratingTotal.Count();

            VolunteeringMission volunteeringMission = new();
            volunteeringMission = new()
            {
                SingleTitle = mission.Title,
                Description = mission.Description,
                OrganizationDetails = mission.OrganizationDetail,
                GoalText = goalMission != null ? goalMission.GoalObjectiveText : "null",
                StartDate = (DateTime)mission.StartDate,
                EndDate = (DateTime)mission.EndDate,
                StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                missionType = mission.MissionType,
                MissionId = mission.MissionId,
                City = city.Name,
                Theme = theme.Title,
                Organization = mission.OrganizationName,
                Rating = ratings != null ? ratings.Rating : 0,
                isFavrouite = favmission == null ? null : favmission.MissionId,
                UserId = favmission == null ? null : favmission.UserId.ToString(),
                AverageRating = rat,
                TotalRatedByUsers = TotalRatedByUser,
                Comments = _CipContext.Comments.Where(CM => CM.MissionId == MissionId).ToList() != null ? _CipContext.Comments.Where(CM => CM.MissionId == MissionId).ToList() : null,
                AllUsers = _CipContext.Users.ToList(),

            };



            return View(volunteeringMission);
        }


        //add ratings
        [HttpPost]
        public async Task<IActionResult> Addrating(int rating, long Id, long missionId)
        {
            MissionRating ratingExists = await _CipContext.MissionRatings.FirstOrDefaultAsync(fm => fm.UserId == Id && fm.MissionId == missionId);
            if (ratingExists != null)
            {
                ratingExists.Rating = rating;
                _CipContext.Update(ratingExists);
                _CipContext.SaveChanges();
                return Json(new { success = true, ratingExists, isRated = true });
            }
            else
            {
                var ratingele = new MissionRating();
                ratingele.Rating = rating;
                ratingele.UserId = Id;
                ratingele.MissionId = missionId;
                _CipContext.Add(ratingele);
                _CipContext.SaveChanges();
                return Json(new { success = true, ratingele, isRated = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToFav(long MissionId, long UserId)
        {
            FavoriteMission favoriteMission = await _CipContext.FavoriteMissions.FirstOrDefaultAsync(FM => FM.MissionId == MissionId && FM.UserId == UserId);

            if (favoriteMission != null)
            {
                FavoriteMission favoriteMissiondel = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == MissionId && FM.UserId == UserId).FirstOrDefault();
                _CipContext.Remove(favoriteMissiondel);
                _CipContext.SaveChanges();
                return Json(new { success = true, favoriteMissiondel, isRated = true });
            }
            else
            {
                
                var favoriteMissionadd = new FavoriteMission();
                favoriteMissionadd.UserId = UserId;
                favoriteMissionadd.MissionId = MissionId;
                _CipContext.Add(favoriteMissionadd);
                _CipContext.SaveChanges();
                return Json(new { success = true, favoriteMissionadd, isRated = true });
            }
        }

        public IActionResult SingleMission1(int MissionId)
        {


            var mission = _CipContext.Missions.FirstOrDefault(m => m.MissionId == MissionId);
            ViewBag.mission = mission;

            var favmission = _CipContext.FavoriteMissions.FirstOrDefault(FM => FM.MissionId == MissionId);
            ViewBag.Fav = favmission;

            List<Mission> missionlist = _CipContext.Missions.ToList();
            missionlist = missionlist.Where(t => t.ThemeId == mission.ThemeId && t.MissionId != mission.MissionId).Take(3).ToList();
            ViewBag.related = missionlist;


            var theme = _CipContext.MissionThemes.FirstOrDefault(t => t.MissionThemeId == mission.ThemeId);
            ViewBag.theme = theme;

            var goaltxt = _CipContext.GoalMissions.FirstOrDefault(g => g.MissionId == mission.MissionId);
            ViewBag.goaltxt = goaltxt;

            var city = _CipContext.Cities.FirstOrDefault(s => s.CityId == mission.CityId);
            ViewBag.city = city;

            //var recentvol = _CipContext.MissionApplications.Where(ma => ma.MissionId == mission.MissionId);
            //var recentvoldetails = from MA in _CipContext.MissionApplications join M in _CipContext.Missions on MA.MissionId equals M.MissionId join U in _CipContext.Users on MA.UserId equals U.UserId select U;
            //var recvoldet1 = from U in _CipContext.Users join MA in _CipContext.MissionApplications on U.UserId equals MA.UserId where MA.MissionId == mission.MissionId select U;
            //ViewBag.recentvoldethfghails = recvoldet1;

            //if (FavMissionId != 0)
            //{
            //    FavoriteMission favoriteMission = new FavoriteMission();
            //    favoriteMission.MissionId = FavMissionId;
            //    favoriteMission.UserId = UserId;
            //    FavoriteMission favM = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == FavMissionId).FirstOrDefault();
            //    if (favM == null)
            //    {
            //        _CipContext.FavoriteMissions.Add(favoriteMission);
            //        _CipContext.SaveChanges();
            //    }
            //    else
            //    {
            //        FavoriteMission favoriteMissiondel = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == FavMissionId).FirstOrDefault();
            //        _CipContext.FavoriteMissions.Remove(favoriteMissiondel);
            //        _CipContext.SaveChanges();
            //    }

            //    return RedirectToAction("SingleMission", "Home");
            //}

            return View();

        }

       
















            public IActionResult TestMission()
        {
            return View();
        }

        public IActionResult GetCities()
        {
            List<City> cities = _CipContext.Cities.ToList();
            return View(cities);
        }
        public IActionResult GetCountries()
        {
            List<Country> Countries = _CipContext.Countries.ToList();
            return View(Countries);
        }

        [HttpGet]
        public IActionResult StoryListing()
        {
            ViewModel mymodel = new ViewModel();
            mymodel.Cities = (IEnumerable<City>)GetCities();
            mymodel.Countries = (IEnumerable<Country>)GetCountries();
            return View(mymodel);

        }






        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPass(ForgetPass model)
        {
            if (ModelState.IsValid)
            {
                var user = _CipContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    ViewBag.emailnotexist = "Email not exist please register first";
                    return View();
                }
                else
                {

                    // Generate a password reset token for the user
                    var token = Guid.NewGuid().ToString();

                    // Store the token in the password resets table with the user's email
                    var passwordReset = new PasswordReset
                    {
                        Email = model.Email,
                        Token = token
                    };

                    _CipContext.PasswordResets.Add(passwordReset);
                    _CipContext.SaveChanges();

                    // Send an email with the password reset link to the user's email address
                    var resetLink = Url.Action("ResetPass", "Home", new { email = model.Email, token }, Request.Scheme);
                    // Send email to user with reset password link
                    // ...
                    var fromAddress = new MailAddress("evanzandu@gmail.com", "CI Platform");
                    var toAddress = new MailAddress(model.Email);
                    var subject = "Password reset request";
                    var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";
                    var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("evanzandu@gmail.com", "timrrquqhqzvdpns"),
                        EnableSsl = true
                    };
                    smtpClient.Send(message);

                    return RedirectToAction("ForgetPass", "Home");
                }
            }

            return View();
        }




        [HttpGet]
        [AllowAnonymous]
        public ActionResult Resetpass(string email, string token)
        {
            var passwordReset = _CipContext.PasswordResets.FirstOrDefault(pr => pr.Email == email && pr.Token == token);
            if (passwordReset == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Pass the email and token to the view for resetting the password
            var model = new ResetpassModel
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Resetpass(ResetpassModel rpm)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = _CipContext.Users.FirstOrDefault(u => u.Email == rpm.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgetPassword", "Home");
                }

                // Find the password reset record by email and token
                var passwordReset = _CipContext.PasswordResets.FirstOrDefault(pr => pr.Email == rpm.Email && pr.Token == rpm.Token);
                if (passwordReset == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Update the user's password
                user.Password = rpm.Password;
                //user.UpdatedAt = rpm.UpdatedAt;
                _CipContext.SaveChanges();


                return RedirectToAction("Login", "Login");





            }

            return View(rpm);
        }












        // GET: ForgetController


        // GET: ForgetController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ForgetController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ForgetController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ForgetController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ForgetController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ForgetController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ForgetController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}







