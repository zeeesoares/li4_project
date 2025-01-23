using BitOk.Data.Models;
using BitOk.Data.Services;
using System.Data.SqlClient;

public class OrderServiceBackground : IOrderServiceBackground
{
    public async Task UpdateOrderAsync(EncomendaModel updatedOrder, SqlConnection connection)
    {
        string query = @"
            UPDATE Encomenda
            SET Data_Inicio = @DataInicio, Data_Fim = @DataFim, Estado_idEstado = @EstadoId, Utilizador_idUtilizador = @UtilizadorId
            WHERE idEncomenda = @Id";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@DataInicio", updatedOrder.Data_Inicio);
            command.Parameters.AddWithValue("@DataFim", updatedOrder.Data_Fim);
            command.Parameters.AddWithValue("@EstadoId", updatedOrder.Estado_idEstado);
            command.Parameters.AddWithValue("@UtilizadorId", updatedOrder.Utilizador_idUtilizador);
            command.Parameters.AddWithValue("@Id", updatedOrder.idEncomenda);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<DesktopEncomendaModel>> GetDesktopEncomendaByOrderIdAsync(int orderId, SqlConnection connection)
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

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OrderId", orderId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                var result = new List<DesktopEncomendaModel>();

                while (await reader.ReadAsync())
                {
                    int quantidadeProd = reader.GetInt32(reader.GetOrdinal("Quantidade_Prod"));
                    result.AddRange(Enumerable.Range(0, quantidadeProd).Select(_ => new DesktopEncomendaModel
                    {
                        Encomenda_idEncomenda = reader.GetInt32(reader.GetOrdinal("Encomenda_idEncomenda")),
                        Desktop_idDesktop = reader.GetInt32(reader.GetOrdinal("Desktop_idDesktop")),
                        Quantidade_Prod = 1,
                        Estado = reader.GetString(reader.GetOrdinal("Estado")),
                        Encomenda = new EncomendaModel
                        {
                            idEncomenda = reader.GetInt32(reader.GetOrdinal("idEncomenda")),
                            Data_Inicio = reader.GetDateTime(reader.GetOrdinal("Data_Inicio"))
                        },
                        Desktop = new DesktopModel
                        {
                            idDesktop = reader.GetInt32(reader.GetOrdinal("idDesktop")),
                            Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                            Descricao = reader.GetString(reader.GetOrdinal("Descricao")),
                            Preco = reader.GetDecimal(reader.GetOrdinal("Preco"))
                        }
                    }));
                }

                return result;
            }
        }
    }

    public async Task<bool> UpdateProductStateAsync(int encomendaId, int desktopId, string novoEstado, SqlConnection connection)
    {
        string query = @"
            UPDATE dbo.Desktop_Encomendas
            SET Estado = @NovoEstado
            WHERE Encomenda_idEncomenda = @EncomendaId
              AND Desktop_idDesktop = @DesktopId";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@EncomendaId", encomendaId);
            command.Parameters.AddWithValue("@DesktopId", desktopId);
            command.Parameters.AddWithValue("@NovoEstado", novoEstado);

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}