namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal class DebugBailout : DebugSectionBase
{
    public override void Draw()
    {
        ImGui.Checkbox(nameof(BailoutManager.SimulateStuckOnQuit), ref BailoutManager.SimulateStuckOnQuit);
        ImGui.Checkbox(nameof(BailoutManager.SimulateStuckOnVoyagePanel), ref BailoutManager.SimulateStuckOnVoyagePanel);
        ImGuiEx.Text($"無選單卡住時間：{Environment.TickCount64 - BailoutManager.NoSelectString}");
        ImGuiEx.Text($"角色選擇卡住時間：{Environment.TickCount64 - BailoutManager.CharaSelectStuck}");
    }
}
