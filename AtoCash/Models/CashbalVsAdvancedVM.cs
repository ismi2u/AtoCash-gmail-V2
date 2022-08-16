using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{


   public class CashbalVsAdvancedVM
    {
        public double CurCashBal { get; set; }
        public double MaxCashAllowed { get; set; }
    }

    
}
