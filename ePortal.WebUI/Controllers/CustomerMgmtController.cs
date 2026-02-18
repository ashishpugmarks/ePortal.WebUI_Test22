using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using ePortal.Application.APPX.Contracts;
using ePortal.Application.Contracts;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.CustomerMgmt;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using Path = System.IO.Path;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class CustomerMgmtController : Controller
    {
       
        private ICustomerMgmtService _cmService;
        private readonly IAppConfigurationService _env;
        private ISessionService _session;
        ILogger<CustomerMgmtController> _logger;
        public string filePath = "";
        private readonly IEportalESS objess;
        public CustomerMgmtController(ICustomerMgmtService customerMgmt, IAppConfigurationService appConfiguration, ISessionService sessionService, IEportalESS eportalESS, ILogger<CustomerMgmtController> logger) { 
            _cmService = customerMgmt;
            _env = appConfiguration;
            _session = sessionService;
            _logger = logger;  
            objess = eportalESS;
            filePath = serverpath.getFileUploadPath() + "CustomerMaster";
        }

        [HttpGet]
        public async Task<IActionResult> CustomerMasterData(string VMID) {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
           
            var data = new CMMASTER_DATA();
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> CustomerMasterData([FromBody]CMMASTER_DATA cMMASTER)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            cMMASTER.CreatedBy =_session.Get<string>("userID");            
           var data= _cmService.AddCustomerMasterData(cMMASTER, "Add");
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> CustomerMasterDataUpdate([FromBody] CMMASTER_DATA cMMASTER)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            cMMASTER.CreatedBy = _session.Get<string>("userID");
            var data = _cmService.AddCustomerMasterData(cMMASTER, "Update");
            return Json(data);
        }

        [HttpPost]
        public IActionResult CustomerMasterDataList([FromBody] CustomerMgmtResponse data)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<CMMASTER_DATA> dataList=new List<CMMASTER_DATA>();
            DataTable dt = _cmService.FetchCodesByGroupName(data.code);

            if (dt != null && dt.Rows.Count > 0)
            {
                dataList = dt?.AsEnumerable()
                   .Select(r => new CMMASTER_DATA
                   {
                       Code = r.Field<string>("CODE"),
                       CodeDesc = r.Field<string>("CODE_DESC"),
                       PgroupName = r.Field<string>("PGROUP_NAME"),
                       IsActive = r.Field<string>("ISACTIVE"),
                       CmdId = Convert.ToInt32(r["CMD_ID"])
                   })
                   .ToList() ?? new List<CMMASTER_DATA>();
                return PartialView("_CustomerMasterData", dataList);
            }
            else
            {
                return Json("error");
            }
        }
        [HttpPost]
        public IActionResult CustomerMasterList([FromBody] CustomerMgmtResponse data)
        {
            try
            {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<CMMASTER_DATA> dataList = new List<CMMASTER_DATA>();
            DataTable dt = _cmService.FetchCodesByGroupName(data.code);

            if (dt != null && dt.Rows.Count > 0) {
                int cmdid = Convert.ToInt32(data.message);
                dataList = dt?.AsEnumerable()
                      .Select(r => new CMMASTER_DATA
                      {
                          Code = r.Field<string>("CODE"),
                          CodeDesc = r.Field<string>("CODE_DESC"),
                          PgroupName = r.Field<string>("PGROUP_NAME"),
                          IsActive = r.Field<string>("ISACTIVE"),
                          CmdId = Convert.ToInt32(r["CMD_ID"])
                      })
                      .ToList() ?? new List<CMMASTER_DATA>();
                CMMASTER_DATA cmobj = dataList.Where(x => x.CmdId == cmdid).FirstOrDefault();
                return Json(cmobj);
            }
            else
            {
                return Json("error");
            }

            }
            catch (Exception ex) {
                _logger.LogError("CustomerMasterList" + ex.Message);
                return Json("error");
            }


        }
        [HttpGet]
        public IActionResult CMApprovalAuthorityMatrix()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }catch(Exception ex)
            {

            }

                return View();
        }

        [HttpPost]
        public IActionResult CustomerReqApproverDetail([FromBody] CustomerMgmtResponse data)
        {
            try {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<CustomerReqApproverDetails> dataList = _cmService.GetCustomerApprovalDetails(data.code);

                return Json(dataList);
            } catch (Exception ex) {
                _logger.LogError("CustomerReqApproverDetail", ex.Message);
                return Json("error");
            }
           
        }

        [HttpGet]
        public IActionResult FinanceApproverData()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<CMApprovalAuthority> dataList = _cmService.GetFinanceApproverData();

                return PartialView("_FinanceApproverList", dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("FinanceApproverData", ex.Message);
                return Json("error");
            }

        }

        [HttpGet]
        public IActionResult GetSwitchApproverData()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                int userid = _session.Get<int>("userID");
                List<SwitchApprovalAuthority> dataList = _cmService.GeSwitchApproverData(userid);

                return PartialView("_SwitchApproverList", dataList);
                // return Json(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetSwitchApproverData", ex.Message);
                return Json("error");
            }

        }

        [HttpPost]
        public IActionResult GetApproverName([FromBody] CustomerMgmtResponse data)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                var dataList = _cmService.GeApproverName(data.code);

                return Json(new { Name = dataList });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetApproverName", ex.Message);
                return Json("error");
            }

        }
        [HttpPost]
        public IActionResult SaveFinanceData([FromBody] CMApproverMatrx data){
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                data.CreatedBy=Convert.ToInt64(userid);
                int res=0;
                var dataList = _cmService.SaveFinanceApproverData(data, ref res);
                var response = new CustomerMgmtResponse {
                    code=res.ToString(),
                    message=dataList
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("FinanceApproverData", ex.Message);
                return Json("error");
            }
        }
        [HttpGet]
        public IActionResult MasterDataManagement()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                var dataList = _cmService.ShowFieldMaster();
                ViewBag.res = "";
                return View(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterDataManagement", ex.Message);
                return View();
            }
            
        }
        [HttpPost]
        public IActionResult MasterDataManagement(CustomerMasterDataMng data)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string res = "";
                string userid = _session.Get<string>("userID");
                data.CreatedBy= userid;
                data.UpdatedBy= userid;
                var dataList = _cmService.SaveMandatoryData(data,ref res);
                ViewBag.res = res;
                return View(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterDataManagement", ex.Message);
                return View();
            }

        }
        [HttpPost]
        public IActionResult AddMasterDataManagement(CustomerMasterDataMng data)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string res = "";
                string userid = _session.Get<string>("userID");
                data.CreatedBy = userid;
                data.UpdatedBy = userid;
                var dataList = _cmService.AddMandatoryData(data, ref res);
                ViewBag.res = res;
                return View(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterDataManagement", ex.Message);
                return View();
            }

        }
        [HttpGet]
        public IActionResult CustomerManageRequestAdmin()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
             
               
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterDataManagement", ex.Message);
                return View();
            }

        }
        [HttpGet]
        public IActionResult CustomerManageRequest()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterDataManagement", ex.Message);
                return View();
            }

        }
        [HttpPost]
        public IActionResult GetCustomerRequestList([FromBody] CustomerRequestInput objparm)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                objparm.USERID = userid;              
                string res = "";
                var dataList = _cmService.ShowCustomerRequestData(objparm, ref res);
                ViewBag.res = "";
                if (dataList != null)
                {
                    string jstr = JsonConvert.SerializeObject(dataList);
                    TempData.Remove("CMREQLIST");
                    TempData["CMREQLIST"] = jstr;
                    ViewBag.ExcelData = dataList;
                    return PartialView("_CustomerRequestList", dataList);
                }
                else
                {
                    return Json("error");
                }
                    
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCustomerRequestList", ex.Message);
                return Json("error");
            }

        }
        [HttpPost]
        public ActionResult GetCustomerRequestExcel([FromBody] CustomerRequestInput objparm)
        {
            short retVal = 0;
            try
            {   if (_session.Get<string>("userID") == null)
                    {
                        return RedirectToAction("Index", "Login");
                    }
                string userid = _session.Get<string>("userID");
                objparm.USERID = userid;
                string res = "";
                List<CustomerRequestRpt> dataList = null;
                
               string ss= (string)TempData["CMREQLIST"] ;
                if (ss =="")
                {
                    dataList = _cmService.ShowCustomerRequestData(objparm, ref res);
                }
                else
                {
                    //dataList = (List<CustomerRequestRpt>)ViewBag.ExcelData;
                    dataList = JsonConvert.DeserializeObject<List<CustomerRequestRpt>>(ss);
                }

                string str = _cmService.btnExcelExport(dataList);
                TempData.Remove("CMREQ");
                TempData["CMREQ"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        [HttpGet]
        public ActionResult DownloadReportExcel()
        {
            try
            {
                string FileName = "Search CM Details" + DateTime.Now.ToString("ddMMMyyyy HH:mm");
                if (TempData["CMREQ"] == null)
                {
                    return View();
                }
                string str = (string)TempData["CMREQ"];
                // HttpContext.Response.AddHeader("content-disposition", "attachment; filename=POReport.xls");

                //Response.ContentType = "application/vnd.ms-excel";
                //return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
                Response.Headers.Add("Content-Disposition", "attachment; filename="+FileName+".xls");
                Response.ContentType = "application/vnd.ms-excel";

                // Return the file with UTF-8 encoded bytes
                return File(Encoding.UTF8.GetBytes(str), "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }
        /// <summary>
        /// /////////////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CustomerCreation(string VMID)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string VenderHeaderID = "";
                if (VMID != null)
                {
                    VenderHeaderID =WebUtility.UrlDecode(Encryption.Decrypt(VMID));
                    
                }
                ViewBag.hidGenDetailID = VenderHeaderID;

                int userid = _session.Get<int>("userID");
                CustomerMasterDetail objparm = new CustomerMasterDetail();                
                string res = "";
                DataTableConverter dc = new DataTableConverter();
                DataSet ds = _cmService.BindMasterData("COUNTRY");
                var data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlCountry= data;
                DataTable dd = new DataTable();
                dd = ds.Tables[1].Select("GROUP_NAME='DIVISION'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                ViewBag.DIVISION = data;
                //ds = _cmService.BindMasterData("DIVISION");
                // data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);                
                //ViewBag.DIVISION = data;               


                //ds = _cmService.BindMasterData("REGION");
                // data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlRegion = data;
                dd = ds.Tables[1].Select("GROUP_NAME='REGION'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlRegion = data;

                //ds = _cmService.BindMasterData("TRANSPORTATION CODE EXP");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.Trans_ex = data;
                dd = ds.Tables[1].Select("GROUP_NAME='TRANSPORTATION CODE EXP'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.Trans_ex = data;

                //ds = _cmService.BindMasterData("TRANSPORTATION CODE");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.Trans_dm = data;
                dd = ds.Tables[1].Select("GROUP_NAME='TRANSPORTATION CODE'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.Trans_dm = data;

                //ds = _cmService.BindMasterData("INDUSTRY");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlIndustry = data;
                dd = ds.Tables[1].Select("GROUP_NAME='INDUSTRY'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlIndustry = data;

                //ds = _cmService.BindMasterData("CITY CODE");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlCityCode = data;
                dd = ds.Tables[1].Select("GROUP_NAME='CITY CODE'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();                
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlCityCode = data;

                //ds = _cmService.BindMasterData("CUSTOMER CLASS");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddCustomerClass = data;

                dd = ds.Tables[1].Select("GROUP_NAME='CUSTOMER CLASS'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddCustomerClass = data;

                //ds = _cmService.BindMasterData("SALES DISTRICT");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlSalesDistrict = data;
                dd = ds.Tables[1].Select("GROUP_NAME='SALES DISTRICT'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlSalesDistrict = data;

                //ds = _cmService.BindMasterData("SALES OFFICE");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlSalesOffice = data;
                dd = ds.Tables[1].Select("GROUP_NAME='SALES OFFICE'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlSalesOffice = data;

                //ds = _cmService.BindMasterData("SALES GROUP");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlSalesGroup = data;
                dd = ds.Tables[1].Select("GROUP_NAME='SALES GROUP'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlSalesGroup = data;

                //ds = _cmService.BindMasterData("CUSTOMER GROUP");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlCustomerGroup = data;
                dd = ds.Tables[1].Select("GROUP_NAME='CUSTOMER GROUP'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlCustomerGroup = data;

                //ds = _cmService.BindMasterData("CUST. PRICE");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlCustomerPrice = data;
                dd = ds.Tables[1].Select("GROUP_NAME='CUST. PRICE'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlCustomerPrice = data;

                //ds = _cmService.BindMasterData("DELIVERY PRIORITY");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlDeliveryPriority = data;

                dd = ds.Tables[1].Select("GROUP_NAME='DELIVERY PRIORITY'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlDeliveryPriority = data;

                //ds = _cmService.BindMasterData("SHIPPING CONDITIONS");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlShippingConditions = data;

                dd = ds.Tables[1].Select("GROUP_NAME='SHIPPING CONDITIONS'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlShippingConditions = data;

                //ds = _cmService.BindMasterData("INCOTERMS");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlIncoterms = data;
                dd = ds.Tables[1].Select("GROUP_NAME='INCOTERMS'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlIncoterms = data;

                //ds = _cmService.BindMasterData("TERMS OF PAYMENT");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlTermsOfPayment = data;
                dd = ds.Tables[1].Select("GROUP_NAME='TERMS OF PAYMENT'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlTermsOfPayment = data;

                //ds = _cmService.BindMasterData("CREDIT CONTROL AREA");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlCreditControlArea = data;
                dd = ds.Tables[1].Select("GROUP_NAME='CREDIT CONTROL AREA'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlCreditControlArea = data;

                //ds = _cmService.BindMasterData("ACCT ASSIGNMENT GROUP");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlAccountAssignmentGroup = data;
                dd = ds.Tables[1].Select("GROUP_NAME='ACCT ASSIGNMENT GROUP'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlAccountAssignmentGroup = data;

                //ds = _cmService.BindMasterData("EXCHANGE RATE TYPE");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlExchangeRateType = data;
                dd = ds.Tables[1].Select("GROUP_NAME='EXCHANGE RATE TYPE'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlExchangeRateType = data;

                //ds = _cmService.BindMasterData("TIMEZONE");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlTimeZone = data;
                dd = ds.Tables[1].Select("GROUP_NAME='TIMEZONE'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlTimeZone = data;

                //ds = _cmService.BindMasterData("CURRENCY");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlSalesCurrency = data;
                dd = ds.Tables[1].Select("GROUP_NAME='CURRENCY'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlSalesCurrency = data;

                //ds = _cmService.BindMasterData("RECON ACCOUNT");
                //data = dc.TableToList<MasterDataBinding>(ds.Tables[0]);
                //data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                //ViewBag.ddlReconAccount = data;
                dd = ds.Tables[1].Select("GROUP_NAME='RECON ACCOUNT'").CopyToDataTable();
                data = dd?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),

                  })
                  .ToList() ?? new List<MasterDataBinding>();
                data.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlReconAccount = data;

                ds = _cmService.GetSectionHead(userid);
                var dataList = new List<MasterDataBinding>();
                var dataList1 = new List<MasterDataBinding>();
                if (ds.Tables.Count > 0)
                {
                     dataList = ds.Tables[0]?.AsEnumerable()
                 .Select(r => new MasterDataBinding
                 {
                     CODE = (r.Field<long>("ADEMPCODE")).ToString(),
                     CODE_DESC = r.Field<string>("EMPNAME"),
                   
                 })
                 .ToList() ?? new List<MasterDataBinding>();

                dataList1 = ds.Tables[1]?.AsEnumerable()
                     .Select(r => new MasterDataBinding
                     {
                         CODE = (r.Field<long>("ADEMPCODE")).ToString(),
                         CODE_DESC = r.Field<string>("DEPARTMENTHEAD"),

                     })
                     .ToList() ?? new List<MasterDataBinding>();
                }
                dataList.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                dataList1.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                ViewBag.ddlApprovalAuthority = dataList;
                ViewBag.ddlDeptDivHead = dataList1;

                ViewBag.res = "";
                return View(objparm);
            }
            catch (Exception ex)
            {
                _logger.LogError("CustomerCreation", ex.Message);
                ViewBag.res = "Error in data binding!";
                return View();
            }

        }
        [HttpPost]
        public IActionResult ChangeCountry([FromBody]MasterDataBinding data) {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                DataSet ds = _cmService.BindTransportGRP(data.CODE, data.CODE_DESC);
               var dataList1 = ds.Tables[0]?.AsEnumerable()
                      .Select(r => new MasterDataBinding
                      {
                          CODE = r.Field<string>("CODE"),
                          CODE_DESC = r.Field<string>("CODE_DESC"),

                      })
                      .ToList() ?? new List<MasterDataBinding>();
                dataList1.Insert(0, new MasterDataBinding { CODE = "", CODE_DESC = "---Select---" });
                if (dataList1.Count > 0) {
                    return Json(dataList1);
                }
                else
                {
                    return Json("error");
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError("ChangeCountry", ex.Message);
                
                return Json("error");
            }
           
        }
        [HttpPost]
        public async Task<IActionResult> ValidateVendorCode([FromBody] MasterDataBinding data)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                DataSet ds =await  objess.GET_VENDOR_MASTER(data.CODE, "50AG");
                if (ds == null) {                    
                    return Json(new { error="error", message="SAP error" });
                }
                else if (ds.Tables[2].Rows.Count > 0)
                {
                    return Json(new { error = ds.Tables[2].Rows[0]["TYPE"].ToString(), message = ds.Tables[2].Rows[0]["MESSAGE"].ToString() });
                }

                return Json(new { error = "error", message = "SAP error" });

            }
            catch (Exception ex)
            {
                _logger.LogError("ChangeCountry", ex.Message);
                return Json(new { error = "error", message = "SAP error" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ValidateDealerCode([FromBody] MasterDataBinding data)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (data.CODE_DESC == "U")
                {
                    return Json(new { error = "", message = "Not required to check dealer code for Update/Extend request!" });
                }
                string userid = _session.Get<string>("userID");
                //ds = EPOBJ.GET_CUSTOMER_MASTER(CustomerCode, "50AG");
                DataSet ds = await objess.GET_CUSTOMER_MASTER(data.CODE, "50AG");
                
            
                if (ds == null)
                {
                    return Json(new { error = "error", message = "SAP error" });
                }
                else if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["CUSTOMER"].ToString() != "")
                    {
                        return Json(new { error = "N", message = ds.Tables[0].Rows[0]["CUSTOMER"].ToString() });
                    }
                    else
                    {
                        return Json(new { error = "Y", message = ds.Tables[0].Rows[0]["CUSTOMER"].ToString() });
                    }
                       
                }

                return Json(new { error = "error", message = "SAP error" });

            }
            catch (Exception ex)
            {
                _logger.LogError("ChangeCountry", ex.Message);
                return Json(new { error = "error", message = "SAP error" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> btnSearch([FromBody] MasterDataBinding data)
        {
            try {
                if (data.CODE_DESC.Length == 0)
                {
                    return Json(new { error = "error", message = "SAP error" });
                }
                string CustomerCode = data.CODE_DESC.Trim();
                //Check customer code less than 10 digits and numeric value
                if (CustomerCode.Length < 10)
                {
                    long n;
                    if (long.TryParse(CustomerCode, out n))
                    {
                        CustomerCode = CustomerCode.PadLeft(10, '0');
                    }
                }
                DataSet ds = new DataSet();
                ds = await objess.GET_CUSTOMER_MASTER(CustomerCode, "50AG");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["CUSTOMER"].ToString() != "")
                    {
                        DataTable ds1 = await objess.GET_DIVSION_List(CustomerCode);
                        string div_gr = "", distr_chn = "", distr_chn1 = "";
                        if (ds1.Rows.Count > 0)
                        {
                            foreach (DataRow r in ds1.Rows)
                            {
                                //div_gr += r["div_gr"].ToString() + "|";
                                distr_chn1 = r["DISTRCHN"].ToString();
                                div_gr = r["DIVISION"].ToString();
                            }

                            //div_gr = div_gr.Substring(0, div_gr.Length - 1); //Single Division selected
                            //SetDivisionGrp(div_gr);
                            //ddlDistribution.SelectedValue = distr_chn1;
                            //SetMandatoryField();
                            //InitGroupOfData();
                            //ViewState["Gen_data"] = false;
                            //txtDealerCode.Text = CustomerCode;
                            //txtDealerCode.Enabled = false;
                            //CheckAllGroupOfData();
                            //ShowInfo("Please click on 'Save and Next' Button of Genral Data for Extend/Update Request.");
                        }
                        var res = new
                        {
                            error = "N",
                            message = "",
                            NAME = ds.Tables[1].Rows[0]["NAME"].ToString(),
                            NAME_2 = ds.Tables[1].Rows[0]["NAME_2"].ToString(),
                            NAME_3 = ds.Tables[1].Rows[0]["NAME_3"].ToString(),
                            NAME_4 = ds.Tables[1].Rows[0]["NAME_4"].ToString(),
                            CUSTOMER = ds.Tables[1].Rows[0]["CUSTOMER"].ToString(),
                            ACCNT_GRP = ds.Tables[0].Rows[0]["ACCNT_GRP"].ToString(),
                            DISTRCHN = distr_chn1,
                            DIVISION = div_gr
                        };
                        return Json(res);
                    }
                    else
                    {
                        return Json(new { error = "Y", message = "Invalid Customer Code" });
                    }
                }
                else
                {
                    return Json(new { error = "Y", message = "SAP error" });
                }
            }
            catch(Exception ex) {
                _logger.LogError("btnSearch", ex.Message);
                return Json(new { error = "error", message = "SAP error :-"+ex.Message });
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> VerifyBankData([FromBody] MasterDataBinding data)
        {
            try
            {
                if (data.CODE.Length == 0 || data.CODE.Length == 0)
                {
                    return Json(new { error = "error", message = "Please enter bank key" });
                }
                if ( data.CODE_DESC.Length == 0)
                {
                    return Json(new { error = "error", message = "Please select bank key" });
                }
               
                DataTable dt = new DataTable();
                dt = await objess.GET_Bank_Verification(data.CODE, data.CODE_DESC);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["BANK_NAME"].ToString()))
                    {
                        //txtBankName.Text = dt.Rows[0]["BANK_NAME"].ToString();
                        ////Changed by TTL CR7146
                        //if (!string.IsNullOrEmpty(dt.Rows[0]["REGION"].ToString()))
                        //{
                        //    ddlBankRegion.SelectedValue = dt.Rows[0]["REGION"].ToString();
                        //}
                        //txtBankStreet.Text = dt.Rows[0]["STREET"].ToString();
                        //txtBankCity.Text = dt.Rows[0]["CITY"].ToString();
                        //txtBankBranch.Text = dt.Rows[0]["BANK_BRANCH"].ToString();

                        //// Disable textboxes
                        //if (dt.Rows[0]["BANK_NAME"].ToString() != "") txtBankName.Enabled = false;
                        //if (dt.Rows[0]["REGION"].ToString() != "") ddlBankRegion.Enabled = false;
                        //if (dt.Rows[0]["STREET"].ToString() != "") txtBankStreet.Enabled = false;
                        //if (dt.Rows[0]["CITY"].ToString() != "") txtBankCity.Enabled = false;
                        //if (dt.Rows[0]["BANK_BRANCH"].ToString() != "") txtBankBranch.Enabled = false;

                        //// Show verification message
                        //lblVerificationStatus.Text = "Bank details verified.";

                        var res = new
                        {
                            error = "Y",
                            message = "Bank details verified.",
                            BANK_NAME = dt.Rows[0]["BANK_NAME"].ToString(),
                            REGION = dt.Rows[0]["REGION"].ToString(),
                            STREET = dt.Rows[0]["STREET"].ToString(),
                            CITY = dt.Rows[0]["CITY"].ToString(),
                            BANK_BRANCH = dt.Rows[0]["BANK_BRANCH"].ToString()                           
                        };
                        return Json(res);
                    }
                    else
                    {
                        return Json(new { error = "Y", message = "Bank details Not Found" });
                    }
                }
                else
                {
                    return Json(new { error = "Y", message = "SAP error" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnSearch", ex.Message);
                return Json(new { error = "error", message = "SAP error :-" + ex.Message });
            }

        }

        [HttpPost]
        public IActionResult GetMandatoryField([FromBody] CustAccountFlags data)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                DataTable ds = _cmService.SetMandatoryField(data);              

                var dataList1 = ds?.AsEnumerable()
                       .Select(r => new CustomerMasterDataMng
                       {  ID = r.Field<short>("ID"),
                           FIELDNAME = r.Field<string>("FIELDNAME"),
                           CLIENTIDLABEL = r.Field<string>("CLIENTIDLABEL"),
                           CLIENTIDTEXTLABEL = r.Field<string>("CLIENTIDTEXTLABEL"),
                           GROUP_NAME = r.Field<string>("GROUP_NAME")
                       })
                       .ToList() ?? new List<CustomerMasterDataMng>();
                //var ss = JsonConvert.SerializeObject(dataList1);
                //_session.Set<string>("vds", ss);

                if (dataList1.Count > 0)
                {
                    return Json(dataList1);
                }
                else
                {
                    return Json("error");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GetMandatoryField", ex.Message);
                return Json("error");
            }

        }
        [HttpGet] 
        public IActionResult GetDraftData(string genDetailId)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
               
                string  err= "";
                string userid = _session.Get<string>("userID");
                DataTable dt = _cmService.GetDetailDraft(userid, genDetailId,ref err);

                if (!string.IsNullOrEmpty(err))
                {
                    _logger.LogError("GetDraftData service error: {Error}", err);
                    return Json(new { error = err });
                }

                //if (ds.Rows.Count > 0)
                //{
                //    var rs = JsonConvert.SerializeObject(ds);
                //    return Json(new { rows=rs });
                //}
                //else
                //{
                //    return Json("error");
                //}

                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { rows = new List<object>() }); // empty array
                }

                // Build rows array
                var rows = dt.AsEnumerable()
                             .Select(r =>
                             {
                                 var obj = new Dictionary<string, object>(dt.Columns.Count);
                                 foreach (DataColumn col in dt.Columns)
                                 {
                                     obj[col.ColumnName] = r[col] == DBNull.Value ? null : r[col];
                                 }
                                 return (object)obj;
                             })
                             .ToList();

                // Return as JSON with rows array
                return Json(new { rows });


            }
            catch (Exception ex)
            {              
                _logger.LogError(ex, "GetDraftData failed");
                return Json(new { error = "error" });
            }
           
        }
        [HttpGet]
        public IActionResult GetMasterDataWithPG(string GroupName, string PGroup)
        {
            try
            {              
                string err = "";               
                DataTable dt = _cmService.GetCustomerMasterDataWithPGRP(GroupName, PGroup);
                if (!string.IsNullOrEmpty(err))
                {
                    _logger.LogError("GetDraftData service error: {Error}", err);
                    return Json(new { error = err });
                }                             

                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { rows = new List<object>() }); // empty array
                }

                // Build rows array
                //var rows = dt.AsEnumerable()
                //             .Select(r =>
                //             {
                //                 var obj = new Dictionary<string, object>(dt.Columns.Count);
                //                 foreach (DataColumn col in dt.Columns)
                //                 {
                //                     obj[col.ColumnName] = r[col] == DBNull.Value ? null : r[col];
                //                 }
                //                 return (object)obj;
                //             })
                //             .ToList();

                // Return as JSON with rows array
               var rows = dt?.AsEnumerable()
                  .Select(r => new MasterDataBinding
                  {
                      CODE = r.Field<string>("CODE"),
                      CODE_DESC = r.Field<string>("CODE_DESC"),
                     
                  })
                  .ToList() ?? new List<MasterDataBinding>();
                return Json(new { rows });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDraftData failed");
                return Json(new { error = "error" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SaveGeneralData(CustomerMasterDetail data)
        {
            try
            {                
                string CIN_GST_NO_FILE_IN = "";                           

              if(string.IsNullOrEmpty(data.CM_REQ_TYPE) || string.IsNullOrEmpty(data.CUST_ACC_TYPE) 
                    || string.IsNullOrEmpty(data.REQUEST_TYPE) || string.IsNullOrEmpty(data.DISTRIBTUION_CHH) || string.IsNullOrEmpty(data.DIVISION_GRP) ||
                    string.IsNullOrEmpty(data.CUSTOMER_CODE) || string.IsNullOrEmpty(data.SALES_ORG) || string.IsNullOrEmpty(data.COMPANY_CODE))
                {
                    return Json(new CustomerResponseData { RS = 0,MESSAGE="Please check primary field data!" });
                }
                long userid = _session.Get<long>("userID");
                if (data.FILE1 != null)
                {
                    string rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "GST.PDF", "", "1", data.FILE1);
                    if (rs.Substring(0, 1) == "1")
                    {
                        CIN_GST_NO_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }
                
                data.REQUESTEDID = userid;
                Employee_Details emp = _session.Get<Employee_Details>("Employee");
                string emailid = emp.EMail_Id;
                var res = _cmService.SaveGeneral(data, CIN_GST_NO_FILE_IN, emailid);
                res.FILE1 = CIN_GST_NO_FILE_IN;
                res.VIEWSTATE = "1";
                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveGeneralData");                
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SaveBankData(CustomerMasterDetail data)
        {
            try
            {
                string BANK_MANDATE_FILE_IN = "", BANK_CANCELED_FILE_IN = "";

                if (string.IsNullOrEmpty(data.CM_REQ_TYPE) || string.IsNullOrEmpty(data.CUST_ACC_TYPE)
                      || string.IsNullOrEmpty(data.REQUEST_TYPE) || string.IsNullOrEmpty(data.DISTRIBTUION_CHH) || string.IsNullOrEmpty(data.DIVISION_GRP) ||
                      string.IsNullOrEmpty(data.CUSTOMER_CODE) || string.IsNullOrEmpty(data.SALES_ORG) || string.IsNullOrEmpty(data.COMPANY_CODE))
                {
                    return Json(new CustomerResponseData { RS = 0, MESSAGE = "Please check primary field data!" });
                }
                long userid = _session.Get<long>("userID");
                if (data.FILE1 != null)
                {
                    string rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "BANK_MANDATE.PDF", "", "1", data.FILE1);
                    if (rs.Substring(0, 1) == "1")
                    {
                        BANK_MANDATE_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }
                if (data.FILE2 != null) {
                    string rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "CHEQUE.PDF", "", "1", data.FILE2);
                    if (rs.Substring(0, 1) == "1")
                    {
                        BANK_CANCELED_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }
               

                data.REQUESTEDID = userid;
                Employee_Details emp = _session.Get<Employee_Details>("Employee");
                string emailid = emp.EMail_Id;
                var res = _cmService.SaveBankData(data, BANK_MANDATE_FILE_IN, BANK_CANCELED_FILE_IN, emailid);
               
                res.VIEWSTATE = "1";
                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveGeneralData");
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveCINData(CustomerMasterDetail data)
        {
            try
            {
                string  CIN_PAN_NO_FILE_IN="", CIN_GST_NO_FILE_IN="", CIN_CIN_DOC_FILE_IN="";

                if (string.IsNullOrEmpty(data.CM_REQ_TYPE) || string.IsNullOrEmpty(data.CUST_ACC_TYPE)
                      || string.IsNullOrEmpty(data.REQUEST_TYPE) || string.IsNullOrEmpty(data.DISTRIBTUION_CHH) || string.IsNullOrEmpty(data.DIVISION_GRP) ||
                      string.IsNullOrEmpty(data.CUSTOMER_CODE) || string.IsNullOrEmpty(data.SALES_ORG) || string.IsNullOrEmpty(data.COMPANY_CODE))
                {
                    return Json(new CustomerResponseData { RS = 0, MESSAGE = "Please check primary field data!" });
                }
                long userid = _session.Get<long>("userID");
                string rs;
                if (data.FILE1 != null)
                {
                     rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "PAN.PDF", "", "1", data.FILE1);
                    if (rs.Substring(0, 1) == "1")
                    {
                        CIN_PAN_NO_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }
                if (data.FILE2 != null)
                {
                    rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "CIN.PDF", "", "1", data.FILE2);
                    if (rs.Substring(0, 1) == "1")
                    {
                        CIN_CIN_DOC_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }
               

                data.REQUESTEDID = userid;
                Employee_Details emp = _session.Get<Employee_Details>("Employee");
                string emailid = emp.EMail_Id;
                var res = _cmService.SaveCINData(data, CIN_PAN_NO_FILE_IN, CIN_GST_NO_FILE_IN, CIN_CIN_DOC_FILE_IN, emailid);

                res.VIEWSTATE = "1";
                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveGeneralData");
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SaveIndustryData(CustomerMasterDetail data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.CM_REQ_TYPE) || string.IsNullOrEmpty(data.CUST_ACC_TYPE)
                      || string.IsNullOrEmpty(data.REQUEST_TYPE) || string.IsNullOrEmpty(data.DISTRIBTUION_CHH) || string.IsNullOrEmpty(data.DIVISION_GRP) ||
                      string.IsNullOrEmpty(data.CUSTOMER_CODE) || string.IsNullOrEmpty(data.SALES_ORG) || string.IsNullOrEmpty(data.COMPANY_CODE))
                {
                    return Json(new CustomerResponseData { RS = 0, MESSAGE = "Please check primary field data!" });
                }
                long userid = _session.Get<long>("userID");
                data.REQUESTEDID = userid;
                Employee_Details emp = _session.Get<Employee_Details>("Employee");
                string emailid = emp.EMail_Id;
                var res = _cmService.SaveIndustryData(data, emailid);

                res.VIEWSTATE = "1";
                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveGeneralData");
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SaveSalesData(CustomerMasterDetail data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.CM_REQ_TYPE) || string.IsNullOrEmpty(data.CUST_ACC_TYPE)
                      || string.IsNullOrEmpty(data.REQUEST_TYPE) || string.IsNullOrEmpty(data.DISTRIBTUION_CHH) || string.IsNullOrEmpty(data.DIVISION_GRP) ||
                      string.IsNullOrEmpty(data.CUSTOMER_CODE) || string.IsNullOrEmpty(data.SALES_ORG) || string.IsNullOrEmpty(data.COMPANY_CODE))
                {
                    return Json(new CustomerResponseData { RS = 0, MESSAGE = "Please check primary field data!" });
                }
                long userid = _session.Get<long>("userID");
                data.REQUESTEDID = userid;
                Employee_Details emp = _session.Get<Employee_Details>("Employee");
                string emailid = emp.EMail_Id;
                var res = _cmService.SaveSaleData(data, emailid);

                res.VIEWSTATE = "1";
                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveGeneralData");
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SaveCompanyData(CustomerMasterDetail data)
        {
            try
            {
                string COMP_RTO_FILE_IN = "", COMP_LOI_FILE_IN = "", OTHER_DOC_FILE_IN = "";

                if (string.IsNullOrEmpty(data.CM_REQ_TYPE) || string.IsNullOrEmpty(data.CUST_ACC_TYPE)
                      || string.IsNullOrEmpty(data.REQUEST_TYPE) || string.IsNullOrEmpty(data.DISTRIBTUION_CHH) || string.IsNullOrEmpty(data.DIVISION_GRP) ||
                      string.IsNullOrEmpty(data.CUSTOMER_CODE) || string.IsNullOrEmpty(data.SALES_ORG) || string.IsNullOrEmpty(data.COMPANY_CODE))
                {
                    return Json(new CustomerResponseData { RS = 0, MESSAGE = "Please check primary field data!" });
                }
                long userid = _session.Get<long>("userID");
                string rs;
                if (data.FILE1 != null) {
                     rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "RTO.PDF", "", "1", data.FILE1);
                    if (rs.Substring(0, 1) == "1")
                    {
                        COMP_RTO_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }
                if (data.FILE2 != null)
                {
                    rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "LOI.PDF", "", "1", data.FILE2);
                    if (rs.Substring(0, 1) == "1")
                    {
                        COMP_LOI_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }
                if (data.FILE3 != null)
                {
                    rs = uploadDocument(data.CUSTOMER_CODE.Trim().ToUpper() + "OTHER_DOC.PDF", "", "1", data.FILE3);
                    if (rs.Substring(0, 1) == "1")
                    {
                        OTHER_DOC_FILE_IN = rs.Substring(2);
                    }
                    else
                    {
                        return Json(new CustomerResponseData { RS = 0, MESSAGE = rs.Substring(2) });
                    }
                }

                data.REQUESTEDID = userid;
                Employee_Details emp = _session.Get<Employee_Details>("Employee");
                string emailid = emp.EMail_Id;
                var res = _cmService.SaveCompanyData(data, COMP_RTO_FILE_IN, COMP_LOI_FILE_IN, OTHER_DOC_FILE_IN, emailid);

                res.VIEWSTATE = "1";
                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCompanyData");
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SubmitCutomerRequest([FromBody]FinalSubmit data)
        {
            try
            {
                             
               if(data.cmhid==0 || data.SEC_HEAD_ID==0 || data.DEPT_DIV_ID==0 || data.REMARKS=="")
                {
                    return Json(new CustomerResponseData { RS = 0, MESSAGE = "Please check primary field data!" });
                }
                long userid = _session.Get<long>("userID");
                string rs;
              

                data.USERID = userid;
              
                var res = _cmService.SubmitCutomerRequest(data);
                res.VIEWSTATE = "1";
                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCompanyData");
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }
        public async Task<IActionResult> ResetCustomerData(string REQUEST_TYPE)
        {
            try
            {

                long userid = _session.Get<long>("userID");
                string rs;             

                var res = _cmService.ResetCMRequest(userid, REQUEST_TYPE);

                return RedirectToAction("CustomerCreation", "CustomerMgmt");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCompanyData");
                return Json(new CustomerResponseData { RS = 0, MESSAGE = ex.Message });
            }

        }
        [HttpGet]
        public async Task<IActionResult> PreviewCustomerMaster()
        {
            try
            {
                long userid = _session.Get<long>("userID");               
                var res = _cmService.GetPreviewData("",userid);
                return View(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PreviewCustomerMaster");
                return View();
            }

        }
        [HttpGet]
        public async Task<IActionResult> ViewCustomerMaster(string VMID)
        {
            try
            {
                string VenderHeaderID =WebUtility.UrlDecode(VMID);
                string userid = _session.Get<string>("userID");
                var res = _cmService.GetCustomerRequestDetails(VenderHeaderID, userid);
                return View(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PreviewCustomerMaster");
                return View();
            }

        }
        [HttpGet]
        public async Task<IActionResult> CMRequestHistory(string VMID)
        {
            try
            {
                string VenderHeaderID = WebUtility.UrlDecode(VMID);
                if (VenderHeaderID.Trim().Length <=0)
                {
                    _logger.LogError("CM Request number is null value", "CMRequestHistory");
                    return RedirectToAction("home", "home");
                }
                string userid = _session.Get<string>("userID");
                var res = _cmService.GetApprovalHistory(VenderHeaderID, userid);
                if (res.Historymodle != null) {
                    return View(res);
                }
                else
                {
                    _logger.LogError("Error in data binding", "CMRequestHistory");
                    return RedirectToAction("home", "home");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PreviewCustomerMaster");
                return View();
            }

        }
        public string uploadDocument(string strFileName, string strFolderName, string DOCTYPE,IFormFile fp)
        {
            if (strFileName == "")
            {
                return "0@Invalid file name supplied";
            }

            strFileName = Path.GetFileName(strFileName);
            string fullpath = Path.Combine(filePath, strFileName);
            string fileext = Path.GetExtension(fullpath);
           
            string ModifiedName = Path.GetFileNameWithoutExtension(strFileName) + DateTime.Now.ToString("ddmmyyyyhhmmss") + fileext;
            string fullMpath = Path.Combine(filePath, ModifiedName);

            if (fullpath == "")
            { return "0@Path not found"; }

            if (fileext.ToLower() != ".pdf")
            {
                return "0@Cannot upload file because the file format or extension is invalid please select a valid PDF file";
            }

            try
            {
                if (DOCTYPE == "1")
                {
                    if (fp.Length == 0)
                    {
                        return "0@Invalid file content";
                    }
                    else if (fp.ContentType != "application/pdf")
                    {
                        return "0@Invalid file content type";
                    }
                    else if (fp.Length <= 1048576)//1MB maximum limit
                    {
                        using (var stream = new FileStream(fullMpath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                          fp.CopyTo(stream);
                        }
                        return "1@" + ModifiedName;
                    }
                    else
                    {
                        return "0@Mandate Form Attachment - Unable to upload,file exceeds maximum size limit";
                    }
                }              
                else
                {
                    return "0@Permission to upload file denied";
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                return "0@" + ex.Message + " Permission to upload file denied";
            }

        }
        
        /// <summary>
        /// //////////////////////
        /// </summary>
        /// <returns></returns>
                
      
        //////////////////////
        ///Approval Data
        /////////////////
        [HttpGet]
        public IActionResult CMRequestApproval()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long userid = _session.Get<long>("userID");
                var dataList = _cmService.BindMMCreationRequestDetails(userid);

                return View(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("CMRequestApproval", ex.Message);
                return View();
            }

        }
        [HttpGet]
        public async Task<IActionResult> CustomerMasterApproval(string VMID)
        {
            try
            {
                string VenderHeaderID = WebUtility.UrlDecode(VMID);
                string userid = _session.Get<string>("userID");
                var res = _cmService.GetCustomerRequestDetails(VenderHeaderID, userid);
                return View(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CustomerMasterApproval");
                return View();
            }

        }
        [HttpPost]
        public async Task<IActionResult> ApprovalRequestSubmit([FromBody]ApprovalReqest data)
        {
            try
            {              
                long userid = _session.Get<long>("userID");
                data.UserId= userid;
                var res = _cmService.ApprovalRequestSubmit(data);
                return Json(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApprovalRequestSubmit");
                var res = new {RS=0, message=ex.Message};
                return Json(res);
            }

        }

        [HttpGet]
        public IActionResult CustomerRequestSAPSYNC()
        {
            try
            {
                CustomerSyncInput data = new CustomerSyncInput();
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                data.UserId = userid;
                var dataList = _cmService.ShowCustomerSyncData(data);

                return View(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("CMRequestApproval", ex.Message);
                return View();
            }

        }
        [HttpPost]
        public IActionResult CustomerRequestSAPSYNC(CustomerSyncInput data)
        {
            try
            {
              //  CustomerSyncInput data = new CustomerSyncInput();
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");
                data.UserId = userid;
                var dataList = _cmService.ShowCustomerSyncData(data);

                return View(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("CMRequestApproval", ex.Message);
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> CustomerRequestSYNC([FromBody]CustomerSyncInput data)
        {
            try
            {
                
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID")??"";
                if(string.IsNullOrEmpty(data.cmheaderid) || string.IsNullOrEmpty(data.CUSTACCGROUP))
                {
                    return Json(new { RS = 0, MESSAGE = "Empty or blank parameter does not allow!" });
                }
                //SyncCustomerRequest(string cmheaderid, string custAccType, string UserId)
                var dataList =await _cmService.SyncCustomerRequest(data.cmheaderid, data.CUSTACCGROUP, userid);
                //if (dataList.RS == 1)
                //{
                //  return  RedirectToAction("CustomerRequestSAPSYNC",)
                //}
                //else
                //{
                //    return Json(dataList);
                //}
                return Json(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("CustomerRequestSYNC", ex.Message);
                var res = new { RS = 0, MESSAGE = "Error in Sync request!" };
                return Json(res);
            }

        }
    }
}
