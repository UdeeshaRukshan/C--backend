using System;
using Microsoft.Data.SqlClient; // Correct reference for SQL Server

namespace Assignment
{
    public class Employee
    {
        // Google SQL instance connection string
        private string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=ubiataya122;Database=assignment;";

        public string Name { get; set; }
        public int EmployeeID { get; set; }

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
        public DateTime RoasterStartTime { get; set; }
        public DateTime RoasterEndTime { get; set; }
        // Constructor
        public Employee(int employeeID,string name, string department, string designation, string salary, string location, string email, string phone, string address, bool isPermanent)
        {
            EmployeeID = employeeID;
            Name = name;
            Department = department;
            Designation = designation;
            Salary = salary;
            Location = location;
            Email = email;
            Phone = phone;
            Address = address;

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

        public void Login(string username,string password){
            string query = "SELECT * FROM Employee WHERE Username = @Username AND Password = @Password";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Login Successful");
                        }
                        else
                        {
                            Console.WriteLine("Invalid username or password");
                        }
                    }
                }
            }
        }

// Method to apply leave
public string ApplyLeave(string leaveType, DateTime leaveStartDate, DateTime leaveEndDate)
{

    if (leaveStartDate > leaveEndDate)
    {
        return "End date must be after start date.";
    }

    string query = "INSERT INTO LeaveRequests (LeaveType, StartDate, EndDate, Status, EmployeeName) OUTPUT INSERTED.LeaveID VALUES (@LeaveType, @StartDate, @EndDate, @Status, @EmployeeName)";
    
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LeaveType", leaveType);
            command.Parameters.AddWithValue("@StartDate", leaveStartDate);
            command.Parameters.AddWithValue("@EndDate", leaveEndDate);
            command.Parameters.AddWithValue("@Status", "Pending");
            command.Parameters.AddWithValue("@EmployeeName", Name);

            try
            {
                int leaveID = (int)command.ExecuteScalar(); // Get the inserted LeaveID from the database

                // Add the leave request to the employee's LeaveRequests list
                LeaveRequest leaveRequest = new LeaveRequest(this, leaveType, leaveStartDate, leaveEndDate, "Pending");
                LeaveRequests.Add(leaveRequest);

                // Check leave balance and update accordingly
                if (leaveType == "Casual")
                {
                    if (CasualLeaveBalance > 0)
                    {
                        CasualLeaveBalance--;  // Update leave balance (in-memory only, update DB as needed)
                        return $"Casual leave applied successfully. Leave ID: {leaveID}";
                    }
                    else
                    {
                        return "Insufficient casual leave balance.";
                    }
                }
                else if (leaveType == "Annual")
                {
                    if ((leaveStartDate - DateTime.Now).Days >= 7)
                    {
                        if (AnnualLeaveBalance > 0)
                        {
                            AnnualLeaveBalance--;  // Update leave balance (in-memory only, update DB as needed)
                            return $"Annual leave applied successfully. Leave ID: {leaveID}";
                        }
                        else
                        {
                            return "Insufficient annual leave balance.";
                        }
                    }
                    else
                    {
                        return "Annual leave must be applied at least 7 days in advance.";
                    }
                }
                else if (leaveType == "Short")
                {
                    if (ShortLeaveBalance > 0)
                    {
                        ShortLeaveBalance--;  // Update leave balance (in-memory only, update DB as needed)
                        return $"Short leave applied successfully. Leave ID: {leaveID}";
                    }
                    else
                    {
                        return "Insufficient short leave balance.";
                    }
                }
                else
                {
                    return "Invalid leave type.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return $"Error applying leave: {ex.Message}";
            }
        }
    }
}



        // Method to view leave status
        public string ViewLeaveStatus(int leaveID)
        {
            string query = "SELECT LeaveType, StartDate, EndDate, Status FROM LeaveRequests WHERE LeaveID = @LeaveID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LeaveID", leaveID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string leaveType = reader["LeaveType"].ToString();
                            DateTime startDate = (DateTime)reader["StartDate"];
                            DateTime endDate = (DateTime)reader["EndDate"];
                            string status = reader["Status"].ToString();
                            return $"Leave Status: {status} (Type: {leaveType}, Dates: {startDate.ToShortDateString()} to {endDate.ToShortDateString()})";
                        }
                        else
                        {
                            return "Leave request not found.";
                        }
                    }
                }
            }
        }

        // Method to view remaining leaves
        public void ViewRemainingLeaves()
{
    string query = "SELECT AnnualLeaveBalance, CasualLeaveBalance, ShortLeaveBalance FROM Employees WHERE EmployeeID = @EmployeeID";
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@EmployeeID", EmployeeID); // Replace EmployeeID with the current employee's ID

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    int annualLeaveBalance = reader.GetInt32(0); // Get the AnnualLeaveBalance
                    int casualLeaveBalance = reader.GetInt32(1); // Get the CasualLeaveBalance
                    int shortLeaveBalance = reader.GetInt32(2);  // Get the ShortLeaveBalance

                    // Display the remaining leave balances
                    Console.WriteLine($"Remaining Annual Leave: {annualLeaveBalance}");
                    Console.WriteLine($"Remaining Casual Leave: {casualLeaveBalance}");
                    Console.WriteLine($"Remaining Short Leave: {shortLeaveBalance}");
                }
                else
                {
                    Console.WriteLine("Employee not found.");
                }
            }
        }
    }
}


        // Method to delete leave request
        public string DeleteLeave(int leaveID)
{
    string query = "DELETE FROM LeaveRequests WHERE LeaveID = @LeaveID AND Status = 'Pending'";
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@LeaveID", leaveID);

            // Execute the DELETE query
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                // Retrieve the leave type for the deleted leave request
                string leaveType = GetLeaveType(leaveID);

                // Update the in-memory balance
                if (leaveType == "Casual")
                    CasualLeaveBalance++;
                else if (leaveType == "Annual")
                    AnnualLeaveBalance++;
                else if (leaveType == "Short")
                    ShortLeaveBalance++;

                // Persist the updated leave balance in the database
                string updateBalanceQuery = "UPDATE Employees SET CasualLeaveBalance = @CasualLeaveBalance, AnnualLeaveBalance = @AnnualLeaveBalance, ShortLeaveBalance = @ShortLeaveBalance WHERE EmployeeID = @EmployeeID";
                using (SqlCommand updateCommand = new SqlCommand(updateBalanceQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@CasualLeaveBalance", CasualLeaveBalance);
                    updateCommand.Parameters.AddWithValue("@AnnualLeaveBalance", AnnualLeaveBalance);
                    updateCommand.Parameters.AddWithValue("@ShortLeaveBalance", ShortLeaveBalance);
                    updateCommand.Parameters.AddWithValue("@EmployeeID", EmployeeID); // Ensure EmployeeID is available

                    updateCommand.ExecuteNonQuery(); // Update leave balances in the database
                }

                return "Leave request deleted successfully.";
            }
            else
            {
                return "Cannot delete leave request. It might already be approved.";
            }
        }
    }
}


        // Helper method to get leave type for a given leave ID
        private string GetLeaveType(int leaveID)
        {
            string query = "SELECT LeaveType FROM LeaveRequests WHERE LeaveID = @LeaveID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LeaveID", leaveID);
                    return command.ExecuteScalar().ToString();
                }
            }
        }
    }
}
