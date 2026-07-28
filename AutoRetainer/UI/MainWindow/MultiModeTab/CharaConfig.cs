using AutoRetainerAPI.Configuration;
using Dalamud.Interface.Components;
using PunishLib.ImGuiMethods;

namespace AutoRetainer.UI.MainWindow.MultiModeTab;
public class CharaConfig
{
    public static void Draw(OfflineCharacterData data, bool isRetainer)
    {
        ImGuiEx.PushID(data.CID.ToString());
        SharedUI.DrawMultiModeHeader(data);
        var b = new NuiBuilder()

        .Section("角色專屬的一般設定")
        .Widget(() =>
        {
            SharedUI.DrawServiceAccSelector(data);
            SharedUI.DrawPreferredCharacterUI(data);
        });
        if(isRetainer)
        {
            b = b.Section("僱員").Widget(() =>
            {
                ImGuiEx.Text($"自動大國防聯軍精選交易：");
                if(!AutoGCHandin.Operation)
                {
                    ImGuiEx.SetNextItemWidthScaled(200f);
                    ImGuiEx.EnumCombo("##gcHandin", ref data.GCDeliveryType);
                }
                else
                {
                    ImGuiEx.Text($"現在無法變更");
                }
            });
        }
        else
        {
            b = b.Section("載具").Widget(() =>
            {
                ImGui.Checkbox($"等待航行完成", ref data.MultiWaitForAllDeployables);
                ImGuiComponents.HelpMarker("此設定與全域選項作用相同，但只套用於個別角色。啟用後，AutoRetainer 會等所有載具返航才登入該角色。若你因其他原因已經登入，它仍會重新派出已完成的潛水艇——除非全域設定「已登入時也要等待」也一併開啟。");
            });
        }
        b = b.Section("傳送覆寫設定", data.GetAreTeleportSettingsOverriden() ? ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg] with { X = 1f } : null, true)
        .Widget(() =>
        {
            ImGuiEx.Text($"你可以為每個角色覆寫傳送設定。");
            bool? demo = null;
            ImGuiEx.Checkbox("有此標記的選項會採用全域設定的數值", ref demo);
            ImGuiEx.Checkbox("已啟用", ref data.TeleportOptionsOverride.Enabled);
            ImGui.Indent();
            ImGuiEx.Checkbox("為了僱員傳送……", ref data.TeleportOptionsOverride.Retainers);
            ImGui.Indent();
            ImGuiEx.Checkbox("……到個人房屋", ref data.TeleportOptionsOverride.RetainersPrivate);
            ImGuiEx.Checkbox("……到公會房屋", ref data.TeleportOptionsOverride.RetainersFC);
            ImGuiEx.Checkbox("……到公寓", ref data.TeleportOptionsOverride.RetainersApartment);
            ImGui.Text("若以上全部停用或失敗，會改為傳送到旅館。");
            ImGui.Unindent();
            ImGuiEx.Checkbox("為了載具傳送到公會房屋", ref data.TeleportOptionsOverride.Deployables);
            ImGui.Unindent(); 
        }).Draw();
        SharedUI.DrawExcludeReset(data);
        ImGui.PopID();
    }
}
