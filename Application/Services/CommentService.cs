using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.CommentDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<int>> CreatePostComment(CreatePostCommentDTO createPostCommentDTO, User user)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var validationContext = new ValidationContext(createPostCommentDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createPostCommentDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                //var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                //if (existingUser == null)
                //{
                //    response.Success = false;
                //    response.Message = "User not found";
                //    return response;
                //}
                var existingPost = await _unitOfWork.PostRepo.GetByIdNoTrackingAsync("PostId", createPostCommentDTO.PostId);
                if (existingPost == null || existingPost.Status == PostEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    return response;
                }
                if (createPostCommentDTO.ParentCommentId != null)
                {
                    var existingParentComment = await _unitOfWork.CommentRepo.GetByIdNoTrackingAsync("CommentId", (int)createPostCommentDTO.ParentCommentId);
                    if (existingParentComment == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment not found";
                        return response;
                    }
                    var existingPostComment = await _unitOfWork.PostCommentRepo.FindEntityAsync(pc => pc.CommentId == existingParentComment.CommentId);
                    if (existingPostComment == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment not found in any post";
                        return response;
                    }
                    if (existingPostComment.PostId != existingPost.PostId)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment found in another post";
                        return response;
                    }

                }

                Domain.Entities.Comment comment = new()
                {
                    UserId = user.UserId,
                    Content = createPostCommentDTO.Content,
                    CommentId = 0,
                    Status = "Created",
                    ParentCommentId = createPostCommentDTO.ParentCommentId,
                    CreatedDatetime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)
                };
                comment.UpdatedDatetime = comment.CreatedDatetime;
                await _unitOfWork.CommentRepo.AddAsync(comment);
                if (comment.CommentId <= 0)
                {
                    response.Success = false;
                    response.Message = "Comment cannot be created";
                    return response;
                }

                PostComment postComment = new()
                {
                    PostId = existingPost.PostId,
                    CommentId = comment.CommentId
                };
                await _unitOfWork.PostCommentRepo.AddAsync(postComment);
                response.Data = comment.CommentId;
                response.Success = true;
                response.Message = "Comment created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create comment: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<int>> CreateProjectComment(CreateProjectCommentDTO createProjectCommentDTO, User user)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var validationContext = new ValidationContext(createProjectCommentDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createProjectCommentDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                //var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                //if (existingUser == null)
                //{
                //    response.Success = false;
                //    response.Message = "User not found";
                //    return response;
                //}
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", createProjectCommentDTO.ProjectId);
                if (existingProject == null || existingProject.Status == ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                if (createProjectCommentDTO.ParentCommentId != null)
                {
                    var existingParentComment = await _unitOfWork.CommentRepo.GetByIdNoTrackingAsync("CommentId", (int)createProjectCommentDTO.ParentCommentId);
                    if (existingParentComment == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment not found";
                        return response;
                    }
                    var existingProjectComment = await _unitOfWork.ProjectCommentRepo.FindEntityAsync(pc => pc.CommentId == existingParentComment.CommentId);
                    if (existingProjectComment == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment not found in any project";
                        return response;
                    }
                    if (existingProjectComment.ProjectId != existingProject.ProjectId)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment found in another project";
                        return response;
                    }
                }

                Domain.Entities.Comment comment = new()
                {
                    UserId = user.UserId,
                    Content = createProjectCommentDTO.Content,
                    CommentId = 0,
                    Status = "Created",
                    ParentCommentId = createProjectCommentDTO.ParentCommentId,
                    CreatedDatetime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)
                };
                comment.UpdatedDatetime = comment.CreatedDatetime;
                await _unitOfWork.CommentRepo.AddAsync(comment);
                if (comment.CommentId <= 0)
                {
                    response.Success = false;
                    response.Message = "Comment cannot be created";
                    return response;
                }
                ProjectComment projectComment = new()
                {
                    ProjectId = existingProject.ProjectId,
                    CommentId = comment.CommentId
                };
                await _unitOfWork.ProjectCommentRepo.AddAsync(projectComment);

                response.Data = comment.CommentId;
                response.Success = true;
                response.Message = "Comment created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create comment: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<int>> GetCommentCountByPostId(int postId, User? user = null)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var existingPost = await _unitOfWork.PostRepo.GetByIdNoTrackingAsync("PostId", postId);
                if (existingPost == null)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    return response;
                }

                int count = 0;
                if (user == null || user.Role == UserEnum.CUSTOMER)
                {
                    count = await _unitOfWork.CommentRepo.Count(c => c.PostComment != null && c.PostComment.PostId == postId && !c.Status.ToLower().Trim().Contains("deleted"));
                }
                else
                {
                    count = await _unitOfWork.CommentRepo.Count(c => c.PostComment != null && c.PostComment.PostId == postId);
                }
                response.Success = true;
                response.Data = count;
                response.Message = "Retrieve comment count successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create comment: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<List<CommentDTO>>> GetComments()
        {
            var response = new ServiceResponse<List<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetAllAsNoTrackingAsync();
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = commentDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No comment found.";
                }
                else
                {
                    response.Message = "Retrieve comment(s) successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get comments: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<int>> GetCommentCountByProjectId(int projectId, User? user = null)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                int count = 0;
                if (user == null || user.Role == UserEnum.CUSTOMER)
                {
                    count = await _unitOfWork.CommentRepo.Count(c => c.ProjectComment != null && c.ProjectComment.ProjectId == projectId && !c.Status.ToLower().Trim().Contains("deleted"));
                }
                else
                {
                    count = await _unitOfWork.CommentRepo.Count(c => c.ProjectComment != null && c.ProjectComment.ProjectId == projectId);
                }

                response.Data = count;
                response.Message = "Retrieve comment count successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create comment: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<PaginationModel<CommentDTO>>> GetPaginatedCommentsByProjectId(int projectId, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetCommentsWithCommentsByProjectId(projectId);
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = await Pagination.GetPagination(commentDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No comment found.";
                }
                else
                {
                    response.Message = "Retrieve comment(s) successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get comments: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<CommentDTO>>> GetCommentsByProjectId(int projectId)
        {
            var response = new ServiceResponse<List<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetCommentsWithCommentsByProjectId(projectId);
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = commentDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No comment found.";
                }
                else
                {
                    response.Message = "Retrieve comment(s) successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get comments: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PaginationModel<CommentDTO>>> GetPaginatedCommentsByPostId(int postId, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetCommentsWithCommentsByPostId(postId);
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = await Pagination.GetPagination(commentDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No comment found.";
                }
                else
                {
                    response.Message = "Retrieve comment(s) successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get comments: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<CommentDTO>>> GetCommentsByPostId(int userId)
        {
            var response = new ServiceResponse<List<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetCommentsWithCommentsByPostId(userId);
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = commentDTOs;
                response.Success = true;
                if (response.Data.Count <= 0)
                {
                    response.Message = "No comment found.";
                }
                else
                {
                    response.Message = "Retrieve comment(s) successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get comments: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<PaginationModel<CommentDTO>>> GetPaginatedCommentsByUserId(int userId, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetCommentsByUserId(userId);
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = await Pagination.GetPagination(commentDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No comment found.";
                }
                else
                {
                    response.Message = "Retrieve comment(s) successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get comments: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<CommentDTO>>> GetCommentsByUserId(int userId)
        {
            var response = new ServiceResponse<List<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetCommentsByUserId(userId);
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = commentDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No comment found.";
                }
                else
                {
                    response.Message = "Retrieve comment(s) successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get comments: {ex.Message}";
            }
            return response;
        }


        public async Task<ServiceResponse<string>> UpdateComment(UpdateCommentDTO updateCommentDTO)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var validationContext = new ValidationContext(updateCommentDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(updateCommentDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingComment = await _unitOfWork.CommentRepo.GetByIdAsync(updateCommentDTO.CommentId);
                if (existingComment == null)
                {
                    response.Success = false;
                    response.Message = "Comment not found";
                    return response;
                }

                existingComment.Content = updateCommentDTO.Content;
                existingComment.UpdatedDatetime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
                existingComment.Status = "Updated";
                await _unitOfWork.CommentRepo.UpdateAsync(existingComment);
                response.Data = "Comment updated successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update comment: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> RemoveComment(int commentId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var existingComment = await _unitOfWork.CommentRepo.GetByIdAsync(commentId);
                if (existingComment == null)
                {
                    response.Success = false;
                    response.Message = "Comment not found";
                    return response;
                }

                await DeleteChildComments(existingComment);

                var existingProjectComment = await _unitOfWork.ProjectCommentRepo.GetProjectCommentByCommentId(commentId);
                if (existingProjectComment != null)
                {
                    await _unitOfWork.ProjectCommentRepo.RemoveAsync(existingProjectComment);
                }
                var existingPostComment = await _unitOfWork.PostCommentRepo.GetPostCommentByCommentId(commentId);
                if (existingPostComment != null)
                {
                    await _unitOfWork.PostCommentRepo.RemoveAsync(existingPostComment);
                }

                await _unitOfWork.CommentRepo.RemoveAsync(existingComment);

                response.Data = "Comment removed successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove comment: {ex.Message}";
            }

            return response;
        }

        private async Task DeleteChildComments(Domain.Entities.Comment parentComment)
        {
            var childComments = await _unitOfWork.CommentRepo.GetCommentsByParentCommentId(parentComment.CommentId);

            if (childComments != null && childComments.Any())
            {
                foreach (var childComment in childComments)
                {
                    await DeleteChildComments(childComment);
                }

                var existingProjectComment = await _unitOfWork.ProjectCommentRepo.GetProjectCommentByCommentId(parentComment.CommentId);
                if (existingProjectComment != null)
                {
                    await _unitOfWork.ProjectCommentRepo.RemoveAsync(existingProjectComment);
                }
                var existingPostComment = await _unitOfWork.PostCommentRepo.GetPostCommentByCommentId(parentComment.CommentId);
                if (existingPostComment != null)
                {
                    await _unitOfWork.PostCommentRepo.RemoveAsync(existingPostComment);
                }

                await _unitOfWork.CommentRepo.RemoveAsync(parentComment);
            }
        }
        public async Task<ServiceResponse<string>> SoftRemoveComment(int commentId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var existingComment = await _unitOfWork.CommentRepo.GetByIdAsync(commentId);
                if (existingComment == null)
                {
                    response.Success = false;
                    response.Message = "Comment not found";
                    return response;
                }
                existingComment.Status = "Deleted";
                await _unitOfWork.CommentRepo.UpdateAsync(existingComment);
                response.Data = "Comment removed successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove comment: {ex.Message}";
            }

            return response;
        }
        public async Task<bool> CheckIfCommentHasUserId(int commentId, int userId)
        {
            try
            {
                var existingComment = await _unitOfWork.CommentRepo.GetByIdNoTrackingAsync("CommentId", commentId);
                return existingComment != null && existingComment.UserId == userId;
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
                var existingPost = await _unitOfWork.PostRepo.GetByIdNoTrackingAsync("PostId", postId);
                if (existingPost == null || (existingPost.Status == PostEnum.DELETED && user.Role == UserEnum.CUSTOMER))
                {
                    //return new NotFoundResult();
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The post associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }

                return await CheckIfUserHasPermissionsByProjectId(user, existingPost.ProjectId);
            }
            catch
            {
            }
            return new BadRequestResult();
        }


        public async Task<IActionResult?> CheckIfUserHasPermissionsByProjectId(User user, int projectId)
        {
            try
            {
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null)
                {
                    //return new NotFoundResult();
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The project associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }
                if (user.Role == UserEnum.CUSTOMER)
                {

                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    if (existingCollaborator == null && user.UserId != existingProject.CreatorId)
                    {
                        existingCollaborator = null;
                        var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(user.UserId, existingProject.ProjectId);
                        if ((existingPledge == null || existingPledge.TotalAmount <= 0))
                        {
                            existingPledge = null;
                            //return new ForbidResult();
                            var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "The customer cannot comment without becoming a backer or a collaborator." };
                            return new BadRequestObjectResult(result);
                        }

                    }
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
