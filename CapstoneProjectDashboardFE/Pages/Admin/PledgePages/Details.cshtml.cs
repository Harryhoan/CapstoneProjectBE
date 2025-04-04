using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FePledgeDTO;
using CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.PledgePages
{
    public class DetailModel : PageModel
    {
        public PledgeDTO pledgeDTO { get; set; } = new PledgeDTO();
        public string Message { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int pledgeId)
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

                    // Append userId as a query parameter
                    var response = await httpClient.GetAsync($"https://marvelous-gentleness-production.up.railway.app/api/Pledge/GetPledgeById?pledgeId={pledgeId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<PledgeDTO>>(content);

                        if (result != null && result.Success)
                        {
                            pledgeDTO = result.Data ?? new PledgeDTO();
                            return Page();
                        }
                        else
                        {
                            Message = result?.Message ?? "Failed to retrieve user details.";
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
