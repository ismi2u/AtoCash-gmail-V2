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
  // [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, Manager, User")]
    public class BusinessAreasController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public BusinessAreasController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("BusinessAreasForDropdown")]
        public async Task<ActionResult<IEnumerable<BusinessAreaVM>>> GetBusinessAreasForDropdown()
        {
            List<BusinessAreaVM> ListBusinessAreaVM = new List<BusinessAreaVM>();

            var BusinessAreas = await _context.BusinessAreas.Where(d => d.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (BusinessArea BusinessArea in BusinessAreas)
            {
                BusinessAreaVM BusinessAreaVM = new BusinessAreaVM
                {
                    Id = BusinessArea.Id,
                    BusinessAreaName = BusinessArea.BusinessAreaCode + ":" + BusinessArea.BusinessAreaName
                };

                ListBusinessAreaVM.Add(BusinessAreaVM);
            }
            return ListBusinessAreaVM;

        }
        [HttpGet("{id}")]
        [ActionName("BusinessAreasForDropdownByCostCentre")]
        public async Task<ActionResult<IEnumerable<BusinessAreaVM>>> GetBusinessAreasForDropdownByCostCentre(int id)
        {
            List<BusinessAreaVM> ListBusinessAreaVM = new List<BusinessAreaVM>();

            var BusinessAreas = await _context.BusinessAreas.Where(d => d.StatusTypeId == (int)EStatusType.Active && d.CostCenterId == id).ToListAsync();
            foreach (BusinessArea BusinessArea in BusinessAreas)
            {
                BusinessAreaVM BusinessAreaVM = new BusinessAreaVM
                {
                    Id = BusinessArea.Id,
                    BusinessAreaName = BusinessArea.BusinessAreaCode + ":" + BusinessArea.BusinessAreaName
                };

                ListBusinessAreaVM.Add(BusinessAreaVM);
            }
            return ListBusinessAreaVM;

        }

        // GET: api/BusinessAreas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessAreaDTO>>> GetBusinessAreas()
        {
            List<BusinessAreaDTO> ListBusinessAreaDTO = new List<BusinessAreaDTO>();

            var BusinessAreas = await _context.BusinessAreas.ToListAsync();

            foreach (BusinessArea BusinessArea in BusinessAreas)
            {
                BusinessAreaDTO BusinessAreaDTO = new BusinessAreaDTO
                {
                    Id = BusinessArea.Id,
                    BusinessAreaCode = BusinessArea.BusinessAreaCode,
                    BusinessAreaName = BusinessArea.BusinessAreaName,
                    CostCenterId = BusinessArea.CostCenterId,
                    CostCenter = _context.CostCenters.Find(BusinessArea.CostCenterId).CostCenterCode,
                    StatusTypeId = BusinessArea.StatusTypeId,
                    StatusType = _context.StatusTypes.Find(BusinessArea.StatusTypeId).Status

                };

                ListBusinessAreaDTO.Add(BusinessAreaDTO);

            }

            return ListBusinessAreaDTO;
        }

        // GET: api/BusinessAreas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessAreaDTO>> GetBusinessArea(int id)
        {
            BusinessAreaDTO BusinessAreaDTO = new BusinessAreaDTO();

            var BusinessArea = await _context.BusinessAreas.FindAsync(id);

            if (BusinessArea == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "BusinessArea Id invalid!" });
            }

            BusinessAreaDTO.Id = BusinessArea.Id;
            BusinessAreaDTO.BusinessAreaCode = BusinessArea.BusinessAreaCode;
            BusinessAreaDTO.BusinessAreaName = BusinessArea.BusinessAreaName;
            BusinessAreaDTO.CostCenterId = BusinessArea.CostCenterId;
            BusinessAreaDTO.CostCenter = _context.CostCenters.Find(BusinessArea.CostCenterId).CostCenterCode;
            BusinessAreaDTO.StatusTypeId = BusinessArea.StatusTypeId;
            BusinessAreaDTO.StatusType = _context.StatusTypes.Find(BusinessArea.StatusTypeId).Status;

            return BusinessAreaDTO;
        }

        // PUT: api/BusinessAreas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
      // [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutBusinessArea(int id, BusinessAreaDTO BusinessAreaDto)
        {
            if (id != BusinessAreaDto.Id)
            {
                return Conflict(new Authentication.RespStatus { Status = "Failure", Message = "Id not Valid for BusinessArea" });
            }

            var BusinessArea = await _context.BusinessAreas.FindAsync(id);

            BusinessArea.BusinessAreaName = BusinessAreaDto.BusinessAreaName;
            BusinessArea.CostCenterId = BusinessAreaDto.CostCenterId;
            BusinessArea.StatusTypeId = BusinessAreaDto.StatusTypeId;

            _context.BusinessAreas.Update(BusinessArea);
            //_context.Entry(projectDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessAreaExists(id))
                {
                    return Conflict(new RespStatus { Status = "Failure", Message = "BusinessArea is invalid" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new RespStatus { Status = "Success", Message = "BusinessArea Details Updated!" });
        }

        // POST: api/BusinessAreas
        [HttpPost]
      // [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<BusinessArea>> PostBusinessArea(BusinessAreaDTO BusinessAreaDto)
        {
            var dept = _context.BusinessAreas.Where(c => c.BusinessAreaCode == BusinessAreaDto.BusinessAreaCode).FirstOrDefault();
            if (dept != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "BusinessArea Already Exists" });
            }

            BusinessArea BusinessArea = new BusinessArea
            {
                BusinessAreaCode = BusinessAreaDto.BusinessAreaCode,
                BusinessAreaName = BusinessAreaDto.BusinessAreaName,
                CostCenterId = BusinessAreaDto.CostCenterId,
                StatusTypeId = BusinessAreaDto.StatusTypeId
            };

            _context.BusinessAreas.Add(BusinessArea);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "BusinessArea Created!" });
        }

        // DELETE: api/BusinessAreas/5
        [HttpDelete("{id}")]
      // [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteBusinessArea(int id)
        {

            var emp = _context.Employees.Where(e => e.BusinessAreaId == id).FirstOrDefault();

            if (emp != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "BusinessArea in Use - Can't delete" });
            }


            var BusinessArea = await _context.BusinessAreas.FindAsync(id);
            if (BusinessArea == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "BusinessArea Id invalid!" });
            }

            _context.BusinessAreas.Remove(BusinessArea);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "BusinessArea Deleted!" });
        }

        private bool BusinessAreaExists(int id)
        {
            return _context.BusinessAreas.Any(e => e.Id == id);
        }


        //
    }
}
