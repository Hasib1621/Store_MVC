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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(CompanyList);
        }

        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
            //.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString()
            //}
            //);
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;

            
            if(id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company company = _unitOfWork.Company.Get(u => u.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company Company)
        {
            
            
            if (ModelState.IsValid)
            {
                
                if(Company.Id == 0)
                {
                    _unitOfWork.Company.Add(Company);
                }
                else
                {
                    _unitOfWork.Company.Update(Company);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(new Company());
            }
            
        }

        /*public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? Company = _unitOfWork.Company.Get(c => c.Id == id);
            if (Company == null)
            {
                return NotFound();
            }
            return View(Company);
        }

        [HttpPost]
        public IActionResult Edit(Company Company)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Update(Company);
                _unitOfWork.Save();
                TempData["success"] = "Company updated successfully";
                return RedirectToAction("Index", "Company");
            }
            return View();

        }*/

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? Company = _unitOfWork.Company.Get(c => c.Id == id);
        //    if (Company == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(Company);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Company? Company = _unitOfWork.Company.Get(c => c.Id == id);

        //    if (Company == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Company.Remove(Company);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Company deleted successfully";
        //    return RedirectToAction("Index", "Company");
        //}

        #region

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data = CompanyList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company Company = _unitOfWork.Company.Get(u=>u.Id == id);
            if(Company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(Company); _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}
