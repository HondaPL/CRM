using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CRM.Models;
using CRM.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        const string SessionName = "_Name";
        const string SessionAge = "_Age";

        private readonly CRMContext _context;

        
        public HomeController(CRMContext context)
        {
            _context = context;
        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            var user = await _context.User.FindAsync(1);
            HttpContext.Session.SetString(SessionName, "Adam!");
            //var user2 = await _context.User.FirstAsync(m => m.Id == 1);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            return View(user);
        }

        //[Authorize]
        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //public async Task<IActionResult> Login(int? login)
        //{
        //    //if (login == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    var x = _context.Role.Any(m => m.Id == login);
        //    if (x) {
        //        return RedirectToAction("Index", "Users");
        //    }
        //    return View();

        //    //if (user == null)
        //    //{
        //    //    return NotFound();
        //    //}

            
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
