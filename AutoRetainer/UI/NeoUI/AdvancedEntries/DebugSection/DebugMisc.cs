using ECommons.Configuration;
using ECommons.Events;
using ECommons.ExcelServices;
using ECommons.Interop;
using ECommons.MathHelpers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ItemLevel = AutoRetainer.Helpers.ItemLevel;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugMisc : DebugSectionBase
{
    public override void Draw()
    {
        if(ImGui.CollapsingHeader("CMenu"))
        {
            if(TryGetAddonMaster<AddonMaster.ContextMenu>(out var m) && m.IsAddonReady)
            {
                foreach(var x in m.Entries)
                {
                    ImGuiEx.Text($"{x.Text}/{x.Enabled}");
                }
            }
        }
        if(ImGui.CollapsingHeader("僱員裝備數值"))
        {
            var im = InventoryManager.Instance();
            var c = im->GetInventoryContainer(InventoryType.RetainerEquippedItems);
            for(var i = 0; i < c->Size; i++)
            {
                var slot = c->GetInventorySlot(i);
                ImGuiEx.Text($"{i}（{slot->GetItemId()}）：{ExcelItemHelper.GetName(slot->GetItemId() % 1000000)}，採集力：{slot->GetStat(BaseParamEnum.Gathering)} [{slot->GetStatCap(BaseParamEnum.Gathering)}]，鑑別力：{slot->GetStat(BaseParamEnum.Perception)} [{slot->GetStatCap(BaseParamEnum.Perception)}]");
            }
        }
        if(ImGui.Button("測試 Haseltweaks"))
        {
            Utils.EnsureEnhancedLoginIsOff();
        }
        if(ImGui.Button("透過外部程序寫入設定"))
        {
            ExternalWriter.PlaceWriteOrder(new(System.IO.Path.Combine(Svc.PluginInterface.ConfigDirectory.FullName, "WriterTest.json"), EzConfig.DefaultSerializationFactory.Serialize(C, true)));
        }
        ImGuiEx.Text($"公會點數：{Utils.FCPoints}");
        if(ImGui.CollapsingHeader("房屋"))
        {
            var h = HousingManager.Instance();
            ImGuiEx.Text($"目前分區 {h->GetCurrentDivision()}");
            ImGuiEx.Text($"目前房屋 ID {h->GetCurrentIndoorHouseId()}");
            ImGuiEx.Text($"目前房區號 {h->GetCurrentPlot()}");
            ImGuiEx.Text($"目前房間 {h->GetCurrentRoom()}");
            ImGuiEx.Text($"目前住宅區 {h->GetCurrentWard()}");
            if(ImGui.Button("模擬登入"))
            {
                ProperOnLogin.FireArtificially();
            }
            if(h->OutdoorTerritory != null)
            {
                for(var i = 0; i < 30; i++)
                {
                    ImGuiEx.Text($"是房屋住戶 {i}：{P.Memory.OutdoorTerritory_IsEstateResident((nint)h->OutdoorTerritory, (byte)i)}");
                }
            }
        }
        if(ImGui.Button("安裝回呼掛鉤")) Callback.InstallHook();
        if(ImGui.Button("停用回呼掛鉤")) Callback.UninstallHook();
        ImGuiEx.TextCopy($"{(nint)(&TargetSystem.Instance()->Target):X16}");
        ImGui.Checkbox($"記錄封包代碼", ref P.LogOpcodes);
        ImGuiEx.Text($"影格計數：{CSFramework.Instance()->FrameCounter}");
        if(ImGui.Button("測試寄放重複道具"))
        {
            if(TryGetAddonByName<AtkUnitBase>("RetainerItemTransferList", out var addon))
            {
                Callback.Fire(addon, true, 0, (uint)29);
            }
        }
        ImGuiEx.Text($"鎖定狀態：{*(byte*)((nint)TargetSystem.Instance() + 309)}");
        if(ImGui.Button("ChillFrames 鎖定"))
        {
            FPSManager.LockChillFrames();
        }
        if(ImGui.Button("解除影格鎖定"))
        {
            FPSManager.UnlockChillFrames();
        }
        ImGui.Separator();
        ImGuiEx.Text($"視窗非作用中：{CSFramework.Instance()->WindowInactive}");
        ImGuiEx.Text($"暫時領取鍵已按下：{IsKeyPressed(C.TempCollectB)}");
        ImGuiEx.Text($"暫時領取鍵狀態位元：{Bitmask.IsBitSet(TerraFX.Interop.Windows.Windows.GetKeyState((int)C.TempCollectB), 15)}");
        ImGuiEx.Text($"不重新指派：{C.DontReassign}，按鍵 {C.TempCollectB}/{(int)C.TempCollectB}");
        foreach(var x in C.OfflineData)
        {
            ImGuiEx.Text($"{x.Name}@{x.World}: {x.Gil + x.RetainerData.Sum(z => z.Gil)}");
        }
        var ocd = Data;
        if(ocd != null)
        {
            ImGuiEx.Text($"等級陣列：");
            ImGuiEx.Text(ocd.ClassJobLevelArray.Print());
        }

        ImGuiEx.Text($"{Utils.TryGetCurrentRetainer(out var n)}/{n}");
        ImGuiEx.Text($"{ItemLevel.Calculate(out var g, out var p)}/{g}/{p}");
        if(ImGui.Button("重新產生匿名種子"))
        {
            C.CensorSeed = Guid.NewGuid().ToString();
        }
        var inv = Utils.GetActiveRetainerInventoryName();
        ImGuiEx.Text($"目前僱員背包：{inv.Name} {inv.EntrustDuplicatesIndex}");
        ImGuiEx.Text($"條件曾啟用={P.ConditionWasEnabled}");
        if(ImGui.CollapsingHeader("工作除錯"))
        {
            ImGuiEx.Text($"忙碌中：{P.TaskManager.IsBusy}，剩餘中止時間 {P.TaskManager.RemainingTimeMS}");
            if(ImGui.Button($"產生亂數 1/500"))
            {
                P.TaskManager.Enqueue(() => { var r = new Random().Next(0, 500); InternalLog.Verbose($"Gen 1/500: {r}"); return r == 0; });
            }
            if(ImGui.Button($"產生亂數 1/5000"))
            {
                P.TaskManager.Enqueue(() => { var r = new Random().Next(0, 5000); InternalLog.Verbose($"Gen 1/5000: {r}"); return r == 0; });
            }
            if(ImGui.Button($"產生亂數 1/100"))
            {
                P.TaskManager.Enqueue(() => { var r = new Random().Next(0, 100); InternalLog.Verbose($"Gen 1/100: {r}"); return r == 0; });
            }
        }
        ImGuiEx.Text($"快速賣出掛鉤狀態：{P.quickSellItems?.openInventoryContextHook?.IsEnabled}");
        ImGuiEx.Text($"快速賣出可用：{QuickSellItems.IsReadyToUse()}");

        foreach(var x in S.VentureStats.CharTotal)
        {
            ImGuiEx.Text($"{x.Key} : {x.Value}");
        }
        foreach(var x in S.VentureStats.RetTotal)
        {
            ImGuiEx.Text($"{x.Key} : {x.Value}");
        }

        ImGui.Separator();
        {
            if(ImGui.Button("觸發") && TryGetAddonByName<AtkUnitBase>("GrandCompanySupplyList", out var addon) && IsAddonReady(addon) && addon->UldManager.NodeList[5]->IsVisible())
            {
                AutoGCHandin.InvokeHandin(addon, 0);
            }
        }

        {
            if(TryGetAddonByName<AtkUnitBase>("GrandCompanySupplyList", out var addon) && IsAddonReady(addon))
            {
                ImGuiEx.Text($"所選篩選有效：{AutoGCHandin.IsSelectedFilterValid(addon)}");
            }
        }

    }
}
