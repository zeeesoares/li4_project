using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public interface IOrderService
    {
        Task<List<EncomendaModel>> GetAllOrdersAsync();
        Task<List<EncomendaModel>> GetOrdersByStatusAsync(int orderId);
        Task CreateOrderAsync(EncomendaModel newOrder);
        Task UpdateOrderAsync(EncomendaModel updatedOrder);
        Task DeleteOrderAsync(int orderId);
    }
}
