using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class ExpenseCategory
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ExpenseCategoryName { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string ExpenseCategoryDesc { get; set; }

        [Required]
        [ForeignKey("StatusTypeId")]
        public virtual StatusType StatusType { get; set; }
        public int StatusTypeId { get; set; }

    }


    public class ExpenseCategoryDTO
    {

        public int Id { get; set; }
        public string ExpenseCategoryName { get; set; }
        public string ExpenseCategoryDesc { get; set; }
        public string StatusType { get; set; }
        public int StatusTypeId { get; set; }

    }


    public class ExpenseCategoryVM
    {
        public int Id { get; set; }
        public string ExpenseCategoryName { get; set; }

    }
}
