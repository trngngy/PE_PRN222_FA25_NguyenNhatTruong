using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsLend.BLL.Service;
using SportsLend.DAL.Models;

namespace SportsLendDB_NguyenNhatTruong.Pages.Equipment
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly EquipmentService _equipmentService;
        private const int PageSize = 4;

        public IndexModel(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public List<SportsLend.DAL.Models.Equipment> Equipment { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public string SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            var allEquipment = string.IsNullOrWhiteSpace(SearchTerm)
                ? await _equipmentService.GetAllEquipmentAsync()
                : await _equipmentService.SearchEquipmentAsync(SearchTerm);

            TotalItems = allEquipment.Count;
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

            // Get items for current page
            Equipment = allEquipment
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public bool CanEdit()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role == "Manager" || role == "Staff";
        }

        public bool IsManager()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role == "Manager";
        }
    }
}