﻿using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public interface IProductService
    {
        Task<List<DesktopModel>> GetAllProducts();
        Task AddProduct(DesktopModel novoProduto, Dictionary<string, string> selecionadas);
    }
}
