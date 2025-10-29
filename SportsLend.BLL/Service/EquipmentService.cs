using SportsLend.BLL.Repository;
using SportsLend.DAL.Models;

namespace SportsLend.BLL.Service
{
    public class EquipmentService
    {
        private readonly EquipmentRepository _equipmentRepository;

        public EquipmentService(EquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public EquipmentService()
        {
            _equipmentRepository = new EquipmentRepository();
        }

        public async Task<List<Equipment>> GetAllEquipmentAsync()
        {
            return await _equipmentRepository.GetAllWithTypeAsync();
        }

        public async Task<List<Equipment>> SearchEquipmentAsync(string searchTerm)
        {
            return await _equipmentRepository.SearchEquipmentAsync(searchTerm);
        }

        public async Task<string> GenerateNextEquipmentIdAsync()
        {
            return await _equipmentRepository.GenerateNextEquipmentIdAsync();
        }

        public async Task<bool> CreateEquipmentAsync(Equipment equipment)
        {
            try
            {
                await _equipmentRepository.CreateAsync(equipment);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _equipmentRepository.GetEquipmentByIdWithTypeAsync(id);
        }

        public async Task<bool> UpdateEquipmentAsync(Equipment equipment)
        {
            try
            {
                await _equipmentRepository.UpdateAsync(equipment);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsEquipmentOnLoanAsync(int equipmentId)
        {
            return await _equipmentRepository.IsEquipmentOnLoanAsync(equipmentId);
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            return await _equipmentRepository.DeleteEquipmentAsync(id);
        }

        public async Task<List<EquipmentType>> GetAllEquipmentTypesAsync()
        {
            return await _equipmentRepository.GetAllEquipmentTypesAsync();
        }
    }
}