using CI_Platform_Entity.Models;
namespace CI_Platform.Models
{
    public class VolunteeringMission
    {
        public long? MissionId { get; set; }

        public string? SingleTitle { get; set; }
        public string? Description { get; set; }

        public string? Organization { get; set; }
        public string? OrganizationDetails { get; set; }

        public long? Rating { get; set; } = null;
        public long? AverageRating { get; set; } = null;
        public long? TotalRatedByUsers { get; set; } = null;
        public string? ImgUrl { get; set; }

        public string? Theme { get; set; }

        public string? missionType { get; set; }
        public long? isFavrouite { get; set; } = null;

        public bool? userApplied { get; set; }

        public string? City { get; set; }

        public string? StartDateEndDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? NoOfSeatsLeft { get; set; }

        public string? Deadline { get; set; }

        public DateTime? createdAt { get; set; }
        public string? UserId { get; set; }
        public string? GoalText { get; set; }
        public long? addedtofav { get; set; }

        public string? UserName { get; set; }
        public List<Comment> Comments { get; set; }
        public List<User> AllUsers { get; set; }


    }
}