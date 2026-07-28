namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public class HardList : InventoryManagementBase
{
    public override string Name => "背包整理/無條件出售清單";
    private InventoryManagementCommon InventoryManagementCommon = new();

    private HardList()
    {
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
            .Section(Name)
            .TextWrapped("只要堆疊數量不超過下方指定的數量，這些道具不論來源都一定會被出售。另外，只有這些道具才會被賣給 NPC。")
            .InputInt(150f, $"可出售的最大堆疊數量", () => ref InventoryCleanupCommon.SelectedPlan.IMAutoVendorHardStackLimit)
            .Widget(() => InventoryManagementCommon.DrawListNew(
                itemId => InventoryCleanupCommon.SelectedPlan.AddItemToList(IMListKind.HardSell, itemId, out _),
                itemId => InventoryCleanupCommon.SelectedPlan.IMAutoVendorHard.Remove(itemId),
                InventoryCleanupCommon.SelectedPlan.IMAutoVendorHard, 
                (x) =>
                {
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGuiEx.CollectionButtonCheckbox(FontAwesomeIcon.Database.ToIconString(), x, InventoryCleanupCommon.SelectedPlan.IMAutoVendorHardIgnoreStack);
                    ImGui.PopFont();
                    ImGuiEx.Tooltip($"此道具忽略堆疊設定");
                },
                filter: item => item.PriceLow != 0))
            .Separator()
            .Widget(() =>
            {
                InventoryManagementCommon.ImportFromArDiscard(InventoryCleanupCommon.SelectedPlan.IMAutoVendorHard);
            });
    }
}
