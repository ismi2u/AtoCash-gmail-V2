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
    public class RequestsController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public RequestsController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("RequestTypesForDropdown")]
        public async Task<ActionResult<IEnumerable<RequestTypeVM>>> GetRequestTypesForDropDown()
        {
            List<RequestTypeVM> ListRequestTypeVM = new List<RequestTypeVM>();

            var requestTypes = await _context.RequestTypes.ToListAsync();
            foreach (RequestType requestType in requestTypes)
            {
                RequestTypeVM requestTypeVM = new RequestTypeVM
                {
                    Id = requestType.Id,
                     RequestName = requestType.RequestName
                };

                ListRequestTypeVM.Add(requestTypeVM);
            }

            return ListRequestTypeVM;

        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestType>>> GetRequestTypes()
        {
            return await _context.RequestTypes.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestType>> GetRequestType(int id)
        {
            var requestType = await _context.RequestTypes.FindAsync(id);

            if (requestType == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Request Type Id is Invalid!" });
            }

            return requestType;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutRequestType(int id, RequestType requestType)
        {
            if (id != requestType.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var rType = await _context.RequestTypes.FindAsync(id);
            rType.RequestTypeDesc = requestType.RequestTypeDesc;
            _context.RequestTypes.Update(rType);

            _context.Entry(requestType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Request Type Updated!" });
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<ActionResult<RequestType>> PostRequestType(RequestType requestType)
        {
            var ReqType = _context.RequestTypes.Where(r => r.RequestName == requestType.RequestName).FirstOrDefault();
            if (ReqType != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "RequestType Already Exists" });
            }

            _context.RequestTypes.Add(requestType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequestType", new { id = requestType.Id }, requestType);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteRequestType(int id)
        {
            var requestType = await _context.RequestTypes.FindAsync(id);
            if (requestType == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id Is Invalid!" });
            }

            _context.RequestTypes.Remove(requestType);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Request Type Deleted!" });
        }

        private bool RequestTypeExists(int id)
        {
            return _context.RequestTypes.Any(e => e.Id == id);
        }
    }
}
