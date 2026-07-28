namespace AutoRetainer.UI.NeoUI.Experiments;
public class Notifications : ExperimentUIEntry
{
    // 基底類別預設拿類別名稱當節點標題，翻譯表抓不到，所以這裡明寫一個。
    public override string Name => "通知";

    public override void Draw()
    {
        ImGui.Checkbox($"有僱員完成探險時顯示浮動視窗通知", ref C.NotifyEnableOverlay);
        ImGui.Checkbox($"在任務或戰鬥中不顯示浮動視窗", ref C.NotifyCombatDutyNoDisplay);
        ImGui.Checkbox($"包含其他角色", ref C.NotifyIncludeAllChara);
        ImGui.Checkbox($"忽略未在多角模式中啟用的其他角色", ref C.NotifyIgnoreNoMultiMode);
        ImGui.Checkbox($"在遊戲聊天顯示通知", ref C.NotifyDisplayInChatX);
        ImGuiEx.Text($"當遊戲未在前景時：（需安裝並啟用 NotificationMaster）");
        ImGui.Checkbox($"僱員可用時傳送桌面通知", ref C.NotifyDeskopToast);
        ImGui.Checkbox($"閃爍工作列", ref C.NotifyFlashTaskbar);
        ImGui.Checkbox($"AutoRetainer 已啟用或多角模式執行中時不通知", ref C.NotifyNoToastWhenRunning);
    }
}
