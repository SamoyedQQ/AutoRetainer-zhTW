using AutoRetainer.Internal.InventoryManagement;
using ECommons.GameHelpers;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public class GeneralSettings : InventoryManagementBase
{
    public override string Name { get; } = "背包整理/一般設定";

    private GeneralSettings()
    {
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
            .Section(Name)
            .Checkbox($"自動開啟探險寶箱", () => ref InventoryCleanupCommon.SelectedPlan.IMEnableCofferAutoOpen, "僅限多角模式。登出前會開啟所有寶箱，除非背包空位不足。")
            .Checkbox($"允許把道具賣給僱員", () => ref InventoryCleanupCommon.SelectedPlan.IMEnableAutoVendor, "AutoRetainer 重新派遣僱員探險時，會依照背包整理方案出售道具。")
            .Checkbox($"允許把道具賣給房屋 NPC", () => ref InventoryCleanupCommon.SelectedPlan.IMEnableNpcSell, "AutoRetainer 進入房屋時，會依照背包整理方案出售道具。房屋入口（不是工坊入口）附近必須擺放可收購道具的房屋商人——進門後應該要能立刻與該 NPC 互動。")
            .Indent()
            .Checkbox($"有僱員可用時忽略 NPC", () => ref InventoryCleanupCommon.SelectedPlan.IMSkipVendorIfRetainer)
            .Widget("立即出售", (x) =>
            {
                if(ImGuiEx.Button(x, Player.Interactable && InventoryCleanupCommon.SelectedPlan.IMEnableNpcSell && NpcSaleManager.GetValidNPC() != null && !IsOccupied() && !P.TaskManager.IsBusy))
                {
                    NpcSaleManager.EnqueueIfItemsPresent(true);
                }
            })
            .Unindent()
            .Checkbox($"自動分解道具", () => ref InventoryCleanupCommon.SelectedPlan.IMEnableItemDesynthesis)
            .Checkbox($"啟用右鍵選單整合", () => ref InventoryCleanupCommon.SelectedPlan.IMEnableContextMenu)
            .Checkbox($"允許出售／捨棄兵裝庫裡的道具", () => ref InventoryCleanupCommon.SelectedPlan.AllowSellFromArmory)
            .Checkbox($"展示模式", () => ref InventoryCleanupCommon.SelectedPlan.IMDry, "不要真的出售／捨棄道具，改在聊天視窗列出會被處理的道具")
            ;
    }
}
