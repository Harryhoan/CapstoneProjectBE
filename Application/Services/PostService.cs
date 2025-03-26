using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.FileDTO;
using Application.ViewModels.PledgeDTO;
using Application.ViewModels.PostDTO;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
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

        public async Task<ServiceResponse<PostDTO>> GetPostById(int postId, int? userId = null)
        {
            var response = new ServiceResponse<PostDTO>();
            try
            {
                var post = await _unitOfWork.PostRepo.GetByIdAsync(postId);
                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", post.ProjectId);
                if (existingProject == null)
                {
                    await _unitOfWork.PostRepo.RemoveAsync(post);
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                if (userId != null && userId > 0)
                {
                    var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", (int)userId);
                    if (user != null && user.Role == "Customer")
                    {
                        if (post.Status == "Private" || post.Status == "Exclusive")
                        {
                            var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                            if (existingCollaborator == null && user.UserId != post.UserId && user.UserId != existingProject.CreatorId)
                            {
                                response.Success = false;
                                response.Message = "Post not accessible";
                                return response;
                            }
                            else if (post.Status == "Exclusive")
                            {
                                var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(user.UserId, existingProject.ProjectId);
                                if ((existingPledge == null || existingPledge.Amount <= 0))
                                {
                                    response.Success = false;
                                    response.Message = "Post not accessible";
                                    return response;
                                }
                            }
                        }
                        else if (post.Status == "Deleted")
                        {
                            response.Success = false;
                            response.Message = "Post not found";
                            return response;
                        }
                    }
                }
                response.Data = _mapper.Map<PostDTO>(post);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create post: {ex.Message}";
            }
            return response;

        }

        private async Task<List<Post>?> FilterPostsByProjectIdAndUserId(int projectId, int? userId = null)
        {
            var posts = await _unitOfWork.PostRepo.GetPostsByProjectId(projectId);
            var existingProject = await _unitOfWork.ProjectRepo.GetProjectById(projectId);
            if (existingProject == null)
            {
                await _unitOfWork.PostRepo.RemoveAll(posts);
                return null;
            }
            if (userId != null && userId > 0)
            {
                var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", (int)userId);
                if (user != null && user.Role == "Customer")
                {
                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    if (existingCollaborator == null && existingProject.CreatorId != user.UserId)
                    {
                        posts.RemoveAll(p => (p.Status == "Private" || p.Status == "Exclusive") && p.UserId != userId);
                    }
                    var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(user.UserId, existingProject.ProjectId);
                    if ((existingPledge == null || existingPledge.Amount <= 0) && existingCollaborator == null)
                    {
                        posts.RemoveAll(p => p.Status == "Exclusive");
                    }
                    posts.RemoveAll(p => p.Status == "Deleted");
                }
            }
            else
            {
                posts.RemoveAll(p => p.Status == "Deleted" || p.Status == "Exclusive" || p.Status == "Private");
            }
            return posts;
        }

        private async Task<List<Post>?> FilterPostsByUserId(int userId, int? currentUserId = null)
        {
            var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            var posts = await _unitOfWork.PostRepo.GetPostsByUserId(userId);

            if (existingUser == null)
            {
                await _unitOfWork.PostRepo.RemoveAll(posts);
                return null;
            }

            if (currentUserId != null && currentUserId > 0)
            {
                var currentUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", (int)currentUserId);
                if (currentUser != null && currentUser.Role == "Customer")
                {
                    int i = 0;
                    while (i < posts.Count)
                    {
                        var projectId = posts[i].ProjectId;
                        var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);

                        if (existingProject == null)
                        {
                            var temp = posts.Where(p => p.ProjectId == projectId);
                            posts.RemoveAll(p => p.ProjectId == projectId);
                            await _unitOfWork.PostRepo.RemoveAll(temp);
                            continue;
                        }

                        var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(currentUser.UserId, existingProject.ProjectId);
                        if (existingCollaborator == null && existingProject.CreatorId != currentUser.UserId)
                        {
                            if ((posts[i].Status == "Private" || posts[i].Status == "Exclusive") && posts[i].UserId != currentUser.UserId)
                            {
                                posts.RemoveAt(i);
                                continue;
                            }
                        }

                        var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(currentUser.UserId, existingProject.ProjectId);
                        if ((existingPledge == null || existingPledge.Amount <= 0) && existingCollaborator == null)
                        {
                            if (posts[i].Status == "Exclusive")
                            {
                                posts.RemoveAt(i);
                                continue;
                            }
                        }
                        i++;
                    }
                }
            }
            else
            {
                posts.RemoveAll(p => p.Status == "Deleted" || p.Status == "Exclusive" || p.Status == "Private");
            }

            return posts;
        }

        public async Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByProjectId(int projectId, int page = 1, int pageSize = 20, int? userId = null)
        {
            var response = new ServiceResponse<PaginationModel<PostDTO>>();

            try
            {
                var posts = await FilterPostsByProjectIdAndUserId(projectId, userId);
                if (posts == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
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

        public async Task<ServiceResponse<List<PostDTO>>> GetPostsByProjectId(int projectId, int? userId = null)
        {
            var response = new ServiceResponse<List<PostDTO>>();

            try
            {
                var posts = await FilterPostsByProjectIdAndUserId(projectId, userId);
                if (posts == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
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

        public async Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByUserId(int userId, int page = 1, int pageSize = 20, int? currentUserId = null)
        {
            var response = new ServiceResponse<PaginationModel<PostDTO>>();

            try
            {
                //var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                //var posts = await _unitOfWork.PostRepo.GetPostsByUserId(userId);
                //if (existingUser == null)
                //{
                //    foreach (var post in posts)
                //    {
                //        await _unitOfWork.PostRepo.Remove(post);
                //    }
                //    response.Success = false;
                //    return response;
                //}
                //if (!includeDeleted)
                //{
                //    posts.RemoveAll(p => p.Status == "Deleted");
                //}
                var posts = await FilterPostsByUserId(userId, currentUserId);
                if (posts == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

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

        public async Task<ServiceResponse<List<PostDTO>>> GetPostsByUserId(int userId, int? currentUserId = null)
        {
            var response = new ServiceResponse<List<PostDTO>>();

            try
            {
                var posts = await FilterPostsByUserId(userId, currentUserId);
                if (posts == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

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

        public async Task<ServiceResponse<string>> SoftRemovePost(int postId)
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
                //await _unitOfWork.PostRepo.Remove(existingPost);
                existingPost.Status = "Deleted";
                await _unitOfWork.PostRepo.UpdateAsync(existingPost);
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

        public async Task<bool> CheckIfPostHasUserId(int postId, int userId)
        {
            try
            {
                var existingPost = await _unitOfWork.PostRepo.GetByIdAsync(postId);
                return existingPost != null && existingPost.UserId == userId;
            }
            catch
            {
            }

            return false;

        }

        public async Task<IActionResult?> CheckIfUserHasPermissionsByPostId(User user, int postId)
        {
            try
            {
                if (user == null || !(user.UserId > 0))
                {
                    return new UnauthorizedObjectResult("This request is not authorized.");
                }
                var existingPost = await _unitOfWork.PostRepo.GetByIdNoTrackingAsync("PostId", postId);
                if (existingPost == null)
                {
                    return new NotFoundObjectResult("The requested post cannot be found.");
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", existingPost.ProjectId);
                if (existingProject == null)
                {
                    return new NotFoundObjectResult("The project associated with the requested post cannot be found.");
                }
                if (user.Role == "Customer")
                {
                    if (existingPost.Status == "Private" || existingPost.Status == "Exclusive")
                    {
                        var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                        if (existingCollaborator == null && user.UserId != existingPost.UserId && user.UserId != existingProject.CreatorId)
                        {
                            existingCollaborator = null;
                            return new ForbidResult("This request is forbidden.");
                        }

                        else if (existingPost.Status == "Exclusive")
                        {
                            var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(user.UserId, existingProject.ProjectId);
                            if ((existingPledge == null || existingPledge.Amount <= 0))
                            {
                                existingPledge = null;
                                return new ForbidResult("This request is forbidden.");
                            }
                        }
                    }
                    else if (existingPost.Status == "Deleted")
                    {
                        return new NotFoundObjectResult("The requested post cannot be found.");
                    }
                    return null;
                }
            }
            catch
            {
            }
            return new BadRequestObjectResult("This request cannot be processed.");
        }
    }

}
