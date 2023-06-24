using CafeManagement.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace CafeManagement.Web.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            var viewModel = new CustomerCreateViewModel();
            viewModel.SelectListItems = new List<SelectListItem>();

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://localhost:7013/api/Company/getall");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var companyModel = JsonConvert.DeserializeObject<CompanyListModel>(responseContent);

                foreach (var company in companyModel.data.ToList())
                {
                    viewModel.SelectListItems.Add(new SelectListItem() { Text = company.name, Value = company.id.ToString() });
                }
            }

            httpClient.Dispose();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var httpClient = new HttpClient();

                var name = collection["Name"];
                var surname = collection["Surname"];
                var phone = collection["Phone"];
                var email = collection["Email"];
                var identityNumber = collection["IdentityNumber"];
                var birthDate = DateTime.Parse(collection["BirthDate"]);
                var companyId = int.Parse(collection["CompanyId"]);

                var createModel = new CustomerCreateViewModel
                {
                    Name = name,
                    Surname = surname,
                    Phone = phone,
                    Email = email,
                    IdentityNumber = identityNumber,
                    BirthDate = birthDate,
                    CompanyId = companyId
                };

                var jsonData = JsonConvert.SerializeObject(createModel);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://localhost:7013/api/Customer/create", content);

                httpClient.Dispose();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Create request failed.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        public async Task<ActionResult> List()
        {
            var viewModel = new CustomerListModel();
       

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://localhost:7013/api/Customer/getall");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                viewModel = JsonConvert.DeserializeObject<CustomerListModel>(responseContent);

            }

            httpClient.Dispose();

            return View(viewModel.data);
        }

        public async Task<ActionResult> SettingsUpdate()
        {
            try
            {
                var httpClient = new HttpClient();

                // Company listesini API'den al
                var companyListResponse = await httpClient.GetAsync("https://localhost:7013/api/Company/getall");
                var companyListContent = await companyListResponse.Content.ReadAsStringAsync();
                var companyList = JsonConvert.DeserializeObject<CompanyListModel>(companyListContent);

      

                httpClient.Dispose();

  
                var viewModel = new SettingsUpdateViewModel();

                viewModel.SelectListItems = new List<SelectListItem>();

                foreach (var company in companyList.data)
                {
                    viewModel.SelectListItems.Add(new SelectListItem() { Text = company.name, Value = company.id.ToString() });
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SettingsUpdate(SettingsUpdateViewModel viewModel)
        {
            try
            {
               
                    var httpClient = new HttpClient();

                    var obj = new
                    {
                        CompanyId=viewModel.CompanyId,
                        IsMernis=viewModel.IsMernis
                    };
                    var jsonData = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var updateResponse = await httpClient.PostAsync("https://localhost:7013/api/Company/CompanySettingUpdate", content);

                    httpClient.Dispose();

                    if (updateResponse.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(SettingsUpdate));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Update request failed.");
                    }
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetIsMernisStatus(int companyId)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://localhost:7013/api/Company/getusecompany?companyId={companyId}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var isMernis = JsonConvert.DeserializeObject<SettingsResultModel>(responseContent);
                return Json(isMernis.data);
            }

            return Json(false);
        }
    }
}
