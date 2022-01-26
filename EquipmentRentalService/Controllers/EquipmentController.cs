using EquipmentRentalService.Database;
using EquipmentRentalService.Models;
using EquipmentRentalService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalService.Controllers
{
    public class RentDataModel
    {
        public int EquipmentId { get; set; }
        public DateTimeOffset RentFrom { get; set; }
        public DateTimeOffset RentTo { get; set; }
    }

    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRentalService _rentalService;
        private readonly UserManager<IdentityUser> _userManager;
        public EquipmentController(ApplicationDbContext dbContext,
                                   IRentalService rentalService,
                                   UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _rentalService = rentalService;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rent(RentDataModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var equipment = await _dbContext.RentalEquipment.FindAsync(model.EquipmentId);
            if (equipment == null || !equipment.IsAvailable) return NotFound();

            equipment.IsAvailable = false;

            var historyEntry = new RentalHistory
            {
                RentingUser = user,
                RentalEquipment = equipment,
                RentedDate = model.RentFrom,
                RentedDue = model.RentTo
            };

            equipment.RentalHistory.Add(historyEntry);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Return(int equipmentId)
        {
            var equipment = await _dbContext.RentalEquipment.FindAsync(equipmentId);
            if (equipment == null || equipment.IsAvailable) return NotFound();

            equipment.IsAvailable = true;
            equipment.RentalHistory.Last().ReturnedDate = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]

        public IActionResult List()
        {
            return View(_rentalService.GetAll());
        }
        [HttpGet]
        public IActionResult AddEquipment()
        {
            return View();
        }



        [HttpPost]
        public IActionResult AddEquipment(RentalEquipment equipment)
        {
            _rentalService.AddEquipment(equipment);
            return RedirectToAction("List");
        }
        [HttpPost]
        public IActionResult DeleteEquipment(int id)
        {

            _rentalService.DeleteEquipment(id);

            return RedirectToAction("List");


        }

        [HttpGet]
        [Route("{controller}/EditEquipment/{id}")]
        public IActionResult EditEquipment(int? id)
        {
            RentalEquipment equipment = _rentalService.GetItem(id.Value);


            return View(equipment);

        }

        [HttpPost]
        [Route("{controller}/EditEquipment/{id}")]
        public ActionResult Edit(RentalEquipment edit)
        {
            _rentalService.EditEquipment(edit);
            return RedirectToAction("List");
        }


    }
}