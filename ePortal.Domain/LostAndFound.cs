using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ePortal.DomainClasses
{
    [Table("LOST_AND_FOUND")]
    public class LostAndFound
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        public string? ItemType { get; set; } 
        public string? ItemName { get; set; }
        public string? IdentificationMark { get; set; }
        public string? MaterialType { get; set; }
        public string? AdditionalDetails { get; set; }
        [Column("LOCATION_ID")]
        public long LocationId { get; set; }   
        [ForeignKey("LocationId")]
        public virtual SYSITE? Location { get; set; }
        public DateTime Date { get; set; }
        public string? ItemPhotoPath { get; set; }
        public int Status { get; set; } 
        public string? ClaimedBy { get; set; }
        public string? ClaimedByEmpCode { get; set; }
        public DateTime? DateClaimed { get; set; }
        public string? VerificationDetails { get; set; }
        public short IsActive { get; set; } 
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int? EmployeeType { get; set; }  
        public string? CompanyName { get; set; }

        public short IsResubmit { get; set; }
    }
}
