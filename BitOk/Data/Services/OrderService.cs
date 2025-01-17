﻿using BitOk.Data.Models;
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

        public async Task<List<EncomendaModel>> GetOrdersByStatusAsync(int orderId, int userId)
        {
            string query = "SELECT * FROM Encomenda WHERE Estado_idEstado = @EstadoId AND Utilizador_idUtilizador = @UtilizadorId";

            var parameters = new { EstadoId = orderId, UtilizadorId = userId };

            var result = await _sqlDataAccess.LoadData<EncomendaModel, object>(query, parameters);
            return result;
        }

        public async Task<List<EncomendaModel>> GetOrdersByStatusAsyncAdmin(int orderId)
        {
            string query = "SELECT * FROM Encomenda WHERE Estado_idEstado = @EstadoId";

            var parameters = new { EstadoId = orderId};

            var result = await _sqlDataAccess.LoadData<EncomendaModel, object>(query, parameters);
            return result;
        }



        public async Task CreateOrderAsync(EncomendaModel newOrder, List<DesktopEncomendaModel> products)
        {
            string orderQuery = @"INSERT INTO Encomenda (Data_Inicio, Data_Fim, Estado_idEstado, Utilizador_idUtilizador)
                          VALUES (@DataInicio, @DataFim, @EstadoId, @UtilizadorId);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = new
            {
                DataInicio = newOrder.Data_Inicio,
                DataFim = newOrder.Data_Fim,
                EstadoId = newOrder.Estado_idEstado,
                UtilizadorId = newOrder.Utilizador_idUtilizador
            };

            int orderId = await _sqlDataAccess.ExecuteScalarAsync<int>(orderQuery, parameters);

            if (products != null && products.Any())
            {
                foreach (var product in products)
                {
                    product.Encomenda_idEncomenda = orderId;
                }
                await AddProductsToOrderAsync(orderId, products);
            }
        }


        public async Task AddProductsToOrderAsync(int orderId, List<DesktopEncomendaModel> products)
        {
            const string query = @"
        INSERT INTO Desktop_Encomendas (Encomenda_idEncomenda, Desktop_idDesktop, Quantidade_Prod, Estado)
        VALUES (@EncomendaId, @DesktopId, @QuantidadeProd, @Estado)";

            foreach (var product in products)
            {
                var parameters = new
                {
                    EncomendaId = orderId,
                    DesktopId = product.Desktop_idDesktop,
                    QuantidadeProd = product.Quantidade_Prod,
                    Estado = product.Estado
                };

                await _sqlDataAccess.SaveData(query, parameters);
            }
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
            string deleteDependenciesQuery = "DELETE FROM dbo.Desktop_Encomendas WHERE Encomenda_idEncomenda = @OrderId";
            var parameters = new { OrderId = orderId };
            await _sqlDataAccess.SaveData(deleteDependenciesQuery, parameters);

            string deleteOrderQuery = "DELETE FROM dbo.Encomenda WHERE idEncomenda = @Id";
            var deleteOrderParameters = new { Id = orderId };
            await _sqlDataAccess.SaveData(deleteOrderQuery, deleteOrderParameters);
        }
    }
}
