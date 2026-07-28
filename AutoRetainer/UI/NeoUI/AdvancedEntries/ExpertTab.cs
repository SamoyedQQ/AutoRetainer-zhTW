using ECommons.Configuration;
using ECommons.Reflection;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries;
public class ExpertTab : NeoUIEntry
{
    public override string Path => "進階/專家設定";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("行為")
        .EnumComboFullWidth(null, "使用傳喚鈴且沒有探險可領取時的動作：", () => ref C.OpenBellBehaviorNoVentures)
        .EnumComboFullWidth(null, "使用傳喚鈴且有探險可領取時的動作：", () => ref C.OpenBellBehaviorWithVentures)
        .EnumComboFullWidth(null, "使用傳喚鈴後的工作完成行為：", () => ref C.TaskCompletedBehaviorAccess)
        .EnumComboFullWidth(null, "手動啟用後的工作完成行為：", () => ref C.TaskCompletedBehaviorManual)
        .EnumComboFullWidth(null, "外掛運作期間的工作完成行為：", () => ref C.TaskCompletedBehaviorAuto)
        .TextWrapped(ImGuiColors.DalamudGrey, "多角模式運作期間，上述三項設定會強制套用「關閉僱員列表並停用外掛」。")
        .Checkbox("若有僱員將在 5 分鐘內完成探險，就留在僱員選單", () => ref C.Stay5, "多角模式運作期間會強制套用此選項。")
        .Checkbox($"關閉僱員列表時自動停用外掛", () => ref C.AutoDisable, "只有在你自己離開選單時才適用，其他情況套用上方的設定。")
        .Checkbox($"不顯示外掛狀態圖示", () => ref C.HideOverlayIcons)
        .Checkbox($"顯示多角模式類型選擇器", () => ref C.DisplayMMType)
        .Checkbox($"在工坊顯示載具核取方塊", () => ref C.ShowDeployables)
        .Checkbox("啟用救援模組", () => ref C.EnableBailout)
        .InputInt(150f, "AutoRetainer 嘗試脫困前的逾時時間（秒）", () => ref C.BailoutTimeout)

        .Section("設定")
        .Checkbox($"停用排序與展開／收合", () => ref C.NoCurrentCharaOnTop)
        .Checkbox($"在外掛介面列顯示多角模式核取方塊", () => ref C.MultiModeUIBar)
        .SliderIntAsFloat(100f, "僱員選單延遲（秒）", () => ref C.RetainerMenuDelay.ValidateRange(0, 2000), 0, 2000)
        .Checkbox($"允許探險計時顯示負數", () => ref C.TimerAllowNegative)
        .Checkbox($"不對探險規劃器做錯誤檢查", () => ref C.NoErrorCheckPlanner2)
        .Checkbox("手動重新登入時執行角色後處理", () => ref C.AllowManualPostprocess, "AutoRetainer 在後處理鎖定期間仍允許手動執行指令。 ")
        .Widget("市場冷卻浮動視窗", (x) =>
        {
            if(ImGui.Checkbox(x, ref C.MarketCooldownOverlay))
            {
                if(C.MarketCooldownOverlay)
                {
                    P.Memory.OnReceiveMarketPricePacketHook?.Enable();
                }
                else
                {
                    P.Memory.OnReceiveMarketPricePacketHook?.Disable();
                }
            }
        })

        .Section("整合")
        .Checkbox($"Artisan 整合", () => ref C.ArtisanIntegration, "當探險可以領取且傳喚鈴在範圍內時，會暫停 Artisan 的作業並自動啟用 AutoRetainer。探險處理完畢後，Artisan 會重新啟用並繼續原本的工作。")

        .Section("伺服器時間")
        .Checkbox("使用伺服器時間而非電腦時間", () => ref C.UseServerTime)

        .Section("工具")
        .Widget("清除幽靈僱員", (x) =>
        {
            if(ImGui.Button(x))
            {
                var i = 0;
                foreach(var d in C.OfflineData)
                {
                    i += d.RetainerData.RemoveAll(x => x.Name == "");
                }
                DuoLog.Information($"Cleaned {i} entries");
            }
        })

        .Section("匯入／匯出")
        .Widget(() =>
        {
            if(ImGui.Button("匯出（不含角色資料）"))
            {
                var clone = C.JSONClone();
                clone.OfflineData = null;
                clone.AdditionalData = null;
                clone.FCData = null;
                clone.SelectedRetainers = null;
                clone.Blacklist = null;
                clone.AutoLogin = "";
                Copy(EzConfig.DefaultSerializationFactory.Serialize(clone, false));
            }
            if(ImGui.Button("匯入並與角色資料合併"))
            {
                try
                {
                    var c = EzConfig.DefaultSerializationFactory.Deserialize<Config>(Paste());
                    c.OfflineData = C.OfflineData;
                    c.AdditionalData = C.AdditionalData;
                    c.FCData = C.FCData;
                    c.SelectedRetainers = C.SelectedRetainers;
                    c.Blacklist = C.Blacklist;
                    c.AutoLogin = C.AutoLogin;
                    if(c.GetType().GetFieldPropertyUnions().Any(x => x.GetValue(c) == null)) throw new NullReferenceException();
                    EzConfig.SaveConfiguration(C, $"Backup_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.json");
                    P.SetConfig(c);
                }
                catch(Exception e)
                {
                    e.LogDuo();
                }
            }
        });
}
