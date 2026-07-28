namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public class SoftList : InventoryManagementBase
{
    public override string Name => "背包整理/快速探險出售清單";
    private InventoryManagementCommon InventoryManagementCommon = new();
    private SoftList()
    {
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
            .Section(Name)
            .TextWrapped("這些道具若是從快速探險取得就會被出售，除非它們與相同道具堆疊在一起。")
            .Widget(() => InventoryManagementCommon.DrawListNew(
                itemId => InventoryCleanupCommon.SelectedPlan.AddItemToList(IMListKind.SoftSell, itemId, out _),
                itemId => InventoryCleanupCommon.SelectedPlan.IMAutoVendorSoft.Remove(itemId), InventoryCleanupCommon.SelectedPlan.IMAutoVendorSoft,
                filter: item => item.PriceLow != 0))
            .Widget(() =>
            {
                InventoryManagementCommon.ImportFromArDiscard(InventoryCleanupCommon.SelectedPlan.IMAutoVendorSoft);
            });
    }
}
