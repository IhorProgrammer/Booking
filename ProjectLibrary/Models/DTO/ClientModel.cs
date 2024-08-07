using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ProjectLibrary.Models.DTO
{
    public class ClientModel
    {
        //[Url(ErrorMessage = "Avatar must be a valid URL.")]
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = default!;
        [JsonPropertyName("real_name")]
        public string RealName { get; set; } = default!;
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = default!;
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = default!;
        [JsonPropertyName("birthday")]
        public DateTime Birthday { get; set; }
        [JsonPropertyName("gender")]
        public bool Gender { get; set; }
        [JsonPropertyName("citizenship")]
        public string Citizenship { get; set; } = default!;
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

            if( String.IsNullOrEmpty(RealName) ) return false;
            if (RealName.Length > 32) return false;

            var regex = new Regex(@"^\p{Lu}\p{Ll}*$");
            if (!regex.IsMatch(RealName))
            {
                return false;
            }

            return true;
        }
        public bool NicknameValidation() 
        {
            if (String.IsNullOrEmpty(Nickname)) return false;
            if (Nickname.Length > 16) return false;

            var regex = new Regex("^[a-zA-Z]+$");
            if (!regex.IsMatch(RealName))
            {
                return false;
            }
            return true;
        }
        public bool EmailValidation()
        {
            if (String.IsNullOrEmpty(Email)) return false;
            if (Email.Length > 254) return false;
            try
            {
                MailAddress m = new MailAddress(Email);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool PhoneValidation()
        {
            if (String.IsNullOrEmpty(Phone)) return false;
            if (Phone.Length > 16) return false;
            Regex validatePhoneNumberRegex = new Regex("^\\+?[1-9][0-9]{7,14}$");
            if (!validatePhoneNumberRegex.IsMatch(Phone)) return false;
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
            if (String.IsNullOrEmpty(Citizenship)) return false;
            if (Citizenship.Length > 16) return false;
            return true;
        }
        public bool PasswordValidation()
        {
            if (String.IsNullOrEmpty(Password)) return false;
            if (Password.Length > 100) return false;
            Regex validatePasswordRegex = new Regex("^(?=.*?[A-Za-z])(?=.*?[0-9]).{8,}$");
            if (!validatePasswordRegex.IsMatch(Password)) return false;
            return true;
        }
    }
}
