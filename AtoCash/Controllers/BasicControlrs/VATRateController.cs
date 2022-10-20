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
    public class VATRateController : ControllerBase
    {
        private readonly AtoCashDbContext _context;

        public VATRateController(AtoCashDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ActionName("GetVATPercentage")]
        public ActionResult<VATRate> GetVATPercentage()
        {

            var VATRate = _context.VATRates.Where(r => r.VATPercentage != 0).SingleOrDefault();

            return VATRate;

        }

        // PUT: api/Bank/5
        [HttpPut("{id}")]
      [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutVATRate(int id, VATRate VATRate)
        {
            if (id != VATRate.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var updVATRate = await _context.VATRates.FindAsync(id);
            updVATRate.VATPercentage = VATRate.VATPercentage;
            _context.VATRates.Update(updVATRate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "VAT Rate Updated!" });
        }

     
    }
}
