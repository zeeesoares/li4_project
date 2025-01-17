using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public class ProductService : IProductService
    {
        private readonly ISqlDataAccess _db;

        public ProductService(ISqlDataAccess sqlDataAccess)
        {
            _db = sqlDataAccess;
        }

        public async Task AddProduct(DesktopModel novoProduto, Dictionary<string, string> selecionadas)
        {
            try
            {
                var sqlProduto = @"
                INSERT INTO Desktop (Descricao, Imagem_URL, Preco, Categoria, Catalogo_idCatalogo)
                VALUES (@Descricao, @Imagem_Url, @Preco, @Categoria, @Catalogo_idCatalogo);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";

                var produtoParameters = new
                {
                    Descricao = novoProduto.Descricao,
                    Imagem_Url = novoProduto.Imagem_Url,
                    Preco = novoProduto.Preco,
                    Categoria = novoProduto.Categoria,
                    Catalogo_idCatalogo = novoProduto.Catalogo_idCatalogo
                };

                novoProduto.idDesktop = await _db.ExecuteScalarAsync<int>(sqlProduto, produtoParameters);


                foreach (var tipo in selecionadas.Keys)
                {
                    var pecaId = selecionadas[tipo];

                    if (int.TryParse(pecaId, out int pecaIdInt))
                    {
                        var pecasDesktop = new PecasDesktopModel
                        {
                            DesktopId = novoProduto.idDesktop,
                            PecaId = pecaIdInt,
                            Quantidade = 1
                        };

                        var sqlPecaDesktop = @"
                        INSERT INTO Pecas_Desktop (Peca_idPeca, Desktop_idDesktop, Quantidade)
                        VALUES (@Peca_idPeca, @Desktop_idDesktop, @Quantidade)";

                        var pecaParameters = new
                        {
                            Peca_idPeca = pecasDesktop.PecaId,
                            Desktop_idDesktop = pecasDesktop.DesktopId,
                            Quantidade = pecasDesktop.Quantidade
                        };

                        await _db.SaveData(sqlPecaDesktop, pecaParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar produto com peças: {ex.Message}");
            }
        }
    

        public async Task<List<DesktopModel>> GetAllProducts()
        {
            string query = "SELECT * FROM dbo.Desktop";
            return await _db.LoadData<DesktopModel, object>(query, new { });
        }
    }
}
      