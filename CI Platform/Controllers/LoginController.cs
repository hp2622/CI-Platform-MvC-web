using System.Security.Claims;
using CI_Platform.Controllers;
using CI_Platform.Models;
using CI_Platform_Entity.CIDbContext;
using CI_Platform_Entity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class LoginController : Controller
{
    private readonly CipContext _CipContext;

    public LoginController(CipContext CipContext)
    {
        _CipContext = CipContext;


    }

   
    public IActionResult Login()
    {
        HttpContext.Session.Clear();
       
        return View();
    }

   

    [HttpPost]
    public async Task<IActionResult> Login(Login model)
    {

        if (ModelState.IsValid)
        {

            
            var user = await _CipContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
            List<City> cities = _CipContext.Cities.ToList();
            var username = model.Email.Split("@")[0];
            if (user != null)
            {
                int userid = ((int)user.UserId);
                HttpContext.Session.SetString("userID", username);
                //HttpContext.Session.SetInt32("userIDforfavmission", userid);
                HttpContext.Session.SetString("userid", userid.ToString());
                HttpContext.Session.SetString("firstname", user.FirstName);
                
                
                
                return RedirectToAction("LandingDummy", "MissionLandingPage");
            }
            else
            {
                ViewBag.Error = "Email or Password is Incorrect";
                
            }
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(LoginController.Login), "Login");
    }
}