using EquipmentRentalService.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalService.Models
{
    public class RentalHistory
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public IdentityUser RentingUser { get; set; }
        [Required]
        public RentalEquipment RentalEquipment { get; set; }
        [Required]
        public DateTimeOffset RentedDate { get; set; }
        public DateTimeOffset RentedDue { get; set; }
        public DateTimeOffset ReturnedDate { get; set; }
    }
}
