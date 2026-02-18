using ePortal.DomainClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.Web.Models
{
    public class AssetTransferTrasactionData
    {
       public AT_ASSSET_TRANSFER_HEADER Header  { get; set; }
       public List<AT_ASSET_TRANSFER_DETAIL>  Details { get; set; }

        public List<AT_APPROVAL_AUTHORITY> Authority { get; set; }

        public string flag { get; set; }
        public string AssetType { get; set; }
    }
}
