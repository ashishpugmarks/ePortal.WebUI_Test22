using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.Shared;
using System.Data;
using System.Reflection;
using System.Xml.Xsl;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ePortal.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBaseController : Controller
    {
        //DataManagement oDataMgmt = new DataManagement();

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        protected string strServerPath;
        protected readonly ISessionService _sessionService;
        protected readonly ILogger<AdminBaseController> _logger;
        private readonly IWebHostEnvironment _env;

        protected readonly string _userId;

        public AdminBaseController(IDataManagement _oDataMgmt, IConnectionString _objCnStr, ILogger<AdminBaseController> logger, IConfiguration settings, ISessionService sessionService, IWebHostEnvironment env)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
            strServerPath = serverpath.getServerPath();
            _sessionService = sessionService;
            _logger = logger;

            _userId = _sessionService.Get<string>("userID").ToString();
            _env = env;
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //LoadMenu(Session["userID"].ToString());
            LoadMenu(_userId);
        }

        protected void LoadMenu(string strUserCode)
        {
            try
            {
                DataSet ds = new DataSet();
                //ConnectionString objCnStr = new ConnectionString();
                string connStr = objCnStr.getConnectingString();

                //if (Session["MENU_DATASET"] == null)
                if (_sessionService.Get<DataSet>("MENU_DATASET") == null)
                {
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.CommandText = "PKG_NAVIGATION.SPROC_MENULISTBYEMPCODE_GET";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strUserCode;
                    oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    ds = oDataMgmt.GetDataSet(oCmd);

                    var data = from row in ds.Tables[0].AsEnumerable()
                               select new
                               {
                                   MENUID = row.Field<string>("MenuID"),
                                   Description = row.Field<string>("Description"),
                                   ParentID = row.Field<string>("ParentID"),
                                   URL = strServerPath + row.Field<string>("URL"),
                                   ADEMPCODE = row.Field<string>("ADEMPCODE"),
                                   DisplayOrder = row.Field<string>("DISPLAYORDER")
                               };

                    DataTable dt = new DataTable();

                    dt = LINQToDataTable(data);

                    ds = new DataSet();
                    ds.Tables.Add(dt);

                    ds.DataSetName = "Menus";
                    ds.Tables[0].TableName = "Menu";
                    DataRelation relation = new DataRelation("ParentChild",
                            ds.Tables["Menu"].Columns["MenuID"],
                            ds.Tables["Menu"].Columns["ParentID"],
                            false);

                    relation.Nested = true;
                    ds.Relations.Add(relation);

                    //Session["MENU_DATASET"] = ds;
                    var json = JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.Indented);
                    _sessionService.Set("MENU_DATASET", json);

                }
                else
                {
                    //ds = Session["MENU_DATASET"] as DataSet;
                    var JsonString = _sessionService.Get<string>("MENU_DATASET");
                    ds = JsonConvert.DeserializeObject<DataSet>(JsonString);
                    ds.DataSetName = "Menus"; // Restore the original name
                    DataRelation relation = new DataRelation("ParentChild",
                            ds.Tables["Menu"].Columns["MenuID"],
                            ds.Tables["Menu"].Columns["ParentID"],
                            false);

                    relation.Nested = true;
                    ds.Relations.Add(relation);

                }

                String strXML = ds.GetXml();
                System.Text.StringBuilder str = new System.Text.StringBuilder();

                XmlDocument xAdminDoc = new XmlDocument();
                xAdminDoc.LoadXml(strXML);

                System.Xml.XPath.XPathNavigator xPathNav = xAdminDoc.CreateNavigator();

                XslCompiledTransform xAdminslt = new XslCompiledTransform();
                //xAdminslt.Load(Server.MapPath("~/Includes/Xslt/AdminMenuXSLT.xsl"));
                xAdminslt.Load(Path.Combine(_env.WebRootPath, "Includes", "Xslt", "AdminMenuXSLT.xsl"));                

                XmlWriter result = XmlWriter.Create(str, xAdminslt.OutputSettings);
                xAdminslt.Transform(xPathNav, result);

                ViewBag.MenuHtml = str.ToString();
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                //throw ex;
            }
        }

        private DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {

            DataTable dtReturn = new DataTable();

            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
    }
}
