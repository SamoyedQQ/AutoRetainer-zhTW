namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeFPSLimiter : NeoUIEntry
{
    public override string Path => "多角模式/FPS 限制器";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("FPS 限制器")
        .TextWrapped("FPS 限制器只在多角模式啟用時生效")
        .Widget("閒置時的目標影格率", (x) =>
        {
            ImGui.SetNextItemWidth(100f);
            UIUtils.SliderIntFrameTimeAsFPS(x, ref C.TargetMSPTIdle, C.ExtraFPSLockRange ? 1 : 10);
        })
        .Widget("閒置時的目標影格率", (x) =>
        {
            ImGui.SetNextItemWidth(100f);
            UIUtils.SliderIntFrameTimeAsFPS("Target frame rate when operating", ref C.TargetMSPTRunning, C.ExtraFPSLockRange ? 1 : 20);
        })
        .Checkbox("遊戲在前景時解除 FPS 鎖定", () => ref C.NoFPSLockWhenActive)
        .Checkbox($"允許設定更低的 FPS 限制值", () => ref C.ExtraFPSLockRange, "啟用此選項後，若多角模式出現任何錯誤，一律不提供支援")
        .Checkbox($"只在設定關機計時後才啟用限制器", () => ref C.FpsLockOnlyShutdownTimer);
}
