using System.ComponentModel.DataAnnotations;

namespace RaceDayWebAPI.ViewModels
{
    public class EventViewModel
    {
        public int EventID { get; set; }
        public int OrganiserID { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public DateTime Date { get; set; }
        [Required] public string Location { get; set; } = string.Empty;
        [Required] public decimal Distance { get; set; }
        [Required] public string EventType { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
    }
}
