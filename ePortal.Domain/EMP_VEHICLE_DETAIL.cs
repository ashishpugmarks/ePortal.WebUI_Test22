using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class EMP_VEHICLE_DETAIL
    {
        public int ID { get; set; }
        public string VEHICLENO { get; set; } = string.Empty;
        public short FUELTYPE { get; set; }
        public DateTime MODELYEAR { get; set; }
        public short OWNERTYPE { get; set; }
        public string? OWNERNAME { get; set; } = string.Empty;
        public string? VEHICLECUSTODIAN { get; set; }

        public int VEHICLEID { get; set; }

        [ForeignKey("VEHICLEID")]
        public virtual EMP_VEHICLE_MASTER? EMPVEHICLEMASTER { get; set; }

        // RC
        public string? RCNO { get; set; }
        public DateTime? RCVALIDTILL { get; set; }
        public string? RCPHOTOPATH { get; set; }

        // Insurance
        public string? INSURANCENO { get; set; }
        public DateTime? INSURANCEVALIDTILL { get; set; }
        public string? INSURANCEPHOTOPATH { get; set; }

        // PUC
        public string? PUCNO { get; set; }
        public DateTime? PUCVALIDTILL { get; set; }
        public string? PUCPHOTOPATH { get; set; }

        public string? UPLOADEDBY { get; set; } = string.Empty;
        public DateTime UPLOADEDON { get; set; }
    }
}