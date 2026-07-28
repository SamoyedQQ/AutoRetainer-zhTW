namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeCommon : NeoUIEntry
{
    public override string Path => "多角模式/共用設定";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("共用設定")
        .Checkbox($"停留在登入畫面等待", () => ref C.MultiWaitOnLoginScreen, "若目前沒有任何角色可以派遣探險，會先登出，直到有角色可用為止。啟用本選項與多角模式期間，標題畫面動畫會被停用。")
        .Checkbox($"手動登入時停用多角模式", () => ref C.MultiDisableOnRelog, "透過 AutoRetainer 介面或指令重新登入時，停用多角模式。")
        .Checkbox($"手動登入時不要重設優先角色", () => ref C.MultiNoPreferredReset, "透過 AutoRetainer 介面或指令重新登入時，不要重設優先角色。")
        .Checkbox("允許進入共用房屋", () => ref C.SharedHET)
        .Checkbox("即使停用多角模式，登入時仍嘗試進入房屋", () => ref C.HETWhenDisabled)
        .Checkbox("已經站在傳喚鈴旁時，不要為了僱員傳送或進入房屋", () => ref C.NoTeleportHetWhenNextToBell)

        .Section("遊戲啟動")
        .Checkbox($"遊戲啟動時啟用多角模式", () => ref C.MultiAutoStart)
        .Widget("遊戲啟動時自動登入", (x) =>
        {
            ImGui.SetNextItemWidth(150f);
            var names = C.OfflineData.Where(s => !s.Name.IsNullOrEmpty()).Select(s => $"{s.Name}@{s.World}");
            var dict = names.ToDictionary(s => s, s => Censor.Character(s));
            dict.Add("", "Disabled");
            dict.Add("~", "Last logged in character");
            ImGuiEx.Combo(x, ref C.AutoLogin, ["", "~", .. names], names: dict);
        })
        .SliderInt(150f, "延遲", () => ref C.AutoLoginDelay.ValidateRange(0, 60), 0, 20, "設定適當的延遲，讓外掛在登入前完全載入，也留一點時間讓你在需要時取消登入")

        .Section("背包警告")
        .InputInt(100f, $"僱員列表：剩餘背包空位警告", () => ref C.UIWarningRetSlotNum.ValidateRange(2, 1000))
        .InputInt(100f, $"僱員列表：剩餘探險次數警告", () => ref C.UIWarningRetVentureNum.ValidateRange(2, 1000))
        .InputInt(100f, $"載具列表：剩餘背包空位警告", () => ref C.UIWarningDepSlotNum.ValidateRange(2, 1000))
        .InputInt(100f, $"載具列表：剩餘燃料警告", () => ref C.UIWarningDepTanksNum.ValidateRange(20, 1000))
        .InputInt(100f, $"載具列表：剩餘修理套件警告", () => ref C.UIWarningDepRepairNum.ValidateRange(5, 1000))

        .Section("傳送")
        .Widget(() => ImGuiEx.Text("需要 Lifestream 外掛"))
        .Widget(() => ImGuiEx.PluginAvailabilityIndicator([new("Lifestream", new Version("2.2.1.1"))]))
        .TextWrapped("想讓此選項生效，必須為每個角色在 Lifestream 外掛中登記房屋，或改為啟用簡易傳送。")
        .TextWrapped("你可以在角色設定選單中為每個角色個別調整這些設定。")
        .Widget(() =>
        {
            if(Data != null && Data.GetAreTeleportSettingsOverriden())
            {
                ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, "目前角色的傳送選項為自訂設定。");
            }
        })
        .Checkbox("已啟用", () => ref C.GlobalTeleportOptions.Enabled)
        .Indent()
        .Checkbox("為了僱員傳送……", () => ref C.GlobalTeleportOptions.Retainers)
        .Indent()
        .Checkbox("……到個人房屋", () => ref C.GlobalTeleportOptions.RetainersPrivate)
        .Checkbox("……到公會房屋", () => ref C.GlobalTeleportOptions.RetainersFC)
        .Checkbox("……到公寓", () => ref C.GlobalTeleportOptions.RetainersApartment)
        .TextWrapped("若以上全部停用或失敗，會改為傳送到旅館。")
        .Unindent()
        .Checkbox("為了載具傳送到公會房屋", () => ref C.GlobalTeleportOptions.Deployables)
        .Checkbox("啟用簡易傳送", () => ref C.AllowSimpleTeleport)
        .Unindent()
        .Widget(() => ImGuiEx.HelpMarker("不必在 Lifestream 登記房屋也能傳送。注意：傳送功能本身仍然需要 Lifestream 外掛。\r\n\r\n警告：本選項比在 Lifestream 登記房屋更不穩定，非必要請勿使用。", EColor.RedBright, FontAwesomeIcon.ExclamationTriangle.ToIconString()))

        .Section("救援模組")
        .Checkbox("連線錯誤時自動關閉並重試登入", () => ref C.ResolveConnectionErrors, "斷線後 AutoRetainer 會嘗試重新登入。若連線階段已過期則不會嘗試登入。")
        .Widget(() => ImGuiEx.PluginAvailabilityIndicator([new("NoKillPlugin")]));
}
