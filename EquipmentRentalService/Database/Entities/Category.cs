using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalService.Database.Entities
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Category must have a name!")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

    }
}
