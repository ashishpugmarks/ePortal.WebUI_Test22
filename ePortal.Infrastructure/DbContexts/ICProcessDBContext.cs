using ePortal.DomainClasses;
using Microsoft.EntityFrameworkCore;
namespace ePortal.Infrastructure.DbContexts
{
    public class ICProcessBDContext : DbContext
    {
        public ICProcessBDContext(DbContextOptions<ICProcessBDContext> options) : base(options) { }

        public virtual DbSet<DGIT_ICAPPAUTHSEQ> DGIT_ICAPPAUTHSEQ { get; set; }
        public virtual DbSet<DGIT_ICAPPHISTORY> DGIT_ICAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_ICCONFIG_MST> DGIT_ICCONFIG_MST { get; set; }
        public virtual DbSet<DGIT_ICDOCDETAIL> DGIT_ICDOCDETAIL { get; set; }
        public virtual DbSet<DGIT_ICREQ_SCHEDULE> DGIT_ICREQ_SCHEDULE { get; set; }
        public virtual DbSet<DGIT_ICREQHEADER> DGIT_ICREQHEADER { get; set; }
        public virtual DbSet<ADEMPLOYEEs> ADEMPLOYEEs1 { get; set; }
        public virtual DbSet<DGIT_ICINVEST_MST> DGIT_ICINVEST_MST { get; set; }
        public virtual DbSet<VW_ASSOCIATELVLDETAILS1> VW_ASSOCIATELVLDETAILS1 { get; set; }        
        public virtual DbSet<ADDESIGNATION1> ADDESIGNATION1 { get; set; }
        public virtual DbSet<SYKI1> SYKI1 { get; set; }
        public virtual DbSet<DGIT_ICMEMBER_MST> DGIT_ICMEMBER_MST { get; set; }
        public virtual DbSet<DGIT_ICASSETAPPAUTHSEQ> DGIT_ICASSETAPPAUTHSEQ { get; set; }
        public virtual DbSet<DGIT_ICASSETAPPHISTORY> DGIT_ICASSETAPPHISTORY { get; set; }
        public virtual DbSet<DGIT_ICASSETDOCDETAIL> DGIT_ICASSETDOCDETAIL { get; set; }
        public virtual DbSet<DGIT_ICASSETREQ_SCHEDULE> DGIT_ICASSETREQ_SCHEDULE { get; set; }
        public virtual DbSet<DGIT_ICASSETREQHEADER> DGIT_ICASSETREQHEADER { get; set; }
        public virtual DbSet<DGIT_ICRPTRIGHTS> DGIT_ICRPTRIGHTS { get; set; }
        public virtual DbSet<ICIBM_FINANCEDASHBOARD> ICIBM_FINANCEDASHBOARD { get; set; }
        public virtual DbSet<ICIBM_PPCDASHBOARD> ICIBM_PPCDASHBOARD { get; set; }
        public virtual DbSet<ICIBM_TAXCREDITDASHBOARD> ICIBM_TAXCREDITDASHBOARD { get; set; }
        public virtual DbSet<ICIBM_FINAUCDASHBOARD> ICIBM_FINAUCDASHBOARD { get; set; }
        public virtual DbSet<ICADORGLEVEL> ICADORGLEVELs { get; set; }
        public virtual DbSet<VW_DGIT_IOMREPORT> VW_DGIT_IOMREPORT { get; set; }
        public virtual DbSet<VW_DGIT_POREPORT> VW_DGIT_POREPORT { get; set; }
        public virtual DbSet<SYAPPLICATION> SYAPPLICATION { get; set; }
        public virtual DbSet<EMP_LOGINDETAIL> EMP_LOGINDETAIL { get; set; }
        public virtual DbSet<EMP_APPLICATIONMAP_AD> EMP_APPLICATIONMAP_AD { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VW_ASSOCIATELVLDETAILS1>().HasNoKey();
            modelBuilder.Entity<VW_DGIT_IOMREPORT>().HasNoKey();
            modelBuilder.Entity<DGIT_ICRPTRIGHTS>().HasNoKey();
            //modelBuilder.Entity<ICADORGLEVEL>().HasNoKey();
        }
    }
}
