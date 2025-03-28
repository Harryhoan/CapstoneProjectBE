using CapstoneProjectDashboardFE.ModelDTO.FeUserDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProjectDashboardFE.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public LoginModel LoginModel { get; set; }
        public string Message { get; set; } = default!;
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync("https://marvelous-gentleness-production.up.railway.app/api/Authentication/login", LoginModel);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

                        HttpContext.Session.SetString("Token", result.Token);
                        HttpContext.Session.SetString("Role", result.Role);
                        //HttpContext.Session.SetInt32("Hint", result.Hint);
                        if (result.Role != "Admin" && result.Role != "Staff")
                        {
                            Message = "You are not allow to access.";
                            return Page();
                        }
                        if (result.Role == "Role")
                        {
                            return RedirectToPage("/Staff/UserPages/Index");
                        }
                        return RedirectToPage("/Admin/UserPages/Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        if (errorContent.Length != 0)
                        {
                            throw new Exception(errorContent);
                        }
                        else
                        {
                            throw new Exception("Field is not null!");
                        }
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
