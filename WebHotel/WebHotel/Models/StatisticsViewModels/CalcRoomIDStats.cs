using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.StatisticsViewModels
{
    public class CalcRoomIDStats
    {
        [Key]
        [Display(Name ="Room ID")]
        public int RoomID { get; set; }
        [Display(Name="Number of Bookings")]
        public int NoOfBookings { get; set; }
    }
}
