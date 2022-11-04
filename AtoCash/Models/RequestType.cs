using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public class RequestType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string RequestName { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string RequestTypeDesc { get; set; }
    }

    public class RequestTypeVM
    {
        public int Id { get; set; }
        public string RequestName { get; set; }

    }
}
