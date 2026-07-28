namespace AutoRetainer.UI.NeoUI;
public class LoginOverlay : NeoUIEntry
{
    public override string Path => "登入浮動視窗";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
            .Section("登入浮動視窗")
            .Checkbox("顯示登入浮動視窗", () => ref C.LoginOverlay)
            .Widget("登入浮動視窗縮放倍率", (x) =>
            {
                ImGuiEx.SetNextItemWidthScaled(150f);
                if(ImGuiEx.SliderFloat(x, ref C.LoginOverlayScale.ValidateRange(0.1f, 5f), 0.2f, 2f)) P.LoginOverlay.bWidth = 0;
            })
            .Widget($"登入浮動視窗按鈕內距", (x) =>
            {
                ImGuiEx.SetNextItemWidthScaled(150f);
                if(ImGuiEx.SliderFloat(x, ref C.LoginOverlayBPadding.ValidateRange(0.5f, 5f), 1f, 1.5f)) P.LoginOverlay.bWidth = 0;
            })
        .Checkbox("搜尋時顯示已隱藏的角色", () => ref C.LoginOverlayAllSearch);
}
