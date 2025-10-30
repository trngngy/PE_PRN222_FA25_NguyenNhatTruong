using Microsoft.EntityFrameworkCore;
using SportsLend.BLL.Models;
using SportsLend.DAL.Models;

namespace SportsLend.BLL.Repository
{
    public class ReportRepository : GenericRepository<Loan>
    {
        public ReportRepository()
        {

        }

        public ReportRepository(SportsLendDBContext context)
        {
            _context = context;
        }

        // 1. Get Loans by Month for current year
        public async Task<List<LoansByMonthDto>> GetLoansByMonthAsync(int year)
        {
            var loans = await _context.Loans
                .Where(l => l.LoanDate.Year == year)
                .GroupBy(l => l.LoanDate.Month)
                .Select(g => new LoansByMonthDto
                {
                    Month = g.Key,
                    TotalLoans = g.Count()
                })
                .ToListAsync();

            // Fill all 12 months with 0 if no data
            var allMonths = Enumerable.Range(1, 12).Select(m => new LoansByMonthDto
            {
                Month = m,
                TotalLoans = loans.FirstOrDefault(x => x.Month == m)?.TotalLoans ?? 0
            }).ToList();

            return allMonths;
        }

        // 2. Get Top Equipment Types by date range
        public async Task<List<TopEquipmentTypeDto>> GetTopEquipmentTypesAsync(DateOnly fromDate, DateOnly toDate, int topCount = 5)
        {
            var topTypes = await _context.Loans
                .Include(l => l.Equipment)
                .ThenInclude(e => e.Type)
                .Where(l => l.LoanDate >= fromDate && l.LoanDate <= toDate)
                .GroupBy(l => l.Equipment.Type.TypeName)
                .Select(g => new TopEquipmentTypeDto
                {
                    TypeName = g.Key,
                    LoanCount = g.Count()
                })
                .OrderByDescending(x => x.LoanCount)
                .Take(topCount)
                .ToListAsync();

            return topTypes;
        }

        // 3. Get Inventory Summary grouped by Type
        public async Task<List<InventorySummaryDto>> GetInventorySummaryAsync()
        {
            var summary = await _context.Equipment
                .Include(e => e.Type)
                .GroupBy(e => e.Type.TypeName)
                .Select(g => new InventorySummaryDto
                {
                    TypeName = g.Key,
                    TotalItems = g.Count(),
                    TotalInStock = g.Sum(e => e.InStock)
                })
                .OrderBy(x => x.TypeName)
                .ToListAsync();

            return summary;
        }
    }

    // DTOs for Report Data
    public class LoansByMonthDto
    {
        public int Month { get; set; }
        public int TotalLoans { get; set; }
    }

    public class TopEquipmentTypeDto
    {
        public string TypeName { get; set; }
        public int LoanCount { get; set; }
    }

    public class InventorySummaryDto
    {
        public string TypeName { get; set; }
        public int TotalItems { get; set; }
        public int TotalInStock { get; set; }
    }
}