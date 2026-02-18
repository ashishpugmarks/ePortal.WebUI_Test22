using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class HomePageViewModel
    {
        public PresidentDeskViewModel PresidentDesk { get; set; }
        public List<ContentViewModel> Announcement { get; set; }
        public List<EmergencyContViewModel> ContactList { get; set; }
        public PersonalityQuotesViewModel PersonalityQuotes { get; set; }

        public long ApprovalCount { get; set; }
        public long RequestCount { get; set; }
        public long RequestSelfCount { get; set; }

        public string DOB { get; set; }

        public List<MenuViewModel> QuickLink { get; set; }
        public List<BcMenuViewModel> DreamerCafeLink { get; set; }
        public bool ISPOPUPENABLEFOR_SEATMGMT { get; set; } //Seat booking popup
        public bool ISPOPUPENABLEFOR_KAIZENDEPTCOMMITTEE { get; set; } //kaizen dept committee popup
        public bool ISPOPUPENABLEFOR_KAIZENDivCOMMITTEE { get; set; } //kaizen div committee popup
        public List<NewsLetterHomeViewModel> NewsLetters { get; set; }  //Added By Bhupesh - NTT for CR-4894
    }
    public class ADEMP_FAMILYDeclaration
    {
        public string FamilyName { get; set; }
        public string RelationShip { get; set; }
        public string ISPastorCurrent { get; set; }
        public string addedby { get; set; }
        public Int64 FAMILYASSOCIATE_ID { get; set; }
    }

    public class HomeViewModel
    {
        public bool ShowError { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ChangePasswordViewModel
    {
    [Required]
    public string OldPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }       

    }

    public class ChangePasswordNintyDaysViewModel
    {
        public string StatusText { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public int chklogin { get; set; }

    }

}
