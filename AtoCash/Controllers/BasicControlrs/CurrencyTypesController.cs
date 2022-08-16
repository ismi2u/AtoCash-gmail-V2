using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtoCash.Data;
using AtoCash.Models;
using AtoCash.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AtoCash.Controllers.BasicControlrs
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class CurrencyTypesController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public CurrencyTypesController(AtoCashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ActionName("CurrencyTypesForDropdown")]
        public async Task<ActionResult<IEnumerable<CurrencyTypeVM>>> GetCurrencyTypesForDropdown()
        {
            List<CurrencyTypeVM> ListCurrencyTypeVM = new();

            var currencyTypes = await _context.CurrencyTypes.Where(c => c.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (CurrencyType currencyType in currencyTypes)
            {
                CurrencyTypeVM currencyTypeVM = new()
                {
                    Id = currencyType.Id,
                    CurrencyCode = currencyType.CurrencyCode,
                };

                ListCurrencyTypeVM.Add(currencyTypeVM);
            }

            return ListCurrencyTypeVM;

        }


        // GET: api/CurrencyTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyTypeDTO>>> GetCurrencyTypes()
        {

            List<CurrencyTypeDTO> ListCurrencyTypeDTO = new();

            var currencyTypes = await _context.CurrencyTypes.ToListAsync();

            foreach (CurrencyType currencyType in currencyTypes)
            {
                CurrencyTypeDTO currencyTypeDTO = new()
                {
                    Id = currencyType.Id,
                    CurrencyCode = currencyType.CurrencyCode,
                    CurrencyName = currencyType.CurrencyName,
                    Country = currencyType.Country,
                    StatusTypeId = currencyType.StatusTypeId
                };

                ListCurrencyTypeDTO.Add(currencyTypeDTO);

            }
            return Ok(ListCurrencyTypeDTO);
        }

        // GET: api/CurrencyTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyTypeDTO>> GetCurrencyType(int id)
        {
            var currencyType = await _context.CurrencyTypes.FindAsync(id);

            if (currencyType == null)
            {
                return NotFound();
            }

            CurrencyTypeDTO currencyTypeDTO = new()
            {
                Id = currencyType.Id,
                CurrencyCode = currencyType.CurrencyCode,
                CurrencyName = currencyType.CurrencyName,
                Country = currencyType.Country,
                StatusTypeId = currencyType.StatusTypeId
            };


            return currencyTypeDTO;
        }

        // PUT: api/CurrencyTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutCurrencyType(int id, CurrencyTypeDTO currencyTypeDTO)
        {
            if (id != currencyTypeDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var currencyType = await _context.CurrencyTypes.FindAsync(id);
            currencyType.CurrencyName = currencyTypeDTO.CurrencyName;
            currencyType.Country = currencyTypeDTO.Country;
            currencyType.StatusTypeId = currencyTypeDTO.StatusTypeId;

            _context.CurrencyTypes.Update(currencyType);

            //_context.Entry(currencyType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "CurrencyType Details Updated!" });
        }

        // POST: api/CurrencyTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<CurrencyType>> PostCurrencyType(CurrencyTypeDTO currencyTypeDto)
        {

            var curncyType = _context.CurrencyTypes.Where(c => c.CurrencyCode == currencyTypeDto.CurrencyCode).FirstOrDefault();
            if (curncyType != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Currency Already Exists" });
            }

            CurrencyType currencyTyp = new();

            currencyTyp.CurrencyCode = currencyTypeDto.CurrencyCode;

            if (currencyTypeDto.CurrencyCode.Length > 3)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Currency Code Exeeds the acceptable Length!" });
            }
            currencyTyp.CurrencyName = currencyTypeDto.CurrencyName;
            currencyTyp.Country = currencyTypeDto.Country;
            currencyTyp.StatusTypeId = currencyTypeDto.StatusTypeId;

            _context.CurrencyTypes.Add(currencyTyp);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "New Currency Created!" });
        }

        // DELETE: api/CurrencyTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteCurrencyType(int id)
        {
            bool blnUsedInEmployees = _context.Employees.Where(e => e.CurrencyTypeId == id).Any();
            bool blnUsedInCashAdvReq = _context.PettyCashRequests.Where(t => t.CurrencyTypeId == id).Any();
            bool blnUsedInExpeReimReq = _context.ExpenseReimburseRequests.Where(t => t.CurrencyTypeId == id).Any();

            if (blnUsedInEmployees || blnUsedInCashAdvReq || blnUsedInExpeReimReq)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Currency in Use, Cant delete!" });
            }


            CurrencyType currencyType = await _context.CurrencyTypes.FindAsync(id);

            if (currencyType.Id == 0)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }
            _context.CurrencyTypes.Remove(currencyType);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Currency Deleted!" });
        }

      

        //
    }
}
