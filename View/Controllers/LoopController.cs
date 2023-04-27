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
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult GetAllLoops()
    {
        return View(_loopRepository.GetAllLoops());
    }

    [HttpGet]
    public IActionResult GetLoopById(int id)
    {
        var loop = _loopRepository.GetLoop(id);
        return View(loop);
    }

    [HttpPost]
    public IActionResult AddBus(Loop loop)
    {
        if (!ModelState.IsValid) return View(loop);
        
        _loopRepository.AddLoop(loop);
        return RedirectToAction(nameof(Index));

    }

    [HttpPut]
    public IActionResult EditLoop(int id, Loop loop)
    {
        if (!ModelState.IsValid) return View(loop);
        _loopRepository.UpdateLoop(id, loop);
        return RedirectToAction(nameof(System.Index));

    }

    [HttpDelete]
    public IActionResult DeleteLoop(int id)
    {
        _loopRepository.DeleteLoop(id);
        return RedirectToAction(nameof(Index));
    }
}