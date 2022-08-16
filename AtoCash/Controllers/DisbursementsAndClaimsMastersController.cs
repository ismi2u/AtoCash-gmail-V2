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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Finmgr, AccPayable, User")]
    public class DisbursementsAndClaimsMastersController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public DisbursementsAndClaimsMastersController(AtoCashDbContext context)
        {
            _context = context;
        }

        // GET: api/DisbursementsAndClaimsMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisbursementsAndClaimsMasterDTO>>> GetDisbursementsAndClaimsMasters()
        {
            List<DisbursementsAndClaimsMasterDTO> ListDisbursementsAndClaimsMasterDTO = new();

            var disbursementsAndClaimsMasters = await _context.DisbursementsAndClaimsMasters.Where(d=> d.RequestTypeId== (int)ERequestType.ExpenseReim && d.IsSettledAmountCredited == false).ToListAsync();

            foreach (DisbursementsAndClaimsMaster disbursementsAndClaimsMaster in disbursementsAndClaimsMasters)
            {
                DisbursementsAndClaimsMasterDTO disbursementsAndClaimsMasterDTO = new();

                disbursementsAndClaimsMasterDTO.Id = disbursementsAndClaimsMaster.Id;
                disbursementsAndClaimsMasterDTO.EmployeeId = disbursementsAndClaimsMaster.EmployeeId;
                disbursementsAndClaimsMasterDTO.EmployeeName = _context.Employees.Find(disbursementsAndClaimsMaster.EmployeeId).GetFullName();
                disbursementsAndClaimsMasterDTO.ExpenseReimburseReqId = disbursementsAndClaimsMaster.ExpenseReimburseReqId;
                disbursementsAndClaimsMasterDTO.DepartmentId = disbursementsAndClaimsMaster.DepartmentId;
                disbursementsAndClaimsMasterDTO.DepartmentName = disbursementsAndClaimsMaster.DepartmentId != null? _context.Departments.Find(disbursementsAndClaimsMaster.DepartmentId).DeptName : string.Empty;
                disbursementsAndClaimsMasterDTO.ProjectId = disbursementsAndClaimsMaster.ProjectId;
                disbursementsAndClaimsMasterDTO.ProjectName = disbursementsAndClaimsMaster.ProjectId != null ? _context.Projects.Find(disbursementsAndClaimsMaster.ProjectId).ProjectName : string.Empty;
                disbursementsAndClaimsMasterDTO.SubProjectId = disbursementsAndClaimsMaster.SubProjectId;
                disbursementsAndClaimsMasterDTO.SubProjectName = disbursementsAndClaimsMaster.SubProjectId != null ? _context.SubProjects.Find(disbursementsAndClaimsMaster.SubProjectId).SubProjectName : string.Empty;
                disbursementsAndClaimsMasterDTO.WorkTaskId = disbursementsAndClaimsMaster.WorkTaskId;
                disbursementsAndClaimsMasterDTO.WorkTaskName = disbursementsAndClaimsMaster.WorkTaskId != null ? _context.WorkTasks.Find(disbursementsAndClaimsMaster.WorkTaskId).TaskName : string.Empty;
                disbursementsAndClaimsMasterDTO.CurrencyTypeId = disbursementsAndClaimsMaster.CurrencyTypeId;
                disbursementsAndClaimsMasterDTO.CurrencyType = _context.CurrencyTypes.Find(disbursementsAndClaimsMaster.CurrencyTypeId).CurrencyCode;
                disbursementsAndClaimsMasterDTO.RecordDate = disbursementsAndClaimsMaster.RecordDate.ToShortDateString();
                disbursementsAndClaimsMasterDTO.ClaimAmount = disbursementsAndClaimsMaster.ClaimAmount;
                disbursementsAndClaimsMasterDTO.AmountToWallet = disbursementsAndClaimsMaster.AmountToWallet;
                disbursementsAndClaimsMasterDTO.AmountToCredit = disbursementsAndClaimsMaster.AmountToCredit;
                disbursementsAndClaimsMasterDTO.IsSettledAmountCredited = disbursementsAndClaimsMaster.IsSettledAmountCredited ?? false;
                disbursementsAndClaimsMasterDTO.SettledDate = disbursementsAndClaimsMaster.SettledDate != null ? disbursementsAndClaimsMaster.SettledDate.Value.ToShortDateString() : string.Empty;
                disbursementsAndClaimsMasterDTO.SettlementComment = disbursementsAndClaimsMaster.SettlementComment;
                disbursementsAndClaimsMasterDTO.SettlementAccount = disbursementsAndClaimsMaster.SettlementAccount;
                disbursementsAndClaimsMasterDTO.SettlementBankCard = disbursementsAndClaimsMaster.SettlementBankCard;
                disbursementsAndClaimsMasterDTO.AdditionalData = disbursementsAndClaimsMaster.AdditionalData;
                disbursementsAndClaimsMasterDTO.CostCenterId = disbursementsAndClaimsMaster.CostCenterId;
                disbursementsAndClaimsMasterDTO.ApprovalStatusId = disbursementsAndClaimsMaster.ApprovalStatusId;
                disbursementsAndClaimsMasterDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(disbursementsAndClaimsMaster.ApprovalStatusId).Status;


                ListDisbursementsAndClaimsMasterDTO.Add(disbursementsAndClaimsMasterDTO);

            }

            return ListDisbursementsAndClaimsMasterDTO;
        }



        

        // GET: api/DisbursementsAndClaimsMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DisbursementsAndClaimsMasterDTO>> GetDisbursementsAndClaimsMaster(int id)
        {


            var disbursementsAndClaimsMaster = await _context.DisbursementsAndClaimsMasters.FindAsync(id);

            if (disbursementsAndClaimsMaster == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Disburese Id invalid!" });
            }

            DisbursementsAndClaimsMasterDTO disbursementsAndClaimsMasterDTO = new();

            disbursementsAndClaimsMasterDTO.Id = disbursementsAndClaimsMaster.Id;
            disbursementsAndClaimsMasterDTO.EmployeeId = disbursementsAndClaimsMaster.EmployeeId;
            disbursementsAndClaimsMasterDTO.EmployeeName = _context.Employees.Find(disbursementsAndClaimsMaster.EmployeeId).GetFullName();
            disbursementsAndClaimsMasterDTO.ExpenseReimburseReqId = disbursementsAndClaimsMaster.ExpenseReimburseReqId;
            disbursementsAndClaimsMasterDTO.DepartmentId = disbursementsAndClaimsMaster.DepartmentId;
            disbursementsAndClaimsMasterDTO.DepartmentName = disbursementsAndClaimsMaster.DepartmentId != null ? _context.Departments.Find(disbursementsAndClaimsMaster.DepartmentId).DeptName : string.Empty;
            disbursementsAndClaimsMasterDTO.ProjectId = disbursementsAndClaimsMaster.ProjectId;
            disbursementsAndClaimsMasterDTO.ProjectName = disbursementsAndClaimsMaster.ProjectId != null ? _context.Projects.Find(disbursementsAndClaimsMaster.ProjectId).ProjectName : string.Empty;
            disbursementsAndClaimsMasterDTO.SubProjectId = disbursementsAndClaimsMaster.SubProjectId;
            disbursementsAndClaimsMasterDTO.SubProjectName = disbursementsAndClaimsMaster.SubProjectId != null ? _context.SubProjects.Find(disbursementsAndClaimsMaster.SubProjectId).SubProjectName : string.Empty;
            disbursementsAndClaimsMasterDTO.WorkTaskId = disbursementsAndClaimsMaster.WorkTaskId;
            disbursementsAndClaimsMasterDTO.WorkTaskName = disbursementsAndClaimsMaster.WorkTaskId != null ? _context.WorkTasks.Find(disbursementsAndClaimsMaster.WorkTaskId).TaskName : string.Empty;
            disbursementsAndClaimsMasterDTO.CurrencyTypeId = disbursementsAndClaimsMaster.CurrencyTypeId;
            disbursementsAndClaimsMasterDTO.CurrencyType = _context.CurrencyTypes.Find(disbursementsAndClaimsMaster.CurrencyTypeId).CurrencyCode;
            disbursementsAndClaimsMasterDTO.RecordDate = disbursementsAndClaimsMaster.RecordDate.ToShortDateString();
            disbursementsAndClaimsMasterDTO.ClaimAmount = disbursementsAndClaimsMaster.ClaimAmount;
            disbursementsAndClaimsMasterDTO.AmountToWallet = disbursementsAndClaimsMaster.AmountToWallet;
            disbursementsAndClaimsMasterDTO.AmountToCredit = disbursementsAndClaimsMaster.AmountToCredit;
            disbursementsAndClaimsMasterDTO.IsSettledAmountCredited = disbursementsAndClaimsMaster.IsSettledAmountCredited ?? false;
            disbursementsAndClaimsMasterDTO.SettledDate = disbursementsAndClaimsMaster.SettledDate != null ? disbursementsAndClaimsMaster.SettledDate.Value.ToShortDateString() : string.Empty;
            disbursementsAndClaimsMasterDTO.SettlementComment = disbursementsAndClaimsMaster.SettlementComment;
            disbursementsAndClaimsMasterDTO.SettlementAccount = disbursementsAndClaimsMaster.SettlementAccount;
            disbursementsAndClaimsMasterDTO.SettlementBankCard = disbursementsAndClaimsMaster.SettlementBankCard;
            disbursementsAndClaimsMasterDTO.AdditionalData = disbursementsAndClaimsMaster.AdditionalData;
            disbursementsAndClaimsMasterDTO.CostCenterId = disbursementsAndClaimsMaster.CostCenterId;
            disbursementsAndClaimsMasterDTO.ApprovalStatusId = disbursementsAndClaimsMaster.ApprovalStatusId;
            disbursementsAndClaimsMasterDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(disbursementsAndClaimsMaster.ApprovalStatusId).Status;




            return Ok(disbursementsAndClaimsMasterDTO);
        }

        // PUT: api/DisbursementsAndClaimsMasters/5
        [HttpPut("{id}")]
        [Authorize(Roles = "AccPayable")] // Only AccountPayables clerk can upated DisbursementsAndClaimsMaster
        public async Task<IActionResult> PutDisbursementsAndClaimsMaster(int id, DisbursementsAndClaimsMasterDTO disbursementsAndClaimsMasterDTO)
        {
            if (id != disbursementsAndClaimsMasterDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id state is invalid" });
            }

            var disbursementsAndClaimsMaster = await _context.DisbursementsAndClaimsMasters.FindAsync(id);

            //check the Credit To Wallet and Credit to Bank details to adjust for CashOnhand in EmpCurrentPettyCashBalance.CashOnHand
            double RoleMaxLimit = _context.JobRoles.Find(_context.Employees.Find(disbursementsAndClaimsMaster.EmployeeId).RoleId).MaxPettyCashAllowed;

            double CreditToWallet = disbursementsAndClaimsMaster.AmountToWallet ?? 0;
            EmpCurrentPettyCashBalance empPettyCashBal = await _context.EmpCurrentPettyCashBalances.FindAsync(disbursementsAndClaimsMaster.EmployeeId);

            //if PettyCash Request then update the CashOnHand in EmpPettyCash Balances table

            if(disbursementsAndClaimsMaster.PettyCashRequestId != null )
            { 
                empPettyCashBal.CashOnHand = empPettyCashBal.CashOnHand + disbursementsAndClaimsMaster.AmountToCredit ?? 0;
                //empPettyCashBal.CurBalance = empPettyCashBal.CurBalance - empPettyCashBal.CashOnHand;
                //empPettyCashBal.CurBalance = (empPettyCashBal.CurBalance - empPettyCashBal.CashOnHand) >= 0 ? (empPettyCashBal.CurBalance - empPettyCashBal.CashOnHand) : RoleMaxLimit;
            }
            else
            {
                empPettyCashBal.CashOnHand = (empPettyCashBal.CashOnHand - CreditToWallet) >= 0 ? empPettyCashBal.CashOnHand - CreditToWallet : 0;
                empPettyCashBal.CurBalance = (empPettyCashBal.CurBalance + (disbursementsAndClaimsMaster.AmountToWallet ?? 0)) < RoleMaxLimit ? (empPettyCashBal.CurBalance + (disbursementsAndClaimsMaster.AmountToWallet ?? 0)) : RoleMaxLimit;
            }
            

           
            _context.Update(empPettyCashBal);





            disbursementsAndClaimsMaster.IsSettledAmountCredited = disbursementsAndClaimsMasterDTO.IsSettledAmountCredited;
            disbursementsAndClaimsMaster.SettledDate = DateTime.Now;
            disbursementsAndClaimsMaster.SettlementComment = disbursementsAndClaimsMasterDTO.SettlementComment;
            disbursementsAndClaimsMaster.SettlementAccount = disbursementsAndClaimsMasterDTO.SettlementAccount;
            disbursementsAndClaimsMaster.SettlementBankCard = disbursementsAndClaimsMasterDTO.SettlementBankCard;
            disbursementsAndClaimsMaster.AdditionalData = disbursementsAndClaimsMasterDTO.AdditionalData;

            _context.DisbursementsAndClaimsMasters.Update(disbursementsAndClaimsMaster);
            //_context.Entry(disbursementsAndClaimsMasterDTO).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Accounts Payable Entry Updated!" });
        }

       

    }
}
