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

namespace AtoCash.Controllers.ExpenseReimburse
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class ExpenseSubClaimsController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public ExpenseSubClaimsController(AtoCashDbContext context)
        {
            _context = context;
        }

        // GET: api/ExpenseSubClaims
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseSubClaimDTO>>> GetExpenseSubClaims()
        {
           List<ExpenseSubClaim> expenseSubClaims = await _context.ExpenseSubClaims.ToListAsync();


            List<ExpenseSubClaimDTO> expenseSubClaimDTOs = new();

            foreach (ExpenseSubClaim expenseSubClaim in expenseSubClaims)
            {
                // Expense reimburse request Id
                int expReimbReqId = _context.ExpenseSubClaims.Find(expenseSubClaim.Id).ExpenseReimburseRequestId;
                ExpenseReimburseRequest expReimReq = _context.ExpenseReimburseRequests.Find(expReimbReqId);
                int empId = _context.ExpenseReimburseRequests.Find(expReimbReqId).EmployeeId;
                string empFullName = _context.Employees.Find(empId).GetFullName();
                //

                ExpenseSubClaimDTO expenseSubClaimsDto = new();
                expenseSubClaimsDto.Id = expenseSubClaim.Id;
                expenseSubClaimsDto.EmployeeId = empId;
                expenseSubClaimsDto.EmployeeName = empFullName;
                expenseSubClaimsDto.ExpenseReimbClaimAmount = expenseSubClaim.ExpenseReimbClaimAmount;
                expenseSubClaimsDto.DocumentIDs = expenseSubClaim.DocumentIDs;
                expenseSubClaimsDto.ExpReimReqDate = expReimReq.ExpReimReqDate;
                expenseSubClaimsDto.InvoiceNo = expenseSubClaim.InvoiceNo;
                expenseSubClaimsDto.InvoiceDate = expenseSubClaim.InvoiceDate;
                expenseSubClaimsDto.Tax = expenseSubClaim.Tax;
                expenseSubClaimsDto.TaxAmount = expenseSubClaim.TaxAmount;
                expenseSubClaimsDto.Vendor = expenseSubClaim.Vendor;
                expenseSubClaimsDto.Location = expenseSubClaim.Location;
                expenseSubClaimsDto.Description = expenseSubClaim.Description;
                expenseSubClaimsDto.CurrencyTypeId = expReimReq.CurrencyTypeId;
                expenseSubClaimsDto.CurrencyType = _context.CurrencyTypes.Find(expReimReq.CurrencyTypeId).CurrencyCode;
                expenseSubClaimsDto.ExpenseTypeId = expenseSubClaim.ExpenseTypeId;
                expenseSubClaimsDto.ExpenseType = _context.ExpenseTypes.Find(expenseSubClaim.ExpenseTypeId).ExpenseTypeName;
                expenseSubClaimsDto.DepartmentName = _context.Departments.Find(expReimReq.DepartmentId).DeptName;
                expenseSubClaimsDto.DepartmentId = expReimReq.DepartmentId;
                expenseSubClaimsDto.ProjectName = _context.Projects.Find(expReimReq.ProjectId).ProjectName;
                expenseSubClaimsDto.ProjectId = expReimReq.ProjectId;
                expenseSubClaimsDto.SubProjectName = _context.SubProjects.Find(expReimReq.SubProjectId).SubProjectName;
                expenseSubClaimsDto.SubProjectId = expReimReq.SubProjectId;
                expenseSubClaimsDto.WorkTaskName = _context.WorkTasks.Find(expReimReq.WorkTaskId).TaskName;
                expenseSubClaimsDto.WorkTaskId = expReimReq.WorkTaskId;


                expenseSubClaimsDto.ApprovalStatusType = _context.ApprovalStatusTypes.Find(expReimReq.ApprovalStatusTypeId).Status;
                expenseSubClaimsDto.ApprovalStatusTypeId = expReimReq.ApprovalStatusTypeId;
                expenseSubClaimsDto.ApprovedDate = expReimReq.ApprovedDate;


                expenseSubClaimDTOs.Add(expenseSubClaimsDto);
            }


            return Ok(expenseSubClaimDTOs);
        }

        // GET: api/ExpenseSubClaims/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseSubClaimDTO>> GetExpenseSubClaim(int id)
        {
            var expenseSubClaim = await _context.ExpenseSubClaims.FindAsync(id);

            if (expenseSubClaim == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Id is not Valid!" });
            }


            // Expense reimburse request Id
            int expReimbReqId = _context.ExpenseSubClaims.Find(expenseSubClaim.Id).ExpenseReimburseRequestId;
            ExpenseReimburseRequest expReimReq = _context.ExpenseReimburseRequests.Find(expReimbReqId);
            int empId = _context.ExpenseReimburseRequests.Find(expReimbReqId).EmployeeId;
            string empFullName = _context.Employees.Find(empId).GetFullName();
            //

            ExpenseSubClaimDTO expenseSubClaimsDto = new();
            expenseSubClaimsDto.Id = expenseSubClaim.Id;
            expenseSubClaimsDto.EmployeeId = empId;
            expenseSubClaimsDto.EmployeeName = empFullName;
            expenseSubClaimsDto.ExpenseReimbClaimAmount = expenseSubClaim.ExpenseReimbClaimAmount;
            expenseSubClaimsDto.DocumentIDs = expenseSubClaim.DocumentIDs;
            expenseSubClaimsDto.ExpReimReqDate = expReimReq.ExpReimReqDate;
            expenseSubClaimsDto.InvoiceNo = expenseSubClaim.InvoiceNo;
            expenseSubClaimsDto.InvoiceDate = expenseSubClaim.InvoiceDate;
            expenseSubClaimsDto.Tax = expenseSubClaim.Tax;
            expenseSubClaimsDto.TaxAmount = expenseSubClaim.TaxAmount;
            expenseSubClaimsDto.Vendor = expenseSubClaim.Vendor;
            expenseSubClaimsDto.Location = expenseSubClaim.Location;
            expenseSubClaimsDto.Description = expenseSubClaim.Description;
            expenseSubClaimsDto.CurrencyTypeId = expReimReq.CurrencyTypeId;
            expenseSubClaimsDto.CurrencyType = _context.CurrencyTypes.Find(expReimReq.CurrencyTypeId).CurrencyCode;
            expenseSubClaimsDto.ExpenseTypeId = expenseSubClaim.ExpenseTypeId;
            expenseSubClaimsDto.ExpenseType = _context.ExpenseTypes.Find(expenseSubClaim.ExpenseTypeId).ExpenseTypeName;
            expenseSubClaimsDto.DepartmentName = expReimReq.DepartmentId != null ? _context.Departments.Find(expReimReq.DepartmentId).DeptName : null;
            expenseSubClaimsDto.DepartmentId = expReimReq.DepartmentId;
            expenseSubClaimsDto.ProjectName = expReimReq.ProjectId != null ? _context.Projects.Find(expReimReq.ProjectId).ProjectName : null;
            expenseSubClaimsDto.ProjectId = expReimReq.ProjectId;
            expenseSubClaimsDto.SubProjectName = expReimReq.SubProjectId != null ? _context.SubProjects.Find(expReimReq.SubProjectId).SubProjectName : null;
            expenseSubClaimsDto.SubProjectId = expReimReq.SubProjectId;
            expenseSubClaimsDto.WorkTaskName = expReimReq.WorkTaskId != null ? _context.WorkTasks.Find(expReimReq.WorkTaskId).TaskName : null;
            expenseSubClaimsDto.WorkTaskId = expReimReq.WorkTaskId;

            expenseSubClaimsDto.ApprovalStatusType = _context.ApprovalStatusTypes.Find(expReimReq.ApprovalStatusTypeId).Status;
            expenseSubClaimsDto.ApprovalStatusTypeId = expReimReq.ApprovalStatusTypeId;
            expenseSubClaimsDto.ApprovedDate = expReimReq.ApprovedDate;


            

            return Ok(expenseSubClaimsDto);
        }


        // GET: api/ExpenseSubClaims/5
        [HttpGet("{id}")]
        [ActionName("GetExpenseSubClaimsByExpenseId")]
        public async Task<ActionResult<List<ExpenseSubClaimDTO>>> GetExpenseSubClaimsByExpenseId(int id)
        {
            if (id == 0)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Id is not Valid!" });
            }

            var expenseSubClaims = await _context.ExpenseSubClaims.Where(e => e.ExpenseReimburseRequestId == id).ToListAsync();
            
            
            List<ExpenseSubClaimDTO> expenseSubClaimDTOs = new();
   
            foreach (ExpenseSubClaim expenseSubClaim in expenseSubClaims)
            {
                // Expense reimburse request Id
                int expReimbReqId =    _context.ExpenseSubClaims.Find(expenseSubClaim.Id).ExpenseReimburseRequestId;
                ExpenseReimburseRequest expReimReq = _context.ExpenseReimburseRequests.Find(expReimbReqId);
                int empId = _context.ExpenseReimburseRequests.Find(expReimbReqId).EmployeeId;
                string empFullName = _context.Employees.Find(empId).GetFullName();
                //

                ExpenseSubClaimDTO expenseSubClaimsDto = new();
                expenseSubClaimsDto.Id = expenseSubClaim.Id;
                expenseSubClaimsDto.EmployeeId = empId;
                expenseSubClaimsDto.EmployeeName = empFullName;
                expenseSubClaimsDto.ExpenseReimbClaimAmount = expenseSubClaim.ExpenseReimbClaimAmount;
                expenseSubClaimsDto.DocumentIDs = expenseSubClaim.DocumentIDs;
                expenseSubClaimsDto.ExpReimReqDate = expReimReq.ExpReimReqDate;
                expenseSubClaimsDto.InvoiceNo = expenseSubClaim.InvoiceNo;
                expenseSubClaimsDto.InvoiceDate = expenseSubClaim.InvoiceDate;
                expenseSubClaimsDto.Tax = expenseSubClaim.Tax;
                expenseSubClaimsDto.TaxAmount = expenseSubClaim.TaxAmount;
                expenseSubClaimsDto.Vendor = expenseSubClaim.Vendor;
                expenseSubClaimsDto.Location = expenseSubClaim.Location;
                expenseSubClaimsDto.Description = expenseSubClaim.Description;
                expenseSubClaimsDto.CurrencyTypeId = expReimReq.CurrencyTypeId;
                expenseSubClaimsDto.CurrencyType = _context.CurrencyTypes.Find(expReimReq.CurrencyTypeId).CurrencyCode;
                expenseSubClaimsDto.ExpenseTypeId = expenseSubClaim.ExpenseTypeId;
                expenseSubClaimsDto.ExpenseType = _context.ExpenseTypes.Find(expenseSubClaim.ExpenseTypeId).ExpenseTypeName;
                expenseSubClaimsDto.DepartmentName = expReimReq.DepartmentId != null ? _context.Departments.Find(expReimReq.DepartmentId).DeptName : null;
                expenseSubClaimsDto.DepartmentId = expReimReq.DepartmentId;

                
                expenseSubClaimsDto.ProjectName = expReimReq.ProjectId != null ? _context.Projects.Find(expReimReq.ProjectId).ProjectName : null;
                expenseSubClaimsDto.ProjectId = expReimReq.ProjectId;
                expenseSubClaimsDto.SubProjectName = expReimReq.SubProjectId != null ? _context.SubProjects.Find(expReimReq.SubProjectId).SubProjectName : null;
                expenseSubClaimsDto.SubProjectId = expReimReq.SubProjectId;
                expenseSubClaimsDto.WorkTaskName = expReimReq.WorkTaskId != null ? _context.WorkTasks.Find(expReimReq.WorkTaskId).TaskName : null;
                expenseSubClaimsDto.WorkTaskId = expReimReq.WorkTaskId;

                expenseSubClaimsDto.ApprovalStatusType = _context.ApprovalStatusTypes.Find(expReimReq.ApprovalStatusTypeId).Status;
                expenseSubClaimsDto.ApprovalStatusTypeId = expReimReq.ApprovalStatusTypeId;
                expenseSubClaimsDto.ApprovedDate = expReimReq.ApprovedDate;


                expenseSubClaimDTOs.Add(expenseSubClaimsDto);
            }

            return Ok(expenseSubClaimDTOs);
        }
        // PUT: api/ExpenseSubClaims/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpenseSubClaim(int id, ExpenseSubClaim expenseSubClaim)
        {
            if (id != expenseSubClaim.Id)
            {
                return BadRequest();
            }

            _context.Entry(expenseSubClaim).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseSubClaimExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new RespStatus { Status = "Success", Message = "Expense Approval Updated!" });
        }

        // POST: api/ExpenseSubClaims
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExpenseSubClaim>> PostExpenseSubClaim(ExpenseSubClaim expenseSubClaim)
        {
            _context.ExpenseSubClaims.Add(expenseSubClaim);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpenseSubClaim", new { id = expenseSubClaim.Id }, expenseSubClaim);
        }

        // DELETE: api/ExpenseSubClaims/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseSubClaim(int id)
        {
            var expenseSubClaim = await _context.ExpenseSubClaims.FindAsync(id);
            if (expenseSubClaim == null)
            {
                return NotFound();
            }

            _context.ExpenseSubClaims.Remove(expenseSubClaim);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Expsense Sub Claim Deleted!" });
        }

        private bool ExpenseSubClaimExists(int id)
        {
            return _context.ExpenseSubClaims.Any(e => e.Id == id);
        }
    }
}
