﻿using AtoCash.Authentication;
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
               new EmploymentType { Id = 1,  EmpJobTypeCode= "FT01", EmpJobTypeDesc= "Full Time Emp" },
                new EmploymentType { Id = 2, EmpJobTypeCode = "PT01", EmpJobTypeDesc = "Part Time Emp" }
               );



            builder.Entity<CurrencyType>().HasData(
             new CurrencyType { Id = 1,  CurrencyCode = "SAR",  CurrencyName = "Saudi Riyal", Country="Saudi", StatusTypeId=1 },
              new CurrencyType { Id = 2, CurrencyCode = "AED", CurrencyName = "UAE", Country = "Emirian", StatusTypeId = 1 },
              new CurrencyType { Id = 3, CurrencyCode = "INR", CurrencyName = "Indian Rupees", Country = "Indian", StatusTypeId = 1 },
              new CurrencyType { Id = 4, CurrencyCode = "CAD", CurrencyName = "Canadian Dollar", Country = "Canadian", StatusTypeId = 1 }
             );



            builder.Entity<JobRole>().HasData(
           new JobRole { Id = 1, RoleCode = "DEPT-EMP001", RoleName = "DEPT BASE EMPLOYEE", IsStoreRole = false, MaxPettyCashAllowed = 10000 },
           new JobRole { Id = 2, RoleCode = "DEPT-EMP002", RoleName = "DEPT SUPERVISOR EMPLOYEE", IsStoreRole = false, MaxPettyCashAllowed = 20000 },
           new JobRole { Id = 3, RoleCode = "DEPTMGR", RoleName = "DEPARTMENT MANAGER", IsStoreRole = false, MaxPettyCashAllowed = 50000 },
           new JobRole { Id = 4, RoleCode = "DEPT-FINMGR", RoleName = "DEPARTMENT FINANCE MANAGER", IsStoreRole = false, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 5, RoleCode = "DEPT-FIN HEAD", RoleName = "DEPARTMENT FINANCE HEAD", IsStoreRole = false, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 6, RoleCode = "COIM STORE-MGR001", RoleName = "COIMBTR STORE MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 7, RoleCode = "CHEN STORE-MGR001", RoleName = "CHENNAI STORE MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 8, RoleCode = "MUMB STORE-MGR001", RoleName = "MUMBAI STORE MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 9, RoleCode = "TRUP STORE-MGR001", RoleName = "TIRUPUR STORE MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 10, RoleCode = "MADU STORE-MGR001", RoleName = "MADURAI STORE MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 11, RoleCode = "OOTY STORE-MGR001", RoleName = "OOTY STORE MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 12, RoleCode = "STOR-AREA-MGR-001", RoleName = "AREA-001  MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 13, RoleCode = "STOR-AREA-MGR-002", RoleName = "AREA-002 MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 },
           new JobRole { Id = 14, RoleCode = "STOR OPS-MGR", RoleName = "STORE OPS MANAGER", IsStoreRole = true, MaxPettyCashAllowed = 100000 }
           );

            builder.Entity<ApprovalLevel>().HasData(
          new ApprovalLevel { Id = 1, Level = 1, LevelDesc = "Level 1" },
          new ApprovalLevel { Id = 2, Level = 2, LevelDesc = "Level 2" },
          new ApprovalLevel { Id = 3, Level = 3, LevelDesc = "Level 3" },
          new ApprovalLevel { Id = 4, Level = 4, LevelDesc = "Level 4" }
          );



            builder.Entity<CostCenter>().HasData(
           new CostCenter { Id = 1, CostCenterCode = "CC-DEPT-001", CostCenterDesc = "MFG DEPT COST Centre 001", StatusTypeId =1 },
           new CostCenter { Id = 2, CostCenterCode = "CC-DEPT-002", CostCenterDesc = "MFG DEPT COST Centre 002", StatusTypeId = 1 },
           new CostCenter { Id = 3, CostCenterCode = "CC-STOR-001", CostCenterDesc = "STOR COST Centre 001", StatusTypeId = 1 },
           new CostCenter { Id = 4, CostCenterCode = "CC-STOR-002", CostCenterDesc = "STOR COST Centre 002", StatusTypeId = 1 }

           );



            builder.Entity<Department>().HasData(
           new Department { Id = 1,  DeptCode  = "MAIN MFG DEPT",  DeptName  = "MAIN MFG DEPT ", CostCenterId = 1, StatusTypeId = 1 },
           new Department { Id = 2, DeptCode = "CBE MFG DEPT", DeptName = "COIMBATORE MFG DEPT", CostCenterId = 2, StatusTypeId = 1 },
           new Department { Id = 3, DeptCode = "CBE STORE", DeptName = "COIMBATORE STORE", CostCenterId = 3, StatusTypeId = 1 },
           new Department { Id = 4, DeptCode = "CHENNAI STORE", DeptName = "CHENNAI STORE", CostCenterId = 4, StatusTypeId = 1 }

           );


            builder.Entity<ApprovalGroup>().HasData(
           new ApprovalGroup { Id = 1,  ApprovalGroupCode  = "DEPT-APPRL-GROUP-001",  ApprovalGroupDesc  = "DEPT-APPRL-GROUP-001" },
           new ApprovalGroup { Id = 2, ApprovalGroupCode = "DEPT-APPRL-GROUP-002", ApprovalGroupDesc = "DEPT-APPRL-GROUP-002" },
           new ApprovalGroup { Id = 3, ApprovalGroupCode = "DEPT-APPRL-GROUP-003", ApprovalGroupDesc = "DEPT-APPRL-GROUP-003" },
           new ApprovalGroup { Id = 4, ApprovalGroupCode = "STOR-APPRL-GROUP-001", ApprovalGroupDesc = "STOR-APPRL-GROUP-001" },
           new ApprovalGroup { Id = 5, ApprovalGroupCode = "STOR-APPRL-GROUP-002", ApprovalGroupDesc = "STOR-APPRL-GROUP-002" }

           );



            builder.Entity<BusinessArea>().HasData(
           new BusinessArea { Id = 1, BusinessAreaCode = "COIMBATORE-STOR", BusinessAreaName = "CBE-STORE-GROUP-001", CostCenterId = 3, StatusTypeId = 1 }
           );





            builder.Entity<Employee>().HasData(
                     new Employee { Id = 1,   FirstName = "Irfan",  MiddleName = "H",  LastName = "Rashid",  EmpCode = "EMP001",
                          BankAccount = "SBIN0012345",  BankCardNo =  "SBC001234",  NationalID  = "AADH001243", PassportNo = "MDB12345",
                          TaxNumber = "TIN12345", Nationality="Indian",  DOB = Convert.ToDateTime("06/12/2000"), DOJ = Convert.ToDateTime("06/12/2019"),  Gender="Male", Email="irfan3@gmail.com", MobileNumber="1234533325", 
                         EmploymentTypeId=1,DepartmentId=1, BusinessAreaId=1, RoleId=1, ApprovalGroupId=1,  BusinessAreaApprovalGroupId=4,BusinessAreaRoleId=6,
                          CurrencyTypeId=2, StatusTypeId=1 }
                     );


            builder.Entity<EmpCurrentPettyCashBalance>().HasData(
                     new EmpCurrentPettyCashBalance {  Id =1, EmployeeId=1,  CurBalance=100000, CashOnHand = 0, UpdatedOn= Convert.ToDateTime("06/12/2022") });



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
