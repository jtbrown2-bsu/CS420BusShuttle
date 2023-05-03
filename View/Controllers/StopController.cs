using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;
[Authorize(Policy = "ManagerOnly")]
public class StopController : Controller
{
    private readonly IStopRepository _stopRepository;
    private readonly ILogger<BusController> _logger;

    public StopController(IStopRepository stopRepository, ILogger<BusController> logger)
    {
        _stopRepository = stopRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var stops = await _stopRepository.Get();
        return View(stops);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Stop model)
    {
        if (ModelState.IsValid)
        {
            await _stopRepository.Add(model);
            _logger.LogInformation("Created new stop with ID {id}, Name {name} at {time}.", model.Id, model.Name, DateTime.Now);
            return RedirectToAction("Index");
        }
        _logger.LogError("Failed stop creation validation at {time}.", DateTime.Now);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _stopRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Stop to update not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Stop model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _stopRepository.Update(model);
                _logger.LogInformation("Updated stop with ID {id} at {time}.", model.Id, DateTime.Now);
            }
            catch
            {
                _logger.LogError("Updating stop with ID {id} failed at {time}.", model.Id, DateTime.Now);
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _stopRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Stop to delete not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            _logger.LogInformation("Deleted stop with ID {id} at {time}.", id, DateTime.Now);
            await _stopRepository.Delete(id);
        }
        catch
        {
            _logger.LogError("Deleting stop with ID {id} failed at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
