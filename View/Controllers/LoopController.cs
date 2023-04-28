using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class LoopController : Controller
{
    private readonly LoopRepository _loopRepository;

    public LoopController(LoopRepository loopRepository)
    {
        _loopRepository = loopRepository;
    }
    public IActionResult LoopCreate()
    {
        ViewData["Loops"] = _loopRepository.GetAllLoops();
        return View();
    }
    
    [HttpPost]
    public IActionResult LoopCreate(Loop model)
    {
        if (ModelState.IsValid)
        {
            _loopRepository.AddLoop(model);
            ViewData["Loops"] = _loopRepository.GetAllLoops();
            return View();
        }

        ViewData["Loops"] = _loopRepository.GetAllLoops();
        return View(model);
    }
    
    public IActionResult EditLoop(int id)
    {
        var model = _loopRepository.GetLoop(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }
    
    [HttpPost]
    public IActionResult EditLoop(int id, Loop loop)
    {
        if (ModelState.IsValid)
        {
            _loopRepository.UpdateLoop(id, loop);
            ViewData["Loops"] = _loopRepository.GetAllLoops();
            return View("LoopCreate");
        }

        ViewData["Loops"] = _loopRepository.GetAllLoops();
        return View(loop);
    }
    
    [HttpPost]
    public IActionResult DeleteLoop(int id)
    {
        _loopRepository.DeleteLoop(id);
        ViewData["Loops"] = _loopRepository.GetAllLoops();
        return View("LoopCreate");
    }
}