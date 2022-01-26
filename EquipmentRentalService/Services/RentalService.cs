using EquipmentRentalService.Database;

using EquipmentRentalService.Models;
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

        public List<RentalHistory> GetAllHistory(string userId = null)
        {
            IQueryable<RentalHistory> historyQuery = _dbContext.RentalHistory;

            if (!string.IsNullOrEmpty(userId))
            {
                historyQuery = historyQuery.Where(x => x.RentingUser.Id.Contains(userId));
            }

            var history = historyQuery.ToList();
            return history;
        }

        public List<RentalEquipment> GetAllAvailable(int? categoryId = null)
        {
            IQueryable<RentalEquipment> equipmentQuery = _dbContext.RentalEquipment;

            if (categoryId != null) equipmentQuery = equipmentQuery.Where(x => x.IsAvailable);
            else equipmentQuery = equipmentQuery.Where(x => x.IsAvailable);

            var availableEquipment = equipmentQuery.ToList();
            return availableEquipment;
        }
        public List<RentalEquipment> GetAll(int? categoryId = null)
        {
            List<RentalEquipment> equipment = _dbContext.RentalEquipment.ToList();

            return equipment;
        }

        public List<RentalEquipment> GetAllRented(string userId = null)
        {
            IQueryable<RentalEquipment> equipmentQuery = _dbContext.RentalEquipment;

            if (!string.IsNullOrEmpty(userId))
            {
                IQueryable<RentalHistory> historyQuery = _dbContext.RentalHistory;

                historyQuery = historyQuery.Where(x => x.RentingUser.Id.Contains(userId) && x.ReturnedDate == DateTime.MinValue);
                equipmentQuery = equipmentQuery.Where(x => !x.IsAvailable);



            }

            else equipmentQuery = equipmentQuery.Where(x => !x.IsAvailable);

            var rentedEquipment = equipmentQuery.ToList();
            return rentedEquipment;
        }

        public void AddEquipment(RentalEquipment equipment)

        {
            _dbContext.RentalEquipment.Add(equipment);
            _dbContext.SaveChanges();
        }
        public RentalEquipment GetItem(int id)

        {
            var item = _dbContext.RentalEquipment.Find(id);
            return item;
        }

        public void DeleteEquipment(int id)
        {
            RentalEquipment equipment = _dbContext.RentalEquipment.Find(id);
            _dbContext.RentalEquipment.Remove(equipment);
            _dbContext.SaveChanges();
        }
        public void EditEquipment(RentalEquipment equipment)
        {
            var model = _dbContext.RentalEquipment.Find(equipment.ID);
            model.Name = equipment.Name;
            model.Description = equipment.Description;
            model.IsAvailable = equipment.IsAvailable;
            model.OverdueRate = equipment.OverdueRate;
            model.DailyRentPrice = equipment.DailyRentPrice;
            model.BaseRentPrice = equipment.BaseRentPrice;
            _dbContext.RentalEquipment.Update(model);
            _dbContext.SaveChanges();
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
