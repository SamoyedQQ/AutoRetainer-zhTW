using AutoRetainer.Modules.Voyage;
using Dalamud.Game;
using ECommons.GameHelpers;
using ECommons.Reflection;

namespace AutoRetainer.UI.MainWindow;
public static unsafe class TroubleshootingUI
{
    private static readonly Config EmptyConfig = new();
    public static void Draw()
    {
        ImGuiEx.TextWrapped("這一頁會檢查你的設定是否有常見問題，讓你在求助前先自行排除。");

        if(!Svc.ClientState.ClientLanguage.EqualsAny(ClientLanguage.Japanese, ClientLanguage.German, ClientLanguage.French, ClientLanguage.English))
        {
            Error($"Local publisher client detected. AutoRetainer was not tested to work with local publisher's FFXIV clients. Some or all functions may not work. Additionally, keep in mind that ottercorp's Chinese Dalamud fork collects telemetry about your pc, characters, used plugins and Dalamud configuration without your consent and without a possibility to opt-out.");
        }

        if(C.DontLogout)
        {
            Error("DontLogout debug option is enabled");
        }

        foreach(var x in C.OfflineData)
        {
            if(x.WorkshopEnabled)
            {
                var a = x.OfflineSubmarineData.Select(x => x.Name);
                if(a.Count() > a.Distinct().Count())
                {
                    Error($"Character {Censor.Character(x.Name, x.World)} has duplicate submersible names. Submersible names must be unique.");
                }
            }
        }

        if((C.GlobalTeleportOptions.Enabled || C.OfflineData.Any(x => x.TeleportOptionsOverride.Enabled == true)) && !Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "Lifestream" && x.IsLoaded))
        {
            Error("\"Teleportation is enabled but Lifestream plugin is not installed/loaded. AutoRetainer can not function in this configuration. Either disable teleportation or install Lifestream plugin.");
        }

        foreach(var x in C.SubmarineUnlockPlans)
        {
            if(x.EnforcePlan)
            {
                Info($"Submarine unlock plan {x.Name.NullWhenEmpty() ?? x.GUID} is set as enforced and will override any submarine settings if there is anything to unlock.");
            }
        }

        foreach(var x in C.SubmarineUnlockPlans)
        {
            if(x.EnforceDSSSinglePoint)
            {
                Info($"Submarine unlock plan {x.Name.NullWhenEmpty() ?? x.GUID} is set to deploy on single point in Deep sea site, and it will ignore unlock behavior that is manually set.");
            }
        }

        try
        {
            if(DalamudReflector.IsOnStaging())
            {
                Error($"Non-release Dalamud branch detected. This may cause issues. If possible, please open branch switcher by typing /xlbranch, change to \"release\" and restart your game.");
            }
        }
        catch(Exception e)
        {
        }

        if(Player.Available)
        {
            if(Player.CurrentWorld != Player.HomeWorld)
            {
                Error("You are visiting another world. You must return to your home world before AutoRetainer can continue working on this character.");
            }
            if(C.Blacklist.Any(x => x.CID == Player.CID))
            {
                Error("Your current character is excluded from AutoRetainer completely, prevenging it from being processed in any way. Go to settings - exclusions to change it.");
            }
            if(Data.ExcludeRetainer)
            {
                Error("Your current character is excluded from retainer list. Go to settings - exclusions to change it.");
            }
            if(Data.ExcludeWorkshop)
            {
                Error("Your current character is excluded from deployable list. Go to settings - exclusions to change it.");
            }
        }

        {
            var list = C.OfflineData.Where(x => x.GetAreTeleportSettingsOverriden());
            if(list.Any())
            {
                Info("For some of your characters, teleportation options are customized. Hover to see list.", list.Select(x => $"{x.Name}@{x.World}").Print("\n"));
            }
        }

        if(C.NoTeleportHetWhenNextToBell)
        {
            Warning("Teleporting or entering house/apartment is disabled when character is next to retainer bell. Pay attention to house demolition timer.");
        }



        if(C.AllowSimpleTeleport)
        {
            Warning("Simple Teleport option is enabled. It's less reliable than registering your houses with Lifestream. If you are experiencing issues with teleportation, consider disabling this option and registering your property with Lifestream.");
        }

        if(!C.EnableEntrustManager && C.AdditionalData.Any(x => x.Value.EntrustPlan != Guid.Empty))
        {
            Warning($"Entrust manager is globally disabled, while some retainers have their entrust plans assigned. Entrust plans will only be processed manually.");
        }

        if(C.ExtraDebug)
        {
            Info("Extra logging option active. It will spam your log. Only use it when collecting debug information.");
        }

        if(C.UnsyncCompensation > -5)
        {
            Warning("Time Desynchronization Compensation is set too high (>-5). This may cause issues.");
        }

        if(UIUtils.GetFPSFromMSPT(C.TargetMSPTIdle) < 10)
        {
            Warning("Your target frame rate when idling is set too low (<10). This may cause issues.");
        }

        if(UIUtils.GetFPSFromMSPT(C.TargetMSPTRunning) < 20)
        {
            Warning("Your target frame rate when operating is set too low (<20). This may cause issues.");
        }

        if(Data?.GetIMSettings().AllowSellFromArmory == true)
        {
            Info("Allow selling items from Armory Chest is enabled. Make sure to add your savage gear and ultimate weapons to protection list.");
        }

        {
            var list = C.OfflineData.Where(x => !x.ExcludeRetainer && !x.Enabled && x.RetainerData.Count > 0);
            if(list.Any())
            {
                Warning($"Some of your characters are not enabled for Retainer Multi Mode even though they have retainers. Hover to see list.", list.Print("\n"));
            }
        }
        {
            var list = C.OfflineData.Where(x => !x.ExcludeRetainer && x.Enabled && x.RetainerData.Count > 0 && C.SelectedRetainers.TryGetValue(x.CID, out var rd) && !x.RetainerData.All(r => rd.Contains(r.Name)));
            if(list.Any())
            {
                Warning($"Some of your characters have not all retainers enabled for processing. Hover to see list.", list.Print("\n"));
            }
        }
        {
            var list = C.OfflineData.Where(x => !x.ExcludeWorkshop && !x.WorkshopEnabled && (x.OfflineSubmarineData.Count + x.OfflineAirshipData.Count) > 0);
            if(list.Any())
            {
                Warning($"Some of your characters are not enabled for Deployables Multi Mode even though they have deployables registered. Hover to see list.", list.Print("\n"));
            }
        }

        {
            var list = C.OfflineData.Where(x => !x.ExcludeWorkshop && x.WorkshopEnabled && x.GetEnabledVesselsData(Internal.VoyageType.Airship).Count + x.GetEnabledVesselsData(Internal.VoyageType.Submersible).Count < Math.Min(x.OfflineAirshipData.Count + x.OfflineSubmarineData.Count, 4));
            if(list.Any())
            {
                Warning($"Some of your characters have not all deployables enabled for processing. Hover to see list.", list.Print("\n"));
            }
        }

        if(C.MultiModeType != AutoRetainerAPI.Configuration.MultiModeType.Everything)
        {
            Warning($"Your MultiMode type is set to {C.MultiModeType}. This will limit functions that AutoRetainer will perform.");
        }

        if(C.OfflineData.Any(x => x.MultiWaitForAllDeployables))
        {
            Info("Some characters have \"Wait For All Pending Deployables\" option enabled. This means that for these characters AutoRetainer will wait for all deployables to return before processing them. Hover to see complete list of characters with enabled option.", C.OfflineData.Where(x => x.MultiWaitForAllDeployables).Select(x => $"{x.Name}@{x.World}").Print("\n"));
        }

        if(C.MultiModeWorkshopConfiguration.MultiWaitForAll)
        {
            Info("Global option \"Wait For Venture Completion\" is enabled. This means that for all characters AutoRetainer will wait for all deployables to return before processing them, even for these whose per-character option is disabled.");
        }

        if(C.MultiModeWorkshopConfiguration.WaitForAllLoggedIn)
        {
            Info("Option \"Wait even when already logged in\" is enabled for deployables. This means that AutoRetainer will wait for all deployables on a character to be completed before processing them even when you are logged in.");
        }

        if(C.DisableRetainerVesselReturn > 0)
        {
            if(C.DisableRetainerVesselReturn > 10)
            {
                Warning("Option \"Retainer venture processing cutoff\" is set to abnormally high value. You may experience significant delays with resending retainers when deployables are soon to be available.");
            }
            else
            {
                Info("Option \"Retainer venture processing cutoff\" is enabled. You may experience delays with resending retainers when deployables are soon to be available.");
            }
        }

        if(C.MultiModeRetainerConfiguration.MultiWaitForAll)
        {
            Info("Option \"Wait For Venture Completion\" is enabled. This means that AutoRetainer will wait for all ventures from all retainers on a character to be completed before logging in to process them.");
        }

        if(C.MultiModeRetainerConfiguration.WaitForAllLoggedIn)
        {
            Info("Option \"Wait even when already logged in\" is enabled for retainers. This means that AutoRetainer will wait for all ventures from all retainers on a character to be completed before processing them even when you are logged in.");
        }

        {
            var manualList = new List<string>();
            var deletedList = new List<string>();
            foreach(var x in C.OfflineData)
            {
                foreach(var ret in x.RetainerData)
                {
                    var planId = Utils.GetAdditionalData(x.CID, ret.Name).EntrustPlan;
                    var plan = C.EntrustPlans.FirstOrDefault(s => s.Guid == planId);
                    if(plan != null && plan.ManualPlan) manualList.Add($"{Censor.Character(x.Name)} - {Censor.Retainer(ret.Name)}");
                    if(plan == null && planId != Guid.Empty) deletedList.Add($"{Censor.Character(x.Name)} - {Censor.Retainer(ret.Name)}");
                }
            }
            if(manualList.Count > 0)
            {
                Info("Some of your retainers have manual entrust plans set. These plans won't be processed automatically after resending retainer for venture, but only manually upon clicking button in overlay. Hover to see the list.", manualList.Print("\n"));
            }
            if(deletedList.Count > 0)
            {
                Warning("Some of your retainers' entrust plans were deleted before. Retainers with deleted entrust plans will not entrust anything. Hover to see list.", deletedList.Print("\n"));
            }
        }

        if(C.No2ndInstanceNotify)
        {
            Info("You have \"Do not warn about second game instance running from same directory\" option enabled, which will skip AutoRetainer's loading on 2nd instance of the game running with the same Dalamud directory automatically.");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "SimpleTweaksPlugin" && x.IsLoaded))
        {
            Info("Simple Tweaks plugin detected. Any tweaks related to retainers or submarines may affect AutoRetainer functions negatively. Please ensure that tweaks are configured in a way to not interfere with AutoRetainer functions.");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "PandorasBox" && x.IsLoaded))
        {
            Info("Pandora's Box plugin detected. Automatic use of actions while AutoRetainer is enabled may affect AutoRetainer functions negatively. Please ensure that Pandora's Box is configured in a way to not automatically use actions while AutoRetainer is active.");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "Automaton" && x.IsLoaded))
        {
            Info("Automaton plugin detected. Automatic use of actions and automatic numeric inputs while AutoRetainer is enabled may affect AutoRetainer functions negatively. Please ensure that Automaton is configured in a way to not use automatically actions while AutoRetainer is active.");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "RotationSolver" && x.IsLoaded))
        {
            Info("RotationSolver plugin detected. Automatic use of actions while AutoRetainer is enabled may affect AutoRetainer functions negatively. Please ensure that RotationSolver is configured in a way to not automatically use actions while AutoRetainer is active.");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName.StartsWith("BossMod") && x.IsLoaded))
        {
            Info("BossMod plugin detected. Automatic use of actions while AutoRetainer is enabled may affect AutoRetainer functions negatively. Please ensure that BossMod is configured in a way to not automatically use actions while AutoRetainer is active.");
        }

        ImGui.Separator();
        ImGuiEx.TextWrapped("專家設定會改變開發者原本設計的行為。回報問題前請先確認不是專家設定沒設好造成的。");
        CheckExpertSetting("使用傳喚鈴且沒有探險可領取時的動作", nameof(C.OpenBellBehaviorNoVentures));
        CheckExpertSetting("使用傳喚鈴且有探險可領取時的動作", nameof(C.OpenBellBehaviorWithVentures));
        CheckExpertSetting("使用傳喚鈴後的工作完成行為", nameof(C.TaskCompletedBehaviorAccess));
        CheckExpertSetting("手動啟用後的工作完成行為", nameof(C.TaskCompletedBehaviorManual));
        CheckExpertSetting("若有僱員將在 5 分鐘內完成探險，就留在僱員選單", nameof(C.Stay5));
        CheckExpertSetting("關閉僱員列表時自動停用外掛", nameof(C.AutoDisable));
        CheckExpertSetting("不顯示外掛狀態圖示", nameof(C.HideOverlayIcons));
        CheckExpertSetting("顯示多角模式類型選擇器", nameof(C.DisplayMMType));
        CheckExpertSetting("在工坊顯示載具核取方塊", nameof(C.ShowDeployables));
        CheckExpertSetting("啟用救援模組", nameof(C.EnableBailout));
        CheckExpertSetting("AutoRetainer 嘗試脫困前的逾時時間（秒）", nameof(C.BailoutTimeout));
        CheckExpertSetting("停用排序與展開／收合", nameof(C.NoCurrentCharaOnTop));
        CheckExpertSetting("在外掛介面列顯示多角模式核取方塊", nameof(C.MultiModeUIBar));
        CheckExpertSetting("僱員選單延遲（秒）", nameof(C.RetainerMenuDelay));
        CheckExpertSetting("不對探險規劃器做錯誤檢查", nameof(C.NoErrorCheckPlanner2));
        CheckExpertSetting("啟用多角模式時，嘗試進入附近的房屋", nameof(C.MultiHETOnEnable));
        CheckExpertSetting("Artisan 整合", nameof(C.ArtisanIntegration));
        CheckExpertSetting("使用伺服器時間而非電腦時間", nameof(C.UseServerTime));
    }

    private static void Error(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.RedBright, "\uf057");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.RedBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void Warning(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.OrangeBright, "\uf071");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.OrangeBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void Info(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.YellowBright, "\uf05a");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.YellowBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void CheckExpertSetting(string setting, string nameOfSetting)
    {
        var original = EmptyConfig.GetFoP(nameOfSetting);
        var current = C.GetFoP(nameOfSetting);
        if(!original.Equals(current))
        {
            Info($"Expert setting \"{setting}\" differs from default", $"Default is \"{original}\", current is \"{current}\".");
        }
    }
}
