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
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, Manager, User")]
    public class SubProjectsController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public SubProjectsController(AtoCashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ActionName("SubProjectsForDropdown")]
        public async Task<ActionResult<IEnumerable<SubProjectVM>>> GetSubProjectsForDropDown()
        {
            List<SubProjectVM> ListSubProjectVM = new();

            var subProjects = await _context.SubProjects.ToListAsync();
            foreach (SubProject subProject in subProjects)
            {
                SubProjectVM subProjectVM = new()
                {
                    Id = subProject.Id,
                    SubProjectName = subProject.SubProjectName + ":" + subProject.SubProjectDesc
                };

                ListSubProjectVM.Add(subProjectVM);
            }

            return ListSubProjectVM;

        }


        [HttpGet("{id}")]
        [ActionName("GetSubProjectsForProjects")]
        public async Task<ActionResult<IEnumerable<SubProjectVM>>> GetSubProjectsForProjects(int id)
        {
            var listOfSubProject = await _context.SubProjects.Where(s => s.ProjectId == id).ToListAsync();

            List<SubProjectVM> ListSubProjectVM = new();

            if (listOfSubProject != null)
            {
                foreach (var item in listOfSubProject)
                {
                    SubProjectVM subproject = new()
                    {
                        Id = item.Id,
                        SubProjectName = item.SubProjectName
                    };
                    ListSubProjectVM.Add(subproject);

                }
                return Ok(listOfSubProject);
            }
            return Ok(new RespStatus { Status = "Success", Message = "No SubProjects Assigned to Employee" });

        }

      


        // GET: api/SubProjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubProjectDTO>>> GetSubProjects()
        {
            List<SubProjectDTO> ListSubProjectDTO = new();

            var SubProjects = await _context.SubProjects.ToListAsync();

            foreach (SubProject SubProj in SubProjects)
            {
                SubProjectDTO SubProjectDTO = new()
                {
                    Id = SubProj.Id,
                    ProjectId = SubProj.ProjectId,
                    ProjectName = _context.Projects.Find(SubProj.ProjectId).ProjectName,
                    SubProjectName = SubProj.SubProjectName,
                    SubProjectDesc = SubProj.SubProjectDesc
                };

                ListSubProjectDTO.Add(SubProjectDTO);

            }

            return ListSubProjectDTO;
        }

        // GET: api/SubProjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubProjectDTO>> GetSubProject(int id)
        {
            SubProjectDTO subProjectDTO = new ();

            var SubProj = await _context.SubProjects.FindAsync(id);

            if (SubProj == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Sub Project Id is Invalid!" });
            }

            subProjectDTO.Id = SubProj.Id;
            subProjectDTO.SubProjectName = SubProj.SubProjectName;
            subProjectDTO.ProjectId = SubProj.ProjectId;
            subProjectDTO.SubProjectDesc = SubProj.SubProjectDesc;

            return subProjectDTO;
        }

        // PUT: api/SubProjects/5
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutSubProject(int id, SubProjectDTO subProjectDto)
        {
            if (id != subProjectDto.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var subProj = await _context.SubProjects.FindAsync(id);

            subProj.Id = subProjectDto.Id;
            subProj.SubProjectName = subProjectDto.SubProjectName;
            subProj.SubProjectDesc = subProjectDto.SubProjectDesc;

            _context.SubProjects.Update(subProj);
            //_context.Entry(SubProjects).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Sub Project Updated!" });
        }

        // POST: api/SubProjects
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<SubProject>> PostSubProject(SubProjectDTO subProjectDto)
        {
            var subproject = _context.SubProjects.Where(c => c.SubProjectName == subProjectDto.SubProjectName).FirstOrDefault();
            if (subproject != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Sub Project Already Exists" });
            }

            SubProject SubProj = new()
            {
                ProjectId = subProjectDto.ProjectId,
                SubProjectName = subProjectDto.SubProjectName,
                SubProjectDesc = subProjectDto.SubProjectDesc
            };

            _context.SubProjects.Add(SubProj);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Sub-Project Created!" });
        }

        // DELETE: api/SubProjects/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteSubProject(int id)
        {
            var wrktask = _context.WorkTasks.Where(w => w.SubProjectId == id).FirstOrDefault();
            if (wrktask != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cant Delete the SubProject in Use" });
            }

            var subProject = await _context.SubProjects.FindAsync(id);
            if (subProject == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Sub Project Id is Invalid!" });
            }

            bool blnUsedInTravelReq = _context.TravelApprovalRequests.Where(t => t.SubProjectId == id).Any();
            bool blnUsedInCashAdvReq = _context.PettyCashRequests.Where(t => t.SubProjectId == id).Any();
            bool blnUsedInExpeReimReq = _context.ExpenseReimburseRequests.Where(t => t.SubProjectId == id).Any();

            if (blnUsedInTravelReq || blnUsedInCashAdvReq || blnUsedInExpeReimReq)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "SubProject in Use, Cant delete!" });
            }

            _context.SubProjects.Remove(subProject);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Sub-Project Deleted!" });
        }

       
    }
}
