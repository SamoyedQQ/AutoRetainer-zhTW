using AutoRetainerAPI.Configuration;
using System.Collections.Frozen;

namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeContingency : NeoUIEntry
{
    private static readonly FrozenDictionary<WorkshopFailAction, string> WorkshopFailActionNames = new Dictionary<WorkshopFailAction, string>()
    {
        [WorkshopFailAction.StopPlugin] = "Halt all plugin operation",
        [WorkshopFailAction.ExcludeVessel] = "Exclude deployable from operation",
        [WorkshopFailAction.ExcludeChar] = "Exclude captain from multi mode rotation",
    }.ToFrozenDictionary();

    public override string Path => "多角模式/應變處理";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("應變處理")
        .TextWrapped("在這裡可以設定各種後備動作，用來因應常見的失敗狀態或可能的操作錯誤。")
        .EnumComboFullWidth(null, "青磷水罐耗盡", () => ref C.FailureNoFuel, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "青磷水罐不足、無法讓載具出發新航行時，執行選定的後備動作。")
        .EnumComboFullWidth(null, "無法修理載具", () => ref C.FailureNoRepair, null, WorkshopFailActionNames, "魔導修理材料不足、無法修理載具時，執行選定的後備動作。")
        .EnumComboFullWidth(null, "背包已滿", () => ref C.FailureNoInventory, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "艇長背包空間不足、無法接收航行報酬時，執行選定的後備動作。")
        .EnumComboFullWidth(null, "嚴重運作失敗", () => ref C.FailureGeneric, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "發生任何未知或其他錯誤時，執行選定的後備動作。")
        .Widget("被 GM 關進小黑屋", (x) =>
        {
            ImGui.BeginDisabled();
            ImGuiEx.SetNextItemFullWidth();
            if(ImGui.BeginCombo("##jailsel", "強制關閉遊戲")) { ImGui.EndCombo(); }
            ImGui.EndDisabled();
        }, "若外掛執行期間被 GM 關進小黑屋，會執行選定的後備動作。祝你好運！");
}
