using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class EMP_VEHICLE_HISTORY
    {
        [Key]
        public long ID { get; set; }
        public int VEHICLEID { get; set; }
        [ForeignKey("VEHICLEID")]
        public EMP_VEHICLE_MASTER EMPVEHICLEMASTER { get; set; } = default!;
        public string ACTIONBY { get; set; } = string.Empty;
        public string ACTIONBYROLE { get; set; } = string.Empty; // ADMIN, SECURITY
        public int? ADMIN_STATUS { get; set; }
        public string? ADMINREMARK { get; set; }
        public int? SECURITY_STATUS { get; set; }
        public string? SPREMARK { get; set; }
        public int? SENDBACK_FOR { get; set; }
        public long? PARKINGZONEID { get; set; }
        [ForeignKey("PARKINGZONEID")]
        public PARKING_ZONE? PARKINGZONE { get; set; }
        public DateTime ACTIONDATE { get; set; } = DateTime.UtcNow;
    }
}
