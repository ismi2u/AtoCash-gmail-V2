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
    //[Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class EmpCurrentPettyCashBalancesController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public EmpCurrentPettyCashBalancesController(AtoCashDbContext context)
        {
            _context = context;
        }

        // GET: api/EmpCurrentPettyCashBalances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpCurrentPettyCashBalanceDTO>>> GetEmpCurrentPettyCashBalances()
        {
            List<EmpCurrentPettyCashBalanceDTO> ListEmpCurrentPettyCashBalanceDTO = new();

            var empCurrentPettyCashBalances = await _context.EmpCurrentPettyCashBalances.ToListAsync();

            foreach (EmpCurrentPettyCashBalance empCurrentPettyCashBalance in empCurrentPettyCashBalances)
            {
                EmpCurrentPettyCashBalanceDTO empCurrentPettyCashBalanceDTO = new()
                {
                    Id = empCurrentPettyCashBalance.Id,
                    EmployeeId = empCurrentPettyCashBalance.EmployeeId,
                    CurBalance = empCurrentPettyCashBalance.CurBalance,
                    UpdatedOn = empCurrentPettyCashBalance.UpdatedOn
                };
                ListEmpCurrentPettyCashBalanceDTO.Add(empCurrentPettyCashBalanceDTO);

            }

            return ListEmpCurrentPettyCashBalanceDTO;
        }

        // GET: api/EmpCurrentPettyCashBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpCurrentPettyCashBalanceDTO>> GetEmpCurrentPettyCashBalance(int id)
        {
            EmpCurrentPettyCashBalanceDTO empCurrentPettyCashBalanceDTO = new();

            var empCurrentPettyCashBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == id).FirstOrDefault();


            if (empCurrentPettyCashBalance == null)
            {
                empCurrentPettyCashBalanceDTO.CurBalance = 0;
                return empCurrentPettyCashBalanceDTO;
            }

            await Task.Run(() =>
            {
                //empCurrentPettyCashBalanceDTO.Id = empCurrentPettyCashBalance.Id;
                empCurrentPettyCashBalanceDTO.EmployeeId = empCurrentPettyCashBalance.EmployeeId;
                empCurrentPettyCashBalanceDTO.CurBalance = empCurrentPettyCashBalance.CurBalance;
                empCurrentPettyCashBalanceDTO.UpdatedOn = empCurrentPettyCashBalance.UpdatedOn;
            });



            return empCurrentPettyCashBalanceDTO;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<EmpAllCurBalStatusDTO>> GetEmpMaxlimitCurBalAndCashInHandStatus(int id)
        {
            EmpAllCurBalStatusDTO empAllCurBalStatusDTO = new();

            var empCurrentPettyCashBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == id).FirstOrDefault();

            if (empCurrentPettyCashBalance == null)
            {
                empAllCurBalStatusDTO.CurBalance = 0;
            }

            await Task.Run(() =>
            {
                empAllCurBalStatusDTO.CurBalance = empCurrentPettyCashBalance.CurBalance;
                empAllCurBalStatusDTO.MaxLimit = _context.JobRoles.Find(_context.Employees.Find(id).RoleId).MaxPettyCashAllowed;


                //condition to check  the cash-in-hand and set it to Zero '0'
                EmpCurrentPettyCashBalance empCurPettyBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == id).FirstOrDefault();

                empAllCurBalStatusDTO.CashInHand = empCurPettyBalance.CashOnHand;

                empAllCurBalStatusDTO.CurBalance = empCurPettyBalance.CurBalance;


                empAllCurBalStatusDTO.PendingSettlement = _context.DisbursementsAndClaimsMasters.Where(d => d.EmployeeId == id && d.IsSettledAmountCredited == false && d.ApprovalStatusId == (int)EApprovalStatus.Approved)
                                                        .Select(s => s.AmountToCredit ?? 0).Sum();

                empAllCurBalStatusDTO.PendingApproval = _context.DisbursementsAndClaimsMasters.Where(d => d.EmployeeId == id && d.IsSettledAmountCredited == false && d.ApprovalStatusId == (int)EApprovalStatus.Pending)
                                                        .Select(s => s.ClaimAmount).Sum();

                empAllCurBalStatusDTO.WalletBalLastUpdated = empCurrentPettyCashBalance.UpdatedOn;

            });
            return empAllCurBalStatusDTO;
        }

        // PUT: api/EmpCurrentPettyCashBalances/5
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutEmpCurrentPettyCashBalance(int id, EmpCurrentPettyCashBalanceDTO empCurrentPettyCashBalanceDto)
        {
            if (id != empCurrentPettyCashBalanceDto.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var empCurrentPettyCashBalance = await _context.EmpCurrentPettyCashBalances.FindAsync(id);

            empCurrentPettyCashBalance.Id = empCurrentPettyCashBalanceDto.Id;
            empCurrentPettyCashBalance.EmployeeId = empCurrentPettyCashBalanceDto.EmployeeId;
            empCurrentPettyCashBalance.CurBalance = empCurrentPettyCashBalanceDto.CurBalance;
            empCurrentPettyCashBalance.UpdatedOn = empCurrentPettyCashBalanceDto.UpdatedOn;

            _context.EmpCurrentPettyCashBalances.Update(empCurrentPettyCashBalance);
            //_context.Entry(projectDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpCurrentPettyCashBalanceExists(id))
                {
                    return Conflict(new RespStatus { Status = "Failure", Message = "Currnet Balance Id invalid!" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new RespStatus { Status = "Success", Message = "Petty Cash Balance Details Updated!" });
        }

        // POST: api/EmpCurrentPettyCashBalances
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<EmpCurrentPettyCashBalance>> PostEmpCurrentPettyCashBalance(EmpCurrentPettyCashBalanceDTO empCurrentPettyCashBalanceDto)
        {
            EmpCurrentPettyCashBalance empCurrentPettyCashBalance = new()
            {
                Id = empCurrentPettyCashBalanceDto.Id,
                EmployeeId = empCurrentPettyCashBalanceDto.EmployeeId,
                CurBalance = empCurrentPettyCashBalanceDto.CurBalance,
                CashOnHand = 0,
                UpdatedOn = empCurrentPettyCashBalanceDto.UpdatedOn
            };

            _context.EmpCurrentPettyCashBalances.Add(empCurrentPettyCashBalance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmpCurrentPettyCashBalance", new { id = empCurrentPettyCashBalance.Id }, empCurrentPettyCashBalance);
        }

        // GET: api/EmpCurrentPettyCashBalances/GetEmpCashBalanceVsAdvanced
        [HttpGet("{id}")]
        [ActionName("GetEmpCashBalanceVsAdvanced")]
        public async Task<ActionResult<CashbalVsAdvancedVM>> GetEmpCashBalanceVsAdvanced(int id)
        {

            CashbalVsAdvancedVM cashbalVsAdvancedVM = new();
            if (id == 0) // atominos admin doesnt have a wallet balance
            {
                cashbalVsAdvancedVM.CurCashBal = 0;
                cashbalVsAdvancedVM.MaxCashAllowed = 0;
                return Ok(cashbalVsAdvancedVM);
            }

            //int test = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == id).FirstOrDefault().Id;
            var empCurPettyBal = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == id).FirstOrDefault();
            //Check if employee cash balance is availabel in the EmpCurrentPettyCashBalance table, if NOT then ADD
            if (empCurPettyBal == null)
            {
                var emp = _context.Employees.Find(id);

                if (emp != null)
                {
                    Double empPettyCashAmountEligible = _context.JobRoles.Find(_context.Employees.Find(id).RoleId).MaxPettyCashAllowed;
                    _context.EmpCurrentPettyCashBalances.Add(new EmpCurrentPettyCashBalance()
                    {
                        EmployeeId = id,
                        CurBalance = empPettyCashAmountEligible,
                        CashOnHand = 0,
                        UpdatedOn = DateTime.Now
                    });

                    await _context.SaveChangesAsync();
                }
            }
            empCurPettyBal = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == id).FirstOrDefault();
            if (empCurPettyBal == null)
            {
                cashbalVsAdvancedVM.CurCashBal = 0;
            }
            else
            {
                cashbalVsAdvancedVM.CurCashBal = empCurPettyBal.CurBalance;
            }
            if (_context.Employees.Find(id).RoleId != 0)
            {
                cashbalVsAdvancedVM.MaxCashAllowed = _context.JobRoles.Find(_context.Employees.Find(id).RoleId).MaxPettyCashAllowed;
            }
            else
            {
                cashbalVsAdvancedVM.MaxCashAllowed = 0;
            }
            return Ok(cashbalVsAdvancedVM);
        }



        // DELETE: api/EmpCurrentPettyCashBalances/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteEmpCurrentPettyCashBalance(int id)
        {
            var empCurrentPettyCashBalance = await _context.EmpCurrentPettyCashBalances.FindAsync(id);
            if (empCurrentPettyCashBalance == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "EmpCurrentPettyCashBalances Id invalid!" });
            }

            _context.EmpCurrentPettyCashBalances.Remove(empCurrentPettyCashBalance);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Cash Balance Deleted!" });
        }

        private bool EmpCurrentPettyCashBalanceExists(int id)
        {
            return _context.EmpCurrentPettyCashBalances.Any(e => e.Id == id);
        }



        //
    }
}
