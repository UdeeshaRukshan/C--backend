USE assignment;

-- Create Employees Table
CREATE TABLE Employees (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    username VARCHAR(100) UNIQUE,
    password VARCHAR(100) NOT NULL,
    department VARCHAR(100),
    designation VARCHAR(100),
    salary VARCHAR(50),
    location VARCHAR(100),
    email VARCHAR(100) UNIQUE,
    phone VARCHAR(20),
    address VARCHAR(255),
    is_permanent BOOLEAN,
    AnnualLeaveBalance INT DEFAULT 0,
    CasualLeaveBalance INT DEFAULT 0,
    ShortLeaveBalance INT DEFAULT 0,
    RoasterStartTime DATETIME,
    RoasterEndTime DATETIME
);

-- Create LeaveRequest Table
CREATE TABLE LeaveRequest (
    LeaveID INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeID INT NOT NULL,
    LeaveType VARCHAR(10) NOT NULL,
    LeaveStartDate DATE NOT NULL,
    LeaveEndDate DATE NOT NULL,
    LeaveDuration INT,  
    LeaveDate DATE,     
    Status VARCHAR(10) DEFAULT 'Pending',
    FOREIGN KEY (EmployeeID) REFERENCES Employees(id),
    CONSTRAINT CHK_LeaveType CHECK (LeaveType IN ('Annual', 'Casual', 'Short')),
    CONSTRAINT CHK_Status CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    CONSTRAINT CHK_LeaveDates CHECK (LeaveEndDate >= LeaveStartDate)
);

-- Create LeaveHistory Table
CREATE TABLE LeaveHistory (
    HistoryID INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeID INT NOT NULL,
    LeaveType VARCHAR(10) NOT NULL,
    LeaveStartDate DATE NOT NULL,
    LeaveEndDate DATE NOT NULL,
    LeaveDuration INT, -- Assuming the duration is in days
    Status VARCHAR(10) NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(id),
    CONSTRAINT CHK_HistoryLeaveType CHECK (LeaveType IN ('Annual', 'Casual', 'Short')),
    CONSTRAINT CHK_HistoryStatus CHECK (Status IN ('Approved', 'Rejected')),
    CONSTRAINT CHK_HistoryDates CHECK (LeaveEndDate >= LeaveStartDate)
);
