using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class EmployeeLoginRepository
    {
        private EPortalDBContext _empLoginDBContext;
        private EPortalDGITDBContext _communicationDBContext;
        private readonly IConfiguration _configuration;
        private CommonRepository _cr;

        private readonly ISessionService _sessionService;
        public EmployeeLoginRepository(ISessionService sessionService, EPortalDBContext empLoginDBContext, EPortalDGITDBContext communicationDBContext, CommonRepository cr, IConfiguration configuration)
        {
            _sessionService = sessionService;
            _empLoginDBContext = empLoginDBContext;
            _communicationDBContext = communicationDBContext;
            _cr = cr;
            _configuration = configuration;
        }
        public string Check_MailLogin(string strUserID)
        {
            string resultout1 = string.Empty;
            Employee_Details emp = GetEmpdetail(strUserID);
            //HttpContext.Current.Session["Employee"] = emp;                        
            _sessionService.Set("Employee", emp);

            return resultout1;
        }



        public string Check_AutoLogin(string strUserID, string strToken, string usertype)
        {
            string resultout1 = string.Empty;
            int resultout2 = 0;
            int type = Convert.ToInt16(usertype);

            //try
            //{
            var sql = "BEGIN SPROC_AUTOLOGIN_GET(:USERID_IN,:TOKENID_IN, :APPLICATIONID, :LOGINEMPCODE_OUT, :LOGINRESULT_OUT); END;";
            OracleParameter lOGINEMPCODE_OUT = new OracleParameter("lOGINEMPCODE_OUT", OracleDbType.Int64) { Direction = ParameterDirection.Output };
            OracleParameter lOGINRESULT_OUT = new OracleParameter("lOGINRESULT_OUT", OracleDbType.Int64) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                    new OracleParameter("USERID_IN", OracleDbType.Varchar2) { Value = strUserID },
                    new OracleParameter("TOKENID_IN", OracleDbType.Varchar2) { Value = strToken },
                    new OracleParameter("APPLICATIONID", OracleDbType.Varchar2) { Value = usertype },
                    lOGINEMPCODE_OUT,
                    lOGINRESULT_OUT
            };
            _empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);
            resultout1 = Convert.ToString(lOGINEMPCODE_OUT?.Value);
            resultout2 = Convert.ToInt16(lOGINRESULT_OUT.Value.ToString());

            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

            if (resultout2 == 1 && !string.IsNullOrEmpty(resultout1))
            {
                Employee_Details emp = GetEmpdetail(resultout1);
                //HttpContext.Current.Session["Employee"] = emp;
                _sessionService.Set("Employee", emp);

                Int64 lngloginid = Convert.ToInt64(emp.Employee_Code);
                string pass = _empLoginDBContext.ADEMPLOGIN.Where(m => m.ADEMPCODE == lngloginid && m.SYUSERTYPEID == type && m.ACTIVE == 1).FirstOrDefault().PASSWORD;
                //HttpContext.Current.Session["pass"] = pass;
                _sessionService.Set("pass", pass);
            }
            return resultout1 + "#" + resultout2;
        }


        public int Check_EmpLogin(string strUserID, string strPWD, string Client_IP, string usertype)
        {
            string strpasswordeny = EncryptionClass.EncodePasswordToBase64(strPWD);
            int resultout = 0;
            long userid = Convert.ToInt64(strUserID);
            int type = Convert.ToInt16(usertype);

            //ObjectParameter objparameter = new ObjectParameter("lOGINRESULT_OUT", typeof(Int64));         
            //_empLoginDBContext.SPROC_VALIDATELOGIN_GET(userid, strpasswordeny, type, Client_IP, objparameter);
            //resultout = Convert.ToInt16(objparameter.Value);

            //try
            //{

            //USERID_IN IN NUMBER,
            //USER_PASSWORD_IN IN VARCHAR2,
            //USER_TYPE_IN IN NUMBER,
            //USER_IP IN VARCHAR2 DEFAULT NULL,

            //LOGINRESULT_OUT OUT NUMBER


            var sql = "BEGIN SPROC_VALIDATELOGIN_GET(:USERID_IN,:USER_PASSWORD_IN, :USER_TYPE_IN, :USER_IP, :lOGINRESULT_OUT); END;";
            OracleParameter lOGINEMPCODE_OUT = new OracleParameter("lOGINRESULT_OUT", OracleDbType.Int64) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                    new OracleParameter("USERID_IN", OracleDbType.Varchar2) { Value = userid },
                    new OracleParameter("USER_PASSWORD_IN", OracleDbType.Varchar2) { Value = strpasswordeny },
                    new OracleParameter("USER_TYPE_IN", OracleDbType.Varchar2) { Value = type },
                    new OracleParameter("USER_IP", OracleDbType.Varchar2) { Value = Client_IP },
                    lOGINEMPCODE_OUT
            };
            _empLoginDBContext.Database.ExecuteSqlRaw(sql, parameters);
            resultout = Convert.ToInt16(lOGINEMPCODE_OUT?.Value.ToString());

            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}



            if (resultout == 1)
            {
                Employee_Details emp = GetEmpdetail(strUserID);
                //HttpContext.Current.Session["Employee"] = emp;
                _sessionService.Set("Employee", emp);
            }
            return resultout;
        }

        public int Check_Switch_EmpLogin(string strUserID, string Client_IP, string usertype)
        {
            int resultout = 0;

            try
            {
                //string strpasswordeny = EncryptionClass.EncodePasswordToBase64(strPWD);               
                long userid = Convert.ToInt64(strUserID);
                int type = Convert.ToInt16(usertype);


                var sql = "BEGIN SPROC_SWITCH_VALIDATELOGIN_GET(:USERID_IN, :USER_TYPE_IN, :USER_IP, :lOGINRESULT_OUT); END;";
                OracleParameter lOGINEMPCODE_OUT = new OracleParameter("lOGINRESULT_OUT", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                var parameters = new OracleParameter[]
                {

                 new OracleParameter("USERID_IN", OracleDbType.Varchar2) { Value = userid },
                 new OracleParameter("USER_TYPE_IN", OracleDbType.Varchar2) { Value = type },
                 new OracleParameter("USER_IP", OracleDbType.Varchar2) { Value = Client_IP },
                 lOGINEMPCODE_OUT
                };
                _empLoginDBContext.Database.ExecuteSqlRaw(sql, parameters);
                resultout = Convert.ToInt16(lOGINEMPCODE_OUT?.Value.ToString());

                //if (resultout == 1)
                //{
                if (!string.IsNullOrEmpty(strUserID))
                {
                    Employee_Details emp = GetEmpdetail(strUserID);
                    _sessionService.Set("Employee", emp);
                }
                //}
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(strUserID))
                {
                    Employee_Details emp = GetEmpdetail(strUserID);
                    _sessionService.Set("Employee", emp);
                }
            }

            return resultout;
        }

        public Employee_Details GetEmpdetail(string strUserID)
        {
            long lngsykiid = Convert.ToInt64(_empLoginDBContext.SYKI.Where(s => s.ACTIVE == 1).FirstOrDefault().SYKIID);

            long userid = Convert.ToInt64(strUserID);

            List<Employee_Details> objdesgdtl;

            try
            {
                //objdesgdtl = (from lu in _empLoginDBContext.ADLOGINUSER.Where(d => d.ADEMPCODE == userid)
                //                                     join emp in _empLoginDBContext.ADEMPLOYEE on lu.ADEMPCODE equals emp.ADEMPCODE into tempemp
                //                                     from emp in tempemp.DefaultIfEmpty()
                //                                     join dds in _empLoginDBContext.VW_ASSOCIATELVLDETAILS.Where(g => g.SYKI == lngsykiid) on lu.ADEMPCODE equals dds.ADEMPCODE into tempdds
                //                                     from dds in tempdds.DefaultIfEmpty()
                //                                     join tdds in _empLoginDBContext.ADEMPDIVDEPTSECT.Where(g => g.SYKI == lngsykiid) on lu.ADEMPCODE equals tdds.ADEMPCODE into edds
                //                                     from tdds in edds.DefaultIfEmpty()
                //                                     join dsg in _empLoginDBContext.ADDESIGNATION on dds.ADDESIGNATIONID equals dsg.ADDESIGNATIONID into tempdsg
                //                                     from dsg in tempdsg.DefaultIfEmpty()
                //                                     select new Employee_Details
                //                                     {
                //                                         _ECode = lu.ADEMPCODE,
                //                                         _EName = lu.FIRSTNAME + " " + lu.LASTNAME,
                //                                         _EFirstName = lu.FIRSTNAME,
                //                                         _ELastName = lu.LASTNAME,
                //                                         _OpDesc = dds.OPERATION,
                //                                         _OpId = dds.OPERATIONID,
                //                                         _DivDesc = dds.DIVISION,
                //                                         _DivId = dds.DIVISIONID,
                //                                         _DepDesc = dds.DEPARTMENT,
                //                                         _DepId = dds.DEPARTMENTID,
                //                                         _SecDescrip = dds.SECTION,
                //                                         _SecId = dds.SECTIONID,
                //                                         _Desig = dsg.DESCRIP,
                //                                         _DesigId = dds.ADDESIGNATIONID,
                //                                         _FnDesigId = dds.ADFUNCTIONALDESIGNATIONID,
                //                                         _FnDesig = string.IsNullOrEmpty(dds.FUNCTIONALDESIGNATION) ? "NORMAL" : dds.FUNCTIONALDESIGNATION,
                //                                         _SiteId = ((dds.SYSITEID == null) || (dds.SYSITEID == 0)) ? lu.SYSITEID : dds.SYSITEID,
                //                                         _EmailId = lu.EMAILID,
                //                                         _PancardNo = emp.PANCARDNO,
                //                                         _PlantId = dds.SYPLANTID,
                //                                         _DOJ = lu.REGDATE,
                //                                         _DOB = emp.DOB,
                //                                         _MobileNo = lu.TMOBILE,
                //                                         _ConfirmationDate = emp.CONFIRMATIONDATE,
                //                                         _Gender = emp.GENDER,
                //                                         ORGLVL = tdds.ADORGLEVELID,
                //                                         _DOM = emp.MARITALDATE,
                //                                         _EmpLtype = emp.EMPLTYPE


                //                                     }
                //              ).ToList();



                objdesgdtl = (
                               from lu in _empLoginDBContext.ADLOGINUSER
                                           .Where(d => d.ADEMPCODE == userid)
                               join emp in _empLoginDBContext.ADEMPLOYEE on lu.ADEMPCODE equals emp.ADEMPCODE into tempemp
                               from emp in tempemp.DefaultIfEmpty()

                               join dds in _empLoginDBContext.VW_ASSOCIATELVLDETAILS
                                           .Where(g => g.SYKI == lngsykiid) on lu.ADEMPCODE equals dds.ADEMPCODE into tempdds
                               from dds in tempdds.DefaultIfEmpty()

                               join tdds in _empLoginDBContext.ADEMPDIVDEPTSECT
                                           .Where(g => g.SYKI == lngsykiid) on lu.ADEMPCODE equals tdds.ADEMPCODE into edds
                               from tdds in edds.DefaultIfEmpty()

                               join dsg in _empLoginDBContext.ADDESIGNATION on dds.ADDESIGNATIONID equals dsg.ADDESIGNATIONID into tempdsg
                               from dsg in tempdsg.DefaultIfEmpty()

                               select new Employee_Details
                               {
                                   _ECode = lu.ADEMPCODE,
                                   _EName = string.Concat(lu.FIRSTNAME ?? "", " ", lu.LASTNAME ?? ""),
                                   _EFirstName = lu.FIRSTNAME,
                                   _ELastName = lu.LASTNAME,
                                   _OpDesc = dds.OPERATION,
                                   _OpId = dds.OPERATIONID,
                                   _DivDesc = dds.DIVISION,
                                   _DivId = dds.DIVISIONID,
                                   _DepDesc = dds.DEPARTMENT,
                                   _DepId = dds.DEPARTMENTID,
                                   _SecDescrip = dds.SECTION,
                                   _SecId = dds.SECTIONID,
                                   _Desig = dsg.DESCRIP,
                                   _DesigId = dds.ADDESIGNATIONID,
                                   _FnDesigId = dds.ADFUNCTIONALDESIGNATIONID,
                                   _FnDesig = string.IsNullOrEmpty(dds.FUNCTIONALDESIGNATION) ? "NORMAL" : Convert.ToString(dds.FUNCTIONALDESIGNATION),
                                   _SiteId = (dds.SYSITEID == null || dds.SYSITEID == 0) ? lu.SYSITEID : dds.SYSITEID,
                                   _EmailId = lu.EMAILID,
                                   _PancardNo = emp.PANCARDNO,
                                   _PlantId = dds.SYPLANTID,
                                   _DOJ = lu.REGDATE,
                                   _DOB = emp.DOB,
                                   _MobileNo = lu.TMOBILE,
                                   _ConfirmationDate = emp.CONFIRMATIONDATE,
                                   _Gender = emp.GENDER,
                                   ORGLVL = tdds.ADORGLEVELID,
                                   _DOM = emp.MARITALDATE,
                                   _EmpLtype = emp.EMPLTYPE
                               }
                           ).ToList();



                return objdesgdtl.FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw;
            }

            return null;
        }

        public List<ContentViewModel> GetContent(ContentViewModel objsearch)
        {
            try
            {
                //CM_PROCESS_MSTMSTPROCESSID": invalid identifier
                //CM_PROCESS_MST.MSTPROCESSID


                DateTime datefilter = DateTime.Today.Date;
                //List<ContentViewModel> objlist = new List<ContentViewModel>();
                //var objdata = (from data in _empLoginDBContext.CM_PROCESSATTACHMENT_TRN
                //               where data.STATUS == objsearch.STATUS
                //               && data.CM_PROCESS_MST.MSTPROCESSID == objsearch.PROCESSID
                //               && datefilter >= data.START_DATE && datefilter <= data.END_DATE
                //               select new
                //               {
                //                   ATTACHMENTID = data.ATTACHMENTID,
                //                   SUBJECT = data.SUBJECT,
                //                   BRIEF = data.BRIEF,
                //                   BANNER = data.BANNER,
                //                   BANNER_CONTENTTYPE = data.BANNER_CONTENTTYPE,
                //                   BANNER_NAME = data.BANNER_NAME,
                //                   DESCRIPTION = data.DESCRIPTION,
                //                   POPUPHEADER = data.CM_PROCESS_MST.PROCESS_BANNER,
                //                   POPUPHEADER_CONTENTTYPE = data.CM_PROCESS_MST.BANNER_CONTENTTYPE,
                //                   POPUPHEADER_NAME = data.CM_PROCESS_MST.BANNER_NAME,
                //                   START_DATE = data.START_DATE,
                //                   FUNCTIONAL_DESIGNATION = data.FUNCTIONAL_DESIGNATION,
                //                   SITENAME = data.SITE,
                //                   DESIGNATION = data.DESIGNATION
                //               }).ToList();

                var objdata = (from data in _empLoginDBContext.CM_PROCESSATTACHMENT_TRN.Where(d => d.STATUS == objsearch.STATUS)
                               join data1 in _empLoginDBContext.CM_PROCESS_MST on data.PROCESSID equals data1.MSTPROCESSID into data1TempData
                               from data1 in data1TempData.DefaultIfEmpty()
                               where data1.MSTPROCESSID == objsearch.PROCESSID
                               && datefilter >= data.START_DATE && datefilter <= data.END_DATE  //added 
                               select new
                               {
                                   ATTACHMENTID = data.ATTACHMENTID,
                                   SUBJECT = data.SUBJECT,
                                   BRIEF = data.BRIEF,
                                   BANNER = data.BANNER,
                                   BANNER_CONTENTTYPE = data.BANNER_CONTENTTYPE,
                                   BANNER_NAME = data.BANNER_NAME,
                                   DESCRIPTION = data.DESCRIPTION,
                                   POPUPHEADER = data1.PROCESS_BANNER,
                                   POPUPHEADER_CONTENTTYPE = data1.BANNER_CONTENTTYPE,
                                   POPUPHEADER_NAME = data1.BANNER_NAME,
                                   START_DATE = data.START_DATE,
                                   FUNCTIONAL_DESIGNATION = data.FUNCTIONAL_DESIGNATION,
                                   SITENAME = data.SITE,
                                   DESIGNATION = data.DESIGNATION
                               }).ToList();

                //var objdata = from data in _empLoginDBContext.CM_PROCESSATTACHMENT_TRN
                //              join data1 in _empLoginDBContext.CM_PROCESS_MST
                //              on data.PROCESSID equals data1.MSTPROCESSID
                //              where data.STATUS = objsearch.STATUS
                //              select new
                //              {

                //              };



                //CommonRepository _cr = new CommonRepository();
                //Employee_Details emp = (Employee_Details)HttpContext.Current.Session["Employee"];
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");


                List<ContentViewModel> iList = new List<ContentViewModel>();
                foreach (var o in objdata)
                {
                    if (string.IsNullOrEmpty(o.FUNCTIONAL_DESIGNATION) && string.IsNullOrEmpty(o.DESIGNATION) && string.IsNullOrEmpty(o.SITENAME))
                    {
                        ContentViewModel obj = new ContentViewModel();
                        obj.ATTACHMENTID = o.ATTACHMENTID;
                        obj.SUBJECT = o.SUBJECT;
                        obj.BRIEF = o.BRIEF;
                        obj.BANNER = o.BANNER;
                        obj.BANNER_CONTENTTYPE = o.BANNER_CONTENTTYPE;
                        obj.BANNER_NAME = o.BANNER_NAME;
                        obj.DESCRIPTION = o.DESCRIPTION;
                        obj.POPUPHEADER = o.POPUPHEADER;
                        obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                        obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                        obj.START_DATE = o.START_DATE;
                        //obj.START_DATE = System.DateTime.Now;
                        obj.FUNCTIONAL_DESIGNATION = o.FUNCTIONAL_DESIGNATION;
                        obj.SITE = o.SITENAME;
                        obj.DESIGNATION = o.DESIGNATION;
                        iList.Add(obj);
                    }
                    else if (_cr.GetParameterValue("DESG_STAFFHOMEPAGE_VALIDATION").Split(',').Contains(emp.Designation_Id))
                    {
                        bool desg = true; bool site = true;
                        if (!string.IsNullOrEmpty(o.DESIGNATION))
                            desg = false;
                        if (!string.IsNullOrEmpty(o.SITENAME))
                            site = false;

                        if (!string.IsNullOrEmpty(o.DESIGNATION) && o.DESIGNATION.TrimEnd(',').Split(',').Contains(emp.Designation_Id))
                            desg = true;
                        if (!string.IsNullOrEmpty(o.SITENAME) && o.SITENAME.TrimEnd(',').Split(',').Contains(emp._SiteId.ToString()))
                            site = true;

                        if (desg && site)
                        {
                            ContentViewModel obj = new ContentViewModel();
                            obj.ATTACHMENTID = o.ATTACHMENTID;
                            obj.SUBJECT = o.SUBJECT;
                            obj.BRIEF = o.BRIEF;
                            obj.BANNER = o.BANNER;
                            obj.BANNER_CONTENTTYPE = o.BANNER_CONTENTTYPE;
                            obj.BANNER_NAME = o.BANNER_NAME;
                            obj.DESCRIPTION = o.DESCRIPTION;
                            obj.POPUPHEADER = o.POPUPHEADER;
                            obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                            obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                            obj.START_DATE = o.START_DATE;
                            //obj.START_DATE = DateTime.Now;
                            obj.FUNCTIONAL_DESIGNATION = o.FUNCTIONAL_DESIGNATION;
                            obj.SITE = o.SITENAME;
                            obj.DESIGNATION = o.DESIGNATION;
                            iList.Add(obj);
                        }
                    }
                    else
                    {
                        bool fundesg = true;
                        bool desg = true;
                        bool site = true;

                        if (!string.IsNullOrEmpty(o.FUNCTIONAL_DESIGNATION))
                            fundesg = false;
                        if (!string.IsNullOrEmpty(o.DESIGNATION))
                            desg = false;
                        if (!string.IsNullOrEmpty(o.SITENAME))
                            site = false;

                        if (!string.IsNullOrEmpty(o.FUNCTIONAL_DESIGNATION) && o.FUNCTIONAL_DESIGNATION.TrimEnd(',').Split(',').Contains(emp.Functional_Designation_Id))
                            fundesg = true;
                        if (!string.IsNullOrEmpty(o.DESIGNATION) && o.DESIGNATION.TrimEnd(',').Split(',').Contains(emp.Designation_Id))
                            desg = true;
                        if (!string.IsNullOrEmpty(o.SITENAME) && o.SITENAME.TrimEnd(',').Split(',').Contains(emp._SiteId.ToString()))
                            site = true;

                        if (fundesg && desg && site)
                        {
                            ContentViewModel obj = new ContentViewModel();
                            obj.ATTACHMENTID = o.ATTACHMENTID;
                            obj.SUBJECT = o.SUBJECT;
                            obj.BRIEF = o.BRIEF;
                            obj.BANNER = o.BANNER;
                            obj.BANNER_CONTENTTYPE = o.BANNER_CONTENTTYPE;
                            obj.BANNER_NAME = o.BANNER_NAME;
                            obj.DESCRIPTION = o.DESCRIPTION;
                            obj.POPUPHEADER = o.POPUPHEADER;
                            obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                            obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                            //obj.START_DATE = o.START_DATE;
                            obj.START_DATE = DateTime.Now;
                            obj.FUNCTIONAL_DESIGNATION = o.FUNCTIONAL_DESIGNATION;
                            obj.SITE = o.SITENAME;
                            obj.DESIGNATION = o.DESIGNATION;
                            iList.Add(obj);
                        }

                    }
                }
                return iList;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public List<ContentViewModel> GetContent()
        {

            DateTime CurDate = DateTime.Now.Date;
            List<ContentViewModel> objlist = new List<ContentViewModel>();
            List<ContentViewModel> iList = new List<ContentViewModel>();
            //CommonRepository _cr = new CommonRepository();
            //Employee_Details emp = (Employee_Details)HttpContext.Current.Session["Employee"];
            int count = _empLoginDBContext.ASR_BENEVOLENT_MST.Where(a => a.TO_DT >= CurDate).Count();
            var count1 = _empLoginDBContext.ASR_BENEVOLENT_MST.Where(a => a.TO_DT >= DateTime.Now);
            //int count = _empLoginDBContext.ASR_BENEVOLENT_MST.Count();
            if (count > 0)
            {
                //ContentViewModel obj = new ContentViewModel();
                //obj.ATTACHMENTID = 1;
                //obj.SUBJECT = "Contribution for the Demised Employee";
                //obj.DESCRIPTION = "Employee working in ours company have died. You may give your voluntary contribution for this.";
                //obj.START_DATE = DateTime.Now;
                //iList.Add(obj);
                long strKIID = (long)_empLoginDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
                var objdata = (from e in _empLoginDBContext.ADEMPLOYEE
                               join data in _empLoginDBContext.ASR_BENEVOLENT_MST on e.ADEMPCODE equals data.EMPLOYEECODE
                               join vw in _empLoginDBContext.VW_ASSOCIATELVLDETAILS.Where(a => a.SYKI == strKIID) on data.EMPLOYEECODE equals vw.ADEMPCODE
                               join desig in _empLoginDBContext.ADDESIGNATION on vw.ADDESIGNATIONID equals desig.ADDESIGNATIONID into JoinDesig
                               from _Desig in JoinDesig.DefaultIfEmpty()
                               join loc in _empLoginDBContext.SYSITE on vw.SYSITEID equals loc.SYSITEID into JoinLoc
                               from _Loc in JoinLoc.DefaultIfEmpty()
                               where CurDate >= data.FROM_DT && CurDate <= data.TO_DT
                               select new
                               {
                                   ATTACHMENTID = 1,
                                   SUBJECT = "Additional Benevolent Contribution (additional with existing Rs.500)",
                                   BRIEF = "Additional Benevolent Contribution to help the demised Associate’s Family",
                                   FIRSTNAME = e.FIRSTNAME,
                                   LASTNAME = e.LASTNAME,
                                   DEMISEDATE = data.DEMISEDATE,
                                   LOCATION = _Loc == null ? "" : _Loc.DESCRIP,
                                   DESIGNATION = _Desig == null ? "" : _Desig.DESCRIP,
                                   OPERATION = vw.OPERATION,
                                   DEPARTMENT = vw.DEPARTMENT,
                                   START_DATE = DateTime.Now,
                                   POPUPHEADER_NAME = "Benevolent",
                                   //BRIEF = "Contribution",
                                   //DESCRIPTION = "It is a great sadness that Our valued colleague " + e.FIRSTNAME.ToString() + " " + e.LASTNAME.ToString() + " has passed away on " + data.DEMISEDATE.ToString() + ". We will definitely miss his support and good heart. Please keep support " + e.FIRSTNAME.ToString() + "'s family in your thoughts as they go through this difficult time.",

                                   ////BANNER =,
                                   //BANNER_CONTENTTYPE = "BANNER_CONTENTTYPE",
                                   //BANNER_NAME = "BANNER_NAME",

                                   //POPUPHEADER = "Benevolent",
                                   //POPUPHEADER_CONTENTTYPE = "POPUPHEADER_CONTENTTYPE",
                                   //POPUPHEADER_NAME = "Benevolent",


                                   //FUNCTIONAL_DESIGNATION = "FUNCTIONAL_DESIGNATION:",
                                   //SITENAME = "Benvole",
                                   //DESIGNATION = "DESIGNATION"

                               }).ToList();


                //CommonRepository _cr = new CommonRepository();
                //Employee_Details emp = (Employee_Details)HttpContext.Current.Session["Employee"];
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");

                foreach (var o in objdata)
                {
                    //if (string.IsNullOrEmpty(o.FUNCTIONAL_DESIGNATION) && string.IsNullOrEmpty(o.DESIGNATION) && string.IsNullOrEmpty(o.SITENAME))
                    //{
                    string qStr = @"""Additional Benevolent Contribution""";
                    ContentViewModel obj = new ContentViewModel();
                    obj.ATTACHMENTID = o.ATTACHMENTID;
                    obj.SUBJECT = o.SUBJECT;
                    obj.DESCRIPTION = "Dear Associate, It is very heart - breaking & mourning moment that one of our valued associate Mr./Ms." + o.FIRSTNAME + " " + o.LASTNAME + ", " +
                                   "Location: " + o.LOCATION + ", Designation: " + o.DESIGNATION + " , Operation: " + o.OPERATION + ", Department: " + o.DEPARTMENT + ", has passed away on " + o.DEMISEDATE.ToString("dd-MMM-yyyy") + ". His/her contribution will always be remembered. " +
                                   "It is very huge loss to HMSI & his/her family. We wouldn't be able to reduce the kind of pain, his/her family is going through, " +
                                   "but we can support them a little bit by contributing towards " + qStr + " (as per your choice).";
                    //"If you wish to contribute, kindly click on the below button and give your consent by mentioning the contribution amount.";

                    obj.START_DATE = o.START_DATE;
                    obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                    obj.BRIEF = o.BRIEF;
                    ////obj.BANNER = o.BANNER;
                    //obj.BANNER_CONTENTTYPE = o.BANNER_CONTENTTYPE;
                    //obj.BANNER_NAME = o.BANNER_NAME;
                    //obj.POPUPHEADER = o.POPUPHEADER;
                    //obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;

                    //obj.FUNCTIONAL_DESIGNATION = o.FUNCTIONAL_DESIGNATION;
                    //obj.SITE = o.SITENAME;
                    //obj.DESIGNATION = o.DESIGNATION;
                    iList.Add(obj);
                    // }

                }
            }

            return iList;
        }
        public ContentViewModel GetVideo(ContentViewModel objsearch)
        {
            List<ContentViewModel> objlist = new List<ContentViewModel>();
            var objdata = (from data in _empLoginDBContext.CM_PROCESSATTACHMENT_TRN
                           where data.ATTACHMENTID == objsearch.ATTACHMENTID
                           select new ContentViewModel
                           {
                               ATTACHMENTID = data.ATTACHMENTID,
                               ATTACHMENT1 = data.ATTACHMENT1,
                               ATTACHMENT1_CONTENTTYPE = data.ATTACHMENT1_CONTENTTYPE,
                               ATTACHMENT1_NAME = data.ATTACHMENT1_NAME
                           }).ToList().FirstOrDefault();

            return (ContentViewModel)objdata;
        }
        public ContentViewModel GetAttachement2(ContentViewModel objsearch)
        {
            List<ContentViewModel> objlist = new List<ContentViewModel>();
            var objdata = (from data in _empLoginDBContext.CM_PROCESSATTACHMENT_TRN
                           where data.ATTACHMENTID == objsearch.ATTACHMENTID
                           select new ContentViewModel
                           {
                               ATTACHMENTID = data.ATTACHMENTID,
                               ATTACHMENT2 = data.ATTACHMENT2,
                               ATTACHMENT2_CONTENTTYPE = data.ATTACHMENT2_CONTENTTYPE,
                               ATTACHMENT2_NAME = data.ATTACHMENT2_NAME
                           }).ToList().FirstOrDefault();

            return (ContentViewModel)objdata;
        }

        //var objTRN = (from data in _empLoginDBContext.CM_PRESIDENTMSG_TRN.Where(d => d.STATUS == objsearch.STATUS)
        //              join data1 in _empLoginDBContext.CM_PRESIDENT_MST on data.PRESIDENT_ID equals data1.PRESIDENT_ID into data1TempData
        //              from data1 in data1TempData.DefaultIfEmpty()
        //              select new PresidentDesk_TRNViewModel
        //              {
        //                  BRIEF = data.BRIEF,
        //                  MESSAGE = data.MESSAGE,
        //                  PRESIDENTMSG_ID = data.PRESIDENTMSG_ID,
        //                  ATTACHMENT_NAME = data.ATTACHMENT_NAME,
        //                  ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE,
        //                  PRESIDENT_ID = data1.PRESIDENT_ID,
        //              }).ToList();


        public List<ContentViewModel> GetPopupContent(ContentViewModel objsearch)
        {
            List<ContentViewModel> objlist = new List<ContentViewModel>();
            var objdata = (from data in _empLoginDBContext.CM_PROCESSATTACHMENT_TRN.Where(a => a.ATTACHMENTID == objsearch.ATTACHMENTID)
                           join data1 in _empLoginDBContext.CM_PROCESS_MST on data.PROCESSID equals data1.MSTPROCESSID into data1TempData
                           from data1 in data1TempData.DefaultIfEmpty()
                           select new ContentViewModel
                           {
                               ATTACHMENTID = data.ATTACHMENTID,
                               SUBJECT = data.SUBJECT,
                               BRIEF = data.BRIEF,
                               BANNER = data.BANNER,
                               BANNER_CONTENTTYPE = data.BANNER_CONTENTTYPE,
                               BANNER_NAME = data.BANNER_NAME,
                               DESCRIPTION = data.DESCRIPTION,
                               ATTACHMENT1_NAME = data.ATTACHMENT1_NAME,
                               ATTACHMENT2_NAME = data.ATTACHMENT2_NAME,
                               //POPUPHEADER = data.CM_PROCESS_MST.PROCESS_BANNER,
                               //POPUPHEADER_CONTENTTYPE = data.CM_PROCESS_MST.BANNER_CONTENTTYPE,
                               //POPUPHEADER_NAME = data.CM_PROCESS_MST.BANNER_NAME,
                               POPUPHEADER = data1.PROCESS_BANNER,
                               POPUPHEADER_CONTENTTYPE = data1.BANNER_CONTENTTYPE,
                               POPUPHEADER_NAME = data1.BANNER_NAME,
                               START_DATE = data.START_DATE
                           }).ToList();

            return (List<ContentViewModel>)objdata;
        }


        public string UpdateEmployeePassword(long uid, string strPassword, long modby, string Usertype)
        {
            //ObjectParameter objresultout = new ObjectParameter("RESULT_OUT", typeof(Int64));
            //ObjectParameter objerrormsg = new ObjectParameter("ERRMSG_OUT", typeof(string));
            //_empLoginDBContext.SPROC_UPDATEENCPASSWORD(uid.ToString(), Usertype, modby.ToString(), strPassword, objresultout, objerrormsg);
            //return Convert.ToString(objresultout.Value) + "#" + Convert.ToString(objerrormsg.Value);

            var sql = "BEGIN SPROC_UPDATEENCPASSWORD(:ADEMPCODE_IN,:ADUSERTYPE_IN, :ADMODBY_IN, :PASSWORD_IN, :RESULT_OUT, :ERRMSG_OUT); END;";
            OracleParameter rESULT_OUT = new OracleParameter("RESULT_OUT", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
            OracleParameter eRRMSG_OUT = new OracleParameter("ERRMSG_OUT", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                    new OracleParameter("ADEMPCODE_IN", OracleDbType.Varchar2) { Value = Convert.ToString(uid) },
                    new OracleParameter("ADUSERTYPE_IN", OracleDbType.Varchar2) { Value = Usertype },
                    new OracleParameter("ADMODBY_IN", OracleDbType.Varchar2) { Value = Convert.ToString(modby) },
                    new OracleParameter("PASSWORD_IN", OracleDbType.Varchar2) { Value = strPassword },
                    rESULT_OUT,
                    eRRMSG_OUT
            };
            _empLoginDBContext.Database.ExecuteSqlRaw(sql, parameters);

            int resultInt = Convert.ToInt16(rESULT_OUT?.Value.ToString());
            string resultString = Convert.ToString(eRRMSG_OUT?.Value.ToString());

            return Convert.ToString(Convert.ToInt16(rESULT_OUT?.Value.ToString())) + "#" + Convert.ToString(eRRMSG_OUT?.Value.ToString());

        }

        public string UpdateEmployeeMobileno(long uid, string strmobile, long modby)
        {
            //ObjectParameter objresultout = new ObjectParameter("RESULT_OUT", typeof(Int64));
            //ObjectParameter objerrormsg = new ObjectParameter("ERRMSG_OUT", typeof(string));
            //_empLoginDBContext.SPROC_UPDATEMOBILENO(uid.ToString(), strmobile, modby.ToString(), objresultout, objerrormsg);
            //return Convert.ToString(objresultout.Value) + "#" + Convert.ToString(objerrormsg.Value);

            var sql = "BEGIN SPROC_UPDATEMOBILENO(:ADEMPCODE_IN,:MOBILENO_IN, :ADMODBY_IN, :RESULT_OUT, :ERRMSG_OUT); END;";
            OracleParameter rESULT_OUT = new OracleParameter("RESULT_OUT", OracleDbType.Int64) { Direction = ParameterDirection.Output };
            OracleParameter eRRMSG_OUT = new OracleParameter("ERRMSG_OUT", OracleDbType.Varchar2,500) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                    new OracleParameter("ADEMPCODE_IN", OracleDbType.Varchar2) { Value = Convert.ToString(uid) },
                    new OracleParameter("MOBILENO_IN", OracleDbType.Varchar2) { Value = strmobile },
                    new OracleParameter("ADMODBY_IN", OracleDbType.Varchar2) { Value = Convert.ToString(modby) },
                    rESULT_OUT,
                    eRRMSG_OUT
            };
            _empLoginDBContext.Database.ExecuteSqlRaw(sql, parameters);
            int resultInt = Convert.ToInt16(rESULT_OUT?.Value.ToString());
            string rESULT_OUTVal = Convert.ToString(eRRMSG_OUT?.Value.ToString());


            return Convert.ToString(Convert.ToInt16(rESULT_OUT?.Value.ToString())) + "#" + Convert.ToString(eRRMSG_OUT?.Value.ToString());

        }


        public List<ContentViewModel> GetCommunicationContent()
        {
            DateTime datefilter = DateTime.Today.Date;
            List<ContentViewModel> objlist = new List<ContentViewModel>();
            var objdata = (from data in _communicationDBContext.CM_COMMUNICATION_HDR
                           join cateMaster in _communicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals cateMaster.COMMCATEGORYID
                           where data.STATUS == 1
                           && data.PROCESS_STATUS == 2
                           && datefilter >= data.START_DATE && datefilter <= data.END_DATE
                           select new
                           {
                               ATTACHMENTID = data.COMMUNICATIONID,
                               SUBJECT = data.SUBJECT,
                               BRIEF = data.SUBJECT,
                               DESCRIPTION = data.CONTENT,
                               //POPUPHEADER = data.CM_PROCESS_MST.PROCESS_BANNER,
                               //POPUPHEADER_CONTENTTYPE = data.CM_PROCESS_MST.BANNER_CONTENTTYPE,
                               POPUPHEADER_NAME = "Communication",//cateMaster.CATEGORY_NAME,
                               START_DATE = data.START_DATE,
                               CREATED_DATE = data.CREATED_DATE,
                               FUNCTIONAL_DESIGNATION = data.FUNCTIONAL_DESIGNATION,
                               SITENAME = data.SITE,
                               DESIGNATION = data.DESIGNATION,
                               OPERATION = data.OPERATION,
                               DIVISION = data.DIVISION,
                               DEPARTMENT = data.DEPARTMENT,
                           }).ToList();

            //CommonRepository _cr = new CommonRepository();
            //Employee_Details emp = (Employee_Details)HttpContext.Current.Session["Employee"];
            Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
            List<ContentViewModel> iList = new List<ContentViewModel>();
            foreach (var o in objdata)
            {
                if (string.IsNullOrEmpty(o.FUNCTIONAL_DESIGNATION) && string.IsNullOrEmpty(o.DESIGNATION) && string.IsNullOrEmpty(o.SITENAME) && string.IsNullOrEmpty(o.OPERATION) && string.IsNullOrEmpty(o.DIVISION) && string.IsNullOrEmpty(o.DEPARTMENT))
                {
                    ContentViewModel obj = new ContentViewModel();
                    obj.ATTACHMENTID = o.ATTACHMENTID;
                    obj.SUBJECT = o.SUBJECT;
                    obj.BRIEF = o.BRIEF;
                    obj.DESCRIPTION = o.DESCRIPTION;
                    //obj.POPUPHEADER = o.POPUPHEADER;
                    //obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                    obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                    obj.START_DATE = o.START_DATE == null ? o.CREATED_DATE : Convert.ToDateTime(o.START_DATE);
                    obj.FUNCTIONAL_DESIGNATION = o.FUNCTIONAL_DESIGNATION;
                    obj.SITE = o.SITENAME;
                    obj.DESIGNATION = o.DESIGNATION;
                    obj.OPERATION = o.OPERATION;
                    obj.DIVISION = o.DIVISION;
                    obj.DEPARTMENT = o.DEPARTMENT;
                    iList.Add(obj);
                }
                else if (_cr.GetParameterValue("DESG_STAFFHOMEPAGE_VALIDATION").Split(',').Contains(emp.Designation_Id))
                {
                    bool desg = true; bool site = true;
                    bool operation = true;
                    bool division = true;
                    bool department = true;
                    if (!string.IsNullOrEmpty(o.DESIGNATION))
                        desg = false;
                    if (!string.IsNullOrEmpty(o.SITENAME))
                        site = false;
                    if (!string.IsNullOrEmpty(o.OPERATION))
                        operation = false;
                    if (!string.IsNullOrEmpty(o.DIVISION))
                        division = false;
                    if (!string.IsNullOrEmpty(o.DEPARTMENT))
                        department = false;

                    if (!string.IsNullOrEmpty(o.DESIGNATION) && o.DESIGNATION.TrimEnd(',').Split(',').Contains(emp.Designation_Id))
                        desg = true;
                    if (!string.IsNullOrEmpty(o.SITENAME) && o.SITENAME.TrimEnd(',').Split(',').Contains(emp._SiteId.ToString()))
                        site = true;
                    if (!string.IsNullOrEmpty(o.OPERATION) && o.OPERATION.TrimEnd(',').Split(',').Contains(emp._OpId.ToString()))
                        operation = true;
                    if (!string.IsNullOrEmpty(o.DIVISION) && o.DIVISION.TrimEnd(',').Split(',').Contains(emp._DivId.ToString()))
                        division = true;
                    if (!string.IsNullOrEmpty(o.DEPARTMENT) && o.DEPARTMENT.TrimEnd(',').Split(',').Contains(emp._DepId.ToString()))
                        department = true;

                    if (desg && site && operation && division && department)
                    {
                        ContentViewModel obj = new ContentViewModel();
                        obj.ATTACHMENTID = o.ATTACHMENTID;
                        obj.SUBJECT = o.SUBJECT;
                        obj.BRIEF = o.BRIEF;
                        obj.DESCRIPTION = o.DESCRIPTION;
                        //obj.POPUPHEADER = o.POPUPHEADER;
                        //obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                        obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                        obj.START_DATE = o.START_DATE == null ? o.CREATED_DATE : Convert.ToDateTime(o.START_DATE);
                        obj.FUNCTIONAL_DESIGNATION = o.FUNCTIONAL_DESIGNATION;
                        obj.SITE = o.SITENAME;
                        obj.DESIGNATION = o.DESIGNATION;
                        obj.OPERATION = o.OPERATION;
                        obj.DIVISION = o.DIVISION;
                        obj.DEPARTMENT = o.DEPARTMENT;
                        iList.Add(obj);
                    }
                }
                else
                {
                    bool fundesg = true;
                    bool desg = true;
                    bool site = true;
                    bool operation = true;
                    bool division = true;
                    bool department = true;

                    if (!string.IsNullOrEmpty(o.FUNCTIONAL_DESIGNATION))
                        fundesg = false;
                    if (!string.IsNullOrEmpty(o.DESIGNATION))
                        desg = false;
                    if (!string.IsNullOrEmpty(o.SITENAME))
                        site = false;
                    if (!string.IsNullOrEmpty(o.OPERATION))
                        operation = false;
                    if (!string.IsNullOrEmpty(o.DIVISION))
                        division = false;
                    if (!string.IsNullOrEmpty(o.DEPARTMENT))
                        department = false;

                    if (!string.IsNullOrEmpty(o.FUNCTIONAL_DESIGNATION) && o.FUNCTIONAL_DESIGNATION.TrimEnd(',').Split(',').Contains(emp.Functional_Designation_Id))
                        fundesg = true;
                    if (!string.IsNullOrEmpty(o.DESIGNATION) && o.DESIGNATION.TrimEnd(',').Split(',').Contains(emp.Designation_Id))
                        desg = true;
                    if (!string.IsNullOrEmpty(o.SITENAME) && o.SITENAME.TrimEnd(',').Split(',').Contains(emp._SiteId.ToString()))
                        site = true;
                    if (!string.IsNullOrEmpty(o.OPERATION) && o.OPERATION.TrimEnd(',').Split(',').Contains(emp._OpId.ToString()))
                        operation = true;
                    if (!string.IsNullOrEmpty(o.DIVISION) && o.DIVISION.TrimEnd(',').Split(',').Contains(emp._DivId.ToString()))
                        division = true;
                    if (!string.IsNullOrEmpty(o.DEPARTMENT) && o.DEPARTMENT.TrimEnd(',').Split(',').Contains(emp._DepId.ToString()))
                        department = true;

                    if (fundesg && desg && site && operation && division && department)
                    {
                        ContentViewModel obj = new ContentViewModel();
                        obj.ATTACHMENTID = o.ATTACHMENTID;
                        obj.SUBJECT = o.SUBJECT;
                        obj.BRIEF = o.BRIEF;
                        obj.DESCRIPTION = o.DESCRIPTION;
                        //obj.POPUPHEADER = o.POPUPHEADER;
                        //obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                        obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                        obj.START_DATE = o.START_DATE == null ? o.CREATED_DATE : Convert.ToDateTime(o.START_DATE);
                        obj.FUNCTIONAL_DESIGNATION = o.FUNCTIONAL_DESIGNATION;
                        obj.SITE = o.SITENAME;
                        obj.DESIGNATION = o.DESIGNATION;
                        obj.OPERATION = o.OPERATION;
                        obj.DIVISION = o.DIVISION;
                        obj.DEPARTMENT = o.DEPARTMENT;
                        iList.Add(obj);
                    }

                }
            }
            return iList;
        }

        //public DataSet EmailApprovalGet(long strEmpCode)
        //{
        //    //ConnectionString objCnStr = new ConnectionString();
        //    string strCn = _conn.getConnectingString();
        //    using (OracleConnection objCn = new OracleConnection())
        //    {
        //        objCn.ConnectionString = strCn;
        //        try
        //        {
        //            objCn.Open();
        //            OracleCommand objCmd = new OracleCommand();
        //            objCmd.Connection = objCn;
        //            objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_EMAILURL_GET";
        //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
        //            objCmd.Parameters.Add("CUR_WINDOWLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            objCmd.BindByName = true;
        //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
        //            DataSet objDs = new DataSet();
        //            objAdr.Fill(objDs);
        //            return objDs;
        //        }
        //        finally
        //        {
        //            if (objCn != null)
        //            {
        //                objCn.Close();
        //            }
        //        }
        //    }
        //}


        public List<SPROC_EMAILURL_GET> EmailApprovalGet(long strEmpCode)
        {
            //List<SPROC_EMAILURL_GET> testList = new List<SPROC_EMAILURL_GET>();

            var sql = "BEGIN PKG_COMMONMETHOD.SPROC_EMAILURL_GET(:EMPCODE_IN,:CUR_WINDOWLIST); END;";

            var resultOut = new OracleParameter("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };

            var parameters = new OracleParameter[]
            {

                    new OracleParameter("EMPCODE_IN", OracleDbType.Int32) { Value = strEmpCode },
                    resultOut
            };

            var result = _empLoginDBContext.Set<SPROC_EMAILURL_GET>()
                 .FromSqlRaw(sql, parameters)
                 //.ToListAsync();
                 .ToList();


            //testList = result;

            return result;
        }

        public void StoreJwtUserLog(string userName, string jwtToken, string refreshToken)
        {
            using var transaction = _empLoginDBContext.Database.BeginTransaction();
            try
            {
                // Step 1: Create and save a new RefreshToken
                var newRefreshToken = new RefreshToken
                {
                    Token = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]))
                };

                _empLoginDBContext.REFRESHTOKENS.Add(newRefreshToken);
                _empLoginDBContext.SaveChanges(); // Save the RefreshToken to get its generated ID

                // Step 2: Create and save the JwtUserLog with the associated RefreshTokenId
                var jwtUserLog = new JwtUserLog
                {
                    UserName = userName,
                    RefreshTokenId = newRefreshToken.Id // Set the RefreshTokenId to link the two entities
                };

                _empLoginDBContext.JWTUSERLOGS.Add(jwtUserLog);
                _empLoginDBContext.SaveChanges(); // Save the JwtUserLog

                // Commit the transaction if both operations succeed
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction if any error occurs
                transaction.Rollback();
                throw; // Optionally rethrow the exception
            }
        }
    }
}
