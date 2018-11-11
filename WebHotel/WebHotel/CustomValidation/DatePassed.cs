using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.CustomValidation
{
    public class DatePassed : ValidationAttribute
    {
        
        public DatePassed()
        {
           
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if ((DateTime)value == DateTime.MinValue.Date)
            {
                return ValidationResult.Success;
            }

            if ((DateTime)value > DateTime.Today.Date)
            {
                return ValidationResult.Success;
            }


            else
            {
                return new ValidationResult(validationContext.DisplayName + " must be later than today.");
            }
        }
    }
}
