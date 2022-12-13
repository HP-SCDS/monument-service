namespace MonumentService.Repository
{
    using MonumentService.Model;
    using System.Linq.Expressions;

    public interface IMonumentRepository
    {
        IEnumerable<Monument> GetAll();

        IEnumerable<Monument> Get(Expression<Func<Monument, bool>> filter);

        int Add(params Monument[] itemsToAdd);

        int Update(params Monument[] itemsToUpdate);

        int AddOrUpdate(params Monument[] itemsToUpdate);

        int Delete(params Monument[] itemsToDelete);

        int Delete(Expression<Func<Monument, bool>> filter);
    }
}
