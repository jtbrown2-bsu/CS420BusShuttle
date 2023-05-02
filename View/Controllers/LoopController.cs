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

    public async Task<IActionResult> Index()
    {
        var loops = await _loopRepository.Get();
        return View(loops);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Loop model)
    {
        if (ModelState.IsValid)
        {
            await _loopRepository.Add(model);
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _loopRepository.Get(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Loop model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _loopRepository.Update(model);
            }
            catch
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _loopRepository.Get(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _loopRepository.Delete(id);
        }
        catch
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
