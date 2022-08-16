using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class ApprovalRoleMap
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ApprovalGroupId")]
        public virtual ApprovalGroup ApprovalGroup { get; set; }
        public int ApprovalGroupId { get; set; }

        [Required]
        [ForeignKey("RoleId")]
        public virtual JobRole JobRole { get; set; }
        public int RoleId { get; set; }

        [Required]
        [ForeignKey("ApprovalLevelId")]
        public virtual ApprovalLevel ApprovalLevel { get; set; }
        public int ApprovalLevelId { get; set; }
    }

    public class ApprovalRoleMapDTO
    {
        public int Id { get; set; }

        public string ApprovalGroup { get; set; }
        public int ApprovalGroupId { get; set; }
        public string Role { get; set; }
        public int RoleId { get; set; }

        public int ApprovalLevel { get; set; }
        public int ApprovalLevelId { get; set; }

        public string EmployeeName { get; set; }
    }
}
