using Application.ServiceResponse;
using Application.ViewModels.CollaboratorDTO;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Application.IService
{
    public interface ICollaboratorService
    {
        public Task<ServiceResponse<CollaboratorDTO>> CreateCollaborator(CreateCollaboratorDTO createCollaboratorDTO, Domain.Entities.User user);
        public Task<ServiceResponse<CollaboratorDTO>> CreateCollaboratorByUserEmail(CreateCollaboratorByEmailDTO createCollaboratorDTO, Domain.Entities.User user);
        public Task<ServiceResponse<PaginationModel<CollaboratorDTO>>> GetPaginatedCollaborators(int page = 1, int pageSize = 20);
        public Task<ServiceResponse<List<CollaboratorDTO>>> GetCollaborators();
        public Task<ServiceResponse<PaginationModel<UserCollaboratorDTO>>> GetPaginatedCollaboratorsByProjectId(int projectId, int page = 1, int pageSize = 20, User? user = null);
        public Task<ServiceResponse<List<UserCollaboratorDTO>>> GetCollaboratorsByProjectId(int projectId, User? user = null);
        public Task<ServiceResponse<PaginationModel<ProjectCollaboratorDTO>>> GetPaginatedCollaboratorsByUserId(int userId, int page = 1, int pageSize = 20, User? user = null);
        public Task<ServiceResponse<List<ProjectCollaboratorDTO>>> GetCollaboratorsByUserId(int userId, User? user = null);
        public Task<IActionResult?> CheckIfUserHasPermissionsByProjectId(int projectId, User? user = null);
        public Task<IActionResult?> CheckIfUserCanCreateByProjectId(int userId, int projectId, User? user = null);
        public Task<IActionResult?> CheckIfUserCanRemoveByProjectId(int userId, int projectId, User? user = null);
        public Task<IActionResult?> CheckIfUserCanUpdateByProjectId(CollaboratorEnum role, int userId, int projectId, User? user = null);
        public Task<ServiceResponse<CollaboratorDTO>> UpdateCollaborator(CreateCollaboratorDTO createCollaboratorDTO);
        public Task<ServiceResponse<string>> RemoveCollaborator(int userId, int projectId);

    }
}
