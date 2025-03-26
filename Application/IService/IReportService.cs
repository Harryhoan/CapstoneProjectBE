using Application.ServiceResponse;
using Application.ViewModels.ReportDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IReportService
    {
        public Task<ServiceResponse<ReportDto>> CreateReportAsync(int userId, CreateReportDto report);
        public Task<ServiceResponse<IEnumerable<Report>>> GetAllReportAsync();
        public Task<ServiceResponse<IEnumerable<ReportDto>>> GetReportByUserIdAsync(int userId);
        public Task<ServiceResponse<ReportDto>> GetReportByIdAsync(int userId, int reportId);
        //public Task<ServiceResponse<int>> DeleteReportAsync(int reportId);
    }
}
