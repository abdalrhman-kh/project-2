using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using project_2.Areas.Admin.ViewModels;
using project_2.Helpers.Email;
using project_2.Helpers.OTPHelper;
using project_2.Models;
using project_2.Models.Repositories;
using project_2.ViewModels;

namespace project_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        UserManager<IdentityUser> user;
        SignInManager<IdentityUser> signin;
        IRepository<OTP> repoOtp;
        IOTPHelper otp;
        IEmailSender mail;

        public AccountController(UserManager<IdentityUser> user, SignInManager<IdentityUser> signin, IOTPHelper otp, IEmailSender mail, IRepository<OTP> repoOtp)
        {
            this.user = user;
            this.signin = signin;
            this.otp = otp;
            this.mail = mail;
            this.repoOtp = repoOtp;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult VerifyEmail()
        {
            return View();
        }
        public IActionResult ChangePassword(string Email)
        {
            if (Email.IsNullOrEmpty())
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = Email });
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail([FromBody]VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var GetUser = await user.FindByEmailAsync(model.Email);

                if (GetUser == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return BadRequest();
                }

                else
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (ModelState.IsValid) { 
                var GetUser = await user.FindByEmailAsync(model.Email);
                if (GetUser != null)
                {
                    var Token = await user.GeneratePasswordResetTokenAsync(GetUser);
                    var result = await user.ResetPasswordAsync(GetUser, Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Something went wronge");
                return View(model);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await signin.PasswordSignInAsync(model.Email, model.Password, true, false);
            if (result.Succeeded)
            {
                return RedirectToAction("ViewAppointment", "Appointment");
            }
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await signin.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            IdentityUser obj = new IdentityUser();
            obj.Email = model.Email;
            obj.UserName = model.Email;
            var result = await user.CreateAsync(obj, model.Password);
            if (result.Succeeded)
            {
                await signin.SignInAsync(obj, true);
                return RedirectToAction("ViewAppointment", "Appointment");
            }
            return View();
        }
        public async Task <IActionResult> RequestOTP([FromBody] OTPRequestViewModel model)
        {
            if (String.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required");
            }
            try
            {
                var exsistingCode = repoOtp.GetAll().FirstOrDefault(x => x.Email == model.Email && x.expiry > DateTime.Now);

                if(exsistingCode != null)
                {
                    return Conflict(new { message = "A valid code has already been sent. Please check your inbox or wait for it to expire." });
                }

                var OtpCode = await otp.GenerateOTP(model.Email);
                string subject = "Access Code";
                string body = $"Your verification code is: {OtpCode} it will stay vaild for 5 minutes do not share it with anyone";
                
                await mail.SendEmailAsync(model.Email, subject, body);
                return Ok(new { message = "A verification code has been sent to your email." });
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerifyViewModel model)
        {
            if (String.IsNullOrEmpty(model.Code))
            {
                return BadRequest("Invalid code");
            }
            try
            {
                var valid = await otp.VerifyOTP(model.Email, model.Code);
                if (!valid)
                {
                    return BadRequest("Wrong code please try again");
                }
                else
                {
                    return Ok( new {Email = model.Email});
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
