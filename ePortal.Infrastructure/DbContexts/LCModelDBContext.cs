using ePortal.DomainClasses;
using Microsoft.EntityFrameworkCore;
using ePortal.ViewModels.DataExchange.NSPPayment;

namespace ePortal.Infrastructure.DbContexts
{
    public class LCModelDBContext : DbContext
    {
        public LCModelDBContext(DbContextOptions<LCModelDBContext> options) : base(options) { }

        public virtual DbSet<ADEMPLOYEE_LC> ADEMPLOYEE_LC { get; set; }
        public virtual DbSet<SYKI_LC> SYKI_LC { get; set; }
        public virtual DbSet<ADDESIGNATION_LC> ADDESIGNATION_LC { get; set; }
        //public virtual DbSet<ADFUNCTIONALDESIGNATION_LC> ADFUNCTIONALDESIGNATION_LC { get; set; }
        public virtual DbSet<ADORGLEVEL_LC> ADORGLEVEL_LC { get; set; }
        public virtual DbSet<ADORGLEVELHEAD_LC> ADORGLEVELHEAD_LC { get; set; }
        public virtual DbSet<SYPARAMETERS_LC> SYPARAMETERS_LC { get; set; }
        public virtual DbSet<LC_DETAIL> LC_DETAIL { get; set; }
        public virtual DbSet<LC_DETAIL_TEMP> LC_DETAIL_TEMP { get; set; }
        public virtual DbSet<LC_EXCEPTION_C_TABLE> LC_EXCEPTION_C_TABLE { get; set; }
        public virtual DbSet<LC_HEAD> LC_HEAD { get; set; }
        public virtual DbSet<LC_ROLEMASTER> LC_ROLEMASTER { get; set; }
        public virtual DbSet<LC_REIMBURSEMENT_B_TABLE> LC_REIMBURSEMENT_B_TABLE { get; set; }
        public virtual DbSet<LC_ZTABLE> LC_ZTABLE { get; set; }
        public virtual DbSet<SYPLANT_LC> SYPLANT_LC { get; set; }
        public virtual DbSet<SYSITE_LC> SYSITE_LC { get; set; }

        public virtual DbSet<LC_OH_DIRAPPROVALFLOW_MASTER> LC_OH_DIRAPPROVALFLOW_MASTER { get; set; }
        public virtual DbSet<LC_OH_DIRAPPROVALFLOW_MASTER_LOG> LC_OH_DIRAPPROVALFLOW_MASTER_LOG { get; set; }
        public virtual DbSet<LC_DETAIL_LOG> LC_DETAIL_LOG { get; set; }
        public virtual DbSet<LC_EXCEPTION_C_TABLE_LOG> LC_EXCEPTION_C_TABLE_LOG { get; set; }
        public virtual DbSet<LC_HEAD_LOG> LC_HEAD_LOG { get; set; }
        public virtual DbSet<LC_REIMBURSEMENT_B_TABLE_LOG> LC_REIMBURSEMENT_B_TABLE_LOG { get; set; }
        public virtual DbSet<LC_ZTABLE_LOG> LC_ZTABLE_LOG { get; set; }

        public virtual DbSet<FINVENDORMASTERMST_LC> FINVENDORMASTERMST_LC { get; set; }
        public virtual DbSet<ADORGCOORDINATOR_LC> ADORGCOORDINATOR_LC { get; set; }
        public virtual DbSet<AT_COMMON_APPROVAL_MASTER> AT_COMMON_APPROVAL_MASTER { get; set; }
        public virtual DbSet<AT_ASSET_TRANSFER_DETAIL> AT_ASSET_TRANSFER_DETAIL { get; set; }
        public virtual DbSet<AT_ASSSET_TRANSFER_HEADER> AT_ASSSET_TRANSFER_HEADER { get; set; }
        public virtual DbSet<AT_APPROVAL_AUTHORITY> AT_APPROVAL_AUTHORITY { get; set; }
        public virtual DbSet<AT_APPROVAL_AUTHORITY_LOG> AT_APPROVAL_AUTHORITY_LOG { get; set; }
        public virtual DbSet<AT_APPROVAL_OPERATION_MAPPING> AT_APPROVAL_OPERATION_MAPPING { get; set; }
        public virtual DbSet<AT_AST_OPERATIONMAPP> AT_AST_OPERATIONMAPP { get; set; }
        public virtual DbSet<NSP_PAYMENT_HEADER> NSP_PAYMENT_HEADER { get; set; }
        public virtual DbSet<NSP_PAYMENT_DETAIL> NSP_PAYMENT_DETAIL { get; set; }
        public virtual DbSet<NSP_APPROVAL_AUTHORITY> NSP_APPROVAL_AUTHORITY { get; set; }
        public virtual DbSet<NSP_ROLE_MASTER> NSP_ROLE_MASTER { get; set; }
        public virtual DbSet<NSP_ROLE_MASTER_LOG> NSP_ROLE_MASTER_LOG { get; set; }
        public virtual DbSet<NSP_TDS_TAX_TYPE_MASTER> NSP_TDS_TAX_TYPE_MASTER { get; set; }
        public virtual DbSet<NSP_TDS_TAX_TYPE_MASTER_LOG> NSP_TDS_TAX_TYPE_MASTER_LOG { get; set; }
        public virtual DbSet<NSP_GST_TAX_MASTER> NSP_GST_TAX_MASTER { get; set; }
        public virtual DbSet<NSP_GST_TAX_MASTER_LOG> NSP_GST_TAX_MASTER_LOG { get; set; }
        public virtual DbSet<NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER> NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER { get; set; }
        public virtual DbSet<NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG> NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG { get; set; }
        public virtual DbSet<NSP_POSTING_DETAIL> NSP_POSTING_DETAIL { get; set; }
        public virtual DbSet<FINTBS_COST_CENTER_MST> FINTBS_COST_CENTER_MST { get; set; }
        public virtual DbSet<VW_ASSOCIATELVLDETAILS_LC> VW_ASSOCIATELVLDETAILS_LC { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<LC_OH_DIRAPPROVALFLOW_MASTER>().HasNoKey();
            modelBuilder.Entity<LC_OH_DIRAPPROVALFLOW_MASTER_LOG>().HasNoKey();


            modelBuilder.Entity<VW_ASSOCIATELVLDETAILS_LC>().HasNoKey();
            modelBuilder.Entity<AT_AST_OPERATIONMAPP>().HasKey(k => new { k.ASSETTRSOPMAPID, k.OPERATIONID });
            //modelBuilder.Entity<NSP_ROLE_MASTER>().HasNoKey();
            //modelBuilder.Entity<NSP_ROLE_MASTER_LOG>().HasNoKey();
            //modelBuilder.Entity<NSP_TDS_TAX_TYPE_MASTER_LOG>().HasNoKey();
            //modelBuilder.Entity<NSP_GST_TAX_MASTER_LOG>().HasNoKey();
            //modelBuilder.Entity<NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG>().HasNoKey();
            modelBuilder.Entity<NSP_NEXTVAL_MODEL>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }

    }
}
