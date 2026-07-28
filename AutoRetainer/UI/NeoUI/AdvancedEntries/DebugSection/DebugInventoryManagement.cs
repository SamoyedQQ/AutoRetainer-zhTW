using AutoRetainer.Internal.InventoryManagement;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;
public unsafe class DebugInventoryManagement : DebugSectionBase
{
    private int slot;
    private InventoryType Type;
    private HashSet<uint> Whitelist = [];

    public override void Draw()
    {
        if(ImGui.CollapsingHeader("背包"))
        {
            foreach(var x in Enum.GetValues<InventoryType>())
            {
                ImGuiEx.TreeNodeCollapsingHeader(x.ToString(), () =>
                {
                    var inv = InventoryManager.Instance()->GetInventoryContainer(x);
                    for(var i = 0; i < inv->Size; i++)
                    {
                        var slot = inv->GetInventorySlot(i);
                        ImGuiEx.Text($"{i}：{ExcelItemHelper.GetName(slot->ItemId)} ×{slot->Quantity} {slot->Flags}");
                    }
                });
            }
        }
        if(ImGui.CollapsingHeader("商店販售測試"))
        {
            ImGuiEx.EnumCombo($"type", ref Type);
            ImGui.InputInt("欄位", ref slot);
            ImGuiEx.Text(ExcelItemHelper.GetName(InventoryManager.Instance()->GetInventoryContainer(Type)->GetInventorySlot(slot)->ItemId));
            if(ImGui.Button("出售"))
            {
                P.Memory.SellItemToShop(Type, slot);
            }
            if(ImGui.Button("若存在則加入佇列"))
            {
                NpcSaleManager.EnqueueIfItemsPresent();
            }
            ImGuiEx.Text($"可用的 NPC：{NpcSaleManager.GetValidNPC()}");
            if(ImGui.Button("與目標互動")) TargetSystem.Instance()->InteractWithObject(Svc.Targets.Target.Struct(), false);
            if(TryGetAddonMaster<AddonMaster.SelectIconString>(out var m))
            {
                foreach(var x in m.Entries)
                {
                    if(ImGui.Selectable(x.Text))
                    {
                        x.Select();
                    }
                }
            }
        }
        if(ImGui.CollapsingHeader("商店出售清單"))
        {
            foreach(var x in Vendors)
            {
                ImGuiEx.Text(Whitelist.Contains(x) ? EColor.GreenBright : null, $"{x}: {Svc.Data.GetExcelSheet<ENpcResident>().GetRowOrDefault(x)?.Plural}");
                if(ImGui.IsItemHovered())
                {
                    if(ImGuiEx.Ctrl)
                    {
                        Whitelist.Add(x);
                    }
                    if(ImGuiEx.Shift) Whitelist.Remove(x);
                }
            }
            if(ImGui.Button("複製")) Copy(Whitelist.Print());
        }
    }

    public IEnumerable<uint> Vendors
    {
        get
        {
            foreach(var x in Svc.Data.GetSubrowExcelSheet<HousingEmploymentNpcList>())
            {
                for(var i = 0; i < x.Count; i++)
                {
                    var ret = x[i];
                    if(ret.RowId != 0) yield return ret.RowId;
                }
            }
        }
    }
}
