using EquipmentRentalService.Database;
using EquipmentRentalService.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalService.Services
{
    public class RentalService : IRentalService
    {
        private readonly ApplicationDbContext _dbContext;
        public RentalService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<RentalHistory>> GetAllHistory(string userId = null)
        {
            IQueryable<RentalHistory> historyQuery = _dbContext.RentalHistory;

            if (!string.IsNullOrEmpty(userId))
            {
                historyQuery = historyQuery.Where(x => x.RentingUser.Id.Contains(userId));
            }

            var history = await historyQuery.ToListAsync();
            return history;
        }

        public async Task<IEnumerable<RentalEquipment>> GetAllAvailable(int? categoryId = null)
        {
            IQueryable<RentalEquipment> equipmentQuery = _dbContext.RentalEquipment;

            if (categoryId != null) equipmentQuery = equipmentQuery.Where(x => x.IsAvailable && x.Category.ID == categoryId);
            else equipmentQuery = equipmentQuery.Where(x => x.IsAvailable);

            var availableEquipment = await equipmentQuery.ToListAsync();
            return availableEquipment;
        }

        public async Task<IEnumerable<RentalEquipment>> GetAllRented(string userId = null)
        {
            IQueryable<RentalEquipment> equipmentQuery = _dbContext.RentalEquipment;

            if (!string.IsNullOrEmpty(userId))
            {
                IQueryable<RentalHistory> historyQuery = _dbContext.RentalHistory;

                historyQuery = historyQuery.Where(x => x.RentingUser.Id.Contains(userId) && x.ReturnedDate == DateTime.MinValue);
                equipmentQuery = equipmentQuery.Where(x => !x.IsAvailable);
                    
                // Linq Join hQ x eQ?

            }

            else equipmentQuery = equipmentQuery.Where(x => !x.IsAvailable);

            var rentedEquipment = await equipmentQuery.ToListAsync();
            return rentedEquipment;
        }

        public async Task<double> CalculatePrice(int equipmentId, DateTimeOffset from, DateTimeOffset to)
        {
            var equipment = await _dbContext.RentalEquipment.FindAsync(equipmentId);
            if (equipment == null) return 0.0;

            return equipment.BaseRentPrice + (to - from).Days * equipment.DailyRentPrice;
        }

        public async Task<double> CalculatePrice(int rentalHistoryEntryId)
        {
            var historyEntry = await _dbContext.RentalHistory.FindAsync(rentalHistoryEntryId);

            if (historyEntry == null) return 0.0;

            var equipment = historyEntry.RentalEquipment;
            DateTimeOffset returnDate;

            if (historyEntry.ReturnedDate == DateTimeOffset.MinValue) returnDate = DateTimeOffset.Now;
            else returnDate = historyEntry.ReturnedDate;

            double price = equipment.BaseRentPrice + (returnDate - historyEntry.RentedDate).Days * equipment.DailyRentPrice;

            if (returnDate > historyEntry.RentedDue)
                return price + (((returnDate - historyEntry.RentedDue).Days * equipment.DailyRentPrice) / 100) * equipment.OverdueRate;
            else return price;
        }

    }
}
