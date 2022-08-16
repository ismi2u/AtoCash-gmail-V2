using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public class ExpenseReimburseRequest
    { 

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [Column(TypeName = "varchar(250)")]
        public string ExpenseReportTitle { get; set; }

        [Required]
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }

        [Required]
        [ForeignKey("CurrencyTypeId")]
        public virtual CurrencyType CurrencyType { get; set; }
        public int CurrencyTypeId { get; set; }

        [Required]
        public Double TotalClaimAmount { get; set; }

        [Required]
        public DateTime ExpReimReqDate { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public int? DepartmentId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public int? ProjectId { get; set; }

        [ForeignKey("SubProjectId")]
        public virtual SubProject SubProject { get; set; }
        public int? SubProjectId { get; set; }

        [ForeignKey("WorkTaskId")]
        public virtual WorkTask WorkTask { get; set; }
        public int? WorkTaskId { get; set; }

        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }
        public int? CostCenterId { get; set; }

        [Required]
        [ForeignKey("ApprovalStatusTypeId")]
        public virtual ApprovalStatusType ApprovalStatusType { get; set; }
        public int ApprovalStatusTypeId { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string Comments { get; set; }
    }




    public class ExpenseReimburseRequestDTO
    {
        public int Id { get; set; }
        public string ExpenseReportTitle { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int CurrencyTypeId { get; set; }
        public Double TotalClaimAmount { get; set; }
        public DateTime ExpReimReqDate { get; set; }

        public string DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectId { get; set; }

        public string SubProjectName { get; set; }
        public int? SubProjectId { get; set; }

        public string WorkTaskName { get; set; }
        public int? WorkTaskId { get; set; }
        public int ApprovalStatusTypeId { get; set; }
        public string ApprovalStatusType { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public bool ShowEditDelete { get; set; }
        public List<ExpenseSubClaimDTO> ExpenseSubClaims { get; set; }

        public double? CreditToWallet { get; set; }

        public double? CreditToBank { get; set; }

        public bool IsSettled { get; set; }

        public string Comments { get; set; }


    }
}
