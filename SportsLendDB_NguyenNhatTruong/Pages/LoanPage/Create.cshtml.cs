using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsLend.BLL.Service;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SportsLendDB_NguyenNhatTruong.Pages.Loan
{
    [Authorize(Roles = "Manager,Staff")]
    public class CreateModel : PageModel
    {
        private readonly LoanService _loanService;
        private readonly EquipmentService _equipmentService;

        public CreateModel(LoanService loanService, EquipmentService equipmentService)
        {
            _loanService = loanService;
            _equipmentService = equipmentService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList Members { get; set; }
        public SelectList AvailableEquipment { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Member is required")]
            [Display(Name = "Member")]
            public int MemberId { get; set; }

            [Required(ErrorMessage = "Equipment is required")]
            [Display(Name = "Equipment")]
            public int EquipmentId { get; set; }

            [Required(ErrorMessage = "Loan Date is required")]
            [Display(Name = "Loan Date")]
            public DateOnly LoanDate { get; set; }

            [Required(ErrorMessage = "Due Date is required")]
            [Display(Name = "Due Date")]
            public DateOnly DueDate { get; set; }

            [Required(ErrorMessage = "Daily Fee is required")]
            [Range(0.5, 200, ErrorMessage = "Daily Fee must be between 0.5 and 200")]
            [Display(Name = "Daily Fee (USD)")]
            public decimal DailyFeeUsd { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Set default dates
            Input = new InputModel
            {
                LoanDate = DateOnly.FromDateTime(DateTime.Today),
                DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7))
            };

            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetEquipmentFeeAsync(int equipmentId)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(equipmentId);
            if (equipment == null)
            {
                return new JsonResult(new { success = false });
            }

            return new JsonResult(new
            {
                success = true,
                dailyFee = equipment.DailyFeeUsd,
                inStock = equipment.InStock
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            // Validate dates
            if (Input.DueDate <= Input.LoanDate)
            {
                ModelState.AddModelError("Input.DueDate", "Due Date must be after Loan Date");
                await LoadDropdownsAsync();
                return Page();
            }

            // Check equipment availability
            var equipment = await _equipmentService.GetEquipmentByIdAsync(Input.EquipmentId);
            if (equipment == null)
            {
                ModelState.AddModelError(string.Empty, "Equipment not found.");
                await LoadDropdownsAsync();
                return Page();
            }

            if (equipment.InStock <= 0)
            {
                ModelState.AddModelError("Input.EquipmentId", "Out of stock.");
                await LoadDropdownsAsync();
                return Page();
            }

            // Get current user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                ModelState.AddModelError(string.Empty, "Invalid user session.");
                await LoadDropdownsAsync();
                return Page();
            }

            // Create loan
            var loan = new SportsLend.DAL.Models.Loan
            {
                MemberId = Input.MemberId,
                EquipmentId = Input.EquipmentId,
                LoanDate = Input.LoanDate,
                DueDate = Input.DueDate,
                DailyFeeUsd = Input.DailyFeeUsd,
                CreatedBy = userId,
                ReturnDate = null
            };

            var result = await _loanService.CreateLoanAsync(loan);

            if (result)
            {
                // Decrease stock
                equipment.InStock -= 1;
                await _equipmentService.UpdateEquipmentAsync(equipment);

                TempData["SuccessMessage"] = "Loan created successfully.";
                return RedirectToPage("/LoanPage/Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create loan.");
            await LoadDropdownsAsync();
            return Page();
        }

        private async Task LoadDropdownsAsync()
        {
            var members = await _loanService.GetAllMembersAsync();
            Members = new SelectList(members, "MemberId", "FullName");

            var equipment = await _loanService.GetAvailableEquipmentAsync();
            AvailableEquipment = new SelectList(
                equipment.Select(e => new
                {
                    e.Id,
                    DisplayText = $"{e.EquipmentId} - {e.Name} ({e.Type?.TypeName}) - Stock: {e.InStock}"
                }),
                "Id",
                "DisplayText"
            );
        }
    }
}