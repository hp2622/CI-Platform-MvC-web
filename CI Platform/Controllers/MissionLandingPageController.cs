using CI_Platform.Models;
using CI_Platform_Entity.CIDbContext;
using CI_Platform_Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Reflection;

namespace CI_Platform.Controllers
{
    public class MissionLandingPageController : Controller
    {

        private readonly CipContext _CipContext;

        private List<Mission> Missions = new List<Mission>();

        private List<User> AllUsers = new List<User>();

        private List<GoalMission> GoalMissions = new List<GoalMission>();

        private List<Mission> FinalMissionsList = new List<Mission>();

        private List<Country> Countries = new List<Country>();

        private List<City> Cities = new List<City>();

        private List<MissionTheme> Themes = new List<MissionTheme>();

        private List<Country> countryElements = new List<Country>();

        private List<FavoriteMission> favoriteMissions = new List<FavoriteMission>();
        public MissionLandingPageController(CipContext CipContext)
        {
            _CipContext = CipContext;


        }

        public IActionResult LandingDummy()
        {
            AllUsers = _CipContext.Users.ToList();
            ViewBag.AllUsers = AllUsers;

            Missions = _CipContext.Missions.ToList();
            Countries = _CipContext.Countries.ToList();
            Cities = _CipContext.Cities.ToList();
            Themes = _CipContext.MissionThemes.ToList();
            ViewBag.countries = Countries;
            ViewBag.themes = Themes;
            ViewBag.cities = Cities;



            return View(Missions);
        }

        [HttpPost]
        public IActionResult LandingDummy(string[]? country, string[]? city, string[]? theme, string? searchTerm, string? sortValue, int pg = 1)
        {

            //UserId for logged in user
            //long useridfavmission = (long)HttpContext.Session.GetInt32("userIDforfavmission");

            var userid = HttpContext.Session.GetString("userid");

            if (userid != null)
            {
                AllUsers = _CipContext.Users.ToList();
                ViewBag.AllUsers = AllUsers;

                Missions = _CipContext.Missions.ToList();
                Countries = _CipContext.Countries.ToList();
                Cities = _CipContext.Cities.ToList();
                Themes = _CipContext.MissionThemes.ToList();
                ViewBag.countries = Countries;
                ViewBag.themes = Themes;
                ViewBag.cities = Cities;




                List<Mission> miss = _CipContext.Missions.ToList();


                var useridforrating = HttpContext.Session.GetInt32("userIDforfavmission");
                var FavMissionUserIdMatching = _CipContext.FavoriteMissions.Where(FM => FM.UserId == long.Parse(userid)).ToList();
                var MissionRatingTotal = _CipContext.MissionRatings.ToList();


                MissionLandingPageViewModel model = new MissionLandingPageViewModel
                {
                    Missions = _CipContext.Missions.ToList(),
                    Country = _CipContext.Countries.ToList(),
                    City = _CipContext.Cities.ToList(),
                    MissionThemes = _CipContext.MissionThemes.ToList(),
                    MissionSkills = _CipContext.MissionSkills.ToList(),
                    GoalMission = _CipContext.GoalMissions.ToList(),
                    FavMissionData = FavMissionUserIdMatching,
                    MissionRatings = MissionRatingTotal,


                };

                if (country.Count() > 0 || city.Count() > 0 || theme.Count() > 0)
                {
                    miss = GetFilteredMission(miss, country, city, theme);
                }



                if (searchTerm != null)
                {
                    miss = miss.Where(m => m.Title.ToLower().Contains(searchTerm)).ToList();

                }

                miss = GetSortedMissions(miss, sortValue);




                //pagination

                const int pageSize = 3;
                if (pg < 1)
                {
                    pg = 1;
                }

                int missionCount = miss.Count();

                var PaginationModel = new PaginationModel(missionCount, pg, pageSize);

                int missionSkip = (pg - 1) * pageSize;

                var FinalMissions = miss.Skip(missionSkip).Take(PaginationModel.PageSize).ToList();

                ViewBag.Pagination = PaginationModel;


                int totalCount = miss.Count();
                ViewBag.TotalCount = totalCount;
                model.Missions = FinalMissions;
                return PartialView("_Cards", model);

            }
            else
            {
                AllUsers = _CipContext.Users.ToList();
                ViewBag.AllUsers = AllUsers;

                Missions = _CipContext.Missions.ToList();
                Countries = _CipContext.Countries.ToList();
                Cities = _CipContext.Cities.ToList();
                Themes = _CipContext.MissionThemes.ToList();
                ViewBag.countries = Countries;
                ViewBag.themes = Themes;
                ViewBag.cities = Cities;




                List<Mission> miss = _CipContext.Missions.ToList();


                var useridforrating = HttpContext.Session.GetInt32("userIDforfavmission");
                //var FavMissionUserIdMatching = _CipContext.FavoriteMissions.Where(FM => FM.UserId == useridforrating).ToList();
                var MissionRatingTotal = _CipContext.MissionRatings.ToList();


                MissionLandingPageViewModel model = new MissionLandingPageViewModel
                {
                    Missions = _CipContext.Missions.ToList(),
                    Country = _CipContext.Countries.ToList(),
                    City = _CipContext.Cities.ToList(),
                    MissionThemes = _CipContext.MissionThemes.ToList(),
                    MissionSkills = _CipContext.MissionSkills.ToList(),
                    GoalMission = _CipContext.GoalMissions.ToList(),
                    //FavMissionData = FavMissionUserIdMatching,
                    MissionRatings = MissionRatingTotal,


                };

                if (country.Count() > 0 || city.Count() > 0 || theme.Count() > 0)
                {
                    miss = GetFilteredMission(miss, country, city, theme);
                }



                if (searchTerm != null)
                {
                    miss = miss.Where(m => m.Title.ToLower().Contains(searchTerm)).ToList();

                }

                miss = GetSortedMissions(miss, sortValue);




                //pagination

                const int pageSize = 3;
                if (pg < 1)
                {
                    pg = 1;
                }

                int missionCount = miss.Count();

                var PaginationModel = new PaginationModel(missionCount, pg, pageSize);

                int missionSkip = (pg - 1) * pageSize;

                var FinalMissions = miss.Skip(missionSkip).Take(PaginationModel.PageSize).ToList();

                ViewBag.Pagination = PaginationModel;


                int totalCount = miss.Count();
                ViewBag.TotalCount = totalCount;
                model.Missions = FinalMissions;
                return PartialView("_Cards", model);
            }


            //return Json(true);

        }

        public List<Mission> GetSortedMissions(List<Mission> miss, string sortValue)
        {
            switch (sortValue)
            {
                case "Newest":
                    return miss.OrderBy(m => m.StartDate).ToList();
                case "Oldest":
                    return miss.OrderByDescending(m => m.StartDate).ToList();
                case "lowest":
                    return miss.OrderBy(m => m.Availability).ToList();
                case "highest":
                    return miss.OrderByDescending(m => m.Availability).ToList();
                default:
                    return miss.ToList();

            }
        }

        public List<Mission> GetFilteredMission(List<Mission> miss, string[] country, string[] city, string[] theme)
        {
            if (country.Length > 0)
            {
                miss = miss.Where(m => country.Contains(m.Country.Name)).ToList();
            }

            if (city.Length > 0)
            {
                miss = miss.Where(m => city.Contains(m.City.Name)).ToList();
            }

            if (theme.Length > 0)
            {
                miss = miss.Where(m => theme.Contains(m.Theme.Title)).ToList();
            }

            return miss;
        }
    }
}