using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MitraKaryaSystem.Models;

namespace MitraKaryaSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService) => _customerService = customerService;
        public IActionResult Index() => View();
        public async Task<JsonResult> GetList()=> Json(await _customerService.GetList());
        [HttpPost]
        public async Task<IActionResult> Save(CustomerModel customer)=> Json(await _customerService.Save(customer));
        [HttpPost]
        public async Task<IActionResult> FillForm(int id)=> PartialView("_CustomerModal", await _customerService.FillForm(id));
        [HttpPost]
        public async Task<JsonResult> Delete(int id)=> Json(await _customerService.Delete(id));
    }
}
