using AtoCash.Data;
using AtoCash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Controllers.CashAdvance
{
    public class PettyCashBalanceAdd
    {
        private readonly AtoCashDbContext _context;

        public PettyCashBalanceAdd(AtoCashDbContext context)
        {
            this._context = context;
           
        }
       
    }
}
