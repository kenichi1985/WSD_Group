using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebHotel.CustomValidation;

namespace WebHotel.Models.RoomAvailabilityViewModels
{
    public class RoomAvailability
    {
        public int BedCount { get; set; }
        [Display(Name = "Check In Date")]
        [DataType(DataType.Date)]
        [DatePassed]
        public DateTime CheckIn { get; set; }
        [DateGreaterThan("CheckIn")]
        [Display(Name = "Check Out Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }
    }
}
