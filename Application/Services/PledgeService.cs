using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.PledgeDTO;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using ClosedXML.Excel;
using Domain.Entities;

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
                    pledgeDto.PledgeDetails = pledgeDetailDtos;
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
                var pledges = await _unitOfWork.PledgeRepo.GetPledgesByUserIdAsync(userId);

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
                    pledgeDto.PledgeDetails = pledgeDetailDtos;
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
                pledgeDto.PledgeDetails = pledgeDetailDtos;

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
        public async Task<ServiceResponse<List<ProjectBackerForAdminDto>>> GetBackerByProjectIdForAdmin(int projectId)
        {
            var response = new ServiceResponse<List<ProjectBackerForAdminDto>>();
            try
            {
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }
                var pledges = await _unitOfWork.PledgeRepo.GetPledgesByProjectIdAsync(projectId);
                if (!pledges.Any() || pledges == null)
                {
                    response.Success = false;
                    response.Message = "No pledges found for the specified user and project.";
                    return response;
                }

                var backerPledge = new List<ProjectBackerForAdminDto>();
                foreach (var pledge in pledges)
                {
                    // Fetch backer (user) details
                    var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", pledge.UserId);
                    if (user == null)
                    {
                        continue; // Skip if user not found
                    }

                    // Fetch pledge details
                    var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                    //var pledgeDetailDtos = _mapper.Map<List<PledgeDetailDto>>(pledgeDetails);
                    var projectBackerDetailDtos = _mapper.Map<List<PledgeDetailDto>>(pledgeDetails);
                    // Map pledge to PledgeDto and include pledge details
                    //var pledgeDto = _mapper.Map<PledgeDto>(pledge);
                    //pledgeDto.PledgeDetails = pledgeDetailDtos;

                    // Create ProjectBackerDto
                    var projectBackerDto = new ProjectBackerForAdminDto
                    {
                        BackerId = user.UserId,
                        BackerName = user.Fullname,
                        BackerAvatar = user.Avatar ?? string.Empty,
                        //pledge = pledgeDto,
                        TotalAmount = pledge.TotalAmount,
                        PledgeDetailDtos = projectBackerDetailDtos
                    };

                    backerPledge.Add(projectBackerDto);
                }
                response.Data = backerPledge;
                response.Success = true;
                response.Message = "Backers retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get project backers: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<List<ProjectBackerDto>>> GetBackerByProjectId(int projectId)
        {
            var response = new ServiceResponse<List<ProjectBackerDto>>();
            try
            {
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }

                var pledges = await _unitOfWork.PledgeRepo.GetPledgesByProjectIdAsync(projectId);

                if (pledges == null || !pledges.Any())
                {
                    response.Success = false;
                    response.Message = "No pledges found for the specified user and project.";
                    return response;
                }

                pledges.RemoveAll(p => p.UserId == existingProject.CreatorId || p.TotalAmount <= 0);

                // Prepare the result list
                var projectBackerDtos = new List<ProjectBackerDto>();

                foreach (var pledge in pledges)
                {
                    // Fetch backer (user) details
                    var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", pledge.UserId);
                    if (user == null)
                    {
                        continue; // Skip if user not found
                    }

                    // Fetch pledge details
                    var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                    //var pledgeDetailDtos = _mapper.Map<List<PledgeDetailDto>>(pledgeDetails);
                    var projectBackerDetailDtos = _mapper.Map<List<ProjectBackerDetailDto>>(pledgeDetails);
                    // Map pledge to PledgeDto and include pledge details
                    //var pledgeDto = _mapper.Map<PledgeDto>(pledge);
                    //pledgeDto.PledgeDetails = pledgeDetailDtos;

                    // Create ProjectBackerDto
                    var projectBackerDto = new ProjectBackerDto
                    {
                        BackerId = user.UserId,
                        BackerName = user.Fullname,
                        BackerAvatar = user.Avatar ?? string.Empty,
                        //pledge = pledgeDto,
                        TotalAmount = pledge.TotalAmount,
                        ProjectBackerDetails = projectBackerDetailDtos
                    };

                    projectBackerDtos.Add(projectBackerDto);
                }

                response.Data = projectBackerDtos;
                response.Success = true;
                response.Message = "Backers retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve backers: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> ExportPledgeToExcelByProjectId(int projectId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var pledges = await _unitOfWork.PledgeRepo.GetPledgesByProjectIdAsync(projectId);
                if (pledges == null || !pledges.Any())
                {
                    response.Success = false;
                    response.Message = "No pledges found for the specified project.";
                    return response;
                }

                // Fetch rewards for the project (can be empty)
                var rewards = await _unitOfWork.RewardRepo.GetRewardsByProjectIdAsync(projectId) ?? new List<Reward>();
                var sortedRewards = rewards.OrderBy(r => r.Amount).ToList();

                // Dictionary to cache user data and reduce redundant calls
                var userCache = new Dictionary<int, (string Username, string Email)>();

                // Calculate total amount and total backers
                var totalAmount = pledges.Sum(p => p.TotalAmount);
                var totalBackers = pledges.Select(p => p.UserId).Distinct().Count();

                // Initialize reward counts
                var rewardCounts = sortedRewards.ToDictionary(r => r.RewardId, r => 0);

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

                        // Get or retrieve user data
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
                        worksheet.Cell(currentRow, 4).Value = pledge.TotalAmount;

                        var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                        var status = pledgeDetails.FirstOrDefault()?.Status;
                        worksheet.Cell(currentRow, 5).Value = status.ToString();

                        // Find the highest reward less than or equal to the pledge amount
                        var reward = sortedRewards.LastOrDefault(r => r.Amount <= pledge.TotalAmount);
                        worksheet.Cell(currentRow, 6).Value = reward?.Details ?? "No Reward";

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

                    // Optional reward summary
                    if (sortedRewards.Any())
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = "Reward Summary:";
                        foreach (var reward in sortedRewards)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = reward.Details;
                            worksheet.Cell(currentRow, 2).Value = rewardCounts[reward.RewardId];
                        }

                        var summaryRange = worksheet.Range(currentRow - sortedRewards.Count, 1, currentRow, 2);
                        summaryRange.Style.Font.Bold = true;
                        summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        summaryRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    }

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
