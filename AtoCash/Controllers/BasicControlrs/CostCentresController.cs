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
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class CostCentersController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public CostCentersController(AtoCashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ActionName("CostCentersForDropdown")]
        public async Task<ActionResult<IEnumerable<CostCenterVM>>> GetCostCentersForDropDown()
        {
            List<CostCenterVM> ListCostCenterVM = new List<CostCenterVM>();

            var costCenters = await _context.CostCenters.Where(c => c.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (CostCenter costCenter in costCenters)
            {
                CostCenterVM costCenterVM = new CostCenterVM
                {
                    Id = costCenter.Id,
                    CostCenterCode = costCenter.CostCenterCode + " " + costCenter.CostCenterDesc,
                };

                ListCostCenterVM.Add(costCenterVM);
            }

            return ListCostCenterVM;

        }

        // GET: api/CostCenters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CostCenterDTO>>> GetCostCenters()
        {

            List<CostCenterDTO> ListCostCenterDTO = new List<CostCenterDTO>();

            var costCenters = await _context.CostCenters.ToListAsync();

            foreach (CostCenter costCenter in costCenters)
            {
                CostCenterDTO costCenterDTO = new CostCenterDTO
                {
                    Id = costCenter.Id,
                    CostCenterCode = costCenter.CostCenterCode,
                    CostCenterDesc = costCenter.CostCenterDesc,
                    StatusTypeId = costCenter.StatusTypeId,
                    StatusType = _context.StatusTypes.Find(costCenter.StatusTypeId).Status
                };

                ListCostCenterDTO.Add(costCenterDTO);

            }
            return Ok(ListCostCenterDTO);
        }

        // GET: api/CostCenters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CostCenterDTO>> GetCostCenter(int id)
        {
            var costCenter = await _context.CostCenters.FindAsync(id);

            if (costCenter == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cost Centre Id invalid!" });
            }

            CostCenterDTO costCenterDTO = new CostCenterDTO
            {
                Id = costCenter.Id,
                CostCenterCode = costCenter.CostCenterCode,
                CostCenterDesc = costCenter.CostCenterDesc,
                StatusTypeId = costCenter.StatusTypeId,
                StatusType = _context.StatusTypes.Find(costCenter.StatusTypeId).Status

            };

            return costCenterDTO;
        }

        // PUT: api/CostCenters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutCostCenter(int id, CostCenterDTO costCenterDTO)
        {
            if (id != costCenterDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var ccentre = await _context.CostCenters.FindAsync(id);
            ccentre.CostCenterDesc = costCenterDTO.CostCenterDesc;
            ccentre.StatusTypeId = costCenterDTO.StatusTypeId;
            _context.CostCenters.Update(ccentre);

            //_context.Entry(costCenter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CostCenterExists(id))
                {
                    return Conflict(new RespStatus { Status = "Failure", Message = "Costcenter is invalid" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new RespStatus { Status = "Success", Message = "CostCenter Details Updated!" });
        }

        // POST: api/CostCenters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<CostCenter>> PostCostCenter(CostCenterDTO costCenterDTO)
        {
            var ccentre = _context.CostCenters.Where(c => c.CostCenterCode == costCenterDTO.CostCenterCode).FirstOrDefault();
            if (ccentre != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "CostCenter Already Exists" });
            }
            CostCenter costCenter = new();
            costCenter.CostCenterCode = costCenterDTO.CostCenterCode;
            costCenter.CostCenterDesc = costCenterDTO.CostCenterDesc;
            costCenter.StatusTypeId = costCenterDTO.StatusTypeId;

            _context.CostCenters.Add(costCenter);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "CostCenter Created!" });
        }

        // DELETE: api/CostCenters/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteCostCenter(int id)
        {
            var dept = _context.Departments.Where(d => d.CostCenterId == id).FirstOrDefault();
            var proj = _context.Projects.Where(p => p.CostCenterId == id).FirstOrDefault();

            if (dept != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cost-Centre in use for Department" });
            }
            if (proj != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cost-Centre in use for Project" });
            }

            var costCenter = await _context.CostCenters.FindAsync(id);
            if (costCenter == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cost Centre Id invalid!" });
            }

            _context.CostCenters.Remove(costCenter);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Cost-Centre Deleted!" });
        }

        private bool CostCenterExists(int id)
        {
            return _context.CostCenters.Any(e => e.Id == id);
        }


     
        //
    }
}
