using AutoRetainer.Modules.Voyage;
using AutoRetainer.UI.MainWindow.MultiModeTab;
using AutoRetainerAPI;
using AutoRetainerAPI.Configuration;
using Dalamud.Interface.Components;
using ECommons.Configuration;
using ECommons.Funding;
using NightmareUI;

namespace AutoRetainer.UI.MainWindow;

internal unsafe class AutoRetainerWindow : Window
{
    private TitleBarButton LockButton;

    public AutoRetainerWindow() : base($"")
    {
        PatreonBanner.IsOfficialPlugin = () => true;
        LockButton = new()
        {
            Click = OnLockButtonClick,
            Icon = C.PinWindow ? FontAwesomeIcon.Lock : FontAwesomeIcon.LockOpen,
            IconOffset = new(3, 2),
            ShowTooltip = () => ImGui.SetTooltip("鎖定視窗位置與大小"),
        };
        SizeConstraints = new()
        {
            MinimumSize = new(250, 100),
            MaximumSize = new(9999, 9999)
        };
        P.WindowSystem.AddWindow(this);
        AllowPinning = false;
        TitleBarButtons.Add(new()
        {
            Click = (m) => { if(m == ImGuiMouseButton.Left) S.NeoWindow.IsOpen = true; },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("開啟設定視窗"),
        });
        TitleBarButtons.Add(LockButton);
    }

    private Action<string> SomeAction;

    private void OnLockButtonClick(ImGuiMouseButton m)
    {
        SomeAction += (s) => { };
        SomeAction -= (s) => { };
        if(m == ImGuiMouseButton.Left)
        {
            C.PinWindow = !C.PinWindow;
            LockButton.Icon = C.PinWindow ? FontAwesomeIcon.Lock : FontAwesomeIcon.LockOpen;
        }
    }

    public override void PreDraw()
    {
        var prefix = SchedulerMain.PluginEnabled ? $" [{SchedulerMain.Reason}]" : "";
        var tokenRem = TimeSpan.FromMilliseconds(Utils.GetRemainingSessionMiliSeconds());
        WindowName = $"{P.Name} {P.GetType().Assembly.GetName().Version}{prefix} | {FormatToken(tokenRem)}###AutoRetainer";
        if(C.PinWindow)
        {
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(C.WindowPos);
            ImGui.SetNextWindowSize(C.WindowSize);
        }
    }

    private string FormatToken(TimeSpan time)
    {
        if(time.TotalMilliseconds > 0)
        {
            if(time.Days > 0)
            {
                return $"Session expires in {time.Days} day{(time.Days == 1 ? "" : "s")}" + (time.Hours > 0 ? $" {time.Hours} hours" : "");
            }
            else
            {
                if(time.Hours > 0)
                {
                    return $"Session expires in {time.Hours} hours";
                }
                else
                {
                    return $"Session expires in less than an hour";
                }
            }
        }
        else
        {
            return "Session expired";
        }
    }
    public override void Draw()
    {
        //ImGuiEx.Text(GradientColor.Get(EColor.RedBright, EColor.YellowBright), "This version MUST NOT BE RUNNING UNATTENDED.");
        if(!C.AcceptedDisclamer)
        {
            new NuiBuilder()
                .Section("免責聲明")
                .TextWrapped(ImGuiColors.DalamudYellow, "請注意，嚴禁將 AutoRetainer 用於 RMT（現金交易）用途。 ")
                .TextWrapped(ImGuiColors.DalamudRed, "為了避免不必要的後果，使用 AutoRetainer 時請遵守以下原則：")
                .TextWrapped("1. 不要在遊戲聊天中承認自己有在用 AutoRetainer；")
                .TextWrapped("2. 不要讓 AutoRetainer 長時間無人看管地掛著；")
                .TextWrapped("3. 確保「遊玩＋AutoRetainer」的每日總時數不超過 16 小時，並且在每輪僱員／潛水艇檢查之間留有閒置時間；")
                .TextWrapped("4. 遇到用交易或聊天做所謂「機器人測試」的玩家，一律不要回應，直接把對方加入黑名單；")
                .TextWrapped("5. 若被 GM 詢問，一律聲稱所有操作都是手動進行，絕不承認使用外掛。")
                .TextWrapped("沒有遵守這些原則可能會讓你的帳號陷入風險。")
                .TextWrapped(GradientColor.Get(ImGuiColors.DalamudYellow, ImGuiColors.DalamudRed), "不得將 AutoRetainer 用於現金交易（RMT）或其他商業用途。若用於這些用途，一律不提供支援。")
                .Widget(() =>
                {
                    if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.Check, "同意並繼續"))
                    {
                        C.AcceptedDisclamer = true;
                        EzConfig.Save();
                    }
                })
                .Draw();
            return;
        }
        var e = SchedulerMain.PluginEnabledInternal;
        var disabled = MultiMode.Active && !ImGui.GetIO().KeyCtrl;

        if(disabled)
        {
            ImGui.BeginDisabled();
        }
        if(ImGui.Checkbox($"啟用 {P.Name}", ref e))
        {
            P.WasEnabled = false;
            if(e)
            {
                SchedulerMain.EnablePlugin(PluginEnableReason.Auto);
            }
            else
            {
                SchedulerMain.DisablePlugin();
            }
        }
        if(C.ShowDeployables && (VoyageUtils.Workshops.Contains(Svc.ClientState.TerritoryType) || VoyageScheduler.Enabled))
        {
            ImGui.SameLine();
            ImGui.Checkbox($"載具", ref VoyageScheduler.Enabled);
        }
        if(disabled)
        {
            ImGui.EndDisabled();
            ImGuiComponents.HelpMarker($"此選項由多角模式控制。按住 CTRL 可強制變更。");
        }

        if(P.WasEnabled)
        {
            ImGui.SameLine();
            ImGuiEx.Text(GradientColor.Get(ImGuiColors.DalamudGrey, ImGuiColors.DalamudGrey3, 500), $"已暫停");
        }

        ImGui.SameLine();
        if(ImGui.Checkbox("多角", ref MultiMode.Enabled))
        {
            MultiMode.OnMultiModeEnabled();
        }
        if(C.ShowNightMode)
        {
            ImGui.SameLine();
            if(ImGui.Checkbox("夜間", ref C.NightMode))
            {
                MultiMode.BailoutNightMode();
            }
        }
        if(C.DisplayMMType)
        {
            ImGui.SameLine();
            ImGuiEx.SetNextItemWidthScaled(100f);
            ImGuiEx.EnumCombo("##mode", ref C.MultiModeType);
        }
        if(C.CharEqualize && MultiMode.Enabled)
        {
            ImGui.SameLine();
            if(ImGui.Button("重設計數"))
            {
                MultiMode.CharaCnt.Clear();
            }
        }

        Svc.PluginInterface.GetIpcProvider<object>(ApiConsts.OnMainControlsDraw).SendMessage();

        if(IPC.Suppressed)
        {
            ImGuiEx.Text(ImGuiColors.DalamudRed, $"外掛運作已被其他外掛暫停。");
            ImGui.SameLine();
            if(ImGui.SmallButton("取消"))
            {
                IPC.Suppressed = false;
            }
        }

        if(P.TaskManager.IsBusy)
        {
            ImGui.SameLine();
            if(ImGui.Button($"中止 {P.TaskManager.NumQueuedTasks} 項工作"))
            {
                P.TaskManager.Abort();
            }
        }

        PatreonBanner.DrawRight();
        ImGuiEx.EzTabBar("tabbar", PatreonBanner.Text,
                        ("Retainers", MultiModeUI.Draw, null, true),
                        ("Deployables", WorkshopUI.Draw, null, true),
                        ("Troubleshooting", TroubleshootingUI.Draw, null, true),
                        ("Statistics", DrawStats, null, true),
                        ("About", CustomAboutTab.Draw, null, true)
                        );
        if(!C.PinWindow)
        {
            C.WindowPos = ImGui.GetWindowPos();
            C.WindowSize = ImGui.GetWindowSize();
        }
    }

    private void DrawStats()
    {
        NuiTools.ButtonTabs([[C.RecordStats ? new("Ventures", S.VentureStats.DrawVentures) : null, new("Gil", S.GilDisplay.Draw), new("FC Data", S.FCData.Draw)]]);
    }

    public override void OnClose()
    {
        EzConfig.Save();
        S.VentureStats.Data.Clear();
        MultiModeUI.JustRelogged = false;
    }

    public override void OnOpen()
    {
        MultiModeUI.JustRelogged = true;
    }
}
