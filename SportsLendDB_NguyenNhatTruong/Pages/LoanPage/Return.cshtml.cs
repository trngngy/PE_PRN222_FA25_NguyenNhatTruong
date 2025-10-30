using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsLend.BLL.Service;

namespace SportsLendDB_NguyenNhatTruong.Pages.Loan
{
    [Authorize(Roles = "Manager,Staff")]
    public class ReturnModel : PageModel
    {
        private readonly LoanService _loanService;

        public ReturnModel(LoanService loanService)
        {
            _loanService = loanService;
        }

        public SportsLend.DAL.Models.Loan Loan { get; set; }
        public decimal CalculatedFee { get; set; }
        public int TotalDays { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan = await _loanService.GetLoanByIdAsync(id.Value);

            if (Loan == null)
            {
                return NotFound();
            }

            if (Loan.ReturnDate.HasValue)
            {
                TempData["ErrorMessage"] = "This equipment has already been returned.";
                return RedirectToPage("/LoanPage/Index");
            }

            // Calculate fee based on today
            var today = DateOnly.FromDateTime(DateTime.Today);
            CalculatedFee = _loanService.CalculateFee(Loan.LoanDate, today, Loan.DailyFeeUsd);
            TotalDays = today.DayNumber - Loan.LoanDate.DayNumber;
            if (TotalDays < 1) TotalDays = 1;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            if (loan.ReturnDate.HasValue)
            {
                TempData["ErrorMessage"] = "This equipment has already been returned.";
                return RedirectToPage("/LoanPage/Index");
            }

            var returnDate = DateOnly.FromDateTime(DateTime.Today);
            var result = await _loanService.ProcessReturnAsync(id, returnDate);

            if (result)
            {
                var calculatedFee = _loanService.CalculateFee(loan.LoanDate, returnDate, loan.DailyFeeUsd);
                TempData["SuccessMessage"] = $"Return processed successfully. Total fee: ${calculatedFee:0.00}";
                return RedirectToPage("/LoanPage/Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to process return.");
            return Page();
        }
    }
}