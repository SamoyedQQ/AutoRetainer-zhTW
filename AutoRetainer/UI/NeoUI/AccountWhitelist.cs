using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI;
public sealed unsafe class AccountWhitelist : NeoUIEntry
{
    public override void Draw()
    {
        ImGuiEx.TextWrapped($"你可以設定帳號白名單。若使用不在白名單內的帳號登入，AutoRetainer 不會記錄任何角色、僱員或潛水艇。");
        if(C.WhitelistedAccounts.Count == 0)
        {
            ImGuiEx.TextWrapped(EColor.GreenBright, "目前白名單狀態：停用。加入任一帳號即可啟用。");
        }
        else
        {
            ImGuiEx.TextWrapped(EColor.YellowBright, "目前白名單狀態：啟用。移除所有帳號即可停用。");
        }

        if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.UserPlus, "加入目前帳號", enabled: Player.Available))
        {
            C.WhitelistedAccounts.Add(*P.Memory.MyAccountId);
        }

        foreach(var x in C.WhitelistedAccounts)
        {
            ImGuiEx.PushID(x.ToString());
            if(ImGuiEx.IconButton(FontAwesomeIcon.Trash))
            {
                new TickScheduler(() => C.WhitelistedAccounts.Remove(x));
            }
            ImGui.SameLine();
            ImGuiEx.TextV($"帳號 {x}");
            ImGui.PopID();
        }
    }
}