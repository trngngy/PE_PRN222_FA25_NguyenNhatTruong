using SportsLend.BLL.Repository;

namespace SportsLend.BLL.Service
{
    public class ReportService
    {
        private readonly ReportRepository _reportRepository;

        public ReportService(ReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public ReportService()
        {
            _reportRepository = new ReportRepository();
        }

        // 1. Loans by Month (current year)
        public async Task<List<LoansByMonthDto>> GetLoansByMonthAsync(int year)
        {
            return await _reportRepository.GetLoansByMonthAsync(year);
        }

        // 2. Top Equipment Types by date range
        public async Task<List<TopEquipmentTypeDto>> GetTopEquipmentTypesAsync(DateOnly fromDate, DateOnly toDate, int topCount = 5)
        {
            return await _reportRepository.GetTopEquipmentTypesAsync(fromDate, toDate, topCount);
        }

        // 3. Inventory Summary
        public async Task<List<InventorySummaryDto>> GetInventorySummaryAsync()
        {
            return await _reportRepository.GetInventorySummaryAsync();
        }
    }
}