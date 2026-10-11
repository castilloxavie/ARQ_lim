using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    void Add(Order order);
    IReadOnlyList<Order> GetLastOrders();
}