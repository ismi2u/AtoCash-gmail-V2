using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class ApprovalGroup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string ApprovalGroupCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string ApprovalGroupDesc{ get; set; }



    }

    public class ApprovalGroupVM
    {
        public int Id { get; set; }
        public string ApprovalGroupCode { get; set; }
    }

    public class ApprovalGroupDTO
    {
        public int Id { get; set; }

        public string ApprovalGroupCode { get; set; }
        public string ApprovalGroupDesc { get; set; }
    }
}
