using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace View.Models
{
    public class RouteViewModel
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int SelectedStopId { get; set; }
        public List<SelectListItem> AvailableStops { get; set; } = new List<SelectListItem>();
    }
}
