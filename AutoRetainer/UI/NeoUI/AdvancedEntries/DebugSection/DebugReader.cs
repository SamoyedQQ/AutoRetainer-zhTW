using AutoRetainer.Internal;
using AutoRetainer.Modules.Voyage.Readers;
using AutoRetainer.Scheduler.Tasks;
using AutoRetainer.UiHelpers;
using ECommons.UIHelpers.AtkReaderImplementations;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugReader : DebugSectionBase
{
    public override void Draw()
    {
        {
            if(TryGetAddonByName<AtkUnitBase>("FreeCompanyCreditShop", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderFreeCompanyCreditShop(a);
                ImGuiEx.Text($"""
                    Rank: {reader.FCRank}
                    Credits: {reader.Credits}
                    Count: {reader.Count}
                    """);
                for(var i = 0; i < reader.Count; i++)
                {
                    var x = reader.Listings[i];
                    ImGuiEx.Text($"{x}");
                    if(ImGuiEx.HoveredAndClicked()) new FreeCompanyCreditShop(a).Buy(0);
                    var amount = Math.Floor((float)reader.Credits / (float)(x.Price));
                }

                if(ImGui.Button("執行工作")) TaskRecursivelyBuyFuel.Enqueue();
            }
        }

        {
            if(TryGetAddonByName<AtkUnitBase>("RetainerList", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderRetainerList(a);
                foreach(var x in reader.Retainers)
                {
                    ImGuiEx.Text($"{x.Name}／啟用 {x.IsActive}／金幣 {x.Gil}／等級 {x.Level}／背包 {x.Inventory}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("RetainerItemTransferList", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderRetainerItemTransferList(a);
                foreach(var r in reader.Items)
                {
                    ImGuiEx.Text($"道具 {r.ItemID}，優質 = {r.IsHQ}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("AirShipExploration", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderAirShipExploration(a);
                ImGuiEx.Text($"距離：{reader.Distance}");
                ImGuiEx.Text($"燃料：{reader.Fuel}");
                foreach(var r in reader.Destinations)
                {
                    ImGuiEx.Text($"目的地 {r.NameFull}，需求階級={r.RequiredRank}，狀態={r.StatusFlag}，可選取={r.CanBeSelected}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("SubmarineExplorationMapSelect", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderSubmarineExplorationMapSelect(a);
                ImGuiEx.Text($"目前階級：{reader.SubmarineRank}");
                foreach(var r in reader.Maps)
                {
                    ImGuiEx.Text($"地圖 {r.Name}，需求階級={r.RequiredRank}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("SelectString", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderSelectString(a);
                foreach(var r in reader.Entries)
                {
                    ImGuiEx.Text($"{r.Text}");
                }
            }
        }
    }
}
