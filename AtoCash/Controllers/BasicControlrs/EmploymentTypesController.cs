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
    public class EmploymentTypesController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public EmploymentTypesController(AtoCashDbContext context)
        {
            _context = context;
        }

        // GET: api/EmploymentTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmploymentTypeDTO>>> GetEmploymentTypes()
        {
            var EmplmntTypes = await _context.EmploymentTypes.ToListAsync();

            List<EmploymentTypeDTO> ListEmploymentTypeDTO = new();

            foreach ( EmploymentType emplmttype in EmplmntTypes)
            {
                EmploymentTypeDTO employmentTypeDTO = new();

                employmentTypeDTO.Id = emplmttype.Id;
                employmentTypeDTO.EmpJobTypeCode = emplmttype.EmpJobTypeCode ;
                employmentTypeDTO.EmpJobTypeDesc = emplmttype.EmpJobTypeDesc;

                ListEmploymentTypeDTO.Add(employmentTypeDTO);
            }


            return Ok(ListEmploymentTypeDTO);
        }

        [HttpGet]
        [ActionName("GetEmploymentTypesForDropDown")]
        public async Task<ActionResult<IEnumerable<EmploymentTypeDTO>>> GetEmploymentTypesForDropDown()
        {
            var EmplmntTypes = await _context.EmploymentTypes.ToListAsync();

            List<EmploymentTypeDTO> ListEmploymentTypeDTO = new();

            foreach (EmploymentType emplmttype in EmplmntTypes)
            {
                EmploymentTypeDTO employmentTypeDTO = new();

                employmentTypeDTO.Id = emplmttype.Id;
                employmentTypeDTO.EmpJobTypeCode = emplmttype.EmpJobTypeCode + ":" + emplmttype.EmpJobTypeDesc;
                ListEmploymentTypeDTO.Add(employmentTypeDTO);
            }


            return Ok(ListEmploymentTypeDTO);
        }

        // GET: api/EmploymentTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmploymentType>> GetEmploymentType(int id)
        {
            var employmentType = await _context.EmploymentTypes.FindAsync(id);

            if (employmentType == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Employment Type Id invalid!" });
            }

            return employmentType;
        }

        // PUT: api/EmploymentTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutEmploymentType(int id, EmploymentTypeDTO employmentType)
        {
            if (id != employmentType.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }


            var empTypes = await _context.EmploymentTypes.FindAsync(id);
            empTypes.EmpJobTypeDesc = employmentType.EmpJobTypeDesc;

            _context.EmploymentTypes.Update(empTypes);

            //_context.Entry(employmentType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmploymentTypeExists(id))
                {
                    return Conflict(new RespStatus { Status = "Failure", Message = "Employment Type Id invalid!" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new RespStatus { Status = "Success", Message = "Employment Type Details Updated!" });
        }

        // POST: api/EmploymentTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<EmploymentType>> PostEmploymentType(EmploymentType employmentType)
        {

            var emplymtTypes = _context.EmploymentTypes.Where(e => e.EmpJobTypeCode == employmentType.EmpJobTypeCode).FirstOrDefault();
            if (emplymtTypes != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "EmploymentType Already Exists" });
            }

            _context.EmploymentTypes.Add(employmentType);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "New EmploymentType Created!" });
        }

        // DELETE: api/EmploymentTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteEmploymentType(int id)
        {
            var employmentType = await _context.EmploymentTypes.FindAsync(id);
            if (employmentType == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Employment Type Id invalid!" });
            }

            _context.EmploymentTypes.Remove(employmentType);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Employment Type Deleted!" });
        }

        private bool EmploymentTypeExists(int id)
        {
            return _context.EmploymentTypes.Any(e => e.Id == id);
        }
    }
}
