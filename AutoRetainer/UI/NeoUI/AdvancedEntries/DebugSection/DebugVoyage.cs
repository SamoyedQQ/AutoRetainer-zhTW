using AutoRetainer.Internal;
using AutoRetainer.Modules.Voyage;
using AutoRetainer.Modules.Voyage.Tasks;
using AutoRetainer.Modules.Voyage.VoyageCalculator;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Diagnostics;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugVoyage : DebugSectionBase
{
    private static string data1 = "";
    private static VoyageType data2 = default;
    private static int r1, r2, r3, r4, r5 = -1;
    public override void Draw()
    {
        if(ImGui.CollapsingHeader("除錯"))
        {
            try
            {
                var h = HousingManager.Instance()->WorkshopTerritory;
                if(h != null)
                {
                    foreach(var x in h->Submersible.Data)
                    {
                        ImGuiEx.Text($"{x.Name.Read()}/{x.ReturnTime}/{x.CurrentExplorationPoints.ToArray().Print()}");
                    }
                }
                if(ImGui.Button("清除離線資料"))
                {
                    Data.OfflineAirshipData.Clear();
                    Data.OfflineSubmarineData.Clear();
                }
                if(ImGui.Button("修理 1")) VoyageScheduler.TryRepair(0);
                if(ImGui.Button("修理 2")) VoyageScheduler.TryRepair(1);
                if(ImGui.Button("修理 3")) VoyageScheduler.TryRepair(2);
                if(ImGui.Button("修理 4")) VoyageScheduler.TryRepair(3);
                if(ImGui.Button("關閉修理視窗")) VoyageScheduler.CloseRepair();
                //if (ImGui.Button("Trigger auto repair")) TaskRepairAll.EnqueueImmediate();
                ImGui.InputText("data1", ref data1, 50);
                ImGuiEx.EnumCombo("data2", ref data2);
                if(CurrentSubmarine.Get() != null)
                {
                    ImGuiEx.Text($"{CurrentSubmarine.Get()->CurrentExp}/{CurrentSubmarine.Get()->NextLevelExp}");
                }
                ImGuiEx.Text($"位於航行面板：{VoyageUtils.IsInVoyagePanel()}，{Lang.PanelName}");
                if(ImGui.Button("IsVesselNeedsRepair"))
                {
                    try
                    {
                        DuoLog.Information($"{VoyageUtils.GetIsVesselNeedsRepair(data1, data2, out var log).Print()}\n{log.Join("\n")}");
                    }
                    catch(Exception e)
                    {
                        e.LogDuo();
                    }
                }
                if(ImGui.Button("GetSubmarineIndexByName"))
                {
                    try
                    {
                        DuoLog.Information($"{VoyageUtils.GetVesselIndexByName(data1, VoyageType.Submersible)}");
                    }
                    catch(Exception e)
                    {
                        e.LogDuo();
                    }
                }
                ImGuiEx.Text($"可用傳喚鈴：{Utils.GetReachableRetainerBell(false)}");
                ImGuiEx.Text($"可用傳喚鈴（含遠距）：{Utils.GetReachableRetainerBell(true)}");
                ImGuiEx.TextWrapped($"已啟用的潛水艇：{Data.GetVesselData(VoyageType.Submersible).Select(x => $"{x.Name}, {x.GetRemainingSeconds()}").Print()}");
                ImGuiEx.Text($"有已啟用的載具可用：{Data.AnyEnabledVesselsAvailable()}");
                ImGuiEx.Text($"面板類型：{VoyageUtils.GetCurrentWorkshopPanelType()}");
                if(TryGetAddonByName<AtkUnitBase>("AirShipExplorationResult", out var addon) && IsAddonReady(addon))
                {
                    var button = addon->UldManager.NodeList[3]->GetAsAtkComponentButton();
                    ImGuiEx.Text($"按鈕：{button->IsEnabled}");
                }
                if(ImGui.Button("與最近的面板互動"))
                {
                    TaskInteractWithNearestPanel.Enqueue();
                }
            }
            catch(Exception e)
            {
                ImGuiEx.TextWrapped(e.ToString());
            }
        }
        ImGuiEx.Text($"僱員被航行卡住：{VoyageUtils.IsRetainerBlockedByVoyage()}");
        if(ImGui.CollapsingHeader("data"))
        {
            try
            {
                ImGuiEx.Text($"目前指標：{(nint)CurrentSubmarine.Get()}");
                if(CurrentSubmarine.Get() != null)
                {
                    ImGuiEx.Text($"名稱：{CurrentSubmarine.Get()->Name.Read()}");
                    ImGuiEx.Text($"艇身：{CurrentSubmarine.Get()->HullId}");
                    ImGuiEx.Text($"→艇尾 ID：{CurrentSubmarine.Get()->SternId}");
                    ImGuiEx.Text($"艦橋 ID：{CurrentSubmarine.Get()->BridgeId}");
                    ImGuiEx.Text($"艇首 ID：{CurrentSubmarine.Get()->BowId}");
                    ImGuiEx.Text($"階級 ID：{CurrentSubmarine.Get()->RankId}");
                    if(ImGui.Button("輸出最佳經驗路線"))
                    {
                        CurrentSubmarine.GetBestExps();
                    }
                    if(ImGui.Button("選擇最佳路線"))
                    {
                        TaskCalculateAndPickBestExpRoute.Enqueue();
                    }
                    ImGuiEx.Text($"探索點：{CurrentSubmarine.Get()->CurrentExplorationPoints.ToArray().Print()}");
                    ImGuiEx.Text($"探索點：{CurrentSubmarine.Get()->CurrentExplorationPoints.ToArray().Select(x => VoyageUtils.GetSubmarineExplorationName(x)).Print()}");
                }
            }
            catch(Exception e)
            {
                ImGuiEx.TextWrapped(e.ToString());
            }
            var curPlotId = (long*)(Process.GetCurrentProcess().MainModule.BaseAddress + 0x215FB68);
            ImGuiEx.TextCopy($"Plot ID: {*curPlotId:X16}");
            ImGuiEx.Text($"房屋 ID：{HousingManager.Instance()->GetCurrentIndoorHouseId()}");
            if(HousingManager.Instance()->WorkshopTerritory != null)
            {
                ImGuiEx.Text($"飛空艇數量：{HousingManager.Instance()->WorkshopTerritory->Airship.AirshipCount}");
                //ImGuiEx.Text($"Num w: {HousingManager.Instance()->WorkshopTerritory->Submersible.DataList}");
                {
                    var data = HousingManager.Instance()->WorkshopTerritory->Airship.Data;
                    for(var i = 0; i < data.Length; i++)
                    {
                        var d = data[i];
                        ImGuiEx.Text($"飛空艇：{d.Name.Read()}，返航時間 {d.GetReturnTime()}，目前經驗 {d.CurrentExp}");
                    }
                }
                {
                    var data = HousingManager.Instance()->WorkshopTerritory->Submersible.Data;
                    for(var i = 0; i < data.Length; i++)
                    {
                        var d = data[i];
                        ImGuiEx.Text($"潛水艇：{d.Name.Read()}，返航時間 {d.GetReturnTime()}，目前經驗 {d.CurrentExp}");
                    }
                }
            }
        }
        if(ImGui.CollapsingHeader("utils"))
        {
            ImGui.InputInt("r1", ref r1);
            if(ImGui.Button("選擇"))
            {
                P.Memory.SelectRoutePointUnsafe(r1);
            }
        }
        if(ImGui.CollapsingHeader("control"))
        {
            if(ImGui.Button($"{nameof(VoyageScheduler.Lockon)}")) DuoLog.Information($"{VoyageScheduler.Lockon()}");
            if(ImGui.Button($"{nameof(VoyageScheduler.Approach)}")) DuoLog.Information($"{VoyageScheduler.Approach()}");
            if(ImGui.Button($"{nameof(VoyageScheduler.AutomoveOffPanel)}")) DuoLog.Information($"{VoyageScheduler.AutomoveOffPanel()}");
            if(ImGui.Button($"{nameof(VoyageScheduler.InteractWithVoyagePanel)}")) DuoLog.Information($"{VoyageScheduler.InteractWithVoyagePanel()}");
            if(ImGui.Button($"{nameof(VoyageScheduler.SelectAirshipManagement)}")) DuoLog.Information($"{VoyageScheduler.SelectAirshipManagement()}");
            if(ImGui.Button($"{nameof(VoyageScheduler.SelectSubManagement)}")) DuoLog.Information($"{VoyageScheduler.SelectSubManagement()}");
            ImGui.InputText("subject name", ref data1, 100);
            if(ImGui.Button($"{nameof(VoyageScheduler.SelectVesselByName)}")) DuoLog.Information($"{VoyageScheduler.SelectVesselByName(data1, VoyageType.Submersible)}");
            if(ImGui.Button($"{nameof(VoyageScheduler.RedeployVessel)}")) DuoLog.Information($"{VoyageScheduler.RedeployVessel()}");
            if(ImGui.Button($"{nameof(VoyageScheduler.DeployVessel)}")) DuoLog.Information($"{VoyageScheduler.DeployVessel()}");
            if(ImGui.Button($"{nameof(TaskDeployOnBestExpVoyage.Deploy)}")) DuoLog.Information($"{TaskDeployOnBestExpVoyage.Deploy()}");
            //if (ImGui.Button($"{nameof(TaskDeployOnBestExpVoyage)}")) TaskDeployOnBestExpVoyage.Enqueue();
            if(ImGui.Button($"{nameof(VoyageScheduler.Approach)}")) DuoLog.Information($"{VoyageScheduler.Approach}");
        }
        if(ImGui.CollapsingHeader("測試工作管理器"))
        {
            if(ImGui.Button("測試重新派出飛空艇"))
            {
                P.TaskManager.Enqueue(VoyageScheduler.Lockon);
                P.TaskManager.Enqueue(VoyageScheduler.Approach);
                P.TaskManager.Enqueue(VoyageScheduler.AutomoveOffPanel);
                P.TaskManager.Enqueue(VoyageScheduler.InteractWithVoyagePanel);
                P.TaskManager.Enqueue(VoyageScheduler.SelectAirshipManagement);
                P.TaskManager.Enqueue(() => VoyageScheduler.SelectVesselByName(data1, VoyageType.Airship));
                P.TaskManager.Enqueue(VoyageScheduler.WaitUntilFinalizeDeployAddonExists);
                P.TaskManager.Enqueue(VoyageScheduler.RedeployVessel);
                P.TaskManager.Enqueue(VoyageScheduler.DeployVessel);
            }
        }
    }
}
