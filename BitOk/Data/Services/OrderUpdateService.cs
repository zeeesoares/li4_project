using BitOk.Data.Models;
using BitOk.Data.Services;
using BitOk.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Data.SqlClient;

public class OrderUpdateService : BackgroundService
{
    private readonly IOrderService _orderService;
    private readonly IOrderServiceBackground _orderServiceBackground;
    private readonly IHubContext<MyHub> _hubContext;
    private readonly IConfiguration _config;

    public OrderUpdateService(IOrderService orderService, IHubContext<MyHub> hubContext, IConfiguration config, IOrderServiceBackground orderServiceBackground)
    {
        _orderService = orderService;
        _hubContext = hubContext;
        _config = config;
        _orderServiceBackground = orderServiceBackground;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var preparationOrders = await _orderService.GetOrdersByStatusAsyncAdmin(2);
                var orderTasks = preparationOrders.Select(order => Task.Run(() => ProcessOrder(order, stoppingToken))).ToList();
                await Task.WhenAll(orderTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task ProcessOrder(EncomendaModel order, CancellationToken stoppingToken)
    {
        SqlConnection connection = null;

        try
        {
            connection = new SqlConnection(_config.GetConnectionString("Default"));
            await connection.OpenAsync(stoppingToken);
            var products = await _orderServiceBackground.GetDesktopEncomendaByOrderIdAsync(order.idEncomenda, connection);

            if (products == null || !products.Any())
            {
                return;
            }

            foreach (var product in products)
            {

                if (product.Estado != "Pronto")
                {
                    var nextState = GetNextState(product.Estado);

                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync(stoppingToken); 
                    }

                    await _orderServiceBackground.UpdateProductStateAsync(
                        product.Encomenda_idEncomenda,
                        product.Desktop_idDesktop,
                        nextState,
                        connection); 

                    await _hubContext.Clients.All.SendAsync(
                        "ReceiveDesktopUpdate",
                        product.Desktop_idDesktop,
                        nextState
                    );
                }
            }

            await Task.Delay(5000, stoppingToken);

            if (products.All(p => p.Estado == "Pronto"))
            {
                order.Estado_idEstado = 3;  
                order.Data_Fim = DateTime.Now;

                await _orderServiceBackground.UpdateOrderAsync(order, connection);

                await _hubContext.Clients.All.SendAsync(
                    "ReceiveUpdate",
                    order.idEncomenda,
                    order.Estado_idEstado
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na encomenda {order.idEncomenda}: {ex.Message}");
        }
        finally
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
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