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

        public PettyCashRequestsController(AtoCashDbContext context, IEmailSender emailSender)
        {
            this._context = context;
            this._emailSender = emailSender;
        }


        // GET: api/PettyCashRequests
        [HttpGet]
        [ActionName("GetPettyCashRequests")]
        public async Task<ActionResult<IEnumerable<PettyCashRequestDTO>>> GetPettyCashRequests()
        {
            List<PettyCashRequestDTO> ListPettyCashRequestDTO = new();

            //var claimApprovalStatusTracker = await _context.ClaimApprovalStatusTrackers.FindAsync(1);

            var pettyCashRequests = await _context.PettyCashRequests.ToListAsync();

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
            pettyCashRequestDTO.CreditToBank = pettyCashRequest.ApprovalStatusTypeId == (int)EApprovalStatus.Approved ?  disbAndClaim.AmountToCredit : 0;
            pettyCashRequestDTO.IsSettled = (disbAndClaim.IsSettledAmountCredited ?? false);

            pettyCashRequestDTO.Comments = pettyCashRequest.Comments;

            return pettyCashRequestDTO;
        }





        [HttpGet("{id}")]
        [ActionName("GetPettyCashRequestRaisedForEmployee")]
        public async Task<ActionResult<IEnumerable<PettyCashRequestDTO>>> GetPettyCashRequestRaisedForEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee Id is Invalid!" });
            }

            //get the employee's approval level for comparison with approver level  to decide "ShowEditDelete" bool
            int reqEmpApprLevelId = _context.ApprovalRoleMaps.Where(a => a.RoleId == _context.Employees.Find(id).RoleId).FirstOrDefault().ApprovalLevelId;
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
                pettyCashRequestDTO.Department = pettyCashRequest.DepartmentId != null ? _context.Departments.Find(pettyCashRequest.DepartmentId).DeptCode + _context.Departments.Find(pettyCashRequest.DepartmentId).DeptName : null;
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
                                                           ( t.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected ||
                                                          t.ApprovalStatusTypeId == (int)EApprovalStatus.Approved) &&
                                                          t.PettyCashRequestId == pettyCashRequest.Id).Any();

                if(ifAnyOfStatusRecordsApproved)
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
                    //####################################
                    var approver = _context.Employees.Where(e => e.RoleId == claim.RoleId && e.ApprovalGroupId == claim.ApprovalGroupId).FirstOrDefault();
                    var approverMailAddress = approver.Email;
                    string subject = "(Modified) Pettycash Request Approval " + pettyCashRequestDto.Id.ToString();
                    Employee emp = await _context.Employees.FindAsync(pettyCashRequest.EmployeeId);
                    var pettycashreq = _context.PettyCashRequests.Find(pettyCashRequestDto.Id);
                    string content = "(Modified) Petty Cash Approval sought by " + emp.GetFullName() + "@<br/>Cash Request for the amount of " + pettycashreq.PettyClaimAmount + "@<br/>towards " + pettycashreq.PettyClaimRequestDesc;
                    var messagemail = new Message(new string[] { approverMailAddress }, subject, content);

                    await _emailSender.SendEmailAsync(messagemail);

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
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new RespStatus { Status = "Success", Message = "Request Updated!" });
        }

        // POST: api/PettyCashRequests
        [HttpPost]
        [ActionName("PostPettyCashRequest")]
        public async Task<ActionResult<PettyCashRequest>> PostPettyCashRequest(PettyCashRequestDTO pettyCashRequestDto)
        {

            /*!!=========================================
               Check Eligibility for Cash Disbursement
             .==========================================*/

            Double empCurAvailBal = GetEmpCurrentAvailablePettyCashBalance(pettyCashRequestDto);

            if (pettyCashRequestDto.PettyClaimAmount <= empCurAvailBal && pettyCashRequestDto.PettyClaimAmount > 0)
            {
                await Task.Run(() => ProcessPettyCashRequestClaim(pettyCashRequestDto, empCurAvailBal));

                return Created("PostPettyCashRequest", new RespStatus() { Status = "Success", Message = "Cash Advance Request Created" });

            }
            else
            {
                return Conflict(new RespStatus() { Status = "Failure", Message = "Invalid Cash Request Amount Or Limit Exceeded" });
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



        //NO HTTPACTION HERE. Void method just to add data to database table
        private async Task ProcessPettyCashRequestClaim(PettyCashRequestDTO pettyCashRequestDto, Double empCurAvailBal)
        {

            if (pettyCashRequestDto.ProjectId != null)
            {
                //Goes to Option 1 (Project)
                await Task.Run(() => ProjectCashRequest(pettyCashRequestDto, empCurAvailBal));
            }
            else
            {
                //Goes to Option 2 (Department)
                await Task.Run(() => DepartmentCashRequest(pettyCashRequestDto, empCurAvailBal));
            }

        }


        /// <summary>
        /// This is the option 1 : : PROJECT BASED CASH ADVANCE REQUEST
        /// </summary>
        /// <param name="pettyCashRequestDto"></param>
        /// <param name="empCurAvailBal"></param>
        private async Task<IActionResult> ProjectCashRequest(PettyCashRequestDTO pettyCashRequestDto, Double empCurAvailBal)
        {

            //### 1. If Employee Eligible for Cash Claim enter a record and reduce the available amount for next claim
            #region
            int costCenterId = _context.Projects.Find(pettyCashRequestDto.ProjectId).CostCenterId;

            int projManagerid = _context.Projects.Find(pettyCashRequestDto.ProjectId).ProjectManagerId;

            var approver = _context.Employees.Find(projManagerid);
            ////
            int empid = pettyCashRequestDto.EmployeeId;
            Double empReqAmount = pettyCashRequestDto.PettyClaimAmount;
            //int empApprGroupId = _context.Employees.Find(empid).ApprovalGroupId;
            double maxCashAllowedForRole = (_context.JobRoles.Find(_context.Employees.Find(pettyCashRequestDto.EmployeeId).RoleId).MaxPettyCashAllowed);

            if (pettyCashRequestDto.PettyClaimAmount > maxCashAllowedForRole)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Advance Amount is not eligibile" });
            }

            var curPettyCashBal = _context.EmpCurrentPettyCashBalances.Where(x => x.EmployeeId == empid).FirstOrDefault();
            curPettyCashBal.Id = curPettyCashBal.Id;
            curPettyCashBal.CurBalance = empCurAvailBal - empReqAmount <= maxCashAllowedForRole ? empCurAvailBal - empReqAmount : maxCashAllowedForRole;
            curPettyCashBal.EmployeeId = empid;
            curPettyCashBal.UpdatedOn = DateTime.Now;
            _context.Update(curPettyCashBal);

            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();

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
                var approverMailAddress = approver.Email;
                string subject = "Pettycash Request Approval " + pettyCashRequestDto.Id.ToString();
                Employee emp = await _context.Employees.FindAsync(pettyCashRequestDto.EmployeeId);
                var pettycashreq = _context.PettyCashRequests.Find(pettyCashRequestDto.Id);
                string content = "Petty Cash Approval sought by " + emp.GetFullName() + "/nCash Request for the amount of " + pettycashreq.PettyClaimAmount + "/ntowards " + pettycashreq.PettyClaimRequestDesc;
                var messagemail = new Message(new string[] { approverMailAddress }, subject, content);

                await _emailSender.SendEmailAsync(messagemail);
                #endregion
            }



            //##### 5. Adding a entry in DisbursementsAndClaimsMaster table for records
            #region

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
            disbursementsAndClaimsMaster.AmountToCredit = isSelfApprovedRequest ? pettyCashRequestDto.PettyClaimAmount : 0 ;
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

            return Ok(new RespStatus { Status = "Success", Message = "Advance Request Created!" });
        }

        /// <summary>
        /// This is option 2 : DEPARTMENT BASED CASH ADVANCE REQUEST
        /// </summary>
        /// <param name="pettyCashRequestDto"></param>
        /// <param name="empCurAvailBal"></param>
        private async Task DepartmentCashRequest(PettyCashRequestDTO pettyCashRequestDto, Double empCurAvailBal)
        {
            //### 1. If Employee Eligible for Cash Claim enter a record and reduce the available amount for next claim
            #region

            int reqEmpid = pettyCashRequestDto.EmployeeId;
            Employee reqEmp = _context.Employees.Find(reqEmpid);
            int reqApprGroupId = reqEmp.ApprovalGroupId;
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
            await _context.SaveChangesAsync();
            #endregion

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


            await _context.SaveChangesAsync();

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
               await  _context.SaveChangesAsync();
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
                        var approverMailAddress = approver.Email;
                        string subject = "Pettycash Request Approval " + pettyCashRequestDto.Id.ToString();
                        Employee emp = await _context.Employees.FindAsync(pettyCashRequestDto.EmployeeId);
                        var pettycashreq = _context.PettyCashRequests.Find(pettyCashRequestDto.Id);
                        string content = "Petty Cash Approval sought by " + emp.GetFullName() + "@<br/>Cash Request for the amount of " + pettycashreq.PettyClaimAmount + "@<br/>towards " + pettycashreq.PettyClaimRequestDesc;
                        var messagemail = new Message(new string[] { approverMailAddress }, subject, content);

                        await _emailSender.SendEmailAsync(messagemail);
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
            disbursementsAndClaimsMaster.AmountToCredit = isSelfApprovedRequest ? empReqAmount : 0 ;
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
