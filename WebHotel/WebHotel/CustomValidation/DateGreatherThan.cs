using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.CustomValidation
{
    public class DateGreaterThan : ValidationAttribute
    {
        private string _checkInDatePropertyName;
        public DateGreaterThan(string checkInDatePropertyName)
        {
            _checkInDatePropertyName = checkInDatePropertyName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(_checkInDatePropertyName);
            if (propertyInfo == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _checkInDatePropertyName));
            }
            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if ((DateTime)propertyValue == DateTime.MinValue.Date)
            {
                return ValidationResult.Success;
            }

            if ((DateTime)value > (DateTime)propertyValue)
            {
                return ValidationResult.Success;
            }
            else
            {
                var startDateDisplayName = propertyInfo
                    .GetCustomAttributes(typeof(DisplayAttribute), true)
                    .Cast<DisplayAttribute>()
                    .Single()
                    .Name;
                return new ValidationResult(validationContext.DisplayName + " must be later than " + startDateDisplayName + ".");
            }
        }
    }
}
