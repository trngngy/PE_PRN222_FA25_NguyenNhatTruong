using Microsoft.EntityFrameworkCore;
using SportsLend.BLL.Models;
using SportsLend.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsLend.BLL.Repository
{
    public class EquipmentRepository : GenericRepository<Equipment>
    {
        public EquipmentRepository() { }

        public EquipmentRepository(SportsLendDBContext context) => _context = context;

        public async Task<List<Equipment>> GetAllWithTypeAsync()
        {
            return await _context.Equipment
                .Include(e => e.Type)
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<List<Equipment>> SearchEquipmentAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllWithTypeAsync();
            }

            searchTerm = searchTerm.Trim().ToLower();

            return await _context.Equipment
                .Include(e => e.Type)
                .Where(e => e.Name.ToLower().Contains(searchTerm) ||
                            e.Brand.ToLower().Contains(searchTerm))
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<string> GenerateNextEquipmentIdAsync()
        {
            var lastEquipment = await _context.Equipment
                .OrderByDescending(e => e.EquipmentId)
                .FirstOrDefaultAsync();

            if (lastEquipment == null)
            {
                return "EQ001";
            }

            string lastId = lastEquipment.EquipmentId;
            if (lastId.StartsWith("EQ") && int.TryParse(lastId.Substring(2), out int number))
            {
                return $"EQ{(number + 1):D3}";
            }

            return "EQ001";
        }

        public async Task<Equipment?> GetEquipmentByIdWithTypeAsync(int id)
        {
            return await _context.Equipment
                .Include(e => e.Type)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> IsEquipmentOnLoanAsync(int equipmentId)
        {
            return await _context.Loans
                .AnyAsync(l => l.EquipmentId == equipmentId && l.ReturnDate == null);
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return false;
            }

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EquipmentType>> GetAllEquipmentTypesAsync()
        {
            return await _context.EquipmentTypes
                .OrderBy(t => t.TypeName)
                .ToListAsync();
        }
    }
}
