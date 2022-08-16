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
    public class WorkTasksController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public WorkTasksController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("WorkTasksForDropdown")]
        public async Task<ActionResult<IEnumerable<WorkTaskVM>>> GetWorkTasksForDropDown()
        {
            List<WorkTaskVM> ListWorkTaskVM = new();

            var workTasks = await _context.WorkTasks.ToListAsync();
            foreach (WorkTask workTask in workTasks)
            {
                WorkTaskVM workTaskVM = new()
                {
                    Id = workTask.Id,
                    TaskName = workTask.TaskName + ":" + workTask.TaskDesc
                };

                ListWorkTaskVM.Add(workTaskVM);
            }

            return ListWorkTaskVM;

        }


        [HttpGet("{id}")]
        [ActionName("GetWorkTasksForSubProjects")]
        public async Task<ActionResult<IEnumerable<SubProjectVM>>> GetWorkTasksForSubProjects(int id)
        {
           
            var listOfTasks = await _context.WorkTasks.Where(t => t.SubProjectId == id).ToListAsync();

            List<WorkTaskVM> ListWorkTaskVM = new();

            if (listOfTasks != null)
            {
                foreach (var item in listOfTasks)
                {
                    WorkTaskVM workTaskVM = new()
                    {
                        Id = item.Id,
                        TaskName = item.TaskName
                    };
                    ListWorkTaskVM.Add(workTaskVM);

                }
                return Ok(ListWorkTaskVM);
            }
            return Ok(new RespStatus { Status = "Success", Message = "No WorkTask Assigned to Employee" });

        }

        [HttpGet]
        [ActionName("GetWorkTasksForDropdown")]
        public async Task<ActionResult<IEnumerable<WorkTaskVM>>> GetWorkTasksForDropdown()
        {
            List<WorkTaskVM> ListWorkTaskVM = new();

            var workTasks = await _context.WorkTasks.ToListAsync();
            foreach (WorkTask workTask in workTasks)
            {
                WorkTaskVM workTaskVM = new()
                {
                    Id = workTask.Id,
                    TaskName = workTask.TaskName
                };

                ListWorkTaskVM.Add(workTaskVM);
            }

            return ListWorkTaskVM;


        }
        // GET: api/WorkTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetWorkTasks()
        {
            List<WorkTaskDTO> ListWorkTaskDto = new();

            var WorkTasks = await _context.WorkTasks.ToListAsync();

            foreach (WorkTask worktask in WorkTasks)
            {
                WorkTaskDTO workTaskDto = new()
                {
                    Id = worktask.Id,
                    SubProjectId = worktask.SubProjectId,
                    SubProject = _context.SubProjects.Find(worktask.SubProjectId).SubProjectName,
                    TaskName = worktask.TaskName,
                    TaskDesc = worktask.TaskDesc
                };

                ListWorkTaskDto.Add(workTaskDto);

            }

            return ListWorkTaskDto;
        }

        // GET: api/WorkTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkTaskDTO>> GetWorkTask(int id)
        {
           

            var worktask = await _context.WorkTasks.FindAsync(id);

            if (worktask == null)
            {
                return Ok(new RespStatus { Status = "Failure", Message = "Work Task Id is invalid!" });
            }
            WorkTaskDTO workTaskDto = new()
            {
                Id = worktask.Id,
                SubProjectId = worktask.SubProjectId,
                SubProject = _context.SubProjects.Find(worktask.SubProjectId).SubProjectName,
                TaskName = worktask.TaskName,
                  
                TaskDesc = worktask.TaskDesc
            };
           

            return workTaskDto;
        }

        // PUT: api/WorkTasks/5
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutWorkTask(int id, WorkTaskDTO workTaskDto)
        {
            if (id != workTaskDto.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var workTask = await _context.WorkTasks.FindAsync(id);

            workTask.TaskName = workTaskDto.TaskName;
            workTask.TaskDesc = workTaskDto.TaskDesc;

            _context.WorkTasks.Update(workTask);
            //_context.Entry(workTaskDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Work Task Details Updated!" });
        }

        // POST: api/WorkTasks
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<WorkTask>> PostWorkTask(WorkTaskDTO workTaskDto)
        {

            var wTask = _context.WorkTasks.Where(c => c.TaskName == workTaskDto.TaskName).FirstOrDefault();
            if (wTask != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "TaskName Already Exists" });
            }

            WorkTask workTask = new()
            {
                SubProjectId = workTaskDto.SubProjectId,
                TaskName = workTaskDto.TaskName,
                TaskDesc = workTaskDto.TaskDesc
            };

            _context.WorkTasks.Add(workTask);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Work-Task Created!" });


        }

        // DELETE: api/WorkTasks/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteWorkTask(int id)
        {
            var workTask = await _context.WorkTasks.FindAsync(id);
            if (workTask == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Work Task Id Invalid!" });
            }

            bool blnUsedInTravelReq = _context.TravelApprovalRequests.Where(t => t.WorkTaskId == id).Any();
            bool blnUsedInCashAdvReq = _context.PettyCashRequests.Where(t => t.WorkTaskId == id).Any();
            bool blnUsedInExpeReimReq = _context.ExpenseReimburseRequests.Where(t => t.WorkTaskId == id).Any();

            if (blnUsedInTravelReq || blnUsedInCashAdvReq || blnUsedInExpeReimReq)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Work Task is in Use!" });
            }

            _context.WorkTasks.Remove(workTask);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Work-Task Deleted!" });
        }

       
    }
}
