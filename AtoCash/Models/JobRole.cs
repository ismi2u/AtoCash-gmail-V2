using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public class JobRole
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string RoleCode { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string RoleName { get; set; }

        [Required]
        public bool IsStoreRole { get; set; }

        [Required]
        public Double MaxPettyCashAllowed { get; set; }

    }

    public class JobRoleDTO
    {

        public int Id { get; set; }

        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public bool IsStoreRole { get; set; }
        public Double MaxPettyCashAllowed { get; set; }

    }





    public class JobRoleVM
    {

        public int Id { get; set; }
        public string RoleCode { get; set; }

    }


}
