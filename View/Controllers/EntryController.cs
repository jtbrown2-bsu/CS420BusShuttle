using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class EntryController : Controller
{
    private readonly EntryRepository _entryRepository;

    public EntryController(EntryRepository entryRepository)
    {
        _entryRepository = entryRepository;
    }

    public async Task<IActionResult> Index()
    {
        var entries = await _entryRepository.Get();
        return View(entries);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Entry model)
    {
        if (ModelState.IsValid)
        {
            await _entryRepository.Add(model);
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _entryRepository.Get(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Entry model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _entryRepository.Update(model);
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
        var model = await _entryRepository.Get(id);

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
            await _entryRepository.Delete(id);
        }
        catch
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
