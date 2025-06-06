﻿using Application.ServiceResponse;
using Application.ViewModels.CommentDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.IService
{
    public interface ICommentService
    {
        public Task<ServiceResponse<int>> CreatePostComment(CreatePostCommentDTO createPostCommentDTO, User user);
        public Task<ServiceResponse<int>> CreateProjectComment(CreateProjectCommentDTO createProjectCommentDTO, User user);
        public Task<ServiceResponse<List<CommentDTO>>> GetComments();
        public Task<ServiceResponse<PaginationModel<CommentDTO>>> GetPaginatedCommentsByProjectId(int projectId, int page = 1, int pageSize = 20);
        public Task<ServiceResponse<List<CommentDTO>>> GetCommentsByProjectId(int projectId);
        public Task<ServiceResponse<PaginationModel<CommentDTO>>> GetPaginatedCommentsByPostId(int postId, int page = 1, int pageSize = 20);
        public Task<ServiceResponse<List<CommentDTO>>> GetCommentsByPostId(int postId);
        public Task<ServiceResponse<PaginationModel<CommentDTO>>> GetPaginatedCommentsByUserId(int userId, int page = 1, int pageSize = 20);
        public Task<ServiceResponse<List<CommentDTO>>> GetCommentsByUserId(int userId);
        public Task<ServiceResponse<int>> GetCommentCountByPostId(int postId, User? user = null);
        public Task<ServiceResponse<int>> GetCommentCountByProjectId(int projectId, User? user = null);
        public Task<ServiceResponse<string>> UpdateComment(UpdateCommentDTO updateCommentDTO);
        public Task<ServiceResponse<string>> RemoveComment(int commentId);
        public Task<ServiceResponse<string>> SoftRemoveComment(int commentId);
        public Task<bool> CheckIfCommentHasUserId(int commentId, int userId);
        public Task<IActionResult?> CheckIfUserHasPermissionsByPostId(User user, int postId);
        public Task<IActionResult?> CheckIfUserHasPermissionsByProjectId(User user, int projectId);
    }
}