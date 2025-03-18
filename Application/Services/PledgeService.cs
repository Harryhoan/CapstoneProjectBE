using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.PledgeDTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var pledgeDtos = _mapper.Map<IEnumerable<PledgeDto>>(pledges);
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
    }
}
