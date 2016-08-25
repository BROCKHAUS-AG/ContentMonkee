using System;
using System.ComponentModel;
using BAG.Common.Data.Cryptography;

namespace BAG.Common.Data.Entities
{
    public partial class User : Base
    {
        public User()
            : base()
        {
            ExitDate = null;
            EnterDate = DateTime.Now.Date;
            LastLogin = DateTime.Now;
            Birthday = DateTime.Now.AddYears(-25);
            IsAdmin = false;
        }

        public string Language { get; set; }
        public bool IsAdmin { get; set; }

        [DisplayName("Anrede")]
        public Gender Gender { get; set; }

        [DisplayName("Vorname")]
        public string FirstName { get; set; }
        [DisplayName("Nachname")]
        public string LastName { get; set; }
        [DisplayName("Geburtsdatum")]
        public DateTime Birthday { get; set; }


        public string GoogleProfileUrl { get; set; }
        public string XprncProfileUrl { get; set; }
        public string XingProfileUrl { get; set; }
        public string FacebookProfileUrl { get; set; }

        public DateTime EnterDate { get; set; }

        public DateTime? ExitDate { get; set; }

        public DateTime LastLogin { get; set; }

        [DisplayName("E-Mail Alias")]
        public string EmailAlias
        {
            get;
            set;
        }
        [DisplayName("E-Mail")]
        public string Email
        {
            get;
            set;
        }

        public string DisplayName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string DisplayByLastName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string Token { get; set; }

        public void SetPassword(string pHash)
        {
            Password = Cryptography.Hash.ComputeHash(pHash);
        }
    }

    public enum Gender
    {
        Male, Female
    }
}
