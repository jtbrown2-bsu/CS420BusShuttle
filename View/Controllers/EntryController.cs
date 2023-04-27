using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Core.Repositories;

namespace View.Controllers;

public class EntryController : Controller
{
    private readonly EntryRepository _entryRepository;

    public EntryController(EntryRepository entryRepository)
    {
        _entryRepository = entryRepository;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View(_entryRepository.Get());
    }

    [HttpGet]
    public IActionResult GetByID(int id)
    {
        var entry = _entryRepository.Get(id);
        return View(entry);
    }

    [HttpPost]
    public IActionResult AddEntry([Bind("Id, TimeStamp, Boarded, LeftBehind")]Entry entry)
    {
        if (!ModelState.IsValid) return View(entry);
        
        _entryRepository.Add(entry);
        return RedirectToAction(nameof(Index));

    }

    [HttpPut]
    public IActionResult EditEntry(int id, Entry entry)
    {
        if (!ModelState.IsValid) return View(entry);
        _entryRepository.Update(id, entry);
        return RedirectToAction(nameof(System.Index));

    }

    [HttpDelete]
    public IActionResult DeleteEntry(int id)
    {
        _entryRepository.DeleteComment(id);
        return RedirectToAction(nameof(Index));
    }


}