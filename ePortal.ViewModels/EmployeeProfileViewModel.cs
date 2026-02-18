using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class Employee_Details
    {
        public long _ECode { get; set; }
        public string _EName { get; set; }
        public string _EFirstName { get; set; }
        public string _ELastName { get; set; }
        public string? _OpDesc { get; set; }
        public string _DivDesc { get; set; }
        public string _DepDesc { get; set; }
        public string _SecDescrip { get; set; }
        public long? _OpId { get; set; }
        public long? _DivId { get; set; }
        public long? _DepId { get; set; }
        public long? _SecId { get; set; }
        public string _FnDesig { get; set; }
        public long? _FnDesigId { get; set; }
        public string _Desig { get; set; }
        public long? _DesigId { get; set; }
        public long? _SiteId { get; set; }
        public string _EmailId { get; set; }
        public string _PancardNo { get; set; }
        public long? _PlantId { get; set; }
        public DateTime? _DOJ { get; set; }
        public DateTime? _DOB { get; set; }
        public string _MobileNo { get; set; }
        public DateTime? _ConfirmationDate { get; set; }
        public string _Gender { get; set; }
        public string _EmpLtype { get; set; }
        public DateTime? _DOM { get; set; }
        public string Functional_Designation {
            get { return _FnDesig; }
        }
        public string Functional_Designation_Id
        {
            get { return _FnDesigId.ToString(); }
        }

        public string Employee_Code
        {
            get { return _ECode.ToString(); }
        }
        public string Site_Id
        {
            get { return _SiteId.ToString(); }
        }

        public string Operation_Id
        {
            get { return _OpId.ToString(); }
        }

        public string Division_Id
        {
            get { return _DivId.ToString(); }
        }

        public string Department_Id
        {
            get { return _DepId.ToString(); }
        }

        public string Section_Id
        {
            get { return _SecId.ToString(); }
        }

        
        public string ConfirmDate
        {
            get { return _ConfirmationDate.ToString(); }
        }
        public string Employee_Name
        {
            get { return _EName; }
        }

        public string Employee_First_Name
        {
            get { return _EFirstName; }
        }

        public string Employee_Last_Name
        {
            get { return _ELastName; }
        }

        public string Operation_Desc
        {
            get { return _OpDesc; }
        }

        public string Division_Desc
        {
            get { return _DivDesc; }
        }

        public string Department_Desc
        {
            get { return _DepDesc; }
        }

        public string Section_Desc
        {
            get { return _SecDescrip; }
        }

        public string Designation
        {
            get { return _Desig; }
        }

        public string Designation_Id
        {
            get { return _DesigId.ToString(); }
        }

        public string EMail_Id
        {
            get { return _EmailId; }
        }

        public string PancardNo
        {
            get { return _PancardNo; }
        }

        public string Plant_Id
        {
            get { return _PlantId.ToString(); }
        }

        public string DOJ
        {
            get { return _DOJ.ToString(); }
        }
        public string DOB
        {
            get {
                //return _DOB.Value.ToString("dd-MMM-yyyy"); 
                return _DOB.HasValue ? _DOB.Value.ToString("dd-MMM-yyyy") : string.Empty;
            }
        }
        public string MobileNo
        {
            get { return _MobileNo; }
        }

        public string Gender
        {
            get { return _Gender; }
        }
        public long? ORGLVL
        {
            get; set;
        }
        public DateTime? DOM
        {
            get { return _DOM; }
        }
    }

    //Added By TTL against CR6695 as on 29-07-2025 
    public class CookieDetails
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string EmpType { get; set; }
        public int ISSSOLOGIN { get; set; }
        public string NetworkType { get; set; }
        public int IdleTimeoutMinutes { get; set; }

        public Employee_Details EmpDetails { get; set; }
    }
}
