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

        public async Task<List<EncomendaModel>> GetOrdersByStatusAsync(int estadoId, int userId)
        {
            string query = "SELECT * FROM Encomenda WHERE Estado_idEstado = @EstadoId AND Utilizador_idUtilizador = @UtilizadorId";

            var parameters = new { EstadoId = estadoId, UtilizadorId = userId };

            var result = await _sqlDataAccess.LoadData<EncomendaModel, object>(query, parameters);
            return result;
        }

        public async Task<List<EncomendaModel>> GetOrdersByStatusAsyncAdmin(int estadoId)
        {
            string query = "SELECT * FROM Encomenda WHERE Estado_idEstado = @EstadoId";

            var parameters = new { EstadoId = estadoId};

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
                DataInicio = updatedOrder.Data_Inicio,
                DataFim = updatedOrder.Data_Fim,
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

        public async Task<List<DesktopModel>> GetProductsByOrderIdAsync(int orderId)
        {
            string getProductIdsQuery = "SELECT Desktop_idDesktop FROM dbo.Desktop_Encomendas WHERE Encomenda_idEncomenda = @OrderId";
            var parameters = new { OrderId = orderId };
            var productIds = await _sqlDataAccess.LoadData<int, dynamic>(getProductIdsQuery, parameters);

            if (productIds == null || !productIds.Any())
                return new List<DesktopModel>();

            string getProductDetailsQuery = "SELECT * FROM dbo.Desktop WHERE idDesktop IN @ProductIds";
            var productDetails = await _sqlDataAccess.LoadData<DesktopModel, dynamic>(
                getProductDetailsQuery,
                new { ProductIds = productIds });

            return productDetails;
        }

        public async Task<List<DesktopEncomendaModel>> GetDesktopEncomendaByOrderIdAsync(int orderId)
        {
        string query = @"
            SELECT 
            de.Encomenda_idEncomenda,
            de.Desktop_idDesktop,
            de.Quantidade_Prod,
            COALESCE(de.Estado, 'Espera') AS Estado, 
            e.idEncomenda,
            e.Data_Inicio,
            d.idDesktop,
            d.Categoria,
            d.Descricao,
            d.Preco
            FROM dbo.Desktop_Encomendas de
            LEFT JOIN dbo.Encomenda e ON de.Encomenda_idEncomenda = e.idEncomenda
            LEFT JOIN dbo.Desktop d ON de.Desktop_idDesktop = d.idDesktop
            WHERE de.Encomenda_idEncomenda = @OrderId";

            var parameters = new { OrderId = orderId };

            var rawData = await _sqlDataAccess.LoadData<dynamic, dynamic>(query, parameters);

            var result = rawData.SelectMany(item => Enumerable.Range(0, (int)item.Quantidade_Prod) 
                .Select(_ => new DesktopEncomendaModel
                {
                    Encomenda_idEncomenda = item.Encomenda_idEncomenda,
                    Desktop_idDesktop = item.Desktop_idDesktop,
                    Quantidade_Prod = 1,
                    Estado = item.Estado,
                    Encomenda = new EncomendaModel
                    {
                        idEncomenda = item.idEncomenda,
                        Data_Inicio = item.Data_Inicio
                    },
                    Desktop = new DesktopModel
                    {
                        idDesktop = item.idDesktop,
                        Categoria = item.Categoria,
                        Descricao = item.Descricao,
                        Preco = item.Preco
                    }
                })).ToList();

            return result ?? new List<DesktopEncomendaModel>();
        }

        public async Task<DesktopEncomendaModel> GetDesktopEncomendaByIdAsync(int desktopId, int encomendaId)
        {
            string query = @"
                SELECT 
                *
                FROM dbo.Desktop_Encomendas
                WHERE Desktop_idDesktop = @DesktopId AND Encomenda_idEncomenda = @EncomendaId";

            var parameters = new { DesktopId = desktopId, EncomendaId = encomendaId };

            var result = await _sqlDataAccess.LoadData<DesktopEncomendaModel, object>(query, parameters).ContinueWith(task => task.Result.FirstOrDefault());
            return result;
        }


        public async Task<EncomendaModel> GetOrderByIdAsync(int idEncomenda)
        {
            var sqlEncomenda = @"
            SELECT 
                e.idEncomenda, 
                e.Data_Inicio, 
                e.Data_Fim, 
                e.Estado_idEstado, 
                e.Utilizador_idUtilizador
            FROM Encomenda e
            WHERE e.idEncomenda = @idEncomenda";

            var parameters = new { idEncomenda };
            var encomenda = await _sqlDataAccess.LoadData<EncomendaModel, dynamic>(sqlEncomenda, parameters);

            if (encomenda != null && encomenda.Any())
            {
                var encomendaResult = encomenda.First();

                var sqlEstado = @"
                SELECT Nome
                FROM Estado
                WHERE idEstado = @Estado_idEstado";

                var estadoParameters = new { Estado_idEstado = encomendaResult.Estado_idEstado };
                var estadoResult = await _sqlDataAccess.LoadData<Estado, dynamic>(sqlEstado, estadoParameters);

                if (estadoResult != null && estadoResult.Any())
                {
                    encomendaResult.Estado = new Estado
                    {
                        Id = encomendaResult.Estado_idEstado,
                        Nome = estadoResult.First().Nome
                    };
                }

                return encomendaResult;
            }

            return null;
        }

        public async Task<bool> UpdateProductStateAsync(int encomendaId, int desktopId, string novoEstado)
        {
            string query = @"
                UPDATE dbo.Desktop_Encomendas
                SET Estado = @NovoEstado
                WHERE Encomenda_idEncomenda = @EncomendaId
                  AND Desktop_idDesktop = @DesktopId";

            var parameters = new { EncomendaId = encomendaId, DesktopId = desktopId, NovoEstado = novoEstado };

            try
            {
                await _sqlDataAccess.SaveData(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<PecaModel>> GetPecasWithoutStockAsync(int encomendaId)
        {
            string query = @"
                WITH QuantidadeCalc AS (
                    SELECT 
                        p.idPeca, 
                        p.Nome, 
                        p.Stock, 
                        (pd.Quantidade * de.Quantidade) AS QuantidadeNecessaria
                    FROM Pecas_Desktop pd
                    JOIN Desktop_Encomendas de ON pd.Desktop_idDesktop = de.Desktop_idDesktop
                    JOIN Peca p ON pd.Peca_idPeca = p.idPeca
                    WHERE de.Encomenda_idEncomenda = @EncomendaId
                )
                SELECT idPeca, Nome, Stock, QuantidadeNecessaria
                FROM QuantidadeCalc
                WHERE Stock < QuantidadeNecessaria";

            var parameters = new { EncomendaId = encomendaId };

            var result = await _sqlDataAccess.LoadData<PecaModel, object>(query, parameters);
            return result;
        }
    }
}
