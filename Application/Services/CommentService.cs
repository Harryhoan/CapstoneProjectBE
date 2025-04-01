using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.CommentDTO;
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
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<int>> CreatePostComment(CreatePostCommentDTO createPostCommentDTO, int userId)
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

                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var existingPost = await _unitOfWork.PostRepo.GetByIdAsync(createPostCommentDTO.PostId);
                if (existingPost == null)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    return response;
                }
                if (createPostCommentDTO.ParentCommentId != null)
                {
                    var existingParentComment = await _unitOfWork.CommentRepo.GetByIdAsync((int)createPostCommentDTO.ParentCommentId);
                    if (existingParentComment == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment not found";
                        return response;
                    }
                }

                Comment comment = new Comment();
                comment.UserId = existingUser.UserId;
                comment.Content = createPostCommentDTO.Content;
                comment.CommentId = 0;
                comment.Status = "Created";
                comment.ParentCommentId = createPostCommentDTO.ParentCommentId;
                comment.CreatedDatetime = DateTime.UtcNow;
                comment.UpdatedDatetime = DateTime.UtcNow;
                await _unitOfWork.CommentRepo.AddAsync(comment);
                if (comment.CommentId <= 0)
                {
                    response.Success = false;
                    response.Message = "Comment cannot be created";
                    return response;
                }

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
        
        public async Task<ServiceResponse<int>> CreateProjectComment(CreateProjectCommentDTO createProjectCommentDTO, int userId)
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

                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var existingPost = await _unitOfWork.ProjectRepo.GetByIdAsync(createProjectCommentDTO.ProjectId);
                if (existingPost == null)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    return response;
                }
                if (createProjectCommentDTO.ParentCommentId != null)
                {
                    var existingParentComment = await _unitOfWork.CommentRepo.GetByIdAsync((int)createProjectCommentDTO.ParentCommentId);
                    if (existingParentComment == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Comment not found";
                        return response;
                    }
                }

                Comment comment = new Comment();
                comment.UserId = existingUser.UserId;
                comment.Content = createProjectCommentDTO.Content;
                comment.CommentId = 0;
                comment.Status = "Created";
                comment.ParentCommentId = createProjectCommentDTO.ParentCommentId;
                comment.CreatedDatetime = DateTime.UtcNow;
                comment.UpdatedDatetime = DateTime.UtcNow;
                await _unitOfWork.CommentRepo.AddAsync(comment);
                if (comment.CommentId <= 0)
                {
                    response.Success = false;
                    response.Message = "Comment cannot be created";
                    return response;
                }

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


        public async Task<ServiceResponse<PaginationModel<CommentDTO>>> GetPaginatedCommentsByProjectId(int projectId, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<CommentDTO>>();

            try
            {
                var comments = await _unitOfWork.CommentRepo.GetCommentsWithCommentsByProjectId(projectId);
                var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
                response.Data = await Pagination.GetPagination(commentDTOs, page, pageSize);
                response.Success = true;
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
                existingComment.UpdatedDatetime = DateTime.UtcNow;
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
                //existingComment.Status = "Deleted";
                //await _unitOfWork.CommentRepo.Update(existingComment);
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


    }
}
