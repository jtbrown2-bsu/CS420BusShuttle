using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using View.Models;

namespace View.Controllers;

public class DriverController : Controller
{
    private readonly IDriverRepository _driverRepository;

    public DriverController(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
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
            }
            catch
            {
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
        }
        catch
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
