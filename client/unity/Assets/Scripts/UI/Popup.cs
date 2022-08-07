using System;
using System.Threading.Tasks;
using Codice.CM.SEIDInfo;
using Cysharp.Threading.Tasks;

namespace UI
{
    public static class Popup
    {
        public static async Task<bool> Boolean(string title, string yes, string no)
        {
            var popup = EntryPoint.uiPool.Get<BoolPopup>();
            popup.SetPopupTexts(title, yes, no);
            popup.SetActive(true);
            var response = await popup.Response();
            popup.SetActive(false);
            return response;
        }
    }
}