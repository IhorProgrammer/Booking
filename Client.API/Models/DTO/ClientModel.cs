using Client.API.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Client.API.Models.DTO
{
    public class ClientModel
    {
        //[Url(ErrorMessage = "Avatar must be a valid URL.")]
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = default!;

        [Required(ErrorMessage = "Real Name is required.")]
        [StringLength(32, ErrorMessage = "Real Name cannot be longer than 32 characters.")]
        [JsonPropertyName("real_name")]
        public string RealName { get; set; } = default!;

        [Required(ErrorMessage = "Nickname is required.")]
        [StringLength(16, ErrorMessage = "Nickname cannot be longer than 16 characters.")]
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [StringLength(254, ErrorMessage = "Email cannot be longer than 254 characters.")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone must be a valid phone number.")]
        [StringLength(16, ErrorMessage = "Phone cannot be longer than 16 characters.")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Birthday is required.")]
        [DataType(DataType.Date)]
        [JsonPropertyName("birthday")]
        public DateTime Birthday { get; set; }
        [JsonPropertyName("gender")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Citizenship is required.")]
        [StringLength(50, ErrorMessage = "Citizenship cannot be longer than 50 characters.")]
        [JsonPropertyName("citizenship")]
        public string Citizenship { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;


        public bool isValid()
        {

            var context = new ValidationContext(this, null, null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(this, context, results, true))
            {
                return false;
            }
            else
            {
                return AvatarValidation()
                && RealNameValidation()
                && NicknameValidation()
                && EmailValidation()
                && PhoneValidation()
                && BirthdayValidation()
                && GenderValidation()
                && CitizenshipValidation()
                && PasswordValidation()
                ;
            }

            
        }


        public bool AvatarValidation()
        {
            return true; 
        }
        public bool RealNameValidation() 
        {
            var regex = new Regex(@"^\p{Lu}\p{Ll}*$");
            if (!regex.IsMatch(RealName))
            {
                return false;
            }

            return true;
        }
        public bool NicknameValidation() 
        {
            var regex = new Regex("^[a-zA-Z]+$");
            if (!regex.IsMatch(RealName))
            {
                return false;
            }
            return true;
        }
        public bool EmailValidation()
        {
            return true;
        }
        public bool PhoneValidation()
        {
            return true;
        }
        public bool BirthdayValidation()
        {
            return true;
        }
        public bool GenderValidation()
        {
            return true;
        }
        public bool CitizenshipValidation()
        {
            return true;
        }
        public bool PasswordValidation()
        {
            return true;
        }
    }
}
