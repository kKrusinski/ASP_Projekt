using EquipmentRentalService.Database;
using EquipmentRentalService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalService.Services
{
    public interface IRentalService
    {
        public List<RentalHistory> GetAllHistory(string userId = null);
        public List<RentalEquipment> GetAllRented(string userId = null);
        public List<RentalEquipment> GetAllAvailable();
        public List<RentalEquipment> GetAll();

        public void AddEquipment(RentalEquipment equipment);
        public void DeleteEquipment(int id);

        public void EditEquipment(RentalEquipment equipment);
        public RentalEquipment GetItem(int id);

        public Task<double> CalculatePrice(int equipmentId, DateTimeOffset from, DateTimeOffset to);
        public Task<double> CalculatePrice(int rentalHistoryEntryId);
    }
}
