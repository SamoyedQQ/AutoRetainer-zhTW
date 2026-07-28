using AutoRetainerAPI;
using AutoRetainerAPI.Configuration;

namespace AutoRetainer.UI.MainWindow.MultiModeTab;
public static unsafe class RetainerConfig
{
    public static void Draw(OfflineRetainerData ret, OfflineCharacterData data, AdditionalRetainerData adata)
    {
        ImGui.CollapsingHeader($"{Censor.Retainer(ret.Name)} - {Censor.Character(data.Name)} 設定  ##conf", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.OpenOnArrow);
        ImGuiEx.Text($"探險後的額外工作：");
        //ImGui.Checkbox($"Entrust Duplicates", ref adata.EntrustDuplicates);
        var selectedPlan = C.EntrustPlans.FirstOrDefault(x => x.Guid == adata.EntrustPlan);
        ImGuiEx.TextV($"寄放道具：");
        if(!C.EnableEntrustManager) ImGuiEx.HelpMarker("已在設定中全域停用", EColor.RedBright, FontAwesomeIcon.ExclamationTriangle.ToIconString());
        ImGui.SameLine();
        ImGui.SetNextItemWidth(150f);
        if(ImGui.BeginCombo($"##select", selectedPlan?.Name ?? "Disabled", ImGuiComboFlags.HeightLarge))
        {
            if(ImGui.Selectable("停用")) adata.EntrustPlan = Guid.Empty;
            for(var i = 0; i < C.EntrustPlans.Count; i++)
            {
                var plan = C.EntrustPlans[i];
                ImGuiEx.PushID(plan.Guid.ToString());
                if(ImGui.Selectable(plan.Name, plan == selectedPlan))
                {
                    adata.EntrustPlan = plan.Guid;
                }
                ImGui.PopID();
            }
            ImGui.EndCombo();
        }
        if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.Copy, "複製寄放方案到……"))
        {
            ImGui.OpenPopup($"CopyEntrustPlanTo");
        }
        if(ImGui.BeginPopup("CopyEntrustPlanTo"))
        {
            if(ImGui.Selectable("套用到此角色的其他所有僱員"))
            {
                var cnt = 0;
                foreach(var x in data.RetainerData)
                {
                    cnt++;
                    Utils.GetAdditionalData(data.CID, x.Name).EntrustPlan = adata.EntrustPlan;
                }
                Notify.Info($"Changed {cnt} retainers");
            }
            if(ImGui.Selectable("套用到此角色中尚無寄放方案的僱員"))
            {
                foreach(var x in data.RetainerData)
                {
                    var cnt = 0;
                    if(!C.EntrustPlans.Any(s => s.Guid == adata.EntrustPlan))
                    {
                        Utils.GetAdditionalData(data.CID, x.Name).EntrustPlan = adata.EntrustPlan;
                        cnt++;
                    }
                    Notify.Info($"Changed {cnt} retainers");
                }
            }
            if(ImGui.Selectable("套用到所有角色的其他所有僱員"))
            {
                var cnt = 0;
                foreach(var offlineData in C.OfflineData)
                {
                    foreach(var x in offlineData.RetainerData)
                    {
                        Utils.GetAdditionalData(offlineData.CID, x.Name).EntrustPlan = adata.EntrustPlan;
                        cnt++;
                    }
                }
                Notify.Info($"Changed {cnt} retainers");
            }
            if(ImGui.Selectable("套用到所有角色中尚無寄放方案的僱員"))
            {
                var cnt = 0;
                foreach(var offlineData in C.OfflineData)
                {
                    foreach(var x in offlineData.RetainerData)
                    {
                        var a = Utils.GetAdditionalData(data.CID, x.Name);
                        if(!C.EntrustPlans.Any(s => s.Guid == a.EntrustPlan))
                        {
                            a.EntrustPlan = adata.EntrustPlan;
                            cnt++;
                        }
                    }
                }
                Notify.Info($"Changed {cnt} retainers");
            }
            ImGui.EndPopup();
        }
        ImGui.Checkbox($"提領／存入金幣", ref adata.WithdrawGil);
        if(adata.WithdrawGil)
        {
            if(ImGui.RadioButton("提領", !adata.Deposit)) adata.Deposit = false;
            if(ImGui.RadioButton("存入", adata.Deposit)) adata.Deposit = true;
            ImGuiEx.SetNextItemWidthScaled(200f);
            ImGui.InputInt($"數量（%）", ref adata.WithdrawGilPercent.ValidateRange(1, 100), 1, 10);
        }
        ImGui.Separator();
        Svc.PluginInterface.GetIpcProvider<ulong, string, object>(ApiConsts.OnRetainerSettingsDraw).SendMessage(data.CID, ret.Name);
        if(C.Verbose)
        {
            if(ImGui.Button("假裝就緒"))
            {
                ret.VentureEndsAt = 1;
            }
            if(ImGui.Button("假裝未就緒"))
            {
                ret.VentureEndsAt = P.Time + 60 * 60;
            }
        }
    }
}
