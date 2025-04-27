using Application.ServiceResponse;
using Application.ViewModels.PostDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.IService
{
    public interface IPostService
    {
        public Task<ServiceResponse<int>> CreatePost(int userId, CreatePostDTO createCardDTO);
        public Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByUserId(int userId, int page = 1, int pageSize = 20, User? currentUser = null);
        public Task<ServiceResponse<List<PostDTO>>> GetPostsByUserId(int userId, User? currentUser = null);
        public Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByProjectId(int projectId, int page = 1, int pageSize = 20, int? userId = null);
        public Task<ServiceResponse<List<PostDTO>>> GetPostsByProjectId(int projectId, int? userId = null);
        public Task<ServiceResponse<string>> UpdatePost(int postId, UpdatePostDTO updatePostDTO);
        public Task<ServiceResponse<string>> RemovePost(int postId);
        public Task<ServiceResponse<string>> SoftRemovePost(int postId);
        public Task<IActionResult?> CheckIfUserCanUpdateOrRemoveByPostId(int postId, User? user = null);
        public Task<IActionResult?> CheckIfUserHasPermissionsByPostId(int postId, User? user = null);
        public Task<ServiceResponse<PostDTO>> GetPostById(int postId, int? userId = null);
    }
}
