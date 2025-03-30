using Application.ServiceResponse;
using CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CapstoneProjectDashboardFE.Pages.Admin.ProjectPages
{
    public class RewardsModel : PageModel
    {
        public List<RewardDTO> Rewards { get; set; } = new List<RewardDTO>();
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

                    var response = await httpClient.GetAsync($"https://marvelous-gentleness-production.up.railway.app/api/Reward/GetRewardByProjectId?projectId={projectId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ServiceResponse<List<RewardDTO>>>(content);

                        if (result != null && result.Success)
                        {
                            Rewards = result.Data ?? new List<RewardDTO>();
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
