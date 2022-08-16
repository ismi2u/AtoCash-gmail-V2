using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtoCash.Data;
using AtoCash.Models;
using Microsoft.AspNetCore.Authorization;
using AtoCash.Authentication;

namespace AtoCash.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class ProjectManagementController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public ProjectManagementController(AtoCashDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectManagement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectManagementDTO>>> GetProjectManagement()
        {
            List<ProjectManagementDTO> ListProjectManagementDTO = new();

            var projManagements = await _context.ProjectManagements.ToListAsync();

            foreach (ProjectManagement projMgt in projManagements)
            {
                ProjectManagementDTO projectmgmtDTO = new()
                {
                    Id = projMgt.Id,
                    ProjectId = projMgt.ProjectId,
                    ProjectName = _context.Projects.Find(projMgt.ProjectId).ProjectName,
                    EmployeeId = projMgt.EmployeeId,
                    EmployeeName = _context.Employees.Find(projMgt.EmployeeId).GetFullName()
                };

                ListProjectManagementDTO.Add(projectmgmtDTO);

            }

            return ListProjectManagementDTO;
        }

        // GET: api/ProjectManagement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectManagementDTO>> GetProjectManagement(int id)
        {
            ProjectManagementDTO projManagementDTO = new();

            var projManagement = await _context.ProjectManagements.FindAsync(id);

            if (projManagement == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "PROJ Management Id is Invalid!" });
            }
            await Task.Run(() =>
            {
                projManagementDTO.Id = projManagement.Id;
                projManagementDTO.ProjectId = projManagement.ProjectId;
                projManagementDTO.EmployeeId = projManagement.EmployeeId;
            });
            return projManagementDTO;
        }


        [HttpGet("{id}")]
        [ActionName("GetProjectsByEmployee")]
        public async Task<ActionResult<IEnumerable<ProjectVM>>> GetProjectsByEmployee(int id)
        {

            var ProjMgmtItems = _context.ProjectManagements.Include("Projects").Where(p => p.EmployeeId == id).Select(s => new { s.ProjectId, s.Project.ProjectName, s.Project.ProjectDesc });
            List<ProjectVM> ListProjectVM = new();

            await Task.Run(() =>
            {
                foreach (var ProjMgmt in ProjMgmtItems)
                {

                    ProjectVM projectVM = new()
                    {
                        Id = ProjMgmt.ProjectId,
                        ProjectName = ProjMgmt.ProjectName

                    };

                    ListProjectVM.Add(projectVM);
                }
            });
         
            return ListProjectVM;

        }


        /// <summary>
        /// Get employees by Project Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if the employee is assigned to the project or Not </returns>

        [HttpGet("{id}")]
        [ActionName("GetEmployeesByProjectId")]
        public async Task<ActionResult<List<GetEmployeesForProject>>> GetEmployeesByProjectId(int id)
        {
            //var ProjMgmtItems = await _context.ProjectManagements.Where(p => p.ProjectId == id).ToListAsync();
            List<GetEmployeesForProject> ListProjectEmployees = new();

            var allEmployees = await _context.Employees.ToListAsync();

            await Task.Run(() =>
            {
                foreach (Employee emp in allEmployees)
                {
                    GetEmployeesForProject projEmployee = new();

                    projEmployee.EmployeeId = emp.Id;
                    projEmployee.EmployeeName = _context.Employees.Find(emp.Id).GetFullName();
                    projEmployee.isAssigned = _context.ProjectManagements.Where(p => p.EmployeeId == emp.Id && p.ProjectId == id).Any();

                    ListProjectEmployees.Add(projEmployee);
                }
            });
            return Ok(ListProjectEmployees);

        }


        [HttpPost]
        [ActionName("AddEmployeesToProject")]
        public async Task<ActionResult> AddEmployeesToProject(AddEmployeesToProjectId model)
        {

            int projId = model.ProjectId;
            var project = _context.Projects.Find(projId);

           if(projId == 0 || project == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "ProjectId is Invalid" });
            }

           //remove previous entries.
           List<ProjectManagement> ProjMgmtItems = await _context.ProjectManagements.Where(p => p.ProjectId == projId).ToListAsync();
            _context.ProjectManagements.RemoveRange(ProjMgmtItems);


            //add new entries
            foreach(var empid in model.EmployeeIds)
            {
                ProjectManagement projectManagement = new();
                projectManagement.EmployeeId = empid;
                projectManagement.ProjectId = projId;

                _context.ProjectManagements.Add(projectManagement);
            }

            await _context.SaveChangesAsync();
            return Ok(new RespStatus { Status = "Success", Message = "Project assigned to Employees!" });

        }

        // PUT: api/ProjectManagement/5
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutProjectManagement(int id, ProjectManagementDTO projectManagementDto)
        {
            if (id != projectManagementDto.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }


            var projMgmt = await _context.ProjectManagements.FindAsync(id);

            projMgmt.Id = projectManagementDto.Id;
            projMgmt.ProjectId = projectManagementDto.ProjectId;
            projMgmt.EmployeeId = projectManagementDto.EmployeeId;

            _context.ProjectManagements.Update(projMgmt);
            //_context.Entry(projectDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Project Management data Updated!" });
        }

        // POST: api/ProjectManagement
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<ProjectManagement>> PostProjectManagement(ProjectManagementDTO projectManagementDTO)
        {

            var projassigned = _context.ProjectManagements.Where(p => p.EmployeeId == projectManagementDTO.EmployeeId && p.ProjectId == projectManagementDTO.ProjectId).FirstOrDefault();

            if (projassigned !=null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee is already assigned to the Project" });
            }

            ProjectManagement projectManagement = new()
            {
                ProjectId = projectManagementDTO.ProjectId,
                EmployeeId = projectManagementDTO.EmployeeId
            };


            _context.ProjectManagements.Add(projectManagement);
            await _context.SaveChangesAsync();


            return Ok(new RespStatus { Status = "Success", Message = "Employee is assigned to the Project" });
        }

        // DELETE: api/ProjectManagement/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteProjectManagement(int id)
        {
            var projectManagement = await _context.ProjectManagements.FindAsync(id);
            if (projectManagement == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is Invalid!" });
            }

            _context.ProjectManagements.Remove(projectManagement);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Project Maangement Item Deleted!" });
        }

     
    }
}
