using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.Shared;

namespace ePortal.Application.Services
{
    public class BenevolentService : IBenevolent
    {
        private readonly BenevolentRepository _benRepo;
        public BenevolentService(BenevolentRepository benRepo)
        {
            _benRepo = benRepo;

        }

        public List<BENEVOLENT_MST> GetBenevolentMST()
        {
            return _benRepo.GetBenevolentMST();
        }
        public List<BenevolentViewModel> GetBenevolentMSTList(SearchBenevolent si)
        {
            return _benRepo.GetBenevolentMSTList(si);
        }

        //public List<BENEVOLENT_MST> GetBenevolentMST()
        //{
        //    return _benRepo.GetBenevolentMST();
        //}

        public short AddBenevolentMST(BENEVOLENT_MST mst)
        {
            return _benRepo.AddBenevolentMST(mst);
        }

        public List<Employee_Details> PortalAutocompleteSuggestions(string term, string designation)
        {
            return _benRepo.PortalAutocompleteSuggestions(term, designation);
        }

        public BenevolentViewModel GetEmpDetail(long id)
        {
            return _benRepo.GetEmpDetail(id);
        }

        public BENEVOLENT_MST GetEmpBenevolentMST(long id)
        {
            return _benRepo.GetEmpBenevolentMST(id);
        }

        public short EditBenevolentMST(BENEVOLENT_MST mst)
        {
            return _benRepo.EditBenevolentMST(mst);
        }

        public short AddContribution(BENEVOLENT_DT dt)
        {
            return _benRepo.AddContribution(dt);
        }

        public List<BenevolentViewModel> ContributionBenMSTList(SearchBenevolent si)
        {
            return _benRepo.ContributionBenMSTList(si);
        }

        public List<BenevolentViewModel> GetContributionReport(SearchBenevolent sb)
        {
            return _benRepo.GetContributionReport(sb);
        }

        public short SentMailToVendor()
        {
            short retVal = 0;
            try
            {

                #region Send mail next approval authority
                string strCCMail = "";

                //ePortal.Core.EmailCore sendMail = new ePortal.Core.EmailCore();
                EmailCore sendMail = new EmailCore();
                //sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailFrom = "mayank.p@aumentotec.com";
                //if (serverpath.isTestServer())
                sendMail.MailTo = "mital.k@aumentotec.com";
                //else
                //    sendMail.MailTo = PHVM.VENDORMAILID;

                sendMail.MailCc = strCCMail;
                //string struid = (Auth_obj.APPEMP_CODE).ToString();
                string strid = "8607";
                string strSubject = "Testing";
                string strBody = string.Empty;
                strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF></font></b></td></tr>" +
                                "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Hi," + "</br>" + " Please find the digitally approved PO copy . PO details are as follows:</td></tr>" +
                                "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + "Mayank" + "</td></tr>" +
               //"<tr><td width=125 height=23 valign=top>Amendment No.</td><td width=389 valign=top>" + PHVM.VERSIONNO + "</td></tr>" +
               "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;

                //sendMail.attContentType = "application/pdf";
                //string path = HttpContext.Current.Server.MapPath("~/Uploads/PO/" + pd.FILENAME);
                //sendMail.Mailattachement = new FileStream(path, FileMode.Open, FileAccess.Read);//File.ReadAllBytes(path);
                //sendMail.attachementName = pd.FILENAME;

                try
                {
                    bool status = sendMail.Send();
                    //retVal = _PORepo.UpdateMailCNT(PHVM.POHEADERID);
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }
                finally
                {
                }

                //}
                //}
                #endregion

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public int GetEmpCode(long id)
        {
            return _benRepo.GetEmpCode(id);
        }

        public BenevolentViewModel GetContributionReportDetail(long id)
        {
            return _benRepo.GetContributionReportDetail(id);
        }

        public BenevolentViewModel GetDemiseEmployeeDetail(long id)
        {
            return _benRepo.GetDemiseEmployeeDetail(id);
        }
    }
}
