using AtoCash.Authentication;
using AtoCash.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Data
{
    public class AtoCashDbContext : IdentityDbContext<ApplicationUser>
    {

        public AtoCashDbContext(DbContextOptions<AtoCashDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<StatusType>().HasData(
                new StatusType { Id = 1, Status = "Active" },
                 new StatusType { Id = 2, Status = "Inactive" }
                );


            builder.Entity<EmploymentType>().HasData(
               new EmploymentType { Id = 1, EmpJobTypeCode = "FT01", EmpJobTypeDesc = "Full Time Employee" },
                new EmploymentType { Id = 2, EmpJobTypeCode = "PT01", EmpJobTypeDesc = "Part Time Employee" }
               );



            builder.Entity<CurrencyType>().HasData(
             new CurrencyType { Id = 1, CurrencyCode = "SAR", CurrencyName = "Saudi Riyal", Country = "Saudi", StatusTypeId = 1 },
              new CurrencyType { Id = 2, CurrencyCode = "AED", CurrencyName = "UAE", Country = "Emirian", StatusTypeId = 1 },
              new CurrencyType { Id = 3, CurrencyCode = "INR", CurrencyName = "Indian Rupees", Country = "Indian", StatusTypeId = 1 },
              new CurrencyType { Id = 4, CurrencyCode = "CAD", CurrencyName = "Canadian Dollar", Country = "Canadian", StatusTypeId = 1 }
             );




            builder.Entity<ApprovalLevel>().HasData(
          new ApprovalLevel { Id = 1, Level = 1, LevelDesc = "Level 1" },
          new ApprovalLevel { Id = 2, Level = 2, LevelDesc = "Level 2" },
          new ApprovalLevel { Id = 3, Level = 3, LevelDesc = "Level 3" },
          new ApprovalLevel { Id = 4, Level = 4, LevelDesc = "Level 4" }
          );



            builder.Entity<CostCenter>().HasData(
           new CostCenter { Id = 1, CostCenterCode = "ADM", CostCenterDesc = "Administration", StatusTypeId = 1 }
           );



            builder.Entity<Department>().HasData(
           new Department { Id = 1, DeptCode = "ADM", DeptName = "Administration", CostCenterId = 1, StatusTypeId = 1 }
           );

            builder.Entity<JobRole>().HasData(
         new JobRole { Id = 1, RoleCode = "SETUP-ROLE", RoleName = "SETUP-ROLE", IsStoreRole = false, MaxPettyCashAllowed = 0 });

            builder.Entity<BusinessArea>().HasData(
          new BusinessArea { Id = 1, BusinessAreaCode = "SETUP-BUSSAREA", BusinessAreaName = "SETUP-BUSSAREA", CostCenterId = 1, StatusTypeId = 1 }
          );

            builder.Entity<ApprovalGroup>().HasData(
           new ApprovalGroup { Id = 1, ApprovalGroupCode = "SETUP-ADMIN", ApprovalGroupDesc = "SETUP-ADMIN" });

            builder.Entity<Employee>().HasData(
                     new Employee
                     {
                         Id = 1,
                         FirstName = "Atominos",
                         MiddleName = "AtoCash",
                         LastName = "Admin",
                         EmpCode = "EMP000",
                         BankAccount = "1234567890",
                         BankCardNo = "1234222222001234",
                         NationalID = "AAAAAAAAAA",
                         PassportNo = "AAAAAAA",
                         TaxNumber = "1234512345",
                         Nationality = "Indian",
                         DOB = Convert.ToDateTime("06/12/2000"),
                         DOJ = Convert.ToDateTime("06/12/2019"),
                         Gender = "Male",
                         Email = "atocash@gmail.com",
                         MobileNumber = "1234533325",
                         EmploymentTypeId = 1,
                         DepartmentId = 1,
                         BusinessAreaId = 1,
                         RoleId = 1,
                         ApprovalGroupId = 1,
                         BusinessAreaApprovalGroupId = 1,
                         BusinessAreaRoleId = 1,
                         CurrencyTypeId = 3,
                         StatusTypeId = 1
                     });


            builder.Entity<EmpCurrentPettyCashBalance>().HasData(
                     new EmpCurrentPettyCashBalance { Id = 1, EmployeeId = 1, CurBalance = 0, CashOnHand = 0, UpdatedOn = Convert.ToDateTime("06/12/2022") });
                    

            builder.Entity<ApprovalStatusType>().HasData(
                    new ApprovalStatusType { Id = 1, Status = "Initiating", StatusDesc = "Request Initiated" },
                    new ApprovalStatusType { Id = 2, Status = "Pending", StatusDesc = "Awaiting Approval" },
                    new ApprovalStatusType { Id = 3, Status = "In Review", StatusDesc = "Request is in progress" },
                    new ApprovalStatusType { Id = 4, Status = "Approved", StatusDesc = "Request Approved" },
                    new ApprovalStatusType { Id = 5, Status = "Rejected", StatusDesc = "Request is Rejected" });

            builder.Entity<RequestType>().HasData(
                new RequestType { Id = 1,  RequestName = "Petty Cash Request",  RequestTypeDesc = "Petty Cash Request"},
                 new RequestType { Id = 2, RequestName = "Department Expense Reimbursement", RequestTypeDesc = "Department Expense Reimbursement" },
                  new RequestType { Id = 3, RequestName = "Store Expense Reimbursement", RequestTypeDesc = "Store Expense Reimbursement" }
                );


    }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ApprovalGroup> ApprovalGroups { get; set; }
        public DbSet<ApprovalRoleMap> ApprovalRoleMaps { get; set; }
        public DbSet<JobRole> JobRoles { get; set; }

        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<PettyCashRequest> PettyCashRequests { get; set; }

        public DbSet<RequestType> RequestTypes { get; set; }

        public DbSet<DisbursementsAndClaimsMaster> DisbursementsAndClaimsMasters { get; set; }

        public DbSet<EmpCurrentPettyCashBalance> EmpCurrentPettyCashBalances { get; set; }

        public DbSet<ClaimApprovalStatusTracker> ClaimApprovalStatusTrackers { get; set; }

        public DbSet<ApprovalStatusType> ApprovalStatusTypes { get; set; }

        public DbSet<ExpenseReimburseRequest> ExpenseReimburseRequests { get; set; }

        public DbSet<ExpenseReimburseStatusTracker> ExpenseReimburseStatusTrackers { get; set; }

        public DbSet<ExpenseSubClaim> ExpenseSubClaims { get; set; }

        public DbSet<BusinessArea> BusinessAreas { get; set; }
        public DbSet<Project> Projects { get; set; }

        public DbSet<SubProject> SubProjects { get; set; }

        public DbSet<WorkTask> WorkTasks { get; set; }

        public DbSet<ProjectManagement> ProjectManagements { get; set; }

        public DbSet<GeneralLedger> GeneralLedger { get; set; }

        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        public DbSet<ExpenseType> ExpenseTypes { get; set; }

        public DbSet<ApprovalLevel> ApprovalLevels { get; set; }

        public DbSet<TravelApprovalRequest> TravelApprovalRequests { get; set; }

        public DbSet<TravelApprovalStatusTracker> TravelApprovalStatusTrackers { get; set; }

        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<FileDocument> FileDocuments { get; set; }
        public DbSet<CurrencyType> CurrencyTypes { get; set; }

    }
}
