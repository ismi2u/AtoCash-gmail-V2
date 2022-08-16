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

namespace AtoCash.Controllers.BasicControlrs
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class StatusTypesController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public StatusTypesController(AtoCashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ActionName("StatusTypesForDropdown")]
        public async Task<ActionResult<IEnumerable<StatusTypeVM>>> GetStatusTypesForDropdown()
        {
            List<StatusTypeVM> ListStatusTypeVM = new();

            var statusTypes = await _context.StatusTypes.ToListAsync();
            foreach (StatusType statusType in statusTypes)
            {
                StatusTypeVM statusTypeVM = new()
                {
                    Id = statusType.Id,
                    Status = statusType.Status,
                };

                ListStatusTypeVM.Add(statusTypeVM);
            }

            return ListStatusTypeVM;

        }


        // GET: api/StatusTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusType>>> GetStatusTypes()
        {
            return await _context.StatusTypes.ToListAsync();
        }

        // GET: api/StatusTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusType>> GetStatusType(int id)
        {
            var statusType = await _context.StatusTypes.FindAsync(id);

            if (statusType == null)
            {
                return NotFound();
            }

            return statusType;
        }

        // PUT: api/StatusTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutStatusType(int id, StatusType statusType)
        {
            if (id != statusType.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Status Type Id is Invalid" });
            }

            var statustyp = await _context.StatusTypes.FindAsync(id);
            statustyp.Status = statusType.Status;
            _context.StatusTypes.Update(statustyp);

            //_context.Entry(statusType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Conflict(new RespStatus { Status = "Failure", Message = "Status Type is Updated!" });
        }

        // POST: api/StatusTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<StatusType>> PostStatusType(StatusType statusType)
        {
            _context.StatusTypes.Add(statusType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStatusType", new { id = statusType.Id }, statusType);
        }

        // DELETE: api/StatusTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteStatusType(int id)
        {
            var statusType = await _context.StatusTypes.FindAsync(id);
            if (statusType == null)
            {
                return NotFound();
            }

            _context.StatusTypes.Remove(statusType);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Status Type Deleted!" });
        }

        private bool StatusTypeExists(int id)
        {
            return _context.StatusTypes.Any(e => e.Id == id);
        }
    }
}
