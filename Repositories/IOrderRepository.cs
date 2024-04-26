using WEBSAIGONGLISTEN.Models;

namespace WEBSAIGONGLISTEN.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime startDate, DateTime endDate);
    }
}
