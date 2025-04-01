using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FeReportDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.ReportPages
{
    public class DetailModel : PageModel
    {
        public ReportDetailDto reportDetail { get; set; } = new ReportDetailDto();
        public string Message { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int reportId)
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
                    var response = await httpClient.GetAsync($"https://marvelous-gentleness-production.up.railway.app/api/Report/GetReportById?reportId={reportId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<ReportDetailDto>>(content);

                        if (result != null && result.Success)
                        {
                            reportDetail = result.Data ?? new ReportDetailDto();
                            return Page();
                        }
                        else
                        {
                            Message = result?.Message ?? "Failed to retrieve report details.";
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
