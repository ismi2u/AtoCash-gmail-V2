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
        public DbSet<Project> Projects { get; set; }

        public DbSet<SubProject> SubProjects { get; set; }

        public DbSet<WorkTask> WorkTasks { get; set; }

        public DbSet<ProjectManagement> ProjectManagements { get; set; }

        public DbSet<ExpenseType> ExpenseTypes { get; set; }

        public DbSet<ApprovalLevel> ApprovalLevels { get; set; }

        public DbSet<TravelApprovalRequest> TravelApprovalRequests { get; set; }

        public DbSet<TravelApprovalStatusTracker> TravelApprovalStatusTrackers { get; set; }

        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<FileDocument> FileDocuments { get; set; }
        public DbSet<CurrencyType> CurrencyTypes { get; set; }

    }
}
