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
using EmailService;
using System.IO;
using Microsoft.Extensions.Logging;

namespace AtoCash.Controllers.ExpenseReimburse
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class ExpenseReimburseStatusTrackersController : ControllerBase
    {
        private readonly AtoCashDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExpenseReimburseStatusTrackersController> _logger;

        public ExpenseReimburseStatusTrackersController(AtoCashDbContext context,
                                                        IEmailSender emailSender,
                                                        ILogger<ExpenseReimburseStatusTrackersController> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET: api/ExpenseReimburseStatusTrackers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseReimburseStatusTracker>>> GetExpenseReimburseStatusTrackers()
        {
            return await _context.ExpenseReimburseStatusTrackers.ToListAsync();
        }

        // GET: api/ExpenseReimburseStatusTrackers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseReimburseStatusTracker>> GetExpenseReimburseStatusTracker(int id)
        {
            var expenseReimburseStatusTracker = await _context.ExpenseReimburseStatusTrackers.FindAsync(id);

            if (expenseReimburseStatusTracker == null)
            {
                _logger.LogError("expenseReimburseStatusTracker Id is not valid:" + id);
                return NotFound();
            }

            return expenseReimburseStatusTracker;
        }


        [HttpGet("{id}")]
        [ActionName("ApprovalFlowForRequest")]
        public ActionResult<IEnumerable<ApprovalStatusFlowVM>> GetApprovalFlowForRequest(int id)
        {

            if (id == 0)
            {
                _logger.LogError("GetApprovalFlowForRequest - Id is not valid:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense-Reimburse Request Id is Invalid" });
            }


            var expenseReimburseStatusTrackers = _context.ExpenseReimburseStatusTrackers.Where(e => e.ExpenseReimburseRequestId == id).FirstOrDefault();


            if (expenseReimburseStatusTrackers == null)
            {
                _logger.LogError("Expense-Reimburse Status Tracker Request Id is returning null records:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense-Reimburse Request Id is Not Found" });
            }

            bool isBussAreaReq = (bool)_context.ExpenseReimburseStatusTrackers.Where(e => e.ExpenseReimburseRequestId == id).FirstOrDefault().IsBusinessAreaReq;

            var ListOfExpReimStatusTrackers = isBussAreaReq ? _context.ExpenseReimburseStatusTrackers.Where(e =>
                                e.ExpenseReimburseRequestId == id).ToList().OrderBy(x => x.BARoleId) : _context.ExpenseReimburseStatusTrackers.Where(e =>
                                e.ExpenseReimburseRequestId == id).ToList().OrderBy(x => x.JobRoleId);

            if (expenseReimburseStatusTrackers == null)
            {
                _logger.LogError("Expense-Reimburse Status Tracker Request Id is returning null records:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Status Tracker Request Id is returning null records:" + id });
            }


            List<ApprovalStatusFlowVM> ListApprovalStatusFlow = new();

            foreach (ExpenseReimburseStatusTracker statusTracker in ListOfExpReimStatusTrackers)
            {
                string claimApproverName = null;

                if (statusTracker.ProjectId > 0)
                {
                    claimApproverName = _context.Employees.Where(e => e.Id == _context.Projects.Find(statusTracker.ProjectId).ProjectManagerId)
                        .Select(s => s.GetFullName()).FirstOrDefault();
                }
                else if (statusTracker.IsBusinessAreaReq == true)
                {
                    claimApproverName = _context.Employees.Where(x => (x.BusinessAreaRoleId == statusTracker.BARoleId) && (x.BusinessAreaApprovalGroupId == statusTracker.BAApprovalGroupId))
                       .Select(s => s.GetFullName()).FirstOrDefault();
                }
                else
                {
                    claimApproverName = _context.Employees.Where(x => (x.RoleId == statusTracker.JobRoleId) && (x.ApprovalGroupId == statusTracker.ApprovalGroupId))
                        .Select(s => s.GetFullName()).FirstOrDefault();
                }

                ApprovalStatusFlowVM approvalStatusFlow = new();
                approvalStatusFlow.ApprovalLevel = statusTracker.ApprovalLevelId;

                if (statusTracker.IsBusinessAreaReq == true)
                {
                    approvalStatusFlow.ApproverRole = _context.JobRoles.Find(statusTracker.BARoleId).RoleName;
                }
                else if (statusTracker.ProjectId != null)
                {
                    approvalStatusFlow.ApproverRole = "Project Manager";
                }
                else
                {
                    approvalStatusFlow.ApproverRole = _context.JobRoles.Find(statusTracker.JobRoleId).RoleName;
                }
                approvalStatusFlow.ApproverName = claimApproverName;
                approvalStatusFlow.ApprovedDate = statusTracker.ApprovedDate;
                approvalStatusFlow.ApprovalStatusType = _context.ApprovalStatusTypes.Find(statusTracker.ApprovalStatusTypeId).Status;
                ListApprovalStatusFlow.Add(approvalStatusFlow);
            }

            return Ok(ListApprovalStatusFlow);

        }


        [HttpGet("{id}")]
        [ActionName("ApprovalsPendingForApprover")]
        public ActionResult<IEnumerable<ClaimApprovalStatusTrackerDTO>> ApprovalsPendingForApprover(int id)
        {


            if (id == 0)
            {
                _logger.LogError("ApprovalsPendingForApprover Employee Id is Invalid:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee Id is Invalid" });
            }


            //get the RoleID of the Employee (Approver)
            Employee apprEmp = _context.Employees.Find(id);
            int jobRoleid = apprEmp.RoleId;
            int apprGroupId = apprEmp.ApprovalGroupId;
            int BARoleid = (int)apprEmp.BusinessAreaRoleId;
            int BAApprovalGroupId = (int)apprEmp.BusinessAreaApprovalGroupId;

            if (jobRoleid == 0)
            {
                _logger.LogError("ApprovalsPendingForApprover JobRole Id is Invalid:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "JobRole Id is Invalid" });
            }

            var expenseReimburseStatusTrackers = _context.ExpenseReimburseStatusTrackers
                                .Where(r =>
                                    (r.JobRoleId == jobRoleid || r.JobRoleId == BARoleid) &&
                                    (r.ApprovalGroupId == apprGroupId || r.ApprovalGroupId == BAApprovalGroupId) &&
                                    r.ApprovalStatusTypeId == (int)EApprovalStatus.Pending
                                    && r.ProjManagerId == null

                                    || r.ProjManagerId == id &&
                                    r.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).ToList();

            List<ExpenseReimburseStatusTrackerDTO> ListExpenseReimburseStatusTrackerDTO = new();

            foreach (ExpenseReimburseStatusTracker expenseReimburseStatusTracker in expenseReimburseStatusTrackers)
            {
                ExpenseReimburseStatusTrackerDTO expenseReimburseStatusTrackerDTO = new();

                expenseReimburseStatusTrackerDTO.Id = expenseReimburseStatusTracker.Id;
                expenseReimburseStatusTrackerDTO.EmployeeId = expenseReimburseStatusTracker.EmployeeId;
                expenseReimburseStatusTrackerDTO.EmployeeName = _context.Employees.Find(expenseReimburseStatusTracker.EmployeeId).GetFullName();
                expenseReimburseStatusTrackerDTO.ExpenseReimburseRequestId = expenseReimburseStatusTracker.ExpenseReimburseRequestId;

                expenseReimburseStatusTrackerDTO.TotalClaimAmount = expenseReimburseStatusTracker.TotalClaimAmount;
                expenseReimburseStatusTrackerDTO.BusinessAreaId = expenseReimburseStatusTracker.BusinessAreaId;
                expenseReimburseStatusTrackerDTO.BusinessArea = expenseReimburseStatusTracker.BusinessAreaId != null ? _context.BusinessAreas.Find(expenseReimburseStatusTracker.BusinessAreaId).BusinessAreaCode + ":" + _context.BusinessAreas.Find(expenseReimburseStatusTracker.BusinessAreaId).BusinessAreaName : null;
                expenseReimburseStatusTrackerDTO.DepartmentId = expenseReimburseStatusTracker.DepartmentId;
                expenseReimburseStatusTrackerDTO.Department = expenseReimburseStatusTracker.DepartmentId != null ? _context.Departments.Find(expenseReimburseStatusTracker.DepartmentId).DeptName : null;
                expenseReimburseStatusTrackerDTO.ProjectId = expenseReimburseStatusTracker.ProjectId;
                expenseReimburseStatusTrackerDTO.Project = expenseReimburseStatusTracker.ProjectId != null ? _context.Projects.Find(expenseReimburseStatusTracker.ProjectId).ProjectName : null;
                expenseReimburseStatusTrackerDTO.JobRoleId = expenseReimburseStatusTracker.JobRoleId;
                expenseReimburseStatusTrackerDTO.JobRole = _context.JobRoles.Find(expenseReimburseStatusTracker.JobRoleId).RoleName;
                expenseReimburseStatusTrackerDTO.ApprovalLevelId = expenseReimburseStatusTracker.ApprovalLevelId;
                expenseReimburseStatusTrackerDTO.ExpReimReqDate = expenseReimburseStatusTracker.ExpReimReqDate;
                expenseReimburseStatusTrackerDTO.ApprovedDate = expenseReimburseStatusTracker.ApprovedDate;
                expenseReimburseStatusTrackerDTO.ApprovalStatusTypeId = expenseReimburseStatusTracker.ApprovalStatusTypeId;
                expenseReimburseStatusTrackerDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(expenseReimburseStatusTracker.ApprovalStatusTypeId).Status;
                expenseReimburseStatusTrackerDTO.Comments = expenseReimburseStatusTracker.Comments;


                ListExpenseReimburseStatusTrackerDTO.Add(expenseReimburseStatusTrackerDTO);

            }


            return Ok(ListExpenseReimburseStatusTrackerDTO.OrderByDescending(o => o.ExpReimReqDate).ToList());

        }



        //To get the counts of pending approvals

        [HttpGet("{id}")]
        [ActionName("CountOfApprovalsPendingForApprover")]
        public ActionResult<int> GetCountOfApprovalsPendingForApprover(int id)
        {

            if (id == 0)
            {
                _logger.LogError("GetCountOfApprovalsPendingForApprover Employee id is null:" + id);
                return NotFound(new RespStatus { Status = "Failure", Message = "Employee Id is Invalid" });
            }
            //get the RoleID of the Employee (Approver)
            int Jobroleid = _context.Employees.Find(id).RoleId;

            if (Jobroleid == 0)
            {
                _logger.LogError("ApprovalsPendingForApprover JobRole Id is Invalid:" + id);
                return NotFound(new RespStatus { Status = "Failure", Message = "JobRole Id is Invalid" });
            }

            return Ok(_context.ExpenseReimburseStatusTrackers.Where(r => r.JobRoleId == Jobroleid && r.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).Count());

        }

        // PUT: api/ExpenseReimburseStatusTrackers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutExpenseReimburseStatusTracker(List<ExpenseReimburseStatusTrackerDTO> ListExpenseReimburseStatusTrackerDto)
        {


            if (ListExpenseReimburseStatusTrackerDto.Count == 0)
            {
                _logger.LogError("ListExpenseReimburseStatusTrackerDto count is 0, no object to loop");
                return Conflict(new RespStatus { Status = "Failure", Message = "No Request to Approve!" });
            }


            bool isNextApproverAvailable = true;
            bool bRejectMessage = false;
            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                _logger.LogInformation("PutExpsensReimburseStatus Tracker record updation START");
                foreach (ExpenseReimburseStatusTrackerDTO expenseReimburseStatusTrackerDto in ListExpenseReimburseStatusTrackerDto)
                {
                    var expenseReimburseStatusTracker = await _context.ExpenseReimburseStatusTrackers.FindAsync(expenseReimburseStatusTrackerDto.Id);

                    //if same status continue to next loop, otherwise process
                    if (expenseReimburseStatusTracker.ApprovalStatusTypeId == expenseReimburseStatusTrackerDto.ApprovalStatusTypeId)
                    {
                        continue;
                    }

                    if (expenseReimburseStatusTrackerDto.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected)
                    {
                        bRejectMessage = true;
                    }
                    expenseReimburseStatusTracker.Id = expenseReimburseStatusTrackerDto.Id;
                    expenseReimburseStatusTracker.EmployeeId = expenseReimburseStatusTrackerDto.EmployeeId;
                    expenseReimburseStatusTracker.ExpenseReimburseRequestId = expenseReimburseStatusTrackerDto.ExpenseReimburseRequestId;
                    expenseReimburseStatusTracker.DepartmentId = expenseReimburseStatusTrackerDto.DepartmentId;
                    expenseReimburseStatusTracker.ProjectId = expenseReimburseStatusTrackerDto.ProjectId;
                    expenseReimburseStatusTracker.JobRoleId = expenseReimburseStatusTrackerDto.JobRoleId;
                    expenseReimburseStatusTracker.ApprovalLevelId = expenseReimburseStatusTrackerDto.ApprovalLevelId;
                    expenseReimburseStatusTracker.ExpReimReqDate = expenseReimburseStatusTrackerDto.ExpReimReqDate;
                    expenseReimburseStatusTracker.ApprovedDate = expenseReimburseStatusTrackerDto.ApprovedDate;
                    expenseReimburseStatusTracker.ApprovalStatusTypeId = expenseReimburseStatusTrackerDto.ApprovalStatusTypeId;
                    expenseReimburseStatusTracker.Comments = bRejectMessage ? expenseReimburseStatusTrackerDto.Comments : "Approved";



                    ExpenseReimburseStatusTracker claimitem;
                    //Department based Expense Reimburse approval/rejection
                    if (expenseReimburseStatusTrackerDto.DepartmentId != null || expenseReimburseStatusTracker.IsBusinessAreaReq == true)
                    {

                        int empApprGroupId = 0;
                        if (expenseReimburseStatusTracker.IsBusinessAreaReq == true)
                        { empApprGroupId = (int)_context.Employees.Find(expenseReimburseStatusTracker.EmployeeId).BusinessAreaApprovalGroupId; }
                        else
                        { empApprGroupId = (int)_context.Employees.Find(expenseReimburseStatusTracker.EmployeeId).ApprovalGroupId; }


                        //Check if the record is already approved
                        //if it is not approved then trigger next approver level email & Change the status to approved
                        if (expenseReimburseStatusTrackerDto.ApprovalStatusTypeId == (int)EApprovalStatus.Approved)
                        {
                            //Get the next approval level (get its ID)
                            //int qExpReimRequestId = expenseReimburseStatusTrackerDto.ExpenseReimburseRequestId ?? 0;
                            int qExpReimRequestId = expenseReimburseStatusTrackerDto.ExpenseReimburseRequestId;

                            isNextApproverAvailable = true;

                            int CurClaimApprovalLevel = _context.ApprovalLevels.Find(expenseReimburseStatusTrackerDto.ApprovalLevelId).Level;
                            int nextClaimApprovalLevel = CurClaimApprovalLevel + 1;
                            int qApprovalLevelId;
                            int apprGroupId = (int)_context.Employees.Find(expenseReimburseStatusTracker.EmployeeId).ApprovalGroupId;

                            if (expenseReimburseStatusTracker.IsBusinessAreaReq == true)
                            {
                                if (_context.ApprovalRoleMaps.Where(a => a.ApprovalGroupId == empApprGroupId && a.ApprovalLevelId == nextClaimApprovalLevel).FirstOrDefault() != null)
                                {
                                    qApprovalLevelId = _context.ApprovalLevels.Where(x => x.Level == nextClaimApprovalLevel).FirstOrDefault().Id;
                                }
                                else
                                {
                                    qApprovalLevelId = _context.ApprovalLevels.Where(x => x.Level == CurClaimApprovalLevel).FirstOrDefault().Id;
                                    isNextApproverAvailable = false;
                                }
                            }
                            else
                            {
                                if (_context.ApprovalRoleMaps.Where(a => a.ApprovalGroupId == apprGroupId && a.ApprovalLevelId == nextClaimApprovalLevel).FirstOrDefault() != null)
                                {
                                    qApprovalLevelId = _context.ApprovalLevels.Where(x => x.Level == nextClaimApprovalLevel).FirstOrDefault().Id;
                                }
                                else
                                {
                                    qApprovalLevelId = _context.ApprovalLevels.Where(x => x.Level == CurClaimApprovalLevel).FirstOrDefault().Id;
                                    isNextApproverAvailable = false;
                                }
                            }

                            int qApprovalStatusTypeId = isNextApproverAvailable ? (int)EApprovalStatus.Initiating : (int)EApprovalStatus.Pending;

                            //update the next level approver Track request to PENDING (from Initiating) 
                            //if claimitem is not null change the status
                            if (isNextApproverAvailable)
                            {
                                if (expenseReimburseStatusTracker.IsBusinessAreaReq == true)
                                {
                                    claimitem = _context.ExpenseReimburseStatusTrackers.Where(c => c.ExpenseReimburseRequestId == qExpReimRequestId &&
                                    c.ApprovalStatusTypeId == qApprovalStatusTypeId &&
                                     c.BAApprovalGroupId == empApprGroupId &&
                                    c.ApprovalLevelId == qApprovalLevelId).FirstOrDefault();
                                }
                                else
                                {
                                    claimitem = _context.ExpenseReimburseStatusTrackers.Where(c => c.ExpenseReimburseRequestId == qExpReimRequestId &&
                                   c.ApprovalStatusTypeId == qApprovalStatusTypeId &&
                                    c.ApprovalGroupId == empApprGroupId &&
                                   c.ApprovalLevelId == qApprovalLevelId).FirstOrDefault();
                                }

                                if (claimitem != null)
                                {
                                    claimitem.ApprovalStatusTypeId = (int)EApprovalStatus.Pending;
                                }
                                else
                                {
                                    _logger.LogError("DisbursementAndClaims table has no record for  ExpenseReimburseRequestId:" + qExpReimRequestId);
                                }

                            }
                            else
                            {
                                //final approver hence update PettyCashRequest

                                if (expenseReimburseStatusTracker.IsBusinessAreaReq == true)
                                {
                                    claimitem = _context.ExpenseReimburseStatusTrackers.Where(c => c.ExpenseReimburseRequestId == qExpReimRequestId &&
                                   c.ApprovalStatusTypeId == qApprovalStatusTypeId &&
                                    c.BAApprovalGroupId == empApprGroupId &&
                                   c.ApprovalLevelId == qApprovalLevelId).FirstOrDefault();
                                }
                                else
                                {
                                    claimitem = _context.ExpenseReimburseStatusTrackers.Where(c => c.ExpenseReimburseRequestId == qExpReimRequestId &&
                                   c.ApprovalStatusTypeId == qApprovalStatusTypeId &&
                                    c.ApprovalGroupId == empApprGroupId &&
                                   c.ApprovalLevelId == qApprovalLevelId).FirstOrDefault();
                                }
                                //claimitem.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                                claimitem.ApprovedDate = DateTime.Now;
                                _logger.LogInformation("DisbursementAndClaims table updated as approved for ExpenseReimburseRequestId:" + qExpReimRequestId);

                                //final Approver hence updating ExpenseReimburseRequest table
                                var expenseReimburseRequest = _context.ExpenseReimburseRequests.Find(qExpReimRequestId);
                                expenseReimburseRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                                expenseReimburseRequest.ApprovedDate = DateTime.Now;
                                expenseReimburseRequest.Comments = bRejectMessage ? expenseReimburseStatusTrackerDto.Comments : "Approved";
                                _context.Update(expenseReimburseRequest);


                                //DisbursementAndClaimsMaster update the record to Approved (ApprovalStatusId
                                int disbAndClaimItemId = _context.DisbursementsAndClaimsMasters.Where(d => d.ExpenseReimburseReqId == claimitem.ExpenseReimburseRequestId).FirstOrDefault().Id;
                                var disbAndClaimItem = await _context.DisbursementsAndClaimsMasters.FindAsync(disbAndClaimItemId);

                                /// #############################
                                //   Crediting back to the wallet 
                                /// #############################
                                /// 
                                _logger.LogInformation("============== Crediting to Wallet =======================");
                                double expenseReimAmt = expenseReimburseRequest.TotalClaimAmount;
                                double RoleLimitAmt = _context.JobRoles.Find(_context.Employees.Find(expenseReimburseRequest.EmployeeId).RoleId).MaxPettyCashAllowed;
                                EmpCurrentPettyCashBalance empCurrentPettyCashBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == expenseReimburseRequest.EmployeeId).FirstOrDefault();
                                double empCurPettyBal = empCurrentPettyCashBalance.CurBalance;

                                //logic goes here

                                if (expenseReimAmt + empCurPettyBal >= RoleLimitAmt) // claiming amount is greater than replishable amount
                                {
                                    disbAndClaimItem.AmountToWallet = RoleLimitAmt - empCurPettyBal;
                                    disbAndClaimItem.AmountToCredit = expenseReimAmt - (RoleLimitAmt - empCurPettyBal);
                                }
                                else
                                {
                                    //fully credit to Wallet - Zero amount to bank amount
                                    disbAndClaimItem.AmountToWallet = expenseReimAmt;
                                    disbAndClaimItem.AmountToCredit = 0;
                                }


                                disbAndClaimItem.ApprovalStatusId = (int)EApprovalStatus.Approved;
                                _context.Update(disbAndClaimItem);


                                //Final Approveer hence update the EmpCurrentPettyCashBalance table for the employee to reflect the credit
                                //empCurrentPettyCashBalance.CurBalance = empCurPettyBal + disbAndClaimItem.AmountToWallet ?? 0;
                                //empCurrentPettyCashBalance.UpdatedOn = DateTime.Now;
                                //_context.EmpCurrentPettyCashBalances.Update(empCurrentPettyCashBalance);

                                ///
                            }

                            //Save to database
                            if (claimitem != null) { _context.Update(claimitem); };
                            await _context.SaveChangesAsync();


                            var getEmpClaimApproversAllLevels = _context.ApprovalRoleMaps.Include(a => a.ApprovalLevel).Where(a => a.ApprovalGroupId == empApprGroupId).OrderBy(o => o.ApprovalLevel.Level).ToList();


                            foreach (var ApprMap in getEmpClaimApproversAllLevels)
                            {

                                //only next level (level + 1) approver is considered here
                                if (ApprMap.ApprovalLevelId == expenseReimburseStatusTracker.ApprovalLevelId + 1)
                                {
                                    int role_id = ApprMap.RoleId;
                                    var approver = new Employee();
                                    if (expenseReimburseStatusTracker.IsBusinessAreaReq == true)
                                    {
                                        approver = _context.Employees.Where(e => e.BusinessAreaRoleId == role_id && e.BusinessAreaApprovalGroupId == empApprGroupId).FirstOrDefault();
                                    }
                                    else
                                    {
                                        approver = _context.Employees.Where(e => e.RoleId == role_id && e.ApprovalGroupId == empApprGroupId).FirstOrDefault();
                                    }

                                    //##### 4. Send email to the Approver
                                    //####################################
                                    _logger.LogInformation("Sending email to Approver " + approver.GetFullName());

                                    string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "ExpApprNotificationEmail.html" };
                                    string FilePath = Path.Combine(paths);
                                    _logger.LogInformation("Email template path " + FilePath);
                                    StreamReader str = new StreamReader(FilePath);
                                    string MailText = str.ReadToEnd();
                                    str.Close();

                                    var expReimReqt = _context.ExpenseReimburseRequests.Find(expenseReimburseStatusTracker.ExpenseReimburseRequestId);
                                    var approverMailAddress = approver.Email;
                                    string subject = expReimReqt.ExpenseReportTitle + " - #" + expenseReimburseStatusTracker.ExpenseReimburseRequest.Id.ToString();
                                    Employee emp = _context.Employees.Find(expenseReimburseStatusTracker.EmployeeId);


                                    var builder = new MimeKit.BodyBuilder();

                                    MailText = MailText.Replace("{Requester}", emp.GetFullName());
                                    MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                                    MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                                    MailText = MailText.Replace("{RequestedAmount}", expenseReimburseStatusTracker.TotalClaimAmount.ToString());
                                    MailText = MailText.Replace("{RequestNumber}", qExpReimRequestId.ToString());
                                    builder.HtmlBody = MailText;

                                    var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                                    await _emailSender.SendEmailAsync(messagemail);
                                    _logger.LogInformation("Email sent to " + approver.GetFullName());

                                    break;


                                }
                            }
                        }

                        //if nothing else then just update the approval status
                        expenseReimburseStatusTracker.ApprovalStatusTypeId = expenseReimburseStatusTrackerDto.ApprovalStatusTypeId;

                        //If no expenseReimburseStatusTrackers are in pending for the Expense request then update the ExpenseReimburse request table

                        int pendingApprovals = _context.ExpenseReimburseStatusTrackers
                                  .Where(t => t.ExpenseReimburseRequestId == expenseReimburseStatusTrackerDto.ExpenseReimburseRequestId &&
                                  t.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).Count();

                        if (pendingApprovals == 0)
                        {
                            var expReimbReq = _context.ExpenseReimburseRequests.Where(p => p.Id == expenseReimburseStatusTrackerDto.ExpenseReimburseRequestId).FirstOrDefault();
                            expReimbReq.ApprovalStatusTypeId = expenseReimburseStatusTrackerDto.ApprovalStatusTypeId;
                            expReimbReq.ApprovedDate = DateTime.Now;
                            expReimbReq.Comments = bRejectMessage ? expenseReimburseStatusTrackerDto.Comments : "Approved";
                            _context.ExpenseReimburseRequests.Update(expReimbReq);
                            await _context.SaveChangesAsync();
                        }



                        //update the Expense Reimburse request table to reflect the rejection
                        if (bRejectMessage)
                        {
                            var expReimbReq = _context.ExpenseReimburseRequests.Where(p => p.Id == expenseReimburseStatusTrackerDto.ExpenseReimburseRequestId).FirstOrDefault();
                            expReimbReq.ApprovalStatusTypeId = expenseReimburseStatusTrackerDto.ApprovalStatusTypeId;
                            expReimbReq.ApprovedDate = DateTime.Now;
                            expReimbReq.Comments = expenseReimburseStatusTrackerDto.Comments;
                            _context.ExpenseReimburseRequests.Update(expReimbReq);

                            //DisbursementAndClaimsMaster update the record to Rejected (ApprovalStatusId = 5)
                            int disbAndClaimItemId = _context.DisbursementsAndClaimsMasters.Where(d => d.ExpenseReimburseReqId == expReimbReq.Id).FirstOrDefault().Id;
                            var disbAndClaimItem = await _context.DisbursementsAndClaimsMasters.FindAsync(disbAndClaimItemId);

                            disbAndClaimItem.ApprovalStatusId = (int)EApprovalStatus.Rejected;
                            _context.Update(disbAndClaimItem);

                            try
                            {
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "disbAndClaimItem update failed" );
                                throw;
                            }
                           
                           
                        }

                    }


                    //project based Expense Reimburse approval/rejection
                    //only one approver (Project manager)
                    else
                    {

                        _logger.LogInformation("Project based Expense update");
                        //final approver hence update Expense Reimburse request claim
                        claimitem = _context.ExpenseReimburseStatusTrackers.Where(c => c.ExpenseReimburseRequestId == expenseReimburseStatusTracker.ExpenseReimburseRequestId &&
                                    c.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).FirstOrDefault();
                        expenseReimburseStatusTracker.ApprovalStatusTypeId = expenseReimburseStatusTrackerDto.ApprovalStatusTypeId;
                        //DisbursementAndClaimsMaster update the record to Approved (ApprovalStatusId
                        int disbAndClaimItemId = _context.DisbursementsAndClaimsMasters.Where(d => d.ExpenseReimburseReqId == claimitem.ExpenseReimburseRequestId).FirstOrDefault().Id;
                        var disbAndClaimItem = await _context.DisbursementsAndClaimsMasters.FindAsync(disbAndClaimItemId);

                        /// #############################
                        //   Crediting back to the wallet 
                        /// #############################
                        double expenseReimAmt = claimitem.TotalClaimAmount;
                        double RoleLimitAmt = _context.JobRoles.Find(_context.Employees.Find(claimitem.EmployeeId).RoleId).MaxPettyCashAllowed;
                        EmpCurrentPettyCashBalance empCurrentPettyCashBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == claimitem.EmployeeId).FirstOrDefault();
                        double empCurPettyBal = empCurrentPettyCashBalance.CurBalance;

                        //logic goes here

                        if (expenseReimAmt + empCurPettyBal >= RoleLimitAmt) // claiming amount is greater than replishable amount
                        {
                            disbAndClaimItem.AmountToWallet = RoleLimitAmt - empCurPettyBal;
                            disbAndClaimItem.AmountToCredit = expenseReimAmt - (RoleLimitAmt - empCurPettyBal);
                        }
                        else
                        {
                            //fully credit to Wallet - Zero amount to bank amount
                            disbAndClaimItem.AmountToWallet = expenseReimAmt;
                            disbAndClaimItem.AmountToCredit = 0;
                        }

                        _logger.LogInformation("Crediting to the wallet and change the status to approved");
                        
                        disbAndClaimItem.ApprovalStatusId = bRejectMessage ? (int)EApprovalStatus.Rejected : (int)EApprovalStatus.Approved;
                        _context.Update(disbAndClaimItem);

                        _logger.LogInformation("Project based Expense update");

                        ////Final Approveer hence update the EmpCurrentPettyCashBalance table for the employee to reflect the credit
                        //empCurrentPettyCashBalance.CurBalance = empCurPettyBal + disbAndClaimItem.AmountToWallet ?? 0;
                        //_context.EmpCurrentPettyCashBalances.Update(empCurrentPettyCashBalance);

                        /////
                        ///


                        //Update ExpenseReimburseRequests table to update the record to Approved as the final approver has approved it.
                        int expenseReimReqId = _context.ExpenseReimburseRequests.Where(d => d.Id == claimitem.ExpenseReimburseRequestId).FirstOrDefault().Id;
                        var expenseReimReq = await _context.ExpenseReimburseRequests.FindAsync(expenseReimReqId);

                        expenseReimReq.ApprovalStatusTypeId = bRejectMessage ? (int)EApprovalStatus.Rejected : (int)EApprovalStatus.Approved;
                        expenseReimReq.Comments = bRejectMessage ? expenseReimburseStatusTrackerDto.Comments : "Approved";
                        expenseReimReq.ApprovedDate = DateTime.Now;
                        _context.Update(expenseReimReq);

                    }

                    _context.ExpenseReimburseStatusTrackers.Update(expenseReimburseStatusTracker);
                }
                try
                {

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PutExpsensReimburseStatus Tracker record updation failed");
                }
                _logger.LogInformation("PutExpsensReimburseStatus Tracker record updation success");
                await _context.SaveChangesAsync();
                await AtoCashDbContextTransaction.CommitAsync();
            }


            RespStatus respStatus = new();

            if (bRejectMessage)
            {
                respStatus.Status = "Success";
                respStatus.Message = "Expense-Reimburse Request(s) Rejected!";
            }
            else
            {
                respStatus.Status = "Success";
                respStatus.Message = "Expense-Reimburse Request(s) Approved!";
            }

            return Ok(respStatus);

        }

        // POST: api/ExpenseReimburseStatusTrackers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExpenseReimburseStatusTracker>> PostExpenseReimburseStatusTracker(ExpenseReimburseStatusTracker expenseReimburseStatusTracker)
        {
            _context.ExpenseReimburseStatusTrackers.Add(expenseReimburseStatusTracker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpenseReimburseStatusTracker", new { id = expenseReimburseStatusTracker.Id }, expenseReimburseStatusTracker);
        }

        // DELETE: api/ExpenseReimburseStatusTrackers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseReimburseStatusTracker(int id)
        {
            var expenseReimburseStatusTracker = await _context.ExpenseReimburseStatusTrackers.FindAsync(id);


            if (expenseReimburseStatusTracker == null)
            {
                _logger.LogError("Expense Tracker Id is invalid.. cant delete");
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Reimburse Request Id is Invalid!" });
            }


            _context.ExpenseReimburseStatusTrackers.Remove(expenseReimburseStatusTracker);
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Expense Reimburse Request Deleted!" });
        }

        private bool ExpenseReimburseStatusTrackerExists(int id)
        {
            return _context.ExpenseReimburseStatusTrackers.Any(e => e.Id == id);
        }
    }
}
