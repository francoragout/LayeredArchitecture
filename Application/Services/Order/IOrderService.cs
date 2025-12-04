using Application.Dtos.Order;

namespace Application.Services.Order
{
    public interface IOrderService
    {
        Task<IEnumerable<ReadOrderDto>> GetAllAsync();
        Task<ReadOrderDto?> GetByIdAsync(int orderId);
        Task<int> CreateAsync(CreateOrderDto createOrderDto);
        Task<bool> UpdateAsync(int orderId, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteAsync(int orderId);
    }
}