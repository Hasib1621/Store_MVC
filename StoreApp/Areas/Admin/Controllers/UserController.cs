using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repository;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;
using System.Data;

namespace StoreApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(u=>u.UserId == userId).RoleId;
            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select(i=>new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _db.Companies.Select(i=>new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
            return View(RoleVM);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                if(user.Company == null)
                {
                    var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                    user.Role = roles.FirstOrDefault(u=>u.Id == roleId).Name;
                    
                    user.Company = new()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new {data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u=>u.Id == id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //locked currently
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion

    }
}
