using ECommons.Configuration;
using ECommons.ExcelServices;
using ECommons.Reflection;
using ECommons.Throttlers;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries;
public class EntrustManager : InventoryManagementBase
{
    public override string Name { get; } = "寄放管理員";
    private Guid SelectedGuid = Guid.Empty;
    private string Filter = "";
    private InventoryManagementCommon InventoryManagementCommon = new();

    public override void Draw()
    {
        ImGuiEx.TextWrapped("用進階寄放管理員把特定道具寄放給特定僱員。在這個視窗設定各個方案，之後就可以在僱員設定視窗把寄放方案指派給僱員。");
        ImGui.Checkbox("啟用", ref C.EnableEntrustManager);
        ImGui.Checkbox("在聊天視窗輸出已寄放的道具", ref C.EnableEntrustChat);
        var selectedPlan = C.EntrustPlans.FirstOrDefault(x => x.Guid == SelectedGuid);

        ImGuiEx.InputWithRightButtonsArea(() =>
        {
            if(ImGui.BeginCombo($"##select", selectedPlan?.Name ?? "選擇方案……", ImGuiComboFlags.HeightLarge))
            {
                for(var i = 0; i < C.EntrustPlans.Count; i++)
                {
                    var plan = C.EntrustPlans[i];
                    ImGuiEx.PushID(plan.Guid.ToString());
                    if(ImGui.Selectable(plan.Name, plan == selectedPlan))
                    {
                        SelectedGuid = plan.Guid;
                    }
                    ImGui.PopID();
                }
                ImGui.EndCombo();
            }
        }, () =>
        {
            if(ImGuiEx.IconButton(FontAwesomeIcon.Plus))
            {
                var plan = new EntrustPlan();
                C.EntrustPlans.Add(plan);
                SelectedGuid = plan.Guid;
                plan.Name = $"寄放方案 {C.EntrustPlans.Count}";
            }
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.Trash, enabled: selectedPlan != null && ImGuiEx.Ctrl))
            {
                C.EntrustPlans.Remove(selectedPlan);
            }
            ImGuiEx.Tooltip("按住 CTRL 再點擊");
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.Copy, enabled: selectedPlan != null))
            {
                Copy(EzConfig.DefaultSerializationFactory.Serialize(selectedPlan, false));
            }
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.Paste, enabled: EzThrottler.Check("ImportPlan")))
            {
                try
                {
                    var plan = EzConfig.DefaultSerializationFactory.Deserialize<EntrustPlan>(Paste()) ?? throw new NullReferenceException();
                    plan.Guid = Guid.NewGuid();
                    if(plan.GetType().GetFieldPropertyUnions(ReflectionHelper.AllFlags).Any(x => x.GetValue(plan) == null)) throw new NullReferenceException();
                    C.EntrustPlans.Add(plan);
                    SelectedGuid = plan.Guid;
                    Notify.Success("Imported plan from clipboard");
                    EzThrottler.Throttle("ImportPlan", 2000, true);
                }
                catch(Exception e)
                {
                    DuoLog.Error(e.Message);
                }
            }
        });
        if(selectedPlan != null)
        {
            ImGuiEx.SetNextItemFullWidth();
            ImGui.InputTextWithHint($"##name", "方案名稱", ref selectedPlan.Name, 100);
            ImGui.Checkbox("寄放重複道具", ref selectedPlan.Duplicates);
            ImGuiEx.HelpMarker("模擬遊戲原本的「寄放重複道具」功能：把僱員背包裡已有的道具繼續寄放，直到該道具堆疊滿為止。不影響水晶。下方清單中明確加入的道具與分類會被排除，不受此選項處理。");
            ImGui.Indent();
            ImGui.Checkbox("允許超過堆疊上限", ref selectedPlan.DuplicatesMultiStack);
            ImGuiEx.HelpMarker("允許「寄放重複道具」為選定僱員已持有的道具建立新的堆疊。");
            ImGui.Unindent();
            ImGui.Checkbox("允許從兵裝庫寄放道具", ref selectedPlan.AllowEntrustFromArmory);
            ImGui.Checkbox("僅限手動執行", ref selectedPlan.ManualPlan);
            ImGuiEx.HelpMarker("把此方案標記為僅限手動執行。只有在手動點擊「寄放道具」時才會處理，永遠不會自動執行。");
            ImGui.Checkbox("排除保護清單中的道具", ref selectedPlan.ExcludeProtected);
            ImGui.Separator();
            ImGuiEx.TreeNodeCollapsingHeader($"Entrust categories ({selectedPlan.EntrustCategories.Count} selected)###ecats", () =>
            {
                ImGuiEx.TextWrapped($"在這裡可以選擇要整類寄放的道具分類。下方個別勾選的道具會被排除在這些規則之外。");
                if(ImGui.BeginTable("EntrustTable", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.BordersInner))
                {
                    ImGui.TableSetupColumn("##1");
                    ImGui.TableSetupColumn("道具名稱", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("保留數量");
                    ImGui.TableHeadersRow();
                    foreach(var x in Svc.Data.GetExcelSheet<ItemUICategory>())
                    {
                        if(x.Name == "" || x.RowId == 39) continue;
                        var contains = selectedPlan.EntrustCategories.Any(s => s.ID == x.RowId);
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        if(ThreadLoadImageHandler.TryGetIconTextureWrap(x.Icon, true, out var icon))
                        {
                            ImGui.Image(icon.Handle, new(ImGui.GetFrameHeight()));
                        }
                        ImGui.TableNextColumn();
                        if(ImGui.Checkbox(x.Name.ToString(), ref contains))
                        {
                            if(contains)
                            {
                                selectedPlan.EntrustCategories.Add(new() { ID = x.RowId });
                            }
                            else
                            {
                                selectedPlan.EntrustCategories.RemoveAll(s => s.ID == x.RowId);
                            }
                        }
                        ImGui.TableNextColumn();
                        if(selectedPlan.EntrustCategories.TryGetFirst(s => s.ID == x.RowId, out var result))
                        {
                            ImGui.SetNextItemWidth(130f);
                            ImGui.InputInt($"##amtkeep{result.ID}", ref result.AmountToKeep);
                        }
                    }
                    ImGui.EndTable();
                }
            });
            ImGuiEx.TreeNodeCollapsingHeader($"Entrust individual items ({selectedPlan.EntrustItems.Count} selected)###eitems", () =>
            {
                InventoryManagementCommon.DrawListNew(
                    itemId => selectedPlan.EntrustItems.Add(itemId), 
                    itemId => selectedPlan.EntrustItems.Remove(itemId), 
                    selectedPlan.EntrustItems, (x) =>
                {
                    var amount = selectedPlan.EntrustItemsAmountToKeep.SafeSelect(x);
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(130f);
                    if(ImGui.InputInt($"##amtkeepitem{x}", ref amount))
                    {
                        selectedPlan.EntrustItemsAmountToKeep[x] = amount;
                    }
                    ImGuiEx.Tooltip("背包中要保留的數量");
                });
            });
            ImGuiEx.TreeNodeCollapsingHeader($"Fast addition/removal", () =>
            {
                ImGuiEx.TextWrapped(GradientColor.Get(EColor.RedBright, EColor.YellowBright), $"這段文字顯示期間，把滑鼠移到道具上並按住：");
                ImGuiEx.Text(!ImGui.GetIO().KeyShift ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Shift－加入寄放方案");
                ImGuiEx.Text(!ImGui.GetIO().KeyAlt ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Alt－從寄放方案刪除");
                if(Svc.GameGui.HoveredItem > 0)
                {
                    var id = (uint)(Svc.GameGui.HoveredItem % 1000000);
                    if(ImGui.GetIO().KeyShift)
                    {
                        if(!selectedPlan.EntrustItems.Contains(id))
                        {
                            selectedPlan.EntrustItems.Add(id);
                            Notify.Success($"Added {ExcelItemHelper.GetName(id)} to entrust plan {selectedPlan.Name}");
                        }
                    }
                    if(ImGui.GetIO().KeyAlt)
                    {
                        if(selectedPlan.EntrustItems.Contains(id))
                        {
                            selectedPlan.EntrustItems.Remove(id);
                            Notify.Success($"Removed {ExcelItemHelper.GetName(id)} from entrust plan {selectedPlan.Name}");
                        }
                    }
                }
            });
        }
    }
}
