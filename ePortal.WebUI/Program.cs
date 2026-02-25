using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ePortal.Application.Contracts;
using ePortal.Application.Services;
using ePortal.Infrastructure.BackgroundServices;
using ePortal.Infrastructure.DbContexts;
using ePortal.Infrastructure.Repositories;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Admin.Services;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Persistence.SIS.Interface;
using ePortal.Persistence.SIS.Services;
using ePortal.Persistence.TourRequest.Interface;
using ePortal.Persistence.TourRequest.Services;
using ePortal.Persistence.TravelBilling.Interface;
using ePortal.Persistence.TravelBilling.Services;
using ePortal.Persistence.VQMS.Interface;
using ePortal.Shared;
using ePortal.Shared.Configuration;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using ePortal.WebUI.Helpers;
using ePortal.WebUI.Middleware;
using ePortal.WebUI.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Web.Mvc;

//Training
using ePortal.Application.APPX.Contracts;
using ePortal.Application.APPX.Services;
using ePortal.Persistence.VQMS.Interface;
using ePortal.Persistence.SIS.Interface;
using ePortal.Persistence.SIS.Services;
//Training
//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;  //need to remove here

var builder = WebApplication.CreateBuilder(args);



// Configure Serilog
// Read configuration from appsettings.json
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
    //.WriteTo.Console();
    /*.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit:7); */ //we can change "rolling interval"
                                                                                                         //.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// Register DbContext
builder.Services.AddDbContext<EPortalDBContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDbConnection")));

builder.Services.AddDbContext<EPortalDGITDBContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDbConnection")));

builder.Services.AddDbContext<LCModelDBContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDbConnection")));

builder.Services.AddDbContext<ICProcessBDContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDbConnection")));


//builder.Services.AddSingleton(resolver =>
//    resolver.GetRequiredService<IOptions<GeneralSettings>>().Value);

builder.Services.Configure<GeneralSettings>(builder.Configuration.GetSection("GeneralSettings"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
//builder.Services.Configure<DbConnectionOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<IAppConfigurationService, AppConfigurationService>();


// Add Session
// Add session services
builder.Services.AddHttpContextAccessor();

// Register Session Services
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddHostedService<AutoRejectItemsTask>();

//Register application services
builder.Services.AddScoped<IEmpLoginService, EmployeeLoginService>();
builder.Services.AddScoped<IHomePageService, HomePageService>();
builder.Services.AddScoped<IBikerCafeService, BikerCafeService>();
builder.Services.AddScoped<ISeatMgmtService, SeatMgmtService>();
builder.Services.AddScoped<IKaizenService, KaizenService>();
builder.Services.AddScoped<ICorporateNews, CorporateNewsService>();
builder.Services.AddScoped<IHostingEnvironmentService, HostingEnvironmentService>();
//builder.Services.AddScoped<IAppConfigurationService, AppConfigurationService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<ICanteenService, CanteenService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
//builder.Services.AddSingleton<IApiClient, ApiClient>();
builder.Services.AddScoped<IA00Service, A00Service>();
builder.Services.AddScoped<IACRService, ACRService>();
builder.Services.AddScoped<IPRService, PRService>();
builder.Services.AddScoped<ISMService, SMService>();
builder.Services.AddScoped<IAssetRegistrationService, AssetRegistrationService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAT_ApprovalService, AT_ApprovalService>();
builder.Services.AddScoped<IAssetDisposalService, AssetDisposalService>();
builder.Services.AddScoped<IBenevolent, BenevolentService>();
builder.Services.AddScoped<IIOMService, IOMService>();
builder.Services.AddScoped<IMenuMappingMasterService, MenuMappingMasterService>();
builder.Services.AddScoped<IMenuMasterServices, MenuMasterServices>();
builder.Services.AddScoped<IProcessMasterService, ProcessMasterService>();
builder.Services.AddScoped<IRoleMappingMasterServices, RoleMappingMasterServices>();
builder.Services.AddScoped<IRoleMasterServices, RoleMasterServices>();
builder.Services.AddScoped<IRoleUserMappingMasterService, RoleUserMappingMasterServices>();
builder.Services.AddScoped<IBirthdayListService, BirthdayListService>();
builder.Services.AddScoped<IFoundationDayBookingService, FoundationDayBookingService>();
builder.Services.AddScoped<IPORequest, PORequest>();
builder.Services.AddScoped<IISMSMaster, ISMSMasterService>();
builder.Services.AddScoped<ILocalConveyanceServices, LocalConveyanceServices>();
builder.Services.AddScoped<ICreativeMaster, CreativeMasterService>();
builder.Services.AddScoped<IMedicalInsuranceService, MedicalInsuranceService>();
builder.Services.AddScoped<INSPPaymentServices, NSPPaymentServices>();
builder.Services.AddScoped<IScreenSaverService, ScreenSaverService>();
builder.Services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();
builder.Services.AddScoped<IPresidentDesk, PresidentDeskService>();
builder.Services.AddScoped<IPOAPowerScopeService, POAPowerScopeService>();
builder.Services.AddScoped<IOperatingHead, OperatingHeadService>();
builder.Services.AddScoped<IPOService, POService>();
builder.Services.AddScoped<IAutoRemindersNoofDaysMasterService, AutoRemindersNoofDaysMasterService>();
builder.Services.AddScoped<IAutoRemindersExcludeDesignationMasterService, AutoRemindersExcludeDesignationMasterService>();
builder.Services.AddScoped<ILostAndFoundService, LostAndFoundService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddScoped<ITrainingService, TrainingService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IWFHService, WFHService>();
builder.Services.AddScoped<ITexiRequest, TaxiRequest>();
builder.Services.AddScoped<IBusRoute, BusRoute>();
builder.Services.AddScoped<IShiftchg, Shiftchg>();
builder.Services.AddScoped<ISeparation, Separation>();
builder.Services.AddScoped<ISummerTraineeMasterService, SummerTraineeMasterService>();
builder.Services.AddScoped<IIOMContractService, IOMContractService>();
builder.Services.AddScoped<IVendorMasterService, VendorMasterService>();
builder.Services.AddScoped<ICustomerMgmtService, CustomerMgmtService>();
builder.Services.AddScoped<IThirdpartyEmployeeService, ThirdpartyEmployeeService>();
builder.Services.AddScoped<IVQMS_DPRService, VQMS_DPRService>();
builder.Services.AddScoped<IVpfCreate, VPF_CreateService>();
builder.Services.AddScoped<IQMSService, QMSService>();
builder.Services.AddScoped<IStationary, Stationary>();
builder.Services.AddScoped<IAD_ITEM_MASTER, AD_ITEM_MASTER>();
builder.Services.AddScoped<ITourBudget, TourBudget>();
builder.Services.AddScoped<ITourSettlement, TourSettlement>();
builder.Services.AddScoped<ITourQueries, TourQueries>();
builder.Services.AddScoped<IFlightSchedule, FlightSchedule>();
builder.Services.AddScoped<IMasterMgmtRepo, MasterMgmtRepo>();
builder.Services.AddScoped<IPIOMService, PIOMService>();
builder.Services.AddScoped<IContineousWorkingService, ContineousWorkingService>(); // aaded by aumento :: SR113877
builder.Services.AddScoped<ILockerManagementService, LockerManagementService>(); // aaded by aumento :: SR113877
builder.Services.Configure<SapRfcConfig>(builder.Configuration.GetSection("SAP_RFC_CONFIG")); // aaded by aumento :: SR113877


//Register DAL services
builder.Services.AddScoped<IManageSelfPending, ManageSelfPending>();
builder.Services.AddScoped<ICommonFunctions, CommonFunctions>();
builder.Services.AddScoped<IDataManagement, DataManagement>();
builder.Services.AddScoped<IConnectionString, ConnectionString>();
builder.Services.AddScoped<IEmpUserDetails, EmpUserDetails>();
builder.Services.AddScoped<IBikerCafe_DAL, BikerCafe_DAL>();
builder.Services.AddScoped<IEportalESS, EportalESS>();
builder.Services.AddScoped<ISapDataProvider, SapDataProvider>();
builder.Services.AddScoped<ISearchEmp, SearchEmp>();
builder.Services.AddScoped<IHumanResource, HumanResource>();
builder.Services.AddScoped<IICIBMService, ICIBMService>();
builder.Services.AddScoped<IcTravelBilling, cTravelBilling>();
builder.Services.AddScoped<IcCostCenterMaster, cCostCenterMaster>();
builder.Services.AddScoped<IcHelpDataProvider, cHelpDataProvider>();

builder.Services.AddScoped<IChroniclesService, ChroniclesService>();
builder.Services.AddScoped<ChroniclesRepository>();
builder.Services.AddScoped<ILogin, Login>();
builder.Services.AddScoped<IPassword, Password>();
//Training Module - Start
builder.Services.AddScoped<ITrainingCalendar, TrainingCalendar>();
builder.Services.AddScoped<IHrIntrnlTraining, HrIntrnlTraining>();
builder.Services.AddScoped<ITraining, Training>();
builder.Services.AddScoped<IDivision, Division>();
builder.Services.AddScoped<IDepartment, Department>();
builder.Services.AddScoped<ISection, Section>();
builder.Services.AddScoped<IVicePresident, VicePresident>();
//Training Module - End
builder.Services.AddScoped<ISurveyConfig, SurveyConfig>();
builder.Services.AddScoped<IInformationSecurity, InformationSecurity>();
//builder.Services.AddScoped<ITraining, Training>();
//builder.Services.AddScoped<IDepartment, Department>();
builder.Services.AddScoped<IPMS, PMS>();
//builder.Services.AddScoped<IDivision, Division>();
builder.Services.AddScoped<IHRConfirmationReview, HRConfirmationReview>();
builder.Services.AddScoped<IIOMContractRepos, IOMContractRepos>();
builder.Services.AddScoped<IVendorMaster, VendorMaster>();
builder.Services.AddScoped<ICustomerMgmtRepo, CustomerMgmtRepo>();
builder.Services.AddScoped<IDPR, DPR>();
builder.Services.AddScoped<IUtilityDesk, UtilityDesk>();
builder.Services.AddScoped<IPMS_DAL, PMS_DAL>();
builder.Services.AddScoped<ILeaveApps_DAL, LeaveApps_DAL>();
builder.Services.AddScoped<IEmpIDDetail, EmpIDDetail>();
builder.Services.AddScoped<IEssServices, EssServices>();
builder.Services.AddScoped<IExcelExport, ExportExcel>();
builder.Services.AddScoped<IUARService, UARService>();
builder.Services.AddScoped<INavigationMaster_DAL, NavigationMaster_DAL>();
builder.Services.AddScoped<ITaxationServices, TaxationServices>();
builder.Services.AddScoped<IISMS, ISMS>();
builder.Services.AddScoped<ILeaveApps, LeaveApps>();
builder.Services.AddScoped<IHCG, HCG>();
builder.Services.AddScoped<IQMSRepos, QMSRepos>();
builder.Services.AddScoped<IBusRouteService, BusRouteService>();
builder.Services.AddScoped<IHRPMS, HRPMS>();
builder.Services.AddScoped<ITaxDeclaration, TaxDeclaration>();
builder.Services.AddScoped<ISafety, Safety>();

builder.Services.AddScoped<IMasterMgmtServices, MasterMgmtServices>();
builder.Services.AddScoped<ICalenderMaster, CalenderMasterService>();

builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleEmailNotificationService, VehicleEmailNotificationService>();

// Register Repository services
builder.Services.AddScoped<EmployeeLoginRepository>();
builder.Services.AddScoped<CommonRepository>();
builder.Services.AddScoped<SeatMgmtRepository>();
builder.Services.AddScoped<KaizenRepository>();
builder.Services.AddScoped<BikerCafeRepository>();
builder.Services.AddScoped<HomeRepository>();
builder.Services.AddScoped<CorporateNewsRepository>();
builder.Services.AddScoped<AnnouncementRepository>();
builder.Services.AddScoped<CanteenRepository>();
builder.Services.AddScoped<A00Repository>();
builder.Services.AddScoped<ACRRepository>();
builder.Services.AddScoped<PRRepository>();
builder.Services.AddScoped<SMRepository>();
builder.Services.AddScoped<AssetRegistrationRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<AssetTransferRepository>();
builder.Services.AddScoped<AssetDisposalRepository>();
builder.Services.AddScoped<BenevolentRepository>();
builder.Services.AddScoped<IOMRepository>();
builder.Services.AddScoped<MenuMappingMasterRepository>();
builder.Services.AddScoped<MenuMasterRepository>();
builder.Services.AddScoped<ProcessMasterRepository>();
builder.Services.AddScoped<RoleMappingMasterRepository>();
builder.Services.AddScoped<RoleMasterRepository>();
builder.Services.AddScoped<RoleUserMappingMasterRepository>();
builder.Services.AddScoped<BirthdayListRepository>();
builder.Services.AddScoped<FoundationDayBookingRepository>();
builder.Services.AddScoped<ICIBMRepository>();
builder.Services.AddScoped<ISMSMasterRepository>();
builder.Services.AddScoped<LocalConveyanceRepository>();
builder.Services.AddScoped<CreativeMasterRepository>();
builder.Services.AddScoped<MedicalInsuranceRepository>();
builder.Services.AddScoped<NSPPaymentRepository>();
builder.Services.AddScoped<ScreenSaverRepository>();
builder.Services.AddScoped<RiskAssessmentRepository>();
builder.Services.AddScoped<PresedentDeskRepository>();
builder.Services.AddScoped<POAPowerScopeRepository>();
builder.Services.AddScoped<OperatingHeadRepository>();
builder.Services.AddScoped<PORepository>();
builder.Services.AddScoped<AutoRemindersNoofDaysMasterRepository>();
builder.Services.AddScoped<AutoRemindersExcludeDesignationMasterRepository>();
builder.Services.AddScoped<LostAndFoundRepository>();
builder.Services.AddScoped<SurveyRepository>();
builder.Services.AddScoped<WFHRepository>();
builder.Services.AddScoped<SummerTraineeMasterRepository>();
builder.Services.AddScoped<UARRepository>();
builder.Services.AddScoped<ThirdpartyEmployeeRepository>();
builder.Services.AddScoped<VpfCreateRepositories>();
builder.Services.AddScoped<CalenderMasterRepository>();
builder.Services.AddScoped<PIOMRepository>();
builder.Services.AddScoped<ContineousWorkingRepository>(); // added by aumento :: SR113877
builder.Services.AddScoped<LockerManagementRepository>();


builder.Services.AddScoped<VehicleRepository>();

//Register Filter
builder.Services.AddScoped<CSPAttribute>();
builder.Services.AddScoped<CSPOptions>();

//Token Service Registeres
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

//builder.Services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
//{
//    var config = serviceProvider.GetRequiredService<IConfiguration>();
//    var apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>();

//    // Set base address
//    client.BaseAddress = new Uri(apiSettings.BaseUrl);

//    client.DefaultRequestHeaders.Accept.Clear();
//    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//});

builder.Services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>();

    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

//FCM Notification Service || CR7770 @ START
builder.Services.AddHttpClient<IFCMNotificationContract, FCMNotificationService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var fcmSettings = config.GetSection("FCMSettings").Get<FCMSettings>();

    client.BaseAddress = new Uri(fcmSettings.FCMNotificationUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).ConfigurePrimaryHttpMessageHandler(() => { return new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }; });
//FCM Notification Service || CR7770 @END

var appConfigService = builder.Services.BuildServiceProvider().GetRequiredService<IAppConfigurationService>();
serverpath.Initialize(appConfigService);


//Added By TTL against CR6695 as on 29 - 07 - 2025
var dataProtectionSettings = builder.Configuration.GetSection("DataProtection");
var keyPath = dataProtectionSettings.GetValue<string>("KeyPath");
var appName = dataProtectionSettings.GetValue<string>("AppName");

//builder.Services.AddDataProtection()
//    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
//    .SetApplicationName(appName);
//Added By TTL against CR6695 as on 29 - 07 - 2025

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{

    var idleTimeOut = builder.Configuration.GetValue<int>("GeneralSettings:IdleTimeoutMinutes");
    options.IdleTimeout = TimeSpan.FromMinutes(idleTimeOut);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// MVC
builder.Services.AddControllersWithViews(options =>
{
    //Register filter attributes
    options.Filters.Add<SessionTimeoutAttribute>();
}).AddSessionStateTempDataProvider()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; //This keeps PascalCase
        options.JsonSerializerOptions.DictionaryKeyPolicy = null; // Disable camelCase for dictionary keys
    })
    .AddRazorRuntimeCompilation(); //for development only

// Enable Cross-Origin Resource Sharing (CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); // Allow all origins, methods, and headers
    });
});


var app = builder.Build();

//if(!app.Environment.IsDevelopment())
//{
//    // Global exception handler
//    app.UseExceptionHandler("/Home/Error");

//    //status code pages
//    app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");
//}

//app.UseHttpsRedirection();



// Example Middleware logging
app.UseSerilogRequestLogging(); // Logs HTTP request information

// Use Middleware
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
        RequestPath = "/Uploads"
    });
}



app.UseRouting();

app.UseSession(); // Session middleware: Enable session after routing
app.UseMiddleware<SessionRestoreMiddleware>(); //Added By TTL against CR6695 as on 29 - 07 - 2025

//app.UseAuthentication();
app.UseMiddleware<AcquireRequestStateMiddleware>();

app.UseAuthorization();

if (!app.Environment.IsDevelopment())
    app.UseMiddleware<GlobalExceptionMiddleware>(); //commentin for dev


app.UseCookiePolicy(); //Added By TTL against CR6695 as on 29 - 07 - 2025

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

