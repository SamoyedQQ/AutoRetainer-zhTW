using AutoRetainerAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;
public sealed unsafe class DebugNeoGCDelivery : DebugSectionBase
{
    public override void Draw()
    {
        if(ImGui.Button("BeginNewPurchase")) GCContinuation.BeginNewPurchase();
        foreach(var x in Utils.SharedGCExchangeListings.Values)
        {
            ImGuiEx.Text($"{x.Data.Name} / {x.ItemID} / {x.Category} / 最低階級 {x.MinPurchaseRank} {x.Rank} / {x.Seals} 軍票 | 可購買：×{new GCExchangeItem(x.ItemID, int.MaxValue).GetAmountThatCanBePurchased()}");
        }
    }
}