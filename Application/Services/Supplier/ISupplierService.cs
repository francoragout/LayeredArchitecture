using Application.Dtos.Supplier;

namespace Application.Services.Supplier
{
    public interface ISupplierService
    {
        Task<IEnumerable<ReadSupplierDto>> GetAllAsync();
        Task<ReadSupplierDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateSupplierDto supplierDto);
        Task<bool> UpdateAsync(int id, UpdateSupplierDto supplierDto);
        Task<bool> DeleteAsync(int id);
    }
}