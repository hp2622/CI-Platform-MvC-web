using CI_Platform_Entity.Models;

namespace CI_Platform.Models
{
    public class ViewModel
    {
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Mission> Missions { get; set; }
        public IEnumerable<MissionTheme> MissionThemes { get; set; }


    }
}
