using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtoCash.Models
{
    public enum EApprovalStatus
    {
        Initiating = 1,
        Pending,
        InReview,
        Approved,
        Rejected

    }


    public enum ERequestType
    {
        CashAdvance = 1,
        ExpenseReim

    }
}
