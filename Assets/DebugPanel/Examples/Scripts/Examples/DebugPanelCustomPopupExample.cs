using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating how to setup a custom popup that
    /// works seamlessly with the debug panel.
    /// </summary>
    public sealed class DebugPanelCustomPopupExample : MonoBehaviour
    {
        public ExampleCustomPopup ExampleCustomPopupPrefab;
        
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();

            IDebugActionsSection section = UDebugPanel.AddSection("Custom Popup");
            
            section.AddButton("Open Popup", () =>
            {
                UDebugPanel.Popups.AddPopup(popupParent =>
                {
                    ExampleCustomPopup instance = Instantiate(ExampleCustomPopupPrefab, popupParent);
                    instance.Setup("Do Something Title", WhenDoSomethingButtonClicked);
                    return instance.gameObject;
                });
            });
        }

        void WhenDoSomethingButtonClicked()
        {
            Debug.Log("We are doing something!");
        }
    }
}