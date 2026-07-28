namespace AutoRetainer.UI.NeoUI.AdvancedEntries;
public class LogTab : NeoUIEntry
{
    public override string Path => "進階/記錄";

    public override void Draw()
    {
        InternalLog.PrintImgui();
    }
}
