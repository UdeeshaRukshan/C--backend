using System;
using MySql.Data.MySqlClient;

public class Admin
{
    private string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=assignment;";

    private string username = "admin";
    private string password = "admin123"; 

    // Admin Login Method
    public bool Login(string inputUsername, string inputPassword)
    {
        return inputUsername == username && inputPassword == password;
    }

    // Register New Employee Method
    public void RegisterEmployee(string name, string department, string username, string password, string designation, decimal salary, string location, string email, string phone, string address, bool isPermanent)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Employees (Name, Department,Username,Password, Designation, Salary, Location, Email, Phone, Address, Is_Permanent) " +
                               "VALUES (@Name, @Department,@Username, @Password, @Designation, @Salary, @Location, @Email, @Phone, @Address, @IsPermanent)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Department", department);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Designation", designation);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@Location", location);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@IsPermanent", isPermanent);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Employee registered successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // Define Leave Balances
    public void SetLeaveBalances(int employeeId, int annualLeave, int casualLeave, int shortLeave)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Employees SET AnnualLeaveBalance = @AnnualLeave, CasualLeaveBalance = @CasualLeave, ShortLeaveBalance = @ShortLeave WHERE id = @EmployeeID";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                command.Parameters.AddWithValue("@AnnualLeave", annualLeave);
                command.Parameters.AddWithValue("@CasualLeave", casualLeave);
                command.Parameters.AddWithValue("@ShortLeave", shortLeave);

                command.ExecuteNonQuery();
                Console.WriteLine("Leave balances updated successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // Define Roaster Times for Employees
    public void SetRoasterTime(int employeeId, DateTime startTime, DateTime endTime)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Employees SET RoasterStartTime = @StartTime, RoasterEndTime = @EndTime WHERE id = @EmployeeID";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                command.Parameters.AddWithValue("@StartTime", startTime);
                command.Parameters.AddWithValue("@EndTime", endTime);

                command.ExecuteNonQuery();
                Console.WriteLine("Roaster times updated successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // View All Employees
    public void ViewAllEmployees()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Name, Department, Designation FROM Employees";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID:  Name: {reader["Name"]}, Department: {reader["Department"]}, Designation: {reader["Designation"]}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // Approve or Reject Leave Requests
    public void HandleLeaveRequest(int LeaveID, bool approve)
    {
        try
        {
            int leaveId = LeaveID;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT LeaveID FROM LeaveRequest WHERE LeaveID = @EmployeeID";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", LeaveID);

                object result = command.ExecuteScalar();
                if (result == null)
                {
                    Console.WriteLine("No leave request found for the provided ");
                    return;
                }

                leaveId = Convert.ToInt32(result);
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE LeaveRequest SET Status = @Status WHERE LeaveID = @LeaveID";

                MySqlCommand command = new MySqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@LeaveID", leaveId);
                command.Parameters.AddWithValue("@Status", approve ? "Approved" : "Rejected");
                command.ExecuteNonQuery();

                Console.WriteLine($"Leave request ID {leaveId} has been {(approve ? "approved" : "rejected")}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // View Leave History for an Employee
    public void ViewEmployeeLeaveHistory(int employeeId, DateTime startDate, DateTime endDate)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT LeaveDate, LeaveType, Status FROM LeaveRequest WHERE EmployeeID = @EmployeeID ";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine($"Leave history for Employee ID {employeeId} from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Date: {reader["LeaveDate"]}, Type: {reader["LeaveType"]}, Status: {reader["Status"]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No leave history found for the given date range.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // View Leave History for All Employees
    public void ViewAllEmployeesLeaveHistory(DateTime startDate, DateTime endDate)
{
    try
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT EmployeeID, LeaveID,LeaveDate, LeaveType, Status FROM LeaveRequest ";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Console.WriteLine($"Leave history from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Employee ID: {reader["EmployeeID"]},LeaveID:{reader["LeaveID"]} Date: {reader["LeaveDate"]}, Type: {reader["LeaveType"]}, Status: {reader["Status"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No leave history found for the given date range.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

         
}    

    

