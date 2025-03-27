using Application.ServiceResponse;
using Application.ViewModels.PostDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IPostService
    {
        public Task<ServiceResponse<int>> CreatePost(int userId, CreatePostDTO createCardDTO);
        public Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByUserId(int userId, int page = 1, int pageSize = 20, int? currentUserId = null);
        public Task<ServiceResponse<List<PostDTO>>> GetPostsByUserId(int userId, int? currentUserId = null);
        public Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByProjectId(int projectId, int page = 1, int pageSize = 20, int? userId = null);
        public Task<ServiceResponse<List<PostDTO>>> GetPostsByProjectId(int projectId, int? userId = null);
        public Task<ServiceResponse<string>> UpdatePost(int postId, CreatePostDTO updatePostDTO);
        public Task<ServiceResponse<string>> RemovePost(int postId);
        public Task<ServiceResponse<string>> SoftRemovePost(int postId);
        public Task<bool> CheckIfPostHasUserId(int postId, int userId);
        public Task<IActionResult?> CheckIfUserHasPermissionsByPostId(User user, int postId);
        public Task<ServiceResponse<PostDTO>> GetPostById(int postId, int? userId = null);
    }
}
