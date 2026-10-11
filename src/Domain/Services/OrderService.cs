using System;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Services;

public class OrderService
{
    private readonly IAppLogger _logger;

    public OrderService(IAppLogger logger)
    {
        _logger = logger;
    }

    public Order CreateOrder(string customer, string product, int qty, decimal price)
    {
        var order = new Order
        {
            Id = GenerateOrderId(),
            CustomerName = customer,
            ProductName = product,
            Quantity = qty,
            UnitPrice = price
        };

        _logger.Log("Created order " + order.Id + " for " + customer);
        return order;
    }

    private static int GenerateOrderId()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var random = System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 1000000);
        return (int)((ticks + random) % int.MaxValue);
    }
}