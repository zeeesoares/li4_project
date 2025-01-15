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
    }
}