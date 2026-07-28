namespace AutoRetainer.UI.NeoUI;
public class MainSettings : NeoUIEntry
{
    public override string Path => "一般";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("延遲")
        .Widget(100f, "時間不同步補償", (x) => ImGuiEx.SliderInt(x, ref C.UnsyncCompensation.ValidateRange(-60, 0), -10, 0), "會從探險結束時間額外扣掉的秒數，用來緩解遊戲與電腦時間不同步造成的問題。")
        .Widget(100f, "額外互動延遲（影格）", (x) => ImGuiEx.SliderInt(x, ref C.ExtraFrameDelay.ValidateRange(-10, 100), 0, 50), "數值越低，外掛執行動作越快。FPS 偏低或延遲較高時可以調高；想讓外掛跑快一點則可以調低。")
        .Widget("額外記錄", (x) => ImGui.Checkbox(x, ref C.ExtraDebug), "此選項會為了除錯而記錄大量訊息。啟用期間記錄會被灌爆，也會影響效能。重新載入外掛或重啟遊戲後會自動關閉。")

            .Section("運作")
        .Widget("指派＋重新指派", (x) =>
        {
            if(ImGui.RadioButton(x, C.EnableAssigningQuickExploration && !C._dontReassign))
            {
                C.EnableAssigningQuickExploration = true;
                C.DontReassign = false;
            }
        }, "若啟用的僱員目前沒有進行中的探險，會自動指派快速探險，並重新指派目前的探險。")
        .Widget("領取", (x) =>
        {
            if(ImGui.RadioButton(x, !C.EnableAssigningQuickExploration && C._dontReassign))
            {
                C.EnableAssigningQuickExploration = false;
                C.DontReassign = true;
            }
        }, "只領取僱員的探險報酬，不重新指派探險。\n與傳喚鈴互動時按住 CTRL 可暫時套用此模式。")
        .Widget("重新指派", (x) =>
        {
            if(ImGui.RadioButton("重新指派", !C.EnableAssigningQuickExploration && !C._dontReassign))
            {
                C.EnableAssigningQuickExploration = false;
                C.DontReassign = false;
            }
        }, "只重新指派僱員正在執行的探險。")
        .Widget("傳喚鈴感應", (x) => ImGui.Checkbox(x, ref C.RetainerSense), "當玩家進入傳喚鈴的互動範圍時，AutoRetainer 會自動啟用。你必須保持靜止不動，否則啟用會被取消。")
        .Widget(200f, "啟用時間", (x) => ImGuiEx.SliderIntAsFloat(x, ref C.RetainerSenseThreshold, 1000, 100000));


}
