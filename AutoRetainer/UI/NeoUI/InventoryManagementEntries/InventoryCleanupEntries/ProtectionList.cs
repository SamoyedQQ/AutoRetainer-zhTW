namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public class ProtectionList : InventoryManagementBase
{
    public override string Name { get; } = "背包整理/保護清單";
    private InventoryManagementCommon InventoryManagementCommon = new();
    private ProtectionList()
    {
        DisplayPriority = -1;
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
            .Section(Name)
            .TextWrapped("即使這些道具出現在其他處理清單中，AutoRetainer 也不會將其出售、分解、捨棄或繳交給大國防聯軍。")
            .Widget(() => InventoryManagementCommon.DrawListNew(
                itemId => InventoryCleanupCommon.SelectedPlan.AddItemToList(IMListKind.Protect, itemId, out _),
                itemId => InventoryCleanupCommon.SelectedPlan.IMProtectList.Remove(itemId), InventoryCleanupCommon.SelectedPlan.IMProtectList))
            .Separator()
            .Widget(() =>
            {
                InventoryManagementCommon.ImportBlacklistFromArDiscard();
            });
    }

}