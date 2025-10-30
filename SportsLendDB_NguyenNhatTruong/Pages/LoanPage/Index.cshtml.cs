using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsLend.BLL.Service;
using SportsLend.DAL.Models;

namespace SportsLendDB_NguyenNhatTruong.Pages.Loan
{
    [Authorize(Roles = "Manager,Staff")]
    public class IndexModel : PageModel
    {
        private readonly LoanService _loanService;

        public IndexModel(LoanService loanService)
        {
            _loanService = loanService;
        }

        public List<SportsLend.DAL.Models.Loan> Loans { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            Loans = await _loanService.GetAllLoansAsync();
        }

        public bool IsManager()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role == "Manager";
        }
    }
}