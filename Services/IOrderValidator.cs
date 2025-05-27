using System.Collections.Generic;
using PizzeriaOrderProcessor.Models;

namespace PizzeriaOrderProcessor.Services
{
    public interface IOrderValidator
    {
        bool IsValid(Order order, out List<string> errors);
        bool IsOrderGroupValid(List<Order> orderGroup, out List<string> errors);
    }
}
