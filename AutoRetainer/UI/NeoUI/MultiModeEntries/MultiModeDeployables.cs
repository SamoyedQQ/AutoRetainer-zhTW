namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeDeployables : NeoUIEntry
{
    public override string Path => "多角模式/載具";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("多角模式－載具")
        .Checkbox("等待航行完成", () => ref C.MultiModeWorkshopConfiguration.MultiWaitForAll, "啟用後，AutoRetainer 會等所有載具返航才登入該角色。若你因其他原因已經登入，它仍會重新派出已完成的潛水艇——除非全域設定「已登入時也要等待」也一併開啟。")
        .Indent()
        .Checkbox("已登入時也要等待", () => ref C.MultiModeWorkshopConfiguration.WaitForAllLoggedIn, "改變「等待航行完成」（全域與個別角色皆適用）的行為：AutoRetainer 在已登入時不再逐艘重新派出潛水艇，而是等到所有潛水艇都回來後才動作。")
        .InputInt(120f, "最長等待時間（分鐘）", () => ref C.MultiModeWorkshopConfiguration.MaxMinutesOfWaiting.ValidateRange(0, 9999), 10, 60, "若等待其他載具返航會超過這個分鐘數，AutoRetainer 會同時忽略「等待航行完成」與「已登入時也要等待」兩項設定。")
        .Unindent()
        .DragInt(60f, "提前重新登入門檻（秒）", () => ref C.MultiModeWorkshopConfiguration.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300, "在此角色的潛水艇可重新派出之前，AutoRetainer 要提前幾秒登入。")
        .DragInt(120f, "僱員探險處理截止時間（分鐘）", () => ref C.DisableRetainerVesselReturn.ValidateRange(0, 60), "設為大於 0 時，AutoRetainer 會在任一角色預定重新派出潛水艇的這幾分鐘前，停止處理所有僱員（並會一併考慮上述設定）。")
        .Checkbox("進入工坊時定期檢查公會寶庫的金幣", () => ref C.FCChestGilCheck, "進入公會工坊時會定期檢查公會寶庫，讓金幣統計保持最新。")
        .Indent()
        .SliderInt(150f, "檢查頻率（小時）", () => ref C.FCChestGilCheckCd, 0, 24 * 5)
        .Widget("重設冷卻", (x) =>
        {
            if(ImGuiEx.Button(x, C.FCChestGilCheckTimes.Count > 0)) C.FCChestGilCheckTimes.Clear();
        })
        .Unindent();
}
