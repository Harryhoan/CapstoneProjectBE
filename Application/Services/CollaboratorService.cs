using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.PostDTO;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CollaboratorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<int>> CreateCollaborator(int userId, int projectId, Domain.Entities.User user)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                if (existingProject.CreatorId != user.UserId)
                {
                    var userCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectIdAsNoTracking(user.UserId, existingProject.ProjectId);
                    if (userCollaborator == null || userCollaborator.Role != "Administrator")
                    {
                        response.Success = false;
                        response.Message = "This request is not permitted";
                        return response;
                    }
                }
                var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(existingUser.UserId, existingProject.ProjectId);
                if (existingCollaborator != null)
                {
                    response.Success = false;
                    response.Message = "Collaborator already exists";
                    return response;
                }
                Collaborator collaborator = new Collaborator();
                collaborator.ProjectId = existingProject.ProjectId;
                collaborator.UserId = existingUser.UserId;
                await _unitOfWork.CollaboratorRepo.AddAsync(collaborator);
                response.Success = true;
                response.Message = "Collaborator created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create collaborator: {ex.Message}";
            }
            return response;
        }

    }
}
