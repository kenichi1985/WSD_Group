using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class Booking
    {
        [Key]
        public int ID { get; set; }
        public int RoomID { get; set; }
        [DataType(DataType.EmailAddress)]
        public string CustomerEmail {get;set;}
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }
        public decimal Cost { get; set; }
        public Customer TheCustomer { get; set; }
        public Room TheRoom { get; set; }
}
}
