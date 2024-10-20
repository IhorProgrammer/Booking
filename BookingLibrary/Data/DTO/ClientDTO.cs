using BookingLibrary.Helpers.FileManager;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Numerics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BookingLibrary.Data.DTO
{
    public class ClientDTO
    {
        [JsonPropertyName("id")]
        public string ClientID { get; set; } = default!;
        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; } = null;
        [JsonPropertyName("gmail_id")]
        public string? GmailID { get; set; } = null;
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; } = null;
        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; } = null;

        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; } = null;

        [JsonPropertyName("new_nickname")]
        public string? NewNickname { get; set; } = null;
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;
        [JsonPropertyName("phone")]
        public string? Phone { get; set; } = null;
        [JsonPropertyName("birthday")]
        public DateTime? Birthday { get; set; } = null;
        [JsonPropertyName("gender")]
        public bool? Gender { get; set; }
        [JsonPropertyName("citizenship")]
        public string? Citizenship { get; set; } = null;
        [JsonPropertyName("password")]
        public string? Password { get; set; } = null;


        public Dictionary<string, string?> ToDictionarySS()
        {
            return new Dictionary<string, string?>
            {
                { "id", this.ClientID },
                { "avatar", this.Avatar },
                { "gmail_id", this.GmailID },
                { "given_name", this.GivenName },
                { "family_name", this.FamilyName },
                { "nickname", this.Nickname },
                { "email", this.Email },
                { "phone", this.Phone },
                { "birthday", this.Birthday.ToString() },
                { "gender", this.Gender.ToString() },
                { "citizenship", this.Citizenship },
                { "password", this.Password },                
            };
        }
        public static async Task<ClientDTO> ToClientModel(IFormCollection form, IFormFileCollection formFiles)
        {
            ClientDTO clientModel = new ClientDTO();
            try
            {
                clientModel.GivenName = form["given_name"];
                clientModel.FamilyName = form["family_name"];
                string? nickname = form["nickname"];
                clientModel.Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname), "ToClientModel Error");
                string? email = form["email"];
                clientModel.Email = email ?? throw new ArgumentNullException(nameof(email), "ToClientModel Error");
                clientModel.Phone = form["phone"];
                string? birthday = form["birthday"];
                clientModel.Birthday = !String.IsNullOrEmpty(birthday) ? DateTime.Parse(birthday) : null;
                string? gender = form["gender"];
                clientModel.Gender = !String.IsNullOrEmpty(gender) ? Boolean.Parse(gender) : null;

                clientModel.Citizenship = form["citizenship"];
                string? password = form["password"];
                clientModel.Password = password ?? throw new ArgumentNullException(nameof(password), "ToClientModel Error"); ;

                clientModel.Avatar = form["avatar"];
                clientModel.GmailID = form["gmail_id"];

                string? clientID = form["id"];
                if(clientID != null) clientModel.ClientID = clientID;

                clientModel.NewNickname = form["new_nickname"];


                if (formFiles.Count >= 1 && formFiles[0] != null)
                {
                    string ext = Path.GetExtension(formFiles[0].FileName).ToLower();
                    string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };
                    if (allowedExtensions.Contains(ext))
                    {
                        string imagePath = Path.Combine("image");
                        Directory.CreateDirectory(imagePath);
                        clientModel.Avatar = await FileManager.SaveFile(formFiles[0], "image");
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(ext), "Extension image error");
                    }
                }
                return clientModel;
            }
            catch (Exception ex) 
            {
                if( !String.IsNullOrEmpty(clientModel.Avatar) )
                {
                    FileManager.DeleteFile(clientModel.Avatar, "image");
                }
                throw ex;
            }
            
        }

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
                return GivenNameValidation()
                && FamilyNameValidation()
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

        public bool GivenNameValidation() 
        {
            if (GivenName == null) return true;

            if( String.IsNullOrEmpty(GivenName) ) return false;
            if (GivenName.Length > 32) return false;

            var regex = new Regex(@"^\p{Lu}\p{Ll}*$");
            if (!regex.IsMatch(GivenName))
            {
                return false;
            }

            return true;
        }
        public bool FamilyNameValidation()
        {
            if (FamilyName == null) return true;

            if (String.IsNullOrEmpty(FamilyName)) return false;
            if (FamilyName.Length > 32) return false;

            var regex = new Regex(@"^\p{Lu}\p{Ll}*$");
            if (!regex.IsMatch(FamilyName))
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
            if (!regex.IsMatch(Nickname))
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
            if (Phone == null) return true;

            if (String.IsNullOrEmpty(Phone)) return false;
            if (Phone.Length > 16) return false;
            Regex validatePhoneNumberRegex = new Regex("^\\+?[1-9][0-9]{7,14}$");
            if (!validatePhoneNumberRegex.IsMatch(Phone)) return false;
            return true;
        }
        public bool BirthdayValidation()
        {
            if( Birthday == null) return true;
            if( Birthday > DateTime.Now.AddYears(-18) ) return false;
            return true;
        }
        public bool GenderValidation()
        {
            return true;
        }
        public bool CitizenshipValidation()
        {
            if (Citizenship == null) return true;
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
