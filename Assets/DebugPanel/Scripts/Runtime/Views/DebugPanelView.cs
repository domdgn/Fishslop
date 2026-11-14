using DebugPanel.Core;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DebugPanel.Views
{
    public sealed class DebugPanelView : MonoBehaviour
    {
        [Header("Prefabs")]
        public DebugPanelSectionView DebugPanelSectionViewPrefab;

        [Header("References")] 
        public RectTransform MainPanelParent;
        public GameObject SectionsContent;
        public GameObject WidgetsContent;
        public GameObject PreviousSectionContent;
        public Button CloseButton;
        public Button ShortcutButton;
        public Button PreviousSectionButton;
        public TextMeshProUGUI PreviousSectionButtonLabel;
        public TextMeshProUGUI ShortcutLabel;
        public RectTransform SectionsContentParent;
        public RectTransform WidgetsContentParent;
        public TextMeshProUGUI WidgetsSelectedSectionLabel;
        public RectTransform PopupsParent;
        public TMP_InputField FilterInputField;
        public Button ClearFilterButton;
        public Button SectionSelectorButton;
        public Button SectionDocumentationButton;
        public DebugPanelDocumentationPopup DocumentationPopup;
        public DebugPanelSectionSelectorPopup SectionSelectorPopup;
        public TextMeshProUGUI VersionLabel;

        [Header("Widgets")] 
        public ButtonDebugActionWidget ButtonDebugActionWidgetPrefab;
        public InfoDebugActionWidget InfoDebugActionWidget;
        public DynamicInfoDebugActionWidget DynamicInfoDebugActionWidget;
        public ToggleDebugActionWidget ToggleDebugActionWidget;
        public IntSelectorDebugActionWidget IntSelectorDebugActionWidget;
        public LongSelectorDebugActionWidget LongSelectorDebugActionWidget;
        public FloatSelectorDebugActionWidget FloatSelectorDebugActionWidget;
        [FormerlySerializedAs("ButtonListSelectorDebugActionWidget")] public ButtonListInputDebugActionWidget ButtonListInputDebugActionWidget;
        [FormerlySerializedAs("ButtonElementListSelectorDebugActionWidget")] public ButtonListSelectorDebugActionWidget ButtonListSelectorDebugActionWidget;
        public ButtonLargeInfoDebugActionWidget ButtonLargeInfoDebugActionWidget;
        public ButtonConsoleDebugActionWidget ButtonConsoleDebugActionWidget;
        public ButtonActionsContainerDebugActionWidget ButtonActionsContainerDebugActionWidget;
        [FormerlySerializedAs("ButtonTextInputDebugActionWidget")] public ButtonStringInputDebugActionWidget ButtonStringInputDebugActionWidget;
        public ButtonStringSelectorDebugActionWidget ButtonStringSelectorDebugActionWidget;
        public ActionsGroupDebugActionWidget ActionsGroupDebugActionWidget;

        [Header("Popups")] 
        public ActionsContainerPopup ActionsContainerPopup;
        public ConsolePopup ConsolePopup;
        
        [Header("Values")] 
        public Color BaseSectionColor;
        public Color SelectedSectionColor;

        void Update()
        {
            UDebugPanel.CoreTrySelectSectionWithMorePriority();
        }
    }
}