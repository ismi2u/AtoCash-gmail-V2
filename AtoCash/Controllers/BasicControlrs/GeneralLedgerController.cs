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
    public class GeneralLedgerController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public GeneralLedgerController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("GeneralLedgerForDropdown")]
        public async Task<ActionResult<IEnumerable<GeneralLedgerVM>>> GetGeneralLedgerForDropdown()
        {
            List<GeneralLedgerVM> ListGeneralLedgerVM = new();

            var GeneralLedger = await _context.GeneralLedger.Where(c => c.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (GeneralLedger generalLedger in GeneralLedger)
            {
                GeneralLedgerVM generalLedgerVM = new()
                {
                    Id = generalLedger.Id,
                    GeneralLedgerAccountNo = generalLedger.GeneralLedgerAccountNo + ":" +  generalLedger.GeneralLedgerAccountName,
                };

                ListGeneralLedgerVM.Add(generalLedgerVM);
            }

            return ListGeneralLedgerVM;

        }
        // GET: api/GeneralLedger
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeneralLedgerDTO>>> GetGeneralLedger()
        {
            List<GeneralLedgerDTO> ListGeneralLedgerDTO = new();

            var GeneralLedger = await _context.GeneralLedger.ToListAsync();

            foreach (GeneralLedger generalLedger in GeneralLedger)
            {
                GeneralLedgerDTO generalLedgerDTO = new()
                {
                    Id = generalLedger.Id,
                    GeneralLedgerAccountNo = generalLedger.GeneralLedgerAccountNo,
                    GeneralLedgerAccountName = generalLedger.GeneralLedgerAccountName,
                    StatusTypeId = generalLedger.StatusTypeId,
                    StatusType = _context.StatusTypes.Find(generalLedger.StatusTypeId).Status
                };

                ListGeneralLedgerDTO.Add(generalLedgerDTO);

            }
            return Ok(ListGeneralLedgerDTO);
        }

        // GET: api/GeneralLedger/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralLedgerDTO>> GetGeneralLedger(int id)
        {
            var generalLedger = await _context.GeneralLedger.FindAsync(id);

            if (generalLedger == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No is Invalid!" });
            }

            GeneralLedgerDTO generalLedgerDTO = new()
            {
                Id = generalLedger.Id,
                GeneralLedgerAccountNo = generalLedger.GeneralLedgerAccountNo,
                GeneralLedgerAccountName = generalLedger.GeneralLedgerAccountName,
                StatusTypeId = generalLedger.StatusTypeId,
                StatusType = _context.StatusTypes.Find(generalLedger.StatusTypeId).Status
            };

            return generalLedgerDTO;
        }

        // PUT: api/GeneralLedger/5
        [HttpPut("{id}")]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutGeneralLedger(int id, GeneralLedgerDTO generalLedgerDTO)
        {
            if (id != generalLedgerDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var genLedger = await _context.GeneralLedger.FindAsync(id);

            //genLedger.GeneralLedgerAccountNo = generalLedgerDTO.GeneralLedgerAccountNo;
            genLedger.GeneralLedgerAccountName = generalLedgerDTO.GeneralLedgerAccountName;
            genLedger.StatusTypeId = generalLedgerDTO.StatusTypeId;
            _context.GeneralLedger.Update(genLedger);

            //_context.Entry(generalLedger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Expsense Category Details Updated!" });
        }

        // POST: api/GeneralLedger
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<GeneralLedger>> PostGeneralLedger(GeneralLedgerDTO generalLedgerDTO)
        {
            var gLedger = _context.GeneralLedger.Where(e => e.GeneralLedgerAccountNo == generalLedgerDTO.GeneralLedgerAccountNo).FirstOrDefault();
            if (gLedger != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No already exists" });
            }

            GeneralLedger generalLedger = new();
            generalLedger.GeneralLedgerAccountNo = generalLedgerDTO.GeneralLedgerAccountNo;
            generalLedger.GeneralLedgerAccountName = generalLedgerDTO.GeneralLedgerAccountName;
            generalLedger.StatusTypeId = generalLedgerDTO.StatusTypeId;
            _context.GeneralLedger.Add(generalLedger);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "General Ledger Account No Created!" });
        }

        // DELETE: api/GeneralLedger/5
        [HttpDelete("{id}")]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteGeneralLedger(int id)
        {

            var generalLedger = await _context.GeneralLedger.FindAsync(id);
            if (generalLedger == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No Invalid!" });
            }

         
            var expTypes = _context.ExpenseTypes.Where(d => d.GeneralLedgerId == id).FirstOrDefault();

            if (expTypes != null)
            {
               return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No in use for Expense Type!" });
            }

            _context.GeneralLedger.Remove(generalLedger);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "General Ledger Account No Deleted!" });
        }

       


  
        ///


    }
}
