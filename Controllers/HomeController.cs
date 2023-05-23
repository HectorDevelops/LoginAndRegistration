using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LoginAndRegistration.Models;
// this using should be imported in order to utilize the PasswordHasher 
using Microsoft.AspNetCore.Identity;
// checks/filters where someone is on session or not (like a validation to the controller)
using Microsoft.AspNetCore.Mvc.Filters;
namespace LoginAndRegistration.Controllers;
#pragma warning disable CS8600 // ignores the current fields that need to be non-nulled 

public class HomeController : Controller
{
    private MyContext db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, MyContext _context)
    {
        db = _context;
        _logger = logger;
    }

    // Login and Registration window
    public IActionResult Index()
    {
        return View("Index");
    }

    // Registration POST method 
    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        // validating the user's input  and if it passes ... 
        if (ModelState.IsValid)
        {
            // begin hashing the password upon a new user registration
            PasswordHasher<User> HashedPassword = new PasswordHasher<User>();
            // accessing the newUser instance to store the created hashed password
            newUser.Password = HashedPassword.HashPassword(newUser, newUser.Password);

            // adding this new user into our db and it's properties upon user input
            db.Users.Add(newUser);
            db.SaveChanges();

            // Using session to be able to display appropriate Name of the user and UserId
            HttpContext.Session.SetInt32("CurrentUser", newUser.UserId);
            HttpContext.Session.SetString("Name", newUser.FirstName);
            return RedirectToAction("Success");
        }
        else
        {
            // If any issues occur, return the user to the home page
            Console.WriteLine("There's an issue here!");
            return Index();

        }
    }

    [HttpPost("/login")]
    public IActionResult Login(LoginUser existingUser)
    {
        if (ModelState.IsValid)
        {
            User? dbUser = db.Users.FirstOrDefault(user => user.Email == existingUser.LoginEmail);

            if (dbUser == null)
            {
                ModelState.AddModelError("LoginEmail", "not found.");
                return Index();
            }
            PasswordHasher<LoginUser> HashedPassword = new PasswordHasher<LoginUser>();
            PasswordVerificationResult pwComparison = HashedPassword.VerifyHashedPassword(existingUser, dbUser.Password, existingUser.LoginPassword);

            if (pwComparison == 0)
            {
                ModelState.AddModelError("LoginPassword", "invalid password");
                return Index();
            }
            HttpContext.Session.SetInt32("CurrentUser", dbUser.UserId);
            HttpContext.Session.SetString("Name", dbUser.FirstName);

            return RedirectToAction("Success");
        }
        else
        {
            Console.WriteLine("Error found!");
            return Index();
        }
    }

    [SessionCheck]
    [HttpGet("/success")]
    public IActionResult Success()
    {
        return View("Success");
    }

    [SessionCheck]
    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Name this anything you want with the word "Attribute" at the end
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Find the session, but remember it may be null so we need int?
            int? userId = context.HttpContext.Session.GetInt32("CurrentUser");
            // Check to see if we got back null
            if (userId == null)
            {
                // Redirect to the Index page if there was nothing in session
                // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }

}