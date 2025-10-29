using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using SportsLend.BLL.Service;
using SportsLend.DAL.Models;
using SportsLendDB_NguyenNhatTruong.Hubs;
using System.ComponentModel.DataAnnotations;

namespace SportsLendDB_NguyenNhatTruong.Pages.Equipment
{
    [Authorize(Roles = "Manager")]
    public class CreateModel : PageModel
    {
        private readonly EquipmentService _equipmentService;
        private readonly IHubContext<SignalRHubcs> _hubContext;

        public CreateModel(EquipmentService equipmentService, IHubContext<SignalRHubcs> hubContext)
        {
            _equipmentService = equipmentService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

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

            [Required(ErrorMessage = "Daily Fee is required")]
            [Range(0.5, 200, ErrorMessage = "Daily Fee must be between 0.5 and 200")]
            [Display(Name = "Daily Fee (USD)")]
            public decimal DailyFeeUsd { get; set; }

            [Required(ErrorMessage = "InStock is required")]
            [Range(0, 999, ErrorMessage = "InStock must be between 0 and 999")]
            [Display(Name = "In Stock")]
            public int InStock { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            // Generate EquipmentId
            var equipmentId = await _equipmentService.GenerateNextEquipmentIdAsync();

            var equipment = new SportsLend.DAL.Models.Equipment
            {
                EquipmentId = equipmentId,
                Name = Input.Name,
                Brand = Input.Brand,
                TypeId = Input.TypeId,
                Condition = Input.Condition,
                DailyFeeUsd = Input.DailyFeeUsd,
                InStock = Input.InStock
            };

            var result = await _equipmentService.CreateEquipmentAsync(equipment);

            if (result)
            {
                // Q3: Broadcast via SignalR
                await _hubContext.Clients.All.SendAsync("EquipmentAdded");

                TempData["SuccessMessage"] = "New equipment added successfully.";
                return RedirectToPage("/Equipment/Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to add equipment.");
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
    }
}