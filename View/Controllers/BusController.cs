using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;
[Authorize(Policy = "ManagerOnly")]
public class BusController : Controller
{
    private readonly IBusRepository _busRepository;
    private readonly ILogger<BusController> _logger;

    public BusController(IBusRepository busRepository, ILogger<BusController> logger)
    {
        _busRepository = busRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var buses = await _busRepository.Get();
        return View(buses);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Bus model)
    {
        if (ModelState.IsValid)
        {
            await _busRepository.Add(model);
            _logger.LogInformation("Created new bus with ID {id}, Bus Number {number} at {time}.", model.Id, model.BusNumber, DateTime.Now);
            return RedirectToAction("Index");
        }
        _logger.LogError("Failed bus creation validation at {time}.", DateTime.Now);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _busRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Bus to update not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Bus model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _busRepository.Update(model);
                _logger.LogInformation("Updated bus with ID {id} at {time}.", model.Id, DateTime.Now);
            } catch
            {
                _logger.LogError("Updating bus with ID {id} failed at {time}.", model.Id, DateTime.Now);
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _busRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Bus to delete not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _busRepository.Delete(id);
            _logger.LogInformation("Deleted bus with ID {id} at {time}.", id, DateTime.Now);
        }
        catch
        {
            _logger.LogError("Deleting bus with ID {id} failed at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
