using AutoRetainer.Modules.Voyage;
using AutoRetainer.Modules.Voyage.VoyageCalculator;
using AutoRetainerAPI.Configuration;
using ECommons.GameHelpers;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;

namespace AutoRetainer.UI.Windows;

internal unsafe class SubmarinePointPlanUI : Window
{
    internal string SelectedPlanGuid = Guid.Empty.ToString();
    internal string SelectedPlanName => VoyageUtils.GetSubmarinePointPlanByGuid(SelectedPlanGuid).GetPointPlanName();
    internal SubmarinePointPlan SelectedPlan => VoyageUtils.GetSubmarinePointPlanByGuid(SelectedPlanGuid);

    public SubmarinePointPlanUI() : base("Submersible Voyage Route Planner")
    {
        P.WindowSystem.AddWindow(this);
    }

    internal int GetAmountOfOtherPlanUsers(string guid)
    {
        var i = 0;
        C.OfflineData.Where(x => x.CID != Player.CID).Each(x => i += x.AdditionalSubmarineData.Count(a => a.Value.SelectedPointPlan == guid));
        return i;
    }

    public override void Draw()
    {
        C.SubmarinePointPlans.RemoveAll(x => x.Delete);
        ImGuiEx.InputWithRightButtonsArea("SUPSelector", () =>
        {
            if(ImGui.BeginCombo("##supsel", SelectedPlanName, ImGuiComboFlags.HeightLarge))
            {
                foreach(var x in C.SubmarinePointPlans)
                {
                    if(ImGui.Selectable(x.GetPointPlanName() + $"##{x.GUID}"))
                    {
                        SelectedPlanGuid = x.GUID;
                    }
                }
                ImGui.EndCombo();
            }
        }, () =>
        {
            if(ImGui.Button("新方案"))
            {
                var x = new SubmarinePointPlan
                {
                    Name = $""
                };
                C.SubmarinePointPlans.Add(x);
                SelectedPlanGuid = x.GUID;
            }
        });
        ImGui.Separator();
        if(SelectedPlan == null)
        {
            ImGuiEx.Text($"未選擇方案或方案未知");
        }
        else
        {
            if(Data != null)
            {
                var users = GetAmountOfOtherPlanUsers(SelectedPlanGuid);
                var my = Data.AdditionalSubmarineData.Where(x => x.Value.SelectedPointPlan == SelectedPlanGuid);
                if(users == 0)
                {
                    if(!my.Any())
                    {
                        ImGuiEx.TextWrapped($"沒有任何潛水艇使用此方案。");
                    }
                    else
                    {
                        ImGuiEx.TextWrapped($"此方案由 {my.Select(X => X.Key).Print()} 使用。");
                    }
                }
                else
                {
                    if(!my.Any())
                    {
                        ImGuiEx.TextWrapped($"此方案由你其他角色的 {users} 艘潛水艇使用。");
                    }
                    else
                    {
                        ImGuiEx.TextWrapped($"此方案由 {my.Select(X => X.Key).Print()} 使用，另有其他角色的 {users} 艘潛水艇也在使用。");
                    }
                }
            }
            ImGuiEx.TextV("名稱： ");
            ImGui.SameLine();
            ImGuiEx.SetNextItemFullWidth();
            ImGui.InputText($"##planname", ref SelectedPlan.Name, 100);
            ImGuiEx.LineCentered($"planbuttons", () =>
            {
                ImGuiEx.TextV($"將此方案套用至：");
                ImGui.SameLine();
                if(ImGui.Button("所有潛水艇"))
                {
                    C.OfflineData.Each(x => x.AdditionalSubmarineData.Each(s => s.Value.SelectedPointPlan = SelectedPlanGuid));
                }
                ImGui.SameLine();
                if(ImGui.Button("目前角色的潛水艇"))
                {
                    Data.AdditionalSubmarineData.Each(s => s.Value.SelectedPointPlan = SelectedPlanGuid);
                }
                ImGui.SameLine();
                if(ImGui.Button("沒有潛水艇"))
                {
                    C.OfflineData.Each(x => x.AdditionalSubmarineData.Where(s => s.Value.SelectedPointPlan == SelectedPlanGuid).Each(s => s.Value.SelectedPointPlan = Guid.Empty.ToString()));
                }
            });
            ImGuiEx.LineCentered($"planbuttons2", () =>
            {
                if(ImGui.Button($"複製方案設定"))
                {
                    Copy(JsonConvert.SerializeObject(SelectedPlan));
                }
                ImGui.SameLine();
                if(ImGui.Button($"貼上方案設定"))
                {
                    try
                    {
                        SelectedPlan.CopyFrom(JsonConvert.DeserializeObject<SubmarinePointPlan>(Paste()));
                    }
                    catch(Exception ex)
                    {
                        DuoLog.Error($"無法匯入方案：{ex.Message}");
                        ex.Log();
                    }
                }
                ImGui.SameLine();
                if(ImGuiEx.ButtonCtrl("刪除此方案"))
                {
                    SelectedPlan.Delete = true;
                }
            });

            ImGuiEx.EzTableColumns("SubPlan",
            [
                delegate
                {
                    if(ImGui.BeginChild("col1"))
                    {
                        foreach(var x in Svc.Data.GetExcelSheet<SubmarineExploration>())
                        {
                            if(x.Destination.GetText() == "")
                            {
                                if(x.Map.Value.Name.GetText() != "")
                                {
                                    ImGui.Separator();
                                    ImGuiEx.Text($"{x.Map.Value.Name}:");
                                }
                                continue;
                            }
                            var disabled = !SelectedPlan.GetMapId().EqualsAny(0u, x.Map.RowId) || SelectedPlan.Points.Count >= 5 && !SelectedPlan.Points.Contains(x.RowId);
                            if (disabled) ImGui.BeginDisabled();
                            var cont = SelectedPlan.Points.Contains(x.RowId);
                            if (ImGui.Selectable(x.FancyDestination(), cont))
                            {
                                SelectedPlan.Points.Toggle(x.RowId);
                            }
                            if (disabled) ImGui.EndDisabled();
                        }
                    }
                    ImGui.EndChild();
                }, delegate
                {
                    if(ImGui.BeginChild("Col2"))
                    {
                        var map = SelectedPlan.GetMap();
                        if(map != null)
                        {
                            ImGuiEx.Text($"{map.Value.Name}:");
                        }
                        var toRem = -1;
                        for (var i = 0; i < SelectedPlan.Points.Count; i++)
                        {
                            ImGui.PushID(i);
                            if(ImGuiEx.IconButton(FontAwesomeIcon.ArrowUp) && i > 0)
                            {
                                (SelectedPlan.Points[i-1], SelectedPlan.Points[i]) = (SelectedPlan.Points[i], SelectedPlan.Points[i-1]);
                            }
                            ImGui.SameLine();
                            if(ImGuiEx.IconButton(FontAwesomeIcon.ArrowDown) && i < SelectedPlan.Points.Count - 1)
                            {
                                (SelectedPlan.Points[i+1], SelectedPlan.Points[i]) = (SelectedPlan.Points[i], SelectedPlan.Points[i+1]);
                            }
                            ImGui.SameLine();
                            if (ImGuiEx.IconButton(FontAwesomeIcon.Trash))
                            {
                                toRem = i;
                            }
                            ImGui.SameLine();
                            ImGuiEx.Text($"{VoyageUtils.GetSubmarineExploration(SelectedPlan.Points[i])?.FancyDestination()}");
                            ImGui.PopID();
                        }
                        if(toRem > -1)
                        {
                            SelectedPlan.Points.RemoveAt(toRem);
                        }
                    }
                    ImGui.EndChild();
                }
            ]);
        }
    }
}
