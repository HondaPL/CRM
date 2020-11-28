using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CRM.Controllers
{
    public class UsersController : Controller
    {
        private readonly CRMContext _context;

        public UsersController(CRMContext context)
        {
            _context = context;
        }

        // GET: Users
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.User.ToListAsync());
        //}
        [Authorize]
        public async Task<IActionResult> Index(int page = 1, string sortExpression = "Id")
        {
            var qry = _context.User.AsNoTracking().OrderBy(p => p.Id);
            var model = await PagingList.CreateAsync(qry, 6, page, sortExpression, "Id");
            string[] roles = { "Admin" , "Moderator", "User"};
            ViewBag.roles = roles;
            return View(model);
        }

        // GET: Users/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            string[] roles = { "Admin", "Moderator", "User" };
            ViewBag.roles = roles;
            return View(user);
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,DateOfBirth,Login,Password,RoleId")] User user)
        {
            try
            {
                var user1 = await _context.User.AsNoTracking().FirstOrDefaultAsync(m => m.Login == user.Login);
                if (user1 == null) { 
                    user.Password = HashPassword(user.Password);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                } else
                {
                    ModelState.AddModelError("", "Login is taken");
                    return View(user);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Login is taken");
                return View(user);
            }
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,DateOfBirth,Login,Password,RoleId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            var adminnumber = await _context.User.CountAsync(m => m.RoleId == "1");


            //var user1 = await _context.User.FirstOrDefaultAsync(m => m.Login == user.Login);
            //var user2 = await _context.User.FindAsync(id);


            try
            {
                var user1 = await _context.User.AsNoTracking().FirstOrDefaultAsync(m => m.Login == user.Login);
                //var user1 = await _context.User.FirstOrDefaultAsync(m => m.Login == user.Login);
                if (user1 == null || id == user1.Id)
                {
                    //try
                    //{
                    if (ModelState.IsValid)
                    {
                        //_context.Update(user1);
                        //_context.Remove(user1);
                        ViewBag.Message = null;
                        if (adminnumber > 2 || user.RoleId == "1")
                        {
                            if (!IsMD5(user.Password))
                            {
                                user.Password = HashPassword(user.Password);
                            }
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Index");
                        } else
                        {
                            ViewBag.Message = String.Format("You cannot delete more admins! There must be at least 2 of them!");
                            return View(user);
                        }
                    }
                    return View(user);
                    //}
                    //catch (Exception)
                    //{
                    //    if (!UserExists(user.Id))
                    //    {
                    //        return NotFound();
                    //    }
                    //    else
                    //    {
                                
                    //    }
                    //}
                    //return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Login is taken");
                    return View(user);
                }
            } catch (Exception)
            {
                //throw;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (id != user.Id)
                        {
                            return NotFound();
                        }
                        if (!IsMD5(user.Password))
                        {
                            user.Password = HashPassword(user.Password);
                        }
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        return View(user);
                    } else
                    {
                        return View(user);
                    }
                }
                catch (Exception)
                {
                    //throw;
                    ModelState.AddModelError("", "Invalid data");
                    return View(user);
                }
            }
            
            
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            string[] roles = { "Admin", "Moderator", "User" };
            ViewBag.roles = roles;
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            var adminnumber = await _context.User.CountAsync(m => m.RoleId == "1");
            Console.Write(adminnumber);
            ViewBag.Message = null;
            if (adminnumber > 2 || user.RoleId != "1") {
                var companyNumber = await _context.Company.CountAsync(m => m.UserId == Convert.ToString(user.Id));
                while (companyNumber > 0)
                {
                    var companyToDelete = await _context.Company.FirstOrDefaultAsync(m => m.UserId == Convert.ToString(user.Id));
                    var noteNumber = await _context.Note.CountAsync(m => m.CompanyId == Convert.ToString(companyToDelete.Id));
                    while (noteNumber > 0)
                    {
                        var noteToDelete = await _context.Note.FirstOrDefaultAsync(m => m.CompanyId == Convert.ToString(companyToDelete.Id));
                        _context.Note.Remove(noteToDelete);
                        await _context.SaveChangesAsync();
                        noteNumber--;
                    }
                    var contactNumber = await _context.Contact.CountAsync(m => m.CompanyId == Convert.ToString(companyToDelete.Id));
                    while (contactNumber > 0)
                    {
                        var contactToDelete = await _context.Contact.FirstOrDefaultAsync(m => m.CompanyId == Convert.ToString(companyToDelete.Id));
                        _context.Contact.Remove(contactToDelete);
                        await _context.SaveChangesAsync();
                        contactNumber--;
                    }
                    _context.Company.Remove(companyToDelete);
                    await _context.SaveChangesAsync();
                    companyNumber--;
                }
                var noteNumberU = await _context.Note.CountAsync(m => m.UserId == Convert.ToString(user.Id));
                while (noteNumberU > 0)
                {
                    var noteToDelete = await _context.Note.FirstOrDefaultAsync(m => m.UserId == Convert.ToString(user.Id));
                    _context.Note.Remove(noteToDelete);
                    await _context.SaveChangesAsync();
                    noteNumberU--;
                }
                var contactNumberU = await _context.Contact.CountAsync(m => m.UserId == Convert.ToString(user.Id));
                while (contactNumberU > 0)
                {
                    var contactToDelete = await _context.Contact.FirstOrDefaultAsync(m => m.UserId == Convert.ToString(user.Id));
                    _context.Contact.Remove(contactToDelete);
                    await _context.SaveChangesAsync();
                    contactNumberU--;
                }
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }   else
            {
                ViewBag.Message = String.Format("You cannot demote more admins! There must be at least 2 of them!");
                return View(user);
            }
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        public string HashPassword(string password)
        {
            byte[] hashedPassword;
            using (MD5 md5 = MD5.Create())
            {
                hashedPassword = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashedPassword.Length; i++)
            {
                sb.Append(hashedPassword[i].ToString("x2"));
            }

            return Convert.ToString(sb);
        }
        public static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }
    }
}
