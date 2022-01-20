using EquipmentRentalService.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalService.Services
{
    public interface IRentalService
    {
        public Task<IEnumerable<RentalHistory>> GetAllHistory(string userId = null);
        public Task<IEnumerable<RentalEquipment>> GetAllRented(string userId = null);
        public Task<IEnumerable<RentalEquipment>> GetAllAvailable(int? categoryId = null);

        public Task<double> CalculatePrice(int equipmentId, DateTimeOffset from, DateTimeOffset to);
        public Task<double> CalculatePrice(int rentalHistoryEntryId);
    }
}
