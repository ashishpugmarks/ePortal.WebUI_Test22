using ePortal.Application.APPX.Contracts;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.MasterMgmt;
using ePortal.ViewModels.APPX.TourRequest;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Spire.Pdf.Graphics;
using System.Data;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class MasterMgmtController : Controller
    {
        private IMasterMgmtServices _mmService;
        private IMasterMgmtRepo oMasterQueries;
        private readonly IAppConfigurationService _env;
        private ISessionService _session;
        ILogger<CustomerMgmtController> _logger;
        public string filePath = "";
        private readonly IEportalESS objess;
        public MasterMgmtController(IMasterMgmtServices masterMgmt, IMasterMgmtRepo mmRepo, IAppConfigurationService appConfiguration, ISessionService sessionService, IEportalESS eportalESS, ILogger<CustomerMgmtController> logger)
        {
            _mmService = masterMgmt;
            oMasterQueries = mmRepo;
            _env = appConfiguration;
            _session = sessionService;
            _logger = logger;
            objess = eportalESS;
            filePath = serverpath.getFileUploadPath() + "CustomerMaster";
        }

        [HttpGet]
        public async Task<IActionResult> MaterialMasterCreateRequest()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                int UserID = _session.Get<int>("userID");
                ViewBag.SELECTEDPLANT = "0";

                DataSet ds = _mmService.BindMMCreationDetails(UserID);

                if (ds != null)
                {
                    int _MMHEADERID = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.MMHeaderId = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                        _session.Set<string>("MMHeaderIdS", ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        _MMHEADERID = Convert.ToInt32(ds.Tables[0].Rows[0]["MMHEADERID"].ToString());

                    }
                    else
                    {
                        ViewBag.MMHeaderId = "0";
                        _session.Set<string>("MMHeaderIdS", "");
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {

                        ViewBag.SELECTEDPLANT = ds.Tables[1].Rows[0]["PLANTCODE"].ToString();
                        DataSet oDs1 = oMasterQueries.GetPlantDetails(ViewBag.SELECTEDPLANT);
                        if (oDs1.Tables[0] != null && oDs1.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.PROFITCENTER = oDs1.Tables[0].Rows[0]["PROFIT_CENTER"].ToString();
                        }
                    List<MaterialDetail> dataList = new List<MaterialDetail>();
                        try {
                            dataList = ds.Tables[1]?.AsEnumerable()
                    .Select(r => new MaterialDetail
                    {
                        MMDETAILID = Convert.ToInt32(r.Field<decimal>("MMDETAILID")),
                        MMHEADERID = _MMHEADERID,
                        MATERIALDISCRIPTION = r.Field<string>("MATERIALDISCRIPTION"),
                        MATERIALSPECIFICATION = r.Field<string>("MATERIALSPECIFICATION"),
                        MMDESCSPEC = r.Field<string>("MMDESCSPEC"),
                        MATERIALTYPE = r.Field<string>("MATERIALTYPE"),
                        MATERIALGROUP = Convert.ToInt32(r.Field<decimal?>("MATERIALGROUP")),
                        VALUATIONCLASS = Convert.ToInt32(r.Field<decimal?>("VALUATIONCLASS")),
                        UOM = r.Field<string>("UOM"),
                        PRICE = Convert.ToDouble(r.Field<decimal?>("PRICE")),
                        PLANTNAMEDESC = r.Field<string>("PLANTNAMEDESC"),
                        PLANTNAME = r.Field<string>("PLANTNAME"),
                        PLANT = r.Field<string>("PLANT"),
                        PLANTCODE = Convert.ToInt32(r.Field<decimal?>("PLANTCODE")),
                        IsSales = r.Field<string>("REQ_TYPE")
                    })
                    .ToList() ?? new List<MaterialDetail>();
                        }
                        catch(Exception ex1) {
                            _logger.LogError(ex1, "MaterialMasterCreateRequest");
                        }
                    
                    ViewBag.DraftDetails = dataList;
                    }
                    else
                    {
                        ViewBag.DraftDetails = new List<MaterialDetail>();
                    }
                    
                }
                else
                {
                    ViewBag.MMHeaderId = "0";
                    _session.Set<string>("MMHeaderIdS", "");
                    ViewBag.SELECTEDPLANT = "0";
                    ViewBag.PROFITCENTER = "";
                    ViewBag.DraftDetails = new List<MaterialDetail>();
                }
                PopulateControls();
                BindMasterData();
                BindApprovalAuthority();
                BindMMCreationRejectDetails();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MaterialMasterCreateRequest");
                return View();
            }

        }
        private void PopulateControls()
        {

            List<MasterData> dataList = new List<MasterData>();
            DataSet oDs = new DataSet();


            //BOUND PLANT          
            oDs = oMasterQueries.GetMasterPlantList();

            dataList = oDs.Tables[0]?.AsEnumerable()
              .Select(r => new MasterData
              {
                  Id = Convert.ToInt64(r.Field<decimal>("PLANTID")),
                  Description = r.Field<string>("PLANTNAMEDESC"),

              })
              .ToList() ?? new List<MasterData>();


            ViewBag.ddlPlantMst = dataList;

            //BOUND MATERIALTYPE
            oDs = new DataSet();
            oDs = oMasterQueries.GetMaterialTypeList();

            dataList = oDs.Tables[0]?.AsEnumerable()
                 .Select(r => new MasterData
                 {
                     Id = Convert.ToInt64(r.Field<decimal>("MATERIALTYPEID")),
                     Description = r.Field<string>("MATDESCRIPTION"),

                 })
                 .ToList() ?? new List<MasterData>();

            ViewBag.ddlMaterialType = dataList;

            //Bount Material Group
            oDs = new DataSet();
            oDs = oMasterQueries.GetMaterialGroupList();

            dataList = oDs.Tables[0]?.AsEnumerable()
                .Select(r => new MasterData
                {
                    Id = Convert.ToInt64(r.Field<decimal>("MATERIALGROUPID")),
                    Description = r.Field<string>("MATERIALDESC"),

                })
                .ToList() ?? new List<MasterData>();

            ViewBag.ddlMaterialGroup = dataList;

            //Bound Unit of Measurement
            oDs = new DataSet();
            oDs = oMasterQueries.GetUOMList();
            dataList = oDs.Tables[0]?.AsEnumerable()
              .Select(r => new MasterData
              {
                  Id = Convert.ToInt64(r.Field<decimal>("UOMID")),
                  Description = r.Field<string>("UOMDESC"),

              })
              .ToList() ?? new List<MasterData>();

            ViewBag.ddlUOM = dataList;

            //Bound valuation class
            string isSales = "";
            DataSet dataSet = _mmService.PopulateValuationClassByMaterialType(0, ref isSales);
            if (dataSet.Tables.Count > 0)
            {
                dataList = dataSet.Tables[0]?.AsEnumerable()
            .Select(r => new MasterData
            {
                Id = Convert.ToInt64(r.Field<decimal>("MATERIALGROUPID")),
                Description = r.Field<string>("MATGDESCRIPTION"),

            })
            .ToList() ?? new List<MasterData>();
                ViewBag.ddlMaterialGroup = dataList;
            }

            ViewBag.isSales = isSales;

            //BOUND MATERIAL INDICATER
            oDs = new DataSet();
            oDs = oMasterQueries.GetMMIndicatorList("", "");

            dataList = oDs.Tables[0]?.AsEnumerable()
           .Select(r => new MasterData
           {
               Id = Convert.ToInt64(r.Field<short>("MMINDICATERID")),
               Description = r.Field<string>("DDLVALUE"),

           })
           .ToList() ?? new List<MasterData>();
            //dataList.Insert(0, new MasterData { Code = "0", Description = "--Select--" });
            ViewBag.ddlindicator = dataList;
            dataList = new List<MasterData>();
            ViewBag.ddlValuationClass = dataList;
        }
        private void BindMasterData()
        {
            try
            {
                List<MasterData> dataList = new List<MasterData>();
                DataSet ds = new DataSet();
                // Bind Loading Group
                ds = oMasterQueries.GetMasterData("LOADING_GROUP");

                dataList = ds.Tables[0]?.AsEnumerable()
                .Select(r => new MasterData
                {
                    Code = r.Field<string>("CODE"),
                    Description = r.Field<string>("CODE_DESC"),

                })
                .ToList() ?? new List<MasterData>();
                ViewBag.ddlLoadingGrp = dataList;

                //Bind Transport Group
                ds = oMasterQueries.GetMasterData("TRANSPORT_GROUP");

                dataList = ds.Tables[0]?.AsEnumerable()
               .Select(r => new MasterData
               {
                   Code = r.Field<string>("CODE"),
                   Description = r.Field<string>("CODE_DESC"),

               })
               .ToList() ?? new List<MasterData>();
                ViewBag.ddlTrasnsportGrp = dataList;

                //Bind Item Category Group
                ds = oMasterQueries.GetMasterData("ITEM_GROUP_CATEG");

                dataList = ds.Tables[0]?.AsEnumerable()
             .Select(r => new MasterData
             {
                 Code = r.Field<string>("CODE"),
                 Description = r.Field<string>("CODE_DESC"),

             })
             .ToList() ?? new List<MasterData>();
                ViewBag.ddlItemCategGroup = dataList;

                //Bind Distribution channel
                ds = oMasterQueries.GetMasterData("DISTRIBUTION");

                dataList = ds.Tables[0]?.AsEnumerable()
              .Select(r => new MasterData
              {
                  Code = r.Field<string>("CODE"),
                  Description = r.Field<string>("CODE_DESC"),

              })
                .ToList() ?? new List<MasterData>();
                ViewBag.ddlDisbChnn = dataList;

                //Bind Storage location

                string plantid = (string)ViewBag.SELECTEDPLANT ?? "0";
                ds = oMasterQueries.GetMasterData("STORAGE_LOCATION", plantid);

                dataList = ds.Tables[0]?.AsEnumerable()
            .Select(r => new MasterData
            {
                Code = r.Field<string>("CODE"),
                Description = r.Field<string>("CODE_DESC"),

            })
              .ToList() ?? new List<MasterData>();
                ViewBag.ddlStorageLoc = dataList;
                //Bind Availabiltiy Check
                ds = oMasterQueries.GetMasterData("AVAILABILITY");

                dataList = ds.Tables[0]?.AsEnumerable()
           .Select(r => new MasterData
           {
               Code = r.Field<string>("CODE"),
               Description = r.Field<string>("CODE_DESC"),

           })
             .ToList() ?? new List<MasterData>();
                ViewBag.ddlCheckAvailCheck = dataList;
                //Bind Gen. Item Cat Grp
                ds = oMasterQueries.GetMasterData("GEN_ITEM_CAT_GROUP");
                dataList = ds.Tables[0]?.AsEnumerable()
           .Select(r => new MasterData
           {
               Code = r.Field<string>("CODE"),
               Description = r.Field<string>("CODE_DESC"),

           })
             .ToList() ?? new List<MasterData>();
                ViewBag.ddlGenItemCatgGrp = dataList;
                //Bind Unit of Measurement
                DataSet oDs = new DataSet();

                oDs = oMasterQueries.GetUOMList();

                dataList = oDs.Tables[0]?.AsEnumerable()
         .Select(r => new MasterData
         {
             Code = Convert.ToString(r.Field<decimal>("UOMID")),
             Description = r.Field<string>("UOMDESC"),

         })
            .ToList() ?? new List<MasterData>();
                ViewBag.ddlBaseUnitOfMeasure = dataList;
                //Bind MATERIAL INDICATER
                ds = new DataSet();
                ds = oMasterQueries.GetMasterData("SALESTAX");

                dataList = ds.Tables[0]?.AsEnumerable()
         .Select(r => new MasterData
         {
             Code = r.Field<string>("CODE"),
             Description = r.Field<string>("CODE_DESC"),

         })
           .ToList() ?? new List<MasterData>();
                ViewBag.ddlTaxClassiFication = dataList;

          //Bind MPN_PROFILE
                ds = new DataSet();
                 ds = oMasterQueries.GetMasterData("MPN_PROFILE");                
                dataList = ds.Tables[0]?.AsEnumerable()
       .Select(r => new MasterData
       {
           Code = r.Field<string>("CODE"),
           Description = r.Field<string>("CODE_DESC"),

       })
         .ToList() ?? new List<MasterData>();
                ViewBag.ddlMPNProfile = dataList;

          //Bind MPN_PROFILE
                ds = new DataSet();
                ds = oMasterQueries.GetMasterData("PLANT_SPECIFIC_STATUS");
                dataList = ds.Tables[0]?.AsEnumerable()
       .Select(r => new MasterData
       {
           Code = r.Field<string>("CODE"),
           Description = r.Field<string>("CODE_DESC"),

       })
         .ToList() ?? new List<MasterData>();
                ViewBag.ddlPlantSpMaterialStatus = dataList;


            }
            catch (Exception e) {
                _logger.LogError(e, "BindMasterData");
            }
        }
        private void BindApprovalAuthority()
        {
            int _empCode = _session.Get<int>("userID");
            List<MasterData> dataList = new List<MasterData>();
            DataSet oDs = new DataSet();
            oDs = oMasterQueries.GetOperationApprovalAuthorityList(_empCode);
            try
            {
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    bool flag = false;
                    if (oDs.Tables[0].Rows[0]["ADEMPCODE"].ToString() != "")
                    {
                        //ddlApprovalAuthority.DataSource = oDs;
                        //ddlApprovalAuthority.DataTextField = "EMPNAME";
                        //ddlApprovalAuthority.DataValueField = "ADEMPCODE";
                        //ddlApprovalAuthority.DataBind();
                        //ddlApprovalAuthority.Items.Insert(0, new ListItem("-- select --", "0"));
                        //txtapprovalauthority = true;
                        dataList = oDs.Tables[0]?.AsEnumerable()
                       .Select(r => new MasterData
                       {
                           Id = r.Field<long>("ADEMPCODE"),
                           Description = r.Field<string>("EMPNAME"),

                       })
                       .ToList() ?? new List<MasterData>();
                    }
                    else
                    {
                        //ddlApprovalAuthority.Items.Insert(0, new ListItem("-- select --", "0"));
                        //txtapprovalauthority = true;
                    }
                    ViewBag.ddlApprovalAuthority = dataList;
                    ViewBag.txtapprovalauthority = flag;
                }
                else
                {
                    //ShowInfo("Your approving authority not updated kindly contact to HR.");
                    //Reset();
                    ViewBag.ddlApprovalAuthority = new List<MasterData>();
                }
                
            }
            catch (Exception)
            {
                ViewBag.ddlApprovalAuthority = new List<MasterData>();
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> PlantMstChange([FromBody] MasterData data)
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string ProfitCentre = "";
                MasterResponse res = _mmService.GetProfitCentreAndStorageLoc(data.Code);

                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MaterialMasterCreateRequest");
                return Json(new { Rs = 0, Message = ex });
            }

        }
        [HttpPost]
        public async Task<IActionResult> MaterialTypeChange([FromBody] MasterData data)
        {

            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                MasterResponse res = _mmService.PopulateValuationClassByMaterialType(Convert.ToInt32(data.Code));
                //SetSalesView();
                //Visible Package fields
                // MateriaTypeID 6 for ZPAC                

                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MaterialMasterCreateRequest");
                return Json(new { Rs = 0, Message = ex });
            }



        }
        [HttpPost]
        public async Task<IActionResult> MaterialGroupChange([FromBody] MasterData data)
        {
            try
            {
                if (data == null)
                {
                    return Json(new { Rs = 0, Message = "Invalid ajax calling" });
                }
                else if (data.Id == null || data.Id == 0)
                {
                    return Json(new { Rs = 0, Message = "Invalid Material Type" });
                }
                else if (data.Code == null || data.Code == "0")
                {
                    return Json(new { Rs = 0, Message = "Invalid Material Group Type!" });
                }
                MasterResponse res = _mmService.PopulateValuationClassByMaterialTypeGroup(Convert.ToInt32(data.Id), Convert.ToInt32(data.Code));
                //SetSalesView();
                //Visible Package fields
                // MateriaTypeID 6 for ZPAC                

                return Json(res);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MaterialGroupChange");
                return Json(new { Rs = 0, Message = ex });
            }



        }
        [HttpPost]
        //AddMore
        public async Task<IActionResult> AddMoreMaterial([FromBody] MMDetailViewModel data)
        {
            List<MaterialDetail> dataList = new List<MaterialDetail>();
            try
            {
                if (data == null)
                {
                    return Json(new { Rs = 0, Message = "Invalid ajax calling" });
                }
                Employee_Details employee = _session.Get<Employee_Details>("Employee");
                int UserId = _session.Get<int>("userID");
                int ppcApprovalAuth = 0; int financeApprovalAuth = 0;
                int plantCode = data.PLANTCODE ?? 0;
                int materialType = data.MATERIALTYPE ?? 0;// Convert.ToInt32(ddlMaterialType.SelectedValue);
                int materialGroup = data.MATERIALGROUP ?? 0;// Convert.ToInt32(ddlMaterialGroup.SelectedValue);
                int valuationClass = data.VALUATIONCLASS ?? 0; //Convert.ToInt32(ddlValuationClass.SelectedValue);
                string materialDesription = data.MATERIALDISCRIPTION ?? "";// txtMaterialDescription.Text.ToUpper().Trim();
                string materialSpecification = data.MATERIALSPECIFICATION ?? ""; //txtMaterialSpecification.Text.ToUpper().Trim();
                decimal price = data.PRICE ?? 0; //(txtPrice.Text == "" ? Convert.ToDecimal(1) : Convert.ToDecimal(txtPrice.Text));
                int UOM = data.MEASUREMENTUNIT ?? 0; //Convert.ToInt32(ddlUOM.SelectedValue);
                string emailId = data.EMAIL_ID ?? employee.EMail_Id;
                int headerId = 0;
                if (data.MMHEADERID != 0)
                {
                    data.MMHEADERID = data.MMHEADERID;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_session.Get<string>("MMHeaderIdS")))
                    {
                        data.MMHEADERID = _session.Get<int>("MMHeaderIdS");

                    }
                }
                headerId = data.MMHEADERID ?? 0;
                string err = "";
                //Bound Details material count not allow more than 20 on 15-Feb-2017
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialCountDetails(headerId);
                if (ds.Tables[0].Rows.Count >= 20)
                {
                    //ShowError("Only 20 material code request can be added in a request.");
                    //Reset();
                    return Json(new { Rs = 0, Message = "Only 20 material code request can be added in a request." });
                }
                else
                {
                    //if ((!Convert.ToBoolean(hdIsPPCApprovalRequired.Value) || ppcApprovalAuth != 0) && plantCode != 0 && materialType != 0 && materialGroup != 0 && valuationClass != 0 && !string.IsNullOrEmpty(materialDesription) && Convert.ToInt32(price) != 0 && UOM != 0 && !string.IsNullOrEmpty(emailId))
                    if (plantCode != 0 && materialType != 0 && materialGroup != 0 && valuationClass != 0 && !string.IsNullOrEmpty(materialDesription) && Convert.ToInt32(price) != 0 && UOM != 0 && data.MMINDICATER != "0" && !string.IsNullOrEmpty(emailId) && !(data.MATERIALTYPETEXT ?? "").StartsWith("ZMPN"))
                    {
                        //if (MaterialDescCheck())
                        //{
                        string output;
                        if (data.MMDETAILID == null || data.MMDETAILID == 0)
                        {
                            if (data.IsSales == "N")
                            {
                                /*This is OLD method for sales creation*/
                                // output = oMasterQueries.AddMarerialCodeCreationRequest(headerId, plantCode, materialDesription, materialSpecification, materialType, materialGroup, valuationClass, UOM, price, Convert.ToInt32(((Employee_Details)Session["Employee"]).Employee_Code), emailId, ppcApprovalAuth, financeApprovalAuth, txthsncode.Text, ddlindicator.SelectedValue.ToString());
                                output = oMasterQueries.AddMarerialCodeCreationWithSalesRequest(headerId, plantCode, materialDesription, materialSpecification, materialType, materialGroup, valuationClass, UOM, price, UserId, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE, data.MMINDICATER, "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                            }
                            else
                            {
                                output = oMasterQueries.AddMarerialCodeCreationWithSalesRequest(headerId, plantCode, materialDesription, materialSpecification, materialType, materialGroup, valuationClass, UOM, price, UserId, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE, data.MMINDICATER, data.TRANSPORTATIONGROUP, data.LOADINGGROUP, data.BASEUNITOFMEASURE, data.SALES_ORG, data.DISTRI_CHN, data.ITEM_CATG_GRP, data.AVAIL_CHK, data.PROFITCENTER, data.STORAGE_LOC, data.TAX_CLASSIFICATION, data.GEN_ITEM_CAT_GRP, data.MAT_GRP_PACK_MATLS ?? "".ToUpper(), data.PACKAGING_MAT_TYPE, "");
                            }

                            if (output != "1")
                            {
                                return Json(new { Rs = 0, Message = output });
                            }
                            else
                            {
                                dataList = BindMMCreationDetails();
                                return Json(new { Rs = 1, Message = "Material master creation requested is added successfully.", dataList });
                                //Reset();

                            }
                        }
                        else
                        {
                            if (data.IsSales == "Y")
                            {
                                output = oMasterQueries.UpdateMarerialCodeCreationRequest(headerId, data.MMDETAILID ?? 0, plantCode, materialDesription, materialSpecification, materialType, materialGroup, valuationClass, UOM, price, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE, data.MMINDICATER, UserId, data.TRANSPORTATIONGROUP, data.LOADINGGROUP, data.BASEUNITOFMEASURE, data.SALES_ORG, data.DISTRI_CHN, data.ITEM_CATG_GRP, data.AVAIL_CHK, data.PROFITCENTER, data.STORAGE_LOC, data.TAX_CLASSIFICATION, data.GEN_ITEM_CAT_GRP, data.MAT_GRP_PACK_MATLS ?? "".ToUpper(), data.PACKAGING_MAT_TYPE, "");
                            }
                            else
                            {
                                output = oMasterQueries.UpdateMarerialCodeCreationRequest(headerId, data.MMDETAILID ?? 0, plantCode, materialDesription, materialSpecification, materialType, materialGroup, valuationClass, UOM, price, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE, data.MMINDICATER, UserId, "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                            }
                            if (output != "1")
                            {
                                // btnAddMore.Text = "Update";
                                return Json(new { Rs = 0, Message = output });
                            }
                            else
                            {
                                dataList = BindMMCreationDetails();
                                //btnAddMore.Text = "Add More";
                                //ShowInfo();
                                return Json(new { Rs = 1, Message = "Material master creation requested (" + data.MMDETAILID.ToString() + ") is updated successfully.", dataList });
                                //ViewState["EditMMDetailId"] = null;
                                //Reset();
                                //BindMMCreationDetails();
                            }

                        }
                        //Reset();
                        //}
                    }

                    /// ZMPN Creation 
                    /// added by Ajit TTL
                    else if ((data.MATERIALTYPETEXT ?? "").StartsWith("ZMPN") && materialType != 0 && materialGroup != 0 && !string.IsNullOrEmpty(materialDesription) && (data.INT_MATERIAL_NO ?? "").Trim() != "")
                    {
                        if ((data.INT_MATERIAL_NO ?? "").Length < 10)
                        {
                            return Json(new { Rs = 0, Message = "Int. Material code must be in 10 digits" });
                        }
                        string output;
                        if (data.MMDETAILID == null || data.MMDETAILID == 0)
                        {
                            //output = oMasterQueries.AddMarerialCodeCreationWithSalesRequest(headerId, plantCode, materialDesription, materialSpecification, materialType, materialGroup, valuationClass, UOM, price, UserId, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE, data.MMINDICATER, data.TRANSPORTATIONGROUP,data.LOADINGGROUP,data.BASEUNITOFMEASURE,data.SALES_ORG,data.DISTRI_CHN,data.ITEM_CATG_GRP,data.CHECKINGGRPAVAILABILITYCHECK,data.PROFITCENTER,data.STORAGE_LOC,data.TAX_CLASSIFICATION,data.GEN_ITEM_CAT_GRP,data.MAT_GRP_PACK_MATLS??"".ToUpper(),data.PACKAGING_MAT_TYPE, "");
                            output = oMasterQueries.AddMarerialCodeCreationWithSalesRequest(headerId, plantCode, materialDesription, "", materialType, materialGroup, 0, 0, 0, UserId, emailId, ppcApprovalAuth, financeApprovalAuth, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", data.INT_MATERIAL_NO ?? "".Trim());
                            if (output != "1")
                            {
                                //ShowError(output);
                                return Json(new { Rs = 0, Message = output });
                            }
                            else
                            {
                                dataList = BindMMCreationDetails();
                                //ShowInfo("Material master creation requested is added successfully.");
                                return Json(new { Rs = 1, Message = "Material master creation requested is added successfully.", dataList });
                                //Reset();
                                //BindMMCreationDetails();
                            }
                        }
                        else
                        {
                            output = oMasterQueries.UpdateMarerialCodeCreationRequest(headerId, UserId, plantCode, materialDesription, "", materialType, materialGroup, 0, 0, 0, emailId, ppcApprovalAuth, financeApprovalAuth, "", "", UserId, "", "", "", "", "", "", "", "", "", "", "", "", "", data.INT_MATERIAL_NO ?? "".Trim());
                            if (output != "1")
                            {
                                //btnAddMore.Text = "Update";
                                //ShowError(output);
                                return Json(new { Rs = 0, Message = output });
                            }
                            else
                            {
                                dataList = BindMMCreationDetails();
                                //btnAddMore.Text = "Add More";
                                //ShowInfo("Material master creation requested (" + ViewData["EditMMDetailId"].ToString() + ") is updated successfully.");
                                //ViewState["EditMMDetailId"] = null;
                                //Reset();
                                //BindMMCreationDetails();
                                return Json(new { Rs = 1, Message = "Material master creation requested (" + data.MMDETAILID.ToString() + ") is updated successfully.", dataList });

                            }
                        }
                    }
                    else
                    {
                        //ShowInfo("Kindly check required fields.");
                        return Json(new { Rs = 0, Message = "Kindly check required fields." });
                        //Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddMoreMaterial");
                return Json(new { Rs = 0, Message = ex });
            }
        }
        private List<MaterialDetail> BindMMCreationDetails()
        {
            List<MaterialDetail> dataList = new List<MaterialDetail>();
            try
            {
                int UserID = _session.Get<int>("userID");
                DataSet ds = _mmService.BindMMCreationDetails(UserID);

                if (ds != null)
                {
                    int _MMHEADERID = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.MMHeaderId = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                        _session.Set<string>("MMHeaderIdS", ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        _MMHEADERID = Convert.ToInt32(ds.Tables[0].Rows[0]["MMHEADERID"].ToString());

                    }
                    else
                    {
                        ViewBag.MMHeaderId = "0";
                        _session.Set<string>("MMHeaderIdS", "");
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {

                        ViewBag.SELECTEDPLANT = ds.Tables[1].Rows[0]["PLANTCODE"].ToString();
                        DataSet oDs1 = oMasterQueries.GetPlantDetails(ViewBag.SELECTEDPLANT);
                        if (oDs1.Tables[0] != null && oDs1.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.PROFITCENTER = oDs1.Tables[0].Rows[0]["PROFIT_CENTER"].ToString();
                        }

                        dataList = ds.Tables[1]?.AsEnumerable()
                .Select(r => new MaterialDetail
                {
                    MMDETAILID = Convert.ToInt32(r.Field<decimal>("MMDETAILID")),
                    MMHEADERID = _MMHEADERID,
                    MATERIALDISCRIPTION = r.Field<string>("MATERIALDISCRIPTION"),
                    MATERIALSPECIFICATION = r.Field<string>("MATERIALSPECIFICATION"),
                    MMDESCSPEC = r.Field<string>("MMDESCSPEC"),
                    MATERIALTYPE = r.Field<string>("MATERIALTYPE"),
                    MATERIALGROUP = Convert.ToInt32(r.Field<decimal>("MATERIALGROUP")),
                    VALUATIONCLASS = Convert.ToInt32(r.Field<decimal>("VALUATIONCLASS")),
                    UOM = r.Field<string>("UOM"),
                    PRICE = Convert.ToDouble(r.Field<decimal>("PRICE")),
                    PLANTNAMEDESC = r.Field<string>("PLANTNAMEDESC"),
                    PLANTNAME = r.Field<string>("PLANTNAME"),
                    PLANT = r.Field<string>("PLANT"),
                    PLANTCODE = Convert.ToInt32(r.Field<decimal>("PLANTCODE")),
                    IsSales = r.Field<string>("REQ_TYPE")
                })
                .ToList() ?? new List<MaterialDetail>();
                        ViewBag.DraftDetails = dataList;
                    }
                }
                else
                {
                    ViewBag.MMHeaderId = "0";
                    _session.Set<string>("MMHeaderIdS", "");
                    ViewBag.SELECTEDPLANT = "0";
                    ViewBag.PROFITCENTER = "";
                    ViewBag.DraftDetails = new List<MaterialDetail>();
                }
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BindMMCreationDetails");
                return dataList;
            }
        }
        public void BindMMCreationRejectDetails()
        {
            List<MaterilaRejectedList> dataList = new List<MaterilaRejectedList>();
            try
            {
                int UserID = _session.Get<int>("userID");
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialCreationRejectedDetails(UserID);
                if (ds.Tables[1].Rows.Count > 0)
                {
                    dataList = ds.Tables[1]?.AsEnumerable()
            .Select(r => new MaterilaRejectedList
            {
                MMHEADERID = Convert.ToInt32(r.Field<decimal>("MMHEADERID")),
                MMHEADERDETAILID = r.Field<string>("MMHEADERDETAILID"),
                REQUESTERID = Convert.ToInt32(r.Field<long>("REQUESTERID")),
                REQUESTERNAME = r.Field<string>("REQUESTERNAME"),
                REQUESTDATE = Convert.ToString(r.Field<DateTime>("REQUESTDATE"))

            })
            .ToList() ?? new List<MaterilaRejectedList>();
                    ViewBag.RejectedDataList = dataList;
                }
                else
                {
                    ViewBag.RejectedDataList = dataList;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BindMMCreationRejectDetails");
                ViewBag.RejectedDataList = dataList;
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetMaterialItemEdit([FromBody] MasterData param)
        {
            try {
            if(param==null || param.Code == null || param.Code == "0")
              {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
              } 
            int MMDetailId=Convert.ToInt32(param.Code);
            MMDetailEdit data = new MMDetailEdit();

            DataSet ds = new DataSet();
          
            ds = oMasterQueries.GetMMCreationDetailById(MMDetailId);
            if (ds==null || ds.Tables.Count <=0 || ds.Tables[0].Rows.Count<=0) {
                data.RS = 0;
                data.MESSAGE = "Error in data binding!";
                return Json(data);
            }
            else
            {
                data.RS = 1;
            }

              string IsSale = "N";
            if (ds.Tables[0].Rows[0]["MATERIALTYPE"].ToString() != "0")
            {
                DataSet oDs = PopulateValuationClassByMaterialType(Convert.ToInt32(ds.Tables[0].Rows[0]["MATERIALTYPE"].ToString()), ref IsSale);

                var dataList = oDs.Tables[0]?.AsEnumerable()
                    .Select(r => new MasterData
                    {
                        Id = Convert.ToInt64(r.Field<decimal>("MATERIALGROUPID")),
                        Description = r.Field<string>("MATGDESCRIPTION"),

                    })
                    .ToList() ?? new List<MasterData>();
                data.MaterialGroupList = dataList;
            }
            else
            {
                data.MaterialGroupList = new List<MasterData>();
            }
            data.ISSALES = IsSale;
            data.PLANTCODE = ds.Tables[0].Rows[0]["PLANTCODE"].ToString();
            data.MATERIALTYPE = ds.Tables[0].Rows[0]["MATERIALTYPE"].ToString();
            data.MATERIALGROUP = ds.Tables[0].Rows[0]["MATERIALGROUP"].ToString();
            data.VALUATIONCLASS = ds.Tables[0].Rows[0]["VALUATIONCLASS"].ToString();
            data.INT_MATERIAL_NO = ds.Tables[0].Rows[0]["INT_MATERIAL_NO"].ToString();
            data.ZMPNDESC = ds.Tables[0].Rows[0]["INT_MATERIAL_NO"].ToString() + "-" + ds.Tables[0].Rows[0]["MATERIALDISCRIPTION"].ToString();
            data.MATERIALDISCRIPTION = ds.Tables[0].Rows[0]["MATERIALDISCRIPTION"].ToString();
            data.MEASUREMENTUNIT = ds.Tables[0].Rows[0]["MEASUREMENTUNIT"].ToString();
            data.MATERIALSPECIFICATION = ds.Tables[0].Rows[0]["MATERIALSPECIFICATION"].ToString();
            data.PRICE = ds.Tables[0].Rows[0]["PRICE"].ToString();
            data.HSNCODE = ds.Tables[0].Rows[0]["HSNCODE"].ToString();
            data.MMINDICATER = ds.Tables[0].Rows[0]["MMINDICATER"].ToString();
            data.ITEM_CATG_GRP = ds.Tables[0].Rows[0]["ITEM_CATG_GRP"].ToString();
            data.GEN_ITEM_CAT_GRP = ds.Tables[0].Rows[0]["GEN_ITEM_CAT_GRP"].ToString();
            data.AVAIL_CHK = ds.Tables[0].Rows[0]["AVAIL_CHK"].ToString();
            data.TAX_CLASSIFICATION = ds.Tables[0].Rows[0]["TAX_CLASSIFICATION"].ToString();
            data.TRANSPORTATIONGROUP = ds.Tables[0].Rows[0]["TRANSPORTATIONGROUP"].ToString();
            data.LOADINGGROUP = ds.Tables[0].Rows[0]["LOADINGGROUP"].ToString();
            data.DISTRI_CHN = ds.Tables[0].Rows[0]["DISTRI_CHN"].ToString();
            data.BASEUNITOFMEASURE = ds.Tables[0].Rows[0]["BASEUNITOFMEASURE"].ToString();
            data.PROFITCENTER = ds.Tables[0].Rows[0]["PROFITCENTER"].ToString();
            data.PACKAGING_MAT_TYPE = ds.Tables[0].Rows[0]["PACKAGING_MAT_TYPE"].ToString();
            data.MAT_GRP_PACK_MATLS = ds.Tables[0].Rows[0]["MAT_GRP_PACK_MATLS"].ToString();
            data.STORAGE_LOC = ds.Tables[0].Rows[0]["STORAGE_LOC"].ToString();
            data.SALES_ORG = ds.Tables[0].Rows[0]["SALES_ORG"].ToString();
            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["STORAGE_LOC"].ToString()))
            {
                DataSet oDs = GetProfitCentreAndStorageLoc(data.PLANTCODE ?? "0");
                data.PROFITCENTER = oDs.Tables[0].Rows[0]["PROFIT_CENTER"].ToString();
                DataSet ds1 = oMasterQueries.GetMasterData("STORAGE_LOCATION", data.PLANTCODE ?? "0");
                var dataList = ds1.Tables[0]?.AsEnumerable()
           .Select(r => new MasterData
           {
               Code = r.Field<string>("CODE"),
               Description = r.Field<string>("CODE_DESC"),

           })
             .ToList() ?? new List<MasterData>();

                data.StorageLocList = dataList;
            }

            if (Convert.ToInt32(data.MATERIALTYPE) != 0 && Convert.ToInt32(data.MATERIALGROUP) != 0)
            {
                //Bound valuation Class
                DataSet oDs = new DataSet();
                oDs = oMasterQueries.GetValuationClassByMaterialTypeGroup(Convert.ToInt32(data.MATERIALTYPE), Convert.ToInt32(data.MATERIALGROUP));
                var dataList = oDs.Tables[0]?.AsEnumerable()
                   .Select(r => new MasterData
                   {
                       Id = Convert.ToInt64(r.Field<decimal>("VALUATIONCLASSID")),
                       Description = r.Field<string>("VALDESCRIPTION"),

                   })
                   .ToList() ?? new List<MasterData>();
                data.ValuationClassList = dataList;
            }
            else
            {
                data.ValuationClassList = new List<MasterData>();
            }


            return Json(data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMaterialItemEdit");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        private DataSet PopulateValuationClassByMaterialType(int matrialType, ref string chSales)
        {
            DataSet oDs = new DataSet();
            try {
              
                oDs = oMasterQueries.GetMaterialGroupByMaterialType(matrialType);

                DataSet oDs1 = new DataSet();
                oDs1 = oMasterQueries.GetISPURCHASISSALEById(matrialType);
                if (oDs1.Tables[0].Rows[0]["ISSALES"].ToString() == "1")
                {
                    chSales = oDs1.Tables[0].Rows[0]["ISSALES"].ToString();

                }
                else
                {
                    chSales = "N";
                }

            } catch (Exception ex) {
                _logger.LogError(ex, "PopulateValuationClassByMaterialType");
            }
           
            return oDs;
        }
        private DataSet GetProfitCentreAndStorageLoc(string plantid)
        {
            DataSet oDs = new DataSet();
            try
            {
                if (Convert.ToInt32(plantid) > 0)
                {
                    oDs = oMasterQueries.GetPlantDetails(plantid);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProfitCentreAndStorageLoc");
            }
            return oDs;
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMaterialItem([FromBody] MasterData param)
        {
            try
            {
                string _MESSAGE = "";
                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
                string output = oMasterQueries.DeleteMarerialCodeCreationRequest(Convert.ToInt32(param.Code));
                DataSet ds = oMasterQueries.MM_HeaderDetailDeleteId_GET(Convert.ToInt32(param.Code));
                if (output != "1")
                {                    
                    return Json(new { RS = 0, MESSAGE = output });
                }
                else
                {
                    //ShowInfo("Material master creation request (" + e.CommandArgument.ToString() + ") is deleted successfully.");
                    if (ds.Tables[1].Rows[0]["ITEMCOUNT"].ToString() != "0")
                    {
                        _MESSAGE="Material master creation request (" + ds.Tables[0].Rows[0]["MMDETAILHID"].ToString() + ") is deleted successfully.";
                    }
                    else
                    {
                        _MESSAGE= "Material master creation request (" + ds.Tables[0].Rows[0]["HEADERID"].ToString() + ") is deleted successfully.";
                    }
                }
              var  dataList = BindMMCreationDetails();



                return Json(new { RS = 0, MESSAGE = _MESSAGE, dataList });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteMaterialItem");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        [HttpPost]
        public async Task<IActionResult> SubmitMaterialRequest([FromBody] MasterData param)
        {
            try
            {
               
                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
                string MMHeaderID = _session.Get<string>("MMHeaderIdS");
                int UserID = _session.Get<int>("userID");
                string message = "";
                int rs = 0;
                List < MaterialDetail > dataList1=new List<MaterialDetail>();

                if (MMHeaderID != "")
                {
                    
                    int MMHeaderId = Convert.ToInt32(MMHeaderID);
                    DataSet ds = oMasterQueries.MMDRAFT_HeaderDetailId_GET(MMHeaderId);
                    var dataList= BindMMCreationDetails();
                    if (dataList.Count > 0)
                    {
                       
                         int   _ApprovalCode = Convert.ToInt32(param.Code);
                       
                        string output = oMasterQueries.SubmitMaterialMasterRequest(MMHeaderId, _ApprovalCode);
                        if (output != "1")
                        {
                             message = output;
                             rs = 0;
                        }
                        else
                        {
                            rs = 1;
                            message="Material master creation request (" + ds.Tables[0].Rows[0]["MMHEADERDETAILID"].ToString() + ") sent successfully for approval.";
                        }
                        // Reset();
                        dataList1=BindMMCreationDetails();
                        //BindApprovalAuthorityDefault();
                    }
                    else
                    {
                         DataSet dsCreation = new DataSet();
                        //Bound old request saved in draft
                        dsCreation = oMasterQueries.GetMaterialCreationPendingDetails(UserID);
                        if (dsCreation.Tables[1].Rows.Count > 0)
                        {
                            string output = oMasterQueries.SubmitMaterialMasterRequestDelete(MMHeaderId);
                            if (output != "1")
                            {
                                rs = 0;
                                message=output;
                            }
                            else
                            {
                                rs = 1;
                                message="Material master creation request (" + ds.Tables[0].Rows[0]["MMHEADERDETAILID"].ToString() + ") sent successfully for approval.";
                            }
                            dataList1 = BindMMCreationDetails();
                        }
                        else
                        {
                            message="No Material request added for creation";
                        }
                    }
                }



                return Json(new { RS = rs, MESSAGE = message, dataList = dataList1 });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddMoreMaterial");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        #region report
        [HttpGet]
        public async Task<IActionResult> MMStatusRequestReport()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string UserID = _session.Get<string>("userID");
                PopulateControlsforReport();
                List<MasterData> ddlDayFrom = new List<MasterData>();
                int intVal_day = 1;
                do
                {
                    ddlDayFrom.Add(new MasterData { Id = intVal_day, Code = intVal_day.ToString() });
                    intVal_day = intVal_day + 1;
                }
                while (intVal_day < 32);
                //ddlDayFrom.Insert(0,new MasterData { Id = 0, Code = "-DD-" });
                ViewBag.ddlDayFrom = ddlDayFrom;
                List<MasterData> ddlMonthFrom = new List<MasterData>();
                for (int i = 0; i <= 11; i++)
                {
                    ddlMonthFrom.Add(new MasterData { Code = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i], Id=(i + 1) });
                }
                //ddlMonthFrom.Insert(0, new MasterData { Id = 0, Code = "-MM-" });               
                ViewBag.ddlMonthFrom = ddlMonthFrom;
                List<MasterData> ddlYearFrom = new List<MasterData>();
                int currentYr = DateTime.Now.Year;
                for (int i = currentYr; i > (currentYr - 20); i--)
                {
                    ddlYearFrom.Add(new MasterData { Code=i.ToString(), Id=i });
                }
                //ddlYearFrom.Insert(0, new MasterData { Code = "-YYYY-", Id = 0 });
                ViewBag.ddlYearFrom = ddlYearFrom;

                List<MasterData> ddlDayTo = new List<MasterData>();
                intVal_day = 1;
                do
                {
                    ddlDayTo.Add(new MasterData { Code=intVal_day.ToString(), Id=intVal_day });
                    intVal_day = intVal_day + 1;
                }
                while (intVal_day < 32);
               // ddlDayTo.Insert(0, new MasterData { Code="-DD-", Id=0 });
                ViewBag.ddlDayTo = ddlDayTo;

                List<MasterData> ddlMonthTo = new List<MasterData>();
                for (int i = 0; i <= 11; i++)
                {
                    ddlMonthTo.Add(new MasterData { Code = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i], Id = (i + 1) });
                }
                //ddlMonthTo.Insert(0, new MasterData {Code= "-MM-", Id=0 });
                ViewBag.ddlMonthTo = ddlMonthTo;

                List<MasterData> ddlYearTo = new List<MasterData>();
                currentYr = DateTime.Now.Year;
                for (int i = currentYr; i > (currentYr - 20); i--)
                {
                    ddlYearTo.Add(new MasterData { Code = i.ToString(), Id = i });
                }
                //ddlYearTo.Insert(0, new MasterData { Code = "-YYYY-", Id = 0 });
                ViewBag.ddlYearTo = ddlYearTo;

                Employee_Details employee_Details = _session.Get<Employee_Details>("Employee"); ;
                string _sisUser = oMasterQueries.showMMSISAuthPage(UserID);
                if (_sisUser == "1")
                {
                    ViewBag.tdRequestor1 = true;
                    ViewBag.tdRequestor2 = true;
                    ViewBag.tdRequestor3 = false;
                    ViewBag.tdRequestor4 = false;
                }
                else
                {
                    ViewBag.tdRequestor1 = false;
                    ViewBag.tdRequestor2 = false;
                    ViewBag.tdRequestor3 = true;
                    ViewBag.tdRequestor4 = true;
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MMStatusRequestReport");
                return View();
            }

        }
        private void PopulateControlsforReport()
        {
            DataSet oDs = new DataSet();
            List<MasterData> dataList = new List<MasterData>();
            //BOUND MATERIALTYPE
            oDs = new DataSet();
            oDs = oMasterQueries.GetMaterialTypeList();          
            dataList = oDs.Tables[0]?.AsEnumerable()
             .Select(r => new MasterData
             {
                 Id = Convert.ToInt64(r.Field<decimal>("MATERIALTYPEID")),
                 Description = r.Field<string>("MATDESCRIPTION"),

             })
             .ToList() ?? new List<MasterData>();
            ViewBag.ddlMaterialType = dataList;

            //Bount Material Group
            oDs = new DataSet();
            oDs = oMasterQueries.GetMaterialGroupList();          
            dataList = oDs.Tables[0]?.AsEnumerable()
            .Select(r => new MasterData
            {
                Id = Convert.ToInt64(r.Field<decimal>("MATERIALGROUPID")),
                Description = r.Field<string>("MATERIALDESC"),

            })
            .ToList() ?? new List<MasterData>();
            ViewBag.ddlMaterialGroup = dataList;

            //BOUND PLANT
            oDs = oMasterQueries.GetMasterPlantList();           
            dataList = oDs.Tables[0]?.AsEnumerable()
           .Select(r => new MasterData
           {
               Id = Convert.ToInt64(r.Field<decimal>("PLANTID")),
               Description = r.Field<string>("PLANTNAMEDESC"),

           })
           .ToList() ?? new List<MasterData>();
            ViewBag.ddlPlantMst = dataList;

            //Bound valuation class        
            oDs = oMasterQueries.GetValuationClassGet(0);           
            dataList = oDs.Tables[0]?.AsEnumerable()
         .Select(r => new MasterData
         {
             Id = Convert.ToInt64(r.Field<decimal>("VALUATIONCLASSID")),
             Description = r.Field<string>("VALDESCRIPTION"),

         })
         .ToList() ?? new List<MasterData>();
            ViewBag.ddlValuationClass = dataList;
        }
        [HttpPost]
        public async Task<IActionResult> SearchMMHeaderStatus([FromBody] SearchMaterilaData data)
        {
            try
            {
                List<SearchMaterialOutput> dataList = new List<SearchMaterialOutput>();
                int userId = _session.Get<int>("userID");                 
                string mmId = data.MMId?? "0";
                string status = data.Status;
                int plantCode = Convert.ToInt32(data.PlantMst);
                string month = "0"; //ddlMonth.SelectedValue;        
                DateTime dtFromDate = new DateTime();
                string fromDate = string.Empty;
                if (data.DayFrom != "0" && data.MonthFrom != "0" && data.YearFrom != "0")
                {                    
                    dtFromDate = new DateTime(Convert.ToInt32(data.YearFrom), Convert.ToInt32(data.MonthFrom), Convert.ToInt32(data.DayFrom), 00, 00, 00);
                }
                else
                {
                    dtFromDate = new DateTime(1001, 1, 1, 00, 00, 00);
                }
                // fromDate = dtFromDate.ToString();
                fromDate = dtFromDate.ToString("MM/dd/yyyy");
                DateTime dtToDate = new DateTime();
                string toDate = string.Empty;
                if (data.DayTo != "0" && data.MonthTo != "0" && data.YearTo != "0")
                {
                    dtToDate = new DateTime(Convert.ToInt32(data.YearTo), Convert.ToInt32(data.MonthTo), Convert.ToInt32(data.DayTo), 23, 59, 59);
                }
                else
                {
                    dtToDate = new DateTime(9999, 1, 1, 23, 59, 59);
                }
                //toDate = dtToDate.ToString();
                toDate = dtToDate.ToString("MM/dd/yyyy");
                //string requestorName = txtRequestorName.Text.Trim();
                string requestType = data.RequestType;
                int materialTypeId = Convert.ToInt32(data.MaterialType);
                int materialGroup = Convert.ToInt32(data.MaterialGroup);
                int valuationClass = Convert.ToInt32(data.ValuationClass);
                int RequestorId = 0;
                if (data.RequestorId != "")
                {
                    RequestorId = Convert.ToInt32(data.RequestorId);
                }
                /***********************Added by Ajit TTL (14-Apr-25) CR5865 **************************************/
                /////////////////////////////ADD NEW FILTER OPTION MATERIAL CODE//////////////////////////////////////////////////////////////
                long materialcode = 0;
                if (data.MetrialCode != "")
                {
                    Int64.TryParse(data.MetrialCode, out materialcode);
                }

                // dt = cmnMasterQueries.SearchMMHeaderStatusDetails(mmId, requestType, materialTypeId, materialGroup, valuationClass, plantCode, status, fromDate, toDate, userId, RequestorId);
               DataTable dt = oMasterQueries.SearchMMHeaderStatusDetails(mmId, requestType, materialTypeId, materialGroup, valuationClass, plantCode, status, fromDate, toDate, userId, RequestorId, materialcode);

                if (dt.Rows.Count > 0)
                {                  
                dataList = dt.AsEnumerable()
               .Select(r => new SearchMaterialOutput
               {
                   MMHEADERID = Convert.ToString(r.Field<decimal>("MMHEADERID")),
                   MMHEADERDETAILID = r.Field<string>("MMHEADERDETAILID"),
                   REQUESTDATE = Convert.ToString(r.Field<DateTime>("REQUESTDATE")),
                   REQUESTERID = Convert.ToString(r.Field<long>("REQUESTERID")),
                   REQUESTORNAME = r.Field<string>("REQUESTORNAME"),
                   STATUS = r.Field<string>("STATUS"),
                   SAP_STATTUS = r.Field<string>("SAP_STATTUS"),
                   SAP_SAPREMARKS = r.Field<string>("SAP_SAPREMARKS"),
                   TotalCount = "Total Record Found : " + Convert.ToString(dt.Rows.Count),
                   IsPPCEditable = (userId == r.Field<long>("REQUESTERID") && (r.Field<string>("STATUS") == "Pending at PPC" || r.Field<string>("STATUS") == "Pending at FIN")) ? "Y" : "N" //REQUESTERID
              
               })
               .ToList() ?? new List<SearchMaterialOutput>();               
                }
                return PartialView("_SearchMMRequest", dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchMMHeaderStatus");
                return Json(new {data= "No any record exist!" });
            }

        }
        [HttpPost]
        public ActionResult GetMMRequestExcel([FromBody] SearchMaterilaData data)
        {
            short retVal = 0;
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string userid = _session.Get<string>("userID");

                string res = "";
                List<SearchMaterialOutput> dataList = null;
               
                    int userId = _session.Get<int>("userID");
                    string mmId = data.MMId ?? "0";
                    string status = data.Status;
                    int plantCode = Convert.ToInt32(data.PlantMst);
                    string month = "0"; //ddlMonth.SelectedValue;        
                    DateTime dtFromDate = new DateTime();
                    string fromDate = string.Empty;
                    if (data.DayFrom != "0" && data.MonthFrom != "0" && data.YearFrom != "0")
                    {
                        dtFromDate = new DateTime(Convert.ToInt32(data.DayFrom), Convert.ToInt32(data.MonthFrom), Convert.ToInt32(data.YearFrom), 00, 00, 00);
                    }
                    else
                    {
                        dtFromDate = new DateTime(1001, 1, 1, 00, 00, 00);
                    }
                    // fromDate = dtFromDate.ToString();
                    fromDate = dtFromDate.ToString("MM/dd/yyyy");
                    DateTime dtToDate = new DateTime();
                    string toDate = string.Empty;
                    if (data.DayTo != "0" && data.MonthTo != "0" && data.YearTo != "0")
                    {
                        dtToDate = new DateTime(Convert.ToInt32(data.DayTo), Convert.ToInt32(data.MonthTo), Convert.ToInt32(data.YearTo), 23, 59, 59);
                    }
                    else
                    {
                        dtToDate = new DateTime(9999, 1, 1, 23, 59, 59);
                    }
                    //toDate = dtToDate.ToString();
                    toDate = dtToDate.ToString("MM/dd/yyyy");
                    //string requestorName = txtRequestorName.Text.Trim();
                    string requestType = data.RequestType;
                    int materialTypeId = Convert.ToInt32(data.MaterialType);
                    int materialGroup = Convert.ToInt32(data.MaterialGroup);
                    int valuationClass = Convert.ToInt32(data.ValuationClass);
                    int RequestorId = 0;
                    if (data.RequestorId != "")
                    {
                        RequestorId = Convert.ToInt32(data.RequestorId);
                    }
                    /***********************Added by Ajit TTL (14-Apr-25) CR5865 **************************************/
                    /////////////////////////////ADD NEW FILTER OPTION MATERIAL CODE//////////////////////////////////////////////////////////////
                    long materialcode = 0;
                    if (data.MetrialCode != "")
                    {
                        Int64.TryParse(data.MetrialCode, out materialcode);
                    }
                    // dt = cmnMasterQueries.SearchMMHeaderStatusDetails(mmId, requestType, materialTypeId, materialGroup, valuationClass, plantCode, status, fromDate, toDate, userId, RequestorId);
                   
                    DataTable dt = oMasterQueries.SearchMMHeaderStatusDetails(mmId, requestType, materialTypeId, materialGroup, valuationClass, plantCode, status, fromDate, toDate, userId, RequestorId, materialcode);                                

                string str = _mmService.ExcelExport(dt);
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
                string FileName = "Search MM Details" + DateTime.Now.ToString("ddMMMyyyy HH:mm");
                if (TempData["CMREQ"] == null)
                {
                    return View();
                }
                string str = (string)TempData["CMREQ"];
                // HttpContext.Response.AddHeader("content-disposition", "attachment; filename=POReport.xls");

                //Response.ContentType = "application/vnd.ms-excel";
                //return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + FileName + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                // Return the file with UTF-8 encoded bytes
                return File(Encoding.UTF8.GetBytes(str), "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }
        #endregion report

        #region mmdetails
        [HttpGet]
        public ActionResult MaterialMasterRequestDetailHistory(string Id)
        {
            try {
                if(Id == null || Id == "")
                {
                    return RedirectToAction("MMStatusRequestReport", "MasterMgmt");
                }
                ViewBag.Id = Id;
                ViewBag.SalesView = "";
                ViewBag.GenMessage = "";
                ViewBag.IsSales = "";
                string strid = Id ?? "";
                if (Id!=null || Id != "")
                {
                    string mm_ID = strid.Substring(0, 1);
                    ViewBag.ReqType = mm_ID;
                    if (mm_ID == Convert.ToString('E'))
                    {
                        BindMaterialDetails(strid);
                        
                        ViewBag.SalesView = "</br>Material Extend Details for Sales view";
                    }
                    else if (mm_ID == Convert.ToString('C'))
                    {
                        BindMaterialCreationDetails(Id);

                        ViewBag.GenMessage ="Material Creation Details";
                    }
                    else if (mm_ID == Convert.ToString('U'))
                    {
                        BindMaterialCreationDetails(Id);

                        ViewBag.GenMessage = "Material Updation Details";
                        
                    }
                }

            }catch (Exception ex) {
                _logger.LogError(ex, "MaterialMasterRequestDetailHistory");                
            }
            return View();
        }
        public void BindMaterialDetails(string Id)
        {
           List<MaterialRequestDetails> dataList = new List<MaterialRequestDetails>();            
            ViewBag.gvMaterialDetails = dataList;
            ViewBag.grdExtendSales = dataList;
            try {
                string MMDetailId = Id;
                int userId = _session.Get<int>("userID");
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                DataTable dt = oMasterQueries.SearchMaterialDetails(MMDetailId, requestType, userId);
                if (dt.Rows.Count > 0)
                {
                 dataList = dt?.AsEnumerable()
                .Select(r => new MaterialRequestDetails
                {
                    MMDETAILHID = r.Field<string>("MMDETAILHID"),
                    MATERIALCODE = Convert.ToString(r.Field<decimal?>("MATERIALCODE")),
                    MATERIALDISCRIPTION = r.Field<string?>("MATERIALDISCRIPTION"),
                    MATERIALSPECIFICATION = r.Field<string?>("MATERIALSPECIFICATION"),
                    PLANTNAME = r.Field<string?>("PLANTNAME"),
                    HSNCODE = r.Field<string?>("HSNCODE"),
                    MMINDICATOR = r.Field<string?>("MMINDICATOR")
                })
                .ToList() ?? new List<MaterialRequestDetails>();
                    ViewBag.gvMaterialDetails = dataList;

                    if (dt.Rows[0]["SALES_ORG"] != null && dt.Rows[0]["SALES_ORG"].ToString().Length > 0)
                    {
                        DataTable db = dt.Select("SALES_ORG ='HMSI'").CopyToDataTable();
                        if(db!=null && db.Rows.Count > 0)
                        {
                            ViewBag.IsSales = "Y";
                        }
                 dataList = db?.AsEnumerable()
                .Select(r => new MaterialRequestDetails
                {
                    MMDETAILHID = r.Field<string>("MMDETAILHID"),
                    MATERIALCODE = Convert.ToString(r.Field<decimal?>("MATERIALCODE")),
                    MATERIALDISCRIPTION = r.Field<string?>("MATERIALDISCRIPTION"),
                    MATERIALSPECIFICATION = r.Field<string?>("MATERIALSPECIFICATION"),
                    PLANTNAME = r.Field<string?>("PLANTNAME"),
                    HSNCODE = r.Field<string?>("HSNCODE"),
                    MMINDICATOR = r.Field<string?>("MMINDICATOR"),
                    MATERIALTYPE = r.Field<string?>("MATERIALTYPE"),
                    MATERIALGROUP = Convert.ToString(r.Field<decimal?>("MATERIALGROUP")),
                    VALUATIONCLASS = Convert.ToString(r.Field<decimal?>("VALUATIONCLASS")),
                    PRICE = Convert.ToString(r.Field<decimal?>("PRICE")),
                    TRANSPORTATIONGROUP = r.Field<string?>("TRANSPORTATIONGROUP"),
                    LOADINGGROUP = r.Field<string?>("LOADINGGROUP"),
                    BASEUNITOFMEASURE = r.Field<string?>("BASEUNITOFMEASURE"),
                    PROFITCENTER = Convert.ToString(r.Field<decimal?>("PROFITCENTER")),
                    DISTRI_CHN = r.Field<string?>("DISTRI_CHN"),
                    ITEM_CATG_GRP = r.Field<string?>("ITEM_CATG_GRP"),
                    GEN_ITEM_CAT_GRP = r.Field<string?>("GEN_ITEM_CAT_GRP"),
                    AVAIL_CHK = r.Field<string?>("AVAIL_CHK"),
                    STORAGE_LOC = r.Field<string?>("STORAGE_LOC"),
                    TAX_CLASSIFICATION = r.Field<string?>("TAX_CLASSIFICATION")
                })
                .ToList() ?? new List<MaterialRequestDetails>();
                        ViewBag.grdExtendSales = dataList;
                    }
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "MaterialMasterRequestDetailHistory");
            }
            
           
        }
        public void BindMaterialCreationDetails(string ID)
        {
            List<MaterialRequestDetails> dataList = new List<MaterialRequestDetails>();
            ViewBag.gvMMCreationDetail = dataList;
            ViewBag.gvMMSalesView = dataList;
            try {
                string MMDetailId = ID;
                int userId = _session.Get<int>("userID");
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                DataTable dt = oMasterQueries.SearchMaterialDetails(MMDetailId, requestType, userId);
                if (dt.Rows.Count > 0)
                {
                    dataList = dt?.AsEnumerable()
                   .Select(r => new MaterialRequestDetails
                   {
                       MMDETAILHID = r.Field<string>("MMDETAILHID"),
                       MATERIALCODE = Convert.ToString(r.Field<decimal?>("MATERIALCODE")),
                       MATERIALDISCRIPTION = r.Field<string?>("MATERIALDISCRIPTION"),
                       MATERIALSPECIFICATION = r.Field<string?>("MATERIALSPECIFICATION"),
                       PLANTNAME = r.Field<string?>("PLANTNAME"),
                       HSNCODE = r.Field<string?>("HSNCODE"),
                       MMINDICATOR = r.Field<string?>("MMINDICATOR"),
                       MATERIALTYPE = r.Field<string?>("MATERIALTYPE"),
                       MATERIALGROUP =Convert.ToString(r.Field<decimal?>("MATERIALGROUP")),
                       VALUATIONCLASS = Convert.ToString(r.Field<decimal?>("VALUATIONCLASS")),
                       PRICE = Convert.ToString(r.Field<decimal?>("PRICE")),
                       TRANSPORTATIONGROUP = r.Field<string?>("TRANSPORTATIONGROUP"),
                       LOADINGGROUP = r.Field<string?>("LOADINGGROUP"),
                       BASEUNITOFMEASURE = r.Field<string?>("BASEUNITOFMEASURE"),
                       PROFITCENTER =Convert.ToString(r.Field<decimal?>("PROFITCENTER")),
                       DISTRI_CHN = r.Field<string?>("DISTRI_CHN"),
                       ITEM_CATG_GRP = r.Field<string?>("ITEM_CATG_GRP"),
                       GEN_ITEM_CAT_GRP = r.Field<string?>("GEN_ITEM_CAT_GRP"),
                       AVAIL_CHK = r.Field<string?>("AVAIL_CHK"),
                       STORAGE_LOC = r.Field<string?>("STORAGE_LOC"),
                       TAX_CLASSIFICATION = r.Field<string?>("TAX_CLASSIFICATION"),
                       UOM = r.Field<string?>("UOM"),
                       MPN_PROFILE = r.Field<string?>("MPN_PROFILE"),
                       PLANT_SP_MAT_STATUS = r.Field<string?>("PLANT_SP_MAT_STATUS"),
                       INT_MATERIAL_NO = r.Field<string?>("INT_MATERIAL_NO")
                   })
                   .ToList() ?? new List<MaterialRequestDetails>();
                    ViewBag.gvMMCreationDetail = dataList;
                    if (dt.Rows[0]["SALES_ORG"] != null && dt.Rows[0]["SALES_ORG"].ToString().Length > 0)
                    {
                       DataTable db = dt.Select("SALES_ORG ='HMSI'").CopyToDataTable();
                        if (db != null && db.Rows.Count > 0)
                        {
                            ViewBag.IsSales = "Y";
                        }
                        dataList = db?.AsEnumerable()
                   .Select(r => new MaterialRequestDetails
                   {
                       MMDETAILHID = r.Field<string>("MMDETAILHID"),
                       MATERIALCODE = Convert.ToString(r.Field<decimal?>("MATERIALCODE")),
                       MATERIALDISCRIPTION = r.Field<string?>("MATERIALDISCRIPTION"),
                       MATERIALSPECIFICATION = r.Field<string?>("MATERIALSPECIFICATION"),
                       PLANTNAME = r.Field<string?>("PLANTNAME"),
                       HSNCODE = r.Field<string?>("HSNCODE"),
                       MMINDICATOR = r.Field<string?>("MMINDICATOR"),
                       MATERIALTYPE = r.Field<string?>("MATERIALTYPE"),
                       MATERIALGROUP = Convert.ToString(r.Field<decimal?>("MATERIALGROUP")),
                       VALUATIONCLASS = Convert.ToString(r.Field<decimal?>("VALUATIONCLASS")),
                       PRICE = Convert.ToString(r.Field<decimal?>("PRICE")),
                       TRANSPORTATIONGROUP = r.Field<string?>("TRANSPORTATIONGROUP"),
                       LOADINGGROUP = r.Field<string?>("LOADINGGROUP"),
                       BASEUNITOFMEASURE = r.Field<string?>("BASEUNITOFMEASURE"),
                       PROFITCENTER = Convert.ToString(r.Field<decimal?>("PROFITCENTER")),
                       DISTRI_CHN = r.Field<string?>("DISTRI_CHN"),
                       ITEM_CATG_GRP = r.Field<string?>("ITEM_CATG_GRP"),
                       GEN_ITEM_CAT_GRP = r.Field<string?>("GEN_ITEM_CAT_GRP"),
                       AVAIL_CHK = r.Field<string?>("AVAIL_CHK"),
                       STORAGE_LOC = r.Field<string?>("STORAGE_LOC"),
                       TAX_CLASSIFICATION = r.Field<string?>("TAX_CLASSIFICATION"),
                       UOM = r.Field<string?>("UOM"),
                       MPN_PROFILE = r.Field<string?>("MPN_PROFILE"),
                       PLANT_SP_MAT_STATUS = r.Field<string?>("PLANT_SP_MAT_STATUS"),
                       INT_MATERIAL_NO = r.Field<string?>("INT_MATERIAL_NO")
                   })
                   .ToList() ?? new List<MaterialRequestDetails>();
                        ViewBag.gvMMSalesView = dataList;
                    }
                }
                }catch(Exception ex) {
                _logger.LogError(ex, "BindMaterialCreationDetails");
            }
            
        }
        #endregion mmdetails
        [HttpGet]
        public ActionResult MMRequestHistoryDetails(string Id)
        {
            try
            {
                if (Id == null || Id == "")
                {
                    return RedirectToAction("MaterialMasterRequestApproval", "MasterMgmt");
                }
                MMApprovalViewModel data =new MMApprovalViewModel();
                List<MMApprovalDetailItem> dataList= BindMaterialDetailsApproval(Id);
                string strid = Id ?? "";
                if (Id != null || Id != "")
                {
                    string mm_ID = strid.Substring(0, 1);
                    ViewBag.ReqType = mm_ID;
                    if (mm_ID == Convert.ToString('E'))
                    {                                                      
                        data=BindExtendRequestDetails(Id);
                    }
                    else if (mm_ID == Convert.ToString('C'))
                    {
                        data = BindCreateRequestDetails(Id);
                    }
                    else if (mm_ID == Convert.ToString('U'))
                    {
                        data = BindUpdateRequestDetails(Id);
                    }                    
                }
                data.RequestNo = Id;
                data.ApprovalDetails = dataList;
                View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MMRequestHistoryDetails");
            }
            return View();
        }
        public List<MMApprovalDetailItem> BindMaterialDetailsApproval(string ID)
        {
            List<MMApprovalDetailItem> dataList = new List<MMApprovalDetailItem>();
            try {
                string MMDetailId = ID;
                int userId = _session.Get<int>("userID");
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                DataTable dt = oMasterQueries.SearchMaterialApprovalDetails(MMDetailId, requestType, userId);
                if (dt.Rows.Count > 0)
                {
                    dataList = dt?.AsEnumerable()
                .Select(r => new MMApprovalDetailItem
                {
                    REQUESTERID =Convert.ToString(r.Field<decimal>("REQUESTERID")),
                    EMPNAME = r.Field<string>("EMPNAME"),
                    STATUS = r.Field<string?>("STATUS"),
                    REMARKS = r.Field<string?>("REMARKS"),
                    LINKSTATUS = r.Field<string?>("LINKSTATUS"),
                    MMHEADERDETAILID = r.Field<string?>("MMHEADERDETAILID"),
                   DATEADDED =r.Field<string>("DATEADDED"),
                })
                .ToList() ?? new List<MMApprovalDetailItem>();
                    //gvMMApprovalDetail.DataSource = dt;
                    //gvMMApprovalDetail.DataBind(); 
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "BindMaterialDetailsApproval");
            }
          return dataList;
            
        }
        private MMApprovalViewModel BindExtendRequestDetails(string ID)
        {
            MMApprovalViewModel data=new MMApprovalViewModel();
            try {
                string MMDetailId = ID;
                int userId = _session.Get<int>("userID");
                //ds = oMasterQueries.GetMMRequestDetail(MMDetailId, userId);
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
              DataSet  ds = oMasterQueries.GetMMApproveRequestDetail(MMDetailId, requestType, userId);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //lblRequestor.Text = ds.Tables[0].Rows[0]["REQUESTER"].ToString();
                    //lblRequestedDate.Text = ds.Tables[0].Rows[0]["CREATEDDATE"].ToString();
                    //lblRecomendationBy.Text = ds.Tables[0].Rows[0]["SUPERVISOR"].ToString();
                    //lblApprovedBy.Text = ds.Tables[0].Rows[0]["SUPERSUPERVISOR"].ToString();
                    //lblPPCApprovalAuthority.Text = "";
                    //lblFinanceApprovalAuthority.Text = "";
                    //lblRecomendationStatus.Text = ds.Tables[0].Rows[0]["RECOMENDATIONSTATUS"].ToString();
                    //lblApprovalStatus.Text = ds.Tables[0].Rows[0]["APPROVALSTATUS"].ToString();
                    //lblPPCStatus.Text = "";
                    //lblFinanceStatus.Text = "";
                    data.Requestor = ds.Tables[0].Rows[0]["REQUESTER"].ToString();
                    data.RequestedDate = ds.Tables[0].Rows[0]["CREATEDDATE"].ToString();
                    data.RecommendationBy = ds.Tables[0].Rows[0]["SUPERVISOR"].ToString();
                    data.ApprovedBy = ds.Tables[0].Rows[0]["SUPERSUPERVISOR"].ToString();
                    data.PPCApprovalAuthority = "";
                    data.FinanceApprovalAuthority = "";
                    data.RecommendationStatus = ds.Tables[0].Rows[0]["RECOMENDATIONSTATUS"].ToString();
                    data.ApprovalStatus = ds.Tables[0].Rows[0]["APPROVALSTATUS"].ToString();
                    data.PPCStatus = "";
                    data.FinanceStatus = ""; 
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "BindExtendRequestDetails");
            }
           return data;
        }
        private MMApprovalViewModel BindCreateRequestDetails(string ID)
        {
            MMApprovalViewModel data = new MMApprovalViewModel();
            try
            {
                DataSet ds = new DataSet();
                string MMDetailId = ID;
                int userId = _session.Get<int>("userID");
                //ds = oMasterQueries.GetMMRequestDetail(MMDetailId, userId);
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                //GetMMApproveRequestDetail
                ds = oMasterQueries.GetMMApproveRequestDetail(MMDetailId, requestType, userId);
                if (ds.Tables[0].Rows.Count > 0)
                {                  
                    data.Requestor = ds.Tables[0].Rows[0]["REQUESTER"].ToString();
                    data.RequestedDate = ds.Tables[0].Rows[0]["CREATEDDATE"].ToString();
                    data.RecommendationBy = ds.Tables[0].Rows[0]["SUPERVISOR"].ToString();
                    data.ApprovedBy = ds.Tables[0].Rows[0]["SUPERSUPERVISOR"].ToString();
                    data.PPCApprovalAuthority = ds.Tables[0].Rows[0]["PPCAUTHORITY"].ToString();
                    data.FinanceApprovalAuthority = ds.Tables[0].Rows[0]["FINANCEAUTHORITY"].ToString();
                    if (ds.Tables[0].Rows[0]["SUPERVISOR"].ToString() == " -  ")
                    {
                        //lblRecomendationStatus.Text = "";
                        data.RecommendationStatus = "";
                    }
                    else
                    {
                        //lblRecomendationStatus.Text = ds.Tables[0].Rows[0]["RECOMENDATIONSTATUS"].ToString();
                        data.RecommendationStatus = ds.Tables[0].Rows[0]["RECOMENDATIONSTATUS"].ToString();
                    }

                    //lblApprovalStatus.Text = ds.Tables[0].Rows[0]["APPROVALSTATUS"].ToString();
                    data.ApprovalStatus = ds.Tables[0].Rows[0]["APPROVALSTATUS"].ToString();
                    //if ( ds.Tables[0].Rows[0]["PPCSTATUS"].ToString() = Convert.ToString("0"))
                    if (ds.Tables[0].Rows[0]["PPCAUTHORITY"].ToString() == " -  ")
                    {                      
                        data.PPCStatus = "";
                    }
                    else
                    {                     
                        data.PPCStatus = ds.Tables[0].Rows[0]["PPCSTATUS"].ToString();
                    }

                    if (ds.Tables[0].Rows[0]["FINANCEAUTHORITY"].ToString() == " -  ")
                    {                     
                        data.FinanceStatus = "";
                    }
                    else
                    {
                       data.FinanceStatus = ds.Tables[0].Rows[0]["FINANCESTATUS"].ToString();
                    }
                   
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "BindCreateRequestDetails");
            }
            return data;
        }
        private MMApprovalViewModel BindUpdateRequestDetails(string ID)
        {
            MMApprovalViewModel data = new MMApprovalViewModel();
            try
            {
                DataSet ds = new DataSet();               
                string MMDetailId = ID;
                int userId = _session.Get<int>("userID");              
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                ds = oMasterQueries.GetMMApproveRequestDetail(MMDetailId, requestType, userId);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    
                    data.Requestor = ds.Tables[0].Rows[0]["REQUESTER"].ToString();
                    data.RequestedDate = ds.Tables[0].Rows[0]["CREATEDDATE"].ToString();
                    data.RecommendationBy = ds.Tables[0].Rows[0]["SUPERVISOR"].ToString();
                    data.ApprovedBy = ds.Tables[0].Rows[0]["SUPERSUPERVISOR"].ToString();
                    data.PPCApprovalAuthority = "";
                    data.FinanceApprovalAuthority = "";                    
                    data.RecommendationStatus = ds.Tables[0].Rows[0]["RECOMENDATIONSTATUS"].ToString();
                    data.ApprovalStatus = ds.Tables[0].Rows[0]["APPROVALSTATUS"].ToString();
                    data.PPCStatus = "";
                    data.FinanceStatus = "";
                   
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "BindUpdateRequestDetails");
            }
            return data;
           
        }
        [HttpGet]
        public IActionResult MMPPCRequestChangeHistory(string Id, string Name, string Date)
        {
            MMCreationViewModel data = new MMCreationViewModel();
            try
            {
                if (Id == null || Id == "")
                {
                    return RedirectToAction("MMStatusRequestReport", "MasterMgmt");
                }               
                List < MMCreationDetailItem > dataList=new List< MMCreationDetailItem >();
                string MMApprove = Id;
                string MMDetailId = Name;
                string Entry_Date = Date;
                string mm_ID = MMDetailId.Substring(0, 1);
                if (mm_ID == Convert.ToString('C'))
                {
                    dataList=BindMaterialCreationDetails(Id,Name, Date);
                    data.RequestNo = Name;
                }
                data.CreationDetails = dataList;
                View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MMPPCRequestChangeHistory");
            }
            return View(data);
        }
        public List<MMCreationDetailItem> BindMaterialCreationDetails(string Id, string Name, string Date)
        {
            List<MMCreationDetailItem> dataList=new List<MMCreationDetailItem>();
            try {
                string MMApprove = Id;
                string MMDetailId = Name;
                string Entry_Date = Date;
                int userId = _session.Get<int>("userID");
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                DataTable dt = oMasterQueries.SearchMaterialDetailsPPCChangeHistory(MMDetailId, requestType, userId, Entry_Date);
                if (dt.Rows.Count > 0)
                {
                    dataList = dt?.AsEnumerable()
                    .Select(r => new MMCreationDetailItem
                    {
                        MMDETAILHID = r.Field<string>("MMDETAILHID"),
                        MATERIALDISCRIPTION = r.Field<string>("MATERIALDISCRIPTION"),
                        MATERIALTYPE = r.Field<string?>("MATERIALTYPE"),
                        MOD_MATERIALTYPE = r.Field<string?>("MOD_MATERIALTYPE"),
                        MATERIALGROUP = r.Field<string?>("MATERIALGROUP"),
                        MOD_MATERIALGROUP = r.Field<string?>("MOD_MATERIALGROUP"),
                        VALUATIONCLASS = r.Field<string>("VALUATIONCLASS"),
                        MOD_VALUATIONCLASS = r.Field<string>("MOD_VALUATIONCLASS"),
                    })
                    .ToList() ?? new List<MMCreationDetailItem>();
                }
            } catch (Exception ex) { 
            }
            return dataList;
        }
        [HttpGet]
        public IActionResult MaterialMasterRequestApproval()
        {
            MaterialMasterApproval data=new MaterialMasterApproval();
            try
            {
                data.MMCreationRequests = BindMMCreationRequestDetails();
                data.MMExtendRequests = BindMMExtendRequestDetails();
                data.MMUpdationRequests = BindMMUpdationRequestDetails();

                View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MaterialMasterRequestApproval");
            }
            return View(data);
        }
        private List<SearchMaterialOutput> BindMMCreationRequestDetails()
        {
            List<SearchMaterialOutput> dataList = new List<SearchMaterialOutput>();
            try {
                DataSet ds = new DataSet();
                int userId = _session.Get<int>("userID");
                ds = oMasterQueries.GetMMApprovalCreationRequestList(userId);
                if(ds!=null && ds.Tables[0].Rows.Count > 0)
                {
                    dataList = ds.Tables[0]?.AsEnumerable()
             .Select(r => new SearchMaterialOutput
             {
                 MMHEADERID = Convert.ToString(r.Field<decimal>("MMHEADERID")),
                 MMHEADERDETAILID = r.Field<string>("MMHEADERDETAILID"),
                 REQUESTDATE = Convert.ToString(r.Field<DateTime>("REQUESTDATE")),
                 REQUESTERID = Convert.ToString(r.Field<long>("REQUESTERID")),
                 REQUESTORNAME = r.Field<string>("REQUESTORNAME"),
                 STATUS = r.Field<string>("STATUS")                 
                 
             })
             .ToList() ?? new List<SearchMaterialOutput>();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "BindMMCreationRequestDetails");
            }
            return dataList;            
        }
        private List<SearchMaterialOutput> BindMMExtendRequestDetails()
        {
            List<SearchMaterialOutput> dataList = new List<SearchMaterialOutput>();
            try
            {
                DataSet ds = new DataSet();
                int userId = _session.Get<int>("userID");
                ds = oMasterQueries.GetMMApprovalExtendRequestList(userId);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    dataList = ds.Tables[0]?.AsEnumerable()
             .Select(r => new SearchMaterialOutput
             {
                 MMHEADERID = Convert.ToString(r.Field<decimal>("MMHEADERID")),
                 MMHEADERDETAILID = r.Field<string>("MMHEADERDETAILID"),
                 REQUESTDATE = Convert.ToString(r.Field<DateTime>("REQUESTDATE")),
                 REQUESTERID = Convert.ToString(r.Field<long>("REQUESTERID")),
                 REQUESTORNAME = r.Field<string>("REQUESTORNAME"),
                 STATUS = r.Field<string>("STATUS")
                 
             })
             .ToList() ?? new List<SearchMaterialOutput>();
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "BindMMExtendRequestDetails");
            }
            return dataList;
        }
        private List<SearchMaterialOutput> BindMMUpdationRequestDetails()
        {
            List<SearchMaterialOutput> dataList = new List<SearchMaterialOutput>();
            try
            {
                DataSet ds = new DataSet();
                int userId = _session.Get<int>("userID");
                ds = oMasterQueries.GetMMApprovalUpdationRequestList(userId);
               
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    dataList = ds.Tables[0]?.AsEnumerable()
             .Select(r => new SearchMaterialOutput
             {
                 MMHEADERID = Convert.ToString(r.Field<decimal>("MMHEADERID")),
                 MMHEADERDETAILID = r.Field<string>("MMHEADERDETAILID"),
                 REQUESTDATE = Convert.ToString(r.Field<DateTime>("REQUESTDATE")),
                 REQUESTERID = Convert.ToString(r.Field<long>("REQUESTERID")),
                 REQUESTORNAME = r.Field<string>("REQUESTORNAME"),
                 STATUS = r.Field<string>("STATUS")                 
             })
             .ToList() ?? new List<SearchMaterialOutput>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BindMMUpdationRequestDetails");
            }
            return dataList;
           

        }
        [HttpGet]
        public IActionResult MMApprovalEntryForm(string Id)
        {
            if (Id == null || Id == "")
            {
                return RedirectToAction("MaterialMasterRequestApproval", "MasterMgmt");
            }
            List<MaterialRequestDetails> dataList = new List<MaterialRequestDetails>();
            ViewBag.ApproveExtend = false;
            ViewBag.ApproveCreate = false;
            ViewBag.UpdateRequest = false;
            ViewBag.MaterialDetail1 = false;
            ViewBag.MaterialDetail2 = false;
            ViewBag.MaterialDetail3 = false;
            ViewBag.termsconditiontextppc = false;
            ViewBag.gvMMCreationRequest = dataList;
            ViewBag.termscondition1 = true;
            ViewBag.termscondition2 = true;
            ViewBag.termscondition3 = true;
            PopulateControls();
            string mm_ID = Id.Substring(0, 1);
            ViewBag.requestNo = Id;
            string MMDetailId = Id;

            if (mm_ID == "E")
            {
                BindMMExtendRequestDetails(MMDetailId);
            }
            else if (mm_ID == "C")
            {
                BindMMCreateRequestDetails(MMDetailId);
            }
            else if (mm_ID == "U")
            {
                BindMMUpdateRequestDetails(MMDetailId);
            }
            return View();
        }
        [HttpPost]
        public IActionResult MMApprovalEntryForm(ApprorvalParam data)
        {
                if (data==null || data.CommandArgument==null || data.CommandArgument=="")
                {
                return RedirectToAction("MaterialMasterRequestApproval", "MasterMgmt");
                }
            List<MaterialRequestDetails> dataList = new List<MaterialRequestDetails>();
            ViewBag.ApproveExtend = false;
            ViewBag.ApproveCreate = false;
            ViewBag.UpdateRequest = false;
            ViewBag.MaterialDetail1 = false;
            ViewBag.MaterialDetail2 = false;
            ViewBag.MaterialDetail3 = false;
            ViewBag.termsconditiontextppc = false;
            ViewBag.gvMMCreationRequest = dataList;
            ViewBag.termscondition1 = true;
            ViewBag.termscondition2 = true;
            ViewBag.termscondition3 = true;
            PopulateControls();
                string mm_ID = data.CommandArgument.Substring(0, 1);
                ViewBag.requestNo = data.CommandArgument;
                string MMDetailId= data.CommandArgument;

                if (mm_ID =="E")
                {
                    BindMMExtendRequestDetails(MMDetailId);
                }
                else if (mm_ID == "C")
                {
                    BindMMCreateRequestDetails(MMDetailId);
                }
                else if (mm_ID == "U")
                {
                    BindMMUpdateRequestDetails(MMDetailId);
                }
           
            return View();
        }
        private void BindMMCreateRequestDetails(string MMDetailId)
        {
            List<MaterialRequestDetails> dataList=new List<MaterialRequestDetails>();
            ViewBag.ApproveExtend = false;
            ViewBag.ApproveCreate = true;
            ViewBag.UpdateRequest = false;
            ViewBag.MaterialDetail1 = false;
            ViewBag.MaterialDetail2 = false;
            ViewBag.MaterialDetail3 = false;
            ViewBag.termsconditiontextppc = false;
            ViewBag.gvMMCreationRequest = dataList;
            ViewBag.termscondition1 = false;
            ViewBag.termscondition2 = false;
            ViewBag.termscondition3 = false;
            try {
                int userId = _session.Get<int>("userID");
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
               DataTable dt = oMasterQueries.GetMMApprovalCreateRequestList(MMDetailId, requestType, userId);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Status"].ToString() == "PPC" || dt.Rows[0]["Status"].ToString() == "Finance")
                    {
                        ViewBag.termscondition1 = false;
                        ViewBag.termscondition2 = false;
                        ViewBag.termscondition3 = false;
                        ViewBag.termsconditiontextppc = false;
                        if (dt.Rows[0]["Status"].ToString() == "PPC")
                        {
                            ViewBag.termsconditiontextppc = true;
                        }
                        ViewBag.idchkval = "fin";
                    }
                    else
                    {
                        ViewBag.termscondition1 = true;
                        ViewBag.termscondition2 = true;
                        ViewBag.termscondition3 = true;
                        ViewBag.termsconditiontextppc = false;
                        ViewBag.idchkval = "All";
                    }
                    if (dt.Rows[0]["Status"].ToString() == "PPC")
                    {
                        ViewBag.ApproveExtend = false;
                        ViewBag.UpdateRequest = false;
                        ViewBag.ApproveCreate = true;
                        //gvMMCreationRequest.Columns[9] = false;
                        //gvMMCreationRequest.Columns[10] = true;   
                        ViewBag.Columns9 = false;
                        ViewBag.Columns11 = true;
                    }
                    else
                    {
                        ViewBag.ApproveExtend = false;
                        ViewBag.UpdateRequest = false;
                        //gvMMCreationRequest.Columns[9] = false;
                        //gvMMCreationRequest.Columns[10] = false;                  
                        ViewBag.Columns9 = false;
                        ViewBag.Columns11 = false;
                       
                    }
                    dataList = dt?.AsEnumerable()
                    .Select(r => new MaterialRequestDetails
                    {
                        MMDETAILHID = r.Field<string>("MMDETAILHID"),
                        MMDETAILID = Convert.ToString(r.Field<decimal>("MMDETAILID")),
                        MMHEADERID = Convert.ToString(r.Field<decimal>("MMHEADERID")),
                        MMHEADERDETAILID = r.Field<string?>("MMHEADERDETAILID"),
                        MATERIALDISCRIPTION = r.Field<string?>("MATERIALDISCRIPTION"),
                        MATERIALSPECIFICATION = r.Field<string?>("MATERIALSPECIFICATION"),
                        PLANTNAME = r.Field<string?>("PLANTNAME"),
                        HSNCODE = r.Field<string?>("HSNCODE"),
                        MMINDICATOR = r.Field<string?>("MMINDICATOR"),
                        MATERIALTYPE = r.Field<string?>("MATERIALTYPE"),
                        MATERIALGROUP = Convert.ToString(r.Field<decimal?>("MATERIALGROUP")),
                        VALUATIONCLASS = Convert.ToString(r.Field<decimal?>("VALUATIONCLASS")),
                        PRICE = Convert.ToString(r.Field<decimal?>("PRICE")),
                        UOM = r.Field<string?>("UOM")
                    })
                    .ToList() ?? new List<MaterialRequestDetails>();
                    ViewBag.gvMMCreationRequest = dataList;
                    ViewBag.MaterialDetail1 = false;
                    ViewBag.MaterialDetail2 = false;
                    ViewBag.MaterialDetail3 = false;
                }
              
            } catch (Exception ex) {
                _logger.LogError(ex, "BindMMCreateRequestDetails");
            }
            
        }
        private void BindMMUpdateRequestDetails(string MMDetailId)
        {
            List<MaterialRequestDetails> dataList = new List<MaterialRequestDetails>();
            ViewBag.ApproveExtend = false;
            ViewBag.ApproveCreate = false;
            ViewBag.UpdateRequest = true;
            ViewBag.MaterialDetail1 = false;
            ViewBag.MaterialDetail2 = false;
            ViewBag.MaterialDetail3 = false;
            ViewBag.termsconditiontextppc = false;
            ViewBag.gvUpdateRequest = dataList;
            ViewBag.termscondition1 = true;
            ViewBag.termscondition2 = true;
            ViewBag.termscondition3 = true;
            try
            {
                int userId = _session.Get<int>("userID");
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                DataTable dt = oMasterQueries.GetMMApprovalCreateRequestList(MMDetailId, requestType, userId);
                if (dt.Rows.Count > 0)
                {

                    ViewBag.ApproveExtend = false;
                    ViewBag.ApproveCreate = false;
                    ViewBag.UpdateRequest = true;

                    //gvUpdateRequest.Columns[9] = false;
                    //gvUpdateRequest.Columns[10] = true;
                    //gvUpdateRequest.DataSource = dt;
                    //gvUpdateRequest.DataBind();
                    ViewBag.MaterialDetail1 = false;
                    ViewBag.MaterialDetail2 = false;
                    ViewBag.MaterialDetail3 = false;
                    dataList = dt?.AsEnumerable()
                        .Select(r => new MaterialRequestDetails
                        {
                            MMDETAILHID = r.Field<string>("MMDETAILHID"),
                            MMDETAILID = Convert.ToString(r.Field<decimal>("MMDETAILID")),
                            MMHEADERID = Convert.ToString(r.Field<decimal>("MMHEADERID")),
                            MMHEADERDETAILID = r.Field<string?>("MMHEADERDETAILID"),
                            MATERIALDISCRIPTION = r.Field<string?>("MATERIALDISCRIPTION"),
                            MATERIALSPECIFICATION = r.Field<string?>("MATERIALSPECIFICATION"),
                            PLANTNAME = r.Field<string?>("PLANTNAME"),
                            HSNCODE = r.Field<string?>("HSNCODE"),
                            MMINDICATOR = r.Field<string?>("MMINDICATOR"),
                            MATERIALTYPE = r.Field<string?>("MATERIALTYPE"),
                            MATERIALGROUP = Convert.ToString(r.Field<decimal?>("MATERIALGROUP")),
                            VALUATIONCLASS = Convert.ToString(r.Field<decimal?>("VALUATIONCLASS")),
                            PRICE = Convert.ToString(r.Field<decimal?>("PRICE")),  
                            UOM = r.Field<string?>("UOM")
                                                       
                        })
                        .ToList() ?? new List<MaterialRequestDetails>();
                    ViewBag.gvUpdateRequest = dataList;
                }
              
            }
            catch(Exception ex) {
                _logger.LogError(ex, "BindMMUpdateRequestDetails");
            }

           
        }
        private void BindMMExtendRequestDetails(string MMDetailId)
        {
            List<MaterialRequestDetails> dataList = new List<MaterialRequestDetails>();
            ViewBag.ApproveCreate = false;
            ViewBag.ApproveExtend = true;
            ViewBag.UpdateRequest = false;
            ViewBag.termsconditiontextppc = false;
            ViewBag.gvMMExtendRequest = dataList;
            ViewBag.termscondition1 = true;
            ViewBag.termscondition2 = true;
            ViewBag.termscondition3 = true;
            try {
                int userId = _session.Get<int>("userID");
                string mm_ID = MMDetailId.Substring(0, 1);
                string requestType = mm_ID;
                DataTable dt = new DataTable();
                dt = oMasterQueries.GetMMApprovalCreateRequestList(MMDetailId, requestType, userId);
                if (dt.Rows.Count > 0)
                {
                    ViewBag.ApproveCreate = false;
                    ViewBag.ApproveExtend = true;
                    ViewBag.UpdateRequest = false;
                    //gvMMExtendRequest.DataSource = dt;
                    //gvMMExtendRequest.DataBind();
                    ViewBag.termsconditiontextppc = false;
                    dataList = dt?.AsEnumerable()
                       .Select(r => new MaterialRequestDetails
                       {
                           MMDETAILHID = r.Field<string>("MMDETAILHID"),
                           MMDETAILID = Convert.ToString(r.Field<decimal>("MMDETAILID")),
                           MMHEADERID = Convert.ToString(r.Field<decimal>("MMHEADERID")),
                           MATERIALCODE = Convert.ToString(r.Field<decimal?>("MATERIALCODE")),
                           MATERIALDISCRIPTION = r.Field<string?>("MATERIALDISCRIPTION"),
                           MATERIALSPECIFICATION = r.Field<string?>("MATERIALSPECIFICATION"),
                           PLANTNAME = r.Field<string?>("PLANTNAME"),
                           HSNCODE = r.Field<string?>("HSNCODE"),
                           MMHEADERDETAILID = r.Field<string?>("MMHEADERDETAILID")                           
                       })
                       .ToList() ?? new List<MaterialRequestDetails>();
                    ViewBag.gvMMExtendRequest = dataList;
                }
               
            }
            catch(Exception ex) {
                _logger.LogError(ex, "BindMMExtendRequestDetails");
            }
          
        }
        [HttpPost]
        public async Task<IActionResult> SubmitApprovalRequest([FromBody] MMApprovalRequest data)
        {
            try {
                if (data == null || data.MMDetailID == "" || data.Status == "")
                {
                    return Json(new { Rs = 0, Message = "Please check mandatory field!" });
                }
                string status = (data.Status == "1") ? "Approved" : "SendBack";
                string MMDetailIdSelected = string.Empty;
                string MMDetailId = data.MMDetailID ?? "";
                string mm_ID = MMDetailId.Substring(0, 1);
                int userId = _session.Get<int>("userID");
                if (data.IsMultiple == "NO")
                {
                    MMDetailId = data.MMDetailID;
                    mm_ID = MMDetailId.Substring(0, 1);
                }
                else
                {
                    MMDetailId = data.MMDetailID;
                    mm_ID = MMDetailId.Substring(0, 1);
                }
                if (data.MMDetailHIDs.Length>0)
                {
                    foreach (string row in data.MMDetailHIDs)
                    {
                        MMDetailIdSelected = string.Concat(MMDetailIdSelected, (MMDetailIdSelected == string.Empty ? "" : ","), row);
                    }
                }
                else
                {
                    MMDetailIdSelected = string.Concat(MMDetailIdSelected, (MMDetailIdSelected == string.Empty ? "" : ","), MMDetailId);
                }

                if (MMDetailIdSelected != "")
                {
                    string output = oMasterQueries.ApproveMMRequest(MMDetailIdSelected, data.Remarks, userId, status);
                    if (output != "1")
                    {
                        return Json(new { Rs = 0, Message = output });
                    }
                    else
                    {
                        return Json(new { Rs = 1, Message = status == "SendBack" ? "Selected request(s) sent back successfully." : "Selected request(s) approved successfully." });
                        // Response.Redirect("~/ASPXView/MasterMgmt/MaterialMasterRequestApproval.aspx", false);
                    }
                }
               
            }
            catch(Exception ex) {
                _logger.LogError(ex, "SubmitApprovalRequest");
            }
            return Json(new { Rs = 0, Message = "Error in data process!" });

        }
        [HttpPost]
        public async Task<IActionResult> UpdateApprovalRequest([FromBody] MMUpdateApprovalReq data)
        {
            try
            {
                if (data == null || data.materialDetailCode == 0 || data.materialType == 0 || data.materialGroup == 0 || data.valuationClass == 0)
                {
                    return Json(new { Rs = 0, Message = "Please check mandatory field!" });
                }

                string output_insert = oMasterQueries.InsertMDPPCApproveRequestChangeHistory(data.materialDetailCode??0, data.materialType ?? 0, data.materialGroup ?? 0, data.valuationClass ?? 0);
                string output = oMasterQueries.UpdateMarerialDetailPPCApproveRequest(data.materialDetailCode ?? 0, data.materialType ?? 0, data.materialGroup ?? 0, data.valuationClass ?? 0);

                if (output != "1")
                {
                    //return output; // return error text
                    return Json(new { Rs = 0, Message = output });
                }
                //return "1"; // success
                return Json(new { Rs = 1, Message = "Submitted" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateApprovalRequest");
            }
            return Json(new { Rs = 0, Message = "Error in data process!" });

        }
        /*********************************** Material extend request**************************************/
        [HttpGet]
        public async Task<IActionResult> MaterialMasterExtendRequest()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                int UserID = _session.Get<int>("userID");
                ViewBag.SELECTEDPLANT = "0";
               
                DataSet ds = oMasterQueries.MMEXTENDDRAFT_HDDETAIL_GET(UserID);

                if (ds != null)
                {
                    int _MMHEADERID = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.MMHeaderIdE = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                        _session.Set<string>("MMHeaderIdE", ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        _MMHEADERID = Convert.ToInt32(ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        ViewBag.txtEmailId= ds.Tables[0].Rows[0]["EMAILID"].ToString();
                    }
                    else
                    {
                        ViewBag.MMHeaderIdE = "0";
                        _session.Set<string>("MMHeaderIdE", "");
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        try {
                            List<MaterialDetail> dataList = new List<MaterialDetail>();
                            dataList = ds.Tables[1]?.AsEnumerable()
                        .Select(r => new MaterialDetail
                        {
                            MMDETAILID = Convert.ToInt32(r.Field<decimal>("MMDETAILID")),
                            MMHEADERID = _MMHEADERID,
                            MATERIALDISCRIPTION = r.Field<string>("MATERIALDISCRIPTION"),                            
                            MATERIALCODE = Convert.ToString(r.Field<decimal>("MATERIALCODE")),
                            SALES_ORG = r.Field<string>("SALES_ORG"),
                            DISTRI_CHN = r.Field<string>("DISTRI_CHN"),
                            ITEM_CATG_GRP = r.Field<string>("ITEM_CATG_GRP"),
                            GEN_ITEM_CAT_GRP = r.Field<string>("GEN_ITEM_CAT_GRP"),
                            LOADINGGROUP = r.Field<string>("LOADINGGROUP"),
                            TAX_CLASSIFICATION = r.Field<string>("TAX_CLASSIFICATION"),
                            BASEUNITOFMEASURE = r.Field<string>("BASEUNITOFMEASURE"),
                            PLANTNAME = r.Field<string>("PLANTNAME"), 
                            AVAIL_CHK = r.Field<string>("AVAIL_CHK")
                        })
                        .ToList() ?? new List<MaterialDetail>();
                            ViewBag.GvList = dataList;
                        }
                        catch(Exception ex) {
                            _logger.LogError(ex, "MaterialMasterExtendRequest-MMEXTENDDRAFT_HDDETAIL_GET");
                        }
                       
                    }
                    else
                    {
                        ViewBag.GvList = null;
                    }

                }
                else
                {
                    ViewBag.MMHeaderIdE = "0";
                    _session.Set<string>("MMHeaderIdE", "");
                    ViewBag.SELECTEDPLANT = "0";
                    ViewBag.PROFITCENTER = "";
                    ViewBag.GvList = new List<MaterialDetail>();
                }
                PopulateControls();
                BindMasterData();
                BindApprovalAuthority();
                BindMMExtendRejectDetails();
                return View();
            }catch(Exception ex) {
                _logger.LogError(ex, "MaterialMasterExtendRequest");
            }
            return View();
        }
        public void BindMMExtendRejectDetails()
        {
            List<MaterilaRejectedList> dataList = null;
            try
            {
                int UserID = _session.Get<int>("userID");
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialExtendRejectedDetails(UserID);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dataList = ds.Tables[0]?.AsEnumerable()
            .Select(r => new MaterilaRejectedList
            {
                MMHEADERID = Convert.ToInt32(r.Field<decimal>("MMHEADERID")),
                MMHEADERDETAILID = r.Field<string>("MMHEADERDETAILID"),
                REQUESTERID = Convert.ToInt32(r.Field<long>("REQUESTERID")),
                REQUESTERNAME = r.Field<string>("REQUESTERNAME"),
                REQUESTDATE = Convert.ToString(r.Field<DateTime>("REQUESTDATE"))

            })
            .ToList() ?? new List<MaterilaRejectedList>();
                    ViewBag.gvMMExtendRequest = dataList;
                }
                else
                {
                    ViewBag.gvMMExtendRequest = dataList;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BindMMCreationRejectDetails");
                ViewBag.gvMMExtendRequest = dataList;
            }
        }
        [HttpPost]
        public async Task<IActionResult> CheckMaterialCode([FromBody] CheckMaterialCodeData data)
        {
            MasterResponse res=new MasterResponse(); 
            try
            {
                if (data == null || data.MaterialCode == "" || data.MaterialCode =="0" )
                {
                    return Json(new { Rs = 0, Message = "Please enter material code!" });
                }
                int userId = _session.Get<int>("userID");
                string hsncode = data.MaterialCode ?? "";
                int plantcode = data.PlantCode ?? 0;
                DataSet ds = new DataSet();
                //ds = oMasterQueries.GetMaterialCodeExist(Convert.ToInt32(txtMaterialCode.Text), Convert.ToInt32(ddlPlantMst.SelectedValue), Convert.ToInt32(((Employee_Details)Session["Employee"]).Employee_Code));
                ds = oMasterQueries.GetMaterialCodeExist(Convert.ToInt64(hsncode), plantcode, userId);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    res.Rs = 1;
                    //lbl_textDesc.Visible = true;
                    //lbl_textSpec.Visible = true;
                    res.MATERIALDISCRIPTION = ds.Tables[0].Rows[0]["MATERIALDISCRIPTION"].ToString();
                    res.MATERIALSPECIFICATION = ds.Tables[0].Rows[0]["MATERIALSPECIFICATION"].ToString();
                   
                    MasterResponse dd= _mmService.GetProfitCentreAndStorageLoc(ds.Tables[0].Rows[0]["PLANTCODE"].ToString());
                    res.ProfitCentre = dd.ProfitCentre;
                    res.list = dd.list;
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["ISSALES"].ToString()) && ds.Tables[0].Rows[0]["ISSALES"].ToString() == "1")
                    {
                        // chSales.Visible = true;
                        res.ISSALES = "YES";                       
                        // lstSales.SelectedValue = "1";
                        DataSet ds1 = oMasterQueries.GetMasterData("STORAGE_LOCATION", ds.Tables[0].Rows[0]["PLANTCODE"].ToString());
                        var dataList = ds1.Tables[0]?.AsEnumerable()
                       .Select(r => new MasterData
                       {
                           Code = r.Field<string>("CODE"),
                           Description = r.Field<string>("CODE_DESC"),

                       })
                     .ToList() ?? new List<MasterData>();

                        res.list = dataList;
                    
                    }
                    else
                    {                    
                    res.ISSALES = "YES";
                
                    }
                    //////////////////////////////////////////////////////////////////////////
                    ds = oMasterQueries.GetMaterialCodeExist(Convert.ToInt64(hsncode), plantcode, userId);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if (row["SALES_ORG"] != null && row["SALES_ORG"].ToString() != "")
                            {
                                res.ISSALES = "YES";

                                if (row["TRANSPORTATIONGROUP"] != null && row["TRANSPORTATIONGROUP"].ToString() != "")
                                {
                                    res.TRANSPORTATIONGROUP = row["TRANSPORTATIONGROUP"].ToString();
                                }
                                if (row["LOADINGGROUP"] != null && row["LOADINGGROUP"].ToString() != "")
                                {
                                    res.LOADINGGROUP = row["LOADINGGROUP"].ToString();
                                }
                                if (row["BASEUNITOFMEASURE"] != null && row["BASEUNITOFMEASURE"].ToString() != "")
                                {
                                    res.BASEUNITOFMEASURE = row["BASEUNITOFMEASURE"].ToString();
                                }
                                //if (row["PROFITCENTER"] != null && row["PROFITCENTER"].ToString() != "")
                                //{
                                //    res.ProfitCentre = row["PROFITCENTER"].ToString();
                                //}
                                if (row["DISTRI_CHN"] != null && row["DISTRI_CHN"].ToString() != "")
                                {
                                    res.DISTRI_CHN = row["DISTRI_CHN"].ToString();
                                }
                                if (row["ITEM_CATG_GRP"] != null && row["ITEM_CATG_GRP"].ToString() != "")
                                {
                                    res.ITEM_CATG_GRP = row["ITEM_CATG_GRP"].ToString();
                                }
                                if (row["AVAIL_CHK"] != null && row["AVAIL_CHK"].ToString() != "")
                                {
                                    res.AVAIL_CHK = row["AVAIL_CHK"].ToString();
                                }
                                if (row["STORAGE_LOC"] != null && row["STORAGE_LOC"].ToString() != "")
                                {
                                    res.STORAGE_LOC = row["STORAGE_LOC"].ToString();
                                }
                                if (row["GEN_ITEM_CAT_GRP"] != null && row["GEN_ITEM_CAT_GRP"].ToString() != "")
                                {
                                    res.GEN_ITEM_CAT_GRP = row["GEN_ITEM_CAT_GRP"].ToString();
                                }
                                if (row["TAX_CLASSIFICATION"] != null && row["TAX_CLASSIFICATION"].ToString() != "")
                                {
                                    res.TAX_CLASSIFICATION = row["TAX_CLASSIFICATION"].ToString();
                                }
                                if (row["MAT_GRP_PACK_MATLS"] != null && row["MAT_GRP_PACK_MATLS"].ToString() != "")
                                {
                                    res.MAT_GRP_PACK_MATLS = row["MAT_GRP_PACK_MATLS"].ToString();
                                }
                                if (row["PACKAGING_MAT_TYPE"] != null && row["PACKAGING_MAT_TYPE"].ToString() != "")
                                {
                                    res.PACKAGING_MAT_TYPE = row["PACKAGING_MAT_TYPE"].ToString();
                                }
                                // '6' IS MATERIAL TYPE ID OF ZPAC and material packaging type is visible
                                if (row["MATERIALTYPE"] != null && row["MATERIALTYPE"].ToString() == "ZPAC")
                                {
                                    //Tr106.Visible = true;
                                    res.MATERIALTYPE = "ZPAC";
                                }
                                else
                                {
                                    //Tr106.Visible = false;
                                    res.MATERIALTYPE = "";
                                }
                                break;
                            }
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////
                }
                else
                {
                    res.Rs = 0;
                    res.Message = "Enter valid material code!";
                }
                return Json(res);
            }
            catch(Exception ex) {
                _logger.LogError(ex, "MaterialMasterExtendRequest");
                return Json(new { Rs = 0, Message = "Error in material code finding!" });
            }
          
        }
       
        [HttpPost]       
        public async Task<IActionResult> AddExntendMaterialRequest([FromBody] MMDetailViewModel data)
        {
            List<MaterialDetail> dataList = new List<MaterialDetail>();
            try
            {
                if (data == null)
                {
                    return Json(new { Rs = 0, Message = "Invalid ajax calling" });
                }
                Employee_Details employee = _session.Get<Employee_Details>("Employee");
                int UserId = _session.Get<int>("userID");               
                int plantCode = data.PLANTCODE ?? 0;
                int MMExheaderId=0;
                string emailId = data.EMAIL_ID ?? employee.EMail_Id;
                int headerId = 0;
                long MaterialCode = data.MATERIALCODE ?? 0;
                int Plant = data.PLANTCODE ?? 0;
                string DisbChnn = data.DISTRI_CHN ?? "";
                string errorMsg = "";
                int lstSales = Convert.ToInt32(data.lstSales==""?"0": data.lstSales);
                if (data.MMHEADERID != 0)
                {
                    data.MMHEADERID = data.MMHEADERID;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_session.Get<string>("MMHeaderIdE")))
                    {
                        data.MMHEADERID = _session.Get<int>("MMHeaderIdE");

                    }
                }

                MMExheaderId = data.MMHEADERID ?? 0;
                headerId = data.MMHEADERID ?? 0;
                string err = "";
              
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialCountDetails(headerId);
                if (ds.Tables[0].Rows.Count >= 20)
                {
                    return Json(new { Rs = 0, Message = "Only 20 material code request can be added in a request." });
                }
                else
                { 
                    if (MaterialCodeCheck(MaterialCode, Plant, DisbChnn, lstSales,ref errorMsg))
                     {
                        string output;
                        if (data.MMDETAILID == null || data.MMDETAILID == 0)
                        {
                            //output = oMasterQueries.AddMarerialCodeExtendRequest(Convert.ToInt32(txtMaterialCode.Text), Convert.ToInt32(ddlPlantMst.SelectedValue), txt_description, txt_specification, Convert.ToInt32(((Employee_Details)Session["Employee"]).Employee_Code), emailId);
                            //if (chSales.Checked)
                            if (lstSales > 0)
                            {
                                if (!ValidateSale(ref err,lstSales,data))
                                {
                                   return Json(new { Rs = 0, Message = "Check Mandatory Field -" + err });
                                }
                                output = oMasterQueries.AddMarerialCodeExtendRequestSales(MMExheaderId, MaterialCode, Plant, UserId, emailId, data.TRANSPORTATIONGROUP, data.LOADINGGROUP,data.BASEUNITOFMEASURE,data.SALES_ORG,data.DISTRI_CHN,data.ITEM_CATG_GRP,data.AVAIL_CHK,data.PROFITCENTER??"".Trim(),data.STORAGE_LOC,data.TAX_CLASSIFICATION,data.GEN_ITEM_CAT_GRP,data.MAT_GRP_PACK_MATLS??"".Trim(), data.PACKAGING_MAT_TYPE??"".Trim(), data.lstSales);
                            }
                            else
                            {
                                output = oMasterQueries.AddMarerialCodeExtendRequest(MMExheaderId, MaterialCode, Plant, UserId, emailId);
                            }
                            if (output == "1")
                            {
                                dataList = GridViewBindMMExtendDetails();
                                return Json(new { Rs = 1, Message = "Material master extend requested is added successfully.", dataList });
                            }
                            else
                            {
                                return Json(new { Rs = 0, Message = output });
                            }
                        }
                        else
                        {
                            //if (chSales.Checked)
                            if (lstSales > 0)
                            {
                                if (!ValidateSale(ref err, lstSales, data))
                                {
                                   return Json(new { Rs = 0, Message = "Check Mandatory Field -" + err });
                                }
                                output = oMasterQueries.UpdateMarerialCodeExtendRequestSales(MMExheaderId, data.MMDETAILID ?? 0, MaterialCode, Plant, emailId, data.TRANSPORTATIONGROUP,data.LOADINGGROUP,data.BASEUNITOFMEASURE,data.SALES_ORG,data.DISTRI_CHN,data.ITEM_CATG_GRP,data.AVAIL_CHK, data.PROFITCENTER.Trim(),data.STORAGE_LOC,data.TAX_CLASSIFICATION,data.GEN_ITEM_CAT_GRP,data.MAT_GRP_PACK_MATLS.Trim(),data.PACKAGING_MAT_TYPE.Trim(),data.lstSales);
                            }
                            else
                            {                                
                                output = oMasterQueries.UpdateMarerialCodeExtendRequestSales(MMExheaderId, data.MMDETAILID ?? 0, MaterialCode, Plant, emailId, "", "", "", "", "", "", "", "", "", "", "", "", "", "0");
                            }
                            if (output != "1")
                            {
                                return Json(new { Rs = 0, Message = output });
                            }
                            else
                            {
                                dataList = GridViewBindMMExtendDetails();
                                string message= "Material master extend requested (" + data.MMDETAILID.ToString() + ") is updated successfully.";
                                return Json(new { Rs = 1, Message = message, dataList });
                            }
                           
                        }
                      
                       
                    }
                    else
                    {
                        return Json(new { Rs = 0, Message = errorMsg });
                    }                                       
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddMoreMaterial");
                return Json(new { Rs = 0, Message = ex });
            }
        }
        private bool MaterialCodeCheck(long MaterialCode, int Plant, string DisbChnn,int lstSales, ref string errmsg)
        {
            bool _isExist = false;
            int UserID = _session.Get<int>("userID");
            DataSet ds = new DataSet();
            int isSales = 0, isPurshase = 0;
            ds = oMasterQueries.GetMaterialCodeExist(MaterialCode, Plant, UserID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    //if (dr["EXTENDREQTYPE"].ToString() == "0") { isPurshase = 1; }  //Added by TTL Ajit 20-04-25 CR5865
                    if (dr["EXTENDREQTYPE"].ToString() == "0" || dr["EXTENDREQTYPE"].ToString() == "1") { isPurshase = 1; } //Added by Ajit TTL 20-04-20

                    if ((dr["EXTENDREQTYPE"].ToString() == "1" || dr["EXTENDREQTYPE"].ToString() == "2") && (dr["EXTENDREQTYPE"].ToString().Length > 0 && dr["EXTENDREQTYPE"].ToString().Substring(0, 1) == "S"))
                    {
                        if (dr["EXTENDREQTYPE"].ToString() == DisbChnn)
                        { isSales = 1; }
                    }
                }
                //DISTRI_CHN
                //if ((isSales > 0 && lstSales.SelectedIndex > -1)|| (isPurshase > 0 && lstSales.SelectedIndex < 0))
                // {
                //      _isExist = false;
                //     errorMsg.Text = "Material Code " + Convert.ToInt64(txtMaterialCode.Text) + "  already exist in " + ds.Tables[0].Rows[0]["PLANTNAME"].ToString() + " ";
                //     errorPanel.Style.Add(HtmlTextWriterStyle.Display, "block");
                // }          
                // else
                // {
                //     _isExist = true;
                // }
                //Added by TTL Ajit 20-04-25 CR5865
                if ((isSales > 0 && lstSales > -1))
                {
                    _isExist = false;
                    errmsg = "Material Code " + MaterialCode + "  already exist in " + ds.Tables[0].Rows[0]["PLANTNAME"].ToString() + " ";
                    
                }
                else if ((isPurshase > 0 && lstSales < 0) || isPurshase > 0 && lstSales == 1)
                {
                    _isExist = false;
                    errmsg = "Material Code " + MaterialCode + "  already exist in " + ds.Tables[0].Rows[0]["PLANTNAME"].ToString() + " ";
                   
                }
                else
                {
                    _isExist = true;
                }
            }
            //Previously comment but as discuss with yogesh san check implement on 06-Feb-2017
            else if (ds.Tables[1].Rows.Count == 0)
            {
                _isExist = false;
                errmsg = "Material Code does not exist";
                
            }
            else
            {
                _isExist = true;
            }
            return _isExist;
        }
        protected bool ValidateSale(ref string err,int lstSale, MMDetailViewModel data)
        {
            err = "";
            bool flag = true;
            //if (chSales.Checked)
            if (lstSale >= 0)
            {
                
                if (string.IsNullOrEmpty(data.ITEM_CATG_GRP))
                {
                    err += "Item Category/";
                    flag = false;
                }
                //if (ddlGenItemCatgGrp.SelectedIndex <= 0)
                //{
                //    err += "Gen Item Cat Grp/";
                //    flag = false;
                //}
                
                if (string.IsNullOrEmpty(data.AVAIL_CHK))
                {
                    err += "Availability check/";
                    flag = false;
                }
                
                if (string.IsNullOrEmpty(data.TAX_CLASSIFICATION))
                {
                    err += "Tax classification/";
                    flag = false;
                }
                if (string.IsNullOrEmpty(data.TRANSPORTATIONGROUP))
                {
                    err += "Transportation Group/";
                    flag = false;
                }
                if (string.IsNullOrEmpty(data.LOADINGGROUP))
                {
                    err += "Loading Group/";
                    flag = false;
                }
                if (string.IsNullOrEmpty(data.STORAGE_LOC))
                {
                    err += "Storage Location/";
                    flag = false;
                }
                if (string.IsNullOrEmpty(data.PROFITCENTER))
                {
                    err += "Profit Centre/";
                    flag = false;
                }
                if (string.IsNullOrEmpty(data.DISTRI_CHN))
                {
                    err += "Distribution Channel/";
                    flag = false;
                }
                if (string.IsNullOrEmpty(data.BASEUNITOFMEASURE))
                {
                    err += "Base unit of measure/";
                    flag = false;
                }


            }

            return flag;
        }
        private List<MaterialDetail> GridViewBindMMExtendDetails()
        {
            List<MaterialDetail> dataList = new List<MaterialDetail>();
            try
            {
                int UserID = _session.Get<int>("userID");
                DataSet ds = oMasterQueries.MMEXTENDDRAFT_HDDETAIL_GET(UserID);

                if (ds != null)
                {
                    int _MMHEADERID = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.MMHeaderIdE = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                        _session.Set<string>("MMHeaderIdE", ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        _MMHEADERID = Convert.ToInt32(ds.Tables[0].Rows[0]["MMHEADERID"].ToString());

                    }
                    else
                    {
                        ViewBag.MMHeaderIdE = "0";
                        _session.Set<string>("MMHeaderIdE", "");
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        
                        dataList = ds.Tables[1]?.AsEnumerable()
                      .Select(r => new MaterialDetail
                      {
                          MMDETAILID = Convert.ToInt32(r.Field<decimal>("MMDETAILID")),
                          MMHEADERID = _MMHEADERID,
                          MATERIALDISCRIPTION = r.Field<string>("MATERIALDISCRIPTION"),
                          MATERIALCODE = Convert.ToString(r.Field<decimal>("MATERIALCODE")),
                          SALES_ORG = r.Field<string>("SALES_ORG"),
                          DISTRI_CHN = r.Field<string>("DISTRI_CHN"),
                          ITEM_CATG_GRP = r.Field<string>("ITEM_CATG_GRP"),
                          GEN_ITEM_CAT_GRP = r.Field<string>("GEN_ITEM_CAT_GRP"),
                          LOADINGGROUP = r.Field<string>("LOADINGGROUP"),
                          TAX_CLASSIFICATION = r.Field<string>("TAX_CLASSIFICATION"),
                          BASEUNITOFMEASURE = r.Field<string>("BASEUNITOFMEASURE"),
                          PLANTNAME = r.Field<string>("PLANTNAME"),
                          AVAIL_CHK = r.Field<string>("AVAIL_CHK")
                      })
                      .ToList() ?? new List<MaterialDetail>();
                        ViewBag.DraftDetails = dataList;
                    }
                }
                else
                {
                    ViewBag.MMHeaderIdE = "0";
                    _session.Set<string>("MMHeaderIdE", "");
                    ViewBag.SELECTEDPLANT = "0";
                    ViewBag.PROFITCENTER = "";
                    ViewBag.DraftDetails = new List<MaterialDetail>();
                }
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridViewBindMMExtendDetails");
                return dataList;
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetMaterialExtendItemRequest([FromBody] MasterData param)
        {
            try
            {
                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
                int MMDetailId = Convert.ToInt32(param.Code);
                MMDetailEdit data = new MMDetailEdit();
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMMExtendDetailById(MMDetailId);
                if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    data.RS = 0;
                    data.MESSAGE = "Error in data binding!";
                    return Json(data);
                }
                else
                {
                    data.RS = 1;
                }              
               data.MATERIALCODE = ds.Tables[0].Rows[0]["MATERIALCODE"].ToString();
               data.PLANTCODE = ds.Tables[0].Rows[0]["PLANTCODE"].ToString();
               //data.MATERIALDISCRIPTION = ds.Tables[0].Rows[0]["MATERIALDISCRIPTION"].ToString();
               //data.MATERIALSPECIFICATION = ds.Tables[0].Rows[0]["MATERIALSPECIFICATION"].ToString();
               data.ISSALES = "N";
                if (ds.Tables[0].Rows[0]["SALES_ORG"] != null && ds.Tables[0].Rows[0]["SALES_ORG"].ToString() != "")
                {
                    data.SALES_ORG = ds.Tables[0].Rows[0]["SALES_ORG"].ToString();
                    data.ISSALES = "Y";  
                    data.lstSales = ds.Tables[0].Rows[0]["EXTENDREQTYPE"].ToString();
                    data.TRANSPORTATIONGROUP = ds.Tables[0].Rows[0]["TRANSPORTATIONGROUP"].ToString();
                    data.LOADINGGROUP = ds.Tables[0].Rows[0]["LOADINGGROUP"].ToString();
                    data.BASEUNITOFMEASURE = ds.Tables[0].Rows[0]["BASEUNITOFMEASURE"].ToString();
                    data.PROFITCENTER = ds.Tables[0].Rows[0]["PROFITCENTER"].ToString();
                    data.DISTRI_CHN = ds.Tables[0].Rows[0]["DISTRI_CHN"].ToString();
                    data.ITEM_CATG_GRP = ds.Tables[0].Rows[0]["ITEM_CATG_GRP"].ToString();
                    data.AVAIL_CHK = ds.Tables[0].Rows[0]["AVAIL_CHK"].ToString();
                    data.TAX_CLASSIFICATION = ds.Tables[0].Rows[0]["TAX_CLASSIFICATION"].ToString();
                    data.GEN_ITEM_CAT_GRP = ds.Tables[0].Rows[0]["GEN_ITEM_CAT_GRP"].ToString();
                    data.MATERIALTYPE = ds.Tables[0].Rows[0]["MATERIALTYPE"].ToString();
                    if (ds.Tables[0].Rows[0]["MATERIALTYPE"].ToString() == "6") 
                    {
                        data.MAT_GRP_PACK_MATLS = ds.Tables[0].Rows[0]["MAT_GRP_PACK_MATLS"].ToString();
                        data.PACKAGING_MAT_TYPE = ds.Tables[0].Rows[0]["PACKAGING_MAT_TYPE"].ToString();
                    }
                    else
                    {
                        data.MAT_GRP_PACK_MATLS = "";
                        data.PACKAGING_MAT_TYPE = "";
                    }
                    data.STORAGE_LOC = ds.Tables[0].Rows[0]["STORAGE_LOC"].ToString();
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["STORAGE_LOC"].ToString()))
                    {
                        DataSet oDs = GetProfitCentreAndStorageLoc(data.PLANTCODE ?? "0");
                        data.PROFITCENTER = oDs.Tables[0].Rows[0]["PROFIT_CENTER"].ToString();
                        DataSet ds1 = oMasterQueries.GetMasterData("STORAGE_LOCATION", data.PLANTCODE ?? "0");
                        var dataList = ds1.Tables[0]?.AsEnumerable()
                   .Select(r => new MasterData
                   {
                       Code = r.Field<string>("CODE"),
                       Description = r.Field<string>("CODE_DESC"),

                   })
                     .ToList() ?? new List<MasterData>();

                        data.StorageLocList = dataList;
                    }

                }
              
                return Json(data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMaterialExtendItemRequest");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        [HttpPost]
        public async Task<IActionResult> DeleteExtendMaterialItem([FromBody] MasterData param)
        {
            try
            {
                string _MESSAGE = "";
                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
                // string output = oMasterQueries.DeleteMarerialCodeExtendRequest(Convert.ToInt32(e.CommandArgument));
                //DataSet ds = oMasterQueries.MM_HeaderDetailDeleteId_GET(Convert.ToInt32(e.CommandArgument));
                string output = oMasterQueries.DeleteMarerialCodeExtendRequest(Convert.ToInt32(param.Code));
                DataSet ds = oMasterQueries.MM_HeaderDetailDeleteId_GET(Convert.ToInt32(param.Code));
                if (output != "1")
                {
                    return Json(new { RS = 0, MESSAGE = output });
                }
                else
                {
                    //ShowInfo("Material master creation request (" + e.CommandArgument.ToString() + ") is deleted successfully.");
                    if (ds.Tables[1].Rows[0]["ITEMCOUNT"].ToString() != "0")
                    {
                        _MESSAGE = "Material master extend request (" + ds.Tables[0].Rows[0]["MMDETAILHID"].ToString() + ") is deleted successfully.";
                    }
                    else
                    {
                        _MESSAGE = "Material master extend request (" + ds.Tables[0].Rows[0]["HEADERID"].ToString() + ") is deleted successfully.";
                    }
                }
                var dataList = GridViewBindMMExtendDetails();

                return Json(new { RS = 1, MESSAGE = _MESSAGE, dataList });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteExtendMaterialItem");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        [HttpPost]
        public async Task<IActionResult> SubmitExtendMaterialRequest([FromBody] MasterData param)
        {
            try
            {

                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
                string MMHeaderID = _session.Get<string>("MMHeaderIdE");
                int UserID = _session.Get<int>("userID");
                string message = "";
                int rs = 0;
                List<MaterialDetail> dataList1 = new List<MaterialDetail>();

                if (MMHeaderID != null)
                {

                    int MMHeaderId = Convert.ToInt32(MMHeaderID);
                    DataSet ds = oMasterQueries.MMDRAFT_HeaderDetailId_GET(MMHeaderId);                    
                    var dataList = GridViewBindMMExtendDetails();
                    if (dataList.Count > 0)
                    {

                        int _ApprovalCode = Convert.ToInt32(param.Code);

                        string output = oMasterQueries.SubmitMaterialMasterRequest(MMHeaderId, _ApprovalCode);
                        if (output != "1")
                        {
                            message = output;
                            rs = 0;
                        }
                        else
                        {
                            rs = 1;
                            message = "Material master extend request (" + ds.Tables[0].Rows[0]["MMHEADERDETAILID"].ToString() + ") sent successfully for approval.";
                        }
                        // Reset();
                        dataList1 = BindMMCreationDetails();
                        
                    }
                    else
                    {
                        DataSet dsExtend = new DataSet();
                        //Bound old request saved in draft
                        dsExtend = oMasterQueries.MMDRAFT_HeaderDetailId_GET(MMHeaderId);
                      
                        if (dsExtend.Tables[0].Rows.Count > 0)
                        {
                            string output = oMasterQueries.SubmitMaterialMasterRequestDelete(MMHeaderId);
                            if (output != "1")
                            {
                                rs = 0;
                                message = output;
                            }
                            else
                            {
                                rs = 1;
                                message = "Material master creation request (" + ds.Tables[0].Rows[0]["MMHEADERDETAILID"].ToString() + ") sent successfully for approval.";
                            }
                            dataList1 = BindMMCreationDetails();
                        }
                        else
                        {
                            message = "No Material request added for creation";
                        }
                    }
                }

                return Json(new { RS = rs, MESSAGE = message, dataList = dataList1 });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubmitExtendMaterialRequest");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        /******************************Update Material Request********************************************************************/
        [HttpGet]
        public async Task<IActionResult> MaterialMasterUpdateRequest()
        {
            try
            {
                if (_session.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                int UserID = _session.Get<int>("userID");
                ViewBag.SELECTEDPLANT = "0";
                DataSet ds = oMasterQueries.GetMaterialUpdatePendingDetails(UserID);
                if (ds != null)
                {
                    int _MMHEADERID = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.MMHeaderIdU = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                        _session.Set<string>("MMHeaderIdU", ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        _MMHEADERID = Convert.ToInt32(ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        //ViewBag.txtEmailId = ds.Tables[0].Rows[0]["EMAILID"].ToString();                       
                         if (ds.Tables[1].Rows.Count > 0)
                        {
                            ViewBag.SELECTEDPLANT = ds.Tables[1].Rows[0]["PLANTCODE"].ToString();
                        }
                    }
                    else
                    {
                        ViewBag.MMHeaderIdU = "0";
                        _session.Set<string>("MMHeaderIdU", "");
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        try
                        {
                            List<MaterialDetail> dataList = new List<MaterialDetail>();
                            dataList = ds.Tables[1]?.AsEnumerable()
                        .Select(r => new MaterialDetail
                        {
                            MMDETAILID = Convert.ToInt32(r.Field<decimal?>("MMDETAILID")),
                            MMHEADERID = _MMHEADERID,
                            MATERIALDISCRIPTION = r.Field<string>("MATERIALDISCRIPTION"),
                            MATERIALSPECIFICATION = r.Field<string>("MATERIALSPECIFICATION"), 
                            UOM = r.Field<string>("UOM"),
                            PRICE = Convert.ToDouble(r.Field<decimal?>("PRICE")),
                            PLANTNAMEDESC = r.Field<string>("PLANTNAMEDESC"),
                            PLANTNAME = r.Field<string>("PLANTNAME"),
                            PLANT = r.Field<string>("PLANT"),
                            PLANTCODE = Convert.ToInt32(r.Field<decimal?>("PLANTCODE")),
                            IsSales = r.Field<string>("REQ_TYPE"),
                            MATERIALCODE =Convert.ToString(r.Field<decimal?>("MATERIALCODE")),                            
                            MPN_PROFILE = r.Field<string>("MPN_PROFILE"),
                            HSNCODE = r.Field<string>("HSNCODE"),
                            INDICATOR = r.Field<string>("DDLVALUE"),
                            PROFITCENTER =Convert.ToString(r.Field<decimal?>("PROFITCENTER")),
                            PLANT_SP_MAT_STATUS = r.Field<string>("PLANT_SP_MAT_STATUS"),
                            DISTRI_CHN = r.Field<string>("DISTRI_CHN"),
                            ITEM_CATG_GRP = r.Field<string>("ITEM_CATG_GRP"),
                            TRANSPORTATIONGROUP = r.Field<string>("TRANSPORTATIONGROUP"),
                            AVAIL_CHK = r.Field<string>("AVAIL_CHK"),
                            TAX_CLASSIFICATION = r.Field<string>("TAX_CLASSIFICATION"),
                            LOADINGGROUP = r.Field<string>("LOADINGGROUP"),

                        })
                        .ToList() ?? new List<MaterialDetail>();
                            ViewBag.gvMMUpdateDraftDetail = dataList;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "MaterialMasterUpdateRequest-GetMaterialUpdatePendingDetails");
                        }
                    }
                    else
                    {
                        ViewBag.gvMMUpdateDraftDetail = null;
                    }
                }
                else
                {
                    ViewBag.MMHeaderIdU = "0";
                    _session.Set<string>("MMHeaderIdU", "");
                    ViewBag.SELECTEDPLANT = "0";
                    ViewBag.PROFITCENTER = "";
                    ViewBag.gvMMUpdateDraftDetail = new List<MaterialDetail>();
                }
                PopulateControls();
                BindMasterData();
                BindApprovalAuthority();
                BindMMExtendRejectDetails();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MaterialMasterUpdateRequest");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CheckMaterialCodeUpdate([FromBody] CheckMaterialCodeData data)
        {
            MasterResponse res = new MasterResponse();
            try
            {
                if (data == null || data.MaterialCode == "" || data.MaterialCode == "0")
                {
                    return Json(new { Rs = 0, Message = "Please enter material code!" });
                }
                int userId = _session.Get<int>("userID");
                string hsncode = data.MaterialCode ?? "";
                int plantcode = data.PlantCode ?? 0;
                DataSet ds = new DataSet();
                //ds = oMasterQueries.GetMaterialCodeExist(Convert.ToInt32(txtMaterialCode.Text), Convert.ToInt32(ddlPlantMst.SelectedValue), Convert.ToInt32(((Employee_Details)Session["Employee"]).Employee_Code));
                ds = oMasterQueries.GetMaterialCodeDetailUpdate(Convert.ToInt64(hsncode), 0, userId);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    res.Rs = 1;
                  
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["MATERIALTYPE"] != null && row["MATERIALTYPE"].ToString() != "")
                        {
                            res.MATERIALTYPE = row["MATERIALTYPE"].ToString();
                        }
                        if (row["MATERIALSPECIFICATION"] != null && row["MATERIALSPECIFICATION"].ToString() != "")
                        {
                            res.MATERIALSPECIFICATION = row["MATERIALSPECIFICATION"].ToString();
                        }
                        if (row["MATERIALDISCRIPTION"] != null && row["MATERIALDISCRIPTION"].ToString() != "")
                        {
                            res.MATERIALDISCRIPTION = row["MATERIALDISCRIPTION"].ToString();
                        }
                    }
                    if (res.MATERIALTYPE == "19")
                    {
                        res.Message = "Enter valid material code or ZMPN material type code cannot update!";
                        res.MATERIALDISCRIPTION = ""; ;
                        res.MATERIALSPECIFICATION="";
                        res.MATERIALTYPE = "";
                        res.Rs = 0;
                        return Json(res);
                    }

                    MasterResponse dd = _mmService.GetProfitCentreAndStorageLoc(ds.Tables[0].Rows[0]["PLANTCODE"].ToString());
                    res.ProfitCentre = dd.ProfitCentre;
                    res.list = dd.list;
                    if (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["ISSALES"].ToString()) && ds.Tables[0].Rows[0]["ISSALES"].ToString() == "1")
                    {
                        res.ISSALES = "YES";                      
                        DataSet ds1 = oMasterQueries.GetMasterData("STORAGE_LOCATION", ds.Tables[0].Rows[0]["PLANTCODE"].ToString());
                        var dataList1 = ds1.Tables[0]?.AsEnumerable()
                       .Select(r => new MasterData
                       {
                           Code = r.Field<string>("CODE"),
                           Description = r.Field<string>("CODE_DESC"),

                       })
                     .ToList() ?? new List<MasterData>();
                        res.list = dataList1;

                    }
                    else
                    {
                        res.ISSALES = "NO";

                    }
                    DataTable tmpdt = ds.Tables[0].DefaultView.ToTable(true, "PLANTCODE", "PLANTNAME");
                    //ddlPlantMst.DataTextField = "PLANTNAME";
                    //ddlPlantMst.DataValueField = "PLANTCODE";
                    //ddlPlantMst.DataSource = tmpdt;
                    //ddlPlantMst.DataBind();
                    var dataList = tmpdt?.AsEnumerable()
                      .Select(r => new MasterData
                      {
                          Code =Convert.ToString(r.Field<decimal?>("PLANTCODE")),
                          Description = r.Field<string>("PLANTNAME"),

                      })
                    .ToList() ?? new List<MasterData>();
                   
                    res.Plantlist = dataList;
                }
                else
                {
                    res.Rs = 0;
                    res.Message = "Enter valid material code!";
                }
                return Json(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MaterialMasterExtendRequest");
                return Json(new { Rs = 0, Message = "Error in material code finding!" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddUpdateMaterialRequest([FromBody] MMDetailViewModel data)
        {
            List<MaterialDetail> dataList = new List<MaterialDetail>();
            try
            {
                if (data == null)
                {
                    return Json(new { Rs = 0, Message = "Invalid ajax calling" });
                }
                if (data.MATERIALCODE==null || data.MATERIALCODE==0)
                {
                    return Json(new { Rs = 0, Message = "Kindly check  material code." });
                }
                if (data.IsSales=="Y" && (data.DISTRI_CHN==null || data.DISTRI_CHN==""))
                {
                   return Json(new { Rs = 0, Message = "Distribution  channel is required for Sales View" });
                }

                int ppcApprovalAuth = 0; int financeApprovalAuth = 0;
               
                string materialType =Convert.ToString(data.MATERIALTYPE);
                string materialDesription =data.MATERIALDISCRIPTION??"".ToUpper().Trim();
                string materialSpecification =data.MATERIALSPECIFICATION??"".ToUpper().Trim();
                string mpn_profile =data.MPN_PROFILE??"";
                string plant_sp_material_status =data.PLANT_SP_MAT_STATUS??"";
                string material_code =Convert.ToString(data.MATERIALCODE??0);
                int UOM = Convert.ToInt32(data.MEASUREMENTUNIT);                
                string DistriChnn = data.DISTRI_CHN??"";
                Employee_Details employee = _session.Get<Employee_Details>("Employee");
                int UserId = _session.Get<int>("userID");
                int plantCode = data.PLANTCODE ?? 0;
                string emailId = data.EMAIL_ID ?? employee.EMail_Id;
                int headerId = 0;
                long MaterialCode = data.MATERIALCODE ?? 0;
                int Plant = data.PLANTCODE ?? 0;
                string DisbChnn = data.DISTRI_CHN ?? "";
                string errorMsg = "";
                int lstSales = Convert.ToInt32(data.lstSales == "" ? "0" : data.lstSales);
                if (data.MMHEADERID != 0)
                {
                    data.MMHEADERID = data.MMHEADERID;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_session.Get<string>("MMHeaderIdU")))
                    {
                        data.MMHEADERID = _session.Get<int>("MMHeaderIdU");

                    }
                }

                headerId = data.MMHEADERID ?? 0;
                string err = "";

                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialCountDetails(headerId);
                if (ds.Tables[0].Rows.Count >= 20)
                {
                    return Json(new { Rs = 0, Message = "Only 20 material code request can be added in a request." });
                }
                else
                {
                    if (MaterialCodeCheck(MaterialCode, Plant, DisbChnn, lstSales, ref errorMsg))
                    {
                        string output;
                        if (data.MMDETAILID == null || data.MMDETAILID == 0) //in case of Add MMDETAILID is 0 or null
                        {
                            //output = oMasterQueries.AddMarerialCodeExtendRequest(Convert.ToInt32(txtMaterialCode.Text), Convert.ToInt32(ddlPlantMst.SelectedValue), txt_description, txt_specification, Convert.ToInt32(((Employee_Details)Session["Employee"]).Employee_Code), emailId);
                            //if (chSales.Checked)
                            if (data.IsSales!="Y")
                            {
                                if (ValidatePurchase(data,ref err))
                                {
                                    return Json(new { Rs = 0, Message = "Kindly select or enter any  field of update!" });
                                }                                
                                output = oMasterQueries.AddMarerialCodeUpdateRequest(headerId, plantCode, materialDesription, materialSpecification, UOM, UserId, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE, data.MMINDICATER, "", "", "", "", "", "", "", "", "", "", "", "", "", material_code, mpn_profile, plant_sp_material_status, materialType);
                            }
                            else
                            {
                                if (ValidateSale(data))
                                {
                                  return Json(new { Rs = 0, Message = "Enter or select any on field  of Sales View!!" });
                                }

                                output = oMasterQueries.AddMarerialCodeUpdateRequest(headerId, plantCode, materialDesription, materialSpecification, UOM, UserId, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE,data.MMINDICATER,data.TRANSPORTATIONGROUP,data.LOADINGGROUP, "",data.SALES_ORG, DistriChnn,data.ITEM_CATG_GRP,data.AVAIL_CHK,data.PROFITCENTER,data.STORAGE_LOC,data.TAX_CLASSIFICATION,data.GEN_ITEM_CAT_GRP,data.MAT_GRP_PACK_MATLS,data.PACKAGING_MAT_TYPE, material_code, mpn_profile, plant_sp_material_status, materialType);
                            }
                            if (output == "1")
                            {
                                string PlantCode = "";
                                dataList = GridViewBindMMUpdateDetails(ref PlantCode);
                                return Json(new { Rs = 1, Message = "Material master creation requested is added successfully.", PlantCode, dataList });
                            }
                            else
                            {
                                return Json(new { Rs = 0, Message = output });
                            }
                        }
                        else
                        {                            
                            if (data.IsSales=="Y")
                            {
                                if (ValidateSale(data))
                                {
                                    return Json(new { Rs = 0, Message = "Enter or select any on field  of Sales View!!" });
                                }
                                output = oMasterQueries.UpdateMarerialCodeUpdateRequest(headerId, data.MMDETAILID??0, plantCode, materialDesription, materialSpecification, UOM, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE,data.MMINDICATER, UserId,data.TRANSPORTATIONGROUP,data.LOADINGGROUP, "",data.SALES_ORG, DistriChnn,data.ITEM_CATG_GRP,data.AVAIL_CHK,data.PROFITCENTER,data.STORAGE_LOC,data.TAX_CLASSIFICATION,data.GEN_ITEM_CAT_GRP,data.MAT_GRP_PACK_MATLS,data.PACKAGING_MAT_TYPE, material_code, mpn_profile, plant_sp_material_status, materialType);
                            }
                            else
                            {
                                if (ValidatePurchase(data, ref err))
                                {
                                    return Json(new { Rs = 0, Message = "Kindly select or enter any  field of update!" });
                                }
                                //output = oMasterQueries.UpdateMarerialCodeExtendRequestSales(MMExheaderId, data.MMDETAILID ?? 0, MaterialCode, Plant, emailId, "", "", "", "", "", "", "", "", "", "", "", "", "", "0");
                                output = oMasterQueries.UpdateMarerialCodeUpdateRequest(headerId, data.MMDETAILID ?? 0, plantCode, materialDesription, materialSpecification, UOM, emailId, ppcApprovalAuth, financeApprovalAuth, data.HSNCODE,data.MMINDICATER, UserId, "", "", "", "", "", "", "", "", "", "", "", "", "", material_code, mpn_profile, plant_sp_material_status, materialType);
                            }
                            if (output != "1")
                            {
                                return Json(new { Rs = 0, Message = output });
                            }
                            else
                            {
                                string PlantCode = "";
                                dataList = GridViewBindMMUpdateDetails(ref PlantCode);
                                string message = "Material master updation requested (" + data.MMDETAILID.ToString() + ") is updated successfully.";
                                return Json(new { Rs = 1, Message = message, PlantCode, dataList });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Rs = 0, Message = errorMsg });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddMoreMaterial");
                return Json(new { Rs = 0, Message = ex });
            }
        }
        protected bool ValidatePurchase(MMDetailViewModel data,ref string err)
        {
            err = "";
            bool flag = true;



            if (data.BASEUNITOFMEASURE!=null && data.BASEUNITOFMEASURE!="")
            {
                flag = false;
            }
            if (data.MMINDICATER!=null && data.MMINDICATER!="")
            {
                flag = false;
            }
            if (data.PLANT_SP_MAT_STATUS != null && data.PLANT_SP_MAT_STATUS != "")
            {

                flag = false;
            }
            if (data.MPN_PROFILE != null && data.MPN_PROFILE != "")
            {
                flag = false;
            }

            if (data.HSNCODE != null && data.HSNCODE != "")
            {
                flag = false;
            }
            if (data.MATERIALDISCRIPTION != null && data.MATERIALDISCRIPTION != "")
            {
                flag = false;
            }
            if (data.MATERIALSPECIFICATION != null && data.MATERIALSPECIFICATION != "")
            {
                flag = false;
            }

            if (data.PLANTCODE != null && data.PLANTCODE ==0)
            {
                err = "Please select plant!";
                flag = true;
            }


            return flag;
        }
        protected bool ValidateSale(MMDetailViewModel data)
        {

            bool flag = true;
            if (data.IsSales=="Y")
            {

                if (data.ITEM_CATG_GRP!=null && data.ITEM_CATG_GRP != "")
                {
                    flag = false;
                }
                if (data.GEN_ITEM_CAT_GRP != null && data.GEN_ITEM_CAT_GRP != "")
                {
                    flag = false;
                }
                if (data.AVAIL_CHK != null && data.AVAIL_CHK != "")
                {
                    flag = false;
                }
                if (data.TAX_CLASSIFICATION != null && data.TAX_CLASSIFICATION != "")
                {
                    flag = false;
                }
                if (data.TRANSPORTATIONGROUP != null && data.TRANSPORTATIONGROUP != "")
                {
                    flag = false;
                }
                if (data.LOADINGGROUP != null && data.LOADINGGROUP != "")
                {
                    flag = false;
                }
                if (data.STORAGE_LOC != null && data.STORAGE_LOC != "")
                {
                    flag = false;
                }
                if (data.PROFITCENTER != null && data.PROFITCENTER != "")
                {
                    flag = false;
                }

                //if (ddlMaterialType.SelectedValue == "ZPAC")
                //{

                //    if (txtMatGrpPack.Text.Trim().Length <= 0)
                //    {
                //        err += "Mat. Grp Pack. Matls/";
                //        flag = false;
                //    }

                //    if (txtPackagingMatType.Text.Trim().Length <= 0)
                //    {
                //        err += "Packaging mat. type";
                //        flag = false;
                //    }
                //}
                //else
                //{
                //    txtMatGrpPack.Text = "";
                //    txtPackagingMatType.Text = "";
                //}
                if (data.DISTRI_CHN == null && data.DISTRI_CHN == "")
                {
                    flag = true;
                }

            }

            return flag;
        }
        private List<MaterialDetail> GridViewBindMMUpdateDetails(ref string PlantCode)
        {
            List<MaterialDetail> dataList = new List<MaterialDetail>();
            try
            {
                int UserID = _session.Get<int>("userID");
                DataSet ds = oMasterQueries.GetMaterialUpdatePendingDetails(UserID);
                 PlantCode = "";
                if (ds != null)
                {
                    int _MMHEADERID = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.MMHeaderIdU = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                        _session.Set<string>("MMHeaderIdU", ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        _MMHEADERID = Convert.ToInt32(ds.Tables[0].Rows[0]["MMHEADERID"].ToString());
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            PlantCode= ds.Tables[1].Rows[0]["PLANTCODE"].ToString();
                        }
                    }
                    else
                    {
                        ViewBag.MMHeaderIdU = "0";
                        _session.Set<string>("MMHeaderIdU", "");
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {

                        dataList = ds.Tables[1]?.AsEnumerable()
                      .Select(r => new MaterialDetail
                      {
                          MMDETAILID = Convert.ToInt32(r.Field<decimal?>("MMDETAILID")),
                          MMHEADERID = _MMHEADERID,
                          MATERIALDISCRIPTION = r.Field<string>("MATERIALDISCRIPTION"),
                          MATERIALSPECIFICATION = r.Field<string>("MATERIALSPECIFICATION"),
                          UOM = r.Field<string>("UOM"),
                          PRICE = Convert.ToDouble(r.Field<decimal?>("PRICE")),
                          PLANTNAMEDESC = r.Field<string>("PLANTNAMEDESC"),
                          PLANTNAME = r.Field<string>("PLANTNAME"),
                          PLANT = r.Field<string>("PLANT"),
                          PLANTCODE = Convert.ToInt32(r.Field<decimal?>("PLANTCODE")),
                          IsSales = r.Field<string>("REQ_TYPE"),
                          MATERIALCODE = Convert.ToString(r.Field<decimal?>("MATERIALCODE")),
                          MPN_PROFILE = r.Field<string>("MPN_PROFILE"),
                          HSNCODE = r.Field<string>("HSNCODE"),
                          INDICATOR = r.Field<string>("DDLVALUE"),
                          PROFITCENTER = Convert.ToString(r.Field<decimal?>("PROFITCENTER")),
                          PLANT_SP_MAT_STATUS = r.Field<string>("PLANT_SP_MAT_STATUS"),
                          DISTRI_CHN = r.Field<string>("DISTRI_CHN"),
                          ITEM_CATG_GRP = r.Field<string>("ITEM_CATG_GRP"),
                          TRANSPORTATIONGROUP = r.Field<string>("TRANSPORTATIONGROUP"),
                          AVAIL_CHK = r.Field<string>("AVAIL_CHK"),
                          TAX_CLASSIFICATION = r.Field<string>("TAX_CLASSIFICATION"),
                          LOADINGGROUP = r.Field<string>("LOADINGGROUP"),
                      })
                      .ToList() ?? new List<MaterialDetail>();
                        ViewBag.DraftDetails = dataList;
                    }
                }
                else
                {
                    ViewBag.MMHeaderIdU = "0";
                    _session.Set<string>("MMHeaderIdU", "");
                    ViewBag.SELECTEDPLANT = "0";
                    ViewBag.PROFITCENTER = "";
                    ViewBag.DraftDetails = new List<MaterialDetail>();
                }
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridViewBindMMUpdateDetails");
                return dataList;
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetMaterialUpdateItemRequest([FromBody] MasterData param)
        {
            try
            {
                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
                int MMDetailId = Convert.ToInt32(param.Code);
                MMDetailEdit data = new MMDetailEdit();
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMMCreationDetailById(MMDetailId);
                if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    data.RS = 0;
                    data.MESSAGE = "Error in data binding!";
                    return Json(data);
                }
                else
                {
                    data.RS = 1;
                }
                data.MATERIALCODE = ds.Tables[0].Rows[0]["MATERIALCODE"].ToString();
                data.PLANTCODE = ds.Tables[0].Rows[0]["PLANTCODE"].ToString();
                data.MATERIALDISCRIPTION = ds.Tables[0].Rows[0]["MATERIALDISCRIPTION"].ToString();
                data.MATERIALSPECIFICATION = ds.Tables[0].Rows[0]["MATERIALSPECIFICATION"].ToString();
                data.MATERIALTYPE= ds.Tables[0].Rows[0]["MATERIALTYPE"].ToString();
                data.MEASUREMENTUNIT = ds.Tables[0].Rows[0]["MEASUREMENTUNIT"].ToString();
                data.HSNCODE= ds.Tables[0].Rows[0]["HSNCODE"].ToString();
                data.PLANT_SP_MAT_STATUS = ds.Tables[0].Rows[0]["PLANT_SP_MAT_STATUS"].ToString();
                data.MPN_PROFILE = ds.Tables[0].Rows[0]["MPN_PROFILE"].ToString();
                data.MMINDICATER = ds.Tables[0].Rows[0]["MMINDICATER"].ToString();

                data.ISSALES = "N";
                if (ds.Tables[0].Rows[0]["SALES_ORG"] != null && ds.Tables[0].Rows[0]["SALES_ORG"].ToString() != "")
                {
                    data.SALES_ORG = ds.Tables[0].Rows[0]["SALES_ORG"].ToString();
                    data.ISSALES = "Y";                    
                    data.TRANSPORTATIONGROUP = ds.Tables[0].Rows[0]["TRANSPORTATIONGROUP"].ToString();
                    data.LOADINGGROUP = ds.Tables[0].Rows[0]["LOADINGGROUP"].ToString();
                    data.BASEUNITOFMEASURE = ds.Tables[0].Rows[0]["BASEUNITOFMEASURE"].ToString();
                    data.PROFITCENTER = ds.Tables[0].Rows[0]["PROFITCENTER"].ToString();
                    data.DISTRI_CHN = ds.Tables[0].Rows[0]["DISTRI_CHN"].ToString();
                    data.ITEM_CATG_GRP = ds.Tables[0].Rows[0]["ITEM_CATG_GRP"].ToString();
                    data.AVAIL_CHK = ds.Tables[0].Rows[0]["AVAIL_CHK"].ToString();
                    data.TAX_CLASSIFICATION = ds.Tables[0].Rows[0]["TAX_CLASSIFICATION"].ToString();
                    data.GEN_ITEM_CAT_GRP = ds.Tables[0].Rows[0]["GEN_ITEM_CAT_GRP"].ToString();
                    data.MATERIALTYPE = ds.Tables[0].Rows[0]["MATERIALTYPE"].ToString();
                    //data.PROFITCENTER = ds.Tables[0].Rows[0]["PROFIT_CENTER"].ToString();
                    if (ds.Tables[0].Rows[0]["PACKAGING_MAT_TYPE"].ToString() != "" || ds.Tables[0].Rows[0]["MAT_GRP_PACK_MATLS"].ToString() != "")
                    {
                        data.MAT_GRP_PACK_MATLS = ds.Tables[0].Rows[0]["MAT_GRP_PACK_MATLS"].ToString();
                        data.PACKAGING_MAT_TYPE = ds.Tables[0].Rows[0]["PACKAGING_MAT_TYPE"].ToString();
                    }
                    else
                    {
                        data.MAT_GRP_PACK_MATLS = "";
                        data.PACKAGING_MAT_TYPE = "";
                    }
                    data.STORAGE_LOC = ds.Tables[0].Rows[0]["STORAGE_LOC"].ToString();
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["STORAGE_LOC"].ToString()))
                    {
                        DataSet oDs = GetProfitCentreAndStorageLoc(data.PLANTCODE ?? "0");
                        data.PROFITCENTER = oDs.Tables[0].Rows[0]["PROFIT_CENTER"].ToString();
                        DataSet ds1 = oMasterQueries.GetMasterData("STORAGE_LOCATION", data.PLANTCODE ?? "0");
                        var dataList = ds1.Tables[0]?.AsEnumerable()
                   .Select(r => new MasterData
                   {
                       Code = r.Field<string>("CODE"),
                       Description = r.Field<string>("CODE_DESC"),

                   })
                     .ToList() ?? new List<MasterData>();

                        data.StorageLocList = dataList;
                    }

                }

                return Json(data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMaterialUpdateItemRequest");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        [HttpPost]
        public async Task<IActionResult> SubmitUpdateMaterialRequest([FromBody] MasterData param)
        {
            try
            {

                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
                string MMHeaderID = _session.Get<string>("MMHeaderIdU");
                int UserID = _session.Get<int>("userID");
                string message = "";
                int rs = 0;
                List<MaterialDetail> dataList1 = new List<MaterialDetail>();

                if (MMHeaderID != null)
                {

                    int MMHeaderId = Convert.ToInt32(MMHeaderID);
                    DataSet ds = oMasterQueries.MMDRAFT_HeaderDetailId_GET(MMHeaderId);
                    string plantcode = "";
                    var dataList = GridViewBindMMUpdateDetails(ref plantcode);
                    if (dataList.Count > 0)
                    {

                        int _ApprovalCode = Convert.ToInt32(param.Code);

                        string output = oMasterQueries.SubmitMaterialMasterUPDATERequest(MMHeaderId, _ApprovalCode);
                        if (output != "1")
                        {
                            message = output;
                            rs = 0;
                        }
                        else
                        {
                            rs = 1;
                            message = "Material master updation request (" + ds.Tables[0].Rows[0]["MMHEADERDETAILID"].ToString() + ") sent successfully for approval.";
                        }
                        // Reset();
                        dataList1 = GridViewBindMMUpdateDetails(ref plantcode);

                    }
                    else
                    {
                        DataSet dsCreation = new DataSet();

                        //Bound old request saved in draft
                        dsCreation = oMasterQueries.GetMaterialCreationPendingDetails(UserID);

                        if (dsCreation.Tables[0].Rows.Count > 0)
                        {
                            string output = oMasterQueries.SubmitMaterialMasterRequestDelete(MMHeaderId);
                            if (output != "1")
                            {
                                rs = 0;
                                message = output;
                            }
                            else
                            {
                                rs = 1;
                                message = "Material master update request (" + ds.Tables[0].Rows[0]["MMHEADERDETAILID"].ToString() + ") sent successfully for approval.";
                            }
                            dataList1 = GridViewBindMMUpdateDetails(ref plantcode);
                        }
                        else
                        {
                            message = "No Material request added for creation";
                        }
                    }
                }

                return Json(new { RS = rs, MESSAGE = message, dataList = dataList1 });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubmitUpdateMaterialRequest");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        [HttpPost]
        public async Task<IActionResult> DeleteUpdateMaterialItem([FromBody] MasterData param)
        {
            try
            {
                string _MESSAGE = "";
                if (param == null || param.Code == null || param.Code == "0")
                {
                    return Json(new { RS = 0, MESSAGE = "Invalid request parameter!" });
                }
              
                string output = oMasterQueries.DeleteMarerialCodeCreationRequest(Convert.ToInt32(param.Code));
                DataSet ds = oMasterQueries.MM_HeaderDetailDeleteId_GET(Convert.ToInt32(param.Code));
                if (output != "1")
                {
                    return Json(new { RS = 0, MESSAGE = output });
                }
                else
                {
                    //ShowInfo("Material master creation request (" + e.CommandArgument.ToString() + ") is deleted successfully.");
                    if (ds.Tables[1].Rows[0]["ITEMCOUNT"].ToString() != "0")
                    {
                        _MESSAGE = "Material master updated request (" + ds.Tables[0].Rows[0]["MMDETAILHID"].ToString() + ") is deleted successfully.";
                    }
                    else
                    {
                        _MESSAGE = "Material master updated request (" + ds.Tables[0].Rows[0]["HEADERID"].ToString() + ") is deleted successfully.";
                    }
                }
                var dataList = GridViewBindMMExtendDetails();

                return Json(new { RS = 1, MESSAGE = _MESSAGE, dataList });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUpdateMaterialItem");
                return Json(new { RS = 0, MESSAGE = ex });
            }


        }
        /******************************************************************************************************/
        [HttpGet]
        public IActionResult MMUOMGroupTypeValuationMatrix()
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new MMUOMGroupTypeValuationMatrix
            {
                UomPanelVisible = false,
                GroupPanelVisible = false,
                TypePanelVisible = false,
                ValuationPanelVisible = false,
                SitePanelVisible = false,
                SubmitPanelVisible = false,
                BtnSubmitText = "Submit",
                DdlPlantMstEnabled = true,
                TrPlantNameVisible = false,
                TrSalesVisible = false,
                PlantList = GetAllPlantList(),
                PlantSLList = GetMasterPlantList()
            };

            return View(model);
        }

        private List<SelectListItem> GetAllPlantList()
        {
            var ds = oMasterQueries.GetAllPlantList();
            var list = new List<SelectListItem>();
            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new SelectListItem
                {
                    Text = row["PLANTDESC"].ToString(),
                    Value = row["PLANTNAME"].ToString()
                });
            }
            list.Insert(0, new SelectListItem { Text = "-- select --", Value = "0" });
            return list;
        }

        private List<SelectListItem> GetMasterPlantList()
        {
            var ds = oMasterQueries.GetMasterPlantList();
            var list = new List<SelectListItem>();
            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new SelectListItem
                {
                    Text = row["PLANTNAMEDESC"].ToString(),
                    Value = row["SAPDESC"].ToString()
                });
            }
            list.Insert(0, new SelectListItem { Text = "-- select --", Value = "0" });
            return list;
        }

        [HttpPost]
        public IActionResult MMUOMGroupTypeValuationMatrixIndexChanged([FromBody] MMUOMGroupTypeValuationMatrix model)
        {
            if (model.SelectedMasterEntry != 0)
            {
                model.TrSalesVisible = false;

                switch (model.SelectedMasterEntry)
                {
                    case 1:
                        model.UomPanelVisible = true;
                        model.GroupPanelVisible = false;
                        model.TypePanelVisible = false;
                        model.ValuationPanelVisible = false;
                        model.SitePanelVisible = false;
                        model.SubmitPanelVisible = true;
                        //GridViewBindDetails(model.SelectedMasterEntry);
                        break;

                    case 2:
                        model.GroupPanelVisible = true;
                        model.UomPanelVisible = false;
                        model.TypePanelVisible = false;
                        model.ValuationPanelVisible = false;
                        model.SitePanelVisible = false;
                        model.SubmitPanelVisible = true;
                        //GridViewBindDetails(model.SelectedMasterEntry);
                        break;

                    case 3:
                        model.TypePanelVisible = true;
                        model.UomPanelVisible = false;
                        model.GroupPanelVisible = false;
                        model.ValuationPanelVisible = false;
                        model.SitePanelVisible = false;
                        model.SubmitPanelVisible = true;
                        //GridViewBindDetails(model.SelectedMasterEntry);
                        break;

                    case 4:
                        model.ValuationPanelVisible = true;
                        model.UomPanelVisible = false;
                        model.GroupPanelVisible = false;
                        model.TypePanelVisible = false;
                        model.SitePanelVisible = false;
                        model.SubmitPanelVisible = true;
                        //GridViewBindDetails(model.SelectedMasterEntry);
                        break;

                    case 5:
                        model.SitePanelVisible = true;
                        model.ValuationPanelVisible = false;
                        model.UomPanelVisible = false;
                        model.GroupPanelVisible = false;
                        model.TypePanelVisible = false;
                        model.SubmitPanelVisible = true;
                        //GridViewBindDetails(model.SelectedMasterEntry);
                        break;

                    case 11:
                        model.TrPlantNameVisible = true;
                        model.TrSalesVisible = true;
                        model.SitePanelVisible = false;
                        model.ValuationPanelVisible = false;
                        model.UomPanelVisible = false;
                        model.GroupPanelVisible = false;
                        model.TypePanelVisible = false;
                        model.SubmitPanelVisible = true;
                        model.HeadSalesText = model.SelectedMasterEntry.ToString().ToLowerInvariant() + " Details";
                        //GridViewBindDetails(model.SelectedMasterEntry);
                        break;

                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 12:
                        model.TrPlantNameVisible = false;
                        model.TrSalesVisible = true;
                        model.SitePanelVisible = false;
                        model.ValuationPanelVisible = false;
                        model.UomPanelVisible = false;
                        model.GroupPanelVisible = false;
                        model.TypePanelVisible = false;
                        model.SubmitPanelVisible = true;
                        model.HeadSalesText = model.SelectedMasterEntry.ToString().ToLowerInvariant() + " Details";
                        //GridViewBindDetails(model.SelectedMasterEntry);
                        break;
                }
            }
            var ds = oMasterQueries.GetMaterialMasterMatrixList(model.SelectedMasterEntry);

            var rows = new List<MaterialGroupDetailsViewModel>();

            if (ds.Tables.Count > 0)
            {
                foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                {
                    rows.Add(new MaterialGroupDetailsViewModel
                    {
                        TYPEID =Convert.ToInt32(row["TYPEID"]?.ToString()),
                        TYPE = row["TYPE"]?.ToString(),
                        DESC = row["DESC"]?.ToString(),
                        Code = row.Table.Columns.Count > 2 ? row["CODE"]?.ToString() : null,
                        SITENAME = row.Table.Columns.Count > 3 ? row["SITENAME"]?.ToString() : null,
                        SITEDESC = row.Table.Columns.Count > 4 ? row["SITEDESC"]?.ToString() : null
                    });
                }
            }
            model.MaterialGroupRecords = rows;

            switch (model.SelectedMasterEntry)
            {
                case 1:

                    //gvList.Columns[4].Visible = false;
                    //gvList.Columns[3].Visible = false;
                    //gvList.Columns[2].Visible = false;
                    //gvList.Columns[0].HeaderText = "UOM";
                    //gvList.Columns[1].HeaderText = "UOM DESCRIPTION";
                    model.Col5.IsVisible = false;
                    model.Col4.IsVisible = false;
                    model.Col3.IsVisible = false;
                    model.Col1.Header = "UOM";
                    model.Col2.Header = "UOM DESCRIPTION";
                    break;
                case 2:
                    //gvList.Columns[4].Visible = false;
                    //gvList.Columns[3].Visible = false;
                    //gvList.Columns[2].Visible = false;
                    //gvList.Columns[0].HeaderText = "Material Group";
                    //gvList.Columns[1].HeaderText = "Group Description";
                    model.Col5.IsVisible = false;
                    model.Col4.IsVisible = false;
                    model.Col3.IsVisible = false;
                    model.Col1.Header = "Material Group";
                    model.Col2.Header = "Group Description";
                    break;
                case 3:
                    //gvList.Columns[4].Visible = true;
                    //gvList.Columns[3].Visible = true;
                    //gvList.Columns[2].Visible = false;
                    //gvList.Columns[0].HeaderText = "Material Type";
                    //gvList.Columns[1].HeaderText = "Type Description";
                    //gvList.Columns[4].HeaderText = "IsSales";
                    //gvList.Columns[3].HeaderText = "IsPurchase";
                    model.Col5.IsVisible = true;
                    model.Col4.IsVisible = true;
                    model.Col3.IsVisible = false;
                    model.Col1.Header = "Material Type";
                    model.Col2.Header = "Type Description";
                    model.Col5.Header = "IsSales";
                    model.Col4.Header = "IsPurchase";
                    break;
                case 4:
                    //gvList.Columns[4].Visible = false;
                    //gvList.Columns[3].Visible = false;
                    //gvList.Columns[2].Visible = false;
                    //gvList.Columns[0].HeaderText = "Valuation Class";
                    //gvList.Columns[1].HeaderText = "Valuation Description";
                    model.Col5.IsVisible = false;
                    model.Col4.IsVisible = false;
                    model.Col3.IsVisible = false;
                    model.Col1.Header = "Valuation Class";
                    model.Col2.Header = "Valuation Description";
                   
                    break;
                case 5:
                    //gvList.Columns[4].Visible = true;
                    //gvList.Columns[3].Visible = true;
                    //gvList.Columns[4].HeaderText = "Sis Plant Code";
                    //gvList.Columns[3].HeaderText = "Sis Plant Name";
                    //gvList.Columns[2].Visible = true;
                    //gvList.Columns[0].HeaderText = "User Plant Name";
                    //gvList.Columns[1].HeaderText = "User Plant Code";
                    model.Col5.IsVisible = true;
                    model.Col4.IsVisible = true;
                    model.Col3.IsVisible = true;
                    model.Col1.Header = "User Plant Name";
                    model.Col2.Header = "User Plant Code";
                    model.Col2.Header = "Sis Plant Name";
                    model.Col2.Header = "Sis Plant Code";
                    break;
                case 11:
                    //gvList.Columns[4].Visible = false;
                    //gvList.Columns[3].Visible = false;
                    //gvList.Columns[2].Visible = true;
                    //gvList.Columns[0].HeaderText = "Code";
                    //gvList.Columns[1].HeaderText = "Description";
                    //gvList.Columns[2].HeaderText = "Plant";
                    //gvList.Columns[3].HeaderText = "Status";
                    model.Col5.IsVisible = true;
                    model.Col4.IsVisible = true;
                    model.Col3.IsVisible = true;
                    model.Col1.Header = "User Plant Name";
                    model.Col2.Header = "User Plant Code";
                    model.Col2.Header = "Sis Plant Name";
                    model.Col2.Header = "Sis Plant Code";
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 12:
                    //gvList.Columns[4].Visible = false;
                    //gvList.Columns[3].Visible = false;
                    //gvList.Columns[2].Visible = false;
                    //gvList.Columns[0].HeaderText = "Code";
                    //gvList.Columns[1].HeaderText = "Description";
                    //gvList.Columns[3].HeaderText = "Status";
                    model.Col5.IsVisible = false;
                    model.Col4.IsVisible = false;
                    model.Col2.IsVisible = false;
                    model.Col1.Header = "Code";
                    model.Col2.Header = "Description";
                    model.Col4.Header = "Status";

                    break;
            }

            return PartialView("_GetMMUOMGroupTypeValuationMatrix", model);
            //return View(model);
        }

        public IActionResult GridViewBindDetails(int selectedMasterEntry)
        {
            // get dataset from your query service
            var ds = oMasterQueries.GetMaterialMasterMatrixList(selectedMasterEntry);

            var rows = new List<MaterialGroupDetailsViewModel>();

            if (ds.Tables.Count > 0)
            {
                foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                {
                    rows.Add(new MaterialGroupDetailsViewModel
                    {
                        TYPE = row[0]?.ToString(),
                        DESC = row[1]?.ToString(),
                        Code = row.Table.Columns.Count > 2 ? row[2]?.ToString() : null,
                        SITENAME = row.Table.Columns.Count > 3 ? row[3]?.ToString() : null,
                        SITEDESC = row.Table.Columns.Count > 4 ? row[4]?.ToString() : null
                    });
                }
            }

            // decide which headers/columns to show based on selectedMasterEntry
            var model = new MMUOMGroupTypeValuationMatrix
            {
                SelectedMasterEntry = selectedMasterEntry,
                MaterialGroupRecords = rows
            };
            switch (selectedMasterEntry)
            {
                case 1:
                    model.UomPanelVisible = true;
                    model.GroupPanelVisible = false;
                    model.TypePanelVisible = false;
                    model.ValuationPanelVisible = false;
                    model.SitePanelVisible = false;
                    model.SubmitPanelVisible = true;
                    break;
                case 2:
                    model.Col5.IsVisible = false;
                    model.Col4.IsVisible = false;
                    model.Col3.IsVisible = false;
                    model.Col1.Header = "Material Group";
                    model.Col2.Header = "Group Description";
                    break;
                case 3:
                    model.Col5.IsVisible = true;
                    model.Col4.IsVisible = true;
                    model.Col3.IsVisible = false;
                    model.Col1.Header = "Material Type";
                    model.Col2.Header = "Type Description";
                    model.Col5.Header = "IsSales";
                    model.Col4.Header = "IsPurchase";
                    break;
                case 4:
                    model.Col5.IsVisible = false;
                    model.Col4.IsVisible = false;
                    model.Col3.IsVisible = false;
                    model.Col1.Header = "Valuation Class";
                    model.Col2.Header = "Valuation Description";
                    break;
                case 5:
                    model.Col5.IsVisible = true;
                    model.Col4.IsVisible = true;
                    model.Col3.IsVisible = true;
                    model.Col1.Header = "User Plant Name";
                    model.Col2.Header = "User Plant Code";
                    model.Col2.Header = "Sis Plant Name";
                    model.Col2.Header = "Sis Plant Code";
                    break;
                case 11:
                    model.Col5.IsVisible = true;
                    model.Col4.IsVisible = true;
                    model.Col3.IsVisible = true;
                    model.Col1.Header = "User Plant Name";
                    model.Col2.Header = "User Plant Code";
                    model.Col2.Header = "Sis Plant Name";
                    model.Col2.Header = "Sis Plant Code";
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 12:
                    model.Col5.IsVisible = false;
                    model.Col4.IsVisible = false;
                    model.Col2.IsVisible = false;
                    model.Col1.Header = "Code";
                    model.Col2.Header = "Description";
                    model.Col4.Header = "Status";
                    break;
            }

            return View("_GetMMUOMGroupTypeValuationMatrix", model);
        }
        //[HttpPost]
        //public IActionResult  SubmitMMUOMGroupRequest([FromBody] MMUOMGroupTypeValuationMatrix model)
        //{
        //    //BindPlantDetails();
        //    string _siteDesc = string.Empty;
        //    int _userId = _session.Get<int>("userID"); ;
        //    int _selectMasterEntry = Convert.ToInt32(model.SelectedPlant);
        //    //DataSet oDs = new DataSet();
        //    ////BOUND PLANT
        //    //oDs = cmnMasterQueries.GetAllPlantList();
        //    //ddlPlantMst.DataSource = oDs;
        //    //_siteDesc = oDs.Tables[0].Rows[0]["PLANTNAME"].ToString();

        //    string _selectSite =model.SelectedPlantSL;
        //    string s = model.SelectedPlantSL;
        //    int _materialId;
        //    string _materialCode, _message, output;


        //    _siteDesc = model.SelectedPlantSL;
        //    if (_siteDesc != "")
        //    {
        //        _siteDesc = model.SelectedPlantSL;
        //    }
        //    else
        //    {
        //        _siteDesc =model.SelectedPlant;
        //    }
        //    if (_session.Get<string>("materialId") != null && _session.Get<string>("materialId") != "")
        //    {
        //        _materialId = Convert.ToInt32(_session.Get<string>("materialId"));
        //    }
        //    if ("Update" == "Update")
        //    {
        //        #region "Updated Master Data"
        //        if (_selectMasterEntry != 0)
        //        {
        //            switch (_selectMasterEntry)
        //            {
        //                case 1:
        //                    if (!string.IsNullOrEmpty(model.UOM) && !string.IsNullOrEmpty(model.UOMDesc))
        //                    { 

        //                        _materialCode = "0";
        //                        output = oMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtUOM.Text.Trim(), txtUOMDesc.Text.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = "Unit of Measurement request is updated successfully.";
        //                    }

        //                    break;
        //                case 2:

        //                    if (!string.IsNullOrEmpty(model.UOM.Trim()) && !string.IsNullOrEmpty(txtMaterialGroupDesc.Text.Trim()))
        //                    {

        //                         _materialCode = "0";
        //                        output = oMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtMaterialGroup.Text.Trim(), txtMaterialGroupDesc.Text.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = "Material Group request is updated successfully.";
        //                    }
        //                    break;
        //                case 3:

        //                    //if (!string.IsNullOrEmpty(txtMaterialType.Text.Trim()) && !string.IsNullOrEmpty(txtMaterialTypeDesc.Text.Trim()))
        //                    //{
        //                    //    cmnMasterQueries = new cMasterQueries();
        //                    //    _materialCode = "0";
        //                    //    output = cmnMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtMaterialType.Text.Trim(), txtMaterialTypeDesc.Text.Trim(), _materialCode, "", "", _userId);
        //                    //    if (output == "1")
        //                    //        _message = "Material Type request is updated successfully.";
        //                    //}

        //                    /////////added by  Ajit TTL ////////////////////////////
        //                    if (!string.IsNullOrEmpty(txtMaterialType.Text.Trim()) && !string.IsNullOrEmpty(txtMaterialTypeDesc.Text.Trim()))
        //                    {
        //                        String sales = "0", purchase = "0";
        //                        if (chPurchase.Checked) purchase = "1";
        //                        if (chSales.Checked) sales = "1";

        //                         _materialCode = "0";
        //                        output = cmnMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtMaterialType.Text.Trim(), txtMaterialTypeDesc.Text.Trim(), _materialCode, purchase, sales, _userId);
        //                        if (output == "1")
        //                            _message = "Material Type request is updated successfully.";
        //                    }
        //                    break;

        //                case 4:
        //                    if (!string.IsNullOrEmpty(txtValuation.Text.Trim()) && !string.IsNullOrEmpty(txtValuationDesc.Text.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        output = cmnMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtValuation.Text.Trim(), txtValuationDesc.Text.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = "Valuation Class request is updated successfully.";
        //                    }
        //                    break;
        //                case 5:
        //                    if (!string.IsNullOrEmpty(_selectSite) && !string.IsNullOrEmpty(txtPlantName.Text.Trim()) && !string.IsNullOrEmpty(txtPlantCode.Text.Trim()) && !string.IsNullOrEmpty(txtProfitCenter.Text.Trim()))
        //                    {

        //                        output = oMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtPlantName.Text.Trim(), txtPlantCode.Text.Trim(), txtProfitCenter.Text.Trim(), _selectSite, _siteDesc, _userId);
        //                        if (output == "1")
        //                            _message = "Plant master request is updated successfully.";
        //                    }
        //                    break;
        //                case 11:
        //                    if (!string.IsNullOrEmpty(txtCodeSales.Text.Trim()) && !string.IsNullOrEmpty(txtDescriptionSales.Text.Trim()) && !string.IsNullOrEmpty(ddlPlantSL.SelectedValue.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        output = cmnMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtCodeSales.Text.Trim(), txtDescriptionSales.Text.Trim(), ddlPlantSL.SelectedValue, "", "", _userId);
        //                        if (output == "1")
        //                            _message = ddlMasterType.SelectedItem.ToString().ToUpperInvariant() + " request is updated successfully.";
        //                    }
        //                    break;
        //                case 6:
        //                case 7:
        //                case 8:
        //                case 9:
        //                case 10:
        //                case 12:
        //                    if (!string.IsNullOrEmpty(txtCodeSales.Text.Trim()) && !string.IsNullOrEmpty(txtDescriptionSales.Text.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        output = cmnMasterQueries.EditMaterialMasterData(_selectMasterEntry, _materialId, txtCodeSales.Text.Trim(), txtDescriptionSales.Text.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = ddlMasterType.SelectedItem.ToString().ToUpperInvariant() + " request is updated successfully.";
        //                    }
        //                    break;
        //            }
        //        }
        //        #endregion

        //    }
        //    else
        //    {
        //        #region "Added Master Data"
        //        if (_selectMasterEntry != 0)
        //        {
        //            switch (_selectMasterEntry)
        //            {
        //                case 1:
        //                    if (!string.IsNullOrEmpty(model.UOM.Trim()) && !string.IsNullOrEmpty(model.UOMDesc.Trim()))
        //                    {

        //                        _materialCode = "0";
        //                        output = oMasterQueries.AddMaterialMasterData(_selectMasterEntry, model.UOM.Trim(), model.UOMDesc.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = "Unit of Measurement request is added successfully.";
        //                    }

        //                    break;
        //                case 2:
        //                    if (!string.IsNullOrEmpty(txtMaterialGroup.Text.Trim()) && !string.IsNullOrEmpty(txtMaterialGroupDesc.Text.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        output = cmnMasterQueries.AddMaterialMasterData(_selectMasterEntry, txtMaterialGroup.Text.Trim(), txtMaterialGroupDesc.Text.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = "Material Group request is added successfully.";
        //                    }
        //                    break;
        //                case 3:

        //                    if (!string.IsNullOrEmpty(txtMaterialType.Text.Trim()) && !string.IsNullOrEmpty(txtMaterialTypeDesc.Text.Trim()))
        //                    {
        //                        //////Added by Ajit ///////////////////////
        //                        String sales = "0", purchase = "0";
        //                        if (chPurchase.Checked) purchase = "1";
        //                        if (chSales.Checked) sales = "1";
        //                        ///////////////////////////////////////////////

        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        //output = cmnMasterQueries.AddMaterialMasterData(_selectMasterEntry, txtMaterialType.Text.Trim(), txtMaterialTypeDesc.Text.Trim(), _materialCode,"","", _userId);

        //                        ////////////////////////Added by Ajit////////////////
        //                        output = cmnMasterQueries.AddMaterialMasterData(_selectMasterEntry, txtMaterialType.Text.Trim(), txtMaterialTypeDesc.Text.Trim(), _materialCode, purchase, sales, _userId);

        //                        if (output == "1")
        //                            _message = "Material Type request is added successfully.";
        //                    }
        //                    break;
        //                case 4:
        //                    if (!string.IsNullOrEmpty(txtValuation.Text.Trim()) && !string.IsNullOrEmpty(txtValuationDesc.Text.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        output = cmnMasterQueries.AddMaterialMasterData(_selectMasterEntry, txtValuation.Text.Trim(), txtValuationDesc.Text.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = "Valuation Class request is added successfully.";
        //                    }
        //                    break;
        //                case 5:
        //                    if (!string.IsNullOrEmpty(_selectSite) && !string.IsNullOrEmpty(txtPlantName.Text.Trim()) && !string.IsNullOrEmpty(txtPlantCode.Text.Trim()) && !string.IsNullOrEmpty(txtProfitCenter.Text.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        output = cmnMasterQueries.AddMaterialMasterData(_selectMasterEntry, txtPlantName.Text.Trim(), txtPlantCode.Text.Trim(), txtProfitCenter.Text.Trim(), _selectSite, _siteDesc, _userId);
        //                        if (output == "1")
        //                            _message = "Plant master request is added successfully.";
        //                    }
        //                    break;
        //                case 11:
        //                    if (!string.IsNullOrEmpty(txtCodeSales.Text.Trim()) && !string.IsNullOrEmpty(txtDescriptionSales.Text.Trim()) && !string.IsNullOrEmpty(ddlPlantSL.SelectedValue.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        output = cmnMasterQueries.AddMaterialMasterData(_selectMasterEntry, txtCodeSales.Text.Trim(), txtDescriptionSales.Text.Trim(), ddlPlantSL.SelectedValue, "", "", _userId);
        //                        if (output == "1")
        //                            _message = ddlMasterType.SelectedItem.ToString().ToUpperInvariant() + " request is added successfully.";
        //                    }
        //                    break;
        //                case 6:
        //                case 7:
        //                case 8:
        //                case 9:
        //                case 10:
        //                case 12:
        //                    if (!string.IsNullOrEmpty(txtCodeSales.Text.Trim()) && !string.IsNullOrEmpty(txtDescriptionSales.Text.Trim()))
        //                    {
        //                        cmnMasterQueries = new cMasterQueries();
        //                        _materialCode = "0";
        //                        output = cmnMasterQueries.AddMaterialMasterData(_selectMasterEntry, txtCodeSales.Text.Trim(), txtDescriptionSales.Text.Trim(), _materialCode, "", "", _userId);
        //                        if (output == "1")
        //                            _message = ddlMasterType.SelectedItem.ToString().ToUpperInvariant() + " request is added successfully.";
        //                    }
        //                    break;
        //            }
        //        }
        //        #endregion

        //    }

        //    if (output != "1")
        //    {
        //        ShowError(output);
        //    }
        //    else
        //    {
        //        ShowInfo(_message);
        //    }
        //    GridViewBindDetails();
        //    Reset();

        //}

        /************************************************************************************************/
        [HttpPost]
        public IActionResult MMApprovalDetailsUpdateForm(string ApproveFormApprovalUpdate)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if(ApproveFormApprovalUpdate==null || ApproveFormApprovalUpdate == "")
            {
                return RedirectToAction("MMStatusRequestReport", "MasterMgmt");
            }
          
            string  MMDetailId = ApproveFormApprovalUpdate;//(Request.QueryString["Id"].ToString());
               //mm_ID = MMDetailId.Substring(0, 1);  
            var model = BindMMApprovalUserUpdateDetails(MMDetailId);

            return View(model);
        }
        public MaterialApprovalUserUpdateModel BindMMApprovalUserUpdateDetails(string mm_DetailID)
        {
            MaterialApprovalUserUpdateModel data=new MaterialApprovalUserUpdateModel();
            try
            {  //Added TTL Ajit 23.04.25

                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialApprovalUserDetails(mm_DetailID);
                if (ds.Tables[0].Rows.Count > 0)
                {

                    ViewData["MMHeaderIdApprove"] = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                    data.MMHeaderIdApprove = ds.Tables[0].Rows[0]["MMHEADERID"].ToString();
                    data.MMHeaderDetailId = ds.Tables[0].Rows[0]["MMHEADERDETAILID"].ToString();
                    if (ds.Tables[0].Rows[0]["STATUS"].ToString() == "Pending at FIN")
                    {
                       data.PPCApprovingAuthority = ds.Tables[0].Rows[0]["PPCAPPROVINGAUTHORITY"].ToString();
                       data.EnablePPCUser = false;
                       data.FinanceApprovingAuthority = ds.Tables[0].Rows[0]["FINANCEAPPROVINGAUTHORITY"].ToString();
                       data.EnableFinanceUser = true;
                    }
                    else
                    {
                        if (ds.Tables[0].Rows[0]["STATUS"].ToString() == "Pending at PPC")
                        {
                            data.PPCApprovingAuthority = ds.Tables[0].Rows[0]["PPCAPPROVINGAUTHORITY"].ToString();
                            data.EnablePPCUser = true;
                            data.FinanceApprovingAuthority = ds.Tables[0].Rows[0]["FINANCEAPPROVINGAUTHORITY"].ToString();
                            data.EnableFinanceUser = false;
                        }
                        else
                        {
                            data.FinanceApprovingAuthority = ds.Tables[0].Rows[0]["FINANCEAPPROVINGAUTHORITY"].ToString();
                            data.EnableFinanceUser = true;
                        }
                    }

                    //ddlFIN_User.SelectedValue = ds.Tables[0].Rows[0]["FINANCEAPPROVINGAUTHORITY"].ToString();
                    data.RequesterId= ds.Tables[0].Rows[0]["REQUESTERID"].ToString();
                    data.RequestorName = ds.Tables[0].Rows[0]["REQUESTORNAME"].ToString();
                    data.RequestDate = ds.Tables[0].Rows[0]["REQUESTDATE"].ToString();
                    data.Status = ds.Tables[0].Rows[0]["STATUS"].ToString();

                    if (ds.Tables[0].Rows[0]["PPCAPPROVINGAUTHORITY"].ToString() != "")
                    {
                        //ddlPPC_User.DataSource = ds.Tables[1];
                        //ddlPPC_User.DataTextField = "PPCUSERLIST";
                        //ddlPPC_User.DataValueField = "ADEMPCODE";
                        //ddlPPC_User.DataBind();

                        data.PPCUsers = ds.Tables[1].Rows
                                           .Cast<DataRow>()
                                           .Select(r => new UserOptionDto
                                           {
                                               Text = r["PPCUSERLIST"]?.ToString() ?? string.Empty,
                                               Value = r["ADEMPCODE"]?.ToString() ?? string.Empty,
                                               Selected = string.Equals(
                                                   r["ADEMPCODE"]?.ToString(),
                                                   data.PPCApprovingAuthority,
                                                   StringComparison.OrdinalIgnoreCase)
                                           })
                                           .ToList();

                    }

                    //ddlFIN_User.DataSource = ds.Tables[2];
                    //ddlFIN_User.DataTextField = "FINUSERLIST";
                    //ddlFIN_User.DataValueField = "ADEMPCODE";
                    //ddlFIN_User.DataBind();

                    data.FinanceUsers = ds.Tables[2].Rows
                                      .Cast<DataRow>()
                                      .Select(r => new UserOptionDto
                                      {
                                          Text = r["FINUSERLIST"]?.ToString() ?? string.Empty,
                                          Value = r["ADEMPCODE"]?.ToString() ?? string.Empty,
                                          Selected = string.Equals(
                                              r["ADEMPCODE"]?.ToString(),
                                              data.FinanceApprovingAuthority,
                                              StringComparison.OrdinalIgnoreCase)
                                      })
                                      .ToList();


                    //if (ds.Tables[1].Rows.Count > 0)
                    //{
                    //    ddlPlantMst.SelectedItem.Text = ds.Tables[1].Rows[0]["PLANT"].ToString();
                    //    ddlPlantMst.SelectedValue = ds.Tables[1].Rows[0]["PLANTCODE"].ToString();
                    //}
                    //gvMMCreationRequest.DataSource = ds.Tables[1];
                    //gvMMCreationRequest.DataBind();
                }
                else
                {
                    ViewData["MMHeaderIdApprove"] = "0";
                    //BOUND PPC User
                    DataSet oDs = new DataSet();
                    oDs = oMasterQueries.GetMasterPlantList();
                    //ddlPPC_User.DataSource = oDs;
                    //ddlPPC_User.DataTextField = "PLANTNAMEDESC";
                    //ddlPPC_User.DataValueField = "PLANTID";
                    //ddlPPC_User.DataBind();
                    //ddlPPC_User.Items.Insert(0, new ListItem("-- select --", "0"));
                    data.PPCUsers = oDs.Tables[0].Rows
                                           .Cast<DataRow>()
                                           .Select(r => new UserOptionDto
                                           {
                                               Text = r["PPCUSERLIST"]?.ToString() ?? string.Empty,
                                               Value = r["ADEMPCODE"]?.ToString() ?? string.Empty,
                                               Selected = string.Equals(
                                                   r["ADEMPCODE"]?.ToString(),
                                                   data.PPCApprovingAuthority,
                                                   StringComparison.OrdinalIgnoreCase)
                                           })
                                           .ToList();


                    //BOUND FIN User
                    DataSet dsFin = new DataSet();
                    dsFin = oMasterQueries.GetMasterPlantList();
                    //ddlFIN_User.DataSource = dsFin;
                    //ddlFIN_User.DataTextField = "PLANTNAMEDESC";
                    //ddlFIN_User.DataValueField = "PLANTID";
                    //ddlFIN_User.DataBind();
                    //ddlFIN_User.Items.Insert(0, new ListItem("-- select --", "0"));
                    data.FinanceUsers = dsFin.Tables[0].Rows
                                      .Cast<DataRow>()
                                      .Select(r => new UserOptionDto
                                      {
                                          Text = r["FINUSERLIST"]?.ToString() ?? string.Empty,
                                          Value = r["ADEMPCODE"]?.ToString() ?? string.Empty,
                                          Selected = string.Equals(
                                              r["ADEMPCODE"]?.ToString(),
                                              data.FinanceApprovingAuthority,
                                              StringComparison.OrdinalIgnoreCase)
                                      })
                                      .ToList();

                }
             
            } //Added TTL Ajit 23.04.25           
            catch (Exception ex)
            {
               
                _logger.LogError(ex, "BindMMApprovalUserUpdateDetails");

            }
            return data;
        }
        [HttpPost]
        public IActionResult MMApprovalDetailsUpdateSubmit([FromBody]MaterialApprovalUserUpdateModel data)
        {
            try
            {
                int _finApprovalAuth = 0, _ppcApprovalAuth = 0;
                string message = "";
                int UserID = _session.Get<int>("userID");
                Int32 MMHeaderId = Convert.ToInt32(data.MMHeaderIdApprove);
                if (data.PPCApprovingAuthority != "")
                {
                    _ppcApprovalAuth = Convert.ToInt32(data.PPCApprovingAuthority);
                }
                else { _ppcApprovalAuth = 01; }
                _finApprovalAuth = Convert.ToInt32(data.FinanceApprovingAuthority??"0");
                if (_finApprovalAuth != 0 || _ppcApprovalAuth != 0)
                {
                    string output = oMasterQueries.SubmitMaterialMasterApprovalUserRequest(MMHeaderId, UserID, _ppcApprovalAuth, _finApprovalAuth);

                    if (output != "1")
                    {
                       return Json(new {Rs=0, Message= output });
                    }
                    else
                    {
                         message="Material master Approval request updated successfully for approval.";
                       // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Material master Approval request updated successfully for approval.')", true);
                    }
                }
                else
                {
                    return Json(new { Rs = 0, Message = "Please check approval authority!" });
                }
                    //Reset();

                    // Response.Redirect("~/ASPXView/MasterMgmt/MMStatusRequestReport.aspx", false);
                    //RedirectToAction("MMStatusRequestReport", "MasterMgmt");
                    //---SR74446 end//
                    return Json(new { Rs = 1, Message = message });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MMApprovalDetailsUpdateSubmit");
                return Json(new { Rs = 0, Message = "Error in Data process!" });
            }
        }
    }
}
