using ECommons.Automation;
using ECommons.ExcelServices;
using ECommons.Throttlers;
using ECommons.WindowsFormsReflector;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public unsafe class FastAddition : InventoryManagementBase
{
    public override string Name { get; } = "背包整理/快速增刪";

    private FastAddition()
    {
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
        .Section(Name)
        .Widget(() =>
        {
            var selectedSettings = InventoryCleanupCommon.SelectedPlan;
            ImGuiEx.TextWrapped(GradientColor.Get(EColor.RedBright, EColor.YellowBright), $"這段文字顯示期間，把滑鼠移到道具上並按住：");
            ImGuiEx.Text(!ImGui.GetIO().KeyShift ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Shift－加入快速探險出售清單");
            ImGuiEx.Text($"＊已經在「無條件出售清單」或「捨棄清單」裡的道具，不會被加進「快速探險出售清單」");
            ImGuiEx.Text(!ImGui.GetIO().KeyCtrl ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Ctrl－加入無條件出售清單");
            ImGuiEx.Text($"＊已經在其他清單裡的道具，會被移動到「無條件出售清單」");
            ImGuiEx.Text(!IsKeyPressed(Keys.Tab) ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Tab－加入捨棄清單");
            ImGuiEx.Text($"＊已經在其他清單裡的道具，會被移動到「捨棄清單」");
            //ImGuiEx.Text(IsKeyPressed(Keys.Space) ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Space - add to Desynthesis List");
            //ImGuiEx.Text($"* Items that already in other lists WILL BE MOVED to Desynthesis List");
            ImGuiEx.Text(!ImGui.GetIO().KeyAlt ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Alt－從任一清單刪除");
            ImGuiEx.Text("\n受保護的道具不受這些動作影響");
            if(Svc.GameGui.HoveredItem > 0)
            {
                var id = (uint)(Svc.GameGui.HoveredItem % 1000000);
                if(ImGui.GetIO().KeyShift)
                {
                    if(!selectedSettings.IMProtectList.Contains(id) 
                    && !selectedSettings.IMAutoVendorSoft.Contains(id)
                    && !selectedSettings.IMAutoVendorHard.Contains(id)
                    && !selectedSettings.IMDiscardList.Contains(id)
                    && !selectedSettings.IMDesynth.Contains(id)
                    )
                    {
                        if(selectedSettings.AddItemToList(IMListKind.SoftSell, id, out var error))
                        {
                            Notify.Success($"Added {ExcelItemHelper.GetName(id)} to Quick Venture Sell List");
                        }
                        else
                        {
                            if(EzThrottler.Throttle($"Error_{error}", 1000)) Notify.Error(error);
                        }
                    }
                }
                if(ImGui.GetIO().KeyCtrl)
                {
                    if(!selectedSettings.IMProtectList.Contains(id) && !selectedSettings.IMAutoVendorHard.Contains(id) && !selectedSettings.IMAutoVendorSoft.Contains(id))
                    {
                        if(selectedSettings.AddItemToList(IMListKind.HardSell, id, out var error))
                        {
                            Notify.Success($"Added {ExcelItemHelper.GetName(id)} to Unconditional Sell List");
                        }
                        else
                        {
                            if(EzThrottler.Throttle($"Error_{error}", 1000)) Notify.Error(error);
                        }
                    }
                }
                if(!CSFramework.Instance()->WindowInactive && IsKeyPressed(Keys.Tab))
                {
                    if(!selectedSettings.IMProtectList.Contains(id) && !selectedSettings.IMDiscardList.Contains(id))
                    {
                        if(selectedSettings.AddItemToList(IMListKind.Discard, id, out var error))
                        {
                            Notify.Success($"Added {ExcelItemHelper.GetName(id)} to Discard List");
                        }
                        else
                        {
                            if(EzThrottler.Throttle($"Error_{error}", 1000)) Notify.Error(error);
                        }
                    }
                }
                /*if(!CSFramework.Instance()->WindowInactive && IsKeyPressed(Keys.Space))
                {
                    if(!selectedSettings.IMProtectList.Contains(id) && !selectedSettings.IMDesynth.Contains(id))
                    {
                        if(selectedSettings.AddItemToList(IMListKind.Desynth, id, out var error))
                        {
                            Notify.Success($"Added {ExcelItemHelper.GetName(id)} to Desynthesis List");
                        }
                        else
                        {
                            if(EzThrottler.Throttle($"Error_{error}", 1000)) Notify.Error(error);
                        }
                    }
                }*/
                if(ImGui.GetIO().KeyAlt)
                {
                    if(selectedSettings.IMAutoVendorSoft.Remove(id)) Notify.Info($"Removed {ExcelItemHelper.GetName(id)} from Quick Venture Sell List");
                    if(selectedSettings.IMAutoVendorHard.Remove(id)) Notify.Info($"Removed {ExcelItemHelper.GetName(id)} from Unconditional Sell List");
                    if(selectedSettings.IMDiscardList.Remove(id)) Notify.Info($"Removed {ExcelItemHelper.GetName(id)} from Discard List");
                    if(selectedSettings.IMDesynth.Remove(id)) Notify.Info($"Removed {ExcelItemHelper.GetName(id)} from Desynthesis List");
                }
            }
        });
        DisplayPriority = -10;
    }
}
