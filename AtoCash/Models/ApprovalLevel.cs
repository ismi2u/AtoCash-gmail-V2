using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public class ApprovalLevel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Level { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string LevelDesc { get; set; }
    }

    public class ApprovalLevelDTO
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string LevelDesc { get; set; }
    }
}
