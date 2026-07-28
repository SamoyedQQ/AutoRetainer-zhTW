namespace AutoRetainer.UI.NeoUI;
public class Keybinds : NeoUIEntry
{
    public override string Path => "快捷鍵";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("使用傳喚鈴／工坊面板的快捷鍵")
        .Widget("暫時避免在使用傳喚鈴／工坊面板時自動啟用 AutoRetainer", (x) =>
        {
            UIUtils.DrawKeybind(x, ref C.Suppress);
        })
        .Widget("暫時切換為「只領取」模式，本輪不指派探險／暫時把載具模式設為「只收尾」", (x) =>
        {
            UIUtils.DrawKeybind(x, ref C.TempCollectB);
        })

        .Section("僱員快捷動作")
        .Widget("出售道具", (x) => UIUtils.QRA(x, ref C.SellKey))
        .Widget("寄放道具", (x) => UIUtils.QRA(x, ref C.EntrustKey))
        .Widget("取回道具", (x) => UIUtils.QRA(x, ref C.RetrieveKey))
        .Widget("上架出售", (x) => UIUtils.QRA(x, ref C.SellMarketKey));
}
