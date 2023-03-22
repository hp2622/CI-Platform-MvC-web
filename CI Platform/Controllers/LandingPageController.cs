using CI_Platform.Models;
using CI_Platform_Entity.CIDbContext;
using CI_Platform_Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text.Json;
using System.Web;

namespace CI_Platform.Controllers
{
    public class LandingPageController : Controller
    {

        private readonly CipContext _CipContext;

        private List<MissionViewModel> missionsVMList = new List<MissionViewModel>();

        private List<Mission> Missions = new List<Mission>();

        private List<User> AllUsers = new List<User>();
        
        private List<GoalMission> GoalMissions = new List<GoalMission>();

        private List<Mission> FinalMissionsList = new List<Mission>();

        private List<Country> Countries = new List<Country>();

        private List<City> Cities = new List<City>();

        private List<MissionTheme> Themes = new List<MissionTheme>();

        private List<Country> countryElements = new List<Country>();

        private List<City> cityElements = new List<City>();

        private List<MissionTheme> themeElements = new List<MissionTheme>();

        private List<FavoriteMission> favoriteMissions = new List<FavoriteMission>();
        public LandingPageController(CipContext CipContext)
        {
            _CipContext = CipContext;


        }
        
        public IActionResult Landingpage(long id, int? pageIndex, string searchQuery, long[] FCountries, long[] FCities, long[] FThemes, string sortOrder, long FavMissionId, long UserId)
        {
            //Get all users

            AllUsers = _CipContext.Users.ToList();
            ViewBag.AllUsers = AllUsers;
            // Check if user is logged in
            int? userId = HttpContext.Session.GetInt32("userID");
            long useridfavmission = (long)HttpContext.Session.GetInt32("userIDforfavmission");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _CipContext.Users.Where(e => e.UserId == id).SingleOrDefault();
            ViewBag.user = user;
            ViewBag.Request = Request;
            Missions = _CipContext.Missions.ToList();
            Countries = _CipContext.Countries.ToList();
            Themes = _CipContext.MissionThemes.ToList();
            ViewBag.countries = Countries;
            ViewBag.themes = Themes;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                Missions = _CipContext.Missions.ToList();
                Missions = Missions.Where(m => m.Title.ToLower().Contains(searchQuery.ToLower())).ToList();
                FinalMissionsList.AddRange(Missions);
                ViewBag.searchQuery = Request.Query["searchQuery"];
            }

            if (FCountries != null && FCountries.Length > 0)
            {
                foreach (var country in FCountries)
                {
                    Missions = _CipContext.Missions.ToList();

                    Missions = Missions.Where(m => m.CountryId == country).ToList();
                    FinalMissionsList.AddRange(Missions);
                    var countryElement = _CipContext.Countries.Where(m => m.CountryId == country).ToList();
                    countryElements.AddRange(countryElement);
                    var cities = _CipContext.Cities.Where(m => m.CountryId == country).ToList();
                    Cities.AddRange(cities);
                }
                ViewBag.countryElements = countryElements;
                ViewBag.cities = Cities;
                ViewBag.FCountries = Request.Query["FCountries"];
            }

            if (FCities != null && FCities.Length > 0)
            {

                var tempFinalList = new List<Mission>();
                foreach (var city in FCities)
                {
                    if (FinalMissionsList.Count > 0)
                    {
                        Missions = FinalMissionsList;
                    }
                    else
                    {
                        Missions = _CipContext.Missions.ToList();
                    }

                    Missions = Missions.Where(m => m.CityId == city).ToList();
                    var cityElement = _CipContext.Cities.Where(m => m.CityId == city).ToList();
                    cityElements.AddRange(cityElement);
                    tempFinalList.AddRange(Missions);

                }
                FinalMissionsList = tempFinalList;
                ViewBag.cityElements = cityElements;
                ViewBag.FCities = Request.Query["FCities"];
            }

            if (FThemes != null && FThemes.Length > 0)
            {
                var tempFinalList = new List<Mission>();
                foreach (var theme in FThemes)
                {
                    if (FinalMissionsList.Count > 0)
                    {
                        Missions = FinalMissionsList;
                    }
                    else
                    {
                        Missions = _CipContext.Missions.ToList();
                    }
                    Missions = Missions.Where(m => m.ThemeId == theme).ToList();
                    var themeElement = _CipContext.MissionThemes.Where(m => m.MissionThemeId == theme).ToList();
                    themeElements.AddRange(themeElement);
                    tempFinalList.AddRange(Missions);
                }
                FinalMissionsList = tempFinalList;
                ViewBag.FThemes = Request.Query["FThemes"];
                ViewBag.themeElements = themeElements;
            }


            //print goaltext for goal based missions
            var GoalMissions = _CipContext.GoalMissions.ToList();
            ViewBag.MA = GoalMissions;

            

            //Pagination
            int pageSize = 6; // change this to your desired page size
            int skip = (pageIndex ?? 0) * pageSize;
            if (FinalMissionsList.Count() != 0)
            {   
                var missions = FinalMissionsList.Skip(skip).Take(pageSize).ToList();
                foreach (var mission in missions)
                {
                    City city = _CipContext.Cities.Where(e => e.CityId == mission.CityId).FirstOrDefault();
                    MissionTheme theme = _CipContext.MissionThemes.Where(e => e.MissionThemeId == mission.ThemeId).FirstOrDefault();
                    GoalMission goalMission = _CipContext.GoalMissions.Where(gm => gm.MissionId == mission.MissionId).FirstOrDefault();
                    FavoriteMission favoriteMissions = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == mission.MissionId).FirstOrDefault();


                    string[] startDateNtime = mission.StartDate.ToString().Split(' ');
                    string[] endDateNtime = mission.EndDate.ToString().Split(' ');
                    var ratings = _CipContext.MissionRatings.Where(e => e.MissionId == mission.MissionId).ToList();
                    var rating = 0;
                    var sum = 0;
                    //foreach (var entry in ratings)
                    //{

                    //    sum = sum + int.Parse(entry.Rating);

                    //}
                    //rating = sum / ratings.Count;
                    if (goalMission != null && favoriteMissions != null)
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            GoalText = goalMission.GoalObjectiveText,
                            UserId = useridfavmission,
                            addedtofavM = favoriteMissions.MissionId,
                            addedtofavU = (long)favoriteMissions.UserId
                        }); 
                    }
                    else if (goalMission != null )
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            //isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            GoalText = goalMission.GoalObjectiveText,
                            UserId = useridfavmission
                            
                        });
                    }
                    else if (favoriteMissions != null)
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            //isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            UserId = useridfavmission,
                           
                            addedtofavM = favoriteMissions.MissionId,
                            addedtofavU = (long)favoriteMissions.UserId
                        });
                    }
                    else
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            //isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            UserId = useridfavmission


                        });
                    }
                    switch (sortOrder)
                    {
                        case "newest":
                            missionsVMList = missionsVMList.OrderByDescending(e => e.StartDate).ToList();
                            break;
                        case "oldest":
                            missionsVMList = missionsVMList.OrderBy(e => e.StartDate).ToList();
                            break;
                        case "lowest":
                            missionsVMList = missionsVMList.OrderBy(e => e.NoOfSeatsLeft).ToList();
                            break;
                        case "highest":
                            missionsVMList = missionsVMList.OrderByDescending(e => e.NoOfSeatsLeft).ToList();
                            break;
                        case "favourites":
                            //missionsVMList = missionsVMList.Where(e => e.isFavrouite != false).ToList();
                            break;
                        case "deadline":
                            missionsVMList = missionsVMList.OrderBy(e => e.Deadline).ToList();
                            break;
                        default:
                            missionsVMList = missionsVMList;
                            break;
                    }
                }

                if (missions.Count() == 0)
                {
                    return RedirectToAction("Nomission", "Home");
                }

                int totalMissions = FinalMissionsList.Count();
                ViewBag.TotalPages = (int)Math.Ceiling(totalMissions / (double)pageSize);
                ViewBag.CurrentPage = pageIndex ?? 0;

                ViewBag.NoOfMissions = FinalMissionsList.Count();
                ViewBag.missions = missionsVMList;
            }
            else
            {
                var missions = Missions.Skip(skip).Take(pageSize).ToList();
                foreach (var mission in missions)
                {
                    City city = _CipContext.Cities.Where(e => e.CityId == mission.CityId).FirstOrDefault();
                    MissionTheme theme = _CipContext.MissionThemes.Where(e => e.MissionThemeId == mission.ThemeId).FirstOrDefault();
                    GoalMission goalMission = _CipContext.GoalMissions.Where(gm => gm.MissionId == mission.MissionId).FirstOrDefault();
                    FavoriteMission favoriteMissions = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == mission.MissionId).FirstOrDefault();

                    string[] startDateNtime = mission.StartDate.ToString().Split(' ');
                    string[] endDateNtime = mission.EndDate.ToString().Split(' ');
                    var ratings = _CipContext.MissionRatings.Where(e => e.MissionId == mission.MissionId).ToList();
                    var rating = 0;
                    var sum = 0;
                    //foreach (var entry in ratings)
                    //{
                    //    sum = sum + int.Parse(entry.Rating);

                    //}
                    //rating = sum / ratings.Count;

                    if (goalMission != null && favoriteMissions != null)
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            //isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            GoalText = goalMission.GoalObjectiveText,
                            UserId = useridfavmission,
                            addedtofavM = favoriteMissions.MissionId,
                            addedtofavU = (long)favoriteMissions.UserId
                        });
                    }
                    else if (goalMission != null)
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            //isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            GoalText = goalMission.GoalObjectiveText,
                            UserId = useridfavmission
                            
                        });
                    }
                    else if(favoriteMissions != null)
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            //isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            UserId = useridfavmission,
                            addedtofavM = favoriteMissions.MissionId,
                            addedtofavU = (long)favoriteMissions.UserId

                        });
                    }
                    else
                    {
                        missionsVMList.Add(new MissionViewModel
                        {
                            MissionId = mission.MissionId,
                            Title = mission.Title,
                            Description = mission.ShortDescription,
                            City = city.Name,
                            Organization = mission.OrganizationName,
                            Theme = theme.Title,
                            //Rating = rating,
                            StartDate = (DateTime)mission.StartDate,
                            EndDate = (DateTime)mission.EndDate,
                            missionType = mission.MissionType,
                            //isFavrouite = (user != null) ? _CipContext.FavoriteMissions.Any(e => e.MissionId == mission.MissionId && e.UserId == id) : false,
                            //userApplied = (user != null) ? _CipContext.MissionApplications.Any(e => e.MissionId == mission.MissionId && e.UserId == id && e.ApprovalStatus == '1') : false,
                            ImgUrl = "~/images/Grow-Trees-On-the-path-to-environment-sustainability-3.png",
                            StartDateEndDate = "From " + startDateNtime[0] + " until " + endDateNtime[0],
                            NoOfSeatsLeft = 10,
                            Deadline = endDateNtime[0],
                            createdAt = (DateTime)mission.CreatedAt,
                            UserId = useridfavmission
                            

                        });
                    }

                    

                }
                switch (sortOrder)
                {
                    case "newest":
                        missionsVMList = missionsVMList.OrderByDescending(e => e.StartDate).ToList();
                        break;
                    case "oldest":
                        missionsVMList = missionsVMList.OrderBy(e => e.StartDate).ToList();
                        break;
                    case "lowest":
                        missionsVMList = missionsVMList.OrderBy(e => e.NoOfSeatsLeft).ToList();
                        break;
                    case "highest":
                        missionsVMList = missionsVMList.OrderByDescending(e => e.NoOfSeatsLeft).ToList();
                        break;
                    case "favourites":
                       // missionsVMList = missionsVMList.Where(e => e.isFavrouite != false).ToList();
                        break;
                    case "deadline":
                        missionsVMList = missionsVMList.OrderBy(e => e.Deadline).ToList();
                        break;
                    default:
                        missionsVMList = missionsVMList;
                        break;
                }

                if (missions.Count() == 0)
                {
                    return RedirectToAction("Nomission", "Home");
                }

                int totalMissions = Missions.Count();
                ViewBag.TotalPages = (int)Math.Ceiling(totalMissions / (double)pageSize);
                ViewBag.CurrentPage = pageIndex ?? 0;

                ViewBag.NoOfMissions = Missions.Count();
                ViewBag.missions = missionsVMList;
            }

            var MissionApp = _CipContext.MissionApplications.ToList();

            // Get the current URL
            UriBuilder uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host);
            if (Request.Host.Port.HasValue)
            {
                uriBuilder.Port = Request.Host.Port.Value;
            }
            uriBuilder.Path = Request.Path;

            // Remove the query parameter you want to exclude
            var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            query.Remove("pageIndex");
            uriBuilder.Query = query.ToString();



            ViewBag.currentUrl = uriBuilder.ToString();



            FavoriteMission favoriteMissions1 = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == FavMissionId).FirstOrDefault();

            //crud for add and remove fav mission
            if (FavMissionId != 0)
            {
                FavoriteMission favoriteMission = new FavoriteMission();
                favoriteMission.MissionId = FavMissionId;
                favoriteMission.UserId = UserId;
                FavoriteMission favM = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == FavMissionId && FM.UserId == useridfavmission).FirstOrDefault();
                if (favM == null)
                {
                    _CipContext.FavoriteMissions.Add(favoriteMission);
                    _CipContext.SaveChanges();
                }
                else
                {
                    FavoriteMission favoriteMissiondel = _CipContext.FavoriteMissions.Where(FM => FM.MissionId == FavMissionId && FM.UserId == useridfavmission).FirstOrDefault();
                    _CipContext.FavoriteMissions.Remove(favoriteMissiondel);
                    _CipContext.SaveChanges();
                }

                return RedirectToAction("Landingpage", "LandingPage");
            }
            return View();
        }

        


    }
}
