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
    public class BankController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public BankController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("BanksForDropdown")]
        public async Task<ActionResult<IEnumerable<BankVM>>> GetBanksForDropdown()
        {
            List<BankVM> ListBankVM = new();

            var Banks = await _context.Banks.Where(c => c.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (Bank Bank in Banks)
            {
                BankVM BankVM = new()
                {
                    Id = Bank.Id,
                    BankName = Bank.BankName
                };

                ListBankVM.Add(BankVM);
            }

            return ListBankVM;

        }
        // GET: api/Bank
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankDTO>>> GetBanks()
        {
            List<BankDTO> ListBankDTO = new();

            var Banks = await _context.Banks.ToListAsync();

            foreach (Bank Bank in Banks)
            {
                BankDTO BankDTO = new()
                {
                    Id = Bank.Id,
                    BankName = Bank.BankName,
                    StatusTypeId = Bank.StatusTypeId,
                    StatusType = _context.StatusTypes.Find(Bank.StatusTypeId).Status
                };

                ListBankDTO.Add(BankDTO);

            }
            return Ok(ListBankDTO);
        }

        // GET: api/Bank/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankDTO>> GetBank(int id)
        {
            var Bank = await _context.Banks.FindAsync(id);

            if (Bank == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No is Invalid!" });
            }

            BankDTO BankDTO = new()
            {
                Id = Bank.Id,
                BankName = Bank.BankName,
                StatusTypeId = Bank.StatusTypeId,
                StatusType = _context.StatusTypes.Find(Bank.StatusTypeId).Status
            };

            return BankDTO;
        }

        // PUT: api/Bank/5
        [HttpPut("{id}")]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutBank(int id, BankDTO BankDTO)
        {
            if (id != BankDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var bank = await _context.Banks.FindAsync(id);

            //genLedger.BankAccountNo = BankDTO.BankAccountNo;
           // bank.BankName = BankDTO.BankName;
            bank.BankDesc = BankDTO.BankDesc;
            bank.StatusTypeId = BankDTO.StatusTypeId;
            _context.Banks.Update(bank);

            //_context.Entry(Bank).State = EntityState.Modified;

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

        // POST: api/Bank
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<Bank>> PostBank(BankDTO BankDTO)
        {
            var bank = _context.Banks.Where(e => e.BankName == BankDTO.BankName).FirstOrDefault();
            if (bank != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Bank Name already exists" });
            }

            Bank Bank = new();
            Bank.BankName = BankDTO.BankName;
            Bank.BankDesc = BankDTO.BankDesc;
            Bank.StatusTypeId = BankDTO.StatusTypeId;
            _context.Banks.Add(Bank);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Bank added to the database!" });
        }

        // DELETE: api/Bank/5
        [HttpDelete("{id}")]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteBank(int id)
        {

            var Bank = await _context.Banks.FindAsync(id);
            if (Bank == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "General Ledger Account No Invalid!" });
            }

         
            var employeesBank = _context.Employees.Where(d => d.BankId == id).FirstOrDefault();

            if (employeesBank != null)
            {
               return Conflict(new RespStatus { Status = "Failure", Message = "Bank in use for Employees Table!" });
            }

            _context.Banks.Remove(Bank);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "General Ledger Account No Deleted!" });
        }

       


  
        ///


    }
}
