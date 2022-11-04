using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class GeneralLedger
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string GeneralLedgerAccountNo { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string GeneralLedgerAccountName { get; set; }

        [Required]
        [ForeignKey("StatusTypeId")]
        public virtual StatusType StatusType { get; set; }
        public int StatusTypeId { get; set; }


    }


    public class GeneralLedgerDTO
    {

        public int Id { get; set; }
        public string GeneralLedgerAccountNo { get; set; }
        public string GeneralLedgerAccountName { get; set; }
        public string StatusType { get; set; }
        public int StatusTypeId { get; set; }

    }


    public class GeneralLedgerVM
    {
        public int Id { get; set; }
        public string GeneralLedgerAccountNo { get; set; }

    }
}
