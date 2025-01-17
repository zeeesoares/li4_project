﻿using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public interface IOrderService
    {
        Task<List<EncomendaModel>> GetAllOrdersAsync();
        Task<List<EncomendaModel>> GetOrdersByStatusAsync(int orderId, int userId);
        Task CreateOrderAsync(EncomendaModel newOrder, List<DesktopEncomendaModel> products);
        Task UpdateOrderAsync(EncomendaModel updatedOrder);
        Task DeleteOrderAsync(int orderId);
        Task<List<DesktopModel>> GetProductsByOrderIdAsync(int orderId);
        Task<EncomendaModel> GetOrderByIdAsync(int idEncomenda);
        Task<List<DesktopEncomendaModel>> GetDesktopEncomendaByOrderIdAsync(int orderId);
    }
}
