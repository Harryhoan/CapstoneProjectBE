using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.PlatformDTO;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PlatformService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<CreatePlatformDto>> CreatePlatform(int userId, CreatePlatformDto createPlatformDto)
        {
            var response = new ServiceResponse<CreatePlatformDto>();
            var user = await _unitOfWork.UserRepo.GetAllAsync();
            try
            {
                var platform = _mapper.Map<Platform>(createPlatformDto);
                await _unitOfWork.PlatformRepo.AddAsync(platform);
                
                response.Data = createPlatformDto;
                response.Success = true;
                response.Message = "Platform created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public Task<ServiceResponse<int>> DeletePlatform(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<IEnumerable<ViewPlatformDto>>> GetAllPlatform()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<ViewPlatformDto>> GetPlatformById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<ViewPlatformDto>> GetPlatformByProjectId(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<PaginationModel<ViewPlatformDto>>> GetPlatformsPaging(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<UpdatePlatformDto>> UpdatePlatform(int platformId, UpdatePlatformDto updateProjectDto)
        {
            throw new NotImplementedException();
        }
    }
}
