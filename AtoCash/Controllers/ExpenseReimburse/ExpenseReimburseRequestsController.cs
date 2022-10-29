using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtoCash.Data;
using AtoCash.Models;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using EmailService;
using Microsoft.AspNetCore.Authorization;
using AtoCash.Authentication;
using System.Net.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace AtoCash.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr, User")]
    public class ExpenseReimburseRequestsController : ControllerBase
    {
        private readonly AtoCashDbContext _context;
        //private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExpenseReimburseRequestsController> _logger;

        public ExpenseReimburseRequestsController(AtoCashDbContext context,
                                                IWebHostEnvironment hostEnv,
                                                IEmailSender emailSender,
                                                ILogger<ExpenseReimburseRequestsController> logger)
        {
            _context = context;
            hostingEnvironment = hostEnv;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET: api/ExpenseReimburseRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseReimburseRequestDTO>>> GetExpenseReimburseRequests()
        {

            var expenseReimburseRequests = await _context.ExpenseReimburseRequests.ToListAsync();


            List<ExpenseReimburseRequestDTO> ListExpenseReimburseRequestDTO = new();
            foreach (ExpenseReimburseRequest expenseReimbRequest in expenseReimburseRequests)
            {

                var disbAndClaim = _context.DisbursementsAndClaimsMasters.Where(d => d.ExpenseReimburseReqId == expenseReimbRequest.Id).FirstOrDefault();

                if (disbAndClaim == null)
                {
                    _logger.LogError("Disbursement table is empty for " + expenseReimbRequest.Id);
                }

                try
                {
                    ExpenseReimburseRequestDTO expenseReimburseRequestDTO = new()
                    {
                        Id = expenseReimbRequest.Id,
                        EmployeeId = expenseReimbRequest.EmployeeId,
                        EmployeeName = _context.Employees.Find(expenseReimbRequest.EmployeeId).GetFullName(),
                        ExpenseReportTitle = expenseReimbRequest.ExpenseReportTitle,
                        CurrencyTypeId = expenseReimbRequest.CurrencyTypeId,
                        TotalClaimAmount = expenseReimbRequest.TotalClaimAmount,


                        DepartmentId = expenseReimbRequest.IsBusinessAreaReq == false ? expenseReimbRequest.DepartmentId : null,
                        DepartmentName = expenseReimbRequest.IsBusinessAreaReq == false ? expenseReimbRequest.DepartmentId != null ? _context.Departments.Find(expenseReimbRequest.DepartmentId).DeptName : null : null,

                        BusinessAreaId = expenseReimbRequest.BusinessAreaId,
                        BusinessArea = expenseReimbRequest.BusinessAreaId != null ? _context.BusinessAreas.Find(expenseReimbRequest.BusinessAreaId).BusinessAreaCode + ":" + _context.BusinessAreas.Find(expenseReimbRequest.BusinessAreaId).BusinessAreaName : null,


                        ProjectId = expenseReimbRequest.ProjectId,
                        ProjectName = expenseReimbRequest.ProjectId != null ? _context.Projects.Find(expenseReimbRequest.ProjectId).ProjectName : null,

                        SubProjectId = expenseReimbRequest.SubProjectId,
                        SubProjectName = expenseReimbRequest.SubProjectId != null ? _context.SubProjects.Find(expenseReimbRequest.SubProjectId).SubProjectName : null,

                        WorkTaskId = expenseReimbRequest.WorkTaskId,
                        WorkTaskName = expenseReimbRequest.WorkTaskId != null ? _context.WorkTasks.Find(expenseReimbRequest.WorkTaskId).TaskName : null,

                        ExpReimReqDate = expenseReimbRequest.ExpReimReqDate,
                        ApprovedDate = expenseReimbRequest.ApprovedDate,
                        ApprovalStatusTypeId = expenseReimbRequest.ApprovalStatusTypeId,
                        ApprovalStatusType = _context.ApprovalStatusTypes.Find(expenseReimbRequest.ApprovalStatusTypeId).Status,


                        CreditToBank = disbAndClaim.IsSettledAmountCredited ?? false ? disbAndClaim.AmountToCredit : 0,
                        CreditToWallet = disbAndClaim.IsSettledAmountCredited ?? false ? disbAndClaim.AmountToWallet : 0,
                        IsSettled = !(disbAndClaim.IsSettledAmountCredited ?? false)
                    };
                    ListExpenseReimburseRequestDTO.Add(expenseReimburseRequestDTO);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetExpenseReimburseRequests DTO Error expenseReimbRequest.Id " + expenseReimbRequest.Id);
                }


            }

            return ListExpenseReimburseRequestDTO.OrderByDescending(o => o.ExpReimReqDate).ToList();
        }

        //GET: api/ExpenseReimburseRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseReimburseRequestDTO>> GetExpenseReimburseRequest(int id)
        {


            var expenseReimbRequest = await _context.ExpenseReimburseRequests.FindAsync(id);

            if (expenseReimbRequest == null)
            {
                _logger.LogError("Expense Reimburse request is null for GetExpenseReimburseRequest " + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Reimburse Id invalid!" });
            }


            double tmpAmountToCredit = 0;
            double tmpAmountToWallet = 0;
            bool tmpIsSettledAmountCredited = false;

            var disbAndClaim = _context.DisbursementsAndClaimsMasters.Where(d => d.ExpenseReimburseReqId == id).FirstOrDefault();



            if (disbAndClaim != null)
            {
                tmpAmountToCredit = disbAndClaim.AmountToCredit ?? 0;
                tmpAmountToWallet = disbAndClaim.AmountToWallet ?? 0;
                tmpIsSettledAmountCredited = (bool)disbAndClaim.IsSettledAmountCredited;

            }


            ExpenseReimburseRequestDTO expenseReimburseRequestDTO = new();


            try
            {


                expenseReimburseRequestDTO.Id = expenseReimbRequest.Id;
                expenseReimburseRequestDTO.EmployeeId = expenseReimbRequest.EmployeeId;
                expenseReimburseRequestDTO.EmployeeName = _context.Employees.Find(expenseReimbRequest.EmployeeId).GetFullName();
                expenseReimburseRequestDTO.ExpenseReportTitle = expenseReimbRequest.ExpenseReportTitle;
                expenseReimburseRequestDTO.CurrencyTypeId = expenseReimbRequest.CurrencyTypeId;
                expenseReimburseRequestDTO.TotalClaimAmount = expenseReimbRequest.TotalClaimAmount;

                expenseReimburseRequestDTO.DepartmentId = expenseReimbRequest.IsBusinessAreaReq == false ? expenseReimbRequest.DepartmentId : null;
                expenseReimburseRequestDTO.DepartmentName = expenseReimbRequest.IsBusinessAreaReq == false ? expenseReimbRequest.DepartmentId != null ? _context.Departments.Find(expenseReimbRequest.DepartmentId).DeptName : null : null;
                expenseReimburseRequestDTO.IsBusinessAreaReq = expenseReimbRequest.IsBusinessAreaReq;

                expenseReimburseRequestDTO.BusinessAreaId = expenseReimbRequest.BusinessAreaId;
                expenseReimburseRequestDTO.BusinessArea = expenseReimbRequest.BusinessAreaId != null ? _context.BusinessAreas.Find(expenseReimbRequest.BusinessAreaId).BusinessAreaCode + ":" + _context.BusinessAreas.Find(expenseReimbRequest.BusinessAreaId).BusinessAreaName : null;

                expenseReimburseRequestDTO.ProjectId = expenseReimbRequest.ProjectId;
                expenseReimburseRequestDTO.ProjectName = expenseReimbRequest.ProjectId != null ? _context.Projects.Find(expenseReimbRequest.ProjectId).ProjectName : null;

                expenseReimburseRequestDTO.SubProjectId = expenseReimbRequest.SubProjectId;
                expenseReimburseRequestDTO.SubProjectName = expenseReimbRequest.SubProjectId != null ? _context.SubProjects.Find(expenseReimbRequest.SubProjectId).SubProjectName : null;

                expenseReimburseRequestDTO.WorkTaskId = expenseReimbRequest.WorkTaskId;
                expenseReimburseRequestDTO.WorkTaskName = expenseReimbRequest.WorkTaskId != null ? _context.WorkTasks.Find(expenseReimbRequest.WorkTaskId).TaskName : null;

                expenseReimburseRequestDTO.ExpReimReqDate = expenseReimbRequest.ExpReimReqDate;
                expenseReimburseRequestDTO.ApprovedDate = expenseReimbRequest.ApprovedDate;
                expenseReimburseRequestDTO.ApprovalStatusTypeId = expenseReimbRequest.ApprovalStatusTypeId;
                expenseReimburseRequestDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(expenseReimbRequest.ApprovalStatusTypeId).Status;

                expenseReimburseRequestDTO.Comments = expenseReimbRequest.Comments;


                expenseReimburseRequestDTO.CreditToBank = expenseReimbRequest.ApprovalStatusTypeId == (int)EApprovalStatus.Approved ? tmpAmountToCredit : 0;
                expenseReimburseRequestDTO.CreditToWallet = expenseReimbRequest.ApprovalStatusTypeId == (int)EApprovalStatus.Approved ? tmpAmountToWallet : 0;
                expenseReimburseRequestDTO.IsSettled = disbAndClaim.IsSettledAmountCredited ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetExpenseReimburseRequest failed for " + id);
            }


            return expenseReimburseRequestDTO;

        }



        [HttpGet("{id}")]
        [ActionName("GetExpenseReimburseRequestRaisedForEmployee")]
        public async Task<ActionResult<IEnumerable<ExpenseReimburseRequestDTO>>> GetExpenseReimburseRequestRaisedForEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                _logger.LogError("Employee invalid Id:" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee Id invalid!" });
            }

            //get the employee's approval level for comparison with approver level  to decide "ShowEditDelete" bool

            int empRoleId = employee.RoleId;
            int empBARoleId = (int)employee.BusinessAreaRoleId;
            var approvalRoleMap = _context.ApprovalRoleMaps.Where(a => a.RoleId == empRoleId || a.RoleId == empBARoleId).FirstOrDefault();
            int reqEmpApprLevelId = 0;
            try
            {
                reqEmpApprLevelId = approvalRoleMap.ApprovalLevelId;
               
            }
            catch( Exception ex)
            {
                _logger.LogError("Employee reqEmpApprLevelId is null for Employee id: " + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee Approval Level not defined!" });
            }

            int reqEmpApprLevel = _context.ApprovalLevels.Find(reqEmpApprLevelId).Level;

            var expenseReimbRequests = await _context.ExpenseReimburseRequests.Where(p => p.EmployeeId == id).ToListAsync();

            if (expenseReimbRequests == null)
            {
                _logger.LogError("expenseReimbRequests  is null for Employee Id" + id);
                return Conflict(new RespStatus { Status = "Failure", Message = "Expense Reimburse Id invalid!" });
            }

            List<ExpenseReimburseRequestDTO> ListExpenseReimburseRequestDTO = new();
            await Task.Run(() =>
            {
                try
                {
                    foreach (ExpenseReimburseRequest expenseReimbRequest in expenseReimbRequests)
                    {
                        ExpenseReimburseRequestDTO expenseReimburseRequestDTO = new();

                        expenseReimburseRequestDTO.Id = expenseReimbRequest.Id;
                        expenseReimburseRequestDTO.EmployeeId = expenseReimbRequest.EmployeeId;
                        expenseReimburseRequestDTO.EmployeeName = _context.Employees.Find(expenseReimbRequest.EmployeeId).GetFullName();
                        expenseReimburseRequestDTO.ExpenseReportTitle = expenseReimbRequest.ExpenseReportTitle;
                        expenseReimburseRequestDTO.CurrencyTypeId = expenseReimbRequest.CurrencyTypeId;
                        expenseReimburseRequestDTO.TotalClaimAmount = expenseReimbRequest.TotalClaimAmount;
                        if (expenseReimbRequest.IsBusinessAreaReq == false)
                        {
                            expenseReimburseRequestDTO.DepartmentId = expenseReimbRequest.DepartmentId;
                            expenseReimburseRequestDTO.DepartmentName = expenseReimbRequest.DepartmentId != null ? _context.Departments.Find(expenseReimbRequest.DepartmentId).DeptName : null;
                        }
                        expenseReimburseRequestDTO.BusinessAreaId = expenseReimbRequest.BusinessAreaId;
                        expenseReimburseRequestDTO.BusinessArea = expenseReimbRequest.BusinessAreaId != null ? _context.BusinessAreas.Find(expenseReimbRequest.BusinessAreaId).BusinessAreaName : null;


                        expenseReimburseRequestDTO.ProjectId = expenseReimbRequest.ProjectId;
                        expenseReimburseRequestDTO.ProjectName = expenseReimbRequest.ProjectId != null ? _context.Projects.Find(expenseReimbRequest.ProjectId).ProjectName : null;

                        expenseReimburseRequestDTO.SubProjectId = expenseReimbRequest.SubProjectId;
                        expenseReimburseRequestDTO.SubProjectName = expenseReimbRequest.SubProjectId != null ? _context.SubProjects.Find(expenseReimbRequest.SubProjectId).SubProjectName : null;

                        expenseReimburseRequestDTO.WorkTaskId = expenseReimbRequest.WorkTaskId;
                        expenseReimburseRequestDTO.WorkTaskName = expenseReimbRequest.WorkTaskId != null ? _context.WorkTasks.Find(expenseReimbRequest.WorkTaskId).TaskName : null;

                        expenseReimburseRequestDTO.ExpReimReqDate = expenseReimbRequest.ExpReimReqDate;
                        expenseReimburseRequestDTO.ApprovedDate = expenseReimbRequest.ApprovedDate;
                        expenseReimburseRequestDTO.ApprovalStatusTypeId = expenseReimbRequest.ApprovalStatusTypeId;
                        expenseReimburseRequestDTO.ApprovalStatusType = _context.ApprovalStatusTypes.Find(expenseReimbRequest.ApprovalStatusTypeId).Status;


                        int NextApproverInPending = _context.ExpenseReimburseStatusTrackers.Where(t =>
                             t.ApprovalStatusTypeId == (int)EApprovalStatus.Pending &&
                             t.ExpenseReimburseRequestId == expenseReimbRequest.Id).Select(s => s.ApprovalLevel.Level).FirstOrDefault();

                       //set the bookean flat to TRUE if No approver has yet approved the Request else FALSE
                       //expenseReimburseRequestDTO.ShowEditDelete = reqEmpApprLevel + 1 == NextApproverInPending ? true : false;
                       expenseReimburseRequestDTO.ShowEditDelete = (reqEmpApprLevel + 1 == NextApproverInPending) && false;

                        ListExpenseReimburseRequestDTO.Add(expenseReimburseRequestDTO);

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetExpenseReimburseRequestRaisedForEmployee Employee Id" + id);
                }

            });

            return Ok(ListExpenseReimburseRequestDTO.OrderByDescending(o => o.ExpReimReqDate).ToList());
        }



        [HttpGet("{id}")]
        [ActionName("CountAllBusinessAreaExpenseReimburseRequestRaisedByEmployee")]
        public async Task<ActionResult> CountAllBusinessAreaExpenseReimburseRequestRaisedByEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                _logger.LogError("Employee invalid Id:" + id);
                return Ok(0);
            }

            var expenseReimburseRequests = await _context.ExpenseReimburseRequests.Where(p => p.EmployeeId == id && p.IsBusinessAreaReq == true).ToListAsync();

            if (expenseReimburseRequests == null)
            {
                _logger.LogInformation("BusinessAreaExpenseReimburseRequests is null with Business Area request for Employeed Id" + id);
                return Ok(0);
            }

            int TotalCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == true).Count();
            int PendingCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == true && c.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).Count();
            int RejectedCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == true && c.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected).Count();
            int ApprovedCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == true && c.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).Count();

            return Ok(new { TotalCount, PendingCount, RejectedCount, ApprovedCount });
        }


        [HttpGet("{id}")]
        [ActionName("CountAllExpenseReimburseRequestRaisedByEmployee")]
        public async Task<ActionResult> CountAllExpenseReimburseRequestRaisedByEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                _logger.LogError("Employee invalid Id:" + id);
                return Ok(0);
            }

            var expenseReimburseRequests = await _context.ExpenseReimburseRequests.Where(p => p.EmployeeId == id && p.IsBusinessAreaReq == false).ToListAsync();

            if (expenseReimburseRequests == null)
            {
                _logger.LogInformation("BusinessAreaExpenseReimburseRequests is null with Business Area request for Employeed Id" + id);
                return Ok(0);
            }

            int TotalCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == false).Count();
            int PendingCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == false && c.ApprovalStatusTypeId == (int)EApprovalStatus.Pending).Count();
            int RejectedCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == false && c.ApprovalStatusTypeId == (int)EApprovalStatus.Rejected).Count();
            int ApprovedCount = _context.ExpenseReimburseRequests.Where(c => c.EmployeeId == id && c.IsBusinessAreaReq == false && c.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).Count();

            return Ok(new { TotalCount, PendingCount, RejectedCount, ApprovedCount });
        }


        // PUT: api/ExpenseReimburseRequests/5
        [HttpPut]
        //[Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> PutExpenseReimburseRequest(int id, ExpenseReimburseRequestDTO expenseReimbRequestDTO)
        {
            if (id != expenseReimbRequestDTO.Id)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Id is invalid" });
            }


            try
            {
                var expenseReimbRequest = await _context.ExpenseReimburseRequests.FindAsync(expenseReimbRequestDTO.Id);

                expenseReimbRequest.Id = expenseReimbRequestDTO.Id;
                expenseReimbRequest.EmployeeId = expenseReimbRequestDTO.EmployeeId;
                expenseReimbRequest.ExpenseReportTitle = expenseReimbRequestDTO.ExpenseReportTitle;
                expenseReimbRequest.CurrencyTypeId = expenseReimbRequestDTO.CurrencyTypeId;
                expenseReimbRequest.TotalClaimAmount = expenseReimbRequestDTO.TotalClaimAmount;

                expenseReimbRequest.DepartmentId = expenseReimbRequestDTO.DepartmentId;
                expenseReimbRequest.ProjectId = expenseReimbRequestDTO.ProjectId;

                expenseReimbRequest.SubProjectId = expenseReimbRequestDTO.SubProjectId;

                expenseReimbRequest.WorkTaskId = expenseReimbRequestDTO.WorkTaskId;

                expenseReimbRequest.ExpReimReqDate = expenseReimbRequestDTO.ExpReimReqDate;
                expenseReimbRequest.ApprovedDate = expenseReimbRequestDTO.ApprovedDate;
                expenseReimbRequest.ApprovalStatusTypeId = expenseReimbRequestDTO.ApprovalStatusTypeId;

                await Task.Run(() => _context.ExpenseReimburseRequests.Update(expenseReimbRequest));


                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "DbUpdateConcurrencyException PutExpenseReimburseRequest " + id);
            }

            return Ok(new RespStatus { Status = "Success", Message = "Expense Reimburse Data Updated!" });
        }

        // POST: api/ExpenseReimburseRequests

        [HttpPost]
        [ActionName("PostDocuments")]
        public async Task<ActionResult<List<FileDocumentDTO>>> PostFiles([FromForm] IFormFileCollection Documents)
        {
            //StringBuilder StrBuilderUploadedDocuments = new();

            List<FileDocumentDTO> fileDocumentDTOs = new();

         
            foreach (IFormFile document in Documents)
            {
                //Store the file to the contentrootpath/images =>
                //for docker it is /app/Images configured with volume mount in docker-compose

                string uploadsFolder = Path.Combine(hostingEnvironment.ContentRootPath, "Images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + document.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);


                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await document.CopyToAsync(stream);
                    stream.Flush();


                    // Save it to the acutal FileDocuments table
                    FileDocument fileDocument = new();
                    fileDocument.ActualFileName = document.FileName;
                    fileDocument.UniqueFileName = uniqueFileName;
                    _context.FileDocuments.Add(fileDocument);
                    await _context.SaveChangesAsync();
                    //

                    // Populating the List of Document Id for FrontEnd consumption
                    FileDocumentDTO fileDocumentDTO = new();
                    fileDocumentDTO.Id = fileDocument.Id;
                    fileDocumentDTO.ActualFileName = document.FileName;
                    fileDocumentDTOs.Add(fileDocumentDTO);

                    //StrBuilderUploadedDocuments.Append(uniqueFileName + "^");
                    //
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Post IformFile documents");
                    return Conflict(new RespStatus { Status = "Failure", Message = "File not uploaded.. Please retry!" + ex.ToString() });

                }




            }

            return Ok(fileDocumentDTOs);
        }

        //############################################################################################################
        /// <summary>
        /// Dont delete the below code code
        /// </summary>
        //############################################################################################################

        ///
        //[HttpGet("{id}")]
        //[ActionName("GetDocumentsBySubClaimsId")]
        ////<List<FileContentResult>
        //public async Task<ActionResult> GetDocumentsBySubClaimsId(int id)
        //{
        //    List<string> documentIds = _context.ExpenseSubClaims.Find(id).DocumentIDs.Split(",").ToList();
        //    string documentsFolder = Path.Combine(hostingEnvironment.ContentRootPath, "Images");
        //    //var content = new MultipartContent();

        //    List<FileContentResult> ListOfDocuments = new();
        //    var provider = new FileExtensionContentTypeProvider();

        //    foreach (string doc in documentIds)
        //    {
        //        var fd = _context.FileDocuments.Find(id);
        //        string uniqueFileName = fd.UniqueFileName;
        //        string actualFileName = fd.ActualFileName;

        //        string filePath = Path.Combine(documentsFolder, uniqueFileName);
        //        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        //        if (!provider.TryGetContentType(filePath, out var contentType))
        //        {
        //            contentType = "application/octet-stream";
        //        }

        //        FileContentResult thisfile = File(bytes, contentType, Path.GetFileName(filePath));

        //        ListOfDocuments.Add(thisfile);
        //    }
        //    return Ok(ListOfDocuments);
        //}
        //############################################################################################################

        [HttpGet("{id}")]
        [ActionName("GetDocumentsBySubClaimsId")]
        //<List<FileContentResult>
        public async Task<ActionResult> GetDocumentsBySubClaimsId(int id)
        {
            List<int> documentIds = _context.ExpenseSubClaims.Find(id).DocumentIDs.Split(",").Select(Int32.Parse).ToList();
            string documentsFolder = Path.Combine(hostingEnvironment.ContentRootPath, "Images");

            List<string> docUrls = new();

            var provider = new FileExtensionContentTypeProvider();
            await Task.Run(() =>
            {
                foreach (int docid in documentIds)
                {
                    var fd = _context.FileDocuments.Find(docid);
                    string uniqueFileName = fd.UniqueFileName;
                    string actualFileName = fd.ActualFileName;

                    string filePath = Path.Combine(documentsFolder, uniqueFileName);

                    string docUrl = Directory.EnumerateFiles(documentsFolder).Select(f => filePath).FirstOrDefault().ToString();
                    docUrls.Add(docUrl);


                }
            });
            _logger.LogInformation("GetDocumentsBySubClaimsId - retrieved documents successfully");
            return Ok(docUrls);
        }


        [HttpGet("{id}")]
        [ActionName("GetDocumentByDocId")]
        public async Task<ActionResult> GetDocumentByDocId(int id)
        {
            string documentsFolder = Path.Combine(hostingEnvironment.ContentRootPath, "Images");
            //var content = new MultipartContent();

            var provider = new FileExtensionContentTypeProvider();

            var fd = _context.FileDocuments.Find(id);
            string uniqueFileName = fd.UniqueFileName;
            //string actualFileName = fd.ActualFileName;

            string filePath = Path.Combine(documentsFolder, uniqueFileName);
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            //FileContentResult thisfile = File(bytes, contentType, Path.GetFileName(filePath));

            _logger.LogInformation("GetDocumentByDocId - returned document to caller");

            return File(bytes, contentType, Path.GetFileName(filePath));
        }



        [HttpPost]
        public async Task<ActionResult> PostExpenseReimburseRequest(ExpenseReimburseRequestDTO expenseReimburseRequestDto)
        {
            int SuccessResult;

            if (expenseReimburseRequestDto == null)
            {
                _logger.LogError("PostExpenseReimburseRequest - null request data");
                return Conflict(new RespStatus { Status = "Failure", Message = "expenseReimburseRequest Id invalid!" });
            }

            if (expenseReimburseRequestDto.ProjectId != null)
            {
                //Goes to Option 1 (Project)
                SuccessResult =   await Task.Run(() => ProjectBasedExpReimRequest(expenseReimburseRequestDto));
            }
            else if (expenseReimburseRequestDto.IsBusinessAreaReq == true)
            {
                //Goes to Option 2 (BusinessArea)
                SuccessResult = await Task.Run(() => BusinessAreaBasedExpReimRequest(expenseReimburseRequestDto));
            }
            else
            {
                //Goes to Option 3 (Department)
                SuccessResult = await Task.Run(() => DepartmentBasedExpReimRequest(expenseReimburseRequestDto));
            }

            if(SuccessResult == 0)
            {
                _logger.LogInformation("PostExpenseReimburseRequest - Process completed");

                return Ok(new RespStatus { Status = "Success", Message = "Expense Reimburse Request Created!" });
            }
            else
            {
                _logger.LogError("Expense Reimburse Request creation failed!");

                return BadRequest(new RespStatus { Status = "Failure", Message = "Expense Reimburse Request creation failed!" });
            }
           
        }


        // DELETE: api/ExpenseReimburseRequests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> DeleteExpenseReimburseRequest(int id)
        {
            var expenseReimburseRequest = await _context.ExpenseReimburseRequests.FindAsync(id);
            if (expenseReimburseRequest == null)
            {
                _logger.LogError("expenseReimburseRequest - request data null");

                return Conflict(new RespStatus { Status = "Failure", Message = "expense Reimburse Request Id Invalid!" });
            }

            int ApprovedCount = _context.ExpenseReimburseStatusTrackers.Where(e => e.ExpenseReimburseRequestId == expenseReimburseRequest.Id && e.ApprovalStatusTypeId == (int)EApprovalStatus.Approved).Count();

            if (ApprovedCount != 0)
            {
                _logger.LogInformation("expenseReimburseRequest - Reimburse Request cant be Deleted after Approval");
                return Conflict(new RespStatus { Status = "Failure", Message = "Reimburse Request cant be Deleted after Approval!" });
            }


            _context.ExpenseReimburseRequests.Remove(expenseReimburseRequest);

            await _context.SaveChangesAsync();
            _logger.LogInformation("expenseReimburseRequest - Deleted");
            return Ok(new RespStatus { Status = "Success", Message = "Expense Reimburse Request Deleted!" });
        }



        /// <summary>
        /// Department based Expreimburse request
        /// </summary>
        /// <param name="expenseReimburseRequestDto"></param>
        /// <returns></returns>

        private async Task<int> DepartmentBasedExpReimRequest(ExpenseReimburseRequestDTO expenseReimburseRequestDto)
        {
            _logger.LogInformation("Dept Area Request Started");
            #region
            int reqEmpid = expenseReimburseRequestDto.EmployeeId;
            Employee reqEmp = _context.Employees.Find(reqEmpid);
            int reqApprGroupId = (int)reqEmp.ApprovalGroupId;
            int reqRoleId = reqEmp.RoleId;
            int costCenterId = _context.Departments.Find(reqEmp.DepartmentId).CostCenterId;


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
            ////
            ///



            ExpenseReimburseRequest expenseReimburseRequest = new();
            double dblTotalClaimAmount = 0;

            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                _logger.LogInformation("Exp Reimb Table insert started");
                //assign values
                expenseReimburseRequest.ExpenseReportTitle = expenseReimburseRequestDto.ExpenseReportTitle;
                expenseReimburseRequest.BusinessAreaId = expenseReimburseRequestDto.BusinessAreaId;
                expenseReimburseRequest.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                expenseReimburseRequest.CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId;
                expenseReimburseRequest.TotalClaimAmount = dblTotalClaimAmount; //Currently Zero but added as per the request
                expenseReimburseRequest.ExpReimReqDate = DateTime.Now;
                expenseReimburseRequest.IsBusinessAreaReq = false;
                expenseReimburseRequest.DepartmentId = reqEmp.DepartmentId;
                expenseReimburseRequest.CostCenterId = costCenterId;
                expenseReimburseRequest.ProjectId = null;
                expenseReimburseRequest.SubProjectId = null;
                expenseReimburseRequest.WorkTaskId = null;
                expenseReimburseRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Pending;
                //expenseReimburseRequest.ApprovedDate = expenseReimburseRequestDto.ApprovedDate;
                expenseReimburseRequest.Comments = "Expense Reimburse Request in Process!";

                _context.ExpenseReimburseRequests.Add(expenseReimburseRequest); //  <= this generated the Id
                await _context.SaveChangesAsync();
                _logger.LogInformation("Exp Reimb Table inserted successfully");

                //assign values
                _logger.LogInformation("Sub Claims section started");
              
                foreach (ExpenseSubClaimDTO expenseSubClaimDto in expenseReimburseRequestDto.ExpenseSubClaims)
                {
                    ExpenseSubClaim expenseSubClaim = new();
                  

                    //get expensereimburserequestId from the saved record and then use here for sub-claims
                    expenseSubClaim.ExpenseReimburseRequestId = expenseReimburseRequest.Id;
                    expenseSubClaim.ExpenseTypeId = expenseSubClaimDto.ExpenseTypeId;
                    expenseSubClaim.EmployeeId = reqEmpid;
                    expenseSubClaim.IsVAT = expenseSubClaimDto.IsVAT;
                    expenseSubClaim.ExpenseReimbClaimAmount = expenseSubClaimDto.ExpenseReimbClaimAmount;
                    expenseSubClaim.DocumentIDs = expenseSubClaimDto.DocumentIDs;
                    expenseSubClaim.InvoiceNo = expenseSubClaimDto.InvoiceNo;
                    expenseSubClaim.InvoiceDate = expenseSubClaimDto.InvoiceDate;
                    expenseSubClaim.IsBusinessAreaReq = false;
                    expenseSubClaim.ExpenseCategoryId = expenseSubClaimDto.ExpenseCategoryId;
                    expenseSubClaim.TaxNo = expenseSubClaimDto.TaxNo;
                    expenseSubClaim.ExpStrtDate = expenseSubClaimDto.ExpStrtDate;
                    expenseSubClaim.ExpEndDate = expenseSubClaimDto.ExpEndDate;
                    expenseSubClaim.ExpNoOfDays = expenseSubClaimDto.ExpNoOfDays;

                    expenseSubClaim.Tax = expenseSubClaimDto.Tax;
                    expenseSubClaim.TaxAmount = expenseSubClaimDto.TaxAmount;
                    expenseSubClaim.Vendor = expenseSubClaimDto.Vendor;
                    expenseSubClaim.Location = expenseSubClaimDto.Location;
                    expenseSubClaim.DepartmentId = reqEmp.DepartmentId;
                    expenseSubClaim.ProjectId = null;
                    expenseSubClaim.SubProjectId = null;
                    expenseSubClaim.WorkTaskId = null;
                    expenseSubClaim.CostCenterId = costCenterId;
                    expenseSubClaim.Description = expenseSubClaimDto.Description;

                    _context.ExpenseSubClaims.Add(expenseSubClaim);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sub Claims Table insert failed");
                        return 1;
                    }

                    dblTotalClaimAmount = dblTotalClaimAmount + expenseSubClaimDto.TaxAmount + expenseSubClaimDto.ExpenseReimbClaimAmount;

                }

                ExpenseReimburseRequest exp = _context.ExpenseReimburseRequests.Find(expenseReimburseRequest.Id);

                exp.TotalClaimAmount = dblTotalClaimAmount;
                _context.ExpenseReimburseRequests.Update(exp);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sub Claims Table records inserted");

                ///////////////////////////// Check if self Approved Request /////////////////////////////

                //if highest approver is requesting Petty cash request himself
                if (maxApprLevel == reqApprLevel)
                {
                    isSelfApprovedRequest = true;
                }
                //////////////////////////////////////////////////////////////////////////////////////////
                //var test = _context.ApprovalRoleMaps.Include(a => a.ApprovalLevel).ToList().OrderBy(o => o.ApprovalLevel.Level);
                int reqApprovGroupId = (int)_context.Employees.Find(reqEmpid).ApprovalGroupId;
                var getEmpClaimApproversAllLevels = _context.ApprovalRoleMaps.Include(a => a.ApprovalLevel).Where(a => a.ApprovalGroupId == reqApprovGroupId).OrderBy(o => o.ApprovalLevel.Level).ToList();

                var ReqEmpRoleId = _context.Employees.Where(e => e.Id == reqEmpid).FirstOrDefault().RoleId;
                var ReqEmpHisOwnApprLevel = _context.ApprovalRoleMaps.Where(a => a.RoleId == ReqEmpRoleId);
                bool isFirstApprover = true;

                if (isSelfApprovedRequest)
                {
                    ExpenseReimburseStatusTracker expenseReimburseStatusTracker = new()
                    {
                        EmployeeId = expenseReimburseRequestDto.EmployeeId,
                        ExpenseReimburseRequestId = expenseReimburseRequest.Id,
                        CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId,
                        TotalClaimAmount = dblTotalClaimAmount,
                        ExpReimReqDate = DateTime.Now,
                        DepartmentId = reqEmp.DepartmentId,
                        ProjectId = null, //Approver Project Id
                        JobRoleId = reqEmp.RoleId,
                        IsBusinessAreaReq = false,
                        BAApprovalGroupId = null,
                        BARoleId = null,
                        ApprovalGroupId = reqApprGroupId,
                        ApprovalLevelId = reqApprLevel,
                        ApprovedDate = null,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Approved, //1-Pending, 2-Approved, 3-Rejected
                        Comments = "Self Approved Request"
                    };
                    _context.ExpenseReimburseStatusTrackers.Add(expenseReimburseStatusTracker);
                    expenseReimburseRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                    expenseReimburseRequest.Comments = "Approved";
                    _context.ExpenseReimburseRequests.Update(expenseReimburseRequest);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Self Approved:Expense table Updated with Approved Status");
                }
                else
                {
                    _logger.LogInformation("Not Self approved - Multiple Approvers ");
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

                        _logger.LogInformation(approver.GetFullName() + " Status Tracker started");

                        ExpenseReimburseStatusTracker expenseReimburseStatusTracker = new();


                        expenseReimburseStatusTracker.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                        expenseReimburseStatusTracker.ExpenseReimburseRequestId = expenseReimburseRequest.Id;
                        expenseReimburseStatusTracker.CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId;
                        expenseReimburseStatusTracker.TotalClaimAmount = dblTotalClaimAmount;
                        expenseReimburseStatusTracker.ExpReimReqDate = DateTime.Now;
                        expenseReimburseStatusTracker.IsBusinessAreaReq = false;
                        expenseReimburseStatusTracker.DepartmentId = reqEmp.DepartmentId;
                        expenseReimburseStatusTracker.ProjectId = null; //Approver Project Id
                        expenseReimburseStatusTracker.JobRoleId = approver.RoleId;
                        expenseReimburseStatusTracker.ApprovalGroupId = reqApprGroupId;
                        expenseReimburseStatusTracker.ApprovalLevelId = ApprMap.ApprovalLevelId;
                        expenseReimburseStatusTracker.BAApprovalGroupId = null;
                        expenseReimburseStatusTracker.BARoleId = null;
                        expenseReimburseStatusTracker.ApprovedDate = null;
                        expenseReimburseStatusTracker.ApprovalStatusTypeId = isFirstApprover ? (int)EApprovalStatus.Pending : (int)EApprovalStatus.Initiating;
                        expenseReimburseStatusTracker.Comments = "Awaiting Approver Action";

                        _context.ExpenseReimburseStatusTrackers.Add(expenseReimburseStatusTracker);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation(approver.GetFullName() + " Status Tracker inserted");
                        //##### 5. Send email to the Approver
                        //####################################

                        if (isFirstApprover)
                        {
                            _logger.LogInformation(approver.GetFullName() + "Email Start");

                            string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "ExpApprNotificationEmail.html" };
                            string FilePath = Path.Combine(paths);
                            _logger.LogInformation("Email template path " + FilePath);
                            StreamReader str = new StreamReader(FilePath);
                            string MailText = str.ReadToEnd();
                            str.Close();


                            var approverMailAddress = approver.Email;
                            string subject = expenseReimburseRequest.ExpenseReportTitle + " - #" + expenseReimburseRequest.Id.ToString();
                            Employee emp = _context.Employees.Find(expenseReimburseRequestDto.EmployeeId);

                            var builder = new MimeKit.BodyBuilder();

                            MailText = MailText.Replace("{Requester}", emp.GetFullName());
                            MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                            MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                            MailText = MailText.Replace("{RequestedAmount}", expenseReimburseRequest.TotalClaimAmount.ToString());
                            MailText = MailText.Replace("{RequestNumber}", expenseReimburseRequest.Id.ToString());
                            builder.HtmlBody = MailText;

                            _logger.LogInformation(MailText);
                            var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                            await _emailSender.SendEmailAsync(messagemail);
                            _logger.LogInformation(approver.GetFullName() + "Email Sent");
                        }
                        isFirstApprover = false;

                        //repeat for each approver
                    }

                }

                //##### 5. Adding a entry in DisbursementsAndClaimsMaster table for records
                #region
                _logger.LogInformation("DisbursementsAndClaimsMaster table Start");
                DisbursementsAndClaimsMaster disbursementsAndClaimsMaster = new();
                disbursementsAndClaimsMaster.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                disbursementsAndClaimsMaster.ExpenseReimburseReqId = expenseReimburseRequest.Id;
                disbursementsAndClaimsMaster.RequestTypeId = (int)ERequestType.ExpenseReim;
                disbursementsAndClaimsMaster.IsBusinessAreaReq = false;
                disbursementsAndClaimsMaster.DepartmentId = reqEmp.DepartmentId;
                disbursementsAndClaimsMaster.ProjectId = expenseReimburseRequestDto.ProjectId;
                disbursementsAndClaimsMaster.SubProjectId = expenseReimburseRequestDto.SubProjectId;
                disbursementsAndClaimsMaster.WorkTaskId = expenseReimburseRequestDto.WorkTaskId;
                disbursementsAndClaimsMaster.RecordDate = DateTime.Now;
                disbursementsAndClaimsMaster.CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId;
                disbursementsAndClaimsMaster.ClaimAmount = dblTotalClaimAmount;
                disbursementsAndClaimsMaster.CostCenterId = _context.Departments.Find(_context.Employees.Find(expenseReimburseRequestDto.EmployeeId).DepartmentId).CostCenterId;
                disbursementsAndClaimsMaster.ApprovalStatusId = (int)EApprovalStatus.Pending; //1-Initiating; 2-Pending; 3-InReview; 4-Approved; 5-Rejected
                disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                //save at the end of the code. not here!
                #endregion


                /// #############################
                //   Crediting back to the wallet (for self approvedRequest Only)
                /// #############################
                /// 
                if (isSelfApprovedRequest)
                {
                    double expenseReimAmt = expenseReimburseRequest.TotalClaimAmount;
                    double RoleLimitAmt = _context.JobRoles.Find(reqEmp.RoleId).MaxPettyCashAllowed;
                    EmpCurrentPettyCashBalance empCurrentPettyCashBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == reqEmp.Id).FirstOrDefault();
                    double empCurPettyBal = empCurrentPettyCashBalance.CurBalance;

                    //logic goes here

                    if (expenseReimAmt + empCurPettyBal >= RoleLimitAmt) // claiming amount is greater than replishable amount
                    {
                        disbursementsAndClaimsMaster.AmountToWallet = RoleLimitAmt - empCurPettyBal;
                        disbursementsAndClaimsMaster.AmountToCredit = expenseReimAmt - (RoleLimitAmt - empCurPettyBal);

                    }
                    else
                    {
                        //fully credit to Wallet - Zero amount to bank amount
                        disbursementsAndClaimsMaster.AmountToWallet = expenseReimAmt;
                        disbursementsAndClaimsMaster.AmountToCredit = 0;
                    }

                    disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                    disbursementsAndClaimsMaster.ApprovalStatusId = (int)EApprovalStatus.Approved;
                    _context.Update(disbursementsAndClaimsMaster);


                    //////Final Approveer hence update the EmpCurrentPettyCashBalance table for the employee to reflect the credit
                    ////empCurrentPettyCashBalance.CurBalance = empCurPettyBal + disbursementsAndClaimsMaster.AmountToWallet ?? 0;
                    ////empCurrentPettyCashBalance.UpdatedOn = DateTime.Now;
                    ////_context.EmpCurrentPettyCashBalances.Update(empCurrentPettyCashBalance);
                    try
                    {
                        await _context.DisbursementsAndClaimsMasters.AddAsync(disbursementsAndClaimsMaster);
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "DisbursementsAndClaimsMasters save failed ");
                        return 1;
                    }
                    _logger.LogInformation("DisbursementsAndClaimsMaster table insert complete");
                    _logger.LogInformation("Department Request Created successfully");

                    await AtoCashDbContextTransaction.CommitAsync();
                    return 0;
                }
                ///

                await _context.DisbursementsAndClaimsMasters.AddAsync(disbursementsAndClaimsMaster);
                await _context.SaveChangesAsync();

                _logger.LogInformation("DisbursementsAndClaimsMaster table insert complete");

                await AtoCashDbContextTransaction.CommitAsync();

            }
            //

            _logger.LogInformation("Deaprtment Area Request Created successfully");
            return 0;


        }

        private async Task<int> BusinessAreaBasedExpReimRequest(ExpenseReimburseRequestDTO expenseReimburseRequestDto)
        {
            _logger.LogInformation("Business Area Request Started");
            #region
            int reqEmpid = expenseReimburseRequestDto.EmployeeId;
            Employee reqEmp = _context.Employees.Find(reqEmpid);
            int reqBAApprGroupId = reqEmp.BusinessAreaApprovalGroupId ?? 0;// here the approval group shoulbe be based on Business Area
            int reqRoleId = reqEmp.BusinessAreaRoleId ?? 0;
            int costCenterId = _context.BusinessAreas.Find(reqEmp.BusinessAreaId).CostCenterId;

            //if Approval Role Map list is null

            var approRoleMap = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqBAApprGroupId).FirstOrDefault();

            if (approRoleMap == null)
            {
                _logger.LogError("Approver Role Map Not defined, approval group id " + reqBAApprGroupId);

                return 1;
            }
            else
            {
                var approRoleMaps = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqBAApprGroupId).ToList();

                foreach (ApprovalRoleMap ApprMap in approRoleMaps)
                {
                    int role_id = ApprMap.RoleId;
                    var approver = _context.Employees.Where(e => e.BusinessAreaRoleId == role_id && e.BusinessAreaApprovalGroupId == reqBAApprGroupId).FirstOrDefault();
                    if (approver == null)
                    {
                        _logger.LogError("Approver employee not mapped for RoleMap RoleId:" + role_id + "ApprovalGroupId:" + reqBAApprGroupId);
                        return 1;
                    }

                }
            }

            _logger.LogInformation("All Approvers defined");

            var approRolMapsList = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqBAApprGroupId).ToList();

            int maxApprLevel = approRolMapsList.Select(x => x.ApprovalLevel).Max(a => a.Level);
            int reqApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqBAApprGroupId && a.RoleId == reqRoleId).Select(x => x.ApprovalLevel).FirstOrDefault().Level;


            bool isSelfApprovedRequest = false;
            ////

            ExpenseReimburseRequest expenseReimburseRequest = new();
            double dblTotalClaimAmount = 0;

            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                _logger.LogInformation("Exp Reimb Table insert started");
                expenseReimburseRequest.ExpenseReportTitle = expenseReimburseRequestDto.ExpenseReportTitle;
                expenseReimburseRequest.BusinessAreaId = expenseReimburseRequestDto.BusinessAreaId;
                expenseReimburseRequest.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                expenseReimburseRequest.CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId;
                expenseReimburseRequest.TotalClaimAmount = dblTotalClaimAmount; //Currently Zero but added as per the request
                expenseReimburseRequest.ExpReimReqDate = DateTime.Now;
                expenseReimburseRequest.BusinessAreaId = reqEmp.BusinessAreaId;
                expenseReimburseRequest.IsBusinessAreaReq = true;
                expenseReimburseRequest.DepartmentId = null;
                expenseReimburseRequest.CostCenterId = costCenterId;
                expenseReimburseRequest.ProjectId = null;
                expenseReimburseRequest.SubProjectId = null;
                expenseReimburseRequest.WorkTaskId = null;
                expenseReimburseRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Pending;
                //expenseReimburseRequest.ApprovedDate = expenseReimburseRequestDto.ApprovedDate;
                expenseReimburseRequest.Comments = "Expense Reimburse Request in Process!";

                _context.ExpenseReimburseRequests.Add(expenseReimburseRequest); //  <= this generated the Id
                await _context.SaveChangesAsync();

                _logger.LogInformation("Exp Reimb Table inserted successfully");
                
                //assign values

                _logger.LogInformation("Sub Claims section started");
                foreach (ExpenseSubClaimDTO expenseSubClaimDto in expenseReimburseRequestDto.ExpenseSubClaims)
                {
                    ExpenseSubClaim expenseSubClaim = new();

                    //get expensereimburserequestId from the saved record and then use here for sub-claims
                    expenseSubClaim.ExpenseReimburseRequestId = expenseReimburseRequest.Id;
                    expenseSubClaim.ExpenseTypeId = expenseSubClaimDto.ExpenseTypeId;
                    expenseSubClaim.EmployeeId = reqEmpid;
                    expenseSubClaim.ExpenseReimbClaimAmount = expenseSubClaimDto.ExpenseReimbClaimAmount;
                    expenseSubClaim.DocumentIDs = expenseSubClaimDto.DocumentIDs;
                    expenseSubClaim.InvoiceNo = expenseSubClaimDto.InvoiceNo;
                    expenseSubClaim.InvoiceDate = expenseSubClaimDto.InvoiceDate;
                    expenseSubClaim.BusinessAreaId = reqEmp.BusinessAreaId;

                    expenseSubClaim.IsBusinessAreaReq = true;
                    expenseSubClaim.ExpenseCategoryId = expenseSubClaimDto.ExpenseCategoryId;
                    expenseSubClaim.IsVAT = expenseSubClaimDto.IsVAT;
                    expenseSubClaim.TaxNo = expenseSubClaimDto.TaxNo;
                    expenseSubClaim.ExpStrtDate = expenseSubClaimDto.ExpStrtDate;
                    expenseSubClaim.ExpEndDate = expenseSubClaimDto.ExpEndDate;
                    expenseSubClaim.ExpNoOfDays = expenseSubClaimDto.ExpNoOfDays;

                    expenseSubClaim.Tax = expenseSubClaimDto.Tax;
                    expenseSubClaim.TaxAmount = expenseSubClaimDto.TaxAmount;
                    expenseSubClaim.Vendor = expenseSubClaimDto.Vendor;
                    expenseSubClaim.Location = expenseSubClaimDto.Location;
                    expenseSubClaim.DepartmentId = null;
                    expenseSubClaim.ProjectId = null;
                    expenseSubClaim.SubProjectId = null;
                    expenseSubClaim.WorkTaskId = null;
                    expenseSubClaim.CostCenterId = costCenterId;
                    expenseSubClaim.Description = expenseSubClaimDto.Description;

                    _context.ExpenseSubClaims.Add(expenseSubClaim);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sub Claims Table insert failed");
                        return 1;
                    }

                    dblTotalClaimAmount = dblTotalClaimAmount + expenseSubClaimDto.TaxAmount + expenseSubClaimDto.ExpenseReimbClaimAmount;

                }

                ExpenseReimburseRequest exp = _context.ExpenseReimburseRequests.Find(expenseReimburseRequest.Id);

                exp.TotalClaimAmount = dblTotalClaimAmount;
                _context.ExpenseReimburseRequests.Update(exp);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "TotalClaimAmount update failed");
                    return 1;
                }

                _logger.LogInformation("Sub Claims Table records inserted");


                ///////////////////////////// Check if self Approved Request /////////////////////////////

                //if highest approver is requesting Petty cash request himself
                if (maxApprLevel == reqApprLevel)
                {
                    isSelfApprovedRequest = true;
                }
                //////////////////////////////////////////////////////////////////////////////////////////
                //var test = _context.ApprovalRoleMaps.Include(a => a.ApprovalLevel).ToList().OrderBy(o => o.ApprovalLevel.Level);
                int reqApprovGroupId = (int)_context.Employees.Find(reqEmpid).BusinessAreaApprovalGroupId;
                var getEmpClaimApproversAllLevels = _context.ApprovalRoleMaps.Include(a => a.ApprovalLevel).Where(a => a.ApprovalGroupId == reqApprovGroupId).OrderBy(o => o.ApprovalLevel.Level).ToList();

                var ReqEmpRoleId = _context.Employees.Where(e => e.Id == reqEmpid).FirstOrDefault().BusinessAreaRoleId;
                var ReqEmpHisOwnApprLevel = _context.ApprovalRoleMaps.Where(a => a.RoleId == ReqEmpRoleId);
                bool isFirstApprover = true;

                if (isSelfApprovedRequest)
                {
                    _logger.LogInformation("Self Approved Request");
                    ExpenseReimburseStatusTracker expenseReimburseStatusTracker = new()
                    {
                        EmployeeId = expenseReimburseRequestDto.EmployeeId,
                        ExpenseReimburseRequestId = expenseReimburseRequest.Id,
                        CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId,
                        TotalClaimAmount = dblTotalClaimAmount,
                        ExpReimReqDate = DateTime.Now,
                        IsBusinessAreaReq = true,
                        DepartmentId = null,
                        BusinessAreaId = reqEmp.BusinessAreaId,
                        BAApprovalGroupId = reqEmp.BusinessAreaApprovalGroupId,
                        BARoleId = reqEmp.BusinessAreaRoleId,
                        ProjectId = null, //Approver Project Id
                        JobRoleId = reqEmp.BusinessAreaRoleId,
                        ApprovalGroupId = reqBAApprGroupId,
                        ApprovalLevelId = reqApprLevel,
                        ApprovedDate = null,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Approved, //1-Pending, 2-Approved, 3-Rejected
                        Comments = "Self Approved Request"
                    };
                    _context.ExpenseReimburseStatusTrackers.Add(expenseReimburseStatusTracker);
                    expenseReimburseRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                    expenseReimburseRequest.Comments = "Approved";
                    _context.ExpenseReimburseRequests.Update(expenseReimburseRequest);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Self approved ExpenseReimburseRequests update failed");
                        return 1;
                    }

                    _logger.LogInformation("Self Approved:Expense table Updated with Approved Status");
                }
                else
                {
                    _logger.LogInformation("Not Self approved - Multiple Approvers ");
                    foreach (ApprovalRoleMap ApprMap in getEmpClaimApproversAllLevels)
                    {
                        int BARole_id = ApprMap.RoleId;
                        var approver = _context.Employees.Where(e => e.BusinessAreaRoleId == BARole_id && e.BusinessAreaApprovalGroupId == reqBAApprGroupId).FirstOrDefault();
                        if (approver == null)
                        {
                            continue;
                        }

                        int approverLevelid = _context.ApprovalRoleMaps.Where(x => x.RoleId == approver.BusinessAreaRoleId && x.ApprovalGroupId == reqBAApprGroupId).FirstOrDefault().ApprovalLevelId;
                        int approverLevel = _context.ApprovalLevels.Find(approverLevelid).Level;

                        if (reqApprLevel >= approverLevel)
                        {
                            continue;
                        }

                        _logger.LogInformation(approver.GetFullName() + " Status Tracker started");
                        ExpenseReimburseStatusTracker expenseReimburseStatusTracker = new()
                        {
                            EmployeeId = expenseReimburseRequestDto.EmployeeId,
                            ExpenseReimburseRequestId = expenseReimburseRequest.Id,
                            CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId,
                            TotalClaimAmount = dblTotalClaimAmount,
                            ExpReimReqDate = DateTime.Now,
                            IsBusinessAreaReq = true,
                            BusinessAreaId = reqEmp.BusinessAreaId,
                            DepartmentId = null,
                            ProjectId = null, //Approver Project Id
                            JobRoleId = approver.RoleId,
                            ApprovalGroupId = approver.ApprovalGroupId,
                            ApprovalLevelId = ApprMap.ApprovalLevelId,
                            BAApprovalGroupId = reqBAApprGroupId,
                            BARoleId = BARole_id,
                            ApprovedDate = null,
                            ApprovalStatusTypeId = isFirstApprover ? (int)EApprovalStatus.Pending : (int)EApprovalStatus.Initiating,
                            Comments = "Awaiting Approver Action"
                        };
                        _context.ExpenseReimburseStatusTrackers.Add(expenseReimburseStatusTracker);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation(approver.GetFullName() + " Status Tracker inserted");
                        //##### 5. Send email to the Approver
                        //####################################

                        if (isFirstApprover)
                        {
                            _logger.LogInformation(approver.GetFullName() + "Email Start");

                            string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "ExpApprNotificationEmail.html" };
                            string FilePath = Path.Combine(paths);
                            _logger.LogInformation("Email template path " + FilePath);
                            StreamReader str = new StreamReader(FilePath);
                            string MailText = str.ReadToEnd();
                            str.Close();

                            var approverMailAddress = approver.Email;
                            string subject = expenseReimburseRequest.ExpenseReportTitle + " - #" + expenseReimburseRequest.Id.ToString();
                            Employee emp = _context.Employees.Find(expenseReimburseRequestDto.EmployeeId);

                            var builder = new MimeKit.BodyBuilder();

                            MailText = MailText.Replace("{Requester}", emp.GetFullName());
                            MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                            MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                            MailText = MailText.Replace("{RequestedAmount}", expenseReimburseRequest.TotalClaimAmount.ToString());
                            MailText = MailText.Replace("{RequestNumber}", expenseReimburseRequest.Id.ToString());
                            builder.HtmlBody = MailText;

                            var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                            await _emailSender.SendEmailAsync(messagemail);
                            _logger.LogInformation(approver.GetFullName() + "Email Sent");
                        }
                        isFirstApprover = false;

                        //repeat for each approver
                    }

                }

                //##### 5. Adding a entry in DisbursementsAndClaimsMaster table for records
                #region
                _logger.LogInformation("DisbursementsAndClaimsMaster table Start");

                DisbursementsAndClaimsMaster disbursementsAndClaimsMaster = new();
                disbursementsAndClaimsMaster.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                disbursementsAndClaimsMaster.ExpenseReimburseReqId = expenseReimburseRequest.Id;
                disbursementsAndClaimsMaster.RequestTypeId = (int)ERequestType.ExpenseReim;
                disbursementsAndClaimsMaster.IsBusinessAreaReq = true;
                disbursementsAndClaimsMaster.DepartmentId = expenseReimburseRequest.IsBusinessAreaReq == false ? reqEmp.DepartmentId : null;
                disbursementsAndClaimsMaster.BusinessAreaId = expenseReimburseRequest.IsBusinessAreaReq == true ? reqEmp.BusinessAreaId : null;
                disbursementsAndClaimsMaster.ProjectId = expenseReimburseRequestDto.ProjectId;
                disbursementsAndClaimsMaster.SubProjectId = expenseReimburseRequestDto.SubProjectId;
                disbursementsAndClaimsMaster.WorkTaskId = expenseReimburseRequestDto.WorkTaskId;
                disbursementsAndClaimsMaster.RecordDate = DateTime.Now;
                disbursementsAndClaimsMaster.CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId;
                disbursementsAndClaimsMaster.ClaimAmount = dblTotalClaimAmount;
                disbursementsAndClaimsMaster.CostCenterId = _context.Departments.Find(_context.Employees.Find(expenseReimburseRequestDto.EmployeeId).DepartmentId).CostCenterId;
                disbursementsAndClaimsMaster.ApprovalStatusId = (int)EApprovalStatus.Pending; //1-Initiating; 2-Pending; 3-InReview; 4-Approved; 5-Rejected
                disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                //save at the end of the code. not here!
                #endregion


                /// #############################
                //   Crediting back to the wallet (for self approvedRequest Only)
                /// #############################
                /// 

                if (isSelfApprovedRequest)
                {
                    double expenseReimAmt = expenseReimburseRequest.TotalClaimAmount;
                    double RoleLimitAmt = _context.JobRoles.Find(reqEmp.RoleId).MaxPettyCashAllowed;
                    EmpCurrentPettyCashBalance empCurrentPettyCashBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == reqEmp.Id).FirstOrDefault();
                    double empCurPettyBal = empCurrentPettyCashBalance.CurBalance;

                    //logic goes here

                    if (expenseReimAmt + empCurPettyBal >= RoleLimitAmt) // claiming amount is greater than replishable amount
                    {
                        disbursementsAndClaimsMaster.AmountToWallet = RoleLimitAmt - empCurPettyBal;
                        disbursementsAndClaimsMaster.AmountToCredit = expenseReimAmt - (RoleLimitAmt - empCurPettyBal);

                    }
                    else
                    {
                        //fully credit to Wallet - Zero amount to bank amount
                        disbursementsAndClaimsMaster.AmountToWallet = expenseReimAmt;
                        disbursementsAndClaimsMaster.AmountToCredit = 0;
                    }

                    disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                    disbursementsAndClaimsMaster.ApprovalStatusId = (int)EApprovalStatus.Approved;
                    _context.Update(disbursementsAndClaimsMaster);
                    _logger.LogInformation("DisbursementsAndClaimsMaster approve/reject update");

                    //////Final Approveer hence update the EmpCurrentPettyCashBalance table for the employee to reflect the credit
                    ////empCurrentPettyCashBalance.CurBalance = empCurPettyBal + disbursementsAndClaimsMaster.AmountToWallet ?? 0;
                    ////empCurrentPettyCashBalance.UpdatedOn = DateTime.Now;
                    ////_context.EmpCurrentPettyCashBalances.Update(empCurrentPettyCashBalance);

                    try
                    {
                        await _context.DisbursementsAndClaimsMasters.AddAsync(disbursementsAndClaimsMaster);
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "DisbursementsAndClaimsMasters save failed ");
                        return 1;
                    }
                    _logger.LogInformation("DisbursementsAndClaimsMaster table insert complete");
                    _logger.LogInformation("Business Area Request Created successfully");
                    await AtoCashDbContextTransaction.CommitAsync();
                    return 0;
                }
                ///

                try
                {
                    await _context.DisbursementsAndClaimsMasters.AddAsync(disbursementsAndClaimsMaster);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("DisbursementsAndClaimsMaster table insert complete");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message.ToString());
                    return 1;
                }
                await AtoCashDbContextTransaction.CommitAsync();
            }
            _logger.LogInformation("Business Area Request Created successfully");
            return 0;


        }
        #endregion


        //
        private async Task<int> ProjectBasedExpReimRequest(ExpenseReimburseRequestDTO expenseReimburseRequestDto)
        {
            _logger.LogInformation("ProjectBasedExpReimRequest Started");
            //### 1. If Employee Eligible for Cash Claim enter a record and reduce the available amount for next claim
            #region
            int costCenterId = _context.Projects.Find(expenseReimburseRequestDto.ProjectId).CostCenterId;
            int projManagerid = _context.Projects.Find(expenseReimburseRequestDto.ProjectId).ProjectManagerId;
            var approver = _context.Employees.Find(projManagerid);
            int reqEmpid = expenseReimburseRequestDto.EmployeeId;
            Employee reqEmp = _context.Employees.Find(reqEmpid);
            int reqApprGroupId = reqEmp.ApprovalGroupId;
            int reqRoleId = reqEmp.RoleId;


            if (approver != null)
            {
                _logger.LogInformation("Project Manager defined, no issues");
            }
            else
            {
                _logger.LogError("Project Manager is not Assigned");
                return 1;
            }



            int maxApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId).ToList().Select(x => x.ApprovalLevel).Max(a => a.Level);
            int reqApprLevel = _context.ApprovalRoleMaps.Include("ApprovalLevel").Where(a => a.ApprovalGroupId == reqApprGroupId && a.RoleId == reqRoleId).Select(x => x.ApprovalLevel).FirstOrDefault().Level;
            bool isSelfApprovedRequest = false;
            ////
            ///

            ExpenseReimburseRequest expenseReimburseRequest = new();
            double dblTotalClaimAmount = 0;

            using (var AtoCashDbContextTransaction = _context.Database.BeginTransaction())
            {
                _logger.LogInformation("Exp Reimb Table insert started");

                expenseReimburseRequest.ExpenseReportTitle = expenseReimburseRequestDto.ExpenseReportTitle;
                expenseReimburseRequest.BusinessAreaId = expenseReimburseRequestDto.BusinessAreaId;
                expenseReimburseRequest.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                expenseReimburseRequest.CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId;
                expenseReimburseRequest.TotalClaimAmount = dblTotalClaimAmount; //Currently Zero but added as per the request
                expenseReimburseRequest.ExpReimReqDate = DateTime.Now;
                expenseReimburseRequest.IsBusinessAreaReq = false;
                expenseReimburseRequest.DepartmentId = null;
                expenseReimburseRequest.ProjectId = expenseReimburseRequestDto.ProjectId;
                expenseReimburseRequest.SubProjectId = expenseReimburseRequestDto.SubProjectId;
                expenseReimburseRequest.WorkTaskId = expenseReimburseRequestDto.WorkTaskId;
                expenseReimburseRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Pending;
                //expenseReimburseRequest.ApprovedDate = expenseReimburseRequestDto.ApprovedDate;
                expenseReimburseRequest.Comments = "Expense Reimburse Request in Process!";

                _context.ExpenseReimburseRequests.Add(expenseReimburseRequest); //  <= this generated the Id
                await _context.SaveChangesAsync();

                _logger.LogInformation("Project: Expense Reimburse Table inserted successfully");

                //assign values

                _logger.LogInformation("Sub Claims section started");

                foreach (ExpenseSubClaimDTO expenseSubClaimDto in expenseReimburseRequestDto.ExpenseSubClaims)
                {
                    ExpenseSubClaim expenseSubClaim = new();

                    //get expensereimburserequestId from the saved record and then use here for sub-claims
                    expenseSubClaim.ExpenseReimburseRequestId = expenseReimburseRequest.Id;
                    expenseSubClaim.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                    expenseSubClaim.ExpenseTypeId = expenseSubClaimDto.ExpenseTypeId;
                    expenseSubClaim.ExpenseReimbClaimAmount = expenseSubClaimDto.ExpenseReimbClaimAmount;
                    expenseSubClaim.DocumentIDs = expenseSubClaimDto.DocumentIDs;
                    expenseSubClaim.InvoiceNo = expenseSubClaimDto.InvoiceNo;
                    expenseSubClaim.InvoiceDate = expenseSubClaimDto.InvoiceDate;
                    expenseSubClaim.IsBusinessAreaReq = false;

                    expenseSubClaim.ExpenseCategoryId = expenseSubClaimDto.ExpenseCategoryId;
                    expenseSubClaim.IsVAT = expenseSubClaimDto.IsVAT;
                    expenseSubClaim.TaxNo = expenseSubClaimDto.TaxNo;
                    expenseSubClaim.ExpStrtDate = expenseSubClaimDto.ExpStrtDate;
                    expenseSubClaim.ExpEndDate = expenseSubClaimDto.ExpEndDate;
                    expenseSubClaim.ExpNoOfDays = expenseSubClaimDto.ExpNoOfDays;

                    expenseSubClaim.Tax = expenseSubClaimDto.Tax;
                    expenseSubClaim.TaxAmount = expenseSubClaimDto.TaxAmount;
                    expenseSubClaim.Vendor = expenseSubClaimDto.Vendor;
                    expenseSubClaim.DepartmentId = null;
                    expenseSubClaim.ProjectId = expenseReimburseRequestDto.ProjectId;
                    expenseSubClaim.SubProjectId = expenseReimburseRequestDto.SubProjectId;
                    expenseSubClaim.WorkTaskId = expenseReimburseRequestDto.WorkTaskId;
                    expenseSubClaim.CostCenterId = costCenterId;
                    expenseSubClaim.Location = expenseSubClaimDto.Location;
                    expenseSubClaim.Description = expenseSubClaimDto.Description;

                    _context.ExpenseSubClaims.Add(expenseSubClaim);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sub Claims Table insert failed");
                        return 1;
                    }


                    dblTotalClaimAmount = dblTotalClaimAmount + expenseSubClaimDto.TaxAmount + expenseSubClaimDto.ExpenseReimbClaimAmount;

                }

                ExpenseReimburseRequest exp = _context.ExpenseReimburseRequests.Find(expenseReimburseRequest.Id);

                exp.TotalClaimAmount = dblTotalClaimAmount;
                _context.ExpenseReimburseRequests.Update(exp);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sub Claims Table records inserted");

                ///////////////////////////// Check if self Approved Request /////////////////////////////
                //if highest approver is requesting Petty cash request himself
                if (maxApprLevel == reqApprLevel || projManagerid == reqEmpid)
                {
                    isSelfApprovedRequest = true;
                }
                //////////////////////////////////////////////////////////////////////////////////////////
                //var test = _context.ApprovalRoleMaps.Include(a => a.ApprovalLevel).ToList().OrderBy(o => o.ApprovalLevel.Level);
                if (isSelfApprovedRequest)
                {
                    _logger.LogInformation("Self Approved Request");
                    ExpenseReimburseStatusTracker expenseReimburseStatusTracker = new()
                    {
                        EmployeeId = expenseReimburseRequestDto.EmployeeId,
                        ExpenseReimburseRequestId = expenseReimburseRequest.Id,
                        CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId,
                        TotalClaimAmount = dblTotalClaimAmount,
                        ExpReimReqDate = DateTime.Now,
                        IsBusinessAreaReq = false,
                        DepartmentId = null,
                        ProjManagerId = projManagerid,
                        ProjectId = expenseReimburseRequestDto.ProjectId, //Approver Project Id
                        SubProjectId = expenseReimburseRequestDto.SubProjectId,
                        WorkTaskId = expenseReimburseRequestDto.WorkTaskId,
                        JobRoleId = _context.Employees.Find(expenseReimburseRequestDto.EmployeeId).RoleId,
                        ApprovalGroupId = reqApprGroupId,
                        ApprovalLevelId = 2,  //(reqApprLevel) or 2  default approval level is 2 for Project based request
                        ApprovedDate = null,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Approved, //1-Pending, 2-Approved, 3-Rejected
                        Comments = "Self Approved Request!"
                    };
                    _context.ExpenseReimburseStatusTrackers.Add(expenseReimburseStatusTracker);
                    expenseReimburseRequest.ApprovalStatusTypeId = (int)EApprovalStatus.Approved;
                    expenseReimburseRequest.Comments = "Approved";
                    _context.ExpenseReimburseRequests.Update(expenseReimburseRequest);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Self Approved:Expense table Updated with Approved Status");
                }
                else
                {
                    _logger.LogInformation("Not Self approved - Project Manager is Approver");
                    _logger.LogInformation(approver.GetFullName() + " Status Tracker started");
                   
                    
                    ExpenseReimburseStatusTracker expenseReimburseStatusTracker = new()
                    {
                        EmployeeId = expenseReimburseRequestDto.EmployeeId,
                        ExpenseReimburseRequestId = expenseReimburseRequest.Id,
                        CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId,
                        TotalClaimAmount = dblTotalClaimAmount,
                        ExpReimReqDate = DateTime.Now,
                        IsBusinessAreaReq = false,
                        DepartmentId = null,
                        ProjManagerId = projManagerid,
                        ProjectId = expenseReimburseRequestDto.ProjectId, //Approver Project Id
                        SubProjectId = expenseReimburseRequestDto.SubProjectId,
                        WorkTaskId = expenseReimburseRequestDto.WorkTaskId,
                        JobRoleId = approver.RoleId,
                        ApprovalGroupId = reqApprGroupId,
                        ApprovalLevelId = 2, // default approval level is 2 for Project based request
                        ApprovedDate = null,
                        ApprovalStatusTypeId = (int)EApprovalStatus.Pending,
                        Comments = "Expense Reimburse is in Process!"
                    };
                    _context.ExpenseReimburseStatusTrackers.Add(expenseReimburseStatusTracker);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation(approver.GetFullName() + " Status Tracker inserted");

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, " Status Tracker insert failed");
                        return 1;
                    }

                   
                    //##### 5. Send email to the Approver
                    //####################################
                    if (isSelfApprovedRequest)
                    {
                        return 0;
                    }

                    _logger.LogInformation(approver.GetFullName() + "Email Start");

                    string[] paths = { Directory.GetCurrentDirectory(), "EmailTemplate", "ExpApprNotificationEmail.html" };
                    string FilePath = Path.Combine(paths);
                    _logger.LogInformation("Email template path " + FilePath);
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();

                    var approverMailAddress = approver.Email;
                    string subject = expenseReimburseRequest.ExpenseReportTitle + " - #" + expenseReimburseRequest.Id.ToString();
                    Employee emp = _context.Employees.Find(expenseReimburseRequestDto.EmployeeId);

                    var builder = new MimeKit.BodyBuilder();

                    MailText = MailText.Replace("{Requester}", emp.GetFullName());
                    MailText = MailText.Replace("{ApproverName}", approver.GetFullName());
                    MailText = MailText.Replace("{Currency}", _context.CurrencyTypes.Find(emp.CurrencyTypeId).CurrencyCode);
                    MailText = MailText.Replace("{RequestedAmount}", expenseReimburseRequest.TotalClaimAmount.ToString());
                    MailText = MailText.Replace("{RequestNumber}", expenseReimburseRequest.Id.ToString());
                    builder.HtmlBody = MailText;

                    var messagemail = new Message(new string[] { approverMailAddress }, subject, builder.HtmlBody);

                    await _emailSender.SendEmailAsync(messagemail);

                    _logger.LogInformation(approver.GetFullName() + "Email Sent");


                    //repeat for each approver
                }
                #endregion

                //##### 5. Adding a entry in DisbursementsAndClaimsMaster table for records
                #region
                _logger.LogInformation("DisbursementsAndClaimsMaster table Start");
                DisbursementsAndClaimsMaster disbursementsAndClaimsMaster = new();
                disbursementsAndClaimsMaster.EmployeeId = expenseReimburseRequestDto.EmployeeId;
                disbursementsAndClaimsMaster.ExpenseReimburseReqId = expenseReimburseRequest.Id;
                disbursementsAndClaimsMaster.RequestTypeId = (int)ERequestType.ExpenseReim;
                disbursementsAndClaimsMaster.IsBusinessAreaReq = false;
                disbursementsAndClaimsMaster.DepartmentId = null;
                disbursementsAndClaimsMaster.ProjectId = expenseReimburseRequestDto.ProjectId;
                disbursementsAndClaimsMaster.SubProjectId = expenseReimburseRequestDto.SubProjectId;
                disbursementsAndClaimsMaster.WorkTaskId = expenseReimburseRequestDto.WorkTaskId;
                disbursementsAndClaimsMaster.RecordDate = DateTime.Now;
                disbursementsAndClaimsMaster.CurrencyTypeId = expenseReimburseRequestDto.CurrencyTypeId;
                disbursementsAndClaimsMaster.ClaimAmount = dblTotalClaimAmount;
                disbursementsAndClaimsMaster.CostCenterId = costCenterId;
                disbursementsAndClaimsMaster.ApprovalStatusId = (int)EApprovalStatus.Pending; //1-Initiating; 2-Pending; 3-InReview; 4-Approved; 5-Rejected
                disbursementsAndClaimsMaster.IsSettledAmountCredited = false;
                //save at the end of the code. not here!
                #endregion


                /// #############################
                //   Crediting back to the wallet (for self approvedRequest Only)
                /// #############################
                /// 
                if (isSelfApprovedRequest)
                {
                    double expenseReimAmt = expenseReimburseRequest.TotalClaimAmount;
                    double RoleLimitAmt = _context.JobRoles.Find(_context.Employees.Find(reqEmp.Id).RoleId).MaxPettyCashAllowed;
                    EmpCurrentPettyCashBalance empCurrentPettyCashBalance = _context.EmpCurrentPettyCashBalances.Where(e => e.EmployeeId == reqEmp.Id).FirstOrDefault();
                    double empCurPettyBal = empCurrentPettyCashBalance.CurBalance;

                    //logic goes here

                    if (expenseReimAmt + empCurPettyBal >= RoleLimitAmt) // claiming amount is greater than replishable amount
                    {
                        disbursementsAndClaimsMaster.AmountToWallet = RoleLimitAmt - empCurPettyBal;
                        disbursementsAndClaimsMaster.AmountToCredit = expenseReimAmt - (RoleLimitAmt - empCurPettyBal);
                    }
                    else
                    {
                        //fully credit to Wallet - Zero amount to bank amount
                        disbursementsAndClaimsMaster.AmountToWallet = expenseReimAmt;
                        disbursementsAndClaimsMaster.AmountToCredit = 0;
                    }


                    disbursementsAndClaimsMaster.ApprovalStatusId = (int)EApprovalStatus.Approved;
                    _context.Update(disbursementsAndClaimsMaster);


                    //////Final Approveer hence update the EmpCurrentPettyCashBalance table for the employee to reflect the credit
                    ////empCurrentPettyCashBalance.CurBalance = empCurPettyBal + disbursementsAndClaimsMaster.AmountToWallet ?? 0;
                    ////empCurrentPettyCashBalance.UpdatedOn = DateTime.Now;
                    ////_context.EmpCurrentPettyCashBalances.Update(empCurrentPettyCashBalance);

                    await _context.DisbursementsAndClaimsMasters.AddAsync(disbursementsAndClaimsMaster);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Project: Self approved Expense request failed");
                        return 1;
                    }

                    _logger.LogInformation("DisbursementsAndClaimsMaster approve/reject updated");
                    return 0;
                }
                ///

                try
                {
                    await _context.DisbursementsAndClaimsMasters.AddAsync(disbursementsAndClaimsMaster);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Project Expense request failed");
                    return 1;
                }

                _logger.LogInformation("DisbursementsAndClaimsMaster approve/reject updated");

              
                await AtoCashDbContextTransaction.CommitAsync();
            }
            return 0;
        }

    }
    #endregion
}
