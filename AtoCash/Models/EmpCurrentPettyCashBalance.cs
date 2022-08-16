using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public class EmpCurrentPettyCashBalance
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }

        [Required]
        
        public Double CurBalance { get; set; }
        
        [Required]
        public Double CashOnHand { get; set; }

        [Required]
        public DateTime UpdatedOn   { get; set; }

    }


    public class EmpCurrentPettyCashBalanceDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Double CurBalance { get; set; }

        public DateTime UpdatedOn { get; set; }

    }



    public class EmpAllCurBalStatusDTO
    {

        public Double MaxLimit { get; set; }
        public Double CurBalance { get; set; }
        public Double CashInHand { get; set; }

        public Double PendingApproval { get; set; }
        public Double PendingSettlement { get; set; }
        public DateTime WalletBalLastUpdated { get; set; }

    }
}
