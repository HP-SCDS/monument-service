namespace MonumentService.Repository
{
    using MonumentService.Model;
    using System.Linq.Expressions;

    public interface IMonumentRepository
    {
        IEnumerable<Monument> GetAll();

        IEnumerable<Monument> Get(Expression<Func<Monument, bool>> filter);

        void Add(params Monument[] itemsToAdd);

        void Update(params Monument[] itemsToUpdate);

        int Delete(params Monument[] itemsToDelete);

        int Delete(Expression<Func<Monument, bool>> filter);
    }
}
