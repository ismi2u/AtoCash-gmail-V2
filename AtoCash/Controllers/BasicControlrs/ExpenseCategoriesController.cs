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
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public ExpenseCategoriesController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("GetSelectedExpenseCategoriesForDropdown")]
        public async Task<ActionResult<IEnumerable<ExpenseCategoryVM>>> GetSelectedExpenseCategoriesForDropdown(bool isBussCategory)
        {
            List<ExpenseCategoryVM> ListExpenseCategoryVM = new();

            var expenseCategories = await _context.ExpenseCategories.Where(c => c.StatusTypeId == (int)EStatusType.Active && c.IsBusinessCategory== isBussCategory).ToListAsync();
            foreach (ExpenseCategory expenseCategory in expenseCategories)
            {
                ExpenseCategoryVM expenseCategoryVM = new()
                {
                    Id = expenseCategory.Id,
                    ExpenseCategoryName = expenseCategory.ExpenseCategoryName + ":" +  expenseCategory.ExpenseCategoryDesc,
                };

                ListExpenseCategoryVM.Add(expenseCategoryVM);
            }

            return ListExpenseCategoryVM;

        }



        [HttpGet]
        [ActionName("ExpenseCategoriesForDropdown")]
        public async Task<ActionResult<IEnumerable<ExpenseCategoryVM>>> GetExpenseCategoriesForDropdown()
        {
            List<ExpenseCategoryVM> ListExpenseCategoryVM = new();

            var expenseCategories = await _context.ExpenseCategories.Where(c => c.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (ExpenseCategory expenseCategory in expenseCategories)
            {
                ExpenseCategoryVM expenseCategoryVM = new()
                {
                    Id = expenseCategory.Id,
                    ExpenseCategoryName = expenseCategory.ExpenseCategoryName + ":" + expenseCategory.ExpenseCategoryDesc,
                };

                ListExpenseCategoryVM.Add(expenseCategoryVM);
            }

            return ListExpenseCategoryVM;

        }
        // GET: api/ExpenseCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseCategoryDTO>>> GetExpenseCategories()
        {
            List<ExpenseCategoryDTO> ListExpenseCategoryDTO = new();

            var expenseCategories = await _context.ExpenseCategories.ToListAsync();

            foreach (ExpenseCategory expenseCategory in expenseCategories)
            {
                ExpenseCategoryDTO expenseCategoryDTO = new()
                {
                    Id = expenseCategory.Id,
                    ExpenseCategoryName = expenseCategory.ExpenseCategoryName,
                    ExpenseCategoryDesc = expenseCategory.ExpenseCategoryDesc,
                    IsBusinessCategory = expenseCategory.IsBusinessCategory,
                    StatusTypeId = expenseCategory.StatusTypeId,
                    StatusType = _context.StatusTypes.Find(expenseCategory.StatusTypeId).Status
                };

                ListExpenseCategoryDTO.Add(expenseCategoryDTO);

            }
            return Ok(ListExpenseCategoryDTO);
        }

        // GET: api/ExpenseCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseCategoryDTO>> GetExpenseCategory(int id)
        {
            var expenseCategory = await _context.ExpenseCategories.FindAsync(id);

            if (expenseCategory == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Category id is Invalid!" });
            }

            ExpenseCategoryDTO expenseCategoryDTO = new()
            {
                Id = expenseCategory.Id,
                ExpenseCategoryName = expenseCategory.ExpenseCategoryName,
                ExpenseCategoryDesc = expenseCategory.ExpenseCategoryDesc,
                IsBusinessCategory = expenseCategory.IsBusinessCategory,
                StatusTypeId = expenseCategory.StatusTypeId,
                StatusType = _context.StatusTypes.Find(expenseCategory.StatusTypeId).Status
            };

            return expenseCategoryDTO;
        }

        // PUT: api/ExpenseCategories/5
        [HttpPut("{id}")]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutExpenseCategory(int id, ExpenseCategoryDTO expenseCategoryDTO)
        {
            if (id != expenseCategoryDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var expCategory = await _context.ExpenseCategories.FindAsync(id);

            expCategory.ExpenseCategoryDesc = expenseCategoryDTO.ExpenseCategoryDesc;
            expCategory.IsBusinessCategory = expenseCategoryDTO.IsBusinessCategory;
            expCategory.StatusTypeId = expenseCategoryDTO.StatusTypeId;
            _context.ExpenseCategories.Update(expCategory);

            //_context.Entry(expenseCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Expsense Category Details Updated!" });
        }

        // POST: api/ExpenseCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<ExpenseCategory>> PostExpenseCategory(ExpenseCategoryDTO expenseCategoryDTO)
        {
            var eCategory = _context.ExpenseCategories.Where(e => e.ExpenseCategoryName == expenseCategoryDTO.ExpenseCategoryName).FirstOrDefault();
            if (eCategory != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Category already exists" });
            }

            ExpenseCategory expenseCategory = new();
            expenseCategory.ExpenseCategoryName = expenseCategoryDTO.ExpenseCategoryName;
            expenseCategory.ExpenseCategoryDesc = expenseCategoryDTO.ExpenseCategoryDesc;
            expenseCategory.IsBusinessCategory = expenseCategoryDTO.IsBusinessCategory;
            expenseCategory.StatusTypeId = expenseCategoryDTO.StatusTypeId;
            _context.ExpenseCategories.Add(expenseCategory);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Expense Category Created!" });
        }

        // DELETE: api/ExpenseCategories/5
        [HttpDelete("{id}")]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteExpenseCategory(int id)
        {

            var expenseCategory = await _context.ExpenseCategories.FindAsync(id);
            if (expenseCategory == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense-Category Id Invalid!" });
            }

            //Commented as of now, till we create a Column in Expense Sub Claim Table */ 
            var expType = _context.ExpenseTypes.Where(d => d.ExpenseCategoryId == id).FirstOrDefault();

            if (expType != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense-Category in use for Expense Type!" });
            }

            _context.ExpenseCategories.Remove(expenseCategory);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Expense-Category Deleted!" });
        }

       


  
        ///


    }
}
