using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsLend.BLL.Service;
using SportsLend.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace SportsLendDB_NguyenNhatTruong.Pages.Equipment
{
    [Authorize(Roles = "Manager,Staff")]
    public class EditModel : PageModel
    {
        private readonly EquipmentService _equipmentService;

        public EditModel(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public int EquipmentDbId { get; set; }
        public string EquipmentId { get; set; }

        public SelectList EquipmentTypes { get; set; }
        public SelectList ConditionList { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Name is required")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Brand is required")]
            [StringLength(30, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 30 characters")]
            public string Brand { get; set; }

            [Required(ErrorMessage = "Type is required")]
            public int TypeId { get; set; }

            [Required(ErrorMessage = "Condition is required")]
            public string Condition { get; set; }

            [Range(0.5, 200, ErrorMessage = "Daily Fee must be between 0.5 and 200")]
            [Display(Name = "Daily Fee (USD)")]
            public decimal DailyFeeUsd { get; set; }

            [Required(ErrorMessage = "InStock is required")]
            [Range(0, 999, ErrorMessage = "InStock must be between 0 and 999")]
            [Display(Name = "In Stock")]
            public int InStock { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _equipmentService.GetEquipmentByIdAsync(id.Value);

            if (equipment == null)
            {
                return NotFound();
            }

            EquipmentDbId = equipment.Id;
            EquipmentId = equipment.EquipmentId;

            Input = new InputModel
            {
                Name = equipment.Name,
                Brand = equipment.Brand,
                TypeId = equipment.TypeId,
                Condition = equipment.Condition,
                DailyFeeUsd = equipment.DailyFeeUsd,
                InStock = equipment.InStock
            };

            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            // Update fields
            equipment.Name = Input.Name;
            equipment.Brand = Input.Brand;
            equipment.TypeId = Input.TypeId;
            equipment.Condition = Input.Condition;
            equipment.InStock = Input.InStock;

            // Q4: Staff cannot change DailyFeeUSD
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (role == "Manager")
            {
                equipment.DailyFeeUsd = Input.DailyFeeUsd;
            }

            var result = await _equipmentService.UpdateEquipmentAsync(equipment);

            if (result)
            {
                TempData["SuccessMessage"] = "Equipment updated successfully.";
                return RedirectToPage("/Equipment/Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to update equipment.");
            await LoadDropdownsAsync();
            return Page();
        }

        private async Task LoadDropdownsAsync()
        {
            var types = await _equipmentService.GetAllEquipmentTypesAsync();
            EquipmentTypes = new SelectList(types, "TypeId", "TypeName");

            ConditionList = new SelectList(new[]
            {
                new { Value = "New", Text = "New" },
                new { Value = "Good", Text = "Good" },
                new { Value = "Used", Text = "Used" },
                new { Value = "Damaged", Text = "Damaged" }
            }, "Value", "Text");
        }

        public bool IsStaff()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role == "Staff";
        }
    }
}