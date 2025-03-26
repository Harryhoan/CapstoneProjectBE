using Application.ViewModels.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.Pages.Admin.UserPages
{
    public class IndexModel : PageModel
    {
        public IList<UserDTO> UserDTO { get; set; } = new List<UserDTO>();
        public string Message { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if(TempData["Message"] != null)
                {
                    Message = TempData["Message"].ToString();
                }
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Login");
                }
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var query = new List<string>();


                    var queryString = string.Join("&", query);
                    var response = await httpClient.GetAsync("https://marvelous-gentleness-production.up.railway.app/api/User/GetAllUser");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<UserDTO>>(content);
                        UserDTO = result;
                        return Page();
                    }
                    else
                    {
                        return RedirectToPage("/Index");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Page();
            }
        }
    }
}
