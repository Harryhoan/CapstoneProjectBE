using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.ReportDTO;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<ReportDto>> CreateReportAsync(int userId, CreateReportDto report)
        {
            var response = new ServiceResponse<ReportDto>();
            try
            {
                var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                // Check the number of reports created by the user in the last 5 minutes
                var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
                var recentReports = await _unitOfWork.ReportRepo.GetReportsByUserIdAndTimeAsync(userId, fiveMinutesAgo);

                const int maxReportsAllowed = 5; // Set the limit
                if (recentReports.Count() >= maxReportsAllowed)
                {
                    response.Success = false;
                    response.Message = $"You can only create up to {maxReportsAllowed} reports within 5 minutes.";
                    return response;
                }

                var newReport = new Report
                {
                    UserId = existingUser.UserId,
                    Detail = report.Detail,
                    CreateDatetime = DateTime.Now
                };
                await _unitOfWork.ReportRepo.AddAsync(newReport);

                var ResponseReportDto = new ReportDto
                {
                    ReportId = newReport.ReportId,
                    UserName = existingUser.Fullname,
                    Detail = newReport.Detail,
                    CreateDatetime = newReport.CreateDatetime
                };

                response.Data = ResponseReportDto;
                response.Success = true;
                response.Message = "Report created successfully";

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create response: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<IEnumerable<Report>>> GetAllReportAsync()
        {
            var response = new ServiceResponse<IEnumerable<Report>>();
            try
            {
                var report = await _unitOfWork.ReportRepo.GetAllAsNoTrackingAsync();

                response.Data = report;
                response.Success = true;
                response.Message = "Get all report successfully.";

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get all report: {ex.Message}";
                return response;
            }
        }
        public async Task<ServiceResponse<ReportDto>> GetReportByIdAsync(int userId, int reportId)
        {
            var response = new ServiceResponse<ReportDto>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                var report = await _unitOfWork.ReportRepo.GetByIdNoTrackingAsync("ReportId", reportId);
                if (report == null)
                {
                    response.Success = false;
                    response.Message = "Report not found.";
                    return response;
                }
                var reportUser = await _unitOfWork.UserRepo.GetByIdAsync(report.UserId);
                if (reportUser == null)
                {
                    response.Success = false;
                    response.Message = "Report user not found.";
                    return response;
                }
                var reportDto = _mapper.Map<ReportDto>(report);
                reportDto.UserName = reportUser.Fullname;
                response.Data = reportDto;
                response.Success = true;
                response.Message = "Get report by id successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get report by id: {ex.Message}";
                return response;
            }
        }
        public async Task<ServiceResponse<IEnumerable<ReportDto>>> GetReportByUserIdAsync(int userId)
        {
            var response = new ServiceResponse<IEnumerable<ReportDto>>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var reports = await _unitOfWork.ReportRepo.GetReportsByUserIdAsync(userId);

                if (!reports.Any())
                {
                    response.Success = false;
                    response.Message = "No reports found for this user.";
                    return response;
                }

                var reportDtos = _mapper.Map<List<ReportDto>>(reports);

                // Populate UserName field manually since it's not available in Report entity
                foreach (var reportDto in reportDtos)
                {
                    reportDto.UserName = user.Fullname;
                }

                response.Data = reportDtos;
                response.Success = true;
                response.Message = "Get report by user id successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get report by user id: {ex.Message}";
            }

            return response;
        }

    }
}
