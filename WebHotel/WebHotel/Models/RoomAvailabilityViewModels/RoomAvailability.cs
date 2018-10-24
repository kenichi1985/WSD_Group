using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.RoomAvailabilityViewModels
{
    public class RoomAvailability
    {
        public int BedCount { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
