using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public interface IPecaService
    {
        Task<List<PecaModel>> GetAllPecas();
        Task AddPeca(PecaModel novaPeca);
        Task UpdatePeca(PecaModel peca);
        Task DeletePeca(int id);
        Task UpdateQuantidade(PecaModel peca);

        Task<bool> CheckStockForProductsAsync(List<DesktopEncomendaModel> products);
    }
}