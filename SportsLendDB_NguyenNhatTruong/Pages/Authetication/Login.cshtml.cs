using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsLend.BLL.Service;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SportsLendDB_NguyenNhatTruong.Pages.Authetication
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            return CheckLogin();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var loginCheck = CheckLogin();
            if (loginCheck is RedirectToPageResult)
            {
                return loginCheck;
            }
            
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userService.GetUserAccountAsync(Email, Password);
            if(user == null)
            {

                ModelState.AddModelError(string.Empty, "Invalid Email or Password!");
                TempData["Message"] = "Invalid Email or Password!";

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false, 
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                authProperties);

            Response.Cookies.Append("FullName", user.FullName);

            return RedirectToPage("/Equipment/Index");
        }

        private IActionResult CheckLogin()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToPage("/Equipment/Index");
            }
            return Page();
        }
    }
}
