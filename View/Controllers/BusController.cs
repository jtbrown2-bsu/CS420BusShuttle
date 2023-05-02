using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class BusController : Controller
{
    private readonly BusRepository _busRepository;

    public BusController(BusRepository busRepository)
    {
        _busRepository = busRepository;
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
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _busRepository.Get(id);

        if (model == null)
        {
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
            } catch
            {
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
        }
        catch
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
