using ePortal.Application.APPX.Contracts;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.CustomerMgmt;
using ePortal.ViewModels.APPX.MasterMgmt;
using Microsoft.Extensions.Logging;
using Spire.Additions.Xps.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.APPX.Services
{
    public class MasterMgmtServices : IMasterMgmtServices
    {
        private IMasterMgmtRepo oMasterQueries;
        private readonly IAppConfigurationService _env;
        private ISessionService _session;
        ILogger<CustomerMgmtService> _logger;
        private readonly IEportalESS objess;
        public MasterMgmtServices(IMasterMgmtRepo masterMgmt, IAppConfigurationService appConfiguration, ISessionService sessionService, IEportalESS eportalESS, ILogger<CustomerMgmtService> logger)
        {
            oMasterQueries = masterMgmt;
            _env = appConfiguration;
            _session = sessionService;
            _logger = logger;
            objess = eportalESS;

        }
        public DataSet BindUserDetail()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetPPCApprovalRequiredOperationId();

                return ds;
            }
            catch (Exception ex)
            {
                _logger.LogError("BindUserDetail", ex.Message);
                return new DataSet();
            }

        }
        public DataSet PopulateValuationClassByMaterialType(int matrialType, ref string isSales)
        {
            try
            {
                DataSet ds = new DataSet();
                if (matrialType == 0)
                {
                    isSales = "N";
                    return ds;
                }
                else
                {
                    DataSet oDs = new DataSet();
                    ds = oMasterQueries.GetMaterialGroupByMaterialType(matrialType);

                    /////////////////Ajit TTL/////////////
                    ////////Get IsSales data //////
                    DataSet oDs1 = new DataSet();
                    oDs1 = oMasterQueries.GetISPURCHASISSALEById(matrialType);

                    if (oDs1.Tables[0].Rows[0]["ISSALES"].ToString() == "1")
                    {
                        isSales = "Y";

                    }
                    else
                    {
                        isSales = "N";
                    }
                    return ds;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "PopulateValuationClassByMaterialType");
                return new DataSet();
            }

        }
        public DataSet BindMMCreationDetails(int userid)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialCreationPendingDetails(userid);
                return ds;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "BindMMCreationDetails");
                return new DataSet();
            }

        }
        public DataSet BindMMCreationRejectDetails(int userid)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetMaterialCreationRejectedDetails(userid);
                return ds;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "BindMMCreationRejectDetails");
                return new DataSet();
            }
        }
        public MasterResponse GetProfitCentreAndStorageLoc(string plantid)
        {
            MasterResponse res = new MasterResponse { Rs = 0, Message = "" };
            try
            {
                if (Convert.ToInt32(plantid) > 0)
                {
                    DataSet oDs = oMasterQueries.GetPlantDetails(plantid);
                    res.ProfitCentre = oDs.Tables[0].Rows[0]["PROFIT_CENTER"].ToString();
                    //Bind Storage location
                    //string plantid = ddlPlantMst.SelectedValue.ToString();
                    DataSet ds = oMasterQueries.GetMasterData("STORAGE_LOCATION", plantid);
                    var dataList = ds.Tables[0]?.AsEnumerable()
                      .Select(r => new MasterData
                      {
                          Code = r.Field<string>("CODE"),
                          Description = r.Field<string>("CODE_DESC"),

                      })
                      .ToList() ?? new List<MasterData>();

                    res.Rs = 1;
                    res.Message = "Success";
                    res.list = dataList;
                    return res;


                }
                else
                {
                    res.ProfitCentre = "";
                }
                res.Message = "Success";
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetProfitCentreAndStorageLoc");
                res.Message = "Error data binding";
                res.Rs = 0;
                return res;
            }
        }
        public MasterResponse PopulateValuationClassByMaterialType(int matrialType)
        {
            {
                MasterResponse res = new MasterResponse { Rs = 0, Message = "" };
                try
                {
                    if (matrialType > 0)
                    {
                        DataSet oDs = new DataSet();
                        oDs = oMasterQueries.GetMaterialGroupByMaterialType(matrialType);
                        //ddlMaterialGroup.DataSource = oDs;
                        //ddlMaterialGroup.DataTextField = "MATGDESCRIPTION";
                        //ddlMaterialGroup.DataValueField = "MATERIALGROUPID";
                        //ddlMaterialGroup.DataBind();
                        //ddlMaterialGroup.Items.Insert(0, new ListItem("-- select --", "0"));
                        var dataList = oDs.Tables[0]?.AsEnumerable()
                        .Select(r => new MasterData
                        {
                            Id = Convert.ToInt64(r.Field<decimal>("MATERIALGROUPID")),
                            Description = r.Field<string>("MATGDESCRIPTION"),

                        })
                        .ToList() ?? new List<MasterData>();
                        res.list = dataList;
                        /////////////////Ajit TTL/////////////
                        ////////Get IsSales data //////
                        DataSet oDs1 = new DataSet();
                        oDs1 = oMasterQueries.GetISPURCHASISSALEById(matrialType);

                        if (oDs1.Tables[0].Rows[0]["ISSALES"].ToString() == "1")
                        {
                            res.ISSALES = "1";

                        }
                        else
                        {
                            res.ISSALES = "0";
                        }

                        res.Rs = 1;
                        res.Message = "Success";
                        return res;


                    }
                    else
                    {
                        res.Rs = 0;
                        res.Message = "Invalid Material Type";
                        res.ISSALES = "";
                    }

                    return res;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, "PopulateValuationClassByMaterialType");
                    res.Message = "Error data binding";
                    res.Rs = 0;
                    return res;
                }
            }
        }
        public MasterResponse PopulateValuationClassByMaterialTypeGroup(int matrialType, int MaterialGroup)
        {
            {
                MasterResponse res = new MasterResponse { Rs = 0, Message = "" };
                try
                {
                    if (matrialType > 0)
                    {
                        
                        DataSet oDs = new DataSet();
                        oDs = oMasterQueries.GetValuationClassByMaterialTypeGroup(matrialType, MaterialGroup);
                        //ddlValuationClass.DataSource = oDs;
                        //ddlValuationClass.DataTextField = "VALDESCRIPTION";
                        //ddlValuationClass.DataValueField = "VALUATIONCLASSID";
                        //ddlValuationClass.DataBind();
                        //ddlValuationClass.Items.Insert(0, new ListItem("-- select --", "0"));
                        var dataList = oDs.Tables[0]?.AsEnumerable()
                        .Select(r => new MasterData
                        {
                            Id = Convert.ToInt64(r.Field<decimal>("VALUATIONCLASSID")),
                            Description = r.Field<string>("VALDESCRIPTION"),

                        })
                        .ToList() ?? new List<MasterData>();
                        res.list = dataList;
                       

                        res.Rs = 1;
                        res.Message = "Success";
                        return res;


                    }
                    else
                    {
                        res.Rs = 0;
                        res.Message = "Invalid Material Type";
                        res.ISSALES = "";
                    }

                    return res;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, "PopulateValuationClassByMaterialType");
                    res.Message = "Error data binding";
                    res.Rs = 0;
                    return res;
                }
            }
        }
        public string ExcelExport(DataTable dt)
        {
            string res = "";
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (dt.Rows.Count > 0)
                {
                    stringBuilder.Append("<table cellpadding='1' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                    stringBuilder.Append("<tr style='background-color: lightgray;'>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>S.No.</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Header No</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Detail No</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>User</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Status</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Material Code</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Material Description</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Material Specification</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Material Type</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Material Group</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Valuation Class</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Plant</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Date</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Price</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Uom</th>");                    
                    stringBuilder.Append("</tr>");
                    int srNo = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        stringBuilder.Append("<tr>");
                        stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border:1px solid;'>" + row["MMHEADERDETAILID"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border: 1px solid;'>" + row["MMDETAILHEADERID"].ToString() + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row["REQUESTERID"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border:1px solid;'>" + row["STATUS"].ToString() + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row["MATERIALCODE"].ToString() + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row["MATERIALDISCRIPTION"].ToString() + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row["MATERIALSPECIFICATION"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border:1px solid;'>" + row["MATERIALTYPE"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border:1px solid;'>" + row["MATERIALGROUP"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border:1px solid;'>" + row["VALUATIONCLASS"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border:1px solid;'>" + row["PLANTNAME"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:left;border:1px solid;'>" + row["REQUESTDATE"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:right;border:1px solid;'>" + row["PRICE"].ToString() + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row["UOM"].ToString() + "</td>");
                        stringBuilder.Append("</tr>");
                    }
                    stringBuilder.Append("</table>");
                    res = stringBuilder.ToString();
                }

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("btnExport", ex.Message);
                return "";
            }
        }
    }
}
