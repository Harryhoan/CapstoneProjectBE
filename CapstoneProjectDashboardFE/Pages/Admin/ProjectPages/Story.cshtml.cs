using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.ProjectPages
{
    public class StoryModel : PageModel
    {
        public ProjectDetailDTO ProjectStory { get; set; } = new ProjectDetailDTO();
        public string Message { get; set; } = string.Empty;

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

                    var response = await httpClient.GetAsync($"https://marvelous-gentleness-production.up.railway.app/api/Project/GetProjectById?id={projectId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<ProjectDetailDTO>>(content);

                        if (result != null && result.Success)
                        {
                            ProjectStory = result.Data ?? new ProjectDetailDTO();
                            return Page();
                        }
                        else
                        {
                            Message = result?.Message ?? "Failed to retrieve project story.";
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
