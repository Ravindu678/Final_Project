using BookIt1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
public class AccountController : Controller
{
    private readonly SignInManager<BookIt1User> _signInManager;

    public AccountController(SignInManager<BookIt1User> signInManager)
    {
        _signInManager = signInManager;
    }


    public IActionResult Login()
    {
        return Redirect("https://localhost:7195/Identity/Account/Login");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync(); //sign out user
        return RedirectToAction("Login", "Account", new { area = "Admin" });
    }
}
