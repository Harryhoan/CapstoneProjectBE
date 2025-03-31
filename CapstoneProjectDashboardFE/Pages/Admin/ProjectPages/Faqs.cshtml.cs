using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.ProjectPages
{
    public class FaqsModel : PageModel
    {
        public List<FaqDTO> Faqs { get; set; } = new List<FaqDTO>();
        public string Message { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int projectId)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToPage("/Index");
                }

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await httpClient.GetAsync($"https://localhost:5001/api/Faq/GetFaqByProjectId?projectId={projectId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<List<FaqDTO>>>(content);

                        if (result != null && result.Success)
                        {
                            Faqs = result.Data ?? new List<FaqDTO>();
                            return Page();
                        }
                        else
                        {
                            Message = result?.Message ?? "Failed to retrieve FAQs.";
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
