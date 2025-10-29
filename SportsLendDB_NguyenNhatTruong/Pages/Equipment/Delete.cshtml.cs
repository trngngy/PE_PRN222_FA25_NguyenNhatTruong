using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsLend.BLL.Service;
using SportsLend.DAL.Models;

namespace SportsLendDB_NguyenNhatTruong.Pages.Equipment
{
    [Authorize(Roles = "Manager")]
    public class DeleteModel : PageModel
    {
        private readonly EquipmentService _equipmentService;

        public DeleteModel(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [BindProperty]
        public SportsLend.DAL.Models.Equipment Equipment { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Equipment = await _equipmentService.GetEquipmentByIdAsync(id.Value);

            if (Equipment == null)
            {
                return NotFound();
            }

            // Q5: Check if equipment is on loan
            var isOnLoan = await _equipmentService.IsEquipmentOnLoanAsync(id.Value);
            if (isOnLoan)
            {
                ErrorMessage = "Cannot delete: equipment is currently on loan.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Q5: Check if equipment is on loan before delete
            var isOnLoan = await _equipmentService.IsEquipmentOnLoanAsync(id.Value);
            if (isOnLoan)
            {
                TempData["ErrorMessage"] = "Cannot delete: equipment is currently on loan.";
                return RedirectToPage("/Equipment/Index");
            }

            var result = await _equipmentService.DeleteEquipmentAsync(id.Value);

            if (result)
            {
                TempData["SuccessMessage"] = "Equipment deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete equipment.";
            }

            return RedirectToPage("/Equipment/Index");
        }
    }
}