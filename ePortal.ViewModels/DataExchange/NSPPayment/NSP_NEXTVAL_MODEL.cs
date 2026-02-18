using System.ComponentModel.DataAnnotations.Schema;

namespace ePortal.ViewModels.DataExchange.NSPPayment
{
    public class NSP_NEXTVAL_MODEL
    {
        [Column("NEXTVAL")]
        public int NEXTVAL { get; set; }
    }
}
