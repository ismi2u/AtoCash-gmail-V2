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


    public class TravelApprovalRequestsController : ControllerBase
    {
        private readonly AtoCashDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<TravelApprovalRequestsController> _logger;
        public TravelApprovalRequestsController(AtoCashDbContext context, IEmailSender emailSender, ILogger<TravelApprovalRequestsController> logger)
        {
            this._context = context;
            this._emailSender = emailSender;
            _logger = logger;
        }


        // GET: api/TravelApprovalRequests
        [HttpGet]
        [ActionName("GetTravelApprovalRequests")]
        public async Task<ActionResult<IEnumerable<TravelApprovalRequestDTO>>> GetTravelApprovalRequests()
        {
            List<TravelApprovalRequestDTO> ListTravelApprovalRequestDTO = new();

            //var claimApprovalStatusTracker = await _context.TravelApprovalRequests.FindAsync(1);

            var TravelApprovalRequests = await _context.TravelApprovalRequests.ToListAsync();

            if (TravelApprovalRequests == null)
            {
                _logger.LogInformation("GetTravelApprovalRequests - null records");
            }

            foreach (TravelApprovalRequest travelApprovalRequest in TravelApprovalRequests)
            {
                TravelApprovalRequestDTO travelApprovalRequestDTO = new();

                travelApprovalRequestDTO.Id = travelApprovalRequest.Id;
                travelApprovalRequestDTO.EmployeeId = travelApprovalRequest.EmployeeId;
                travelApprovalRequestDTO.EmployeeName = _context.Employees.Find(travelApprovalRequest.EmployeeId).GetFullName();
                travelApprovalRequestDTO.TravelStartDate = travelApprovalRequest.TravelStartDate;
                travelApprovalRequestDTO.TravelEndDate = travelApprovalRequest.TravelEndDate;
                travelApprovalRequestDTO.TravelPurpose = travelApprovalRequest.TravelPurpose;
                travelApprovalRequestDTO.ReqRaisedDate = travelApprovalRequest.ReqRaisedDate;
                travelApprovalRequestDTO.DepartmentId = travelApprovalRequest.DepartmentId;
                travelApprovalRequestDTO.DepartmentName = travelApprovalRequest.DepartmentId != null ? _context.Departments.Find(travelApprovalRequest.DepartmentId).DeptCode : null;
                travelApprovalRequestDTO.ProjectId = travelApprovalRequest.ProjectId;
                travelApprovalRequestDTO.ProjectName = travelApprovalRequest.ProjectId != null ? _context.Projects.Find(travelApprovalRequest.ProjectId).ProjectName : null;
                travelApprovalRequestDTO.SubProjectId = travelApprovalRequest.SubProjectId;
                travelApprovalRequestDTO.SubProjectName = travelApprovalRequest.SubProjectId != null ? _context.SubProjects.Find(travelApprovalRequest.SubProjectId).SubProjectName : null;
                travelApprovalRequestDTO.WorkTaskId = travelApprovalRequest.WorkTaskId;
                travelApprovalRequestDTO.WorkTaskName = travelApprovalRequest.WorkTaskId != null ? _context.WorkTasks.Find(travelApprovalRequest.WorkTaskId).TaskName : null;
                travelApprovalRequestDTO.ApprovalStatusTypeId = travelApprovalRequest.ApprovalStatusTypeId;
                travelApprovalRequestDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(travelApprovalRequest.ApprovalStatusTypeId).Status;
                travelApprovalRequestDTO.ApprovedDate = travelApprovalRequest.ApprovedDate;


                ListTravelApprovalRequestDTO.Add(travelApprovalRequestDTO);
            }

            return ListTravelApprovalRequestDTO.OrderByDescending(o => o.ReqRaisedDate).ToList();
        }



        // GET: api/TravelApprovalRequests/5
        [HttpGet("{id}")]
        [ActionName("GetTravelApprovalRequest")]
        public async Task<ActionResult<TravelApprovalRequestDTO>> GetTravelApprovalRequest(int id)
        {


            var travelApprovalRequest = await _context.TravelApprovalRequests.FindAsync(id);

            if (travelApprovalRequest == null)
            {
                _logger.LogError("GetTravelApprovalRequest Request Id is not valid Id:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Travel Approval Request Id invalid!" });
            }
            TravelApprovalRequestDTO travelApprovalRequestDTO = new();


            travelApprovalRequestDTO.Id = travelApprovalRequest.Id;
            travelApprovalRequestDTO.EmployeeId = travelApprovalRequest.EmployeeId;
            travelApprovalRequestDTO.EmployeeName = _context.Employees.Find(travelApprovalRequest.EmployeeId).GetFullName();
            travelApprovalRequestDTO.TravelStartDate = travelApprovalRequest.TravelStartDate;
            travelApprovalRequestDTO.TravelEndDate = travelApprovalRequest.TravelEndDate;
            travelApprovalRequestDTO.TravelPurpose = travelApprovalRequest.TravelPurpose;
            travelApprovalRequestDTO.ReqRaisedDate = travelApprovalRequest.ReqRaisedDate;
            travelApprovalRequestDTO.DepartmentId = travelApprovalRequest.DepartmentId;
            travelApprovalRequestDTO.DepartmentName = travelApprovalRequest.DepartmentId != null ? _context.Departments.Find(travelApprovalRequest.DepartmentId).DeptCode : null;
            travelApprovalRequestDTO.ProjectId = travelApprovalRequest.ProjectId;
            travelApprovalRequestDTO.ProjectName = travelApprovalRequest.ProjectId != null ? _context.Projects.Find(travelApprovalRequest.ProjectId).ProjectName : null;
            travelApprovalRequestDTO.SubProjectId = travelApprovalRequest.SubProjectId;
            travelApprovalRequestDTO.SubProjectName = travelApprovalRequest.SubProjectId != null ? _context.SubProjects.Find(travelApprovalRequest.SubProjectId).SubProjectName : null;
            travelApprovalRequestDTO.WorkTaskId = travelApprovalRequest.WorkTaskId;
            travelApprovalRequestDTO.WorkTaskName = travelApprovalRequest.WorkTaskId != null ? _context.WorkTasks.Find(travelApprovalRequest.WorkTaskId).TaskName : null;
            travelApprovalRequestDTO.ApprovalStatusTypeId = travelApprovalRequest.ApprovalStatusTypeId;
            travelApprovalRequestDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(travelApprovalRequest.ApprovalStatusTypeId).Status;
            travelApprovalRequestDTO.ApprovedDate = travelApprovalRequest.ApprovedDate;

            travelApprovalRequestDTO.Comments = travelApprovalRequest.Comments;

            return travelApprovalRequestDTO;
        }





        [HttpGet("{id}")]
        [ActionName("GetTravelApprovalRequestRaisedForEmployee")]
        public async Task<ActionResult<IEnumerable<TravelApprovalRequestDTO>>> GetTravelApprovalRequestRaisedForEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            var roleid = employee.RoleId;

            if (employee == null)
            {
                _logger.LogError("Travel : Employee Id is not valid:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee Id is Invalid!" });
            }

            //get the employee's approval level for comparison with approver level  to decide "ShowEditDelete" bool
            int reqEmpApprLevelId = 0;
            try
            {
                reqEmpApprLevelId = _context.ApprovalRoleMaps.Where(a => a.RoleId == roleid).FirstOrDefault().ApprovalLevelId;

            }
            catch (Exception ex)
            {
                _logger.LogError("Employee reqEmpApprLevelId is null for Employee id: " + id);
                return Ok(new RespStatus { Status = "Failure", Message = "Employee Approval Level not defined!" });
            }
            int reqEmpApprLevel = _context.ApprovalLevels.Find(reqEmpApprLevelId).Level;


            var TravelApprovalRequests = await _context.TravelApprovalRequests.Where(p => p.EmployeeId == id).ToListAsync();

            if (TravelApprovalRequests == null)
            {
                _logger.LogError("Travel :TravelApprovalRequests is null");
                return Conflict(new RespStatus { Status = "Failure", Message = "Travel Approval Request Id invalid!" });
            }

            List<TravelApprovalRequestDTO> TravelApprovalRequestDTOs = new();

            foreach (var travelApprovalRequest in TravelApprovalRequests)
            {
                TravelApprovalRequestDTO travelApprovalRequestDTO = new();

                travelApprovalRequestDTO.Id = travelApprovalRequest.Id;
                travelApprovalRequestDTO.EmployeeId = travelApprovalRequest.EmployeeId;
                travelApprovalRequestDTO.EmployeeName = _context.Employees.Find(travelApprovalRequest.EmployeeId).GetFullName();
                travelApprovalRequestDTO.TravelStartDate = travelApprovalRequest.TravelStartDate;
                travelApprovalRequestDTO.TravelEndDate = travelApprovalRequest.TravelEndDate;
                travelApprovalRequestDTO.TravelPurpose = travelApprovalRequest.TravelPurpose;
                travelApprovalRequestDTO.ReqRaisedDate = travelApprovalRequest.ReqRaisedDate;
                travelApprovalRequestDTO.DepartmentId = travelApprovalRequest.DepartmentId;
                travelApprovalRequestDTO.DepartmentName = travelApprovalRequest.DepartmentId != null ? _context.Departments.Find(travelApprovalRequest.DepartmentId).DeptCode : null;
                travelApprovalRequestDTO.ProjectId = travelApprovalRequest.ProjectId;
                travelApprovalRequestDTO.ProjectName = travelApprovalRequest.ProjectId != null ? _context.Projects.Find(travelApprovalRequest.ProjectId).ProjectName : null;
                travelApprovalRequestDTO.SubProjectId = travelApprovalRequest.SubProjectId;
                travelApprovalRequestDTO.SubProjectName = travelApprovalRequest.SubProjectId != null ? _context.SubProjects.Find(travelApprovalRequest.SubProjectId).SubProjectName : null;
                travelApprovalRequestDTO.WorkTaskId = travelApprovalRequest.WorkTaskId;
                travelApprovalRequestDTO.WorkTaskName = travelApprovalRequest.WorkTaskId != null ? _context.WorkTasks.Find(travelApprovalRequest.WorkTaskId).TaskName : null;
                travelApprovalRequestDTO.ApprovalStatusTypeId = travelApprovalRequest.ApprovalStatusTypeId;
                travelApprovalRequestDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(travelApprovalRequest.ApprovalStatusTypeId).Status;
                travelApprovalRequestDTO.ApprovedDate = travelApprovalRequest.ApprovedDate;




                // set the bookean flat to TRUE if No approver has yet approved the Request else FALSE
                bool ifAnyOfStatusRecordsApproved = _context.TravelApprovalStatusTrackers.Where(t =>
                                                         (t.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected ||
                                                          t.ApprovalStatusTypeId == (int)EApprovalStatus.Approved) &&
                                                          t.TravelApprovalRequestId == travelApprovalRequest.Id).Any();

                if (ifAnyOfStatusRecordsApproved)
                {
                    travelApprovalRequestDTO.ShowEditDelete = false;
                }
                else
                {
                    travelApprovalRequestDTO.ShowEditDelete = true;
                }


                //;

                TravelApprovalRequestDTOs.Add(travelApprovalRequestDTO);
            }


            return Ok(TravelApprovalRequestDTOs.OrderByDescending(o => o.ReqRaisedDate).ToList());
        }


        [HttpGet("{id}")]
        [ActionName("CountAllTravelRequestRaisedByEmployee")]
        public async Task<ActionResult> CountAllTravelRequestRaisedByEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return Ok(0);
            }

            var travelApprovalRequests = await _context.TravelApprovalRequests.Where(p => p.EmployeeId == id).ToListAsync();

            if (travelApprovalRequests == null)
            {
                return Ok(0);
            }

            int TotalCount = _context.TravelApprovalRequests.Where(c => c.EmployeeId == id).Count();
            int PendingCount = _context.TravelApprovalRequests.Where(c => c.EmployeeId == id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).Count();
            int RejectedCount = _context.TravelApprovalRequests.Where(c => c.EmployeeId == id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected).Count();
            int ApprovedCount = _context.TravelApprovalRequests.Where(c => c.EmployeeId == id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).Count();

            return Ok(new { TotalCount, PendingCount, RejectedCount, ApprovedCount });
        }


        [HttpGet]
        [ActionName("GetTravelReqInPendingForAll")]
        public async Task<ActionResult<int>> GetTravelReqInPendingForAll()
        {
            //debug
            var TravelApprovalRequests = await _context.TravelApprovalRequests.Include("TravelApprovalStatusTrackers").ToListAsync();


            //var TravelApprovalRequests = await _context.TravelApprovalRequests.Where(c => c.ApprovalStatusTypeId == ApprovalStatus.Pending).select( );

            if (TravelApprovalRequests == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Travel Approval Request Id invalid!" });
            }

            return Ok(TravelApprovalRequests.Count);
        }



        // PUT: api/TravelApprovalRequests/5
        [HttpPut("{id}")]
        [ActionName("PutTravelApprovalRequest")]

        public async Task<IActionResult> PutTravelApprovalRequest(int id, TravelApprovalRequestDTO travelApprovalRequestDTO)
        {
            if (id != travelApprovalRequestDTO.Id)
            {
                _logger.LogError("Travel Request: Travel Id is invalid - update failed");
                return Conflict(new RespStatus { Status = "Failure", Message = "Travel Id is invalid" });
            }

            var travelApprovalRequest = await _context.TravelApprovalRequests.FindAsync(id);

            ///update the Wallet of the employe to reflect the changes
            int ApprovedCount = _context.TravelApprovalStatusTrackers.Where(e => e.TravelApprovalRequestId == travelApprovalRequest.Id && e.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).Count();
            if (ApprovedCount != 0)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Travel Requests cant be Edited after Approval!" });
            }


            travelApprovalRequest.Id = travelApprovalRequestDTO.Id;
            travelApprovalRequest.EmployeeId = travelApprovalRequestDTO.EmployeeId;
            travelApprovalRequest.TravelStartDate = travelApprovalRequestDTO.TravelStartDate;
            travelApprovalRequest.TravelEndDate = travelApprovalRequestDTO.TravelEndDate;
            travelApprovalRequest.TravelPurpose = travelApprovalRequestDTO.TravelPurpose;
            travelApprovalRequest.ReqRaisedDate = DateTime.Now;
            if (travelApprovalRequestDTO.DepartmentId != null)
            {
                travelApprovalRequest.DepartmentId = travelApprovalRequestDTO.DepartmentId;
            }
            if (travelApprovalRequestDTO.ProjectId != null)
            {
                travelApprovalRequest.ProjectId = travelApprovalRequestDTO.ProjectId;
                travelApprovalRequest.SubProjectId = travelApprovalRequestDTO.SubProjectId;
                travelApprovalRequest.WorkTaskId = travelApprovalRequestDTO.WorkTaskId;
            }

            _context.TravelApprovalRequests.Update(travelApprovalRequest);


            //now update the TravelApprovalStatus Trackers
            var travelStatusTrackers = _context.TravelApprovalStatusTrackers.Where(e => e.TravelApprovalRequestId == travelApprovalRequest.Id).OrderBy(o => o.Id).ToList();
            bool IsFirstEmail = true;
            foreach (var travel in travelStatusTrackers)
            {

                TravelApprovalStatusTracker travelStatusItem = await _context.TravelApprovalStatusTrackers.FindAsync(travel.Id);


                travelStatusItem.TravelStartDate = travelApprovalRequestDTO.TravelStartDate;
                travelStatusItem.TravelEndDate = travelApprovalRequestDTO.TravelEndDate;

                if (travelApprovalRequestDTO.DepartmentId != null)
                {
                    travelStatusItem.DepartmentId = travelApprovalRequestDTO.DepartmentId;
                }
                if (travelApprovalRequestDTO.ProjectId != null)
                {
                    travelStatusItem.ProjectId = travelApprovalRequestDTO.ProjectId;
                    travelStatusItem.SubProjectId = travelApprovalRequestDTO.SubProjectId;
                    travelStatusItem.WorkTaskId = travelApprovalRequestDTO.WorkTaskId;
                }

                travelStatusItem.ReqDate = DateTime.Now;

                _context.TravelApprovalStatusTrackers.Update(travelStatusItem);

                if (IsFirstEmail)
                {
                    _logger.LogInformation("Travel Approval Email Start");

                    string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "TravelApprNotificationEmail.html" };
                    string FilePath = Path.Combine(paths);
                    _logger.LogInformation("Email template path " + FilePath);
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();

                    var approver = _context.Employees.Where(e => e.RoleId == travelStatusItem.RoleId && e.ApprovalGroupId == travelStatusItem.ApprovalGroupId).FirstOrDefault();
                    var approverMailAddress = approver.Email;
                    string subject = "(Modified) Travel Approval Request No# " + travelApprovalRequestDTO.Id.ToString();
                    Employee emp = await _context.Employees.FindAsync(travelApprovalRequestDTO.EmployeeId);
                    var travelreq = _context.TravelApprovalRequests.Find(travelApprovalRequestDTO.Id);

                    var builder = new MimeKit.BodyBuilder();

                    MailText = MailText.Replace("{Requester}", emp.GetFullName());
                    MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                    MailText = MailText.Replace("{Request}", travelreq.TravelStartDate.ToString() + " - " + travelreq.TravelEndDate.ToString() + " (Purpose): " + travelreq.TravelPurpose.ToString());
                    MailText = MailText.Replace("{RequestNumber}", travelreq.Id.ToString());
                    builder.HtmlBody = MailText;

                    var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                    await _emailSender.SendEmailAsync(messagemail);
                    _logger.LogInformation("Travel Request update Email Sent");

                    IsFirstEmail = false;
                }
            }



            //_context.Entry(travelApprovalRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogInformation("Travel Request update failed ");
                return BadRequest(new RespStatus { Status = "Failure", Message = "Travel Approval Request update failed!" });
            }

            return Ok(new RespStatus { Status = "Success", Message = "Travel Approval Request Updated!" });

        }

        // POST: api/TravelApprovalRequests
        [HttpPost]
        [ActionName("PostTravelApprovalRequest")]
        public async Task<ActionResult<TravelApprovalRequest>> PostTravelApprovalRequest(TravelApprovalRequestDTO travelApprovalRequestDTO)
        {
            //Step ##1

            int SuccessResult;

            var dupReq = _context.TravelApprovalRequests.Where(
                t => t.TravelStartDate.Date == travelApprovalRequestDTO.TravelStartDate.Date &&
                t.TravelEndDate.Date == travelApprovalRequestDTO.TravelEndDate.Date &&
                t.EmployeeId == travelApprovalRequestDTO.EmployeeId &&
                t.TravelPurpose == travelApprovalRequestDTO.TravelPurpose).Count();

            if (dupReq != 0)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Duplicate request cannot be created" });
            }




            //##Step 2

            if (travelApprovalRequestDTO.ProjectId != null)
            {
                //Goes to Option 1 (Project)
                SuccessResult = await Task.Run(() => ProjectTravelRequest(travelApprovalRequestDTO));
            }
            else
            {
                //Goes to Option 2 (Department)
                SuccessResult = await Task.Run(() => DepartmentTravelRequest(travelApprovalRequestDTO));
            }


            if (SuccessResult == 0)
            {
                _logger.LogInformation("PostExpenseReimburseRequest - Process completed");

                return Ok(new RespStatus { Status = "Success", Message = "Travel Request Created!" });
            }
            else
            {
                _logger.LogError("Expense Reimburse Request creation failed!");

                return BadRequest(new RespStatus { Status = "Failure", Message = "Travel Request creation failed!" });
            }

        }

        // DELETE: api/TravelApprovalRequests/5
        [HttpDelete("{id}")]
        [ActionName("DeleteTravelApprovalRequest")]
        public async Task<IActionResult> DeleteTravelApprovalRequest(int id)
        {
            var travelApprovalRequest = await _context.TravelApprovalRequests.FindAsync(id);
            if (travelApprovalRequest == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Travel Approval Request Id invalid!" });
            }

            var trvlappStatusTrackers = _context.TravelApprovalStatusTrackers.Where(c => c.TravelApprovalRequestId == travelApprovalRequest.Id && c.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).ToList();

            int ApprovedCount = trvlappStatusTrackers.Count;

            if (ApprovedCount > 0)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Travel Request cant be Deleted after Approval!" });
            }

            _context.TravelApprovalRequests.Remove(travelApprovalRequest);

            var travlapprStatusTrackers = _context.TravelApprovalStatusTrackers.Where(c => c.TravelApprovalRequestId == travelApprovalRequest.Id).ToList();

            foreach (var item in travlapprStatusTrackers)
            {
                _context.TravelApprovalStatusTrackers.Remove(item);
            }
            await _context.SaveChangesAsync();

            return Ok(new RespStatus { Status = "Success", Message = "Travel Approval Request Deleted!" });
        }



        /// <summary>
        /// This is the option 1 : : PROJECT BASED TRAVEL REQUEST
        /// </summary>
        /// <param name="travelApprovalRequestDto"></param>
        /// <param name="travelApprovalRequestDto"></param>
        private async Task<int> ProjectTravelRequest(TravelApprovalRequestDTO travelApprovalRequestDTO)
        {

            _logger.LogInformation("ProjectBasedTravelRequest Started");
            #region
            int costCenterId = _context.Projects.Find(travelApprovalRequestDTO.ProjectId).CostCenterId;
            int projManagerid = _context.Projects.Find(travelApprovalRequestDTO.ProjectId).ProjectManagerId;
            var approver = _context.Employees.Find(projManagerid);
            int reqEmpid = travelApprovalRequestDTO.EmployeeId;
            int maxApprLevel = _context.ApprovalRoleMaps.Max(a => a.ApprovalLevelId);
            int empApprLevel = _context.ApprovalRoleMaps.Where(a => a.RoleId == _context.Employees.Find(reqEmpid).RoleId).FirstOrDefault().Id;
            bool isSelfApprovedRequest = false;
            #endregion


            if (approver != null)
            {
                _logger.LogInformation("Project Manager defined, no issues");
            }
            else
            {
                _logger.LogError("Project Manager is not Assigned");
                return 1;
            }

            //### 1. If Employee Travel Request enter a record in TravelApprovalRequestTracker
            #region

            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                TravelApprovalRequest travelApprovalRequest = new();

                travelApprovalRequest.Id = travelApprovalRequestDTO.Id;
                travelApprovalRequest.EmployeeId = travelApprovalRequestDTO.EmployeeId;
                travelApprovalRequest.TravelStartDate = travelApprovalRequestDTO.TravelStartDate;
                travelApprovalRequest.TravelEndDate = travelApprovalRequestDTO.TravelEndDate;
                travelApprovalRequest.TravelPurpose = travelApprovalRequestDTO.TravelPurpose;
                travelApprovalRequest.ReqRaisedDate = DateTime.Now;
                travelApprovalRequest.DepartmentId = travelApprovalRequestDTO.DepartmentId;
                travelApprovalRequest.ProjectId = travelApprovalRequestDTO.ProjectId;
                travelApprovalRequest.SubProjectId = travelApprovalRequestDTO.SubProjectId;
                travelApprovalRequest.WorkTaskId = travelApprovalRequestDTO.WorkTaskId;
                travelApprovalRequest.CostCenterId = _context.Projects.Find(travelApprovalRequestDTO.ProjectId).CostCenterId;
                travelApprovalRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Pending;
                travelApprovalRequest.Comments = "Travel Request In Process!";


                _context.TravelApprovalRequests.Add(travelApprovalRequest);

                await _context.SaveChangesAsync();
                //get the saved record Id
                travelApprovalRequestDTO.Id = travelApprovalRequest.Id;
                #endregion

                //##### 3. Add an entry to ClaimApproval Status tracker
                //get costcenterID based on project
                #region

                ///////////////////////////// Check if self Approved Request /////////////////////////////

                //if highest approver is requesting Petty cash request himself
                if (maxApprLevel == empApprLevel || projManagerid == reqEmpid)
                {
                    isSelfApprovedRequest = true;
                }
                //////////////////////////////////////////////////////////////////////////////////////////
                TravelApprovalStatusTracker travelApprovalStatusTracker = new();
                if (isSelfApprovedRequest)
                {
                    travelApprovalStatusTracker.EmployeeId = travelApprovalRequestDTO.EmployeeId;
                    travelApprovalStatusTracker.TravelApprovalRequestId = travelApprovalRequestDTO.Id;
                    travelApprovalStatusTracker.TravelStartDate = travelApprovalRequestDTO.TravelStartDate;
                    travelApprovalStatusTracker.TravelEndDate = travelApprovalRequestDTO.TravelEndDate;
                    travelApprovalStatusTracker.DepartmentId = null;
                    travelApprovalStatusTracker.ProjManagerId = projManagerid;
                    travelApprovalStatusTracker.ProjectId = travelApprovalRequestDTO.ProjectId;
                    travelApprovalStatusTracker.RoleId = approver.RoleId;
                    travelApprovalStatusTracker.ApprovalGroupId = _context.Employees.Find(travelApprovalRequestDTO.EmployeeId).ApprovalGroupId;
                    travelApprovalStatusTracker.ApprovalLevelId = 2; // default approval level is 2 for Project based request
                    travelApprovalStatusTracker.ReqDate = DateTime.Now;
                    travelApprovalStatusTracker.FinalApprovedDate = DateTime.Now;
                    travelApprovalStatusTracker.Comments = "Travel Request is Self Approved!";
                    travelApprovalStatusTracker.ApprovalStatusTypeId = (int)EApprovalStatus.Approved; //status tracker


                    _context.TravelApprovalStatusTrackers.Add(travelApprovalStatusTracker);
                    travelApprovalRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;  //1-Initiating; 2-Pending; 3-InReview; 4-Approved; 5-Rejected
                    travelApprovalRequest.Comments = "Approved";
                    _context.TravelApprovalRequests.Update(travelApprovalRequest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    travelApprovalStatusTracker.EmployeeId = travelApprovalRequestDTO.EmployeeId;
                    travelApprovalStatusTracker.TravelApprovalRequestId = travelApprovalRequestDTO.Id;
                    travelApprovalStatusTracker.TravelStartDate = travelApprovalRequestDTO.TravelStartDate;
                    travelApprovalStatusTracker.TravelEndDate = travelApprovalRequestDTO.TravelEndDate;
                    travelApprovalStatusTracker.DepartmentId = null;
                    travelApprovalStatusTracker.ProjManagerId = projManagerid;
                    travelApprovalStatusTracker.ProjectId = travelApprovalRequestDTO.ProjectId;
                    travelApprovalStatusTracker.RoleId = approver.RoleId;
                    // get the next ProjectManager approval.
                    travelApprovalStatusTracker.ApprovalGroupId = _context.Employees.Find(travelApprovalRequestDTO.EmployeeId).ApprovalGroupId;
                    travelApprovalStatusTracker.ApprovalLevelId = 2; // default approval level is 2 for Project based request
                    travelApprovalStatusTracker.ReqDate = DateTime.Now;
                    travelApprovalStatusTracker.FinalApprovedDate = null;
                    travelApprovalStatusTracker.ApprovalStatusTypeId = (int)EApprovalStatus.Pending; //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                    travelApprovalStatusTracker.Comments = "Travel Request in Proceess";

                    _context.TravelApprovalStatusTrackers.Add(travelApprovalStatusTracker);
                }





                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                #endregion


                //##### 4. Send email to the user
                //####################################
                #region
                if (isSelfApprovedRequest)
                {
                    return 0;
                }

                string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "TravelApprNotificationEmail.html" };
                string FilePath = Path.Combine(paths);
                _logger.LogInformation("Email template path " + FilePath);
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();


                _logger.LogInformation(approver.GetFullName() + "Email Start");
                var approverMailAddress = approver.Email;
                string subject = "Travel Approval Request No# " + travelApprovalRequestDTO.Id.ToString();
                Employee emp = await _context.Employees.FindAsync(travelApprovalRequestDTO.EmployeeId);
                var travelreq = _context.TravelApprovalRequests.Find(travelApprovalRequestDTO.Id);

                var builder = new MimeKit.BodyBuilder();

                MailText = MailText.Replace("{Requester}", emp.GetFullName());
                MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                MailText = MailText.Replace("{Request}", travelreq.TravelStartDate.ToString() + " - " + travelreq.TravelEndDate.ToString() + " (Purpose): " + travelreq.TravelPurpose.ToString());
                MailText = MailText.Replace("{RequestNumber}", travelreq.Id.ToString());
                builder.HtmlBody = MailText;

                var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                await _emailSender.SendEmailAsync(messagemail);
                #endregion

                _logger.LogInformation(approver.GetFullName() + "Email Sent");

                await _context.SaveChangesAsync();

                await AtoCashDbContextTransaction.CommitAsync();
            }

            return 0;
        }

        /// <summary>
        /// This is option 2 : DEPARTMENT BASED CASH ADVANCE REQUEST
        /// </summary>
        /// <param name="travelApprovalRequestDto"></param>

        private async Task<int> DepartmentTravelRequest(TravelApprovalRequestDTO travelApprovalRequestDto)
        {
            //### 1. If Employee Eligible for Cash Claim enter a record and reduce the available amount for next claim
            #region
            _logger.LogInformation("Department based Travel Request Started");

            int reqEmpid = travelApprovalRequestDto.EmployeeId;
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

            _logger.LogInformation("All Approvers defined");

            var approRolMapsList = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId).ToList();
            int maxApprLevel = approRolMapsList.Select(x => x.ApprovalLevel).Max(a => a.Level);
            int reqApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId && a.RoleId == reqRoleId).Select(x => x.ApprovalLevel).FirstOrDefault().Level;


            bool isSelfApprovedRequest = false;

            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                var travelApprovalRequest = new TravelApprovalRequest()

                {
                    EmployeeId = reqEmpid,
                    TravelStartDate = travelApprovalRequestDto.TravelStartDate,
                    TravelEndDate = travelApprovalRequestDto.TravelEndDate,
                    TravelPurpose = travelApprovalRequestDto.TravelPurpose,
                    ReqRaisedDate = DateTime.Now,
                    DepartmentId = _context.Employees.Find(reqEmpid).DepartmentId,
                    ProjectId = travelApprovalRequestDto.ProjectId,
                    SubProjectId = travelApprovalRequestDto.SubProjectId,
                    WorkTaskId = travelApprovalRequestDto.WorkTaskId,
                    CostCenterId = _context.Departments.Find(reqEmp.DepartmentId).CostCenterId,
                    ApprovalStatusTypeId = (int)EApprovalStatus.Pending,
                    Comments = "Travel Request In Process!"


                };
                _context.TravelApprovalRequests.Add(travelApprovalRequest);
                await _context.SaveChangesAsync();

                //get the saved record Id
                travelApprovalRequestDto.Id = travelApprovalRequest.Id;

                #endregion


                #region
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
                    TravelApprovalStatusTracker travelApprovalStatusTracker = new()
                    {
                        EmployeeId = travelApprovalRequestDto.EmployeeId,
                        TravelApprovalRequestId = travelApprovalRequestDto.Id,
                        TravelStartDate = travelApprovalRequestDto.TravelStartDate,
                        TravelEndDate = travelApprovalRequestDto.TravelEndDate,
                        DepartmentId = reqEmp.DepartmentId,
                        ProjectId = null,
                        RoleId = reqEmp.RoleId,
                        ApprovalLevelId = reqApprLevel,
                        ApprovalGroupId = reqApprGroupId,
                        ReqDate = DateTime.Now,
                        FinalApprovedDate = DateTime.Now,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Approved,
                        Comments = "Travel Request in Proceess"
                    };

                    _context.TravelApprovalStatusTrackers.Add(travelApprovalStatusTracker);
                    travelApprovalRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                    travelApprovalRequest.Comments = "Approved";
                    _context.TravelApprovalRequests.Update(travelApprovalRequest);
                    await _context.SaveChangesAsync();
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


                        TravelApprovalStatusTracker travelApprovalStatusTracker = new()
                        {
                            EmployeeId = travelApprovalRequestDto.EmployeeId,
                            TravelApprovalRequestId = travelApprovalRequestDto.Id,
                            TravelStartDate = travelApprovalRequestDto.TravelStartDate,
                            TravelEndDate = travelApprovalRequestDto.TravelEndDate,
                            DepartmentId = reqEmp.DepartmentId,
                            ProjectId = null,
                            RoleId = approver.RoleId,
                            ApprovalLevelId = ApprMap.ApprovalLevelId,
                            ApprovalGroupId = reqApprGroupId,
                            ReqDate = DateTime.Now,
                            FinalApprovedDate = null,
                            ApprovalStatusTypeId = isFirstApprover ? (int)EApprovalStatus.Pending : (int)EApprovalStatus.Initiating,
                            Comments = "Travel Request in Proceess"
                            //1-Initiating, 2-Pending, 3-InReview, 4-Approved, 5-Rejected
                        };


                        _context.TravelApprovalStatusTrackers.Add(travelApprovalStatusTracker);
                        await _context.SaveChangesAsync();


                        if (isFirstApprover)
                        {
                            //##### 4. Send email to the Approver
                            //####################################
                            _logger.LogInformation(approver.GetFullName() + "Email Start");
                            string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "TravelApprNotificationEmail.html" };
                            string FilePath = Path.Combine(paths);
                            _logger.LogInformation("Email template path " + FilePath);
                            StreamReader str = new StreamReader(FilePath);
                            string MailText = str.ReadToEnd();
                            str.Close();

                            var approverMailAddress = approver.Email;
                            string subject = "Travel Approval Request No# " + travelApprovalRequestDto.Id.ToString();
                            Employee emp = await _context.Employees.FindAsync(travelApprovalRequestDto.EmployeeId);
                            var travelreq = _context.TravelApprovalRequests.Find(travelApprovalRequestDto.Id);

                            var builder = new MimeKit.BodyBuilder();

                            MailText = MailText.Replace("{Requester}", emp.GetFullName());
                            MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                            MailText = MailText.Replace("{Request}", travelreq.TravelStartDate.ToString() + " - " + travelreq.TravelEndDate.ToString() + " (Purpose): " + travelreq.TravelPurpose.ToString());
                            MailText = MailText.Replace("{RequestNumber}", travelreq.Id.ToString());
                            builder.HtmlBody = MailText;

                            var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                            await _emailSender.SendEmailAsync(messagemail);
                            _logger.LogInformation(approver.GetFullName() + "Email Sent");
                        }

                        //first approver will be added as Pending, other approvers will be with In Approval Queue
                        isFirstApprover = false;

                    }
                }


                #endregion


                await _context.SaveChangesAsync();

                await AtoCashDbContextTransaction.CommitAsync();
            }

            _logger.LogInformation("Travel Request Created successfully");
            return 0;
        }




    }
}
