using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.PledgeDTO;
using AutoMapper;
using ClosedXML.Excel;

namespace Application.Services
{
    public class PledgeService : IPledgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PledgeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<IEnumerable<PledgeDto>>> GetAllPledgeByAdmin()
        {
            var response = new ServiceResponse<IEnumerable<PledgeDto>>();
            try
            {
                var pledges = await _unitOfWork.PledgeRepo.GetAllAsync();
                if (pledges == null)
                {
                    response.Success = true;
                    response.Message = "Pledge are empty.";
                    return response;
                }
                var pledgeDtos = _mapper.Map<IEnumerable<PledgeDto>>(pledges);
                foreach (var pledgeDto in pledgeDtos)
                {
                    var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledgeDto.PledgeId);
                    var pledgeDetailDtos = _mapper.Map<List<PledgeDetailDto>>(pledgeDetails);
                    pledgeDto.pledgeDetail = pledgeDetailDtos;
                }
                response.Data = pledgeDtos;
                response.Success = true;
                response.Message = "Get all pledges successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get all pledges: {ex.Message}";
                return response;
            }
        }
        public async Task<ServiceResponse<List<PledgeDto>>> GetPledgeByUserId(int userId)
        {
            var response = new ServiceResponse<List<PledgeDto>>();
            try
            {
                var pledges = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAsync(userId);

                if (pledges == null)
                {
                    response.Success = true;
                    response.Message = "Pledge list is empty.";
                    return response;
                }

                var pledgeDtos = _mapper.Map<List<PledgeDto>>(pledges);
                foreach (var pledgeDto in pledgeDtos)
                {
                    var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledgeDto.PledgeId);
                    var pledgeDetailDtos = _mapper.Map<List<PledgeDetailDto>>(pledgeDetails);
                    pledgeDto.pledgeDetail = pledgeDetailDtos;
                }

                response.Data = pledgeDtos;
                response.Success = true;
                response.Message = "Get pledge successfully.";

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get pledge: {ex.Message}";
                return response;
            }
        }
        public async Task<ServiceResponse<PledgeDto>> GetPledgeById(int pledgeId)
        {
            var response = new ServiceResponse<PledgeDto>();
            try
            {
                var pledge = await _unitOfWork.PledgeRepo.GetByIdAsync(pledgeId);
                if (pledge == null)
                {
                    response.Success = false;
                    response.Message = "Pledge not found.";
                    return response;
                }
                var pledgeDto = _mapper.Map<PledgeDto>(pledge);
                var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledgeId);
                var pledgeDetailDtos = _mapper.Map<List<PledgeDetailDto>>(pledgeDetails);
                pledgeDto.pledgeDetail = pledgeDetailDtos;

                response.Data = pledgeDto;
                response.Success = true;
                response.Message = "Get pledge successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get pledge: {ex.Message}";
                return response;
            }
        }
        public async Task<ServiceResponse<List<ProjectBackerDto>>> GetBackerByProjectId(int userId, int projectId)
        {
            var response = new ServiceResponse<List<ProjectBackerDto>>();
            try
            {
                // Fetch pledges for the user in the specified project
                var pledges = await _unitOfWork.PledgeRepo.GetPledgeByProjectIdAsync(projectId);

                if (pledges == null || !pledges.Any())
                {
                    response.Success = true;
                    response.Message = "No pledges found for the specified user and project.";
                    return response;
                }

                // Prepare the result list
                var projectBackerDtos = new List<ProjectBackerDto>();

                foreach (var pledge in pledges)
                {
                    // Fetch backer (user) details
                    var user = await _unitOfWork.UserRepo.GetByIdAsync(pledge.UserId);
                    if (user == null)
                    {
                        continue; // Skip if user not found
                    }

                    // Fetch pledge details
                    var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                    var pledgeDetailDtos = _mapper.Map<List<PledgeDetailDto>>(pledgeDetails);

                    // Map pledge to PledgeDto and include pledge details
                    var pledgeDto = _mapper.Map<PledgeDto>(pledge);
                    pledgeDto.pledgeDetail = pledgeDetailDtos;

                    // Create ProjectBackerDto
                    var projectBackerDto = new ProjectBackerDto
                    {
                        backerId = user.UserId,
                        backerName = user.Fullname,
                        backerAvatar = user.Avatar ?? string.Empty, // Assuming `Avatar` is a property in the User entity
                        pledge = pledgeDto
                    };

                    projectBackerDtos.Add(projectBackerDto);
                }

                response.Data = projectBackerDtos;
                response.Success = true;
                response.Message = "Pledges retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve pledges: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> ExportPledgeToExcelByProjectId(int projectId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var pledges = await _unitOfWork.PledgeRepo.GetPledgeByProjectIdAsync(projectId);
                if (pledges == null || !pledges.Any())
                {
                    response.Success = false;
                    response.Message = "No pledges found for the specified project.";
                    return response;
                }

                // Fetch rewards for the project
                var rewards = await _unitOfWork.RewardRepo.GetRewardsByProjectIdAsync(projectId);
                if (rewards == null || !rewards.Any())
                {
                    response.Success = false;
                    response.Message = "No rewards found for the specified project.";
                    return response;
                }

                // Sort rewards by amount in ascending order for easier comparison
                var sortedRewards = rewards.OrderBy(r => r.Amount).ToList();

                // Dictionary to cache user data and reduce redundant calls
                var userCache = new Dictionary<int, (string Username, string Email)>();

                // Calculate total amount and total backers
                var totalAmount = pledges.Sum(p => p.Amount);
                var totalBackers = pledges.Select(p => p.UserId).Distinct().Count();

                // Count the total number of each type of reward
                var rewardCounts = new Dictionary<int, int>();
                foreach (var reward in sortedRewards)
                {
                    rewardCounts[reward.RewardId] = 0;
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Pledges");
                    var currentRow = 1;

                    // Add headers
                    worksheet.Cell(currentRow, 1).Value = "Pledge ID";
                    worksheet.Cell(currentRow, 2).Value = "Username";
                    worksheet.Cell(currentRow, 3).Value = "Email";
                    worksheet.Cell(currentRow, 4).Value = "Amount";
                    worksheet.Cell(currentRow, 5).Value = "Status";
                    worksheet.Cell(currentRow, 6).Value = "Reward";

                    // Style headers
                    var headerRange = worksheet.Range(currentRow, 1, currentRow, 6);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Add pledge data
                    foreach (var pledge in pledges)
                    {
                        currentRow++;

                        // Get or retrieve user data (username & email)
                        if (!userCache.TryGetValue(pledge.UserId, out var userData))
                        {
                            var user = await _unitOfWork.UserRepo.GetByIdAsync(pledge.UserId);
                            var username = user?.Fullname ?? "Unknown";
                            var email = user?.Email ?? "Unknown";
                            userData = (username, email);
                            userCache[pledge.UserId] = userData;
                        }

                        worksheet.Cell(currentRow, 1).Value = pledge.PledgeId;
                        worksheet.Cell(currentRow, 2).Value = userData.Username;
                        worksheet.Cell(currentRow, 3).Value = userData.Email;
                        worksheet.Cell(currentRow, 4).Value = pledge.Amount;

                        var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                        var status = pledgeDetails.FirstOrDefault()?.Status;
                        worksheet.Cell(currentRow, 5).Value = status.ToString();

                        // Find the highest reward less than or equal to the pledge amount
                        var reward = sortedRewards.LastOrDefault(r => r.Amount <= pledge.Amount);
                        worksheet.Cell(currentRow, 6).Value = reward?.Details ?? "No Reward";

                        // Increment reward count if a reward is found
                        if (reward != null)
                        {
                            rewardCounts[reward.RewardId]++;
                        }
                    }

                    // Style data rows
                    var dataRange = worksheet.Range(2, 1, currentRow, 6);
                    dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Add summary information
                    currentRow += 2;
                    worksheet.Cell(currentRow, 1).Value = "Export Date:";
                    worksheet.Cell(currentRow, 2).Value = DateTime.Now.ToString("yyyy-MM-dd");

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Total Amount:";
                    worksheet.Cell(currentRow, 2).Value = totalAmount;

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Total Backers:";
                    worksheet.Cell(currentRow, 2).Value = totalBackers;

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Reward Summary:";
                    foreach (var reward in sortedRewards)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = reward.Details; // Replace with actual reward property
                        worksheet.Cell(currentRow, 2).Value = rewardCounts[reward.RewardId];
                    }

                    // Style summary section
                    var summaryRange = worksheet.Range(currentRow - sortedRewards.Count - 3, 1, currentRow, 2);
                    summaryRange.Style.Font.Bold = true;
                    summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    summaryRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


                    // Auto-adjust column widths
                    worksheet.Columns().AdjustToContents();

                    // Save the workbook to a memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        var base64 = Convert.ToBase64String(content);
                        response.Data = base64;
                    }
                }

                response.Success = true;
                response.Message = "Pledges exported to Excel successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to export pledges to Excel: {ex.Message}";
            }
            return response;
        }



    }
}
