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
    public class ExpenseTypesController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public ExpenseTypesController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("ExpenseTypesForDropdown")]
        public async Task<ActionResult<IEnumerable<ExpenseTypeVM>>> GetExpenseTypesForDropdown()
        {
            List<ExpenseTypeVM> ListExpenseTypeVM = new();

            var expenseTypes = await _context.ExpenseTypes.Where(c => c.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (ExpenseType expenseType in expenseTypes)
            {
                ExpenseTypeVM expenseTypeVM = new()
                {
                    Id = expenseType.Id,
                    ExpenseTypeName = expenseType.ExpenseTypeName + ":" +  expenseType.ExpenseTypeDesc,
                };

                ListExpenseTypeVM.Add(expenseTypeVM);
            }

            return ListExpenseTypeVM;

        }
        // GET: api/ExpenseTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseTypeDTO>>> GetExpenseTypes()
        {
            List<ExpenseTypeDTO> ListExpenseTypeDTO = new();

            var expenseTypes = await _context.ExpenseTypes.ToListAsync();

            foreach (ExpenseType expenseType in expenseTypes)
            {
                ExpenseTypeDTO expenseTypeDTO = new()
                {
                    Id = expenseType.Id,
                    ExpenseTypeName = expenseType.ExpenseTypeName,
                    ExpenseTypeDesc = expenseType.ExpenseTypeDesc,
                    StatusTypeId = expenseType.StatusTypeId,
                    StatusType = _context.StatusTypes.Find(expenseType.StatusTypeId).Status
                };

                ListExpenseTypeDTO.Add(expenseTypeDTO);

            }
            return Ok(ListExpenseTypeDTO);
        }

        // GET: api/ExpenseTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseTypeDTO>> GetExpenseType(int id)
        {
            var expenseType = await _context.ExpenseTypes.FindAsync(id);

            if (expenseType == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Type id is Invalid!" });
            }

            ExpenseTypeDTO expenseTypeDTO = new()
            {
                Id = expenseType.Id,
                ExpenseTypeName = expenseType.ExpenseTypeName,
                ExpenseTypeDesc = expenseType.ExpenseTypeDesc,
                StatusTypeId = expenseType.StatusTypeId,
                StatusType = _context.StatusTypes.Find(expenseType.StatusTypeId).Status
            };

            return expenseTypeDTO;
        }

        // PUT: api/ExpenseTypes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutExpenseType(int id, ExpenseTypeDTO expenseTypeDTO)
        {
            if (id != expenseTypeDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var expType = await _context.ExpenseTypes.FindAsync(id);

            expType.ExpenseTypeName = expenseTypeDTO.ExpenseTypeName;
            expType.ExpenseTypeDesc = expenseTypeDTO.ExpenseTypeDesc;
            expType.StatusTypeId = expenseTypeDTO.StatusTypeId;
            _context.ExpenseTypes.Update(expType);

            //_context.Entry(expenseType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Expsense Type Details Updated!" });
        }

        // POST: api/ExpenseTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<ExpenseType>> PostExpenseType(ExpenseTypeDTO expenseTypeDTO)
        {
            var eType = _context.ExpenseTypes.Where(e => e.ExpenseTypeName == expenseTypeDTO.ExpenseTypeName).FirstOrDefault();
            if (eType != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Type Already Exists" });
            }

            ExpenseType expenseType = new();
            expenseType.ExpenseTypeName = expenseTypeDTO.ExpenseTypeName;
            expenseType.ExpenseTypeDesc = expenseTypeDTO.ExpenseTypeDesc;
            expenseType.StatusTypeId = expenseTypeDTO.StatusTypeId;
            _context.ExpenseTypes.Add(expenseType);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Expense Type Created!" });
        }

        // DELETE: api/ExpenseTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteExpenseType(int id)
        {

            var expenseType = await _context.ExpenseTypes.FindAsync(id);
            if (expenseType == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense-Type Id Invalid!" });
            }

            var expReimburse = _context.ExpenseSubClaims.Where(d => d.ExpenseTypeId == id).FirstOrDefault();

            if (expReimburse != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense-Type in use for Expense Reimburse!" });
            }

            _context.ExpenseTypes.Remove(expenseType);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Expense-Type Deleted!" });
        }

       


  
        ///


    }
}
