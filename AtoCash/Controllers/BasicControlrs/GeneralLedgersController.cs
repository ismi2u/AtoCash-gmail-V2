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
    public class GeneralLedgersController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public GeneralLedgersController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("GeneralLedgersForDropdown")]
        public async Task<ActionResult<IEnumerable<GeneralLedgerVM>>> GetGeneralLedgersForDropdown()
        {
            List<GeneralLedgerVM> ListGeneralLedgerVM = new();

            var generalLedgers = await _context.GeneralLedgers.Where(c => c.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (GeneralLedger generalLedger in generalLedgers)
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
        // GET: api/GeneralLedgers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeneralLedgerDTO>>> GetGeneralLedgers()
        {
            List<GeneralLedgerDTO> ListGeneralLedgerDTO = new();

            var generalLedgers = await _context.GeneralLedgers.ToListAsync();

            foreach (GeneralLedger generalLedger in generalLedgers)
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

        // GET: api/GeneralLedgers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralLedgerDTO>> GetGeneralLedger(int id)
        {
            var generalLedger = await _context.GeneralLedgers.FindAsync(id);

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

        // PUT: api/GeneralLedgers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutGeneralLedger(int id, GeneralLedgerDTO generalLedgerDTO)
        {
            if (id != generalLedgerDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var genLedger = await _context.GeneralLedgers.FindAsync(id);

            genLedger.GeneralLedgerAccountNo = generalLedgerDTO.GeneralLedgerAccountNo;
            genLedger.GeneralLedgerAccountName = generalLedgerDTO.GeneralLedgerAccountName;
            genLedger.StatusTypeId = generalLedgerDTO.StatusTypeId;
            _context.GeneralLedgers.Update(genLedger);

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

        // POST: api/GeneralLedgers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<GeneralLedger>> PostGeneralLedger(GeneralLedgerDTO generalLedgerDTO)
        {
            var gLedger = _context.GeneralLedgers.Where(e => e.GeneralLedgerAccountNo == generalLedgerDTO.GeneralLedgerAccountNo).FirstOrDefault();
            if (gLedger != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No Already Exists" });
            }

            GeneralLedger generalLedger = new();
            generalLedger.GeneralLedgerAccountNo = generalLedgerDTO.GeneralLedgerAccountNo;
            generalLedger.GeneralLedgerAccountName = generalLedgerDTO.GeneralLedgerAccountName;
            generalLedger.StatusTypeId = generalLedgerDTO.StatusTypeId;
            _context.GeneralLedgers.Add(generalLedger);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "General Ledger Account No Created!" });
        }

        // DELETE: api/GeneralLedgers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteGeneralLedger(int id)
        {

            var generalLedger = await _context.GeneralLedgers.FindAsync(id);
            if (generalLedger == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No Invalid!" });
            }

            /* Commented as of now, till we create a Column in Expense Sub Claim Table */ 
            /*var expReimburse = _context.ExpenseSubClaims.Where(d => d.GeneralLedgerId == id).FirstOrDefault();

            if (expReimburse != null)
            {
            */    return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No in use for Expense Reimburse!" });
            /*}*/

            _context.GeneralLedgers.Remove(generalLedger);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "General Ledger Account No Deleted!" });
        }

       


  
        ///


    }
}
