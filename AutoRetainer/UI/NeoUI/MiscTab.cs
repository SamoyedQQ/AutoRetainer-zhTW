namespace AutoRetainer.UI.NeoUI;
public class MiscTab : NeoUIEntry
{
    public override string Path => "其他";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("統計")
        .Checkbox($"記錄探險統計", () => ref C.RecordStats)

        .Section("自動大國防聯軍精選交易")
        .Checkbox("繳交完成時顯示系統匣通知（需要 NotificationMaster）", () => ref C.GCHandinNotify)

        .Section("效能")

        .If(() => Utils.IsBusy)
        .Widget("", (x) => ImGui.BeginDisabled())
        .EndIf()

        .Checkbox($"外掛運作期間解除最小化時的 FPS 限制", () => ref C.UnlockFPS)
        .Checkbox($"－同時解除整體 FPS 限制", () => ref C.UnlockFPSUnlimited)
        .Checkbox($"－同時暫停 ChillFrames 外掛", () => ref C.UnlockFPSChillFrames)
        .Checkbox($"外掛運作期間提高 FFXIV 的行程優先權", () => ref C.ManipulatePriority, "可能會讓其他程式變慢")

        .If(() => Utils.IsBusy)
        .Widget("", (x) => ImGui.EndDisabled())
        .EndIf();
}
