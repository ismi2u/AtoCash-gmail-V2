using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class Department
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string DeptCode { get; set; }
        [Required]
        [Column(TypeName = "varchar(150)")]
        public string DeptName { get; set; }

        [Required]
        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }
        public int CostCenterId { get; set; }


        [Required]
        [ForeignKey("StatusTypeId")]
        public virtual StatusType StatusType { get; set; }
        public int StatusTypeId { get; set; }
    }

    public class DepartmentDTO
    {

        public int Id { get; set; }

        public string DeptCode { get; set; }

        public string DeptName { get; set; }

        public int CostCenterId { get; set; }

        public string CostCenter { get; set; }

        public string StatusType { get; set; }

        public int StatusTypeId { get; set; }

    }

    public class DepartmentVM
    {

        public int Id { get; set; }

        public string DeptName { get; set; }
        public string DeptDesc { get; set; }

    }
}
