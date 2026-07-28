using AutoRetainer.Scheduler.Handlers;
using AutoRetainer.Scheduler.Tasks;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugScheduler : DebugSectionBase
{
    private string dbgRetName = string.Empty;
    public override void Draw()
    {
        ImGuiEx.Text($"金幣：{TaskDepositGil.Gil}");
        ImGui.Checkbox($"TaskWithdrawGil.forceCheck", ref TaskWithdrawGil.forceCheck);
        ImGuiEx.Text($"{Svc.Data.GetExcelSheet<LogMessage>().GetRow(4578).Text.ToDalamudString().GetText(true)}");
        if(ImGui.Button("關閉僱員視窗"))
        {
            DuoLog.Information($"{RetainerHandlers.CloseAgentRetainer()}");
        }
        ImGuiEx.Text($"目前角色有僱員可用：{Utils.AnyRetainersAvailableCurrentChara()}");
        if(ImGui.Button($"SelectAssignVenture"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectAssignVenture()}");
        }
        if(ImGui.Button($"SelectQuit"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectQuit()}");
        }
        if(ImGui.Button($"SelectViewVentureReport"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectViewVentureReport()}");
        }
        if(ImGui.Button($"ClickResultReassign"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickResultReassign()}");
        }
        if(ImGui.Button($"ClickResultConfirm"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickResultConfirm()}");
        }
        if(ImGui.Button($"ClickAskAssign"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickAskAssign()}");
        }
        if(ImGui.Button($"SelectQuickExploration"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectQuickExploration()}");
        }
        if(ImGui.Button($"SelectEntrustItems"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectEntrustItems()}");
        }
        if(ImGui.Button($"SelectEntrustGil"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectEntrustGil()}");
        }
        if(ImGui.Button($"ClickEntrustDuplicates"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickEntrustDuplicates()}");
        }
        if(ImGui.Button($"ClickEntrustDuplicatesConfirm"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickEntrustDuplicatesConfirm()}");
        }
        if(ImGui.Button($"ClickCloseEntrustWindow"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickCloseEntrustWindow()}");
        }
        if(ImGui.Button($"CloseRetainerInventory"))
        {
            DuoLog.Information($"{RetainerHandlers.CloseAgentRetainer()}");
        }
        if(ImGui.Button($"CloseRetainerInventory"))
        {
            DuoLog.Information($"{RetainerHandlers.CloseAgentRetainer()}");
        }
        if(ImGui.Button($"設定提領金幣量（1%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(1)}");
        }
        if(ImGui.Button($"設定提領金幣量（50%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(50)}");
        }
        if(ImGui.Button($"設定提領金幣量（99%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(99)}");
        }
        if(ImGui.Button($"設定提領金幣量（100%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(100)}");
        }
        if(ImGui.Button($"提領金幣或取消"))
        {
            DuoLog.Information($"{RetainerHandlers.ProcessBankOrCancel()}");
        }
        if(ImGui.Button($"提領金幣或取消（強制取消）"))
        {
            DuoLog.Information($"{RetainerHandlers.ProcessBankOrCancel(true)}");
        }
        if(ImGui.Button($"SwapBankMode"))
        {
            DuoLog.Information($"{RetainerHandlers.SwapBankMode()}");
        }
        if(ImGui.Button($"設定存入金幣量（1%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(1)}");
        }
        if(ImGui.Button($"設定存入金幣量（50%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(50)}");
        }
        if(ImGui.Button($"設定存入金幣量（99%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(99)}");
        }
        if(ImGui.Button($"設定存入金幣量（100%）"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(100)}");
        }

        ImGui.Separator();

        if(ImGui.Button($"TaskAssignQuickVenture"))
        {
            TaskAssignQuickVenture.Enqueue();
        }
        if(ImGui.Button($"TaskReassignVenture"))
        {
            TaskReassignVenture.Enqueue();
        }
        if(ImGui.Button($"TaskWithdrawGil (50%)"))
        {
            TaskWithdrawGil.Enqueue(50);
        }

        ImGuiEx.Text($"背包空位：{Utils.GetInventoryFreeSlotCount()}");
        ImGui.InputText("僱員名稱", ref dbgRetName, 50);
        if(ImGui.Button("依名稱選擇僱員"))
        {
            DuoLog.Information($"{RetainerListHandlers.SelectRetainerByName(dbgRetName)}");
        }

        if(ImGui.Button("AtkStage 取得焦點"))
        {
            var ptr = (nint)AtkStage.Instance()->GetFocus();
            Svc.Chat.Print($"Stage focus: {ptr}");
        }
        if(ImGui.Button("AtkStage 清除焦點"))
        {
            AtkStage.Instance()->ClearFocus();
        }
        if(ImGui.Button("嘗試取得目前僱員名稱"))
        {
            if(TryGetAddonByName<AddonSelectString>("SelectString", out var select) && IsAddonReady(&select->AtkUnitBase))
            {
                var textNode = (AtkTextNode*)select->AtkUnitBase.UldManager.NodeList[3];
                var text = GenericHelpers.ReadSeString(&textNode->NodeText);
                foreach(var x in text.Payloads)
                {
                    PluginLog.Information($"{x.Type}: {x.ToString()}");
                }
            }
        }
        {
            if(ImGui.Button("嘗試關閉") && TryGetAddonByName<AtkUnitBase>("RetainerList", out var addon))
            {
                var v = stackalloc AtkValue[1]
                {
                                        new()
                                        {
                                                Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                                                Int = -1
                                        }
                                };
                addon->FireCallback(1, v);
                Notify.Info("Done");
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("Bank", out var addon) && IsAddonReady(addon))
            {
                if(ImGui.Button("測試寶庫"))
                {
                    var values = stackalloc AtkValue[2]
                    {
                                                new() { Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int, Int = 3 },
                                                new() { Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt, Int = 50 },
                                        };
                    addon->FireCallback(2, values);
                }
            }
        }

        ImGui.Separator();

        if(ImGui.Button("TaskDesynthItems"))
            TaskDesynthItems.Enqueue();
    }
}
