using System;
using System.ComponentModel;

namespace BAG.Common.Data.Entities
{
    public class Employee : Base
    {

        public Employee()
            : base()
         {
            ExitDate = null;
            EnterDate = null;
            Birthday = null;
            ProfilImagePath = string.Empty;
        }

        [DisplayName("Anrede")]
        public Gender Gender { get; set; }
        [DisplayName("Vorname")]
        public string FirstName { get; set; }
        [DisplayName("Nachname")]
        public string LastName { get; set; }
        public string Position { get; set; }
        [DisplayName("E-Mail")]
        public string Email { get; set; }
        [DisplayName("E-Mail Alias")]
        public string EmailAlias { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public DateTime? EnterDate { get; set; }
        public DateTime? ExitDate { get; set; }
        [DisplayName("Geburtsdatum")]
        public DateTime? Birthday { get; set; }
        [DisplayName("Profil-Bild")]
        public string ProfilImagePath { get; set; }
        [DisplayName("User-Id")]
        public Guid UserId { get; set; }
        [DisplayName("Google+ Profil")]
        public string GoogleProfileUrl { get; set; }
        [DisplayName("Xprnc-Profil")]
        public string XprncProfileUrl { get; set; }
        [DisplayName("Xing-Profil")]
        public string XingProfileUrl { get; set; }
        [DisplayName("LinkedIn-Profil")]
        public string LinkedInProfileUrl { get; set; }
        [DisplayName("Facebook-Profil")]
        public string FacebookProfileUrl { get; set; }
        [DisplayName("VCardUrl")]
        public string VCardUrl { get; set; }
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
    }
}

