using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_2.Areas.Admin.ViewModels;

namespace project_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        UserManager<IdentityUser> user;
        SignInManager<IdentityUser> signin;

        public AccountController(UserManager<IdentityUser> user, SignInManager<IdentityUser> signin)
        {
            this.user = user;
            this.signin = signin;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
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
                return RedirectToAction("Index", "Appointment");
            }
            return View();
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
                return RedirectToAction("Index", "Appointment");
            }
            return View();
        }
    }
}
