using Assignment;
public class LeaveRequest
{
    private static int idCounter = 1;

    public int LeaveID { get; private set; }
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public Employee Employee { get; set; } 

    public LeaveRequest(Employee employee, string leaveType, DateTime startDate, DateTime endDate, string status)
    {
        LeaveID = idCounter++;
        Employee = employee;
        LeaveType = leaveType;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
    }
}
