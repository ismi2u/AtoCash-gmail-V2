using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class SubProject
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public int ProjectId { get; set; }

        [Required]
        [Column(TypeName = "varchar(25)")]
        public string SubProjectName { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string SubProjectDesc { get; set; }
    }

    public class SubProjectDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string SubProjectName { get; set; }
        public string SubProjectDesc { get; set; }
    }


    public class SubProjectVM
    {
        public int Id { get; set; }
        public string SubProjectName { get; set; }
    }
}
