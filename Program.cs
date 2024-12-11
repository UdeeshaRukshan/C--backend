using System;
using System.Linq;

partial class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to the Employee Management System");
        
        // Ask for user type (Admin or Employee)
        Console.Write("Are you an Admin or Employee? (Enter 'Admin' or 'Employee'): ");
        string userType = Console.ReadLine().Trim().ToLower();

        if (userType == "admin")
        {
            Admin admin = new Admin();
            AdminLogin(admin);
        }
        else if (userType == "employee")
        {
            EmployeeLogin();
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter 'Admin' or 'Employee'.");
        }
    }

    static void AdminLogin(Admin admin)
    {
        // Admin credentials
        Console.WriteLine("\nAdmin Login");
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        // Admin Login
        if (admin.Login(username, password))
        {
            Console.WriteLine("Login successful!");
            ManageAdminTasks(admin);
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }

    static void EmployeeLogin()
    {
        // Employee login functionality
        Console.WriteLine("\nEmployee Login");
        Console.Write("Enter Employee ID: ");
        string employeeId = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        // For demonstration purposes, assume a valid employee is logged in
        Console.WriteLine("Login successful!");
        ManageEmployeeTasks();
    }

    static void ManageAdminTasks(Admin admin)
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nAdmin Menu:");
            Console.WriteLine("1. Register New Employee");
            Console.WriteLine("2. Set Leave Balances for Employee");
            Console.WriteLine("3. Set Roaster Time for Employee");
            Console.WriteLine("4. View All Employees");
            Console.WriteLine("5. Approve/Reject Leave Request");
            Console.WriteLine("6. View Leave History for Employee");
            Console.WriteLine("7. View Leave History for All Employees");
            Console.WriteLine("8. Exit");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterNewEmployee(admin);
                    break;
                case "2":
                    SetLeaveBalances(admin);
                    break;
                case "3":
                    SetRoasterTime(admin);
                    break;
                case "4":
                    admin.ViewAllEmployees();
                    break;
                case "5":
                    HandleLeaveRequest(admin);
                    break;
                case "6":
                    ViewEmployeeLeaveHistory(admin);
                    break;
                case "7":
                    ViewAllEmployeesLeaveHistory(admin);
                    break;
                case "8":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void ManageEmployeeTasks()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nEmployee Menu:");
            Console.WriteLine("1. Apply for Leave");
            Console.WriteLine("2. View Leave Status");
            Console.WriteLine("3. View Remaining Leave Balances");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ApplyForLeave();
                    break;
                case "2":
                    ViewLeaveStatus();
                    break;
                case "3":
                    ViewRemainingLeaveBalances();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void RegisterNewEmployee(Admin admin)
    {
    Console.WriteLine("\nRegister New Employee:");
    Console.Write("Name: ");
    string name = Console.ReadLine();
    Console.Write("Department: ");
    string department = Console.ReadLine();
    Console.Write("Designation: ");
    string designation = Console.ReadLine();
    Console.Write("Salary: ");

    decimal salary;
    while (!decimal.TryParse(Console.ReadLine(), out salary))
    {
        Console.Write("Invalid input. Please enter a valid salary: ");
    }

    Console.Write("Location: ");
    string location = Console.ReadLine();
    Console.Write("Email: ");
    string email = Console.ReadLine();
    Console.Write("Phone: ");
    string phone = Console.ReadLine();
    Console.Write("Address: ");
    string address = Console.ReadLine();
    Console.Write("Is Permanent (true/false): ");
    bool isPermanent = bool.Parse(Console.ReadLine());

    admin.RegisterEmployee(name, department, designation, salary, location, email, phone, address, isPermanent);
}

    static void SetLeaveBalances(Admin admin)
    {
        Console.WriteLine("\nSet Leave Balances for Employee:");
        Console.Write("Employee Index (starting from 0): ");
        int index = int.Parse(Console.ReadLine());
        Console.Write("Annual Leave: ");
        int annualLeave = int.Parse(Console.ReadLine());
        Console.Write("Casual Leave: ");
        int casualLeave = int.Parse(Console.ReadLine());
        Console.Write("Short Leave: ");
        int shortLeave = int.Parse(Console.ReadLine());

        admin.SetLeaveBalances(index, annualLeave, casualLeave, shortLeave);
    }

 static void SetRoasterTime(Admin admin)
{
    Console.WriteLine("\nSet Roaster Time for Employee:");
    Console.Write("Employee Index (starting from 0): ");
    int index;

    // Validate index input
    while (!int.TryParse(Console.ReadLine(), out index))
    {
        Console.Write("Invalid input. Please enter a valid Employee Index (integer): ");
    }

    DateTime startTime = GetValidDateTime("Start Time (yyyy-MM-dd HH:mm): ");
    DateTime endTime = GetValidDateTime("End Time (yyyy-MM-dd HH:mm): ");

    admin.SetRoasterTime(index, startTime, endTime);
}

// Helper method to validate DateTime input
static DateTime GetValidDateTime(string prompt)
{
    DateTime dateTime;
    while (true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            return dateTime;
        }
        Console.WriteLine("Invalid format. Please enter the date and time in 'yyyy-MM-dd HH:mm' format.");
    }
}

    static void ApplyForLeave()
    {
        // Implement employee leave application
        Console.WriteLine("\nApply for Leave:");
        Console.Write("Enter Leave Type (Casual/Annual/Short): ");
        string leaveType = Console.ReadLine();
        Console.Write("Start Date (yyyy-MM-dd): ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());
        Console.Write("End Date (yyyy-MM-dd): ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());

        // Apply leave (Assume leave is applied successfully for now)
        Console.WriteLine("Leave applied successfully!");
    }

    static void ViewLeaveStatus()
    {
        // Implement viewing leave status
        Console.WriteLine("\nView Leave Status:");
        Console.Write("Enter Leave ID: ");
        int leaveId = int.Parse(Console.ReadLine());

        // View status (Assume status is found for now)
        Console.WriteLine("Leave Status: Pending");
    }

    static void ViewRemainingLeaveBalances()
    {
        // Implement viewing remaining leave balances
        Console.WriteLine("\nRemaining Leave Balances:");
        Console.WriteLine("Annual Leave: 10");
        Console.WriteLine("Casual Leave: 5");
        Console.WriteLine("Short Leave: 2");
    }

    // Admin methods for handling leave requests
    static void HandleLeaveRequest(Admin admin)
    {
        Console.WriteLine("\nApprove/Reject Leave Request:");
        Console.Write("Enter Employee Index (starting from 0): ");
        int employeeIndex = int.Parse(Console.ReadLine());
        Console.Write("Enter Leave Date (yyyy-MM-dd): ");
        DateTime leaveDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Approve or Reject? (true/false): ");
        bool approve = bool.Parse(Console.ReadLine());

        admin.HandleLeaveRequest(employeeIndex, leaveDate, approve);
    }

    static void ViewEmployeeLeaveHistory(Admin admin)
    {
        Console.WriteLine("\nView Leave History for Employee:");
        Console.Write("Enter Employee Index (starting from 0): ");
        int employeeIndex = int.Parse(Console.ReadLine());
        Console.Write("Enter Start Date (yyyy-MM-dd): ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter End Date (yyyy-MM-dd): ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());

        admin.ViewEmployeeLeaveHistory(employeeIndex, startDate, endDate);
    }

    static void ViewAllEmployeesLeaveHistory(Admin admin)
    {
        Console.WriteLine("\nView Leave History for All Employees:");
        Console.Write("Enter Start Date (yyyy-MM-dd): ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter End Date (yyyy-MM-dd): ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());

        admin.ViewAllEmployeesLeaveHistory(startDate, endDate);
    }
}
