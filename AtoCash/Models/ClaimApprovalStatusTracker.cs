using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public class ClaimApprovalStatusTracker
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }



        [ForeignKey("PettyCashRequestId")]
        public virtual PettyCashRequest PettyCashRequest { get; set; }
        public int? PettyCashRequestId { get; set; }

        //Approver Department
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public int? DepartmentId { get; set; }

        //Approver Project (either Department or Project => Can't be both)
        [ForeignKey("ProjManagerId")]
        public virtual Employee ProjManager { get; set; }
        public int? ProjManagerId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public int? ProjectId { get; set; }


        [ForeignKey("SubProjectId")]
        public virtual SubProject SubProject { get; set; }
        public int? SubProjectId { get; set; }


        [ForeignKey("WorkTaskId")]
        public virtual WorkTask WorkTask { get; set; }
        public int? WorkTaskId { get; set; }

        [Required]
        public int ApprovalGroupId { get; set; }

        //Approver Role
        [Required]
        [ForeignKey("RoleId")]
        public virtual JobRole Role { get; set; }
        public int RoleId { get; set; }

        //Approver ApprovalLevel
        [Required]
        [ForeignKey("ApprovalLevelId")]
        public virtual ApprovalLevel ApprovalLevel { get; set; }
        public int ApprovalLevelId { get; set; }


        [Required]
        public DateTime ReqDate { get; set; }

        public DateTime? FinalApprovedDate { get; set; }

        [Required]
        [ForeignKey("ApprovalStatusTypeId")]
        public virtual ApprovalStatusType ApprovalStatusType { get; set; }
        public int ApprovalStatusTypeId { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string Comments { get; set; }
    }


    public class ClaimApprovalStatusTrackerDTO
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public int? PettyCashRequestId { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }

        public int? SubProjectId { get; set; }
        public string SubProjectName { get; set; }

        public int? WorkTaskId { get; set; }
        public string WorkTask { get; set; }


        public int ApprovalGroupId { get; set; }

        public int RoleId { get; set; }
        public string JobRole { get; set; }

        public int ApprovalLevelId { get; set; }

        public DateTime ReqDate { get; set; }

        public DateTime? FinalApprovedDate { get; set; }

        public int ApprovalStatusTypeId { get; set; }
        public string ApprovalStatusType { get; set; }

        public Double ClaimAmount { get; set; }
        public string Comments { get; set; }

    }
}
