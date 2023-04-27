using Microsoft.AspNetCore.Mvc;
using View.Models;

namespace View.Controllers
{
    public class BusController : Controller
    {
        private static List<BusModel> _buses = new List<BusModel>();

        public IActionResult BusCreate()
        {
            ViewData["Buses"] = _buses;
            return View();
        }

        [HttpPost]
        public IActionResult BusCreate(BusModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = _buses.Count + 1;
                _buses.Add(model);
                ViewData["Buses"] = _buses;
                return View();
            }

            ViewData["Buses"] = _buses;
            return View(model);
        }

        public IActionResult EditBus(int id)
        {
            var model = _buses.FirstOrDefault(b => b.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteBus(int id)
        {
            var bus = _buses.FirstOrDefault(b => b.Id == id);

            if (bus == null)
            {
                return NotFound();
            }

            _buses.Remove(bus);

            ViewData["Buses"] = _buses;
            return View("BusCreate");
        }
    }
}
