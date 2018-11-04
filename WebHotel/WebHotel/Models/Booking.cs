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
        [Display(Name="Room ID")]
        public int RoomID { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Customer Email")]
        public string CustomerEmail {get;set;}
        [DataType(DataType.Date)]
        [Display(Name = "Check In Date")]
        public DateTime CheckIn { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Check Out Date")]
        public DateTime CheckOut { get; set; }
        public decimal Cost { get; set; }
        [Display(Name = "Customer")]
        public Customer TheCustomer { get; set; }
        [Display(Name = "Room")]
        public Room TheRoom { get; set; }
}
}
