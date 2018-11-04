using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.StatisticsViewModels
{
    public class CalcBedCountStats
    {
        [Key]
        [Display(Name = "Number of Bed")]
        public int BedCount { get; set; }
        [Display(Name = "Number of Bookings")]
        public int NoOfBookings { get; set; }

    }
}
