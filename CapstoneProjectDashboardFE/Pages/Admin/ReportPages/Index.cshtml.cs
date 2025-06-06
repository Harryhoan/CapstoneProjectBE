using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FeReportDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.ReportPages
{
    public class IndexModel : PageModel
    {
        public IList<ReportDto> Reports { get; set; } = new List<ReportDto>();
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

                    var response = await httpClient.GetAsync("https://marvelous-gentleness-production.up.railway.app/api/Report/GetAllReport");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<List<ReportDto>>>(content);

                        if (result != null && result.Success)
                        {
                            Reports = result.Data ?? new List<ReportDto>();
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
