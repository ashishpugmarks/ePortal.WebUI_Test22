using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class CalenderMasterRepository
    {
        private EPortalDBContext _CalenderMasterDBContext;
        public CalenderMasterRepository(EPortalDBContext CalenderMasterDBContext)
        {
            _CalenderMasterDBContext = CalenderMasterDBContext;
        }       

        public IEnumerable<SYSITE> Bind_SYSite()
        {
            IEnumerable<SYSITE> iList;

            iList = (from data in _CalenderMasterDBContext.SYSITE.ToList()
                     select new SYSITE
                     {
                         SYSITEID = data.SYSITEID,
                         DESCRIP = data.DESCRIP != null ? data.DESCRIP.Substring(data.DESCRIP.IndexOf('-') + 1) : null,
                     });
            return iList.OrderBy(x => x.DESCRIP);
        }

        public IEnumerable<Employee_Details> BindAppAuth1()
        {

            long strKIID = (long)_CalenderMasterDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            IEnumerable<Employee_Details> iList;

            var empdata_fundesg = (from userdata in _CalenderMasterDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
                                   join emp in _CalenderMasterDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                                   join d in _CalenderMasterDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals d.ADDESIGNATIONID
                                   join fg in _CalenderMasterDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                                   from fg in ls.DefaultIfEmpty()
                                   select new
                                   {
                                       ADEMPCODE = emp.ADEMPCODE,
                                       FIRSTNAME = emp.FIRSTNAME,
                                       LASTNAME = emp.LASTNAME,
                                       _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                                   }
                           ).Distinct().ToList();

            iList = (from userdata in empdata_fundesg
                     select new Employee_Details
                     {
                         _ECode = userdata.ADEMPCODE,
                         _EName = userdata.ADEMPCODE + "-" + userdata.FIRSTNAME + " " + userdata.LASTNAME,
                     }).OrderBy(x => x._ECode).ToList();

            return iList;
        }

        public Int16 SaveProcessAttachment_Trn(CalenderMasterViewModel AVM)
        {
            Int16 retVal = 0;
            using (var transaction = _CalenderMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    long maxval = _CalenderMasterDBContext.CALENDARMASTERHEADER.Select(x => x.SRNO).DefaultIfEmpty(0).Max();
                    maxval = maxval + 1;


                    CALENDARMASTERHEADER CM = new CALENDARMASTERHEADER();
                    CM.SRNO = maxval;
                    CM.FINANCIALYEAR = AVM.SelectedFinancialYear.ToString();
                    CM.CALENDER = AVM.CALENDER;
                    CM.LOCATION = AVM.LOCATION;
                    CM.CALENDER_BLOB = AVM.CALENDER_BLOB;
                    CM.CALENDER_CONTENTTYPE = AVM.CALENDER_CONTENTTYPE;
                    CM.CALENDER_UPLOAD = AVM.CALENDER_UPLOAD;
                    CM.STATUS = 2;//WIP
                    CM.APP_AUTH1 = AVM.APP_AUTH1;
                    CM.APP_AUTH2 = AVM.APP_AUTH2;
                    CM.CREATED_BY = AVM.CREATED_BY;
                    CM.CREATED_DATE = DateTime.Now;

                    _CalenderMasterDBContext.CALENDARMASTERHEADER.Add(CM);
                    _CalenderMasterDBContext.SaveChanges();

                    long maxValCMT = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Select(x => x.CALENDERMAPPINGID).DefaultIfEmpty(0).Max();
                    maxValCMT = maxValCMT + 1;

                    if (CM.APP_AUTH1 != null)
                    {


                        CALENDARMASTERAPPHIS CPTM = new CALENDARMASTERAPPHIS();
                        CPTM.CALENDERMAPPINGID = maxValCMT;
                        CPTM.CAL_MAS_SRNO = CM.SRNO;
                        CPTM.STATUS = 0;
                        CPTM.APPAUTH_ECODE = CM.APP_AUTH1;
                        //CPTM.APPAUTH_DATE = DateTime.Now;
                        //CPTM.APPAUTH_REMARKS = AVM.INITIATOR_REMARKS;
                        CPTM.CREATEDBY = long.Parse(CM.CREATED_BY);
                        CPTM.CREATEDDATE = DateTime.Now;
                        CPTM.APP_LEVEL = 1;
                        _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Add(CPTM);
                        _CalenderMasterDBContext.SaveChanges();
                        maxValCMT = maxValCMT + 1;
                    }

                    if (CM.APP_AUTH2 != null)
                    {
                        CALENDARMASTERAPPHIS CPTM = new CALENDARMASTERAPPHIS();
                        CPTM.CALENDERMAPPINGID = maxValCMT;
                        CPTM.CAL_MAS_SRNO = CM.SRNO;
                        CPTM.APPAUTH_ECODE = CM.APP_AUTH2;
                        //CPTM.APPAUTH_DATE = DateTime.Now;
                        //CPTM.APPAUTH_REMARKS = AVM.INITIATOR_REMARKS;
                        CPTM.CREATEDBY = long.Parse(CM.CREATED_BY);
                        CPTM.CREATEDDATE = DateTime.Now;
                        CPTM.APP_LEVEL = 2;
                        _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Add(CPTM);
                        _CalenderMasterDBContext.SaveChanges();
                        maxValCMT = maxValCMT + 1;
                    }

                    // Save for attachment approval
                    //CreativeMasterViewModel AAVM = new CreativeMasterViewModel();
                    //AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                    //AAVM.INITIATED_BY = CPT.CREATED_BY;
                    //AAVM.APPAUTH1_ECODE = AVM.APPAUTH1_ECODE;
                    ////string[] parts = AVM.APPAUTH1_ECODE.Split('-');
                    //AAVM.APPAUTH2_ECODE = AVM.APPAUTH2_ECODE;
                    //AAVM.INITIATOR_REMARKS = AVM.INITIATOR_REMARKS;
                    //SaveAttachmentApproval_Trn(AAVM);
                    //if (AVM.APPAUTH1_ECODE != null)
                    //{
                    //    CM_PROCESSATTACHMENTAPPMAPPING_TRN CPTM = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                    //    CPTM.MAPPINGATTACHMENTID = maxval;
                    //    CPTM.ATTACHMENTID = CPT.ATTACHMENTID;
                    //    CPTM.STATUS = 0;
                    //    CPTM.APPAUTH_ECODE = AVM.APPAUTH1_ECODE;
                    //    //CPTM.APPAUTH_DATE = DateTime.Now;
                    //    //CPTM.APPAUTH_REMARKS = AVM.INITIATOR_REMARKS;
                    //    CPTM.CREATED_BY = AVM.CREATED_BY;
                    //    CPTM.CREATED_DATE = DateTime.Now;
                    //    CPTM.APP_LEVEL = 1;
                    //    _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                    //    _CreativeMasterDBContext.SaveChanges();
                    //    maxval = maxval + 1;
                    //}
                    //if (AVM.APPAUTH2_ECODE != null)
                    //{
                    //    CM_PROCESSATTACHMENTAPPMAPPING_TRN CPTM = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                    //    CPTM.MAPPINGATTACHMENTID = maxval;
                    //    CPTM.ATTACHMENTID = CPT.ATTACHMENTID;
                    //    //CPTM.STATUS = 0;
                    //    CPTM.APPAUTH_ECODE = AVM.APPAUTH2_ECODE;
                    //    //CPTM.APPAUTH_DATE = DateTime.Now;
                    //    //CPTM.APPAUTH_REMARKS = AVM.INITIATOR_REMARKS;
                    //    CPTM.CREATED_BY = AVM.CREATED_BY;
                    //    CPTM.CREATED_DATE = DateTime.Now;
                    //    CPTM.APP_LEVEL = 2;
                    //    _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                    //    _CreativeMasterDBContext.SaveChanges();
                    //}
                    transaction.Commit();
                    retVal = 1;
                    //return AVM;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<CalenderMasterViewModel> GetCalenderMasterList()
        {
            List<CalenderMasterViewModel> iList = new List<CalenderMasterViewModel>();

            List<string> siteIdList = _CalenderMasterDBContext.SYSITE.Select(v => v.SYSITEID.ToString()).ToList();

            var temp = (from data in _CalenderMasterDBContext.CALENDARMASTERHEADER
                        select data.LOCATION).ToArray();

            string[] locationValues = string.Join(",", temp).Split(',');

            var iColl = (from data in _CalenderMasterDBContext.CALENDARMASTERHEADER
                         where locationValues.Any(location => siteIdList.Contains(location))
                         select data).ToList();

            //var Description1 = (from data in _CalenderMasterDBContext.CALENDARMASTERHEADER
            //                   join v in _CalenderMasterDBContext.SYSITE on data.LOCATION.Any(locationValues) equals v.SYSITEID
            //                   where locationValues.Any(location => siteIdList.Contains(location))
            //                   select v.DESCRIP).ToList();

            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {

                    //var Description = (from v in _CalenderMasterDBContext.SYSITE
                    //                   where obj.LOCATION.Contains(v.SYSITEID.ToString()) && v.ACTIVE == 1
                    //                   select v.DESCRIP).ToList();
                    string[] locationValues1 = string.Join(",", obj.LOCATION).Split(',');
                    Int64[] locationIds1 = null;
                    if (locationValues1.Length > 0)
                    {
                        if (locationValues1[0] != "")
                        {
                            locationIds1 = Array.ConvertAll(locationValues1, Int64.Parse);
                        }

                    }

                    string combinedDesc1 = ""; // Initialize the string variable outside the loop
                    if (locationIds1 != null)
                    {
                        foreach (var i in locationIds1)
                        {
                            var Description = (from v in _CalenderMasterDBContext.SYSITE
                                               where v.SYSITEID == i && v.ACTIVE == 1
                                               select v.DESCRIP).ToList();

                            var desc1 = Description.Select(DESCRIP => DESCRIP?.Substring(DESCRIP.IndexOf('-') + 1)).ToList();

                            // Assuming you want to concatenate the results
                            if (desc1.Any())
                            {
                                combinedDesc1 += string.Join(" || ", desc1) + " || "; // Add " || " separator
                            }

                        }

                    }



                    var AppStatus = (from v in _CalenderMasterDBContext.CALENDARMASTERAPPHIS where v.CAL_MAS_SRNO == obj.SRNO && v.APP_LEVEL == 1 select v.STATUS).FirstOrDefault();
                    var AppStatus2 = (from v in _CalenderMasterDBContext.CALENDARMASTERAPPHIS where v.CAL_MAS_SRNO == obj.SRNO && v.APP_LEVEL == 2 select v.STATUS).FirstOrDefault();
                    var calH = (from v in _CalenderMasterDBContext.CALENDARMASTERHEADER where v.SRNO == obj.SRNO select v.STATUS).FirstOrDefault();

                    if (calH == 1 || calH == 0 || calH == 3)
                    {
                        AppStatus = 0;//not dispaly
                    }
                    else if (calH == 2 && (AppStatus == 1 || AppStatus2 == 1))
                    {
                        AppStatus = 0;//not dispaly
                    }
                    else if (calH == 2 && (AppStatus == 2 || AppStatus2 == 2))
                    {
                        AppStatus = 0;//not dispaly
                    }
                    else
                    {
                        AppStatus = 1;//dispaly
                    }
                    //if ((AppStatus == 1 || AppStatus == 0) && calH ==2)
                    //{
                    //    var AppStatus2 = (from v in _CalenderMasterDBContext.CALENDARMASTERAPPHIS where v.CAL_MAS_SRNO == obj.SRNO && v.APP_LEVEL == 2 select v.STATUS).FirstOrDefault();
                    //    if (AppStatus2 == 1 )
                    //    {     
                    //        if(calH == 2)
                    //        {
                    //            AppStatus = 1;//dispaly
                    //        }
                    //        else
                    //        {
                    //            AppStatus = 0;//not dispaly
                    //        }                     

                    //    }
                    //    else{
                    //        AppStatus = 0;
                    //    }

                    //}
                    //else
                    //{
                    //    AppStatus = 0;
                    //}

                    var AppAuthDate = (from v in _CalenderMasterDBContext.CALENDARMASTERAPPHIS where v.CAL_MAS_SRNO == obj.SRNO && v.APP_LEVEL == 2 select v.APPAUTH_DATE).FirstOrDefault();
                    string AppAuthDate1 = "";
                    if (AppAuthDate != null)
                    {
                        AppAuthDate1 = AppAuthDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        AppAuthDate1 = "Pending For Approval";
                    }
                    //var Desc = Description.Select(DESCRIP => DESCRIP?.Substring(DESCRIP.IndexOf('-') + 1)).ToList();

                    //var Desc = Description.ToString().Substring(Description.IndexOf('-') + 1);
                    iList.Add(new CalenderMasterViewModel
                    {
                        SRNO = obj.SRNO,
                        FINANCIALYEAR = obj.FINANCIALYEAR,
                        CALENDER = obj.CALENDER,
                        CALENDER_UPLOAD = obj.CALENDER_UPLOAD,
                        LOCATION = obj.LOCATION,
                        LOCATIONDESC = combinedDesc1.Substring(0, combinedDesc1.Length - 3),
                        //LOCATIONDESC = string.Join(" || ", Desc),
                        STATUS = obj.STATUS,
                        FLAG = AppStatus,
                        LAST_APPPROVED_ON = AppAuthDate1,
                        //LOCATION = (obj.LOCATION + " - " + Description.Substring(Description.IndexOf('-') + 1)),                       
                    });
                }
            }

            return iList;
        }



        public List<CalenderMasterViewModel> GetCalenderMasterListForUser()
        {
            List<CalenderMasterViewModel> iList = new List<CalenderMasterViewModel>();


            List<string> siteIdList = _CalenderMasterDBContext.SYSITE.Select(v => v.SYSITEID.ToString()).ToList();

            var temp = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(data => data.STATUS == 1).Select(data => data.LOCATION).Distinct().ToArray();


            string[] locationValues = string.Join(",", temp).Split(',');


            var iColl = (from data in _CalenderMasterDBContext.CALENDARMASTERHEADER
                         where locationValues.Any(location => siteIdList.Contains(location)) && data.STATUS == 1
                         select data).ToList();


            var List = iColl.GroupBy(r => new { r.FINANCIALYEAR, r.CALENDER, r.LOCATION }).Select(g => g.First()).ToList();



            if (List.Count > 0)
            {

                foreach (var obj in List)
                {

                    string[] locationValues1 = string.Join(",", obj.LOCATION).Split(',');
                    Int64[] locationIds1 = null;
                    if (locationValues1.Length > 0)
                    {
                        if (locationValues1[0] != "")
                        {
                            locationIds1 = Array.ConvertAll(locationValues1, Int64.Parse);
                        }

                    }

                    string combinedDesc1 = ""; // Initialize the string variable outside the loop

                    foreach (var i in locationIds1)
                    {
                        var Description = (from v in _CalenderMasterDBContext.SYSITE
                                           where v.SYSITEID == i && v.ACTIVE == 1
                                           select v.DESCRIP).ToList();

                        var desc1 = Description.Select(DESCRIP => DESCRIP?.Substring(DESCRIP.IndexOf('-') + 1)).ToList();

                        // Assuming you want to concatenate the results
                        if (desc1.Any())
                        {
                            combinedDesc1 += string.Join(" || ", desc1) + " || "; // Add " || " separator
                        }

                    }


                    iList.Add(new CalenderMasterViewModel
                    {
                        SRNO = obj.SRNO,
                        FINANCIALYEAR = obj.FINANCIALYEAR,
                        CALENDER = obj.CALENDER,
                        CALENDER_UPLOAD = obj.CALENDER_UPLOAD,
                        LOCATION = obj.LOCATION,
                        LOCATIONDESC = combinedDesc1.Substring(0, combinedDesc1.Length - 3),
                        STATUS = obj.STATUS,

                    });
                }
            }

            return iList;
        }
        public CalenderMasterViewModel GetCalenderMasterDetails(int SrNo)
        {
            CalenderMasterViewModel PAM = new CalenderMasterViewModel();
            CALENDARMASTERHEADER CMV = new CALENDARMASTERHEADER();

            CMV = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.SRNO == SrNo).SingleOrDefault();



            //var iColl = (from data in _CalenderMasterDBContext.CALENDARMASTERHEADER
            //             where locationValues.Any(location => siteIdList.Contains(location))
            //             select data).ToList();

            var ADEMPCODE = long.Parse(CMV.CREATED_BY);
            //string[] locationValues = string.Join(",", temp).Split(',');
            var LOCATION = CMV.LOCATION;
            var APP_AUTH1_NAME = _CalenderMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == CMV.APP_AUTH1).SingleOrDefault();
            var APP_AUTH2_NAME = _CalenderMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == CMV.APP_AUTH2).SingleOrDefault();
            var CREATED_BY_USER = _CalenderMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ADEMPCODE).SingleOrDefault();

            //var description = _CalenderMasterDBContext.SYSITE.Where(x => x.SYSITEID.ToString().Contains(LOCATION.ToString())).Select(x => x.DESCRIP).ToList();

            //var SiteId = long.Parse(CMV.LOCATION);
            List<string> siteIdList = _CalenderMasterDBContext.SYSITE.Select(v => v.SYSITEID.ToString()).ToList();
            //string[] locationValues = string.Join(",", LOCATION).Split(',');
            //var description = _CalenderMasterDBContext.SYSITE.Where(x => x.SYSITEID.ToString().Contains(locationValues.ToString())).Select(x => x.DESCRIP).ToList();

            string[] locationValues = string.Join(",", LOCATION).Split(',');
            Int64[] locationIds1 = Array.ConvertAll(locationValues, Int64.Parse);


            //List<string> myList = new List<string>();

            var iColl = (from data in _CalenderMasterDBContext.CALENDARMASTERHEADER
                         where locationValues.Any(location => siteIdList.Contains(location))
                         select data).ToList();
            var descriptions = new List<string>();
            var Desc = new List<string>();

            foreach (var obj in iColl)
            {
                //descriptions = (from v in _CalenderMasterDBContext.SYSITE
                //                   where obj.LOCATION.Contains(v.SYSITEID.ToString()) && v.ACTIVE == 1
                //                   select v.DESCRIP).ToList();

                descriptions = (from v in _CalenderMasterDBContext.SYSITE where locationIds1.Contains(v.SYSITEID) && v.ACTIVE == 1 select v.DESCRIP).ToList();

                Desc = descriptions.Select(DESCRIP => DESCRIP?.Substring(DESCRIP.IndexOf('-') + 1)).ToList();
            }


            PAM.SRNO = CMV.SRNO;
            PAM.SelectedFinancialYear = CMV.FINANCIALYEAR;
            PAM.CALENDER = CMV.CALENDER;
            PAM.CALENDER_BLOB = CMV.CALENDER_BLOB;
            PAM.LOCATION = CMV.LOCATION;
            PAM.LOCATIONDESC = string.Join(", ", Desc);
            //PAM.SelectedLocationDesc = myList.ToArray();
            //PAM.LOCATIONDESC = CMV.LOCATION;
            //PAM.LOCATIONDESC = Location.DESCRIP.Substring(Location.DESCRIP.IndexOf('-') + 1).ToString();
            PAM.APP_AUTH1 = CMV.APP_AUTH1;
            PAM.APP_AUTH1_NAME = (CMV.APP_AUTH1 + " - " + APP_AUTH1_NAME.FIRSTNAME + " " + APP_AUTH1_NAME.LASTNAME).ToString();
            var status1 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.APPAUTH_ECODE == CMV.APP_AUTH1 && x.CAL_MAS_SRNO == CMV.SRNO && x.APP_LEVEL == 1).Select(s => s.STATUS).SingleOrDefault();
            if (status1 == 0 || status1 == null)
            {
                PAM.APPAUTH1_REMARKS = " - Pending";
            }
            else if (status1 == 2)
            {
                PAM.APPAUTH1_REMARKS = " - Reject";
            }
            else
            {
                PAM.APPAUTH1_REMARKS = " - Approved";
            }
            PAM.APP_AUTH2 = CMV.APP_AUTH2;
            PAM.APP_AUTH2_NAME = (CMV.APP_AUTH2 + " - " + APP_AUTH2_NAME.FIRSTNAME + " " + APP_AUTH2_NAME.LASTNAME).ToString();
            var status2 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.APPAUTH_ECODE == CMV.APP_AUTH2 && x.CAL_MAS_SRNO == CMV.SRNO && x.APP_LEVEL == 2).Select(s => s.STATUS).SingleOrDefault();
            if (status2 == 0 || status2 == null)
            {
                PAM.APPAUTH2_REMARKS = " - Pending";
            }
            else if (status2 == 2)
            {
                PAM.APPAUTH2_REMARKS = " - Reject";
            }
            else
            {
                PAM.APPAUTH2_REMARKS = " - Approved";
            }
            PAM.CALENDER_UPLOAD = CMV.CALENDER_UPLOAD;
            //PAM.FinancialYears = new List<string>();
            PAM.FinancialYears = GetFinancialYears(2022, 3);
            PAM.CREATED_BY = (CMV.CREATED_BY + " - " + CREATED_BY_USER.FIRSTNAME + " " + CREATED_BY_USER.LASTNAME).ToString();
            PAM.FrmCREATED_DATE = CMV.CREATED_DATE.Value.ToString("dd-MMM-yyyy");
            //PAM.CREATED_DATE = CMV.CREATED_DATE;
            PAM.STATUS = CMV.STATUS;

            //PAM.PROCESSID = CPM.PROCESSID;
            //PAM.CM_PROCESS_MST = new ProcessMstViewModel
            //{
            //    PROCESS_NAME = CPM.CM_PROCESS_MST.PROCESS_NAME,
            //};
            //PAM.STATUS = CPM.STATUS;
            //PAM.CREATED_DATE = CPM.CREATED_DATE;
            //PAM.User_Name = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.CREATED_BY).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "(" + s.ADEMPCODE + ")").SingleOrDefault();
            //PAM.SUBJECT = CommonRepository.HtmlToText(CPM.SUBJECT);
            //PAM.BRIEF = CommonRepository.HtmlToText(CPM.BRIEF);
            ////PAM.DESCRIPTION = CommonRepository.HtmlToText(CPM.DESCRIPTION);
            //PAM.DESCRIPTION = CPM.DESCRIPTION;
            //PAM.START_DATE = CPM.START_DATE.ToString("dd-MMM-yyyy");
            //PAM.END_DATE = CPM.END_DATE.ToString("dd-MMM-yyyy");

            //PAM.BANNER_NAME = CPM.BANNER_NAME;
            //PAM.BANNER_CONTENTTYPE = CPM.BANNER_CONTENTTYPE;
            //PAM.BANNER = CPM.BANNER;

            //PAM.ATTACHMENT1_NAME = CPM.ATTACHMENT1_NAME;
            //PAM.ATTACHMENT1_CONTENTTYPE = CPM.ATTACHMENT1_CONTENTTYPE;
            //PAM.ATTACHMENT1 = CPM.ATTACHMENT1;

            //PAM.ATTACHMENT2_NAME = CPM.ATTACHMENT2_NAME;
            //PAM.ATTACHMENT2_CONTENTTYPE = CPM.ATTACHMENT2_CONTENTTYPE;
            //PAM.ATTACHMENT2 = CPM.ATTACHMENT2;
            //PAM.APPAUTH1_ECODE = CPAM.APPAUTH1_ECODE;
            //var status1 = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.APPAUTH_ECODE == CPAM.APPAUTH1_ECODE && x.ATTACHMENTID == CPM.ATTACHMENTID).Select(s => s.STATUS).SingleOrDefault();
            //if (status1 == 0 || status1 == null)
            //{
            //    PAM.AppAuth1_status = " - Pending";
            //}
            //else if (status1 == 2)
            //{
            //    PAM.AppAuth1_status = " - Reject";
            //}
            //else
            //{
            //    PAM.AppAuth1_status = " - Approved";
            //}

            //PAM.APPAUTH1_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH1_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            ////PAM.APPAUTH1_User = CPAM.APPAUTH1_ECODE+" "+ CPAM.APPAUTH1_ECODE.ToString();
            //PAM.APPAUTH2_ECODE = CPAM.APPAUTH2_ECODE;
            //var status2 = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.APPAUTH_ECODE == CPAM.APPAUTH2_ECODE && x.ATTACHMENTID == CPM.ATTACHMENTID).Select(s => s.STATUS).SingleOrDefault();
            //if (status2 == 0 || status2 == null)
            //{
            //    PAM.AppAuth2_status = " - Pending";
            //}
            //else if (status2 == 2)
            //{
            //    PAM.AppAuth2_status = " - Reject";
            //}
            //else
            //{
            //    PAM.AppAuth2_status = " - Approved";
            //}
            //PAM.APPAUTH2_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH2_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            ////PAM.APPAUTH2_User = CPAM.APPAUTH2_ECODE +" "+CPAM.APPAUTH2_ECODE.ToString();
            //PAM.INITIATOR_REMARKS = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == CPM.ATTACHMENTID).Select(s => s.INITIATOR_REMARKS).SingleOrDefault();
            return PAM;
        }


        public static List<string> GetFinancialYears(int startYear, int numberOfYears)
        {
            List<string> financialYears = new List<string>();


            // Generate financial years (e.g., "2009-2010", "2010-2011", ...)
            for (int i = 0; i < numberOfYears; i++)
            {
                int year = startYear + i;
                string financialYear = $"{year}-{(year + 1).ToString("D4")}";
                financialYears.Add(financialYear);
            }

            return financialYears;
        }
        public Int16 UpdateProcessAttachment_Trn(CalenderMasterViewModel PAM)
        {
            Int16 retVal = 0;
            using (var transaction = _CalenderMasterDBContext.Database.BeginTransaction())
            {
                try
                {

                    //var status = 0;
                    //var iList = _CalenderMasterDBContext.CALENDERMASTER.Where(x => x.SRNO == PAM.SRNO).ToList();

                    //foreach (var obj in iList)
                    //{
                    //    if (obj.STATUS == 1)
                    //    {
                    //        status = 1;
                    //        break;
                    //    }

                    //}
                    //if (status != 1)
                    //{

                    CALENDARMASTERHEADER CPT = new CALENDARMASTERHEADER();

                    CPT = _CalenderMasterDBContext.CALENDARMASTERHEADER.Find(PAM.SRNO);
                    CPT.CALENDER = PAM.CALENDER;
                    CPT.LOCATION = PAM.LOCATION;
                    CPT.FINANCIALYEAR = PAM.SelectedFinancialYear.ToString();
                    CPT.STATUS = 2;//WIP
                                   //CPT.DESCRIPTION = CommonRepository.TextToHtml(PAM.DESCRIPTION);
                                   //CPT.STATUS = true;
                    if (PAM.CALENDER_BLOB != null)
                    {
                        CPT.CALENDER_UPLOAD = PAM.CALENDER_UPLOAD;
                        CPT.CALENDER_CONTENTTYPE = PAM.CALENDER_CONTENTTYPE;
                        CPT.CALENDER_BLOB = PAM.CALENDER_BLOB;
                    }
                    CPT.APP_AUTH1 = PAM.APP_AUTH1;
                    CPT.APP_AUTH2 = PAM.APP_AUTH2;

                    CPT.MODIFIED_DATE = DateTime.Now;
                    CPT.MODIFIED_BY = PAM.MODIFIED_BY;
                    _CalenderMasterDBContext.Entry(CPT).State = EntityState.Modified;
                    _CalenderMasterDBContext.SaveChanges();

                    /// Save for AttachmentApproval       
                    //CreativeMasterViewModel AAVM = new CreativeMasterViewModel();
                    //AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                    //AAVM.INITIATED_BY = CPT.CREATED_BY;
                    //AAVM.APPAUTH1_ECODE = PAM.APPAUTH1_ECODE;
                    //AAVM.APPAUTH2_ECODE = PAM.APPAUTH2_ECODE;
                    //AAVM.INITIATOR_REMARKS = PAM.INITIATOR_REMARKS;
                    //SaveAttachmentApproval_Trn(AAVM);

                    CALENDARMASTERAPPHIS CPTM = new CALENDARMASTERAPPHIS();
                    CPTM = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Find(PAM.SRNO);

                    //List<CreativeMasterViewModel> iList = new List<CreativeMasterViewModel>();
                    var temp = (from userdata in _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(m => m.CAL_MAS_SRNO == PAM.SRNO)
                                select new
                                {
                                    CALENDERMAPPINGID = userdata.CALENDERMAPPINGID,
                                    CAL_MAS_SRNO = userdata.CAL_MAS_SRNO,
                                }
                           ).ToList();


                    if (PAM.APP_AUTH1 != null)
                    {
                        CPTM = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Find(temp[0].CALENDERMAPPINGID);
                        CPTM.CAL_MAS_SRNO = CPT.SRNO;
                        CPTM.STATUS = 0;
                        CPTM.APPAUTH_ECODE = PAM.APP_AUTH1;
                        //CPTM.APPAUTH_DATE = DateTime.Now;
                        //CPTM.APPAUTH_REMARKS = PAM.INITIATOR_REMARKS;
                        CPTM.MODIFIEDBY = long.Parse(PAM.MODIFIED_BY);
                        CPTM.MODIFIEDDATE = DateTime.Now;
                        CPTM.APP_LEVEL = 1;
                        _CalenderMasterDBContext.Entry(CPTM).State = EntityState.Modified;
                        // _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                        _CalenderMasterDBContext.SaveChanges();
                    }
                    if (PAM.APP_AUTH2 != null)
                    {
                        CPTM = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Find(temp[1].CALENDERMAPPINGID);
                        CPTM.CAL_MAS_SRNO = CPT.SRNO;
                        //CPTM.STATUS = 0;
                        CPTM.APPAUTH_ECODE = PAM.APP_AUTH2;
                        //CPTM.APPAUTH_DATE = DateTime.Now;
                        //CPTM.APPAUTH_REMARKS = PAM.INITIATOR_REMARKS;
                        CPTM.MODIFIEDBY = long.Parse(PAM.MODIFIED_BY);
                        CPTM.MODIFIEDDATE = DateTime.Now;
                        CPTM.APP_LEVEL = 2;
                        _CalenderMasterDBContext.Entry(CPTM).State = EntityState.Modified;
                        //_CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                        _CalenderMasterDBContext.SaveChanges();
                    }
                    transaction.Commit();
                    retVal = 1;

                    //}
                    //else
                    //{
                    //    retVal = 0;
                    //}

                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            FileViewModel flvm = new FileViewModel();
            CALENDARMASTERHEADER CCPT;
            //var id1 = long.Parse(id);
            CCPT = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(c => c.SRNO == id).SingleOrDefault();
            if (CCPT != null)
            {
                if (type == "CALENDER")
                {
                    flvm.FileName = CCPT.CALENDER_UPLOAD;
                    flvm.FileContentType = CCPT.CALENDER_CONTENTTYPE;
                    flvm.File = CCPT.CALENDER_BLOB;
                }

            }

            return flvm;
        }

        public CalenderMasterViewModel GetCalenderMSTRequestById(long id)
        {
            CalenderMasterViewModel PAM = new CalenderMasterViewModel();
            CALENDARMASTERHEADER CPM = new CALENDARMASTERHEADER();
            //CALENDERMASTERMAPPINGTRN CPAM = new CALENDERMASTERMAPPINGTRN();
            CALENDARMASTERAPPHIS CPAMT = new CALENDARMASTERAPPHIS();
            CALENDARMASTERAPPHIS CPAMT1 = new CALENDARMASTERAPPHIS();
            //CPM = _CalenderMasterDBContext.CM_PROCESSATTACHMENT_TRN.Include("CM_PROCESS_MST").Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            CPM = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.SRNO == id).SingleOrDefault();
            //CPAM = _CalenderMasterDBContext.CALENDERMASTERMAPPINGTRN.Where(x => x.CAL_MAS_SRNO == id).SingleOrDefault();
            CPAMT = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == id && x.APPAUTH_ECODE == CPM.APP_AUTH1).FirstOrDefault();
            CPAMT1 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == id && x.APPAUTH_ECODE == CPM.APP_AUTH2).FirstOrDefault();



            PAM.SRNO = CPM.SRNO;
            PAM.STATUS = CPM.STATUS;
            PAM.CREATED_DATE = CPM.CREATED_DATE;
            PAM.CREATED_BY1 = Int64.Parse(CPM.CREATED_BY);
            PAM.CREATED_BY = _CalenderMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == PAM.CREATED_BY1).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            PAM.FINANCIALYEAR = CPM.FINANCIALYEAR;
            PAM.CALENDER = CPM.CALENDER;
            //var LOCATION = CPM.LOCATION;

            string[] locationValues = string.Join(",", CPM.LOCATION).Split(',');
            Int64[] locationIds1 = Array.ConvertAll(locationValues, Int64.Parse);
            var descriptions = new List<string>();
            descriptions = (from v in _CalenderMasterDBContext.SYSITE where locationIds1.Contains(v.SYSITEID) && v.ACTIVE == 1 select v.DESCRIP).ToList();
            var Desc = new List<string>();
            Desc = descriptions.Select(DESCRIP => DESCRIP?.Substring(DESCRIP.IndexOf('-') + 1)).ToList();
            PAM.LOCATION = string.Join(" || ", Desc);

            //var LOCATION = long.Parse(CPM.LOCATION);
            //var LOCATION1 = _CalenderMasterDBContext.SYSITE.Where(x => x.SYSITEID == LOCATION).Select(x => x.DESCRIP).SingleOrDefault().ToString();
            //PAM.LOCATION = LOCATION1.Substring(LOCATION1.IndexOf('-') + 1);
            PAM.CALENDER_UPLOAD = CPM.CALENDER_UPLOAD;
            PAM.CALENDER_CONTENTTYPE = CPM.CALENDER_CONTENTTYPE;
            PAM.CALENDER_BLOB = CPM.CALENDER_BLOB;
            PAM.APP_AUTH1 = CPM.APP_AUTH1;
            PAM.APP_AUTH1_NAME = _CalenderMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.APP_AUTH1).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();

            PAM.APP_AUTH2 = CPM.APP_AUTH2;
            PAM.APP_AUTH2_NAME = _CalenderMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.APP_AUTH2).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();

            PAM.CALENDERMASTERMAPPINGTRN = (from CPAMTDetail in _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(d => d.CAL_MAS_SRNO == CPM.SRNO)
                                            join _AppEmp in _CalenderMasterDBContext.ADEMPLOYEE on CPAMTDetail.APPAUTH_ECODE equals _AppEmp.ADEMPCODE
                                            select new CALENDERMASTERMAPPINGTRNViewModel
                                            {
                                                APP_LEVEL = CPAMTDetail.APP_LEVEL,
                                                CAL_MAS_SRNO = CPAMTDetail.CAL_MAS_SRNO,
                                                APPAUTH_ECODE = CPAMTDetail.APPAUTH_ECODE,
                                                APPAUTH_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + "- [" + _AppEmp.ADEMPCODE + "]",
                                                APPAUTH_DATE = CPAMTDetail.APPAUTH_DATE,
                                                APPAUTH_REMARKS = CPAMTDetail.APPAUTH_REMARKS,
                                                STATUS = CPAMTDetail.STATUS,
                                            }).OrderBy(x => x.APP_LEVEL).ToList();

            return PAM;

        }
        public short CalenderMasterAppr(CALENDERMASTERMAPPINGTRNViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = 0;
            using (var transaction = _CalenderMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    CALENDARMASTERAPPHIS DPAH = new CALENDARMASTERAPPHIS();
                    CALENDARMASTERAPPHIS DPAH1 = new CALENDARMASTERAPPHIS();
                    CALENDARMASTERAPPHIS DPAH2 = new CALENDARMASTERAPPHIS();
                    CALENDARMASTERHEADER PAT = new CALENDARMASTERHEADER();
                    if (PHVM.CAL_MAS_SRNO > 0)
                    {
                        DPAH = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == PHVM.CAL_MAS_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.STATUS == 0).FirstOrDefault();

                        if (DPAH != null)
                        {
                            DPAH.APPAUTH_DATE = DateTime.Now;
                            DPAH.APPAUTH_REMARKS = PHVM.APPAUTH_REMARKS;
                            DPAH.MODIFIEDBY = PHVM.MODIFIEDBY;
                            DPAH.MODIFIEDDATE = DateTime.Now;
                            DPAH.STATUS = PHVM.STATUS;
                            _CalenderMasterDBContext.Entry(DPAH).State = EntityState.Modified;
                            _CalenderMasterDBContext.SaveChanges();
                        }

                        var stsl1 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == PHVM.CAL_MAS_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.CALENDERMAPPINGID == DPAH.CALENDERMAPPINGID).Select(x => x.CALENDERMAPPINGID).FirstOrDefault();

                        var MAPPINGATTACHMENTID = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == PHVM.CAL_MAS_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.CALENDERMAPPINGID == DPAH.CALENDERMAPPINGID).Select(x => x.CALENDERMAPPINGID + 1).FirstOrDefault();



                        DPAH1 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == PHVM.CAL_MAS_SRNO && x.CALENDERMAPPINGID == MAPPINGATTACHMENTID).FirstOrDefault();

                        PAT = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.SRNO == PHVM.CAL_MAS_SRNO).FirstOrDefault();
                        DPAH2 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == PHVM.CAL_MAS_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.APP_LEVEL == 2).FirstOrDefault();

                        var CalH1 = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.FINANCIALYEAR == PAT.FINANCIALYEAR && x.LOCATION == PAT.LOCATION && x.CALENDER == PAT.CALENDER).Select(x => x.STATUS == 1).Count();
                        if (CalH1 >= 1)
                        {
                            if (DPAH2 != null)
                            {
                                //if (DPAH.STATUS != 2)
                                if (DPAH2.STATUS == 1)
                                {
                                    var CalH2 = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.FINANCIALYEAR == PAT.FINANCIALYEAR && x.LOCATION == PAT.LOCATION && x.CALENDER == PAT.CALENDER && x.STATUS == 1).ToList();

                                    var itemsToUpdate = from item in CalH2
                                                        where item.STATUS == 1
                                                        select item;

                                    foreach (var item in itemsToUpdate)
                                    {
                                        item.STATUS = 0; // De-Active
                                        _CalenderMasterDBContext.Entry(item).State = EntityState.Modified;
                                        _CalenderMasterDBContext.SaveChanges();
                                    }
                                }
                            }

                        }

                        if (PAT != null)
                        {
                            if (DPAH != null)
                            {
                                if (DPAH1 != null)
                                {
                                    if (DPAH.STATUS != 2)
                                    {
                                        DPAH1.STATUS = 0;//Pending
                                        _CalenderMasterDBContext.Entry(DPAH1).State = EntityState.Modified;
                                        _CalenderMasterDBContext.SaveChanges();
                                    }
                                }

                                if (DPAH.STATUS == 2)
                                {
                                    PAT.STATUS = 0;//De-Active
                                    _CalenderMasterDBContext.Entry(PAT).State = EntityState.Modified;
                                    _CalenderMasterDBContext.SaveChanges();
                                }
                            }

                        }

                        if (PAT != null)
                        {
                            if (DPAH2 != null)
                            {

                                if (DPAH2.STATUS == 1)
                                {
                                    PAT.STATUS = 1;//Active                                    
                                }
                                else if (DPAH2.STATUS == 2)
                                {
                                    PAT.STATUS = 0;//De-Active                                   
                                }
                                else
                                {
                                    PAT.STATUS = 2;//WIP                                    
                                }
                                _CalenderMasterDBContext.Entry(PAT).State = EntityState.Modified;
                                _CalenderMasterDBContext.SaveChanges();
                            }

                        }
                        //var PAT1 = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.SRNO == PHVM.CAL_MAS_SRNO).FirstOrDefault();

                        retVal = 1;
                    }


                    transaction.Commit();
                }

                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public short DeactiveReqById(long id)
        {

            using (var transaction = _CalenderMasterDBContext.Database.BeginTransaction())
            {
                short retVal = 0;
                try
                {
                    CALENDARMASTERHEADER CPM = new CALENDARMASTERHEADER();
                    CPM = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.SRNO == id && x.STATUS == 1).SingleOrDefault();

                    if (CPM != null)
                    {
                        CPM.STATUS = 0;
                        _CalenderMasterDBContext.Entry(CPM).State = EntityState.Modified;
                        _CalenderMasterDBContext.SaveChanges();
                    }
                    retVal = 1;
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }

        }

        public ADEMPLOYEE GetEmpDtl(string Adempcode)
        {
            int Emp_Code = Int32.Parse(Adempcode);
            var Email_id = _CalenderMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Emp_Code && x.ACTIVE == 1).FirstOrDefault();
            return Email_id;
        }

        public ADEMPLOYEE GetNextAppECodeEmail(int srno)
        {
            var app_auth2 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == srno && x.APP_LEVEL == 2).Select(x => x.APPAUTH_ECODE).FirstOrDefault();
            var Email_id = _CalenderMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == app_auth2 && x.ACTIVE == 1).FirstOrDefault();
            return Email_id;
        }
        public CALENDARMASTERAPPHIS GetApp_Lvl(int srno, int userId)
        {
            var App_Level1 = _CalenderMasterDBContext.CALENDARMASTERAPPHIS.Where(x => x.CAL_MAS_SRNO == srno && x.APPAUTH_ECODE == userId).FirstOrDefault();
            return App_Level1;
        }

        public CALENDARMASTERHEADER GetHeaderDetails(int srno)
        {
            var HeaderDTL = _CalenderMasterDBContext.CALENDARMASTERHEADER.Where(x => x.SRNO == srno).FirstOrDefault();
            return HeaderDTL;
        }

    }
}
