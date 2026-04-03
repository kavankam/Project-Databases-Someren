using Someren.Models;

namespace Someren.Repositories;

public interface IOrderRepository
{
    void AddOrder(Order order);
}