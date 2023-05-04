using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace View.Models
{
    public class DriverViewModel
    {
        public string Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}
