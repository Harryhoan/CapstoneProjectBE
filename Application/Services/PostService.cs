using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
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
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<int>> CreatePost(CreatePostDTO createPostDTO)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var validationContext = new ValidationContext(createPostDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createPostDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(createPostDTO.UserId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdAsync(createPostDTO.ProjectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }

                Post post = new Post();
                post.UserId = createPostDTO.UserId;
                post.ProjectId = createPostDTO.ProjectId;
                post.Title = createPostDTO.Title;
                post.Description = createPostDTO.Description;
                post.Status = createPostDTO.Status;
                post.PostId = 0;
                post.CreatedDatetime = DateTime.Now;
                await _unitOfWork.PostRepo.AddAsync(post);
                response.Data = post.PostId;
                response.Success = true;
                response.Message = "Post created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create post: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByProjectId(int projectId, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<PostDTO>>();

            try
            {
                var posts = await _unitOfWork.PostRepo.GetPostsByProjectId(projectId);
                var postDTOs = _mapper.Map<List<PostDTO>>(posts);
                response.Data = await Pagination.GetPagination(postDTOs, page, pageSize);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get posts: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<PostDTO>>> GetPostsByProjectId(int projectId)
        {
            var response = new ServiceResponse<List<PostDTO>>();

            try
            {
                var posts = await _unitOfWork.PostRepo.GetPostsByProjectId(projectId);
                var postDTOs = _mapper.Map<List<PostDTO>>(posts);
                response.Data = postDTOs;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get posts: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByUserId(int userId, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<PostDTO>>();

            try
            {
                var posts = await _unitOfWork.PostRepo.GetPostsByUserId(userId);
                var postDTOs = _mapper.Map<List<PostDTO>>(posts);
                response.Data = await Pagination.GetPagination(postDTOs, page, pageSize);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get posts: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<PostDTO>>> GetPostsByUserId(int userId)
        {
            var response = new ServiceResponse<List<PostDTO>>();

            try
            {
                var posts = await _unitOfWork.PostRepo.GetPostsByUserId(userId);
                var postDTOs = _mapper.Map<List<PostDTO>>(posts);
                response.Data = postDTOs;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get posts: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<string>> UpdatePost(int postId, CreatePostDTO createPostDTO)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var validationContext = new ValidationContext(createPostDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createPostDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingPost = await _unitOfWork.PostRepo.GetByIdAsync(postId);
                if (existingPost == null)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdAsync(createPostDTO.ProjectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }

                existingPost.ProjectId = createPostDTO.ProjectId;
                existingPost.Title = createPostDTO.Title;
                existingPost.Status = createPostDTO.Status;
                existingPost.Description = createPostDTO.Description;

                await _unitOfWork.PostRepo.UpdateAsync(existingPost);
                response.Data = "Post updated successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update post: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> RemovePost(int postId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var existingPost = await _unitOfWork.PostRepo.GetByIdAsync(postId);
                if (existingPost == null)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    return response;
                }
                await _unitOfWork.PostRepo.RemoveAsync(existingPost);
                //existingPost.Status = "Deleted";
                //await _unitOfWork.PostRepo.Update(existingPost);
                response.Data = "Post removed successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove post: {ex.Message}";
            }

            return response;
        }

    }
}
