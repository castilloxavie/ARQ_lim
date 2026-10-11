using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;

namespace Application.UseCases;

public class CreateOrderHandler
{
    private readonly OrderService _orderService;
    private readonly IOrderRepository _orderRepository;
    private readonly IAppLogger _logger;

    public CreateOrderHandler(
        OrderService orderService,
        IOrderRepository orderRepository,
        IAppLogger logger)
    {
        _orderService = orderService;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public Order Execute(string customer, string product, int qty, decimal price)
    {
        _logger.Log("CreateOrderHandler inicio");

        var order = _orderService.CreateOrder(customer, product, qty, price);
        _orderRepository.Add(order);

        _logger.Log("CreateOrderHandler orden " + order.Id + " persistida");
        return order;
    }
}