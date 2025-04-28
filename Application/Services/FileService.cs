using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.FileDTO;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;
        public FileService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        //public async Task<ServiceResponse<PaginationModel<FileDTO>>> GetPaginatedFilesByUserId(int userId, int page = 1, int pageSize = 20, Domain.Entities.User? user = null)
        //{
        //    var response = new ServiceResponse<PaginationModel<FileDTO>>();

        //    try
        //    {
        //        var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(userId);
        //        var files = await _unitOfWork.FileRepo.GetFilesByUserId(userId);
        //        if (existingUser == null)
        //        {
        //            await _unitOfWork.FileRepo.RemoveAll(files);
        //            response.Success = false;
        //            response.Message = "User not found";
        //            return response;
        //        }

        //        if (user == null || user.Role == UserEnum.CUSTOMER)
        //        {
        //            files.RemoveAll(f => f.Status == "Deleted");
        //        }

        //        var fileDTOs = _mapper.Map<List<FileDTO>>(files);
        //        response.Data = await Pagination.GetPagination(fileDTOs, page, pageSize);
        //        response.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = $"Failed to get files: {ex.Message}";
        //    }
        //    return response;
        //}

        public async Task<IActionResult> GetPaginatedFiles(int page = 1, int pageSize = 20, Domain.Entities.User? user = null)
        {
            var response = new ServiceResponse<PaginationModel<FileDTO>>();

            try
            {
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Try logging in";
                    return new UnauthorizedObjectResult(response);
                }
                var files = (await _unitOfWork.FileRepo.GetAllAsNoTrackingAsync()).OrderByDescending(f => f.CreatedDatetime).ToList();
                if (user.Role == UserEnum.CUSTOMER)
                {
                    files.RemoveAll(f => f.Status != "Deleted");
                }
                var fileDTOs = _mapper.Map<List<FileDTO>>(files);
                response.Data = await Pagination.GetPagination(fileDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No file found";
                }
                else
                {
                    response.Message = "Retrieve file(s) successfully";
                }
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get files: {ex.Message}";
                return new BadRequestObjectResult(response);
            }

        }

        public async Task<IActionResult> GetFiles(Domain.Entities.User? user = null)
        {
            var response = new ServiceResponse<IList<FileDTO>>();

            try
            {
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Try logging in";
                    return new UnauthorizedObjectResult(response);
                }
                var files = (await _unitOfWork.FileRepo.GetAllAsNoTrackingAsync()).OrderByDescending(f => f.CreatedDatetime).ToList();
                if (user.Role == UserEnum.CUSTOMER)
                {
                    files.RemoveAll(f => f.Status != "Deleted");
                }
                var fileDTOs = _mapper.Map<IList<FileDTO>>(files);
                response.Data = fileDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No file found";
                }
                else
                {
                    response.Message = "Retrieve file(s) successfully";
                }
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get files: {ex.Message}";
                return new BadRequestObjectResult(response);
            }

        }

        public async Task<IActionResult> GetPaginatedFilesByUserId(int userId, int page = 1, int pageSize = 20, Domain.Entities.User? user = null)
        {
            var response = new ServiceResponse<PaginationModel<FileDTO>>();

            try
            {
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Try logging in";
                    return new UnauthorizedObjectResult(response);
                }
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                var files = (await _unitOfWork.FileRepo.GetFilesByUserId(userId)).OrderByDescending(f => f.CreatedDatetime).ToList();
                if (existingUser == null)
                {
                    await _unitOfWork.FileRepo.RemoveAll(files);
                    response.Success = false;
                    response.Message = "User not found";
                    return new NotFoundObjectResult(response);
                }

                if (user.Role == UserEnum.CUSTOMER)
                {
                    if (user.UserId != existingUser.UserId)
                    {
                        return new ForbidResult();
                    }
                    files.RemoveAll(f => f.Status == "Deleted");
                }
                var fileDTOs = _mapper.Map<List<FileDTO>>(files);
                response.Data = await Pagination.GetPagination(fileDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No file found";
                }
                else
                {
                    response.Message = "Retrieve file(s) successfully";
                }
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get files: {ex.Message}";
                return new BadRequestObjectResult(response);
            }

        }

        public async Task<IActionResult> GetFilesByUserId(int userId, Domain.Entities.User? user = null)
        {
            var response = new ServiceResponse<IList<FileDTO>>();

            try
            {
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Try logging in";
                    return new UnauthorizedObjectResult(response);
                }
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                var files = (await _unitOfWork.FileRepo.GetFilesByUserId(userId)).OrderByDescending(f => f.CreatedDatetime).ToList();
                if (existingUser == null)
                {
                    await _unitOfWork.FileRepo.RemoveAll(files);
                    response.Success = false;
                    response.Message = "User not found";
                    return new NotFoundObjectResult(response);
                }

                if (user.Role == UserEnum.CUSTOMER)
                {
                    if (user.UserId != existingUser.UserId)
                    {
                        return new ForbidResult();
                    }
                    files.RemoveAll(f => f.Status == "Deleted");
                }
                var fileDTOs = _mapper.Map<List<FileDTO>>(files);
                response.Data = fileDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No file found";
                }
                else
                {
                    response.Message = "Retrieve file(s) successfully";
                }
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get files: {ex.Message}";
                return new BadRequestObjectResult(response);
            }
        }

        public async Task<IActionResult> GetFileById(int fileId, Domain.Entities.User? user = null)
        {
            var response = new ServiceResponse<FileDTO>();
            try
            {

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Try logging in";
                    return new UnauthorizedObjectResult(response);
                }

                var existingFile = await _unitOfWork.FileRepo.GetByIdNoTrackingAsync("FileId", fileId);
                if (existingFile == null || (user.Role == UserEnum.CUSTOMER && existingFile.Status == "Deleted"))
                {
                    response.Success = false;
                    response.Message = "File not found";
                    return new NotFoundObjectResult(response);
                }

                if (user.Role == UserEnum.CUSTOMER && user.UserId != existingFile.UserId)
                {
                    return new ForbidResult();
                }

                var fileDTO = _mapper.Map<FileDTO>(existingFile);
                response.Data = fileDTO;
                response.Success = true;
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get files: {ex.Message}";
                return new BadRequestObjectResult(response);
            }
        }

        public async Task<ServiceResponse<List<FileDTO>>> CreateFiles(int userId, List<IFormFile> formFiles)
        {
            var response = new ServiceResponse<List<FileDTO>>();

            try
            {
                if (formFiles == null || formFiles.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No files provided.";
                    return response;
                }

                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                var fileDTOs = new List<FileDTO>();

                foreach (var formFile in formFiles)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = formFile.OpenReadStream())
                        {
                            RawUploadParams uploadParams = new();

                            if (IsImage(formFile.ContentType))
                            {
                                uploadParams = new ImageUploadParams
                                {
                                    File = new FileDescription(formFile.FileName, stream),
                                    Transformation = new Transformation().Crop("fill").Gravity("face")
                                };
                            }
                            else if (IsVideo(formFile.ContentType))
                            {
                                uploadParams = new VideoUploadParams
                                {
                                    File = new FileDescription(formFile.FileName, stream),
                                };
                            }
                            else if (IsAudio(formFile.ContentType))
                            {
                                uploadParams = new RawUploadParams
                                {
                                    File = new FileDescription(formFile.FileName, stream),
                                };
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "Invalid file type. Please upload either an image, video, GIF, or audio file.";
                                return response;
                            }

                            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                            if (uploadResult.SecureUrl != null)
                            {
                                var file = new Domain.Entities.File
                                {
                                    Status = "Uploaded",
                                    Source = uploadResult.Url.ToString().Trim(),
                                    UserId = existingUser.UserId,
                                    CreatedDatetime = DateTime.UtcNow.AddHours(7)
                                };
                                await _unitOfWork.FileRepo.AddAsync(file);

                                fileDTOs.Add(_mapper.Map<FileDTO>(file));
                            }
                        }
                    }
                }

                response.Data = fileDTOs;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to add files: {ex.Message}";
            }
            return response;
        }

        private static bool IsImage(string contentType)
        {
            return contentType.StartsWith("image/") || contentType.Equals("image/gif", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsVideo(string contentType)
        {
            return contentType.StartsWith("video/") || contentType.Equals("video/webm", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAudio(string contentType)
        {
            return contentType.StartsWith("audio/");
        }

        public async Task<IActionResult?> CheckIfUserHasPermissionsByFileId(int fileId, Domain.Entities.User? user = null)
        {
            if (user == null || !(user.UserId > 0))
            {
                return new UnauthorizedResult();
            }
            if (user.IsDeleted || !user.IsVerified)
            {
                //return new ForbidResult();
                var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "This account is either deleted or unverified." };
                return new ObjectResult(result);
            }
            var existingFile = await _unitOfWork.FileRepo.GetByIdNoTrackingAsync("FileId", fileId);
            if (existingFile == null || (user.Role == UserEnum.CUSTOMER && existingFile.Status == "Deleted"))
            {
                //return new NotFoundResult();
                var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "This file associated with the request cannot be found." };
                return new NotFoundObjectResult(result);
            }
            return null;
        }

        public async Task<ServiceResponse<string>> UpdateFile(UpdateFileDTO updateFileDTO)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var validationContext = new ValidationContext(updateFileDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(updateFileDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingFile = await _unitOfWork.FileRepo.GetByIdAsync(updateFileDTO.FileId);
                if (existingFile == null)
                {
                    response.Success = false;
                    response.Message = "File not found";
                    return response;
                }

                existingFile.Status = updateFileDTO.Status;
                existingFile.Source = updateFileDTO.Source;
                await _unitOfWork.FileRepo.UpdateAsync(existingFile);
                response.Data = "File updated successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update file: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> RemoveFile(int fileId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var existingFile = await _unitOfWork.FileRepo.GetByIdAsync(fileId);
                if (existingFile == null)
                {
                    response.Success = false;
                    response.Message = "File not found";
                    return response;
                }

                var publicId = ExtractPublicIdFromUrl(existingFile.Source);
                if (string.IsNullOrWhiteSpace(publicId))
                {
                    response.Success = false;
                    response.Message = "Failed to extract public ID from file URL";
                    return response;
                }

                var deletionParams = new DeletionParams(publicId);
                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                if (deletionResult.Error != null)
                {
                    response.Success = false;
                    response.Message = $"Failed to delete file from Cloudinary: {deletionResult.Error.Message}";
                    return response;
                }

                await _unitOfWork.FileRepo.RemoveAsync(existingFile);

                response.Data = "File removed successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove file: {ex.Message}";
            }

            return response;
        }

        private static string? ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            Uri uri = new Uri(url);
            var path = uri.AbsolutePath;
            var segments = path.Split('/');

            return segments[segments.Length - 1].Split('.')[0];
        }

        public async Task<ServiceResponse<string>> SoftRemoveFile(int fileId)
        {
            var response = new ServiceResponse<string>();

            try
            {

                var existingFile = await _unitOfWork.FileRepo.GetByIdAsync(fileId);
                if (existingFile == null)
                {
                    response.Success = false;
                    response.Message = "File not found";
                    return response;
                }
                existingFile.Status = "Deleted";

                await _unitOfWork.FileRepo.UpdateAsync(existingFile);
                response.Data = "File removed successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove file: {ex.Message}";
            }

            return response;
        }

    }
}
