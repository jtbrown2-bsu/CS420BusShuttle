using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using View.Models;

namespace View.Controllers;
[Authorize(Policy = "ManagerOnly")]
public class DriverController : Controller
{
    private readonly IDriverRepository _driverRepository;
    private readonly ILogger<BusController> _logger;

    public DriverController(IDriverRepository driverRepository, ILogger<BusController> logger)
    {
        _driverRepository = driverRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var drivers = await _driverRepository.Get();
        return View(drivers);
    }

    public IActionResult Create()
    {
        return View();
    }

    public async Task<IActionResult> Edit(string id)
    {
        var model = await _driverRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Driver to update not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        var viewModel = new DriverViewModel
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DriverViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var driver = new Driver
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                await _driverRepository.Update(driver);
                _logger.LogInformation("Updated driver with ID {id} at {time}.", model.Id, DateTime.Now);
            }
            catch
            {
                _logger.LogError("Updating driver with ID {id} failed at {time}.", model.Id, DateTime.Now);
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(string id)
    {
        var model = await _driverRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Driver to delete not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeletePost(string id)
    {
        try
        {
            await _driverRepository.Delete(id);
            _logger.LogInformation("Deleted driver with ID {id} at {time}.", id, DateTime.Now);
        }
        catch
        {
            _logger.LogError("Deleting driver with ID {id} failed at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
