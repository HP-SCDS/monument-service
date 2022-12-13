namespace MonumentService.Repository
{
    public interface IFacetsRepository
    {
        IEnumerable<string> GetAllProvincias();

        int AddOrUpdateProvincias(params string[] itemsToUpdate);

        IEnumerable<string> GetAllTiposMonumento();

        int AddOrUpdateTiposMonumento(params string[] itemsToUpdate);

        IEnumerable<string> GetAllTiposConstruccion();

        int AddOrUpdateTiposConstruccion(params string[] itemsToUpdate);

        IEnumerable<string> GetAllClasificaciones();

        int AddOrUpdateClasificaciones(params string[] itemsToUpdate);

        IEnumerable<string> GetAllPeriodosHistoricos();

        int AddOrUpdatePeriodosHistoricos(params string[] itemsToUpdate);
    }
}
