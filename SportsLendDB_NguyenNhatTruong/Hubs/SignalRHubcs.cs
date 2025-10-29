using Microsoft.AspNetCore.SignalR;
using SportsLend.BLL.Service;

namespace SportsLendDB_NguyenNhatTruong.Hubs

{
    public class SignalRHubcs : Hub
    {
        private readonly EquipmentService _equipmentService;

        public SignalRHubcs(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public async Task NotifyEquipmentAdded()
        {
            await Clients.All.SendAsync("EquipmentAdded");
        }
    }
}
