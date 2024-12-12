using System;
using MySql.Data.MySqlClient;

namespace Assignment
{
    public class Employee
    {
        // Google SQL instance connection string
        private string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=ubiataya122;Database=assignment;";

        public string Name { get; set; }
        
        public string EmployeeID { get; set; }

        public string Department { get; set; }
        public string Designation { get; set; }
        public string Salary { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsPermanent { get; set; }
        public List<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public int AnnualLeaveBalance { get; set; }
        public int CasualLeaveBalance { get; set; }
        public int ShortLeaveBalance { get; set; }
        public DateTime? RoasterStartTime { get; set; }
        public DateTime? RoasterEndTime { get; set; }

        // Constructor
        public Employee(string name, string department, string designation, string salary, string location, string email, string phone, string address, bool isPermanent)
        {
            
            Name = name;
            Department = department;
            Designation = designation;
            Salary = salary;
            Location = location;
            Email = email;
            Phone = phone;
            Address = address;
            IsPermanent = isPermanent;

            if (isPermanent)
            {
                AnnualLeaveBalance = 14;
                CasualLeaveBalance = 7;
                ShortLeaveBalance = 2;
            }
            else
            {
                AnnualLeaveBalance = 7;
                CasualLeaveBalance = 3;
                ShortLeaveBalance = 1;
            }
        }

    public Employee(){
        
    }
private string employeeNumber = "user";
private string password = "user123"; 

// Employee Login 
public bool UserLogin(string empNumber, string inputPassword)
{
    string query = "SELECT * FROM Employees WHERE username = @username AND password = @password";
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        connection.Open();

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@username", empNumber);
            command.Parameters.AddWithValue("@password", inputPassword);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Check if the retrieved username and password match the inputs
                    if (reader["username"].ToString() == empNumber && reader["password"].ToString() == inputPassword)
                    {
                        return true; 
                    }
                }
            }
        }
    }

    return false; 
}


public Employee GetEmployeeById(int id)
{
    string query = "SELECT * FROM Employees WHERE id = @id";

    try
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Safely handle potential NULL values
                        string name = reader["Name"]?.ToString();
                        string department = reader["Department"]?.ToString();
                        string designation = reader["Designation"]?.ToString();
                        string salary = reader["Salary"]?.ToString();
                        string location = reader["Location"]?.ToString();
                        string email = reader["Email"]?.ToString();
                        string phone = reader["Phone"]?.ToString();
                        string address = reader["Address"]?.ToString();
                        bool isPermanent = reader["Is_Permanent"] != DBNull.Value && Convert.ToBoolean(reader["Is_Permanent"]);

                        Employee employee = new Employee(
                            name,
                            department,
                            designation,
                            salary,
                            location,
                            email,
                            phone,
                            address,
                            isPermanent
                        );

                        // Handle optional fields safely
                        employee.AnnualLeaveBalance = reader["AnnualLeaveBalance"] != DBNull.Value
                            ? Convert.ToInt32(reader["AnnualLeaveBalance"])
                            : 0;

                        employee.CasualLeaveBalance = reader["CasualLeaveBalance"] != DBNull.Value
                            ? Convert.ToInt32(reader["CasualLeaveBalance"])
                            : 0;

                        employee.ShortLeaveBalance = reader["ShortLeaveBalance"] != DBNull.Value
                            ? Convert.ToInt32(reader["ShortLeaveBalance"])
                            : 0;

                        employee.RoasterStartTime = reader["RoasterStartTime"] != DBNull.Value
                            ? Convert.ToDateTime(reader["RoasterStartTime"])
                            : (DateTime?)null;

                        employee.RoasterEndTime = reader["RoasterEndTime"] != DBNull.Value
                            ? Convert.ToDateTime(reader["RoasterEndTime"])
                            : (DateTime?)null;


                        return employee;
                    }
                }
            }
        }

        return null; 
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching employee: {ex.Message}");
        return null; // Return null in case of an error
    }

}
public string ApplyLeave(int EmployeeID, string leaveType, DateTime leaveStartDate, DateTime leaveEndDate)
{
    // Validate the leave dates
    if (leaveStartDate > leaveEndDate)
    {
        return "End date must be after start date.";
    }

    // Check leave balance before proceeding
    if (!CheckLeaveBalance(leaveType, leaveStartDate, leaveEndDate))
    {
        return $"Insufficient {leaveType.ToLower()} leave balance.";
    }

    // Ensure annual leave is applied 7 days in advance
    if (leaveType == "Annual" && (leaveStartDate - DateTime.Now).Days < 7)
    {
        return "Annual leave must be applied at least 7 days in advance.";
    }

    string query = @"
    INSERT INTO LeaveRequest (EmployeeID, LeaveType, LeaveStartDate, LeaveEndDate, LeaveDuration, LeaveDate, Status) 
    VALUES (@EmployeeID, @LeaveType, @StartDate, @EndDate, @Duration, @LeaveDate, @Status);";

    try
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand(query, connection);

            // Calculate leave duration in days (for annual and casual leaves)
            int duration = (leaveEndDate - leaveStartDate).Days + 1;

            // Add parameters (use the passed EmployeeID here)
            command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            command.Parameters.AddWithValue("@LeaveType", leaveType);
            command.Parameters.AddWithValue("@StartDate", leaveStartDate);
            command.Parameters.AddWithValue("@EndDate", leaveEndDate);
            command.Parameters.AddWithValue("@LeaveDate", leaveStartDate); // You can change this if needed
            command.Parameters.AddWithValue("@Duration", duration);
            command.Parameters.AddWithValue("@Status", "Pending");

            // Execute the query
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                return "Failed to apply leave. Please try again.";
            }

            // Retrieve the auto-generated LeaveID
            command.CommandText = "SELECT LAST_INSERT_ID();";
            object result = command.ExecuteScalar();
            if (result == null || !int.TryParse(result.ToString(), out int leaveID))
            {
                return "Failed to retrieve leave ID. Please try again.";
            }

            // Update leave balance in the database
            UpdateLeaveBalance(leaveType, duration);

            return $"{leaveType} leave applied successfully. Leave ID: {leaveID}";
        }
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine($"Error applying leave: {ex.Message}");
        return $"Error applying leave: {ex.Message}";
    }
}

    // Method to check the leave balance
    private bool CheckLeaveBalance(string leaveType, DateTime leaveStartDate, DateTime leaveEndDate)
    {
        int requiredBalance = (leaveEndDate - leaveStartDate).Days + 1;

        if (leaveType == "Annual" && 14 >= requiredBalance)
            return true;
        else if (leaveType == "Casual" && 14 >= requiredBalance)
            return true;
        else if (leaveType == "Short" && 14 >= requiredBalance)
            return true;

        return false;
    }

    // Method to update the leave balance after applying for leave
    private void UpdateLeaveBalance(string leaveType, int duration)
    {
        string query = "";

        if (leaveType == "Annual")
        {
            AnnualLeaveBalance -= duration;
            query = "UPDATE Employees SET AnnualLeaveBalance = @AnnualLeaveBalance WHERE id = @EmployeeID";
        }
        else if (leaveType == "Casual")
        {
            CasualLeaveBalance -= duration;
            query = "UPDATE Employees SET CasualLeaveBalance = @CasualLeaveBalance WHERE id = @EmployeeID";
        }
        else if (leaveType == "Short")
        {
            ShortLeaveBalance -= duration;
            query = "UPDATE Employees SET ShortLeaveBalance = @ShortLeaveBalance WHERE id = @EmployeeID";
        }

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmployeeID", 1);

            if (leaveType == "Annual")
                command.Parameters.AddWithValue("@AnnualLeaveBalance", AnnualLeaveBalance);
            else if (leaveType == "Casual")
                command.Parameters.AddWithValue("@CasualLeaveBalance", CasualLeaveBalance);
            else if (leaveType == "Short")
                command.Parameters.AddWithValue("@ShortLeaveBalance", ShortLeaveBalance);

            command.ExecuteNonQuery();
        }
    }

    // Method to add leave history after applying for leave
    private void AddLeaveHistory(string leaveType, DateTime leaveStartDate, DateTime leaveEndDate, int duration)
    {
        string query = @"
        INSERT INTO LeaveHistory (EmployeeID, LeaveType, LeaveStartDate, LeaveEndDate, LeaveDuration, Status) 
        VALUES (@EmployeeID, @LeaveType, @StartDate, @EndDate, @Duration, @Status);";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand(query, connection);

            // Add parameters
            command.Parameters.AddWithValue("@EmployeeID", 1);
            command.Parameters.AddWithValue("@LeaveType", leaveType);
            command.Parameters.AddWithValue("@StartDate", leaveStartDate);
            command.Parameters.AddWithValue("@EndDate", leaveEndDate);
            command.Parameters.AddWithValue("@Duration", duration);
            command.Parameters.AddWithValue("@Status", "Pending");

            // Execute the query
            command.ExecuteNonQuery();
        }
    }

public List<LeaveRequestDetails> ViewLeaveStatus(int employeeID)
{
    List<LeaveRequestDetails> leaveRequests = new List<LeaveRequestDetails>();
    
    // Query to get current leave requests
    string currentLeaveQuery = @"
        SELECT lr.LeaveID, lr.LeaveType, lr.LeaveStartDate, lr.LeaveEndDate, lr.LeaveDuration, lr.Status
        FROM LeaveRequest lr
        WHERE lr.LeaveID = @EmployeeID AND lr.Status IN ('Pending', 'Approved', 'Rejected')
        ORDER BY lr.LeaveStartDate DESC;";

    // Query to get historical leave requests
    string historyLeaveQuery = @"
        SELECT lh.HistoryID AS LeaveID, lh.LeaveType, lh.LeaveStartDate, lh.LeaveEndDate, lh.LeaveDuration, lh.Status
        FROM LeaveHistory lh
        WHERE lh.EmployeeID = @EmployeeID
        ORDER BY lh.LeaveStartDate DESC;";

    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        connection.Open();

        // Get current leave requests
        using (MySqlCommand currentCommand = new MySqlCommand(currentLeaveQuery, connection))
        {
            currentCommand.Parameters.AddWithValue("@EmployeeID", employeeID);

            using (MySqlDataReader reader = currentCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    LeaveRequestDetails leaveRequest = new LeaveRequestDetails
                    {
                        LeaveID = reader.GetInt32("LeaveID"),
                        LeaveType = reader.GetString("LeaveType"),
                        LeaveStartDate = reader.GetDateTime("LeaveStartDate"),
                        LeaveEndDate = reader.GetDateTime("LeaveEndDate"),
                        LeaveDuration = reader.GetInt32("LeaveDuration"),
                        Status = reader.GetString("Status")
                    };
                    leaveRequests.Add(leaveRequest);
                }
            }
        }

        // Get leave history
        using (MySqlCommand historyCommand = new MySqlCommand(historyLeaveQuery, connection))
        {
            historyCommand.Parameters.AddWithValue("@EmployeeID", employeeID);

            using (MySqlDataReader reader = historyCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    LeaveRequestDetails leaveHistory = new LeaveRequestDetails
                    {
                        LeaveID = reader.GetInt32("LeaveID"),
                        LeaveType = reader.GetString("LeaveType"),
                        LeaveStartDate = reader.GetDateTime("LeaveStartDate"),
                        LeaveEndDate = reader.GetDateTime("LeaveEndDate"),
                        LeaveDuration = reader.GetInt32("LeaveDuration"),
                        Status = reader.GetString("Status")
                    };
                    leaveRequests.Add(leaveHistory);
                }
            }
        }
    }

    return leaveRequests;
}

        // Method to view remaining leaves
public void ViewRemainingLeaveBalances(int employeeID)
    {
        string query = "SELECT AnnualLeaveBalance, CasualLeaveBalance, ShortLeaveBalance FROM Employees WHERE id = @employeeID";
        
        
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open(); // Open the connection
            MySqlCommand cmd = new MySqlCommand(query, connection);

            // Add parameter to prevent SQL injection
            cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

            using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute the query and read results
            {
                if (reader.Read())
                {
                    // Get the leave balances from the result set
                    int annualLeaveBalance = reader.GetInt32(0);
                    int casualLeaveBalance = reader.GetInt32(1);
                    int shortLeaveBalance = reader.GetInt32(2);

                    // Display the remaining leave balances
                    Console.WriteLine("\nRemaining Leave Balances:");
                    Console.WriteLine($"Annual Leave: {annualLeaveBalance} days");
                    Console.WriteLine($"Casual Leave: {casualLeaveBalance} days");
                    Console.WriteLine($"Short Leave: {shortLeaveBalance} days");
                }
                else
                {
                    // If no employee with the given ID is found
                    Console.WriteLine("Employee not found.");
                }
            }
        }
    }
        // Method to delete leave request
public string DeleteLeave(int leaveID)
{
    int EmployeeID = leaveID;
    
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        connection.Open();

        // Start a transaction to ensure atomicity of the operations
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                // First, delete the leave request
                string query = "DELETE FROM LeaveRequest WHERE LeaveID = @LeaveId"; 
                MySqlCommand cmd = new MySqlCommand(query, connection, transaction);
                cmd.Parameters.AddWithValue("@LeaveId", EmployeeID);

                // Execute the DELETE query
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    // If no rows were affected, return an error message
                    return "Cannot delete leave request. It might already be approved or does not exist.";
                }
                return "Leave request deleted successfully.";
            }
            catch (Exception ex)
            {
                // If any error occurs, roll back the transaction to maintain data integrity
                transaction.Rollback();
                return $"Error occurred while deleting leave: {ex.Message}";
            }
        }
    }
}
// Helper method to get leave type for a given leave ID
private string GetLeaveType(int leaveID, MySqlConnection connection, MySqlTransaction transaction)
{
    string leaveType = null;

    string query = "SELECT LeaveType FROM LeaveRequest WHERE LeaveID = @LeaveID";
    MySqlCommand cmd = new MySqlCommand(query, connection, transaction);
    cmd.Parameters.AddWithValue("@LeaveID", leaveID);

    try
    {
        leaveType = cmd.ExecuteScalar()?.ToString(); // Execute and get the leave type
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching leave type: {ex.Message}");
    }

    return leaveType;
}


}
}

    public class LeaveRequestDetails
{
    public int LeaveID { get; set; }
    public string LeaveType { get; set; }
    public DateTime LeaveStartDate { get; set; }
    public DateTime LeaveEndDate { get; set; }
    public int LeaveDuration { get; set; }
    public string Status { get; set; }
}