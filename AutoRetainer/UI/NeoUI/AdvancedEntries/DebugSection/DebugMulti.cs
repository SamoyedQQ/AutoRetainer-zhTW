using AutoRetainer.Internal;
using AutoRetainer.Scheduler.Tasks;
using Dalamud.Utility;
using ECommons.Automation.NeoTaskManager.Tasks;
using ECommons.ExcelServices;
using ECommons.ExcelServices.TerritoryEnumeration;
using ECommons.GameHelpers;
using ECommons.Reflection;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugMulti : DebugSectionBase
{
    public override void Draw()
    {
        if(ImGui.CollapsingHeader("已排序的資料"))
        {
            ImGuiEx.Text($"{MultiMode.GetRetainerSortedOfflineDatas(true).Where(x => !x.ExcludeRetainer).Select(x => $"{x.Name}@{x.World}").Print("\n")}");
        }
        if(ImGui.CollapsingHeader("NeoHET"))
        {
            if(ImGui.Button("把進屋傳送加入佇列")) TaskNeoHET.Enqueue(null);
            if(ImGui.Button("把工坊加入佇列")) TaskNeoHET.TryEnterWorkshop(() => DuoLog.Error("Fail"));
            ImGuiEx.Text($"""
                Can enter workshop: {S.LifestreamIPC.CanMoveToWorkshop()}
                """);
        }
        if(ImGui.CollapsingHeader("工作"))
        {
            if(ImGui.Button("TestAutomoveTask")) P.TaskManager.EnqueueTask(NeoTasks.ApproachObjectViaAutomove(() => Svc.Targets.FocusTarget));
            if(ImGui.Button("TestInteractTask")) P.TaskManager.EnqueueTask(NeoTasks.InteractWithObject(() => Svc.Targets.FocusTarget));
            if(ImGui.Button("兩者都測試"))
            {
                P.TaskManager.EnqueueTask(NeoTasks.ApproachObjectViaAutomove(() => Svc.Targets.FocusTarget));
                P.TaskManager.EnqueueTask(NeoTasks.InteractWithObject(() => Svc.Targets.FocusTarget));
            }
        }
        ImGui.Checkbox("不要登出", ref C.DontLogout);
        ImGui.Checkbox("已啟用", ref MultiMode.Enabled);
        ImGuiEx.Text($"預期角色：{TaskChangeCharacter.Expected}");
        if(ImGui.Button("強制製造不一致")) TaskChangeCharacter.Expected = ("AAAAAAAA", "BBBBBBB");
        if(ImGui.Button("模擬已無剩餘項目"))
        {
            MultiMode.Relog(null, out var error, RelogReason.MultiMode);
        }
        if(ImGui.Button($"模擬自動啟動"))
        {
            MultiMode.PerformAutoStart();
        }
        if(ImGui.Button("刪除已載入的資料"))
        {
            DalamudReflector.DeleteSharedData("AutoRetainer.WasLoaded");
        }
        ImGuiEx.Text($"移動中：{AgentMap.Instance()->IsPlayerMoving}");
        ImGuiEx.Text($"忙碌中：{IsOccupied()}");
        ImGuiEx.Text($"詠唱中：{Player.Object?.IsCasting}");
        ImGuiEx.TextCopy($"CID: {Player.CID}");
        ImGuiEx.Text($"{Svc.Data.GetExcelSheet<Addon>()?.GetRow(115).Text.ToDalamudString().GetText()}");
        ImGuiEx.Text($"伺服器時間：{CSFramework.GetServerTime()}");
        ImGuiEx.Text($"電腦時間：{DateTimeOffset.Now.ToUnixTimeSeconds()}");
        if(ImGui.CollapsingHeader("HET"))
        {
            ImGuiEx.Text($"最近的入口：{Utils.GetNearestEntrance(out var d)}，距離={d}");
            if(ImGui.Button("進入房屋"))
            {
                TaskNeoHET.Enqueue(null);
            }
        }
        if(ImGui.CollapsingHeader("房屋區域"))
        {
            ImGuiEx.Text(ResidentalAreas.List.Select(x => GenericHelpers.GetTerritoryName(x)).Join("\n"));
            ImGuiEx.Text($"位於住宅區：{ResidentalAreas.List.Contains(Svc.ClientState.TerritoryType)}");
        }
        ImGuiEx.Text($"位於休息區：{TerritoryInfo.Instance()->InSanctuary}");
        ImGuiEx.Text($"位於休息區（依 Excel 判定）：{ExcelTerritoryHelper.IsSanctuary(Svc.ClientState.TerritoryType)}");
        ImGui.Checkbox($"略過休息區檢查", ref C.BypassSanctuaryCheck);
        if(Svc.ClientState.LocalPlayer != null && Svc.Targets.Target != null)
        {
            ImGuiEx.Text($"與目標的距離：{Vector3.Distance(Svc.ClientState.LocalPlayer.Position, Svc.Targets.Target.Position)}");
            ImGuiEx.Text($"目標判定半徑：{Svc.Targets.Target.HitboxRadius}");
            ImGuiEx.Text($"與目標判定範圍的距離：{Vector3.Distance(Svc.ClientState.LocalPlayer.Position, Svc.Targets.Target.Position) - Svc.Targets.Target.HitboxRadius}");
        }
        if(ImGui.CollapsingHeader("CharaSelect"))
        {
            foreach(var x in Utils.GetCharacterNames())
            {
                ImGuiEx.Text($"{x.Name}@{x.World}");
            }
        }
    }
}
