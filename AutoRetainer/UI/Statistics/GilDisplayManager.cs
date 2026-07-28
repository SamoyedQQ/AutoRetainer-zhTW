using AutoRetainerAPI.Configuration;
using Dalamud.Interface.Components;
using ECommons.ExcelServices;

namespace AutoRetainer.UI.Statistics;

public sealed class GilDisplayManager
{
    private GilDisplayManager() { }

    public void Draw()
    {
        ImGuiEx.SetNextItemWidthScaled(200f);
        ImGui.InputInt("忽略金幣少於此數量的角色／僱員", ref C.MinGilDisplay.ValidateRange(0, int.MaxValue));
        ImGuiComponents.HelpMarker($"被忽略的僱員金幣仍會計入角色／大區總額。只有當角色本身與其所有僱員的金幣都低於此數值時，該角色才會被忽略。被忽略的角色不計入大區總額。");
        ref var filter = ref Ref<string>.Get();
        ImGui.Checkbox("只顯示角色總額", ref C.GilOnlyChars);
        ImGui.SameLine();
        ImGuiEx.SetNextItemFullWidth();
        ImGui.InputTextWithHint("##fltr", "篩選……", ref filter, 50);
        Dictionary<ExcelWorldHelper.Region, List<OfflineCharacterData>> data = [];
        foreach(var x in C.OfflineData)
        {
            if(ExcelWorldHelper.TryGet(x.World, out var world))
            {
                if(!data.ContainsKey((ExcelWorldHelper.Region)world.DataCenter.Value.Region))
                {
                    data[(ExcelWorldHelper.Region)world.DataCenter.Value.Region] = [];
                }
                data[(ExcelWorldHelper.Region)world.DataCenter.Value.Region].Add(x);
            }
        }
        var globalTotal = 0L;
        foreach(var x in data)
        {
            ImGuiEx.Text($"{x.Key}:");
            var dcTotal = 0L;
            foreach(var c in x.Value)
            {
                if(c.NoGilTrack) continue;
                if(filter != "" && !$"{c.Name}@{c.World}".Contains(filter, StringComparison.OrdinalIgnoreCase)) continue;
                FCData fcdata = null;
                var charTotal = c.Gil + c.RetainerData.Sum(s => s.Gil);
                foreach(var fc in C.FCData)
                {
                    if(S.FCData.GetHolderChara(fc.Key, fc.Value) == c && fc.Value.GilCountsTowardsChara)
                    {
                        fcdata = fc.Value;
                        charTotal += fcdata.Gil;
                        break;
                    }
                }
                if(charTotal > C.MinGilDisplay)
                {
                    if(!C.GilOnlyChars)
                    {
                        ImGuiEx.Text($"    {Censor.Character(c.Name, c.World)}: {c.Gil:N0}");
                        foreach(var r in c.RetainerData)
                        {
                            if(r.Gil > C.MinGilDisplay)
                            {
                                ImGuiEx.Text($"        {Censor.Retainer(r.Name)}: {r.Gil:N0}");
                            }
                        }
                        if(fcdata != null && fcdata.Gil > 0)
                        {
                            ImGuiEx.Text(ImGuiColors.DalamudYellow, $"        公會 {fcdata.Name}：{fcdata.Gil:N0}");
                        }
                    }
                    ImGuiEx.Text(ImGuiColors.DalamudViolet, $"    {Censor.Character(c.Name, c.World)}{(fcdata != null && fcdata.Gil > 0 ? "+FC" : "")} 合計：{charTotal:N0}");
                    if(ImGuiEx.HoveredAndClicked("點擊可切換登入"))
                    {
                        if(!MultiMode.Relog(c, out var error, Internal.RelogReason.Command))
                        {
                            Notify.Error(error);
                        }
                    }
                    dcTotal += charTotal;
                    ImGui.Separator();
                }
            }
            ImGuiEx.Text(ImGuiColors.DalamudOrange, $"大區總額（{x.Key}）：{dcTotal:N0}");
            globalTotal += dcTotal;
            ImGui.Separator();
            ImGui.Separator();
        }
        ImGuiEx.Text(ImGuiColors.DalamudOrange, $"總計：{globalTotal:N0}");
    }
}
