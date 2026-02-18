namespace ePortal.DomainClasses
{
    public static class LostAndFoundStatus
    {
        public const int Pending = 0;       // Initial status when item is reported
        public const int Approved = 1;      // Approved by security personnel (visible to users)
        public const int SendBack = 2;      // Sent back to user for corrections
        public const int Rejected = 3;      // Rejected by security personnel
        public const int Closed = 4;       // Item has been Closed
        public const int Cancel = 5;       

        public static string GetStatusMyRequest(int status)
        {
            return status switch
            {
                Pending => "Pending Review for security person",
                Approved => "Approved",
                SendBack => "Sent Back",
                Rejected => "Rejected",
                Closed => "Closed",
                Cancel => "Cancel",

                _ => "Unknown"
            };
        }

        public static string GetStatusText(int status)
        {
            return status switch
            {
                Pending => "Pending Review",
                Approved => "Approved",
                SendBack => "Sent Back",
                Rejected => "Rejected",
                Closed => "Closed",
                Cancel => "Cancel",

                _ => "Unknown"
            };
        }

        public static string GetStatusClass(int status)
        {
            return status switch
            {
                Pending => "badge-warning",
                Approved => "badge-success",
                SendBack => "badge-info",
                Rejected => "badge-danger",
                Closed => "badge-primary",
                _ => "badge-light"
            };
        }
    }
}
