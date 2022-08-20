using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class BusinessArea
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string BusinessAreaCode { get; set; }
        [Required]
        [Column(TypeName = "varchar(150)")]
        public string BusinessAreaName { get; set; }

        [Required]
        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }
        public int CostCenterId { get; set; }


        [Required]
        [ForeignKey("StatusTypeId")]
        public virtual StatusType StatusType { get; set; }
        public int StatusTypeId { get; set; }
    }

    public class StoreDTO
    {

        public int Id { get; set; }

        public string StoreCode { get; set; }

        public string StoreName { get; set; }

        public int CostCenterId { get; set; }

        public string CostCenter { get; set; }

        public string StatusType { get; set; }

        public int StatusTypeId { get; set; }

    }

    public class StoreVM
    {

        public int Id { get; set; }

        public string StoreName { get; set; }
        public string StoreDesc { get; set; }

    }
}
