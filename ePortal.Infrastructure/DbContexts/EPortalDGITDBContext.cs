using System.Data;
using ePortal.DomainClasses;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Infrastructure.DbContexts
{
    public class EPortalDGITDBContext: DbContext
    {
        public EPortalDGITDBContext(DbContextOptions<EPortalDGITDBContext> options) : base(options) { }

        public virtual DbSet<ADEMPLOYEE_DGIT> ADEMPLOYEE { get; set; }
        public virtual DbSet<ADORGLEVEL_DGIT> ADORGLEVEL { get; set; }
        public virtual DbSet<ADORGLEVELHEAD_DGIT> ADORGLEVELHEAD { get; set; }
        //public virtual DbSet<ADORGLEVELTYPE_DGIT> ADORGLEVELTYPE { get; set; }
        public virtual DbSet<ASRCALOPMAPPING_DGIT> ASRCALOPMAPPING { get; set; }
        public virtual DbSet<ADDESIGNATION_DGIT> ADDESIGNATION { get; set; }
        public virtual DbSet<ADFUNCTIONALDESIGNATION_DGIT> ADFUNCTIONALDESIGNATION { get; set; }
        public virtual DbSet<HMSIHOLIDAYS_DGIT> HMSIHOLIDAYS { get; set; }
        public virtual DbSet<SM_DIVISIONWISESEATDETAILS> SM_DIVISIONWISESEATDETAILS { get; set; }
        public virtual DbSet<SM_FLOOR_OPMAP> SM_FLOOR_OPMAP { get; set; }
        public virtual DbSet<SM_FLOORSEATMST> SM_FLOORSEATMST { get; set; }
        public virtual DbSet<SM_ROSTER_TRN> SM_ROSTER_TRN { get; set; }

        //Start SR102091
        public virtual DbSet<SM_EMP_ACTIVEINACTIVEFORROSTER> SM_EMP_ACTIVEINACTIVEFORROSTER { get; set; }
        public virtual DbSet<SM_EMP_ACTIVEINACTIVEFORROSTER_LOG> SM_EMP_ACTIVEINACTIVEFORROSTER_LOG { get; set; }
        public virtual DbSet<SM_WFOMANDATORYDAYS_LOG> SM_WFOMANDATORYDAYS_LOG { get; set; }
        //End SR102091

        public virtual DbSet<SM_SEATALLOCATION_TRN> SM_SEATALLOCATION_TRN { get; set; }
        public virtual DbSet<SM_SETTING_MST> SM_SETTING_MST { get; set; }

        public virtual DbSet<SM_EXTRASEATFORDIV> SM_EXTRASEATFORDIV { get; set; }
        public virtual DbSet<SYSITE_DGIT> SYSITE { get; set; }
        public virtual DbSet<VW_ASSOCIATELVLDETAILS_DGIT> VW_ASSOCIATELVLDETAILS { get; set; }
        public virtual DbSet<SYKI_DGIT> SYKI { get; set; }  //not present in DB

        public virtual DbSet<SM_FIXSEATMAP> SM_FIXSEATMAP { get; set; }
        public virtual DbSet<SM_ALLOWPERCENT> SM_ALLOWPERCENT { get; set; }
        public virtual DbSet<SM_FLOORMASTER> SM_FLOORMASTER { get; set; }
        public virtual DbSet<BC_ADMIN_MEALSBOOKING_TRN_DGIT> BC_ADMIN_MEALSBOOKING_TRN { get; set; }
        public virtual DbSet<BC_MEETINGFOOD_HDR> BC_MEETINGFOOD_HDR { get; set; }
        public virtual DbSet<BC_MEETINGFOOD_MST> BC_MEETINGFOOD_MST { get; set; }
        public virtual DbSet<BC_MEETINGFOOD_TRN> BC_MEETINGFOOD_TRN { get; set; }
        public virtual DbSet<BC_ALACARTEFOOD_HDR_DGIT> BC_ALACARTEFOOD_HDR { get; set; }
        public virtual DbSet<BC_ALACARTEFOOD_MST_DGIT> BC_ALACARTEFOOD_MST { get; set; }
        public virtual DbSet<BC_ALACARTEFOOD_TRN_DGIT> BC_ALACARTEFOOD_TRN { get; set; }
        public virtual DbSet<BC_GUEST_DTL_DGIT> BC_GUEST_DTL { get; set; }
        public virtual DbSet<BC_GUESTMEALBOOKING_DTL_DGIT> BC_GUESTMEALBOOKING_DTL { get; set; }
        public virtual DbSet<BC_GUESTMEALBOOKING_TRN_DGIT> BC_GUESTMEALBOOKING_TRN { get; set; }
        public virtual DbSet<BC_MEALS_AVAILABILITY_DGIT> BC_MEALS_AVAILABILITY { get; set; }
        public virtual DbSet<BC_MEALSBOOKING_TRN_DGIT> BC_MEALSBOOKING_TRN { get; set; }
        public virtual DbSet<BC_MENUMST_DGIT> BC_MENUMST { get; set; }
        //public virtual DbSet<BC_MST_ALACARTE_TYPE_DGIT> BC_MST_ALACARTE_TYPE { get; set; }
        public virtual DbSet<BC_MST_MEALS_DGIT> BC_MST_MEALS { get; set; }
        public virtual DbSet<BC_PRICE_VALIDITY_DGIT> BC_PRICE_VALIDITY { get; set; }
        //public virtual DbSet<BC_SUBSIDIZED_MEAL_TOKEN_DGIT> BC_SUBSIDIZED_MEAL_TOKEN { get; set; }
        //public virtual DbSet<BC_SUSIDIZED_MEAL_TRN_DGIT> BC_SUSIDIZED_MEAL_TRN { get; set; }
        //public virtual DbSet<BC_HRECODEMAP_DGIT> BC_HRECODEMAP { get; set; }
        public virtual DbSet<ADORGCOORDINATOR_DGIT> ADORGCOORDINATOR { get; set; }
        public virtual DbSet<BC_MST_MEAL_TYPE_DGIT> BC_MST_MEAL_TYPE { get; set; }
        public virtual DbSet<BC_FAMILYMEALBOOKING_DTL_DGIT> BC_FAMILYMEALBOOKING_DTL { get; set; }
        public virtual DbSet<BC_FAMILYMEALBOOKING_TRN_DGIT> BC_FAMILYMEALBOOKING_TRN { get; set; }
        public virtual DbSet<BC_MST_VALIDATION_DGIT> BC_MST_VALIDATION { get; set; }
        public virtual DbSet<BC_MEALSLOTMST_DGIT> BC_MEALSLOTMST { get; set; }
        public virtual DbSet<BC_FMLY_SLOT_MAP> BC_FMLY_SLOT_MAP { get; set; }
        //public virtual DbSet<CSD_ADMINUSER_PLANT_MAP> CSD_ADMINUSER_PLANT_MAP { get; set; }
        public virtual DbSet<CSD_BAKERYITEM_MST> CSD_BAKERYITEM_MST { get; set; }
        public virtual DbSet<CSD_CANTEEN_MST> CSD_CANTEEN_MST { get; set; }
        public virtual DbSet<CSD_CANTEENMEALTYPE_MAP> CSD_CANTEENMEALTYPE_MAP { get; set; }
        public virtual DbSet<CSD_GUESTMEALBOOKING_DTL> CSD_GUESTMEALBOOKING_DTL { get; set; }
        public virtual DbSet<CSD_GUESTMEALBOOKING_TRN> CSD_GUESTMEALBOOKING_TRN { get; set; }
        public virtual DbSet<CSD_MEAL_AVAILABILITY> CSD_MEAL_AVAILABILITY { get; set; }
        public virtual DbSet<CSD_MEALBOOKING_TRN> CSD_MEALBOOKING_TRN { get; set; }
        public virtual DbSet<CSD_MEALMST> CSD_MEALMST { get; set; }
        public virtual DbSet<CSD_MEALOPTION_MST> CSD_MEALOPTION_MST { get; set; }
        public virtual DbSet<CSD_MEALTIME_MAP> CSD_MEALTIME_MAP { get; set; }
        public virtual DbSet<CSD_MEALTYPE_MST> CSD_MEALTYPE_MST { get; set; }
        public virtual DbSet<CSD_PRICE_VALIDITY> CSD_PRICE_VALIDITY { get; set; }
        //public virtual DbSet<CSD_READER_MAPP> CSD_READER_MAPP { get; set; }
        public virtual DbSet<CSD_VALIDATION_MST> CSD_VALIDATION_MST { get; set; }
        public virtual DbSet<CM_COMMAPPAUTH_SEQ> CM_COMMAPPAUTH_SEQ { get; set; }
        public virtual DbSet<CM_COMMCATEGORY_MST> CM_COMMCATEGORY_MST { get; set; }
        public virtual DbSet<CM_COMMTYPE_MST> CM_COMMTYPE_MST { get; set; }
        public virtual DbSet<CM_COMMREQ_VIEWEMPMAPP> CM_COMMREQ_VIEWEMPMAPP { get; set; }
        public virtual DbSet<CM_COMMUNICATION_APPHISTORY> CM_COMMUNICATION_APPHISTORY { get; set; }
        public virtual DbSet<CM_COMMUNICATION_DTL> CM_COMMUNICATION_DTL { get; set; }
        public virtual DbSet<CM_COMMUNICATION_HDR> CM_COMMUNICATION_HDR { get; set; }
        public virtual DbSet<FD_BOKKING_SLOT_MAP> FD_BOKKING_SLOT_MAP { get; set; }
        public virtual DbSet<FD_VISITBOOKING_TRN> FD_VISITBOOKING_TRN { get; set; }
        public virtual DbSet<FD_VISITBOOKING_DTL> FD_VISITBOOKING_DTL { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ADEMPLOGIN>().HasKey(k => new { k.ADEMPCODE, k.SYUSERTYPEID }); // for composite key
                                                                                                //modelBuilder.Entity<VW_ASSOCIATELVLDETAILS>().HasNoKey();

            //Not in DB
            //modelBuilder.Entity<SYKI_DGIT>().HasNoKey();  
            //modelBuilder.Entity<BC_MENUMST_DGIT>().HasNoKey();
            //modelBuilder.Entity<ADEMPLOYEE_DGIT>().HasNoKey();
            //modelBuilder.Entity<ADDESIGNATION_DGIT>().HasNoKey();
            //modelBuilder.Entity<ADFUNCTIONALDESIGNATION_DGIT>().HasNoKey();

            //modelBuilder.Entity<SYSITE_DGIT>().HasNoKey();
            // modelBuilder.Entity<ADORGLEVEL_DGIT>().HasNoKey();
            modelBuilder.Entity<VW_ASSOCIATELVLDETAILS_DGIT>().HasNoKey();
            // modelBuilder.Entity<BC_MST_MEAL_TYPE_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_MEALSLOTMST_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_MEALSBOOKING_TRN_DGIT>().HasNoKey();



            //modelBuilder.Entity<BC_MEALS_AVAILABILITY_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_MST_MEALS_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_GUESTMEALBOOKING_TRN_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_GUESTMEALBOOKING_DTL_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_GUEST_DTL_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_PRICE_VALIDITY_DGIT>().HasNoKey();
            //modelBuilder.Entity<ADORGCOORDINATOR_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_ALACARTEFOOD_HDR_DGIT>().HasNoKey();
            //modelBuilder.Entity<BC_ALACARTEFOOD_TRN_DGIT>().HasNoKey();
            modelBuilder.Entity<BC_FMLY_SLOT_MAP>().HasNoKey();
            //modelBuilder.Entity<BC_MST_VALIDATION_DGIT>().HasNoKey();

            //Start SR102091
            modelBuilder.Entity<SM_EMP_ACTIVEINACTIVEFORROSTER>().HasNoKey();
            modelBuilder.Entity<SM_EMP_ACTIVEINACTIVEFORROSTER_LOG>().HasNoKey();
            modelBuilder.Entity<SM_WFOMANDATORYDAYS_LOG>().HasNoKey();
            //End SR102091

            modelBuilder.Entity<CM_COMMUNICATION_HDR>()
                .Property(e => e.EMAIL_CONTENT)
                .HasColumnType("CLOB");

            modelBuilder.Entity<CM_COMMUNICATION_HDR>()
              .Property(e => e.EMAIL_CONTENT_ORIG)
              .HasColumnType("CLOB");

            base.OnModelCreating(modelBuilder);
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedure<T>(string procedureName, OracleParameter[] parameters) where T : class
        {
            // Create a cursor parameter for output
            var cursorParam = new OracleParameter("CUR_OUTPUT", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };

            // Combine parameters with the cursor parameter
            var allParameters = parameters.Append(cursorParam).ToArray();

            // Build the command string dynamically with uppercase procedure name
            var commandText = $"BEGIN {procedureName.ToUpper()}({string.Join(", ", parameters.Select(p => $":{p.ParameterName.ToUpper()}"))}, :CUR_OUTPUT); END;";

            // Execute the stored procedure
            var result = await Set<T>() // Using Set<T>() directly
                .FromSqlRaw(commandText, allParameters)
                .ToListAsync();

            return result; // The method now allows exceptions to propagate
        }

        #region MealFeedback
        public DbSet<CSD_FEEDBACK_POINTS_MST> CSD_FEEDBACK_POINTS_MST { get; set; }
        public DbSet<CSD_FEEDBACK_DETAILS_TRN> CSD_FEEDBACK_DETAILS_TRN { get; set; }
        public DbSet<CSD_FEEDBACK_IMAGE> CSD_FEEDBACK_IMAGE { get; set; }
        #endregion
    }
}
