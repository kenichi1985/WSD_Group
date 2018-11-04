using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.StatisticsViewModels
{
    public class CalcPostcodeStats
    {
        [Key]
        [Display(Name = "Customer's Postcode")]
        public string PostcodeOfCustomer { get; set; }
        [Display(Name = "Number of Customers ")]
        public int NoOfCustomer { get; set; }

    }
}
