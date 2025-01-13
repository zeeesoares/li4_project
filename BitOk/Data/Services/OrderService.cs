using BitOk.Data.Models;
using BitOk.Data;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitOk.Data.Services
{
    public class OrderService : IOrderService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public OrderService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<EncomendaModel>> GetAllOrdersAsync()
        {
            string query = "SELECT * FROM dbo.Encomenda";
            return await _sqlDataAccess.LoadData<EncomendaModel, object>(query, new { });
        }

        public async Task<List<EncomendaModel>> GetOrdersByStatusAsync(int orderId)
        {
            string query = "SELECT * FROM Encomenda WHERE Estado_idEstado = @Id";
            var parameters = new { Id = orderId };
            var result = await _sqlDataAccess.LoadData<EncomendaModel, object>(query, parameters);
            return result; 
        }

        public async Task CreateOrderAsync(EncomendaModel newOrder)
        {
            string query = @"INSERT INTO dbo.Encomenda (Data_Inicio, Data_Fim, Estado_idEstado, Utilizador_idUtilizador)
                             VALUES (@DataInicio, @DataFim, @EstadoId, @UtilizadorId)";
            var parameters = new
            {
                newOrder.Data_Inicio,
                newOrder.Data_Fim,
                EstadoId = newOrder.Estado_idEstado,
                UtilizadorId = newOrder.Utilizador_idUtilizador
            };
            await _sqlDataAccess.SaveData(query, parameters);
        }

        public async Task UpdateOrderAsync(EncomendaModel updatedOrder)
        {
            string query = @"UPDATE Encomenda
                             SET Data_Inicio = @DataInicio, Data_Fim = @DataFim, Estado_idEstado = @EstadoId, Utilizador_idUtilizador = @UtilizadorId
                             WHERE idEncomenda = @Id";
            var parameters = new
            {
                updatedOrder.Data_Inicio,
                updatedOrder.Data_Fim,
                EstadoId = updatedOrder.Estado_idEstado,
                UtilizadorId = updatedOrder.Utilizador_idUtilizador,
                Id = updatedOrder.idEncomenda
            };
            await _sqlDataAccess.SaveData(query, parameters);
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            string query = "DELETE FROM Encomenda WHERE idEncomenda = @Id";
            var parameters = new { Id = orderId };
            await _sqlDataAccess.SaveData(query, parameters);
        }
    }
}
