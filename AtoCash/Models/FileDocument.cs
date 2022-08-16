using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace AtoCash.Models
{
    public class FileDocument
    {


        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UniqueFileName { get; set; }
        
        [Required]
        public string ActualFileName { get; set; }

    }

    public class FileDocumentDTO
    {

        public int Id { get; set; }

        //public string GeneratedFileName { get; set; }

        public string ActualFileName { get; set; }

    }

}
