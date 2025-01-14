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

        public async Task<List<DesktopModel>> GetAllProducts()
        {
            string query = "SELECT * FROM dbo.Desktop";
            return await _db.LoadData<DesktopModel, object>(query, new { });
        }
    }
}
      