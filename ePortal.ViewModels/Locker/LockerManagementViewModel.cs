using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.ViewModels.Locker;

public class LockerManagementViewModel
{
    public string? SelectedLocation { get; set; }
    public string? SelectedFloor { get; set; }
    public string? SelectedMainLocker { get; set; }

    public List<string>? Location { get; set; }
    public List<string>? Floor { get; set; }
    public List<string>? MainLocker { get; set; }

    public bool ShowResults { get; set; }

    public List<SubLockerViewModel>? SubLockers { get; set; }

    public int TotalLockers => SubLockers?.Count ?? 0;
    public int TotalOccupied => SubLockers?.Count(x => x.IsOccupied) ?? 0;
    public int TotalAvailable => SubLockers?.Count(x => !x.IsOccupied) ?? 0;
}

public class SubLockerViewModel
{
    public string? LockerNumber { get; set; }
    public bool IsOccupied { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
}
public class LockerRequestViewModel
{
    public int AssignId { get; set; }
    public string EmployeeName { get; set; }
    public string LocationName { get; set; }
    public DateTime RequestDate { get; set; }
    public int Status { get; set; } // 2=Req, 3=Approve, 4=Assign, 5=Release
    public string StatusText => Status switch
    {
        2 => "Requested",
        3 => "Approved",
        4 => "Assigned",
        5 => "Released",
        _ => "Unknown"
    };
}

public class LockerRequestAdminDto
{
    public List<LockerRequestViewModel>  lockerRequestViewModels { get; set; } = new();



    public int TotalRequest  { get; set; }
    public int PendingRequest { get; set; }
    public int ApprovedRequest { get; set; }
    public int ReallocateRequest { get; set; }

    public List<LocationDto>? Location { get; set; }
}