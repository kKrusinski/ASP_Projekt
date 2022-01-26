using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalService.Models
{
    public class RentalEquipment
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Equipment must have a name!")]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [Display(Name = "Added")]
        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;

        [Required(ErrorMessage = "Base price must be set")]
        [Display(Name = "Base price")]
        public double BaseRentPrice { get; set; }

        [Required(ErrorMessage = "Price per day must be set")]
        [Display(Name = "Per day")]
        public double DailyRentPrice { get; set; }

        [Required(ErrorMessage = "Overdue rate must be set")]
        [Display(Name = "Overdue rate [%]")]
        public int OverdueRate { get; set; }
        public List<RentalHistory> RentalHistory { get; set; } = new();
        [Required]
        public bool IsAvailable { get; set; } = true;

    }
}
