using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class MenuParamsGeneral
    {
        public int MenuId { get; set; }
        public UserTypes UserType { get; set; }
        public NetworkSource networkSource { get; set; }
        public int BPOid { get; set; }
        public int OrgId { get; set; }
    }

    public class MenuParamsFnDesignations
    {
        public int MenuId { get; set; }
        public int FnDesignationId { get; set; }
    }

    public class MenuParamLocations
    {
        public int MenuId { get; set; }
        public int LocationId { get; set; }
    }

    public class MenuParamOrgIds
    {
        public int MenuId { get; set; }
        public int BPOId { get; set; }
    }

    public enum UserTypes
    {
        User,
        Admin
        
    }

    public enum NetworkSource
    {
        Internet,
        Intranet,
        Both
    }

    public enum Operator
    {
        Equal,
        NotEqual
    }

    public class MenuMaster
    {
        public int MenuId {get;set;}
        public int MenuLevel { get; set; }
        public int ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public string Menutext { get; set; }
        public string Tooltip { get; set; }
        public string URL { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }
}
