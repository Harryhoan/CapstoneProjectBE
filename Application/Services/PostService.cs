using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.PostDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        public async Task<ServiceResponse<int>> CreatePost(int userId, CreatePostDTO createPostDTO)
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

                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", createPostDTO.ProjectId);
                if (existingProject == null || existingProject.Status == ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }

                Post post = new Post();
                post.UserId = userId;
                post.ProjectId = createPostDTO.ProjectId;
                post.Title = createPostDTO.Title;
                post.Description = createPostDTO.Description;
                post.Status = createPostDTO.Status;
                //post.PostId = 0;
                post.CreatedDatetime = DateTime.UtcNow.AddHours(7);
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
                    if (user != null && user.Role == UserEnum.CUSTOMER)
                    {
                        if (post.Status == PostEnum.PRIVATE || post.Status == PostEnum.EXCLUSIVE)
                        {
                            var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                            if (existingCollaborator == null && user.UserId != post.UserId && user.UserId != existingProject.CreatorId)
                            {
                                response.Success = false;
                                response.Message = "Post not accessible";
                                return response;
                            }
                            else if (post.Status == PostEnum.EXCLUSIVE)
                            {
                                var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(user.UserId, existingProject.ProjectId);
                                if ((existingPledge == null || existingPledge.TotalAmount <= 0))
                                {
                                    response.Success = false;
                                    response.Message = "Post not accessible";
                                    return response;
                                }
                            }
                        }
                        else if (post.Status == PostEnum.DELETED)
                        {
                            response.Success = false;
                            response.Message = "Post not found";
                            return response;
                        }
                    }
                }
                else
                {
                    if (post.Status == PostEnum.EXCLUSIVE || post.Status == PostEnum.PRIVATE)
                    {
                        response.Success = false;
                        response.Message = "Post not accessible";
                        return response;
                    }
                    else if (post.Status == PostEnum.DELETED)
                    {
                        response.Success = false;
                        response.Message = "Post not found";
                        return response;
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
                if (user != null && user.Role == UserEnum.CUSTOMER)
                {
                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    if (existingCollaborator == null && existingProject.CreatorId != user.UserId)
                    {
                        if (existingProject.Status == ProjectStatusEnum.INVISIBLE)
                        {
                            return null;
                        }
                        posts.RemoveAll(p => p.Status == PostEnum.PRIVATE && p.UserId != userId);
                        var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(user.UserId, existingProject.ProjectId);
                        if ((existingPledge == null || existingPledge.TotalAmount <= 0))
                        {
                            posts.RemoveAll(p => p.Status == PostEnum.EXCLUSIVE && p.UserId != userId);
                        }
                    }

                    posts.RemoveAll(p => p.Status == PostEnum.DELETED);
                }
            }
            else if (existingProject.Status == ProjectStatusEnum.INVISIBLE)
            {
                return null;
            }
            else
            {
                posts.RemoveAll(p => p.Status == PostEnum.DELETED || p.Status == PostEnum.EXCLUSIVE || p.Status == PostEnum.PRIVATE || p.Project.Status == ProjectStatusEnum.INVISIBLE);
            }
            return posts;
        }

        private async Task<List<Post>?> FilterPostsByUserId(int userId, User? currentUser = null)
        {
            var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            var posts = await _unitOfWork.PostRepo.GetPostsByUserId(userId);

            if (existingUser == null)
            {
                await _unitOfWork.PostRepo.RemoveAll(posts);
                return null;
            }

            if (currentUser != null && currentUser.UserId > 0)
            {
                if (currentUser != null && currentUser.Role == UserEnum.CUSTOMER)
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
                            if (existingProject.Status == ProjectStatusEnum.INVISIBLE)
                            {
                                posts.RemoveAll(p => p.UserId != currentUser.UserId && p.ProjectId == existingProject.ProjectId);
                                continue;
                            }
                            if ((posts[i].Status == PostEnum.PRIVATE || posts[i].Status == PostEnum.EXCLUSIVE) && posts[i].UserId != currentUser.UserId)
                            {
                                posts.RemoveAt(i);
                                continue;
                            }
                        }

                        var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(currentUser.UserId, existingProject.ProjectId);
                        if ((existingPledge == null || existingPledge.TotalAmount <= 0) && existingCollaborator == null)
                        {
                            if (posts[i].Status == PostEnum.EXCLUSIVE)
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
                posts.RemoveAll(p => p.Status == PostEnum.DELETED || p.Status == PostEnum.EXCLUSIVE || p.Status == PostEnum.PRIVATE);
                int k = 0;
                while (k < posts.Count)
                {
                    var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", posts[k].ProjectId);
                    if (existingProject == null)
                    {
                        var temp = posts.Where(p => p.ProjectId == posts[k].ProjectId);
                        posts.RemoveAll(p => p.ProjectId == posts[k].ProjectId);
                        await _unitOfWork.PostRepo.RemoveAll(temp);
                        continue;
                    }

                    if (existingProject.Status == ProjectStatusEnum.INVISIBLE)
                    {
                        posts.RemoveAll(p => p.ProjectId == existingProject.ProjectId);
                        continue;
                    }
                    k++;
                }
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
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No post found";
                }
                else
                {
                    response.Message = "Retrieve post(s) successfully";
                }
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
                if (!response.Data.Any())
                {
                    response.Message = "No post found";
                }
                else
                {
                    response.Message = "Retrieve post(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get posts: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PaginationModel<PostDTO>>> GetPaginatedPostsByUserId(int userId, int page = 1, int pageSize = 20, User? currentUser = null)
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
                //    posts.RemoveAll(p => p.Status == PostEnum.DELETED);
                //}
                var posts = await FilterPostsByUserId(userId, currentUser);
                if (posts == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                var postDTOs = _mapper.Map<List<PostDTO>>(posts);
                response.Data = await Pagination.GetPagination(postDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No post found";
                }
                else
                {
                    response.Message = "Retrieve post(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get posts: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<PostDTO>>> GetPostsByUserId(int userId, User? currentUser)
        {
            var response = new ServiceResponse<List<PostDTO>>();

            try
            {
                var posts = await FilterPostsByUserId(userId, currentUser);
                if (posts == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                var postDTOs = _mapper.Map<List<PostDTO>>(posts);
                response.Data = postDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No post found";
                }
                else
                {
                    response.Message = "Retrieve post(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get posts: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<string>> UpdatePost(int postId, UpdatePostDTO updatePostDTO)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var validationContext = new ValidationContext(updatePostDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(updatePostDTO, validationContext, validationResults, true))
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
                //var existingProject = await _unitOfWork.ProjectRepo.GetByIdAsync(createPostDTO.ProjectId);
                //if (existingProject == null)
                //{
                //    response.Success = false;
                //    response.Message = "Project not found";
                //    return response;
                //}

                //existingPost.ProjectId = createPostDTO.ProjectId;
                existingPost.Title = updatePostDTO.Title;
                existingPost.Status = updatePostDTO.Status;
                existingPost.Description = updatePostDTO.Description;

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
                //existingPost.Status = PostEnum.DELETED;
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
                existingPost.Status = PostEnum.DELETED;
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

        public async Task<IActionResult?> CheckIfUserCanUpdateOrRemoveByPostId(int postId, User? user = null)
        {
            try
            {
                if (user == null || !(user.UserId > 0))
                {
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "Try logging in first." };
                    return new UnauthorizedObjectResult(result);
                }
                var existingPost = await _unitOfWork.PostRepo.GetByIdNoTrackingAsync("PostId", postId);
                if (existingPost == null || (existingPost.Status == PostEnum.DELETED && user.Role == UserEnum.CUSTOMER))
                {
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The post associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }

                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", existingPost.ProjectId);
                if (existingProject == null || (existingPost.Status == PostEnum.DELETED && user.Role == UserEnum.CUSTOMER))
                {
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The project associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }
                if (user.Role == UserEnum.CUSTOMER)
                {

                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    if (existingCollaborator == null || user.UserId != existingPost.UserId)
                    {
                        var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "The request is forbidden to the customer." };
                        return new ObjectResult(result);
                    }
                    existingCollaborator = null;

                }
                else if (user.Role == UserEnum.STAFF)
                {
                    if (existingProject.MonitorId > 0 && existingProject.MonitorId != user.UserId)
                    {
                        var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "The request is forbidden to the staff." };
                        return new ObjectResult(result);
                    }
                }
                return null;
            }
            catch
            {
            }
            return new BadRequestResult();
        }

        public async Task<IActionResult?> CheckIfUserHasPermissionsByPostId(int postId, User? user = null)
        {
            try
            {
                var existingPost = await _unitOfWork.PostRepo.GetByIdNoTrackingAsync("PostId", postId);
                if (existingPost == null)
                {
                    //return new NotFoundResult();
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The post associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }
                if (existingPost.Status != PostEnum.PUBLIC && (user == null || !(user.UserId > 0)))
                {
                    return new UnauthorizedResult();
                }

                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", existingPost.ProjectId);
                if (existingProject == null)
                {
                    //return new NotFoundResult();
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The project associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }
                if (existingPost.Status == PostEnum.PRIVATE || existingPost.Status == PostEnum.EXCLUSIVE)
                {
                    if (user == null || !(user.UserId > 0))
                    {
                        return new UnauthorizedResult();
                    }
                    else if (user.Role == UserEnum.CUSTOMER)
                    {

                        var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                        if (existingCollaborator == null && user.UserId != existingPost.UserId && user.UserId != existingProject.CreatorId)
                        {
                            existingCollaborator = null;
                            //return new ForbidResult();
                            var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "The request is forbidden to the customer." };
                            return new ObjectResult(result);
                        }

                        else if (existingPost.Status == PostEnum.EXCLUSIVE)
                        {
                            var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(user.UserId, existingProject.ProjectId);
                            if ((existingPledge == null || existingPledge.TotalAmount <= 0))
                            {
                                existingPledge = null;
                                //return new ForbidResult();
                                var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "The post is exclusive to backers only." };
                                return new ObjectResult(result);
                            }
                        }
                    }
                }
                else if (existingPost.Status == PostEnum.DELETED && (user == null || !(user.UserId > 0) || user.Role == UserEnum.CUSTOMER))
                {
                    //return new NotFoundResult();
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The post associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }
                return null;

            }
            catch
            {
            }
            return new BadRequestResult();
        }
    }

}
