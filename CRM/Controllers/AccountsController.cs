using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CRM.Controllers
{
    public class AccountsController : Controller
    {
        private readonly CRMContext _context;

        public AccountsController(CRMContext context)
        {
            _context = context;
        }

        const string UserName = "_Name";
        const string SessionAge = "_Age";

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> DetailsAsync()
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return View(user);
            }
        }
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        private async Task<bool> ValidateLoginAsync(string userName, string password)
        {
            // For this sample, all logins are successful.
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userName);
            try
            {
                if (password == user.Password && user.IsDeleted == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } catch(Exception)
            {
                return false;
            }
        }

        private async Task<string> ReturnRole(string userName)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userName);
            var role = await _context.Role.FindAsync(Convert.ToInt32(user.RoleId));
            return role.Name;
            //if(user.RoleId == "1")
            //{
            //    return "Admin";
            //} else if(user.RoleId == "2")
            //{
            //    return "Moderator";
            //} else
            //{
            //    return "User";
            //}
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = HashPassword(model.Password);
                //Here we are checking the values with hardcoded admin and admin
                //You can check these values from a database
                if (await ValidateLoginAsync(model.UserName, model.Password))
                {
                    //Store the Username in session
                    //Session["UserName"] = model.UserName;
                    HttpContext.Session.SetString(UserName, model.UserName);
                    string role = await ReturnRole(model.UserName);
                    var claims = new List<Claim>
                {
                    new Claim("user", model.UserName),
                    new Claim("role", role)
                };

                    await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
                    //Then redirect to the Index Action method of Home Controller
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login or Password");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }

        }

        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                var user1 = await _context.User.FirstOrDefaultAsync(m => m.Login == user.Login);
                if (user1.Id == 0)
                {
                    if (ModelState.IsValid)
                    {
                        user.Password = HashPassword(user.Password);
                        _context.Add(user);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Login");
                    }
                    return View(user);
                } else
                {
                    ModelState.AddModelError("", "Login is taken");
                    return View(user);
                }
            }
            catch (Exception)
            {
                try
                {
                    user.Password = HashPassword(user.Password);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Invalid data");
                    return View(user);
                }
                
            }
        }

        // GET: Accounts/Edit
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.FindFirst("user").Value);

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
        public async Task<IActionResult> Edit(int id,User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            var adminnumber = await _context.User.CountAsync(m => m.RoleId == 1);
            ViewBag.Message = null;


            //if(user1.Id != 0) { 
            //    var login = user1.Login;
            //}
            try
            {
                int x = await LoginUnChangedAsync(id, user.Login);
                //var user1 = await _context.User.FirstOrDefaultAsync(m => m.Login == user.Login);
                if (x<3/*user1.Id == 0 || user1.Id == id*/)
                {
                    if (ModelState.IsValid)
                    {
                        //_context.Update(user1);
                        //_context.Remove(user1);
                        if (adminnumber > 2 || user.RoleId == 1) {
                            if (!IsMD5(user.Password))
                            {
                                user.Password = HashPassword(user.Password);
                            }
                            //_context.Add(user);
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                            if (x == 1 || user.RoleId != Convert.ToInt32(User.FindFirst("role").Value))
                            {
                                await HttpContext.SignOutAsync();
                                string role = await ReturnRole(user.Login);
                                var claims = new List<Claim>
                            {
                                new Claim("user", user.Login),
                                new Claim("role", role)
                            };

                                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
                            }
                            return RedirectToAction("Details");
                        } else
                        {
                            ViewBag.Message = String.Format("You cannot delete more admins! There must be at least 2 of them!");
                            return View(user);
                        }
                    }
                    return View(user);
                }
                else
                {
                    ModelState.AddModelError("", "Login is taken");
                    return View(user);
                }
            }
            catch (Exception)
            {
                //throw;
                try
                {
                    if (!IsMD5(user.Password))
                    {
                        user.Password = HashPassword(user.Password);
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    await HttpContext.SignOutAsync();
                    string role = await ReturnRole(user.Login);
                    var claims = new List<Claim>
                        {
                            new Claim("user", user.Login),
                            new Claim("role", role)
                        };

                    await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

                    return RedirectToAction("Details");
                }
                catch (Exception)
                {
                    //throw;
                    ModelState.AddModelError("", "Invalid data");
                    return View(user);
                }

            }
            //user1 = await _context.User.FirstOrDefaultAsync(m => m.Login == user.Login);
            //var user2 = await _context.User.FindAsync(id);

            //if (ModelState.IsValid)
            //{
            //    if (user1 == null || user1.Login == user2.Login)
            //        {
            //        try
            //        {
            //            _context.Update(user1);
            //            //else { 
            //            //_context.Add(user1);
            //            //}
            //            //_context.Update(user);
            //            await _context.SaveChangesAsync();
            //            }
            //            catch (DbUpdateConcurrencyException)
            //            {
            //                if (!UserExists(user.Id))
            //                {
            //                    return NotFound();
            //                }
            //                else
            //                {
            //                    throw;
            //                }
            //            }
            //            await HttpContext.SignOutAsync();
            //            return Redirect("/");
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("", "Login is taken");
            //            return View(user);
            //        }
            //}
            //return View(user);
        }

        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
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

        [Authorize]
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

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            var adminnumber = await _context.User.CountAsync(m => m.RoleId == 1);
            //Console.Write(adminnumber);
            ViewBag.Message = null;
            if (adminnumber > 2 || user.RoleId != 1)
            {
                /*var companyNumber = await _context.Company.CountAsync(m => m.UserId == user.Id);
                while (companyNumber > 0)
                {
                    var companyToDelete = await _context.Company.FirstOrDefaultAsync(m => m.UserId == user.Id);
                    var noteNumber = await _context.Note.CountAsync(m => m.CompanyId == companyToDelete.Id);
                    while (noteNumber > 0)
                    {
                        var noteToDelete = await _context.Note.FirstOrDefaultAsync(m => m.CompanyId == companyToDelete.Id);
                        _context.Note.Remove(noteToDelete);
                        noteNumber--;
                    }
                    var contactNumber = await _context.Contact.CountAsync(m => m.CompanyId == companyToDelete.Id);
                    while (contactNumber > 0)
                    {
                        var contactToDelete = await _context.Contact.FirstOrDefaultAsync(m => m.CompanyId == companyToDelete.Id);
                        _context.Contact.Remove(contactToDelete);
                        contactNumber--;
                    }
                    _context.Company.Remove(companyToDelete);
                    companyNumber--;
                }
                var noteNumberU = await _context.Note.CountAsync(m => m.UserId == user.Id);
                while (noteNumberU > 0)
                {
                    var noteToDelete = await _context.Note.FirstOrDefaultAsync(m => m.UserId == user.Id);
                    _context.Note.Remove(noteToDelete);
                    noteNumberU--;
                }
                var contactNumberU = await _context.Contact.CountAsync(m => m.UserId == user.Id);
                while (contactNumberU > 0)
                {
                    var contactToDelete = await _context.Contact.FirstOrDefaultAsync(m => m.UserId == user.Id);
                    _context.Contact.Remove(contactToDelete);
                    contactNumberU--;
                }
                */
                //_context.User.Remove(user);
                user.IsDeleted = 1;
                await _context.SaveChangesAsync();
                await HttpContext.SignOutAsync();
                return Redirect("/");
            }
            else
            {
                ViewBag.Message = String.Format("You cannot delete more admins! There must be at least 2 of them!");
                return View(user);
            }



        }
        public static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }

        public async Task<int> LoginUnChangedAsync(int id, string login)
        {
            if (login == User.FindFirst("user").Value)
            {
                return 1;
            }
            else { 
            //var user = await _context.User.FindAsync(id);
            var user1 = await _context.User.FirstOrDefaultAsync(m => m.Login == login);
            if (user1 == null)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
        }
    }
}
