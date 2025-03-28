using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FePledgeDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.PledgePages
{
    public class IndexModel : PageModel
    {
        public IList<PledgeDTO> Pledges { get; set; } = new List<PledgeDTO>();
        public string Message { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (TempData["Message"] != null)
                {
                    Message = TempData["Message"].ToString();
                }
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Index");
                }
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await httpClient.GetAsync("https://marvelous-gentleness-production.up.railway.app/api/Pledge/GetAllPledges");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<List<PledgeDTO>>>(content);

                        if (result != null && result.Success)
                        {
                            Pledges = result.Data ?? new List<PledgeDTO>();
                            return Page();
                        }
                        else
                        {
                            Message = result?.Message ?? "Failed to retrieve users.";
                        }
                    }
                    else
                    {
                        Message = $"Error: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return Page();
        }
    }
}
