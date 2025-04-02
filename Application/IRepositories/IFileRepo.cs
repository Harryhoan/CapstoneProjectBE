namespace Application.IRepositories
{
    public interface IFileRepo : IGenericRepo<Domain.Entities.File>
    {
        public Task<List<Domain.Entities.File>> GetFilesByUserId(int userId);
    }
}
