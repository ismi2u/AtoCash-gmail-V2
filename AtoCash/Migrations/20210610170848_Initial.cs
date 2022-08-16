using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AtoCash.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApprovalGroupCode = table.Column<string>(type: "varchar(20)", nullable: false),
                    ApprovalGroupDesc = table.Column<string>(type: "varchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    LevelDesc = table.Column<string>(type: "varchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalStatusTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "varchar(25)", nullable: false),
                    StatusDesc = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStatusTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpJobTypeCode = table.Column<string>(type: "varchar(20)", nullable: false),
                    EmpJobTypeDesc = table.Column<string>(type: "varchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueFileName = table.Column<string>(type: "text", nullable: false),
                    ActualFileName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleCode = table.Column<string>(type: "varchar(20)", nullable: false),
                    RoleName = table.Column<string>(type: "varchar(50)", nullable: false),
                    MaxPettyCashAllowed = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestName = table.Column<string>(type: "varchar(50)", nullable: false),
                    RequestTypeDesc = table.Column<string>(type: "varchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "varchar(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRoleMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApprovalGroupId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRoleMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRoleMaps_ApprovalGroups_ApprovalGroupId",
                        column: x => x.ApprovalGroupId,
                        principalTable: "ApprovalGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalRoleMaps_ApprovalLevels_ApprovalLevelId",
                        column: x => x.ApprovalLevelId,
                        principalTable: "ApprovalLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalRoleMaps_JobRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "JobRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CostCenterCode = table.Column<string>(type: "varchar(20)", nullable: false),
                    CostCenterDesc = table.Column<string>(type: "varchar(150)", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostCenters_StatusTypes_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyCode = table.Column<string>(type: "varchar(3)", nullable: false),
                    CurrencyName = table.Column<string>(type: "varchar(100)", nullable: false),
                    Country = table.Column<string>(type: "varchar(100)", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyTypes_StatusTypes_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseTypeName = table.Column<string>(type: "varchar(50)", nullable: false),
                    ExpenseTypeDesc = table.Column<string>(type: "varchar(150)", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseTypes_StatusTypes_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeptCode = table.Column<string>(type: "varchar(50)", nullable: false),
                    DeptName = table.Column<string>(type: "varchar(150)", nullable: false),
                    CostCenterId = table.Column<int>(type: "integer", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Departments_StatusTypes_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "varchar(200)", nullable: false),
                    MiddleName = table.Column<string>(type: "varchar(100)", nullable: true),
                    LastName = table.Column<string>(type: "varchar(100)", nullable: false),
                    EmpCode = table.Column<string>(type: "varchar(30)", nullable: false),
                    BankAccount = table.Column<string>(type: "varchar(30)", nullable: false),
                    BankCardNo = table.Column<string>(type: "varchar(50)", nullable: true),
                    NationalID = table.Column<string>(type: "text", nullable: true),
                    PassportNo = table.Column<string>(type: "varchar(20)", nullable: true),
                    TaxNumber = table.Column<string>(type: "varchar(20)", nullable: true),
                    Nationality = table.Column<string>(type: "varchar(50)", nullable: true),
                    DOB = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DOJ = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MobileNumber = table.Column<string>(type: "varchar(20)", nullable: true),
                    EmploymentTypeId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalGroupId = table.Column<int>(type: "integer", nullable: false),
                    CurrencyTypeId = table.Column<int>(type: "integer", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_ApprovalGroups_ApprovalGroupId",
                        column: x => x.ApprovalGroupId,
                        principalTable: "ApprovalGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_CurrencyTypes_CurrencyTypeId",
                        column: x => x.CurrencyTypeId,
                        principalTable: "CurrencyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_EmploymentTypes_EmploymentTypeId",
                        column: x => x.EmploymentTypeId,
                        principalTable: "EmploymentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_JobRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "JobRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_StatusTypes_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpCurrentPettyCashBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    CurBalance = table.Column<double>(type: "double precision", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpCurrentPettyCashBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpCurrentPettyCashBalances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "varchar(25)", nullable: false),
                    CostCenterId = table.Column<int>(type: "integer", nullable: false),
                    ProjectManagerId = table.Column<int>(type: "integer", nullable: false),
                    ProjectDesc = table.Column<string>(type: "varchar(200)", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_Employees_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_StatusTypes_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectManagements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectManagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectManagements_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectManagements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SubProjectName = table.Column<string>(type: "varchar(25)", nullable: false),
                    SubProjectDesc = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubProjectId = table.Column<int>(type: "integer", nullable: false),
                    TaskName = table.Column<string>(type: "varchar(25)", nullable: false),
                    TaskDesc = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTasks_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseReimburseRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseReportTitle = table.Column<string>(type: "varchar(250)", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    CurrencyTypeId = table.Column<int>(type: "integer", nullable: false),
                    TotalClaimAmount = table.Column<double>(type: "double precision", nullable: false),
                    ExpReimReqDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    CostCenterId = table.Column<int>(type: "integer", nullable: true),
                    ApprovalStatusTypeId = table.Column<int>(type: "integer", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Comments = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseReimburseRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_ApprovalStatusTypes_ApprovalStatus~",
                        column: x => x.ApprovalStatusTypeId,
                        principalTable: "ApprovalStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_CurrencyTypes_CurrencyTypeId",
                        column: x => x.CurrencyTypeId,
                        principalTable: "CurrencyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseRequests_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PettyCashRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    PettyClaimAmount = table.Column<double>(type: "double precision", nullable: false),
                    PettyClaimRequestDesc = table.Column<string>(type: "varchar(150)", nullable: false),
                    CashReqDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CurrencyTypeId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    CostCenterId = table.Column<int>(type: "integer", nullable: true),
                    ApprovalStatusTypeId = table.Column<int>(type: "integer", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Comments = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PettyCashRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_ApprovalStatusTypes_ApprovalStatusTypeId",
                        column: x => x.ApprovalStatusTypeId,
                        principalTable: "ApprovalStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_CurrencyTypes_CurrencyTypeId",
                        column: x => x.CurrencyTypeId,
                        principalTable: "CurrencyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PettyCashRequests_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TravelApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    TravelStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TravelEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TravelPurpose = table.Column<string>(type: "varchar(150)", nullable: false),
                    ReqRaisedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    CostCenterId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalStatusTypeId = table.Column<int>(type: "integer", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Comments = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelApprovalRequests_ApprovalStatusTypes_ApprovalStatusTy~",
                        column: x => x.ApprovalStatusTypeId,
                        principalTable: "ApprovalStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalRequests_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelApprovalRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelApprovalRequests_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelApprovalRequests_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseReimburseStatusTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    ExpenseReimburseRequestId = table.Column<int>(type: "integer", nullable: false),
                    CurrencyTypeId = table.Column<int>(type: "integer", nullable: false),
                    TotalClaimAmount = table.Column<double>(type: "double precision", nullable: false),
                    ExpReimReqDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjManagerId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    ApprovalGroupId = table.Column<int>(type: "integer", nullable: false),
                    JobRoleId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalLevelId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalStatusTypeId = table.Column<int>(type: "integer", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Comments = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseReimburseStatusTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_ApprovalLevels_ApprovalLevel~",
                        column: x => x.ApprovalLevelId,
                        principalTable: "ApprovalLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_ApprovalStatusTypes_Approval~",
                        column: x => x.ApprovalStatusTypeId,
                        principalTable: "ApprovalStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_CurrencyTypes_CurrencyTypeId",
                        column: x => x.CurrencyTypeId,
                        principalTable: "CurrencyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_Employees_ProjManagerId",
                        column: x => x.ProjManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_ExpenseReimburseRequests_Exp~",
                        column: x => x.ExpenseReimburseRequestId,
                        principalTable: "ExpenseReimburseRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_JobRoles_JobRoleId",
                        column: x => x.JobRoleId,
                        principalTable: "JobRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseReimburseStatusTrackers_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseSubClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseReimburseRequestId = table.Column<int>(type: "integer", nullable: false),
                    ExpenseTypeId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    ExpenseReimbClaimAmount = table.Column<double>(type: "double precision", nullable: false),
                    DocumentIDs = table.Column<string>(type: "text", nullable: true),
                    InvoiceNo = table.Column<string>(type: "varchar(50)", nullable: false),
                    Tax = table.Column<float>(type: "real", nullable: false),
                    TaxAmount = table.Column<double>(type: "double precision", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Vendor = table.Column<string>(type: "varchar(50)", nullable: false),
                    Location = table.Column<string>(type: "varchar(20)", nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    CostCenterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseSubClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_ExpenseReimburseRequests_ExpenseReimburseR~",
                        column: x => x.ExpenseReimburseRequestId,
                        principalTable: "ExpenseReimburseRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_ExpenseTypes_ExpenseTypeId",
                        column: x => x.ExpenseTypeId,
                        principalTable: "ExpenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseSubClaims_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClaimApprovalStatusTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    PettyCashRequestId = table.Column<int>(type: "integer", nullable: true),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjManagerId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    ApprovalGroupId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalLevelId = table.Column<int>(type: "integer", nullable: false),
                    ReqDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FinalApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovalStatusTypeId = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimApprovalStatusTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_ApprovalLevels_ApprovalLevelId",
                        column: x => x.ApprovalLevelId,
                        principalTable: "ApprovalLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_ApprovalStatusTypes_ApprovalSta~",
                        column: x => x.ApprovalStatusTypeId,
                        principalTable: "ApprovalStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_Employees_ProjManagerId",
                        column: x => x.ProjManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_JobRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "JobRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_PettyCashRequests_PettyCashRequ~",
                        column: x => x.PettyCashRequestId,
                        principalTable: "PettyCashRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimApprovalStatusTrackers_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisbursementsAndClaimsMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    PettyCashRequestId = table.Column<int>(type: "integer", nullable: true),
                    ExpenseReimburseReqId = table.Column<int>(type: "integer", nullable: true),
                    RequestTypeId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CurrencyTypeId = table.Column<int>(type: "integer", nullable: false),
                    ClaimAmount = table.Column<double>(type: "double precision", nullable: false),
                    AmountToWallet = table.Column<double>(type: "double precision", nullable: true),
                    AmountToCredit = table.Column<double>(type: "double precision", nullable: true),
                    IsSettledAmountCredited = table.Column<bool>(type: "boolean", nullable: true),
                    SettledDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SettlementComment = table.Column<string>(type: "varchar(150)", nullable: true),
                    SettlementAccount = table.Column<string>(type: "varchar(150)", nullable: true),
                    SettlementBankCard = table.Column<string>(type: "varchar(150)", nullable: true),
                    AdditionalData = table.Column<string>(type: "varchar(150)", nullable: true),
                    CostCenterId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalStatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisbursementsAndClaimsMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_ApprovalStatusTypes_ApprovalS~",
                        column: x => x.ApprovalStatusId,
                        principalTable: "ApprovalStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_CurrencyTypes_CurrencyTypeId",
                        column: x => x.CurrencyTypeId,
                        principalTable: "CurrencyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_ExpenseReimburseRequests_Expe~",
                        column: x => x.ExpenseReimburseReqId,
                        principalTable: "ExpenseReimburseRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_PettyCashRequests_PettyCashRe~",
                        column: x => x.PettyCashRequestId,
                        principalTable: "PettyCashRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_RequestTypes_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "RequestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisbursementsAndClaimsMasters_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TravelApprovalStatusTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    TravelApprovalRequestId = table.Column<int>(type: "integer", nullable: false),
                    TravelStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TravelEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ProjManagerId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    SubProjectId = table.Column<int>(type: "integer", nullable: true),
                    WorkTaskId = table.Column<int>(type: "integer", nullable: true),
                    ApprovalGroupId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalLevelId = table.Column<int>(type: "integer", nullable: false),
                    ReqDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FinalApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovalStatusTypeId = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "varchar(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelApprovalStatusTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_ApprovalLevels_ApprovalLevelId",
                        column: x => x.ApprovalLevelId,
                        principalTable: "ApprovalLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_ApprovalStatusTypes_ApprovalSt~",
                        column: x => x.ApprovalStatusTypeId,
                        principalTable: "ApprovalStatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_Employees_ProjManagerId",
                        column: x => x.ProjManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_JobRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "JobRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_TravelApprovalRequests_TravelA~",
                        column: x => x.TravelApprovalRequestId,
                        principalTable: "TravelApprovalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelApprovalStatusTrackers_WorkTasks_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRoleMaps_ApprovalGroupId",
                table: "ApprovalRoleMaps",
                column: "ApprovalGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRoleMaps_ApprovalLevelId",
                table: "ApprovalRoleMaps",
                column: "ApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRoleMaps_RoleId",
                table: "ApprovalRoleMaps",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_ApprovalLevelId",
                table: "ClaimApprovalStatusTrackers",
                column: "ApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_ApprovalStatusTypeId",
                table: "ClaimApprovalStatusTrackers",
                column: "ApprovalStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_DepartmentId",
                table: "ClaimApprovalStatusTrackers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_EmployeeId",
                table: "ClaimApprovalStatusTrackers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_PettyCashRequestId",
                table: "ClaimApprovalStatusTrackers",
                column: "PettyCashRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_ProjectId",
                table: "ClaimApprovalStatusTrackers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_ProjManagerId",
                table: "ClaimApprovalStatusTrackers",
                column: "ProjManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_RoleId",
                table: "ClaimApprovalStatusTrackers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_SubProjectId",
                table: "ClaimApprovalStatusTrackers",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimApprovalStatusTrackers_WorkTaskId",
                table: "ClaimApprovalStatusTrackers",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_StatusTypeId",
                table: "CostCenters",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyTypes_StatusTypeId",
                table: "CurrencyTypes",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CostCenterId",
                table: "Departments",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_StatusTypeId",
                table: "Departments",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_ApprovalStatusId",
                table: "DisbursementsAndClaimsMasters",
                column: "ApprovalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_CostCenterId",
                table: "DisbursementsAndClaimsMasters",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_CurrencyTypeId",
                table: "DisbursementsAndClaimsMasters",
                column: "CurrencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_DepartmentId",
                table: "DisbursementsAndClaimsMasters",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_EmployeeId",
                table: "DisbursementsAndClaimsMasters",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_ExpenseReimburseReqId",
                table: "DisbursementsAndClaimsMasters",
                column: "ExpenseReimburseReqId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_PettyCashRequestId",
                table: "DisbursementsAndClaimsMasters",
                column: "PettyCashRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_ProjectId",
                table: "DisbursementsAndClaimsMasters",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_RequestTypeId",
                table: "DisbursementsAndClaimsMasters",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_SubProjectId",
                table: "DisbursementsAndClaimsMasters",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementsAndClaimsMasters_WorkTaskId",
                table: "DisbursementsAndClaimsMasters",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCurrentPettyCashBalances_EmployeeId",
                table: "EmpCurrentPettyCashBalances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ApprovalGroupId",
                table: "Employees",
                column: "ApprovalGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CurrencyTypeId",
                table: "Employees",
                column: "CurrencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmploymentTypeId",
                table: "Employees",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RoleId",
                table: "Employees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StatusTypeId",
                table: "Employees",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_ApprovalStatusTypeId",
                table: "ExpenseReimburseRequests",
                column: "ApprovalStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_CostCenterId",
                table: "ExpenseReimburseRequests",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_CurrencyTypeId",
                table: "ExpenseReimburseRequests",
                column: "CurrencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_DepartmentId",
                table: "ExpenseReimburseRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_EmployeeId",
                table: "ExpenseReimburseRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_ProjectId",
                table: "ExpenseReimburseRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_SubProjectId",
                table: "ExpenseReimburseRequests",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseRequests_WorkTaskId",
                table: "ExpenseReimburseRequests",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_ApprovalLevelId",
                table: "ExpenseReimburseStatusTrackers",
                column: "ApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_ApprovalStatusTypeId",
                table: "ExpenseReimburseStatusTrackers",
                column: "ApprovalStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_CurrencyTypeId",
                table: "ExpenseReimburseStatusTrackers",
                column: "CurrencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_DepartmentId",
                table: "ExpenseReimburseStatusTrackers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_EmployeeId",
                table: "ExpenseReimburseStatusTrackers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_ExpenseReimburseRequestId",
                table: "ExpenseReimburseStatusTrackers",
                column: "ExpenseReimburseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_JobRoleId",
                table: "ExpenseReimburseStatusTrackers",
                column: "JobRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_ProjectId",
                table: "ExpenseReimburseStatusTrackers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_ProjManagerId",
                table: "ExpenseReimburseStatusTrackers",
                column: "ProjManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_SubProjectId",
                table: "ExpenseReimburseStatusTrackers",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReimburseStatusTrackers_WorkTaskId",
                table: "ExpenseReimburseStatusTrackers",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_CostCenterId",
                table: "ExpenseSubClaims",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_DepartmentId",
                table: "ExpenseSubClaims",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_EmployeeId",
                table: "ExpenseSubClaims",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_ExpenseReimburseRequestId",
                table: "ExpenseSubClaims",
                column: "ExpenseReimburseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_ExpenseTypeId",
                table: "ExpenseSubClaims",
                column: "ExpenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_ProjectId",
                table: "ExpenseSubClaims",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_SubProjectId",
                table: "ExpenseSubClaims",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSubClaims_WorkTaskId",
                table: "ExpenseSubClaims",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTypes_StatusTypeId",
                table: "ExpenseTypes",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_ApprovalStatusTypeId",
                table: "PettyCashRequests",
                column: "ApprovalStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_CostCenterId",
                table: "PettyCashRequests",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_CurrencyTypeId",
                table: "PettyCashRequests",
                column: "CurrencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_DepartmentId",
                table: "PettyCashRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_EmployeeId",
                table: "PettyCashRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_ProjectId",
                table: "PettyCashRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_SubProjectId",
                table: "PettyCashRequests",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashRequests_WorkTaskId",
                table: "PettyCashRequests",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectManagements_EmployeeId",
                table: "ProjectManagements",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectManagements_ProjectId",
                table: "ProjectManagements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CostCenterId",
                table: "Projects",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectManagerId",
                table: "Projects",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StatusTypeId",
                table: "Projects",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProjects_ProjectId",
                table: "SubProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalRequests_ApprovalStatusTypeId",
                table: "TravelApprovalRequests",
                column: "ApprovalStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalRequests_CostCenterId",
                table: "TravelApprovalRequests",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalRequests_DepartmentId",
                table: "TravelApprovalRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalRequests_EmployeeId",
                table: "TravelApprovalRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalRequests_ProjectId",
                table: "TravelApprovalRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalRequests_SubProjectId",
                table: "TravelApprovalRequests",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalRequests_WorkTaskId",
                table: "TravelApprovalRequests",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_ApprovalLevelId",
                table: "TravelApprovalStatusTrackers",
                column: "ApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_ApprovalStatusTypeId",
                table: "TravelApprovalStatusTrackers",
                column: "ApprovalStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_DepartmentId",
                table: "TravelApprovalStatusTrackers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_EmployeeId",
                table: "TravelApprovalStatusTrackers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_ProjectId",
                table: "TravelApprovalStatusTrackers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_ProjManagerId",
                table: "TravelApprovalStatusTrackers",
                column: "ProjManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_RoleId",
                table: "TravelApprovalStatusTrackers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_SubProjectId",
                table: "TravelApprovalStatusTrackers",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_TravelApprovalRequestId",
                table: "TravelApprovalStatusTrackers",
                column: "TravelApprovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelApprovalStatusTrackers_WorkTaskId",
                table: "TravelApprovalStatusTrackers",
                column: "WorkTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_SubProjectId",
                table: "WorkTasks",
                column: "SubProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRoleMaps");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ClaimApprovalStatusTrackers");

            migrationBuilder.DropTable(
                name: "DisbursementsAndClaimsMasters");

            migrationBuilder.DropTable(
                name: "EmpCurrentPettyCashBalances");

            migrationBuilder.DropTable(
                name: "ExpenseReimburseStatusTrackers");

            migrationBuilder.DropTable(
                name: "ExpenseSubClaims");

            migrationBuilder.DropTable(
                name: "FileDocuments");

            migrationBuilder.DropTable(
                name: "ProjectManagements");

            migrationBuilder.DropTable(
                name: "TravelApprovalStatusTrackers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PettyCashRequests");

            migrationBuilder.DropTable(
                name: "RequestTypes");

            migrationBuilder.DropTable(
                name: "ExpenseReimburseRequests");

            migrationBuilder.DropTable(
                name: "ExpenseTypes");

            migrationBuilder.DropTable(
                name: "ApprovalLevels");

            migrationBuilder.DropTable(
                name: "TravelApprovalRequests");

            migrationBuilder.DropTable(
                name: "ApprovalStatusTypes");

            migrationBuilder.DropTable(
                name: "WorkTasks");

            migrationBuilder.DropTable(
                name: "SubProjects");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "ApprovalGroups");

            migrationBuilder.DropTable(
                name: "CurrencyTypes");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "EmploymentTypes");

            migrationBuilder.DropTable(
                name: "JobRoles");

            migrationBuilder.DropTable(
                name: "CostCenters");

            migrationBuilder.DropTable(
                name: "StatusTypes");
        }
    }
}
