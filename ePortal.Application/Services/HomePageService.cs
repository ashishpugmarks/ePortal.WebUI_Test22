using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;


namespace ePortal.Application.Services
{
    public class HomePageService : IHomePageService
    {
        private readonly HomeRepository _objHomeRepositry;
        private readonly CommonRepository _objcommRespository;
        public HomePageService(HomeRepository objHomeRepositry, CommonRepository objcommRespository)
        {
            _objHomeRepositry = objHomeRepositry;
            _objcommRespository = objcommRespository;
        }

        public long GetApprovalCount(long userid)
        {
            return _objHomeRepositry.GetApprovalCount(userid);
        }

        public List<EmergencyContViewModel> GetContactList(EmergencyContViewModel objsearch)
        {
            return _objHomeRepositry.GetContactList(objsearch);
        }

        public List<ContentViewModel> GetContent(ContentViewModel objsearch)
        {
            return _objHomeRepositry.GetContent(objsearch);
        }

        public List<ContentViewModel> GetContent()
        {
            return _objHomeRepositry.GetContent();
        }

        public List<ContentViewModel> GetCommunicationContent()
        {
            return _objHomeRepositry.GetCommunicationContent();
        }

        public PersonalityQuotesViewModel GetFamousQuotes(PersonalityQuotesViewModel objsearch)
        {
            return _objHomeRepositry.GetFamousQuotes(objsearch);
        }

        public List<MenuViewModel> GetMenu(int internet)
        {
            List<MenuViewModel> objdata = _objHomeRepositry.GetMenu(internet);
            return objdata.ToList();
        }

        public PresidentDeskViewModel GetPresidentDetail(PresidentDeskViewModel objsearch)
        {
            PresidentDeskViewModel objRet = new PresidentDeskViewModel();
            objRet = _objHomeRepositry.GetPresidentDetail(objsearch);
            return objRet;
        }

        public long GetRequestCount(long userid)
        {
            return _objHomeRepositry.GetRequestCount(userid);
        }
        public List<PeopleSerchViewModel> GetAssociateDetails(string KI, string OperationID, string DivisionID, string DepartmentID, string SectionID, string EmpCode, string FirstName, string LastName, string Designation, string EMailID, string BloodGroup)
        {
            return _objHomeRepositry.GetAssociateDetails(KI, OperationID, DivisionID, DepartmentID, SectionID, EmpCode, FirstName, LastName, Designation, EMailID, BloodGroup);
        }

        public List<PresidentDesk_TRNViewModel> GetPresidentContent(List<PresidentDesk_TRNViewModel> objsearch)
        {
            return _objHomeRepositry.GetPresidentContent(objsearch);
        }
        public List<PresidentDesk_TRNViewModel> GetPresidentContentDwn(List<PresidentDesk_TRNViewModel> objsearch)
        {
            return _objHomeRepositry.GetPresidentContentDwn(objsearch);
        }

        public async Task CreateUserMenuLog(string Ecode, string URL_IN)
        {
            await _objHomeRepositry.CreateUserMenuLog(Ecode, URL_IN);
        }

        public List<MenuViewModel> GetQuickLinkMenu(int internet)
        {
            return _objHomeRepositry.GetQuickLinkMenu(internet);
        }
        public MenuViewModel GetMenuURL(MenuViewModel objMenu)
        {
            return _objcommRespository.GetMenuURL(objMenu);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name = "Ecode" ></ param >
        /// < param name="Usertype"></param>
        /// <param name = "ISOnlyMandatory" > 1 means only mandatory and 2 means all</param>
        /// <returns></returns>
        public async Task<string> GetMandatorytoFilledPage(decimal Ecode, decimal Usertype, Int16 ISOnlyMandatory, decimal ISSSOLOGIN) //--90/30 days password policy
        {

            string strmandatory = await _objcommRespository.GetMandatorytoFilledPage(Ecode, Usertype, ISSSOLOGIN); //--90/30 days password policy
            if (strmandatory == "" || strmandatory == "null")
                return "";

            //if (string.IsNullOrEmpty(strmandatory))
            //    return "";

            bool flag = false;
            string menuid = "";
            string[] arrayurl = strmandatory.Split(',');
            //check mandatory count
            foreach (var o in arrayurl)
            {
                string[] arrman = o.Split('~');
                if (arrman[1] == "1")
                {
                    flag = true;
                    menuid = arrman[0];
                    break;
                }
            }
            if (flag)
                return _objcommRespository.GetMenuURL(new MenuViewModel { MenuId = Convert.ToInt64(menuid) }).URL;
            else
            {
                if (ISOnlyMandatory == 1)
                    return "";

                foreach (var o in arrayurl)
                {
                    string[] arrman = o.Split('~');
                    if (arrman[1] == "0")
                    {
                        flag = true;
                        menuid = arrman[0];
                        break;
                    }
                }
                if (flag)
                    return _objcommRespository.GetMenuURL(new MenuViewModel { MenuId = Convert.ToInt64(menuid) }).URL;
                else
                    return "";
            }
        }

        public async Task<string> ISAUTH_ACCESS(string Ecode, string URL)
        {
            return await _objcommRespository.ISAUTH_ACCESS(Ecode, URL);

        }

        public List<ExtPortalViewModel> GetExtPortalList()
        {
            return _objHomeRepositry.GetExtPortalList();
        }
    }
}
