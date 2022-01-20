using EquipmentRentalService.Database;
using EquipmentRentalService.Database.Entities;
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
    }
}
