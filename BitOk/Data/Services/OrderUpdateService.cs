using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BitOk.Data.Services
{
    public class OrderUpdateService : BackgroundService
    {
        private readonly IOrderService _orderService;

        public OrderUpdateService(IOrderService orderService)
        {
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var preparationOrders = await _orderService.GetOrdersByStatusAsyncAdmin(2);

                    foreach (var order in preparationOrders)
                    {
                        var products = await _orderService.GetDesktopEncomendaByOrderIdAsync(order.idEncomenda);

                        foreach (var product in products)
                        {
                            if (product.Estado != "Pronto")
                            {
                                var nextState = GetNextState(product.Estado);
                                await _orderService.UpdateProductStateAsync(product.Encomenda_idEncomenda, product.Desktop_idDesktop, nextState);
                                await Task.Delay(5000, stoppingToken);
                            }
                        }

                        if (products.All(p => p.Estado == "Pronto"))
                        {
                            order.Estado_idEstado = 3;
                            order.Data_Fim = DateTime.Now;
                            await _orderService.UpdateOrderAsync(order);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao atualizar encomendas: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private string GetNextState(string estadoAtual)
        {
            return estadoAtual switch
            {
                "Espera" => "Montar CPU",
                "Montar CPU" => "Montar RAM",
                "Montar RAM" => "Montar Disco",
                "Montar Disco" => "Montar Cooler",
                "Montar Cooler" => "Montar Motherboard",
                "Montar Motherboard" => "Montar GPU",
                "Montar GPU" => "Montar Fonte de Alimentação",
                "Montar Fonte de Alimentação" => "Montar Caixa",
                "Montar Caixa" => "Pronto",
                _ => "Pronto"
            };
        }
    }
}
