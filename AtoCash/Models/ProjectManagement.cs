using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtoCash.Models
{
    public class ProjectManagement
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public int ProjectId { get; set; }


        [Required]
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }

    }

    public class ProjectManagementDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

    }





    public class GetEmployeesForProject
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool isAssigned { get; set; }

    }

    public class AddEmployeesToProjectId
    {
        public int ProjectId { get; set; }
        public List<int> EmployeeIds { get; set; }

    }
}
