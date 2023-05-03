using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using View.Models;

namespace View.Controllers;

public class LoopController : Controller
{
    private readonly ILoopRepository _loopRepository;

    public LoopController(ILoopRepository loopRepository)
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
        return View(new LoopViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoopViewModel model)
    {
        if (ModelState.IsValid)
        {
            var loop = new Loop
            {
                Name = model.Name
            };
            await _loopRepository.Add(loop);
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

        var viewModel = new LoopViewModel
        {
            Id = model.Id,
            Name = model.Name,
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LoopViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var loop = new Loop
                {
                    Id = model.Id,
                    Name = model.Name
                };
                await _loopRepository.Update(loop);
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
