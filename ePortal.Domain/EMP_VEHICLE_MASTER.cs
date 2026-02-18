using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class EMP_VEHICLE_MASTER
    {
        [Key]
        public int VEHICLEID { get; set; }
        public string EMPCODE { get; set; } = string.Empty;
        public short STATUS { get; set; }
        public bool IsExpired { get; set; }

        // Admin Step
        public string? ADMIN_ACTION_BY { get; set; }
        public DateTime? ADMIN_ACTION_DATE { get; set; }


        public long? PARKING_ZONE_ID { get; set; }

        [ForeignKey("PARKING_ZONE_ID")]
        public PARKING_ZONE? PARKING_ZONE { get; set; }

        // Security Step
        public string? SECURITY_ACTION_BY { get; set; }
        public DateTime? SECURITY_ACTION_DATE { get; set; }


        // Vehicle Info
        public short VEHICLE_TYPE { get; set; }
        public short VEHICLE_CATEGORY { get; set; }

        public string? DRIVER_NAME { get; set; }
        public string DL_NUMBER { get; set; } = string.Empty;
        public DateTime? DL_VALID_TILL { get; set; }
        public short DL_TYPE { get; set; }
        public string? DL_PHOTOPATH { get; set; }

        // Audit
        public DateTime CREATED_DATE { get; set; } = DateTime.UtcNow;

        [Column("LOCATION_ID")]
        public long LOCATION_ID { get; set; }
        [ForeignKey("LOCATION_ID")]
        public SYSITE? LOCATION { get; set; }
    }

}