using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class CostCenter
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string CostCenterCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string CostCenterDesc{ get; set; }

        [Required]
        [ForeignKey("StatusTypeId")]
        public virtual StatusType StatusType { get; set; }
        public int StatusTypeId { get; set; }

    }

    public class CostCenterDTO
    {

        public int Id { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterDesc { get; set; }

        public int StatusTypeId { get; set; }
        public string StatusType { get; set; }

    }

    public class CostCenterVM
    {
        public int Id { get; set; }
        public string CostCenterCode { get; set; }


    }
}
