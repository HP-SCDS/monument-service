namespace MonumentService.Repository
{
    using MonumentService.Model;

    public interface IMonumentRepository
    {
        IEnumerable<Monument> GetAll();

        IEnumerable<Monument> Get(Func<Monument, bool> filter);

        int AddOrUpdate(params Monument[] itemsToUpdate);
    }
}
