using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtoCash.Data;
using AtoCash.Models;
using EmailService;
using AtoCash.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.Extensions.Logging;

namespace AtoCash.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]

    public class PettyCashRequestsController : ControllerBase
    {
        private readonly AtoCashDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<PettyCashRequestsController> _logger;

        public PettyCashRequestsController(AtoCashDbContext context, IEmailSender emailSender, ILogger<PettyCashRequestsController> logger)
        {
            this._context = context;
            this._emailSender = emailSender;
            _logger = logger;
        }


        // GET: api/PettyCashRequests
        [HttpGet]
        [ActionName("GetPettyCashRequests")]
        public async Task<ActionResult<IEnumerable<PettyCashRequestDTO>>> GetPettyCashRequests()
        {
            List<PettyCashRequestDTO> ListPettyCashRequestDTO = new();

            //var claimApprovalStatusTracker = await _context.ClaimApprovalStatusTrackers.FindAsync(1);

            var pettyCashRequests = await _context.PettyCashRequests.ToListAsync();
            if (pettyCashRequests == null)
            {
                _logger.LogError("GetPettyCashRequests is null");
            }

            foreach (PettyCashRequest pettyCashRequest in pettyCashRequests)
            {
                PettyCashRequestDTO pettyCashRequestDTO = new();

                pettyCashRequestDTO.Id = pettyCashRequest.Id;
                pettyCashRequestDTO.EmployeeName = _context.Employees.Find(pettyCashRequest.EmployeeId).GetFullName();
                pettyCashRequestDTO.CurrencyTypeId = pettyCashRequest.CurrencyTypeId;
                pettyCashRequestDTO.CurrencyType = pettyCashRequest.CurrencyType != null ? _context.CurrencyTypes.Find(pettyCashRequest.CurrencyType).CurrencyName : null;
                pettyCashRequestDTO.PettyClaimAmount = pettyCashRequest.PettyClaimAmount;
                pettyCashRequestDTO.PettyClaimRequestDesc = pettyCashRequest.PettyClaimRequestDesc;
                pettyCashRequestDTO.CashReqDate = pettyCashRequest.CashReqDate;
                pettyCashRequestDTO.Department = pettyCashRequest.DepartmentId != null ? _context.Departments.Find(pettyCashRequest.DepartmentId).DeptCode : null;
                pettyCashRequestDTO.ProjectId = pettyCashRequest.ProjectId;
                pettyCashRequestDTO.Project = pettyCashRequest.ProjectId != null ? _context.Projects.Find(pettyCashRequest.ProjectId).ProjectName : null;
                pettyCashRequestDTO.SubProjectId = pettyCashRequest.SubProjectId;
                pettyCashRequestDTO.SubProject = pettyCashRequest.SubProjectId != null ? _context.SubProjects.Find(pettyCashRequest.SubProjectId).SubProjectName : null;
                pettyCashRequestDTO.WorkTaskId = pettyCashRequest.WorkTaskId;
                pettyCashRequestDTO.WorkTask = pettyCashRequest.WorkTaskId != null ? _context.WorkTasks.Find(pettyCashRequest.WorkTaskId).TaskName : null;
                pettyCashRequestDTO.ApprovalStatusType = pettyCashRequest.ApprovalStatusTypeId != 0 ? _context.ApprovalStatusTypes.Find(pettyCashRequest.ApprovalStatusTypeId).Status : null;
                pettyCashRequestDTO.ApprovalStatusTypeId = pettyCashRequest.ApprovalStatusTypeId;
                pettyCashRequestDTO.ApprovedDate = pettyCashRequest.ApprovedDate;
                ListPettyCashRequestDTO.Add(pettyCashRequestDTO);
            }

            return ListPettyCashRequestDTO.OrderByDescending(o => o.CashReqDate).ToList();
        }



        // GET: api/PettyCashRequests/5
        [HttpGet("{id}")]
        [ActionName("GetPettyCashRequest")]
        public async Task<ActionResult<PettyCashRequestDTO>> GetPettyCashRequest(int id)
        {


            var pettyCashRequest = await _context.PettyCashRequests.FindAsync(id);

            var disbAndClaim = _context.DisbursementsAndClaimsMasters.Where(d => d.PettyCashRequestId == id).FirstOrDefault();

            if (pettyCashRequest == null)
            {
                _logger.LogError("GetPettyCashRequests: disbAndClaim is null for id:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "GetPettyCashRequest Id is Invalid!" });
            }
            PettyCashRequestDTO pettyCashRequestDTO = new();

            pettyCashRequestDTO.Id = pettyCashRequest.Id;
            pettyCashRequestDTO.EmployeeName = _context.Employees.Find(pettyCashRequest.EmployeeId).GetFullName();
            pettyCashRequestDTO.CurrencyTypeId = pettyCashRequest.CurrencyTypeId;
            pettyCashRequestDTO.CurrencyType = pettyCashRequest.CurrencyType != null ? _context.CurrencyTypes.Find(pettyCashRequest.CurrencyType).CurrencyName : null;
            pettyCashRequestDTO.PettyClaimAmount = pettyCashRequest.PettyClaimAmount;
            pettyCashRequestDTO.PettyClaimRequestDesc = pettyCashRequest.PettyClaimRequestDesc;
            pettyCashRequestDTO.CashReqDate = pettyCashRequest.CashReqDate;
            pettyCashRequestDTO.Department = pettyCashRequest.DepartmentId != null ? _context.Departments.Find(pettyCashRequest.DepartmentId).DeptCode : null;
            pettyCashRequestDTO.ProjectId = pettyCashRequest.ProjectId;
            pettyCashRequestDTO.Project = pettyCashRequest.ProjectId != null ? _context.Projects.Find(pettyCashRequest.ProjectId).ProjectName : null;
            pettyCashRequestDTO.SubProjectId = pettyCashRequest.SubProjectId;
            pettyCashRequestDTO.SubProject = pettyCashRequest.SubProjectId != null ? _context.SubProjects.Find(pettyCashRequest.SubProjectId).SubProjectName : null;
            pettyCashRequestDTO.WorkTaskId = pettyCashRequest.WorkTaskId;
            pettyCashRequestDTO.WorkTask = pettyCashRequest.WorkTaskId != null ? _context.WorkTasks.Find(pettyCashRequest.WorkTaskId).TaskName : null;
            pettyCashRequestDTO.ApprovalStatusTypeId = pettyCashRequest.ApprovalStatusTypeId;
            pettyCashRequestDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(pettyCashRequest.ApprovalStatusTypeId).Status;
            pettyCashRequestDTO.ApprovedDate = pettyCashRequest.ApprovedDate;
            pettyCashRequestDTO.CreditToBank = pettyCashRequest.ApprovalStatusTypeId == (int)EApprovalStatus.Approved ? disbAndClaim.AmountToCredit : 0;
            pettyCashRequestDTO.IsSettled = (disbAndClaim.IsSettledAmountCredited ?? false);

            pettyCashRequestDTO.Comments = pettyCashRequest.Comments;

            return pettyCashRequestDTO;
        }





        [HttpGet("{id}")]
        [ActionName("GetPettyCashRequestRaisedForEmployee")]
        public async Task<ActionResult<IEnumerable<PettyCashRequestDTO>>> GetPettyCashRequestRaisedForEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            int roleId = employee.RoleId;
            int BARoleId = employee.BusinessAreaRoleId ?? 0;

            if (employee == null)
            {
                _logger.LogError("GetPettyCashRequestRaisedForEmployee: Employee is null for id:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee Id is Invalid!" });
            }

            //get the employee's approval level for comparison with approver level  to decide "ShowEditDelete" bool
            int reqEmpApprLevelId = 0;
            try
            {
                reqEmpApprLevelId = _context.ApprovalRoleMaps.Where(a => a.RoleId == roleId || a.RoleId == BARoleId).FirstOrDefault().ApprovalLevelId;

            }
            catch (Exception ex)
            {
                _logger.LogError("Employee reqEmpApprLevelId is null for Employee id: " + id);
                return Ok(new RespStatus { Status = "Failure", Message = "Employee Approval Level not defined!" });
            }

            int reqEmpApprLevel = _context.ApprovalLevels.Find(reqEmpApprLevelId).Level;

            var pettyCashRequests = await _context.PettyCashRequests.Where(p => p.EmployeeId == id).ToListAsync();

            if (pettyCashRequests == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cash Advance Id is Invalid!" });
            }

            List<PettyCashRequestDTO> PettyCashRequestDTOs = new();

            foreach (var pettyCashRequest in pettyCashRequests)
            {
                PettyCashRequestDTO pettyCashRequestDTO = new();

                pettyCashRequestDTO.Id = pettyCashRequest.Id;
                pettyCashRequestDTO.EmployeeId = pettyCashRequest.EmployeeId;
                pettyCashRequestDTO.EmployeeName = _context.Employees.Find(pettyCashRequest.EmployeeId).GetFullName();
                pettyCashRequestDTO.CurrencyTypeId = pettyCashRequest.CurrencyTypeId;
                pettyCashRequestDTO.CurrencyType = pettyCashRequest.CurrencyType != null ? _context.CurrencyTypes.Find(pettyCashRequest.CurrencyType).CurrencyName : null;
                pettyCashRequestDTO.PettyClaimAmount = pettyCashRequest.PettyClaimAmount;
                pettyCashRequestDTO.PettyClaimRequestDesc = pettyCashRequest.PettyClaimRequestDesc;
                pettyCashRequestDTO.CashReqDate = pettyCashRequest.CashReqDate;
                pettyCashRequestDTO.DepartmentId = pettyCashRequest.DepartmentId;
                pettyCashRequestDTO.Department = pettyCashRequest.DepartmentId != null ? _context.Departments.Find(pettyCashRequest.DepartmentId).DeptCode + ":" + _context.Departments.Find(pettyCashRequest.DepartmentId).DeptName : null;
                pettyCashRequestDTO.ProjectId = pettyCashRequest.ProjectId;
                pettyCashRequestDTO.Project = pettyCashRequest.ProjectId != null ? _context.Projects.Find(pettyCashRequest.ProjectId).ProjectName : null;
                pettyCashRequestDTO.SubProjectId = pettyCashRequest.SubProjectId;
                pettyCashRequestDTO.SubProject = pettyCashRequest.SubProjectId != null ? _context.SubProjects.Find(pettyCashRequest.SubProjectId).SubProjectName : null;
                pettyCashRequestDTO.WorkTaskId = pettyCashRequest.WorkTaskId;
                pettyCashRequestDTO.WorkTask = pettyCashRequest.WorkTaskId != null ? _context.WorkTasks.Find(pettyCashRequest.WorkTaskId).TaskName : null;
                pettyCashRequestDTO.ApprovalStatusTypeId = pettyCashRequest.ApprovalStatusTypeId;
                pettyCashRequestDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(pettyCashRequest.ApprovalStatusTypeId).Status;
                pettyCashRequestDTO.ApprovedDate = pettyCashRequest.ApprovedDate;

                // set the bookean flat to TRUE if No approver has yet approved the Request else FALSE
                bool ifAnyOfStatusRecordsApproved = _context.ClaimApprovalStatusTrackers.Where(t =>
                                                           (t.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected ||
                                                          t.ApprovalStatusTypeId == (int)EApprovalStatus.Approved) &&
                                                          t.PettyCashRequestId == pettyCashRequest.Id).Any();

                if (ifAnyOfStatusRecordsApproved)
                {
                    pettyCashRequestDTO.ShowEditDelete = false;
                }
                else
                {
                    pettyCashRequestDTO.ShowEditDelete = true;
                }


                ///
                PettyCashRequestDTOs.Add(pettyCashRequestDTO);
            }


            return Ok(PettyCashRequestDTOs.OrderByDescending(o => o.CashReqDate).ToList());
        }



        [HttpGet("{id}")]
        [ActionName("CountAllPettyCashRequestRaisedByEmployee")]
        public async Task<ActionResult> CountAllPettyCashRequestRaisedByEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return Ok(0);
            }

            var pettyCashRequests = await _context.PettyCashRequests.Where(p => p.EmployeeId == id).ToListAsync();

            if (pettyCashRequests == null)
            {
                return Ok(0);
            }

            int TotalCount = _context.PettyCashRequests.Where(c => c.EmployeeId == id).Count();
            int PendingCount = _context.PettyCashRequests.Where(c => c.EmployeeId == id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).Count();
            int RejectedCount = _context.PettyCashRequests.Where(c => c.EmployeeId == id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected).Count();
            int ApprovedCount = _context.PettyCashRequests.Where(c => c.EmployeeId == id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).Count();

            return Ok(new { TotalCount, PendingCount, RejectedCount, ApprovedCount });
        }




        [HttpGet]
        [ActionName("GetPettyCashReqInPendingForAll")]
        public async Task<ActionResult<int>> GetPettyCashReqInPendingForAll()
        {
            //debug
            var pettyCashRequests = await _context.PettyCashRequests.Include("ClaimApprovalStatusTrackers").ToListAsync();


            //var pettyCashRequests = await _context.ClaimApprovalStatusTrackers.Where(c => c.ApprovalStatusTypeId == ApprovalStatus.Pending).select( );

            if (pettyCashRequests == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "pettyCashRequests is Empty!" });
            }

            return Ok(pettyCashRequests.Count);
        }



        // PUT: api/PettyCashRequests/5
        [HttpPut("{id}")]
        [ActionName("PutPettyCashRequest")]
        public async Task<IActionResult> PutPettyCashRequest(int id, PettyCashRequestDTO pettyCashRequestDto)
        {
            if (id != pettyCashRequestDto.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }

            var pettyCashRequest = await _context.PettyCashRequests.FindAsync(id);
            pettyCashRequestDto.EmployeeId = pettyCashRequest.EmployeeId;

            Double empCurAvailBal = GetEmpCurrentAvailablePettyCashBalance(pettyCashRequestDto);

            if (!(pettyCashRequestDto.PettyClaimAmount <= empCurAvailBal && pettyCashRequestDto.PettyClaimAmount > 0))
            {
                return Conflict(new RespStatus() { Status = "Failure", Message = "Invalid Cash Request Amount Or Limit Exceeded" });
            }




            int ApprovedCount = _context.ExpenseReimburseStatusTrackers.Where(e => e.ExpenseReimburseRequestId == pettyCashRequest.Id && e.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).Count();
            if (ApprovedCount != 0)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "PettyCash Requests cant be Edited after Approval!" });
            }


            //if Pettycash request is modified then trigger changes to other tables
            if (pettyCashRequest.PettyClaimAmount != pettyCashRequestDto.PettyClaimAmount)
            {

                //update the EmpPettyCashBalance to credit back the deducted amount
                EmpCurrentPettyCashBalance empPettyCashBal = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == pettyCashRequest.EmployeeId).FirstOrDefault();
                double oldBal = empPettyCashBal.CurBalance;
                double prevAmt = pettyCashRequest.PettyClaimAmount;
                double NewAmt = pettyCashRequestDto.PettyClaimAmount;

                pettyCashRequest.PettyClaimAmount = pettyCashRequestDto.PettyClaimAmount;
                pettyCashRequest.PettyClaimRequestDesc = pettyCashRequestDto.PettyClaimRequestDesc;


                //check employee allowed limit to Cash Advance, if limit exceeded return with an conflict message.
                double maxAllowed = _context.JobRoles.Find(_context.Employees.Find(pettyCashRequest.EmployeeId).RoleId).MaxPettyCashAllowed;
                if (maxAllowed >= oldBal + prevAmt - NewAmt && oldBal + prevAmt - NewAmt > 0)
                {
                    empPettyCashBal.CurBalance = oldBal + prevAmt - NewAmt;
                    empPettyCashBal.UpdatedOn = DateTime.Now;
                    _context.EmpCurrentPettyCashBalances.Update(empPettyCashBal);
                }
                else
                {
                    return Conflict(new RespStatus() { Status = "Failure", Message = "Invalid Cash Request Amount Or Limit Exceeded" });
                }



            }
            ////
            ///

            pettyCashRequest.PettyClaimAmount = pettyCashRequestDto.PettyClaimAmount;
            pettyCashRequest.PettyClaimRequestDesc = pettyCashRequestDto.PettyClaimRequestDesc;
            pettyCashRequest.CashReqDate = DateTime.Now;

            _context.PettyCashRequests.Update(pettyCashRequest);




            //Step -2 change the claim approval status tracker records
            var claims = await _context.ClaimApprovalStatusTrackers.Where(c => c.PettyCashRequestId == pettyCashRequestDto.Id).ToListAsync();
            bool IsFirstEmail = true;
            int? newDeptId = pettyCashRequest.DepartmentId;
            int? newProjId = pettyCashRequestDto.ProjectId;
            int? newSubProjId = pettyCashRequestDto.SubProjectId;
            int? newWorkTaskId = pettyCashRequestDto.WorkTaskId;


            foreach (ClaimApprovalStatusTracker claim in claims)
            {
                claim.DepartmentId = newDeptId;
                claim.ProjectId = newProjId;
                claim.SubProjectId = newSubProjId;
                claim.WorkTaskId = newWorkTaskId;
                claim.ReqDate = pettyCashRequest.CashReqDate;
                claim.FinalApprovedDate = null;
                //claim.ApprovalStatusTypeId = claim.ApprovalLevelId == 1 ? (int)EApprovalStatus.Pending : (int)EApprovalStatus.Initiating;
                claim.Comments = "Modified Request";

                _context.ClaimApprovalStatusTrackers.Update(claim);

                if (IsFirstEmail)
                {
                    var approver = _context.Employees.Where(e => e.RoleId == claim.RoleId && e.ApprovalGroupId == claim.ApprovalGroupId).FirstOrDefault();
                    var approverMailAddress = approver.Email;
                    string subject = "(Modified) Pettycash Request Approval " + pettyCashRequestDto.Id.ToString();
                    Employee emp = _context.Employees.Find(pettyCashRequestDto.EmployeeId);
                    var pettycashreq = _context.PettyCashRequests.Find(pettyCashRequestDto.Id);

                    _logger.LogInformation(approver.GetFullName() + "Email Start");

                    string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "PettyCashApprNotificationEmail.html" };
                    string FilePath = Path.Combine(paths);
                    _logger.LogInformation("Email template path " + FilePath);
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();




                    var builder = new MimeKit.BodyBuilder();

                    MailText = MailText.Replace("{Requester}", emp.GetFullName());
                    MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                    MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                    MailText = MailText.Replace("{RequestedAmount}", pettycashreq.PettyClaimAmount.ToString());
                    MailText = MailText.Replace("{RequestNumber}", pettycashreq.Id.ToString());
                    builder.HtmlBody = MailText;

                    var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                    await _emailSender.SendEmailAsync(messagemail);
                    _logger.LogInformation(approver.GetFullName() + "Email Sent");

                    IsFirstEmail = false;
                }
            }
            //_context.Entry(pettyCashRequest).State = EntityState.Modified;

            //Step-3 change the Disbursements and Claims Master record

            var disburseMasterRecord = _context.DisbursementsAndClaimsMasters.Where(d => d.PettyCashRequestId == pettyCashRequestDto.Id).FirstOrDefault();

            disburseMasterRecord.DepartmentId = newDeptId;
            disburseMasterRecord.ProjectId = newProjId;
            disburseMasterRecord.SubProjectId = newSubProjId;
            disburseMasterRecord.WorkTaskId = newWorkTaskId;
            disburseMasterRecord.RecordDate = DateTime.Now;
            disburseMasterRecord.ClaimAmount = pettyCashRequestDto.PettyClaimAmount;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DisbursementsAndClaimsMasters update failed ");
            }
            _logger.LogInformation("DisbursementsAndClaimsMaster table update complete");
            _logger.LogInformation("Petty Cash Request updated successfully");



            return Ok(new RespStatus { Status = "Success", Message = "Request Updated!" });
        }

        // POST: api/PettyCashRequests
        [HttpPost]
        [ActionName("PostPettyCashRequest")]
        public async Task<ActionResult<PettyCashRequest>> PostPettyCashRequest(PettyCashRequestDTO pettyCashRequestDto)
        {
            int SuccessResult;

            if (pettyCashRequestDto == null)
            {
                _logger.LogError("PostPettyCashRequestDto - null request data");
                return Conflict(new RespStatus { Status = "Failure", Message = "PettyCash Request invalid!" });
            }

            /*!!=========================================
               Check Eligibility for Cash Disbursement
             .==========================================*/

            Double empCurAvailBal = GetEmpCurrentAvailablePettyCashBalance(pettyCashRequestDto);
            Double empCurMaxLimit = GetEmpCurrentMaxCashBorrowLimit(pettyCashRequestDto);
            //Check any pending pettycash requests for employee, if then total them all to find the amount eligible

            double pendingPettCashRequestAmounts = _context.PettyCashRequests.Where(p => p.EmployeeId == pettyCashRequestDto.EmployeeId && p.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).Select(s => s.PettyClaimAmount).Sum();

            if (pendingPettCashRequestAmounts + pettyCashRequestDto.PettyClaimAmount <= empCurAvailBal
                || pettyCashRequestDto.PettyClaimAmount <= empCurAvailBal
                && pettyCashRequestDto.PettyClaimAmount > 0
                || pendingPettCashRequestAmounts + pettyCashRequestDto.PettyClaimAmount <= empCurMaxLimit)
            {
                SuccessResult = await Task.Run(() => ProcessPettyCashRequestClaim(pettyCashRequestDto, empCurAvailBal));
            }
            else
            {
                return Conflict(new RespStatus() { Status = "Failure", Message = "Invalid Cash Request Amount Or Limit Exceeded" });
            }

            if (SuccessResult == 0)
            {
                _logger.LogInformation("Petty Cash Request - Process completed");

                return Created("PostPettyCashRequest", new RespStatus() { Status = "Success", Message = "Cash Advance Request Created" });
            }
            else
            {
                _logger.LogError("Petty Cash Request creation failed -Check approval Role Map assignment!");

                return BadRequest(new RespStatus { Status = "Failure", Message = "Petty Cash Request - Approval Role Map Undefined!" });
            }

        }

        // DELETE: api/PettyCashRequests/5
        [HttpDelete("{id}")]
        [ActionName("DeletePettyCashRequest")]

        public async Task<IActionResult> DeletePettyCashRequest(int id)
        {
            var pettyCashRequest = await _context.PettyCashRequests.FindAsync(id);
            if (pettyCashRequest == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cash Advance Request Id Invalid!" });
            }

            var ClmApprvStatusTrackers = _context.ClaimApprovalStatusTrackers.Where(c => c.PettyCashRequestId == pettyCashRequest.Id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Approved);

            int ApprovedCount = ClmApprvStatusTrackers.Count();

            if (ApprovedCount > 0)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Cash Advance Request cant be Deleted after Approval!" });
            }


            //update the EmpPettyCashBalance to credit back the deducted amount
            EmpCurrentPettyCashBalance empPettyCashBal = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == pettyCashRequest.EmployeeId).FirstOrDefault();
            empPettyCashBal.CurBalance += pettyCashRequest.PettyClaimAmount;
            empPettyCashBal.UpdatedOn = DateTime.Now;
            _context.EmpCurrentPettyCashBalances.Update(empPettyCashBal);

            _context.PettyCashRequests.Remove(pettyCashRequest);

            var ClaimApprStatusTrackers = _context.ClaimApprovalStatusTrackers.Where(c => c.PettyCashRequestId == pettyCashRequest.Id).ToList();

            foreach (var claim in ClaimApprStatusTrackers)
            {
                _context.ClaimApprovalStatusTrackers.Remove(claim);
            }

            var disburseAndClaims = _context.DisbursementsAndClaimsMasters.Where(d => d.PettyCashRequestId == pettyCashRequest.Id).ToList();
            foreach (var disburse in disburseAndClaims)
            {
                _context.DisbursementsAndClaimsMasters.Remove(disburse);
            }
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Cash Advance Request Deleted!" });
        }


        private Double GetEmpCurrentAvailablePettyCashBalance(PettyCashRequestDTO pettyCashRequest)
        {

            var empCurPettyBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == pettyCashRequest.EmployeeId).FirstOrDefault();

            if (empCurPettyBalance != null)
            {
                return empCurPettyBalance.CurBalance;
            }

            AddEmpCurrentPettyCashBalanceForEmployee(pettyCashRequest.EmployeeId);

            return 0;
        }

        private Double GetEmpCurrentMaxCashBorrowLimit(PettyCashRequestDTO pettyCashRequest)
        {
            Employee emp = _context.Employees.Find(pettyCashRequest.EmployeeId);

            var empMaxLimit = _context.JobRoles.Where(e => e.Id == emp.RoleId).FirstOrDefault().MaxPettyCashAllowed;

            if (empMaxLimit != 0)
            {
                return empMaxLimit;
            }

            return 0;
        }

        //NO HTTPACTION HERE. Void method just to add data to database table
        private async Task<int> ProcessPettyCashRequestClaim(PettyCashRequestDTO pettyCashRequestDto, Double empCurAvailBal)
        {

            int SuccessResult;

            if (pettyCashRequestDto.ProjectId != null)
            {
                //Goes to Option 1 (Project)
                SuccessResult = await Task.Run(() => ProjectCashRequest(pettyCashRequestDto, empCurAvailBal));
            }
            else if (pettyCashRequestDto.BusinessAreaId != null)
            {
                //Goes to Option 1 (Business Area)
                SuccessResult = await Task.Run(() => BusinessAreaCashRequest(pettyCashRequestDto, empCurAvailBal));
            }
            else
            {
                //Goes to Option 2 (Department)
                SuccessResult = await Task.Run(() => DepartmentCashRequest(pettyCashRequestDto, empCurAvailBal));
            }

           
            return SuccessResult;
        }


        /// <summary>
        /// This is the option 1 : : PROJECT BASED CASH ADVANCE REQUEST
        /// </summary>
        /// <param name="pettyCashRequestDto"></param>
        /// <param name="empCurAvailBal"></param>
        private async Task<int> ProjectCashRequest(PettyCashRequestDTO pettyCashRequestDto, Double empCurAvailBal)
        {

            //### 1. If Employee Eligible for Cash Claim enter a record and reduce the available amount for next claim
            #region

            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                int costCenterId = _context.Projects.Find(pettyCashRequestDto.ProjectId).CostCenterId;

                int projManagerid = _context.Projects.Find(pettyCashRequestDto.ProjectId).ProjectManagerId;

                var approver = _context.Employees.Find(projManagerid);


                if (approver != null)
                {
                    _logger.LogInformation("Project Manager defined, no issues");
                }
                else
                {
                    _logger.LogError("Project Manager is not Assigned");
                    return 1;
                }
                ////
                int empid = pettyCashRequestDto.EmployeeId;
                Double empReqAmount = pettyCashRequestDto.PettyClaimAmount;
                //int empApprGroupId = _context.Employees.Find(empid).ApprovalGroupId;
                double maxCashAllowedForRole = (_context.JobRoles.Find(_context.Employees.Find(pettyCashRequestDto.EmployeeId).RoleId).MaxPettyCashAllowed);

                if (pettyCashRequestDto.PettyClaimAmount > maxCashAllowedForRole)
                {
                    return 1;
                }

                var curPettyCashBal = _context.EmpCurrentPettyCashBalances.Where(x => x.EmployeeId == empid).FirstOrDefault();
                curPettyCashBal.Id = curPettyCashBal.Id;
                curPettyCashBal.CurBalance = empCurAvailBal - empReqAmount <= maxCashAllowedForRole ? empCurAvailBal - empReqAmount : maxCashAllowedForRole;
                curPettyCashBal.EmployeeId = empid;
                curPettyCashBal.UpdatedOn = DateTime.Now;
                _context.Update(curPettyCashBal);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Project: PettyCashRequests");
                }

                #endregion

                //##### 2. Adding entry to PettyCashRequest table for record
                #region
                var pcrq = new PettyCashRequest()
                {
                    EmployeeId = empid,
                    PettyClaimAmount = empReqAmount,
                    CashReqDate = DateTime.Now,
                    DepartmentId = null,
                    ProjectId = pettyCashRequestDto.ProjectId,
                    SubProjectId = pettyCashRequestDto.SubProjectId,
                    WorkTaskId = pettyCashRequestDto.WorkTaskId,
                    PettyClaimRequestDesc = pettyCashRequestDto.PettyClaimRequestDesc,
                    CurrencyTypeId = pettyCashRequestDto.CurrencyTypeId,
                    ApprovalStatusTypeId = (int)EApprovalStatus.Pending,
                    Comments = "Cash Advance Request in Process!"

                };
                _context.PettyCashRequests.Add(pcrq);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Project: PettyCashRequests");
                }

                pettyCashRequestDto.Id = pcrq.Id;
                #endregion

                //##### 3. Add an entry to ClaimApproval Status tracker
                //get costcenterID based on project
                #region

                ///////////////////////////// Check if self Approved Request /////////////////////////////
                int maxApprLevel = _context.ApprovalRoleMaps.Max(a => a.ApprovalLevelId);
                int empApprLevel = _context.ApprovalRoleMaps.Where(a => a.RoleId == _context.Employees.Find(empid).RoleId).FirstOrDefault().Id;
                bool isSelfApprovedRequest = false;
                //if highest approver is requesting Petty cash request himself
                if (maxApprLevel == empApprLevel || projManagerid == empid)
                {
                    isSelfApprovedRequest = true;
                }
                //////////////////////////////////////////////////////////////////////////////////////////
                if (isSelfApprovedRequest)
                {
                    ClaimApprovalStatusTracker claimAppStatusTrack = new()
                    {
                        EmployeeId = pettyCashRequestDto.EmployeeId,
                        PettyCashRequestId = pettyCashRequestDto.Id,
                        DepartmentId = null,
                        ProjManagerId = projManagerid,
                        ProjectId = pettyCashRequestDto.ProjectId,
                        SubProjectId = pettyCashRequestDto.SubProjectId,
                        WorkTaskId = pettyCashRequestDto.WorkTaskId,
                        RoleId = approver.RoleId,
                        // get the next ProjectManager approval.
                        ApprovalGroupId = _context.Employees.Find(pettyCashRequestDto.EmployeeId).ApprovalGroupId,
                        ApprovalLevelId = 2, //empApprLevel or 2 default approval level is 2 for Project based request
                        ReqDate = DateTime.Now,
                        FinalApprovedDate = DateTime.Now,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Approved, //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                        Comments = "Self Approved Request!"
                    };


                    _context.ClaimApprovalStatusTrackers.Add(claimAppStatusTrack);
                    pcrq.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                    pcrq.Comments = "Approved";
                    _context.PettyCashRequests.Update(pcrq);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ClaimApprovalStatusTracker claimAppStatusTrack = new()
                    {
                        EmployeeId = pettyCashRequestDto.EmployeeId,
                        PettyCashRequestId = pettyCashRequestDto.Id,
                        DepartmentId = null,
                        ProjManagerId = projManagerid,
                        ProjectId = pettyCashRequestDto.ProjectId,
                        SubProjectId = pettyCashRequestDto.SubProjectId,
                        WorkTaskId = pettyCashRequestDto.WorkTaskId,
                        RoleId = approver.RoleId,
                        // get the next ProjectManager approval.
                        ApprovalGroupId = _context.Employees.Find(pettyCashRequestDto.EmployeeId).ApprovalGroupId,
                        ApprovalLevelId = 2, // default approval level is 2 for Project based request
                        ReqDate = DateTime.Now,
                        FinalApprovedDate = null,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Pending, //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                        Comments = "Awaiting Approver Action"
                    };


                    _context.ClaimApprovalStatusTrackers.Add(claimAppStatusTrack);
                    await _context.SaveChangesAsync();
                    #endregion


                    //##### 4. Send email to the user
                    //####################################
                    #region
                    _logger.LogInformation("Project: PettyCash Email Start");

                    string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "PettyCashApprNotificationEmail.html" };
                    string FilePath = Path.Combine(paths);
                    _logger.LogInformation("Email template path " + FilePath);
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();

                    var approverMailAddress = approver.Email;
                    string subject = "Pettycash Request Approval " + pettyCashRequestDto.Id.ToString();
                    Employee emp = _context.Employees.Find(pettyCashRequestDto.EmployeeId);
                    var pettycashreq = _context.PettyCashRequests.Find(pettyCashRequestDto.Id);

                    var builder = new MimeKit.BodyBuilder();

                    MailText = MailText.Replace("{Requester}", emp.GetFullName());
                    MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                    MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                    MailText = MailText.Replace("{RequestedAmount}", pettycashreq.PettyClaimAmount.ToString());
                    MailText = MailText.Replace("{RequestNumber}", pettycashreq.Id.ToString());
                    builder.HtmlBody = MailText;

                    var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                    await _emailSender.SendEmailAsync(messagemail);
                    _logger.LogInformation("Project: PettyCash Email Sent");
                    #endregion
                }



                //##### 5. Adding a entry in DisbursementsAndClaimsMaster table for records
                #region
                _logger.LogInformation("Project: Disbursement table insert start");
                DisbursementsAndClaimsMaster disbursementsAndClaimsMaster = new();

                disbursementsAndClaimsMaster.EmployeeId = pettyCashRequestDto.EmployeeId;
                disbursementsAndClaimsMaster.PettyCashRequestId = pettyCashRequestDto.Id;
                disbursementsAndClaimsMaster.ExpenseReimburseReqId = null;
                disbursementsAndClaimsMaster.RequestTypeId = (int)ERequestType.CashAdvance;
                disbursementsAndClaimsMaster.ProjectId = pettyCashRequestDto.ProjectId;
                disbursementsAndClaimsMaster.SubProjectId = pettyCashRequestDto.SubProjectId;
                disbursementsAndClaimsMaster.WorkTaskId = pettyCashRequestDto.WorkTaskId;
                disbursementsAndClaimsMaster.RecordDate = DateTime.Now;
                disbursementsAndClaimsMaster.CurrencyTypeId = pettyCashRequestDto.CurrencyTypeId;
                disbursementsAndClaimsMaster.ClaimAmount = pettyCashRequestDto.PettyClaimAmount;
                disbursementsAndClaimsMaster.AmountToWallet = 0;
                disbursementsAndClaimsMaster.AmountToCredit = isSelfApprovedRequest ? pettyCashRequestDto.PettyClaimAmount : 0;
                disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                disbursementsAndClaimsMaster.CostCenterId = _context.Projects.Find(pettyCashRequestDto.ProjectId).CostCenterId;
                disbursementsAndClaimsMaster.ApprovalStatusId = isSelfApprovedRequest ? (int)EApprovalStatus.Approved : (int)EApprovalStatus.Pending; //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected

                _context.DisbursementsAndClaimsMasters.Add(disbursementsAndClaimsMaster);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string error = ex.Message;
                }
                #endregion
                _logger.LogInformation("Project: Disbursement table insert Completed");
                await AtoCashDbContextTransaction.CommitAsync();
            }
            return 1;
        }

        /// <summary>
        /// This is option 2 : DEPARTMENT BASED CASH ADVANCE REQUEST
        /// </summary>
        /// <param name="pettyCashRequestDto"></param>
        /// <param name="empCurAvailBal"></param>
        private async Task<int> DepartmentCashRequest(PettyCashRequestDTO pettyCashRequestDto, Double empCurAvailBal)
        {
            //### 1. If Employee Eligible for Cash Claim enter a record and reduce the available amount for next claim
            #region
            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                int reqEmpid = pettyCashRequestDto.EmployeeId;
                Employee reqEmp = _context.Employees.Find(reqEmpid);
                int reqApprGroupId = reqEmp.ApprovalGroupId;
                int reqRoleId = reqEmp.RoleId;

                //if Approval Role Map list is null

                var approRoleMap = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId).FirstOrDefault();

                if (approRoleMap == null)
                {
                    _logger.LogError("Approver Role Map Not defined, approval group id " + reqApprGroupId);
                    return 1;
                }
                else
                {
                    var approRoleMaps = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId).ToList();

                    foreach (ApprovalRoleMap ApprMap in approRoleMaps)
                    {
                        int role_id = ApprMap.RoleId;
                        var approver = _context.Employees.Where(e => e.RoleId == role_id && e.ApprovalGroupId == reqApprGroupId).FirstOrDefault();
                        if (approver == null)
                        {
                            _logger.LogError("Approver employee not mapped for RoleMap RoleId:" + role_id + "ApprovalGroupId:" + reqApprGroupId);
                            return 1;
                        }

                    }
                }
                int maxApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId).ToList().Select(x => x.ApprovalLevel).Max(a => a.Level);
                int reqApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId && a.RoleId == reqRoleId).Select(x => x.ApprovalLevel).FirstOrDefault().Level;

                //var apprRolMap = _context.ApprovalRoleMaps.Where(a => a.ApprovalGroupId == reqApprGroupId && a.RoleId == reqRoleId).FirstOrDefault();
                //var apprLevels = apprRolMap.ApprovalLevel;
                //int reqApprLevel = apprLevels.Level;
                //.Select(x => x.ApprovalLevel).FirstOrDefault().Level
                bool isSelfApprovedRequest = false;

                Double empReqAmount = pettyCashRequestDto.PettyClaimAmount;

                _logger.LogInformation("Department: pettyCashRequestDto balance check Start");


                var curPettyCashBal = _context.EmpCurrentPettyCashBalances.Where(x => x.EmployeeId == reqEmpid).FirstOrDefault();
                if (_context.JobRoles.Find(_context.Employees.Find(pettyCashRequestDto.EmployeeId).RoleId).MaxPettyCashAllowed >= empCurAvailBal - empReqAmount)
                {
                    curPettyCashBal.CurBalance = empCurAvailBal - empReqAmount;
                }

                curPettyCashBal.EmployeeId = reqEmpid;
                curPettyCashBal.UpdatedOn = DateTime.Now;
                _context.Update(curPettyCashBal);
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Department: pettyCashRequestDto balance check completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Department: pettyCashRequestDto balance check failed");

                }

                #endregion
                _logger.LogInformation("Department: pettyCashRequest insert start");
                //##### 2. Adding entry to PettyCashRequest table for record
                #region
                var pcrq = new PettyCashRequest()
                {
                    EmployeeId = reqEmpid,
                    PettyClaimAmount = empReqAmount,
                    CashReqDate = DateTime.Now,
                    PettyClaimRequestDesc = pettyCashRequestDto.PettyClaimRequestDesc,
                    ProjectId = pettyCashRequestDto.ProjectId,
                    SubProjectId = pettyCashRequestDto.SubProjectId,
                    WorkTaskId = pettyCashRequestDto.WorkTaskId,
                    DepartmentId = _context.Employees.Find(reqEmpid).DepartmentId,
                    CurrencyTypeId = pettyCashRequestDto.CurrencyTypeId,
                    ApprovalStatusTypeId = (int)EApprovalStatus.Pending,
                    Comments = "Cash Advance Request in Process!"

                };
                _context.PettyCashRequests.Add(pcrq);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Department: pettyCashRequest insert success");
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Department: pettyCashRequest insert failed");
                }



                //get the saved record Id
                pettyCashRequestDto.Id = pcrq.Id;

                #endregion

                //##### STEP 3. ClaimsApprovalTracker to be updated for all the allowed Approvers


                ///////////////////////////// Check if self Approved Request /////////////////////////////

                //if highest approver is requesting Petty cash request himself
                if (maxApprLevel == reqApprLevel)
                {
                    isSelfApprovedRequest = true;
                }
                //////////////////////////////////////////////////////////////////////////////////////////


                var getEmpClaimApproversAllLevels = _context.ApprovalRoleMaps
                                    .Include(a => a.ApprovalLevel)
                                    .Where(a => a.ApprovalGroupId == reqApprGroupId)
                                    .OrderBy(o => o.ApprovalLevel.Level).ToList();
                bool isFirstApprover = true;
                _logger.LogInformation("Department: pettyCashRequest status tracker insert start");
                if (isSelfApprovedRequest)
                {

                    ClaimApprovalStatusTracker claimAppStatusTrack = new()
                    {
                        EmployeeId = pettyCashRequestDto.EmployeeId,
                        PettyCashRequestId = pettyCashRequestDto.Id,
                        DepartmentId = _context.Employees.Find(pettyCashRequestDto.EmployeeId).DepartmentId,
                        ProjectId = null,
                        SubProjectId = null,
                        WorkTaskId = null,
                        RoleId = _context.Employees.Find(pettyCashRequestDto.EmployeeId).RoleId,
                        ApprovalGroupId = reqApprGroupId,
                        ApprovalLevelId = reqApprLevel,
                        ReqDate = DateTime.Now,
                        FinalApprovedDate = DateTime.Now,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Approved,
                        Comments = "Self Approved Request!"
                        //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                    };
                    _context.ClaimApprovalStatusTrackers.Add(claimAppStatusTrack);
                    pcrq.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                    pcrq.Comments = "Approved";


                    _context.PettyCashRequests.Update(pcrq);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Department: pettyCashRequest status tracker insert start");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Department: pettyCashRequest status tracker insert failed");
                    }

                }
                else
                {
                    foreach (ApprovalRoleMap ApprMap in getEmpClaimApproversAllLevels)
                    {

                        int role_id = ApprMap.RoleId;
                        var approver = _context.Employees.Where(e => e.RoleId == role_id && e.ApprovalGroupId == reqApprGroupId).FirstOrDefault();
                        if (approver == null)
                        {
                            continue;
                        }
                        int approverLevelid = _context.ApprovalRoleMaps.Where(x => x.RoleId == approver.RoleId && x.ApprovalGroupId == reqApprGroupId).FirstOrDefault().ApprovalLevelId;
                        int approverLevel = _context.ApprovalLevels.Find(approverLevelid).Level;

                        if (reqApprLevel >= approverLevel)
                        {
                            continue;
                        }


                        ClaimApprovalStatusTracker claimAppStatusTrack = new()
                        {
                            EmployeeId = pettyCashRequestDto.EmployeeId,
                            PettyCashRequestId = pettyCashRequestDto.Id,
                            DepartmentId = reqEmp.DepartmentId, //fix this
                            ProjManagerId = null,
                            ProjectId = null,
                            SubProjectId = null,
                            WorkTaskId = null,
                            RoleId = approver.RoleId,
                            ApprovalGroupId = reqApprGroupId,
                            ApprovalLevelId = ApprMap.ApprovalLevelId,
                            ReqDate = DateTime.Now,
                            FinalApprovedDate = null,
                            ApprovalStatusTypeId = isFirstApprover ? (int)EApprovalStatus.Pending : (int)EApprovalStatus.Initiating,
                            Comments = "Awaiting Approver Action"
                            //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                        };


                        _context.ClaimApprovalStatusTrackers.Add(claimAppStatusTrack);

                        await _context.SaveChangesAsync();



                        if (isFirstApprover)
                        {
                            //##### 4. Send email to the Approver
                            //####################################
                            _logger.LogInformation(approver.GetFullName() + "Email Start");

                            string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "PettyCashApprNotificationEmail.html" };
                            string FilePath = Path.Combine(paths);
                            _logger.LogInformation("Email template path " + FilePath);
                            StreamReader str = new StreamReader(FilePath);
                            string MailText = str.ReadToEnd();
                            str.Close();

                            var approverMailAddress = approver.Email;
                            string subject = "Pettycash Request Approval " + pettyCashRequestDto.Id.ToString();
                            Employee emp = _context.Employees.Find(pettyCashRequestDto.EmployeeId);
                            var pettycashreq = _context.PettyCashRequests.Find(pettyCashRequestDto.Id);

                            var builder = new MimeKit.BodyBuilder();

                            MailText = MailText.Replace("{Requester}", emp.GetFullName());
                            MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                            MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                            MailText = MailText.Replace("{RequestedAmount}", pettycashreq.PettyClaimAmount.ToString());
                            MailText = MailText.Replace("{RequestNumber}", pettycashreq.Id.ToString());
                            builder.HtmlBody = MailText;

                            var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                            await _emailSender.SendEmailAsync(messagemail);
                            _logger.LogInformation(approver.GetFullName() + "Email Sent");

                        }

                        //first approver will be added as Pending, other approvers will be with In Approval Queue
                        isFirstApprover = false;

                    }

                }

                //##### STEP 5. Adding a SINGLE entry in DisbursementsAndClaimsMaster table for records
                #region
                DisbursementsAndClaimsMaster disbursementsAndClaimsMaster = new();

                disbursementsAndClaimsMaster.EmployeeId = reqEmpid;
                disbursementsAndClaimsMaster.PettyCashRequestId = pcrq.Id;
                disbursementsAndClaimsMaster.ExpenseReimburseReqId = null;
                disbursementsAndClaimsMaster.RequestTypeId = (int)ERequestType.CashAdvance;
                disbursementsAndClaimsMaster.DepartmentId = _context.Employees.Find(reqEmpid).DepartmentId;
                disbursementsAndClaimsMaster.ProjectId = null;
                disbursementsAndClaimsMaster.SubProjectId = null;
                disbursementsAndClaimsMaster.WorkTaskId = null;
                disbursementsAndClaimsMaster.RecordDate = DateTime.Now;
                disbursementsAndClaimsMaster.CurrencyTypeId = pettyCashRequestDto.CurrencyTypeId;
                disbursementsAndClaimsMaster.ClaimAmount = empReqAmount;
                disbursementsAndClaimsMaster.AmountToCredit = isSelfApprovedRequest ? empReqAmount : 0;
                disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                disbursementsAndClaimsMaster.CostCenterId = _context.Departments.Find(_context.Employees.Find(reqEmpid).DepartmentId).CostCenterId;
                disbursementsAndClaimsMaster.ApprovalStatusId = isSelfApprovedRequest ? (int)EApprovalStatus.Approved : (int)EApprovalStatus.Pending;

                _context.DisbursementsAndClaimsMasters.Add(disbursementsAndClaimsMaster);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                #endregion
                await AtoCashDbContextTransaction.CommitAsync();
            }
            return 0;
        }

        private async Task<int> BusinessAreaCashRequest(PettyCashRequestDTO pettyCashRequestDto, Double empCurAvailBal)
        {
            //### 1. If Employee Eligible for Cash Claim enter a record and reduce the available amount for next claim
            #region
            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                _logger.LogInformation("BusinessAreaCashRequest - Start");
                int reqEmpid = pettyCashRequestDto.EmployeeId;
                Employee reqEmp = _context.Employees.Find(reqEmpid);
                int reqApprGroupId = (int)reqEmp.BusinessAreaApprovalGroupId;
                int reqRoleId = reqEmp.RoleId;
                int maxApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId).ToList().Select(x => x.ApprovalLevel).Max(a => a.Level);
                int reqApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId && a.RoleId == reqRoleId).Select(x => x.ApprovalLevel).FirstOrDefault().Level;
                bool isSelfApprovedRequest = false;

                Double empReqAmount = pettyCashRequestDto.PettyClaimAmount;




                var curPettyCashBal = _context.EmpCurrentPettyCashBalances.Where(x => x.EmployeeId == reqEmpid).FirstOrDefault();
                if (_context.JobRoles.Find(_context.Employees.Find(pettyCashRequestDto.EmployeeId).RoleId).MaxPettyCashAllowed >= empCurAvailBal - empReqAmount)
                {
                    curPettyCashBal.CurBalance = empCurAvailBal - empReqAmount;
                }

                curPettyCashBal.EmployeeId = reqEmpid;
                curPettyCashBal.UpdatedOn = DateTime.Now;
                _context.Update(curPettyCashBal);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("BusinessAreaCashRequest Balance check - Start");
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "BusinessAreaCashRequest Balance check - failed");
                }

                #endregion

                //##### 2. Adding entry to PettyCashRequest table for record
                #region

                _logger.LogInformation("BusinessAreaCash Request insert Start");
                var pcrq = new PettyCashRequest()
                {
                    EmployeeId = reqEmpid,
                    PettyClaimAmount = empReqAmount,
                    CashReqDate = DateTime.Now,
                    PettyClaimRequestDesc = pettyCashRequestDto.PettyClaimRequestDesc,
                    ProjectId = pettyCashRequestDto.ProjectId,
                    SubProjectId = pettyCashRequestDto.SubProjectId,
                    WorkTaskId = pettyCashRequestDto.WorkTaskId,
                    DepartmentId = _context.Employees.Find(reqEmpid).DepartmentId,
                    CurrencyTypeId = pettyCashRequestDto.CurrencyTypeId,
                    ApprovalStatusTypeId = (int)EApprovalStatus.Pending,
                    Comments = "Cash Advance Request in Process!"

                };
                _context.PettyCashRequests.Add(pcrq);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("BusinessAreaCash Request insert complete");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BusinessAreaCash Request insert failed");
                }



                //get the saved record Id
                pettyCashRequestDto.Id = pcrq.Id;

                #endregion

                //##### STEP 3. ClaimsApprovalTracker to be updated for all the allowed Approvers


                ///////////////////////////// Check if self Approved Request /////////////////////////////

                //if highest approver is requesting Petty cash request himself
                if (maxApprLevel == reqApprLevel)
                {
                    isSelfApprovedRequest = true;
                }
                //////////////////////////////////////////////////////////////////////////////////////////


                var getEmpClaimApproversAllLevels = _context.ApprovalRoleMaps
                                    .Include(a => a.ApprovalLevel)
                                    .Where(a => a.ApprovalGroupId == reqApprGroupId)
                                    .OrderBy(o => o.ApprovalLevel.Level).ToList();
                bool isFirstApprover = true;

                if (isSelfApprovedRequest)
                {
                    _logger.LogInformation("BusinessAreaCash Request Status tracker insert start");

                    ClaimApprovalStatusTracker claimAppStatusTrack = new()
                    {
                        EmployeeId = pettyCashRequestDto.EmployeeId,
                        PettyCashRequestId = pettyCashRequestDto.Id,
                        DepartmentId = _context.Employees.Find(pettyCashRequestDto.EmployeeId).DepartmentId,
                        ProjectId = null,
                        SubProjectId = null,
                        WorkTaskId = null,
                        RoleId = _context.Employees.Find(pettyCashRequestDto.EmployeeId).RoleId,
                        ApprovalGroupId = reqApprGroupId,
                        ApprovalLevelId = reqApprLevel,
                        ReqDate = DateTime.Now,
                        FinalApprovedDate = DateTime.Now,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Approved,
                        Comments = "Self Approved Request!"
                        //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                    };
                    _context.ClaimApprovalStatusTrackers.Add(claimAppStatusTrack);
                    pcrq.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                    pcrq.Comments = "Approved";
                    _context.PettyCashRequests.Update(pcrq);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("BusinessAreaCash Request Status tracker insert complete");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "BusinessAreaCash Request Status tracker insert failed");
                    }

                }
                else
                {
                    _logger.LogInformation("BusinessAreaCash Request Status tracker insert start");
                    foreach (ApprovalRoleMap ApprMap in getEmpClaimApproversAllLevels)
                    {

                        int role_id = ApprMap.RoleId;
                        var approver = _context.Employees.Where(e => e.RoleId == role_id && e.ApprovalGroupId == reqApprGroupId).FirstOrDefault();
                        if (approver == null)
                        {
                            continue;
                        }
                        int approverLevelid = _context.ApprovalRoleMaps.Where(x => x.RoleId == approver.RoleId && x.ApprovalGroupId == reqApprGroupId).FirstOrDefault().ApprovalLevelId;
                        int approverLevel = _context.ApprovalLevels.Find(approverLevelid).Level;

                        if (reqApprLevel >= approverLevel)
                        {
                            continue;
                        }



                        ClaimApprovalStatusTracker claimAppStatusTrack = new()
                        {
                            EmployeeId = pettyCashRequestDto.EmployeeId,
                            PettyCashRequestId = pettyCashRequestDto.Id,
                            DepartmentId = reqEmp.DepartmentId, //fix this
                            ProjManagerId = null,
                            ProjectId = null,
                            SubProjectId = null,
                            WorkTaskId = null,
                            RoleId = approver.RoleId,
                            ApprovalGroupId = reqApprGroupId,
                            ApprovalLevelId = ApprMap.ApprovalLevelId,
                            ReqDate = DateTime.Now,
                            FinalApprovedDate = null,
                            ApprovalStatusTypeId = isFirstApprover ? (int)EApprovalStatus.Pending : (int)EApprovalStatus.Initiating,
                            Comments = "Awaiting Approver Action"
                            //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                        };


                        _context.ClaimApprovalStatusTrackers.Add(claimAppStatusTrack);


                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation("BusinessAreaCash Request Status tracker insert failed");
                        }



                        if (isFirstApprover)
                        {
                            //##### 4. Send email to the Approver
                            //####################################
                            //string[] paths = {@"d:\archives", "2001", "media", "images"};

                            _logger.LogInformation(approver.GetFullName() + "Email Start");

                            string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "PettyCashApprNotificationEmail.html" };
                            string FilePath = Path.Combine(paths);
                            _logger.LogInformation("Email template path " + FilePath);
                            StreamReader str = new StreamReader(FilePath);
                            string MailText = str.ReadToEnd();
                            str.Close();

                            var approverMailAddress = approver.Email;
                            string subject = "Pettycash Request Approval " + pettyCashRequestDto.Id.ToString();
                            Employee emp = _context.Employees.Find(pettyCashRequestDto.EmployeeId);
                            var pettycashreq = _context.PettyCashRequests.Find(pettyCashRequestDto.Id);

                            var builder = new MimeKit.BodyBuilder();

                            MailText = MailText.Replace("{Requester}", emp.GetFullName());
                            MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                            MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                            MailText = MailText.Replace("{RequestedAmount}", pettycashreq.PettyClaimAmount.ToString());
                            MailText = MailText.Replace("{RequestNumber}", pettycashreq.Id.ToString());
                            builder.HtmlBody = MailText;

                            var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                            await _emailSender.SendEmailAsync(messagemail);

                            _logger.LogInformation(approver.GetFullName() + "Email Sent");
                        }

                        //first approver will be added as Pending, other approvers will be with In Approval Queue
                        isFirstApprover = false;

                    }

                }

                //##### STEP 5. Adding a SINGLE entry in DisbursementsAndClaimsMaster table for records

                #region

                _logger.LogInformation("DisbursementsAndClaimsMaster Insert start");

                DisbursementsAndClaimsMaster disbursementsAndClaimsMaster = new();

                disbursementsAndClaimsMaster.EmployeeId = reqEmpid;
                disbursementsAndClaimsMaster.PettyCashRequestId = pcrq.Id;
                disbursementsAndClaimsMaster.ExpenseReimburseReqId = null;
                disbursementsAndClaimsMaster.RequestTypeId = (int)ERequestType.CashAdvance;
                disbursementsAndClaimsMaster.DepartmentId = _context.Employees.Find(reqEmpid).DepartmentId;
                disbursementsAndClaimsMaster.ProjectId = null;
                disbursementsAndClaimsMaster.SubProjectId = null;
                disbursementsAndClaimsMaster.WorkTaskId = null;
                disbursementsAndClaimsMaster.RecordDate = DateTime.Now;
                disbursementsAndClaimsMaster.CurrencyTypeId = pettyCashRequestDto.CurrencyTypeId;
                disbursementsAndClaimsMaster.ClaimAmount = empReqAmount;
                disbursementsAndClaimsMaster.AmountToCredit = isSelfApprovedRequest ? empReqAmount : 0;
                disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                disbursementsAndClaimsMaster.CostCenterId = _context.Departments.Find(_context.Employees.Find(reqEmpid).DepartmentId).CostCenterId;
                disbursementsAndClaimsMaster.ApprovalStatusId = isSelfApprovedRequest ? (int)EApprovalStatus.Approved : (int)EApprovalStatus.Pending;

                _context.DisbursementsAndClaimsMasters.Add(disbursementsAndClaimsMaster);
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("DisbursementsAndClaimsMaster Insert completed");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError("DisbursementsAndClaimsMaster Insert failed");
                }
                #endregion
                await AtoCashDbContextTransaction.CommitAsync();
            }
            return 0;
        }

        private void AddEmpCurrentPettyCashBalanceForEmployee(int id)
        {
            if (id == 0)
            {
                return;
            }

            var emp = _context.Employees.Find(id);

            if (emp != null)
            {
                Double empPettyCashAmountEligible = _context.JobRoles.Find(_context.Employees.Find(id).RoleId).MaxPettyCashAllowed;
                _context.EmpCurrentPettyCashBalances.Add(new EmpCurrentPettyCashBalance()
                {
                    EmployeeId = id,
                    CurBalance = empPettyCashAmountEligible,
                    UpdatedOn = DateTime.Now
                });

                _context.SaveChangesAsync();
            }
            return;

        }
    }
}
