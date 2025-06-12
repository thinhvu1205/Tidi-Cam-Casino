using Globals;
using UnityEngine;

public class PopupSupport : BaseView
{
    // Thay link dưới bằng link thật của bạn (Telegram username / Facebook page)
    private const string TelegramLink = "https://t.me/kh999channel";

    public void OnclickTele()
    {
        Application.OpenURL(TelegramLink);
    }

    public void OnclickMess()
    {
        Application.OpenURL(Config.chat_support_link);
    }
}
