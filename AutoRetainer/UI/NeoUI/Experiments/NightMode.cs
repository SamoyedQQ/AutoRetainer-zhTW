namespace AutoRetainer.UI.NeoUI.Experiments;

internal class NightMode : ExperimentUIEntry
{
    public override string Name => "夜間模式";
    public override void Draw()
    {
        ImGuiEx.TextWrapped($"夜間模式：\n" +
                $"－會強制啟用「停留在登入畫面等待」\n" +
                $"－會強制套用內建的 FPS 限制\n" +
                $"－視窗非作用中且處於等待狀態時，遊戲會被限制在 0.2 FPS\n" +
                $"－看起來會像遊戲當掉，但切回遊戲視窗後給它最多 5 秒就會恢復。\n" +
                $"－夜間模式預設只處理載具\n" +
                $"－關閉夜間模式後，救援模組會啟動並把你重新登入回遊戲。");
        if(ImGui.Checkbox("啟用夜間模式", ref C.NightMode)) MultiMode.BailoutNightMode();
        ImGui.Checkbox("顯示夜間模式核取方塊", ref C.ShowNightMode);
        ImGui.Checkbox("夜間模式處理僱員", ref C.NightModeRetainers);
        ImGui.Checkbox("夜間模式處理載具", ref C.NightModeDeployables);
        ImGui.Checkbox("讓夜間模式狀態保持不變", ref C.NightModePersistent);
        ImGui.Checkbox("讓關機指令改為啟用夜間模式，而不是關閉遊戲", ref C.ShutdownMakesNightMode);
    }
}
