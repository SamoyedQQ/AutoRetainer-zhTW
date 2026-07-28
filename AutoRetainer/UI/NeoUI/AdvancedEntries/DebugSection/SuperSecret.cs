using Dalamud.Interface.Components;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal class SuperSecret : DebugSectionBase
{
    public override void Draw()
    {
        ImGuiEx.TextWrapped(ImGuiColors.ParsedOrange, "這裡什麼事都可能發生。");
        ImGui.Checkbox("舊版 RetainerSense", ref C.OldRetainerSense);
        ImGuiComponents.HelpMarker("偵測並使用玩家有效距離內最近的傳喚鈴。");
        ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey, "多角模式運作期間會強制啟用 RetainerSense。");
        ImGui.Separator();
        ImGui.Checkbox($"危險選項保護", ref C.UnsafeProtection);
        ImGui.SameLine();
        if(ImGui.Button($"寫入登錄檔"))
        {
            Safety.Set(C.UnsafeProtection);
        }
        var g = Safety.Get();
        ImGuiEx.Text(g ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, $"安全旗標：{(g ? "Present" : "Absent")}");
    }
}
