using AtoCash.Data;
using AtoCash.Models;
using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AtoCash.Authentication
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender _emailSender;
        private readonly AtoCashDbContext context;


        public AccountController(IEmailSender emailSender, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AtoCashDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        [ActionName("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            if (userid == null  || token == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "userid or token is invalid" });
            }

            var user = await userManager.FindByIdAsync(userid);

            if (user == null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "User not found!" });
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok(new RespStatus { Status = "Success", Message = "Thank you for confirming Email!" });
            }

            return Conflict(new RespStatus { Status = "Failure", Message = "Email nor confirmed!" });
        
    }


        // GET: api/<AccountController>
        [HttpPost]
        [ActionName("Register")]
        [Authorize(Roles = "AtominosAdmin, Admin, Manager, Finmgr")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            //check if employee-id is already registered

            var empid = model.EmployeeId;

            bool empIdExists = context.Users.Where(x => x.EmployeeId == model.EmployeeId).Any();

            if (empIdExists)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Employee Id is already taken" });
            }


            //check if email is already in use if yes.. throw error

            var useremail = await userManager.FindByEmailAsync(model.Email);

            if (useremail != null)
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Email is already taken" });
            }

            MailAddress mailAddress = new(model.Email);
            
            //MODIFY HOST DOMAIN NAME HERE => CURRENTLY only GMAIL and MAILINATOR
            if ( mailAddress.Host.ToUpper() != Constants.EmailDomain.ToUpper())
            {
                return Conflict(new RespStatus { Status = "Failure", Message = "Use company mail address!" });
            }
            //Creating a IdentityUser object
            var user = new ApplicationUser { 
                EmployeeId = model.EmployeeId,
                UserName= model.Username, 
                Email = model.Email, 
                PasswordHash = model.Password };

             IdentityResult result = await userManager.CreateAsync(user, model.Password);


            RespStatus respStatus = new();

            if (result.Succeeded)
            {
                //Assigning a User Role to the Registered user
                result = await userManager.AddToRoleAsync(user, "User");

                if (result.Succeeded)
                {
                    respStatus.Message = "User Registered and User Access Role added to Employee!";
                    respStatus.Status = "Success";
                }
                else
                {
                    respStatus.Message = "User Registered but User Access Role not added to Employee!";
                    respStatus.Status = "Failure";
                }
            }
            else
            {
                respStatus.Message = "User Name should be Unique!";
                respStatus.Status = "Failure";
            }

            return Ok(respStatus);
        }


        [HttpPost]
        [ActionName("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //Creating a IdentityUser object

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(new RespStatus { Status = "Failure", Message = "Username or Password Incorrect" });
            }

            if (user.EmployeeId!=0)
            {
                var emplye = context.Employees.Find(user.EmployeeId);
                bool isEmpActive = emplye.StatusTypeId == (int)EStatusType.Active;
                if (!isEmpActive)
                {
                    return Unauthorized(new RespStatus { Status = "Failure", Message = "Employee is Inactive" });
                }

            }

            //check if the employee is 1. active or 2.inactive to deny login
          

           var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            

            //if signin successful send message
            if (result.Succeeded)
            {
                var secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKey12323232"));

                var signingcredentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

                var modeluser = await userManager.FindByEmailAsync(model.Email);
                var userroles = await userManager.GetRolesAsync(modeluser);
                var empid = user.EmployeeId;
                int currencyId;
                string currencyCode;
                string empFirstName;
                string empLastName;
                string empEmail;

                var employee = await context.Employees.FindAsync(empid);
                if (empid == 0)
                {
                    empFirstName = "Admin";
                    empLastName = Constants.EmailDomain;
                    empEmail = "admin@" + Constants.EmailDomain;
                    currencyCode = "INR";
                    currencyId = 1;
                }
                else
                {
                    empFirstName = employee.FirstName;
                    empLastName = employee.LastName;
                    empEmail = employee.Email;
                    currencyId = employee.CurrencyTypeId;
                     currencyCode = context.CurrencyTypes.Find(currencyId).CurrencyCode;
                }
                    
   

                //add claims
                var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, modeluser.UserName),
                 new Claim(ClaimTypes.Email, model.Email),
                 new Claim("EmployeeId", empid.ToString())

                };
                //add all roles belonging to the user
                 foreach (var role in userroles) 
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenOptions = new JwtSecurityToken(

                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: claims,
                    expires: DateTime.Now.AddHours(5),
                     signingCredentials: signingcredentials
                    ) ;


                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                
                return Ok( new { Token = tokenString, Role = userroles, FirstName = empFirstName, LastName = empLastName, EmpId = empid.ToString(), Email= empEmail, currencyCode, currencyId });
            }

            return Unauthorized(new RespStatus { Status = "Failure", Message = "Username or Password Incorrect" });
        }



        [HttpPost]
        [ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            //check if employee-id is already registered

           

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.email);

                //bool isUserConfirmed = await userManager.IsEmailConfirmedAsync(user);
                //if (user != null && isUserConfirmed)

                if (user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    //var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var passwordResetlink= Url.Action("ResetPassword", "Account", new { email = model.email, token = token, Request.Scheme });

                    //return Ok(passwordResetLink);
                    token = token.Replace("+", "^^^");
                    var receiverEmail = model.email;
                    string subject = "Password Reset Link";
                    string content = "Please click the below Password Reset Link to reset your password:"  + Environment.NewLine + 
                                        "https://atocash.netlify.app/change-password?token=" + token +"&email=" + model.email;

                    //"<a href=\"https://atocash.netlify.app/change-password?token=" + token + "&email=" + model.email + "\">";
                    var messagemail = new Message(new string[] { receiverEmail }, subject, content);

                    await _emailSender.SendEmailAsync(messagemail);

                }

                return Ok(new RespStatus { Status = "Success", Message = "Password Reset Email Sent!" });
            }
            return Conflict(new RespStatus { Status = "Failure", Message = "Input params invalid" });
        }




        [HttpPost]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.email);

                if (user != null)
                {

                    model.Token = model.Token.Replace("^^^", "+");
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        var receiverEmail = model.email;
                        string subject = "Password Changed";
                        string content = "Your new Password is:" + model.Password;
                        var messagemail = new Message(new string[] { receiverEmail }, subject, content);

                        await _emailSender.SendEmailAsync(messagemail);
                        return Ok(new RespStatus { Status = "Success", Message = "Your Password has been reset!" });
                    }

                    List<object> errResp = new();
                    foreach (var error in result.Errors)
                    {
                        errResp.Add(error.Description);
                    }
                    return Ok(errResp);
                }

                return Conflict(new RespStatus { Status = "Failure", Message = "User is invalid" });

            }

            return Conflict(new RespStatus { Status = "Failure", Message = "Model state is invalid" });

        }


       

        ////


    }
}
