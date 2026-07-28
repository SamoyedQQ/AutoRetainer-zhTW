using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI;
public sealed unsafe class UserInterface : NeoUIEntry
{
    public override string Path => "使用者介面";

    public override NuiBuilder Builder => new NuiBuilder()

        .Section("使用者介面")
        .Checkbox("隱藏僱員名稱", () => ref C.NoNames, "一般介面上的僱員名稱會被遮蔽，但除錯選單與外掛記錄中不會隱藏。啟用此選項時，外掛不同區塊裡的角色與僱員編號不保證一致（例如「僱員」頁的僱員 1，未必是「統計」頁的同一位僱員）。")
        .Checkbox("在僱員介面顯示快捷選單", () => ref C.UIBar)
        .Checkbox("顯示僱員詳細資訊", () => ref C.ShowAdditionalInfo, "在主介面顯示僱員的裝備等級／採集力／鑑別力，以及目前的探險名稱。")
        .Widget("按 ESC 時不要關閉 AutoRetainer 視窗", (x) =>
        {
            if(ImGui.Checkbox(x, ref C.IgnoreEsc)) Utils.ResetEscIgnoreByWindows();
        })
        .Checkbox("狀態列只顯示最重要的圖示", () => ref C.StatusBarMSI)
        .SliderInt(120f, "狀態列圖示大小", () => ref C.StatusBarIconWidth, 32, 128)
        .Checkbox("遊戲啟動時開啟 AutoRetainer 視窗", () => ref C.DisplayOnStart)
        //.Checkbox("Skip item sell/trade confirmation while plugin is active", () => ref C.SkipItemConfirmations)
        .Checkbox("啟用標題畫面按鈕（需重新載入外掛）", () => ref C.UseTitleScreenButton)
        .Checkbox("隱藏角色搜尋", () => ref C.NoCharaSearch)
        .Checkbox("已完成的角色不要閃爍背景", () => ref C.NoGradient)
        .Checkbox("不要警告有第二個遊戲執行個體從同一目錄啟動", () => ref C.No2ndInstanceNotify, "這會讓第二個遊戲執行個體自動略過載入 AutoRetainer；在主執行個體關閉此選項之前，你都沒辦法載入它")

        .Section("「僱員」頁的角色排序")
        .Checkbox("啟用", () => ref C.EnableRetainerSort)
        .TextWrapped("這只是顯示順序，完全不影響角色的處理方式。")
        .Widget(() => UIUtils.DrawSortableEnumList("rorder", C.RetainersVisualOrders))

        .Section("「載具」頁的角色排序")
        .Checkbox("啟用", () => ref C.EnableDeployablesSort)
        .TextWrapped("這只是顯示順序，完全不影響角色的處理方式。")
        .Widget(() => UIUtils.DrawSortableEnumList("dorder", C.DeployablesVisualOrders));



}