using System.Collections.Generic;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Data;

public class SqlOrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public SqlOrderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Add(Order order)
    {
        const string sql = "INSERT INTO Orders (Id, Customer, Product, Qty, Price) VALUES (@Id, @Customer, @Product, @Qty, @Price)";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", order.Id);
        command.Parameters.AddWithValue("@Customer", order.CustomerName);
        command.Parameters.AddWithValue("@Product", order.ProductName);
        command.Parameters.AddWithValue("@Qty", order.Quantity);
        command.Parameters.AddWithValue("@Price", order.UnitPrice);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<Order> GetLastOrders()
    {
        const string sql = "SELECT TOP 50 Id, Customer, Product, Qty, Price FROM Orders ORDER BY Id DESC";

        var orders = new List<Order>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            orders.Add(new Order
            {
                Id = reader.GetInt32(0),
                CustomerName = reader.GetString(1),
                ProductName = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                UnitPrice = reader.GetDecimal(4)
            });
        }

        return orders;
    }
}