using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public class ExpenseSubClaim
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [ForeignKey("ExpenseReimburseRequestId")]
        public virtual ExpenseReimburseRequest ExpenseReimburseRequest { get; set; }
        public int ExpenseReimburseRequestId { get; set; }

        [Required]
        [ForeignKey("ExpenseCategoryId")]
        public virtual ExpenseCategory ExpenseCategory { get; set; }
        public int ExpenseCategoryId { get; set; }



       

        [Required]
        [ForeignKey("ExpenseTypeId")]
        public virtual ExpenseType ExpenseType { get; set; }
        public int ExpenseTypeId { get; set; }


        public DateTime? ExpStrtDate { get; set; }
 
        public DateTime? ExpEndDate { get; set; }

        public int? ExpNoOfDays { get; set; }

        public string TaxNo { get; set; }


        [Required]
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }

        [Required]
        public Double ExpenseReimbClaimAmount { get; set; }

        public string DocumentIDs { get; set; }


        [Required]
        [Column(TypeName = "varchar(100)")]
        public string InvoiceNo { get; set; }


        [Required]

        public bool IsVAT { get; set; }


        [Required]

        public float Tax { get; set; }

        [Required]
        public double TaxAmount { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }  
        
        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Vendor { get; set; } 
        
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Location { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string Description { get; set; }

        [Required]
        public bool IsBusinessAreaReq { get; set; }

        [ForeignKey("BusinessAreaId")]
        public virtual BusinessArea BusinessArea { get; set; }
        public int? BusinessAreaId { get; set; }

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

        [Required]
        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }
        public int CostCenterId { get; set; }

    }

    public class ExpenseSubClaimDTO
    {

        public int Id { get; set; }

        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }

        
        public int ExpenseCategoryId { get; set; }

        public string ExpenseCategoryName { get; set; }

        public DateTime? ExpStrtDate { get; set; }
        public DateTime? ExpEndDate { get; set; }
        public int? ExpNoOfDays { get; set; }

        public bool IsVAT { get; set; }
        public string TaxNo { get; set; }
        public int ExpenseReimburseReqId { get; set; }
        public Double ExpenseReimbClaimAmount { get; set; }

        public string DocumentIDs { get; set; }

        public DateTime ExpReimReqDate { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public float Tax { get; set; }

        public double TaxAmount { get; set; }

        public string Vendor { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public bool IsBusinessAreaReq { get; set; }
        //Foreign Key Relationsions

        public int CurrencyTypeId { get; set; }
        public string CurrencyType { get; set; }
        public int ExpenseTypeId { get; set; }
        public string ExpenseType { get; set; }

        public int? GeneralLedgerId { get; set; }
        public string GeneralLedger { get; set; }


        public int? BusinessAreaId { get; set; }
        public string BusinessArea { get; set; }

        public string DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectId { get; set; }

        public string SubProjectName  { get; set; }
        public int? SubProjectId { get; set; }

        public string WorkTaskName  { get; set; }
        public int? WorkTaskId { get; set; }

        public int? CostCenterId { get; set; }
        public string CostCenter { get; set; }
        public string ApprovalStatusType { get; set; }
        public int ApprovalStatusTypeId { get; set; }


        public DateTime? ApprovedDate { get; set; }

    }
}
