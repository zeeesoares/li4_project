using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public class PecaService : IPecaService
    {
        private readonly ISqlDataAccess _db;

        public PecaService(ISqlDataAccess sqlDataAccess)
        {
            _db = sqlDataAccess;
        }


        public async Task<List<PecaModel>> GetAllPecas()
        {
            string query = "SELECT * FROM dbo.Peca";
            return await _db.LoadData<PecaModel, object>(query, new { });
        }

        public async Task AddPeca(PecaModel novaPeca)
        {
            string query = @"
        INSERT INTO dbo.Peca (Nome, Tipo, Stock, Imagem_URL)
        VALUES (@Nome, @Tipo, @Stock, @Imagem_URL)";

            await _db.SaveData(query, novaPeca);
        }

        public async Task UpdatePeca(PecaModel peca)
        {
            string query = @"
                 UPDATE dbo.Peca
                 SET Nome = @Nome,
                 Tipo = @Tipo,
                 Stock = @Stock,
                 Imagem_URL = @Imagem_URL,
                 WHERE idPeca = @idPeca";

            await _db.SaveData(query, peca);
        }

        public async Task DeletePeca(int id)
        {
            string query = "DELETE FROM dbo.Peca WHERE idPeca = @idPeca";
            await _db.SaveData(query, new { Id = id });
        }

        public async Task UpdateQuantidade(PecaModel peca)
        {
            string query = @"
                UPDATE dbo.Peca
                SET Stock = @Stock
                WHERE idPeca = @idPeca";

            await _db.SaveData(query, peca);
        }

        public async Task<bool> CheckStockForProductsAsync(List<DesktopEncomendaModel> products)
        {
            var queries = new Dictionary<string, object>();

            foreach (var product in products)
            {
                string stockQuery = @"
                    SELECT 1
                    FROM Pecas_Desktop pd
                    INNER JOIN Peca p ON pd.Peca_idPeca = p.idPeca
                    WHERE pd.Desktop_idDesktop = @DesktopId
                    AND p.Stock < @RequiredQuantity";

                var stockParameters = new
                {
                    DesktopId = product.Desktop_idDesktop,
                    RequiredQuantity = product.Quantidade_Prod
                };

                var result = await _db.ExecuteScalarAsync<int?>(stockQuery, stockParameters);
                if (result != null)
                {
                    return false;
                }

                string updateStockQuery = @"
                    UPDATE Peca
                    SET Stock = Stock - @QuantityUsed
                    WHERE idPeca IN (
                        SELECT pd.Peca_idPeca
                        FROM Pecas_Desktop pd
                        WHERE pd.Desktop_idDesktop = @DesktopId
                    )";

                var updateParams = new
                {
                    QuantityUsed = product.Quantidade_Prod,
                    DesktopId = product.Desktop_idDesktop
                };

                string queryKey = $"UpdateQuery_{product.Desktop_idDesktop}";
                queries.Add(queryKey, new { Query = updateStockQuery, Parameters = updateParams });
            }

            foreach (var query in queries.Values)
            {
                var q = (dynamic)query;
                await _db.SaveData(q.Query, q.Parameters);
            }

            return true;
        }

    }
}