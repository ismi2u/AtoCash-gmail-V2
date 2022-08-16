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
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, Manager, User")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public DepartmentsController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("DepartmentsForDropdown")]
        public async Task<ActionResult<IEnumerable<DepartmentVM>>> GetDepartmentsForDropdown()
        {
            List<DepartmentVM> ListDepartmentVM = new List<DepartmentVM>();

            var departments = await _context.Departments.Where(d => d.StatusTypeId == (int)EStatusType.Active).ToListAsync();
            foreach (Department department in departments)
            {
                DepartmentVM departmentVM = new DepartmentVM
                {
                    Id = department.Id,
                    DeptDesc = department.DeptCode + ":" + department.DeptName
                };

                ListDepartmentVM.Add(departmentVM);
            }
            return ListDepartmentVM;

        }
        [HttpGet("{id}")]
        [ActionName("DepartmentsForDropdownByCostCentre")]
        public async Task<ActionResult<IEnumerable<DepartmentVM>>> GetDepartmentsForDropdownByCostCentre(int id)
        {
            List<DepartmentVM> ListDepartmentVM = new List<DepartmentVM>();

            var departments = await _context.Departments.Where(d => d.StatusTypeId == (int)EStatusType.Active && d.CostCenterId == id).ToListAsync();
            foreach (Department department in departments)
            {
                DepartmentVM departmentVM = new DepartmentVM
                {
                    Id = department.Id,
                    DeptDesc = department.DeptCode + ":" + department.DeptName
                };

                ListDepartmentVM.Add(departmentVM);
            }
            return ListDepartmentVM;

        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetDepartments()
        {
            List<DepartmentDTO> ListDepartmentDTO = new List<DepartmentDTO>();

            var departments = await _context.Departments.ToListAsync();

            foreach (Department department in departments)
            {
                DepartmentDTO departmentDTO = new DepartmentDTO
                {
                    Id = department.Id,
                    DeptCode = department.DeptCode,
                    DeptName = department.DeptName,
                    CostCenterId = department.CostCenterId,
                    CostCenter = _context.CostCenters.Find(department.CostCenterId).CostCenterCode,
                    StatusTypeId = department.StatusTypeId,
                    StatusType = _context.StatusTypes.Find(department.StatusTypeId).Status

                };

                ListDepartmentDTO.Add(departmentDTO);

            }

            return ListDepartmentDTO;
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDTO>> GetDepartment(int id)
        {
            DepartmentDTO departmentDTO = new DepartmentDTO();

            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Department Id invalid!" });
            }

            departmentDTO.Id = department.Id;
            departmentDTO.DeptCode = department.DeptCode;
            departmentDTO.DeptName = department.DeptName;
            departmentDTO.CostCenterId = department.CostCenterId;
            departmentDTO.CostCenter = _context.CostCenters.Find(department.CostCenterId).CostCenterCode;
            departmentDTO.StatusTypeId = department.StatusTypeId;
            departmentDTO.StatusType = _context.StatusTypes.Find(department.StatusTypeId).Status;

            return departmentDTO;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutDepartment(int id, DepartmentDTO departmentDto)
        {
            if (id != departmentDto.Id)
            {
                return Conflict(new Authentication.RespStatus { Status = "Failure", Message = "Id not Valid for Department" });
            }

            var department = await _context.Departments.FindAsync(id);

            department.DeptName = departmentDto.DeptName;
            department.CostCenterId = departmentDto.CostCenterId;
            department.StatusTypeId = departmentDto.StatusTypeId;

            _context.Departments.Update(department);
            //_context.Entry(projectDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return Conflict(new RespStatus { Status = "Failure", Message = "Department is invalid" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new RespStatus { Status = "Success", Message = "Department Details Updated!" });
        }

        // POST: api/Departments
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<Department>> PostDepartment(DepartmentDTO departmentDto)
        {
            var dept = _context.Departments.Where(c => c.DeptCode == departmentDto.DeptCode).FirstOrDefault();
            if (dept != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Department Already Exists" });
            }

            Department department = new Department
            {
                DeptCode = departmentDto.DeptCode,
                DeptName = departmentDto.DeptName,
                CostCenterId = departmentDto.CostCenterId,
                StatusTypeId = departmentDto.StatusTypeId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Department Created!" });
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {

            var emp = _context.Employees.Where(e => e.DepartmentId == id).FirstOrDefault();

            if (emp != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Department in Use - Can't delete" });
            }


            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Department Id invalid!" });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Department Deleted!" });
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }


        //
    }
}
