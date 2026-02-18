using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using System.Web;
using ePortal.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ePortal.Infrastructure.Repositories
{
    public class CommonRepository
    {

        private EPortalDBContext _empLoginDBContext;
        public CommonRepository(EPortalDBContext empLoginDBContext)
        {
            _empLoginDBContext = empLoginDBContext;
        }
        public String GetParameterValue(string strParmaName)
        {
            var SelectedParameter = (from v in _empLoginDBContext.SYPARAMETERS
                                     where v.PARAMNAME == strParmaName
                                     select v
                                  ).ToList();


            return (SelectedParameter.FirstOrDefault().PARAMVALUE);
        }


        public async Task<string> GetMandatorytoFilledPage(decimal Ecode, decimal Usertype, decimal ISSSOLOGIN)
        {
            string resultout = string.Empty;
            string sql = "BEGIN SPROC_MANADATORY_CHECK(:USERID_IN, :USER_TYPE_IN, :ISSSOLOGIN, :LOGINRESULT_OUT); END;";

            try
            {
                var lOGINEMPCODE_OUT = new OracleParameter("LOGINRESULT_OUT", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new OracleParameter[]
                {
                    new OracleParameter("USERID_IN", OracleDbType.Decimal) { Value = Ecode },
                    new OracleParameter("USER_TYPE_IN", OracleDbType.Decimal) { Value = Usertype },
                    new OracleParameter("ISSSOLOGIN", OracleDbType.Decimal) { Value = ISSSOLOGIN },
                    lOGINEMPCODE_OUT
                };
                await _empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);

                resultout = lOGINEMPCODE_OUT.Value?.ToString();
            }
            catch (Exception ex)
            {
                // Consider logging
                throw;
            }

            return resultout;
        }

        public MenuViewModel GetMenuURL(MenuViewModel objmodal)
        {
            decimal menuid = Convert.ToDecimal(objmodal.MenuId);
            var oMenulist = (from data in _empLoginDBContext.ADMENU_MST.Where(m => m.MENU_ID == menuid)
                             select data).ToList();
            //select new ADMENU_MST
            //{
            //    MENU_ID = data.MENU_ID,
            //    MENU_PARENT_ID = data.MENU_PARENT_ID,
            //    MENU_LEVEL =data.MENU_LEVEL,
            //    MENU_URL = data.MENU_URL,
            //    MENU_TOOLTIP = data.MENU_TOOLTIP,
            //    MENU_TEXT = data.MENU_TEXT,
            //    MENU_TARGET = data.MENU_TARGET
            //}).ToList();

            var oadmenulist = (from odata in oMenulist
                               select new MenuViewModel
                               {
                                   MenuId = Convert.ToInt64(odata.MENU_ID),
                                   URL = odata.MENU_URL,
                                   MenuLevel = Convert.ToInt16(odata.MENU_LEVEL),
                                   MenuParentId = Convert.ToInt64(odata.MENU_PARENT_ID),
                                   MenuTarget = Convert.ToString(odata.MENU_TARGET),
                                   Title = odata.MENU_TEXT
                               }).ToList();

            return oadmenulist.FirstOrDefault();
        }

        ////public static string HtmlEncode(string text)
        ////{
        ////    string result;
        ////    using (StringWriter sw = new StringWriter())
        ////    {
        ////        var x = new HtmlTextWriter(sw);
        ////        x.WriteEncodedText(text);
        ////        result = sw.ToString();
        ////    }
        ////    return result;

        ////}
        ////
        public static string TextToHtml(string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            text = HttpUtility.HtmlEncode(text);
            text = WebUtility.HtmlEncode(text);
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            //text = text.Replace(" ", "&nbsp;");
            //text = text.Replace("  ", " &nbsp;");
            text = text.Replace("<", "&lt;");
            text = text.Replace(">", "&gt;");
            text = text.Replace("&", "&amp;");
            text = text.Replace("”", "&rdquo;");
            text = text.Replace("“", "&ldquo;");
            text = text.Replace("–", "&ndash;");
            text = text.Replace("‘", "&lsquo;");
            text = text.Replace("’", "rsquo;");
            text = text.Replace("'", "&apos;");
            text = text.Replace("\"", "&quot;");
            foreach (char c in text)
            {
                if (c > 127) // special chars
                    sb.Append(String.Format("&#{0};", (int)c));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static string HtmlToText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = HttpUtility.HtmlDecode(text);
                text = WebUtility.HtmlDecode(text);
                //text = text.Replace(" &nbsp;", "  ");
                //text = text.Replace("&nbsp;", " ");
                text = text.Replace("<br>\r\n", "\r");
                text = text.Replace("\n<br>\n", "<br>");
                text = text.Replace("\n<br>", "<br>");
                text = text.Replace("<br>\n", "<br>");
                text = text.Replace("\n<br/>\n", "<br/>");
                text = text.Replace("\n<br/>", "<br/>");
                text = text.Replace("<br/>\n", "<br/>");
                text = text.Replace("<br>", "\r");
                text = text.Replace("\r", "\n");
                text = text.Replace("<br/>", "\n");
                text = text.Replace("&lt;", "<");
                text = text.Replace("&gt;", ">");
                text = text.Replace("&amp;", "&");
                text = text.Replace("&rdquo;", "”");
                text = text.Replace("&ldquo;", "“");
                text = text.Replace("&ndash;", "–");
                text = text.Replace("&lsquo;", "‘");
                text = text.Replace("rsquo;", "’");
                text = text.Replace("&apos;", "'");
                text = text.Replace("&#39;", "'");
                text = text.Replace("&quot;", "\"");
            }

            return text;
        }

        public async Task<string> ISAUTH_ACCESS(string Ecode, string url)
        {
            //ObjectParameter objmsg = new ObjectParameter("ISAUTH_IN", typeof(Int64));
            //_empLoginDBContext.SPROC_CHECKRIGHTS(Ecode, url, objmsg);
            //return Convert.ToString(objmsg.Value);

            string resultout = string.Empty;

            var sql = "BEGIN SPROC_CHECKRIGHTS(:ADEMPCODE_IN,:URL_IN, :ISAUTH_IN); END;";
            OracleParameter isAuth_OUT = new OracleParameter("ISAUTH_IN", OracleDbType.Int64) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                    new OracleParameter("ADEMPCODE_IN", OracleDbType.Varchar2) { Value = Ecode },
                    new OracleParameter("URL_IN", OracleDbType.Varchar2) { Value = url },
                    isAuth_OUT
            };
            await _empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);
            return Convert.ToString(isAuth_OUT.Value);

            //var outputValue = parameters[2].Value;
            //if (outputValue != DBNull.Value && outputValue != null)
            //{
            //    resultout = Convert.ToString(outputValue.ToString());
            //}
            //return resultout;
        }


        public long GetSiteIdByEmpCode(long EmpCode)
        {
            short syki = Convert.ToInt16(_empLoginDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault());
            return (long)_empLoginDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == EmpCode && x.ACTIVE == 1 && x.SYKI == syki).Select(s => s.SYSITEID).FirstOrDefault();
        }


        public UserApprovalAuthority CheckApprovalAuthority(long Empcode)
        {
            short syki = Convert.ToInt16(_empLoginDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault());
            var _obj = (from data in _empLoginDBContext.ADEMPLOYEE
                        join _VW in _empLoginDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE

                        join _SH in _empLoginDBContext.ADORGLEVELHEAD on _VW.SECTIONID equals _SH.ADORGLEVELID into _SHJoin
                        from SH in _SHJoin.Where(s => s.ISACTIVE == 1).DefaultIfEmpty()

                        join _DH in _empLoginDBContext.ADORGLEVELHEAD on _VW.DEPARTMENTID equals _DH.ADORGLEVELID into _DHJoin
                        from DH in _DHJoin.Where(h => h.ISACTIVE == 1).DefaultIfEmpty()

                        join _DV in _empLoginDBContext.ADORGLEVELHEAD on _VW.DIVISIONID equals _DV.ADORGLEVELID into _DVJoin
                        from DV in _DVJoin.Where(v => v.ISACTIVE == 1).DefaultIfEmpty()

                        join _V3 in _empLoginDBContext.ADORGCOORDINATOR on _VW.DIVISIONID equals _V3.ADORGLEVELID into _V3Join
                        from V3 in _V3Join.Where(d => d.ISACTIVE == 1).DefaultIfEmpty()

                        join _V4 in _empLoginDBContext.ADORGCOORDINATOR on _VW.OPERATIONID equals _V4.ADORGLEVELID into _V4Join
                        from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()

                        where data.ADEMPCODE == Empcode && data.ACTIVE == 1 && _VW.SYKI == syki
                        select new UserApprovalAuthority  ///// Director Approval not working
                        {
                            Empcode = data.ADEMPCODE,
                            FirstName = data.FIRSTNAME,
                            LastName = data.LASTNAME,
                            SectionManager = SH == null ? 0 : SH.ADEMPCODE,
                            DepartmentManager = DH == null ? 0 : DH.ADEMPCODE,
                            DivisionHead = DV == null ? 0 : DV.ADEMPCODE,
                            OperationHead = V3.OPHEAD == null ? V4.OPHEAD : V3.OPHEAD,
                            Coordinator = V3.COORDINATOR == null ? V4.COORDINATOR : V3.COORDINATOR,
                            EXECoordinator = V3.EXECOORDINATOR == null ? V4.EXECOORDINATOR : V3.EXECOORDINATOR,
                            Director = V3.DIRECTOR == null ? V4.DIRECTOR : V3.DIRECTOR,
                            DivisionId = _VW.DIVISIONID,
                            OperationId = _VW.OPERATIONID,
                            Director2 = V3.DIRECTOR2 == null ? V4.DIRECTOR2 : V3.DIRECTOR2,
                        }).FirstOrDefault();
            return _obj;
        }
        public UserApprovalAuthority CheckApprovalAuthorityForAsset(long Empcode, long? opMappId)
        {
            short syki = Convert.ToInt16(_empLoginDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault());
            var _obj = (from data in _empLoginDBContext.ADEMPLOYEE
                        join _VW in _empLoginDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE

                        join _SH in _empLoginDBContext.ADORGLEVELHEAD on _VW.SECTIONID equals _SH.ADORGLEVELID into _SHJoin
                        from SH in _SHJoin.Where(s => s.ISACTIVE == 1).DefaultIfEmpty()

                        join _DH in _empLoginDBContext.ADORGLEVELHEAD on _VW.DEPARTMENTID equals _DH.ADORGLEVELID into _DHJoin
                        from DH in _DHJoin.Where(h => h.ISACTIVE == 1).DefaultIfEmpty()

                        join _DV in _empLoginDBContext.ADORGLEVELHEAD on _VW.DIVISIONID equals _DV.ADORGLEVELID into _DVJoin
                        from DV in _DVJoin.Where(v => v.ISACTIVE == 1).DefaultIfEmpty()

                        join _V3 in _empLoginDBContext.ADORGCOORDINATOR on _VW.DIVISIONID equals _V3.ADORGLEVELID into _V3Join
                        from V3 in _V3Join.Where(d => d.ISACTIVE == 1).DefaultIfEmpty()

                        join _V4 in _empLoginDBContext.ADORGCOORDINATOR on (opMappId == null || opMappId == 0 ? _VW.OPERATIONID : opMappId)
                        equals _V4.ADORGLEVELID into _V4Join
                        from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()

                        where data.ADEMPCODE == Empcode && data.ACTIVE == 1 && _VW.SYKI == syki
                        select new UserApprovalAuthority
                        {
                            Empcode = data.ADEMPCODE,
                            FirstName = data.FIRSTNAME,
                            LastName = data.LASTNAME,
                            SectionManager = SH == null ? 0 : SH.ADEMPCODE,
                            DepartmentManager = DH == null ? 0 : DH.ADEMPCODE,
                            DivisionHead = DV == null ? 0 : DV.ADEMPCODE,
                            //OperationHead = V3.OPHEAD == null ? V4.OPHEAD : V3.OPHEAD,
                            Coordinator = V3.COORDINATOR == null ? 0 : V3.COORDINATOR,
                            //EXECoordinator = V3.EXECOORDINATOR == null ? V4.EXECOORDINATOR : V3.EXECOORDINATOR,
                            //Director = V3.DIRECTOR == null ? V4.DIRECTOR : V3.DIRECTOR,
                            OperationHead = V4.OPHEAD,
                            //Coordinator = V4.COORDINATOR,
                            EXECoordinator = V4.EXECOORDINATOR,
                            Director = V4.DIRECTOR,
                            DivisionId = _VW.DIVISIONID,
                            OperationId = _VW.OPERATIONID,
                            //Director2 = V3.DIRECTOR2 == null ? V4.DIRECTOR2 : V3.DIRECTOR2,
                            Director2 = V4.DIRECTOR2,
                            IsSkip = V3.COORDINATOR == null ? null : V3.ISSKIP, //// isSkip for coordinator
                            IsSkipExe = V4.EXECOORDINATOR == null ? null : V4.ISSKIP, //// isSkip for Exe. coordinator
                        }).FirstOrDefault();
            return _obj;

        }

        public List<UserApprovalAuthority> GetCoordinatorList()
        {
            var _obj = (from data in _empLoginDBContext.ADORGCOORDINATOR
                        where data.ISACTIVE == 1
                        select new UserApprovalAuthority
                        {
                            OperationHead = data.OPHEAD,
                            Coordinator = data.COORDINATOR,
                            EXECoordinator = data.EXECOORDINATOR,
                            Director = data.DIRECTOR,
                            OrgLevel = data.ADORGLEVELID,
                            IsSkip = data.ISSKIP,
                            Director2 = data.DIRECTOR2
                        }).ToList();
            return _obj;
        }

        ////public Employee_Details GetEmpDetailById(long empCode)
        ////{
        ////    var _obj = (from data in _empLoginDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
        ////                select new Employee_Details
        ////                {
        ////                    _ECode = data.ADEMPCODE,
        ////                    _EFirstName = data.FIRSTNAME,
        ////                    _ELastName = data.LASTNAME,
        ////                    _EName = data.FIRSTNAME + " " + data.LASTNAME + " [" + data.ADEMPCODE + "]",
        ////                    _EmailId = data.EMAILID
        ////                }).FirstOrDefault();
        ////    return _obj;
        ////}

        public Employee_Details GetEmpDetailById(long empCode)
        {
            short syki = Convert.ToInt16(_empLoginDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault());
            var _obj = (from data in _empLoginDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _empLoginDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _empLoginDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID into _DesgJoin
                        from DESG in _DesgJoin.DefaultIfEmpty()
                        where _VW.SYKI == syki
                        select new Employee_Details
                        {
                            _ECode = data.ADEMPCODE,
                            _EFirstName = data.FIRSTNAME,
                            _ELastName = data.LASTNAME,
                            _EName = data.FIRSTNAME + " " + data.LASTNAME + " [" + data.ADEMPCODE + "]",
                            _EmailId = data.EMAILID,
                            _Desig = DESG.DESCRIP,
                            _FnDesigId = _VW.ADFUNCTIONALDESIGNATIONID,
                            _FnDesig = _VW.FUNCTIONALDESIGNATION,
                            _DesigId = _VW.ADDESIGNATIONID
                        }).FirstOrDefault();
            return _obj;
        }

        public Employee_Details GetDirectorDetailById(long empCode, long opid, out short authtype)
        {
            var apptype = (_empLoginDBContext.DGIT_SIGNAUTHMAP.Where(x => x.OPERATIONID == opid && x.SIGNAUTHECODE == empCode && x.STATUS == 1)).FirstOrDefault();
            short syki = Convert.ToInt16(_empLoginDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault());
            var _obj = (from data in _empLoginDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _empLoginDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _empLoginDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID into _DesgJoin
                        from DESG in _DesgJoin.DefaultIfEmpty()

                        where _VW.SYKI == syki
                        select new Employee_Details
                        {
                            _ECode = data.ADEMPCODE,
                            _EFirstName = data.FIRSTNAME,
                            _ELastName = data.LASTNAME,
                            _EName = data.FIRSTNAME + " " + data.LASTNAME + " [" + data.ADEMPCODE + "]",
                            _EmailId = data.EMAILID,
                            _Desig = DESG.DESCRIP,
                            _DesigId = DESG.ADDESIGNATIONID
                        }).FirstOrDefault();
            if (apptype != null)
                authtype = 2;
            else
                authtype = 1;
            return _obj;
        }
        public Employee_Details GetEVPDetailById(long opid)
        {
            //var apptype = (_empLoginDBContext.DGIT_SIGNAUTHMAP.Where(x => x.OPERATIONID == opid && x.SIGNAUTHECODE == empCode && x.STATUS == 1)).FirstOrDefault();
            short syki = Convert.ToInt16(_empLoginDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(s => s.SYKIID).FirstOrDefault());
            var objecode = (_empLoginDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.ADDESIGNATIONID == 30 && m.OPERATIONID == opid && m.SYKI == syki && m.ACTIVE == 1)).FirstOrDefault();

            if (objecode != null)
            {
                long ecode = objecode.ADEMPCODE;
                var _obj = (from data in _empLoginDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == ecode && v.ACTIVE == 1)
                            join _VW in _empLoginDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                            join _Desg in _empLoginDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID into _DesgJoin
                            from DESG in _DesgJoin.DefaultIfEmpty()
                            where _VW.SYKI == syki
                            select new Employee_Details
                            {
                                _ECode = data.ADEMPCODE,
                                _EFirstName = data.FIRSTNAME,
                                _ELastName = data.LASTNAME,
                                _EName = data.FIRSTNAME + " " + data.LASTNAME + " [" + data.ADEMPCODE + "]",
                                _EmailId = data.EMAILID,
                                _Desig = DESG.DESCRIP,
                                _DesigId = DESG.ADDESIGNATIONID
                            }).FirstOrDefault();
                return _obj;
            }
            return null;
        }

        public long GetOperationIdByEmpCode(long EmpCode)
        {
            short syki = Convert.ToInt16(_empLoginDBContext.SYKI.Where(x => x.ACTIVE == new Decimal(1)).Select(s => s.SYKIID).FirstOrDefault());
            long? _opId = _empLoginDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == EmpCode && x.ACTIVE == 1 && x.SYKI == (long?)(long)syki).Select(s => s.OPERATIONID).FirstOrDefault();
            return Convert.ToInt64(_opId == null ? 0 : _opId);
        }

        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
        public List<DropdownList> GetEmployeeListAsPerOrgLevel(long orgLevelId, long? KIID)
        {
            List<DropdownList> data = new List<DropdownList>();
            try
            {
                var tempData = (from v in _empLoginDBContext.VW_ASSOCIATELVLDETAILS
                                join e in _empLoginDBContext.ADEMPLOYEE on v.ADEMPCODE equals e.ADEMPCODE
                                where (v.SYKI == KIID)
                                && v.ACTIVE == 1
                                && e.ACTIVE == 1
                                && new long[] { (long)v.OPERATIONID, (long)v.DIVISIONID, (long)v.DEPARTMENTID, (long)v.SECTIONID }.Contains(orgLevelId)
                                select new
                                {
                                    FirstName = e.FIRSTNAME,
                                    LastName = e.LASTNAME,
                                    EmpCode = v.ADEMPCODE,
                                }).ToList();
                data = tempData.Select(x => new DropdownList
                {
                    Text = $"{x.FirstName.Trim()} {x.LastName.Trim()}-{x.EmpCode}",
                    Value = x.EmpCode.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return data;
        }
        public List<DropdownList> GetEmployeeListAsPerOrgLevel(long[] orgLevelId, long? KIID)
        {
            List<DropdownList> data = new List<DropdownList>();
            try
            {
                var tempData = (from v in _empLoginDBContext.VW_ASSOCIATELVLDETAILS
                                join e in _empLoginDBContext.ADEMPLOYEE on v.ADEMPCODE equals e.ADEMPCODE
                                where v.SYKI == KIID
                                && v.ACTIVE == 1
                                && e.ACTIVE == 1
                                &&
                                    (
                                        orgLevelId.Contains((long)v.OPERATIONID) ||
                                        orgLevelId.Contains((long)v.DIVISIONID) ||
                                        orgLevelId.Contains((long)v.DEPARTMENTID) ||
                                        orgLevelId.Contains((long)v.SECTIONID)
                                    )
                                select new
                                {
                                    FirstName = e.FIRSTNAME,
                                    LastName = e.LASTNAME,
                                    EmpCode = v.ADEMPCODE,
                                }).ToList();
                data = tempData.Select(x => new DropdownList
                {
                    Text = $"{x.FirstName.Trim()} {x.LastName.Trim()}-{x.EmpCode}",
                    Value = x.EmpCode.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return data;
        }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End
    }
}
