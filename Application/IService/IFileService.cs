using Application.ServiceResponse;
using Application.ViewModels.FileDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.IService
{
    public interface IFileService
    {
        //public Task<ServiceResponse<PaginationModel<FileDTO>>> GetPaginatedFilesByUserId(int userId, int page = 1, int pageSize = 20, Domain.Entities.User? user = null);
        //public Task<ServiceResponse<List<FileDTO>>> GetFilesByUserId(int userId, Domain.Entities.User? user = null);
        public Task<IActionResult> GetFileById(int fileId, Domain.Entities.User? user = null);
        public Task<IActionResult> GetPaginatedFiles(int page = 1, int pageSize = 20, Domain.Entities.User? user = null);
        public Task<IActionResult> GetFiles(Domain.Entities.User? user = null);
        public Task<IActionResult> GetPaginatedFilesByUserId(int userId, int page = 1, int pageSize = 20, Domain.Entities.User? user = null);
        public Task<IActionResult> GetFilesByUserId(int userId, Domain.Entities.User? user = null);
        public Task<ServiceResponse<List<FileDTO>>> CreateFiles(int userId, List<IFormFile> formFiles);
        public Task<IActionResult?> CheckIfUserHasPermissionsByFileId(int fileId, Domain.Entities.User? user = null);
        public Task<ServiceResponse<string>> UpdateFile(UpdateFileDTO updateFileDTO);
        public Task<ServiceResponse<string>> RemoveFile(int fileId);
        public Task<ServiceResponse<string>> SoftRemoveFile(int fileId);
    }
}
