using System.Data.SqlClient;
using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public interface IOrderServiceBackground
    {
        Task UpdateOrderAsync(EncomendaModel updatedOrder, SqlConnection connection);
        Task<List<DesktopEncomendaModel>> GetDesktopEncomendaByOrderIdAsync(int orderId, SqlConnection connection);
        Task<bool> UpdateProductStateAsync(int encomendaId, int desktopId, string novoEstado, SqlConnection connection);
    }
}
