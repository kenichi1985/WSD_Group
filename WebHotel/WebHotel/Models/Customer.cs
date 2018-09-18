using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class Customer
    {
        [Required, Key, EmailAddress]
        [DataType(DataType.EmailAddress)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z'-]{2,20}$", ErrorMessage = "The Surname field only within 2 to 20, and only accpect letters, ' and - ")]
        public string Surname { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z'-]{2,20}$", ErrorMessage = "The Surname field only within 2 to 20, and only accpect letters, ' and - ")]
        public string GivenName { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "The post code only 4 digits")]
        public string Postcode { get; set; }
        public ICollection<Booking> TheBookings { get; set; }
    }
}
