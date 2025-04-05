using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.ProjectPages
{
    public class CollaboratorModel : PageModel
    {
        public List<CollaboratorDTO> CollaboratorDTO { get; set; } = new List<CollaboratorDTO>();
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

                    var response = await httpClient.GetAsync($"https://marvelous-gentleness-production.up.railway.app/api/Collaborator/project?projectId={projectId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<List<CollaboratorDTO>>>(content);

                        if (result != null && result.Success)
                        {
                            CollaboratorDTO = result.Data ?? new List<CollaboratorDTO>();
                            return Page();
                        }
                        else
                        {
                            Message = result?.Message ?? "Failed to retrieve rewards.";
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
