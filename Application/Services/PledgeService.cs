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

                // Dictionary to cache user data and reduce redundant calls
                var userCache = new Dictionary<int, (string Username, string Email)>();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Pledges");
                    var currentRow = 1;

                    // Add headers
                    worksheet.Cell(currentRow, 1).Value = "Pledge ID";
                    worksheet.Cell(currentRow, 2).Value = "Username";
                    worksheet.Cell(currentRow, 3).Value = "Email"; // New column for Email
                    worksheet.Cell(currentRow, 4).Value = "Amount";
                    worksheet.Cell(currentRow, 5).Value = "Status";

                    // Add pledge data
                    foreach (var pledge in pledges)
                    {
                        currentRow++;

                        // Get or retrieve user data (username & email)
                        if (!userCache.TryGetValue(pledge.UserId, out var userData))
                        {
                            var user = await _unitOfWork.UserRepo.GetByIdAsync(pledge.UserId);
                            var username = user?.Fullname ?? "Unknown"; // Replace with actual username property
                            var email = user?.Email ?? "Unknown"; // Replace with actual email property
                            userData = (username, email);
                            userCache[pledge.UserId] = userData;
                        }

                        worksheet.Cell(currentRow, 1).Value = pledge.PledgeId;
                        worksheet.Cell(currentRow, 2).Value = userData.Username; // Use fetched username
                        worksheet.Cell(currentRow, 3).Value = userData.Email; // Use fetched email
                        worksheet.Cell(currentRow, 4).Value = pledge.Amount;

                        var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                        var status = pledgeDetails.FirstOrDefault()?.Status;
                        worksheet.Cell(currentRow, 5).Value = status.ToString();
                    }

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
