using AspNestTaskAll.Models;
using AspNestTaskAll.ViewModels.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager; 

        public AuthController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            await _signInManager.SignInAsync(nUser, true);
            return RedirectToAction("Index","Home");
        }
        
    }
}
