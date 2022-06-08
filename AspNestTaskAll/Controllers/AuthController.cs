using AspNestTaskAll.Models;
using AspNestTaskAll.Utility;
using AspNestTaskAll.ViewModels.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNestTaskAll.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM signInVm,string ReturnUrl)
        {
            AppUser appUser;
            if (signInVm.UserNameOrEmail.Contains("@"))
            {
                appUser = await _userManager.FindByIdAsync(signInVm.UserNameOrEmail);
            }
            else
            {
                appUser = await _userManager.FindByNameAsync(signInVm.UserNameOrEmail);
            }
            if (appUser==null)
            {
                ModelState.AddModelError("","login veya parolda sehvlik var!!");
                return View(signInVm);
            }
            var result =  await _signInManager.PasswordSignInAsync(appUser,signInVm.Password,signInVm.RememberMe,true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("","Limiti ashdiniz! 1 kruq vur gelersen:)");
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "login veya parolda sehvlik var!!");
                return View(signInVm);
            }
            if (ReturnUrl !=null)
            {
                return LocalRedirect(ReturnUrl);
            }
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Register));
        }
        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser nUser = new AppUser() { 
            
                Name = registerVM.FirstName,
                Surname = registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.Username
            };
            IdentityResult result = await _userManager.CreateAsync(nUser,registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("",item.Description);
                }
            }
            await _userManager.AddToRoleAsync(nUser, UserRoles.Member.ToString());
            return RedirectToAction("Index","Home");
        }
        public async Task CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(item.ToString()));
                }

            }
        }
        public  IActionResult ChangePassword()
        {
            return  View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound();
            }
            if (string.Compare(model.NewPassword,model.ConfirmPassword) != 0)
            {
                return NotFound();
            }
            
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            return RedirectToAction("index","home");

        }

    }
}
