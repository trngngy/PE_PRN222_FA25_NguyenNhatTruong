using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsLend.BLL.Repository;
using SportsLend.BLL.Service;
using System.Text;

namespace SportsLendDB_NguyenNhatTruong.Pages.ReportPage;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly ReportService _reportService;

    public IndexModel(ReportService reportService)
    {
        _reportService = reportService;
    }

    public List<LoansByMonthDto> LoansByMonth { get; set; }
    public int CurrentYear { get; set; }

    [BindProperty]
    public DateOnly FromDate { get; set; }

    [BindProperty]
    public DateOnly ToDate { get; set; }

    public List<TopEquipmentTypeDto> TopEquipmentTypes { get; set; }
    public List<InventorySummaryDto> InventorySummary { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        CurrentYear = DateTime.Now.Year;
        ToDate = DateOnly.FromDateTime(DateTime.Today);
        FromDate = ToDate.AddDays(-30);

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CurrentYear = DateTime.Now.Year;
        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnGetExportCsvAsync(DateOnly fromDate, DateOnly toDate)
    {
        var data = await _reportService.GetTopEquipmentTypesAsync(fromDate, toDate);

        var csv = new StringBuilder();
        csv.AppendLine("TypeName,LoanCount");
        foreach (var item in data)
        {
            csv.AppendLine($"\"{item.TypeName}\",{item.LoanCount}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"TopEquipmentTypes_{DateTime.Now:yyyyMMdd}.csv");
    }

    private async Task LoadDataAsync()
    {
        LoansByMonth = await _reportService.GetLoansByMonthAsync(CurrentYear);
        TopEquipmentTypes = await _reportService.GetTopEquipmentTypesAsync(FromDate, ToDate);
        InventorySummary = await _reportService.GetInventorySummaryAsync();
    }
}