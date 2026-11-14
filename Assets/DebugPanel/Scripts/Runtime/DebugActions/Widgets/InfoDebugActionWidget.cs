#nullable enable

using TMPro;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class InfoDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI? Label;

        public void Init(string info)
        {
            Label!.text = info;
        }
    }
}