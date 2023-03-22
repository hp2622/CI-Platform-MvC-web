using CI_Platform.Models;
using CI_Platform_Entity.Models;

namespace CI_Platform.Models
{
    public class MissionLandingPageViewModel
    {
        public int id { get; set; }
        public List<City> City { get; set; }
        public List<Country> Country { get; set; }
        public List<MissionTheme> MissionThemes { get; set; }
        public List<MissionSkill> MissionSkills { get; set; }
        public List<MissionRating> MissionRatings { get; set; }

        public List<Mission> RelatedMissions { get; set; }
        public List<Comment> Comments { get; set; }
        public string GoalText { get; set; }
        public List<FavoriteMission> FavMissionData { get; set; }
        public List<Mission> Missions { get; set; }
        public List<GoalMission> GoalMission { get; set; }
        public List<User> Users { get; set; }

        public bool IsFavorite { get; set; }

    }
}