using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public sealed unsafe class CleanupCharacterConfiguration : InventoryManagementBase
{
    public override string Name { get; } = "背包整理/角色設定";

    public override int DisplayPriority => -20;

    public override void Draw()
    {
        ImGuiEx.TextWrapped($"在這裡可以把預先設定好的背包整理清單指派給已登記的角色。");
        ImGuiEx.SetNextItemFullWidth();
        ImGuiEx.FilteringInputTextWithHint("##search", "Search...", out var filter);
        if(ImGuiEx.BeginDefaultTable(["~Character", "Plan"]))
        {
            foreach(var characterData in C.OfflineData)
            {
                if(filter != "" && !characterData.NameWithWorld.Contains(filter, StringComparison.OrdinalIgnoreCase)) continue;
                ImGuiEx.PushID(characterData.Identity);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGuiEx.TextV(characterData.NameWithWorldCensored);
                ImGui.TableNextColumn();
                var plan = characterData.InventoryCleanupPlan == Guid.Empty ? null : C.AdditionalIMSettings.FirstOrDefault(p => p.GUID == characterData.InventoryCleanupPlan);
                ImGui.SetNextItemWidth(200f);
                if(ImGui.BeginCombo("##chPlan", plan?.DisplayName ?? "預設方案", ImGuiComboFlags.HeightLarge))
                {
                    if(ImGui.Selectable("預設方案", plan == null)) characterData.InventoryCleanupPlan = Guid.Empty;
                    ImGui.Separator();
                    foreach(var cleanupPlan in C.AdditionalIMSettings)
                    {
                        ImGuiEx.PushID(cleanupPlan.ID);
                        if(ImGui.Selectable($"{cleanupPlan.DisplayName}"))
                        {
                            characterData.InventoryCleanupPlan = cleanupPlan.GUID;
                        }
                        ImGui.PopID();
                    }
                    ImGui.EndCombo();
                }
                ImGuiEx.DragDropRepopulate("CleanupPlan", plan?.GUID ?? Guid.Empty, ref characterData.InventoryCleanupPlan);

                ImGui.PopID();
            }
            ImGui.EndTable();
        }
    }
}