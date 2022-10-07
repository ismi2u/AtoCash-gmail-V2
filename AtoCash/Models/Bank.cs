using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class Bank
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string BankName { get; set; }
        [Required]
        [Column(TypeName = "varchar(150)")]
        public string BankDesc { get; set; }

        [Required]
        [ForeignKey("StatusTypeId")]
        public virtual StatusType StatusType { get; set; }
        public int StatusTypeId { get; set; }
    }

    public class BankDTO
    {

        public int Id { get; set; }

        public string BankName { get; set; }

        public string BankDesc { get; set; }

        public int StatusTypeId { get; set; }

        public string StatusType { get; set; }

    }

    public class BankVM
    {

        public int Id { get; set; }

        public string BankName { get; set; }


    }
}
