using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class Room
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [RegularExpression(@"^[G123]$", ErrorMessage = "Only accepct G, 1, 2 or 3 ")]
        public string Level { get; set; }
        [RegularExpression(@"^[123]$", ErrorMessage = "Only accepct 1, 2 or 3 ")]
        public int BedCount { get; set; }
        [Range(50,200)]
        public decimal Price { get; set; }
        public ICollection<Booking> TheBookings { get; set; }

    }
}
