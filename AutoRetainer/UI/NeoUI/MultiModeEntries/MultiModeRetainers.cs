namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeRetainers : NeoUIEntry
{
    public override string Path => "多角模式/僱員";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("多角模式－僱員")
        .Checkbox("等待探險完成", () => ref C.MultiModeRetainerConfiguration.MultiWaitForAll, "多角模式運作時，AutoRetainer 會等所有僱員都回來後才輪到下一個角色。")
        .DragInt(60f, "提前重新登入門檻", () => ref C.MultiModeRetainerConfiguration.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300)
        .SliderInt(100f, "繼續運作所需的最少背包空位", () => ref C.MultiMinInventorySlots.ValidateRange(2, 9999), 2, 30)
        .Checkbox("同步僱員（單次）", () => ref MultiMode.Synchronize, "AutoRetainer 會等到所有啟用的僱員都完成探險為止。之後本設定會自動停用，並開始處理所有角色。")
        .Checkbox($"強制完整輪過所有角色", () => ref C.CharEqualize, "建議角色超過 15 個的玩家使用。會強制多角模式依序處理完所有角色的探險，才回到循環開頭。")
        .Indent()
        .Checkbox("依探險完成時間排序角色", () => ref C.LongestVentureFirst, "探險完成時間較早的角色會優先被檢查")
        .Checkbox("依僱員等級與卡等狀況排序角色", () => ref C.CappedLevelsLast, "先處理有可升級僱員的角色，接著是僱員已滿等的角色，最後是僱員未滿等但已卡等的角色。")
        .Unindent();
}
