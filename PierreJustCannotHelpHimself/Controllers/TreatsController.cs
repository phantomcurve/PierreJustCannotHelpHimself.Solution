using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PierreJustCannotHelpHimself.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;


namespace PierreJustCannotHelpHimself.Controllers
{
  public class TreatsController : Controller
  {
    private readonly PierreJustCannotHelpHimselfContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TreatsController(UserManager<ApplicationUser> userManager, PierreJustCannotHelpHimselfContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    
    // [Authorize(Roles = "Employee, Customer")]
    public ActionResult Index()
    {
      // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      // var currentUser = await _userManager.FindByIdAsync(userId);
      IEnumerable<Treat> sortedTreats = _db.Treats.OrderBy(treat => treat.Name);
      return View(sortedTreats.ToList());
    }

    // [Authorize(Roles = "Employee")]
    [Authorize]
    public ActionResult Create()
    {
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View();
    }

    [HttpPost]

    // [Authorize(Roles = "Employee")]
    [Authorize]
    public async Task<ActionResult> Create(Treat treat, int FlavorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      _db.Treats.Add(treat);
      _db.SaveChanges();
      if (FlavorId != 0)
      {
        _db.TreatFlavor.Add(new TreatFlavor() { FlavorId = FlavorId, TreatId = treat.TreatId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    
    // [Authorize(Roles = "Employee, Customer")]
    public ActionResult Details(int id)
    {
      // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      // var currentUser = await _userManager.FindByIdAsync(userId);
      var thisTreat = _db.Treats
        .Include(treat => treat.JoinEntities)
        .ThenInclude(join => join.Flavor)
        .FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    // [Authorize(Roles = "Employee")]
    [Authorize]
    public async Task<ActionResult> Edit(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View(thisTreat);
    }

    [HttpPost]
    
    // [Authorize(Roles = "Employee")]
    [Authorize]
    public ActionResult Edit(Treat treat, int FlavorId)
    {
      if (FlavorId != 0)
      {
        _db.TreatFlavor.Add(new TreatFlavor() { FlavorId = FlavorId, TreatId = treat.TreatId });
      }
      _db.Entry(treat).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", "Treats", new { id = treat.TreatId });
    }
    
    // [Authorize(Roles = "Employee")]
    [Authorize]
    public ActionResult AddFlavor(int id)
    {
        var thisTreat = _db.Treats.FirstOrDefault(treats => treats.TreatId == id);
        ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
        return View(thisTreat);
    }

    [HttpPost]
    [Authorize]
    
    // [Authorize(Roles = "Employee")]
    public ActionResult AddFlavor(Treat treat, int FlavorId)
    {
      if (FlavorId != 0)
      {
        if (_db.TreatFlavor.Any(join => join.FlavorId == FlavorId && join.TreatId == treat.TreatId) == false)
          _db.TreatFlavor.Add(new TreatFlavor() { FlavorId = FlavorId, TreatId = treat.TreatId });
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = treat.TreatId });
    }
    
    // [Authorize(Roles = "Employee")]
    [Authorize]
   public async Task<ActionResult> Delete(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    [HttpPost, ActionName("Delete")]
    
    // [Authorize(Roles = "Employee")]
    [Authorize]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      _db.Treats.Remove(thisTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    [HttpPost]
    
    // [Authorize(Roles = "Employee")]
    [Authorize]
    public ActionResult DeleteFlavor(int joinId)
    {
      var joinEntry = _db.TreatFlavor.FirstOrDefault(entry => entry.TreatFlavorId == joinId);
      _db.TreatFlavor.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = joinEntry.TreatId });
    }

  }
}