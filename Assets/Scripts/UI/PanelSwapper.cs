using UnityEngine;

namespace FishSlop
{
    public class PanelSwapper : MonoBehaviour
    {
        public Panel[] panels;

        public void SwapUIPanel(string panelName)
        {
            foreach (Panel panel in panels)
            {
                if (panel.panelName == panelName)
                {
                    panel.gameObject.SetActive(true);
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }
            }
        }
    }
}
