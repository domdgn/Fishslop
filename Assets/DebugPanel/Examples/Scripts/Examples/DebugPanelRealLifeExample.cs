using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelRealLifeExample : MonoBehaviour
    {
        bool _godMode;
        int _health = 100;
        string _gameMode = "Arcade";
        int _enemiesCount = 52;
        bool _canFly;
        int _targetFps;
        
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();

            IDebugActionsSection gameplaySection = UDebugPanel.AddSection("Gameplay");
            gameplaySection.AddToggle("God Mode", v => _godMode = v, () => _godMode);
            gameplaySection.AddIntSelector("Health", v => _health = v, () => _health);
            gameplaySection.AddInfoDynamic(() => $"Game Mode: {_gameMode}");
            gameplaySection.AddInfoDynamic(() => $"Enemies Count: {_enemiesCount}");
            gameplaySection.AddButton("Kill Player", () => { Debug.Log("Kill Player"); });
            gameplaySection.AddToggle("Can Fly", v => _canFly = v, () => _canFly);
            gameplaySection.AddButton("Spawn Enemy",() => { Debug.Log("Spawn Enemy"); });
            gameplaySection.AddButton("Show Missions List",() => { Debug.Log("Show Missions List"); });
            gameplaySection.AddButton("Complete Mission",() => { Debug.Log("Complete Mission"); });
            gameplaySection.AddButton("Go To Main Menu",() => { Debug.Log("Go To Main Menu"); });
            gameplaySection.AddInfoDynamic(() => "Ping: 32ms");
            gameplaySection.AddInfoDynamic(() => "Fps: 60");
            gameplaySection.AddIntSelector("Target Fps", v => _targetFps = v, () => _targetFps);
            gameplaySection.AddButton("Play As No Internet Available", () => { Debug.Log("Play As No Internet Available"); });
            
            IDebugActionsSection itemsSection = UDebugPanel.AddSection("Items");
            itemsSection.AddToggle("Add Item", v => _godMode = v, () => _godMode);
            itemsSection.AddIntSelector("Items Count", v => _health = v, () => _health);
            itemsSection.AddInfoDynamic(() => $"Remote Items Enabled: {_gameMode}");
            itemsSection.AddInfoDynamic(() => $"Local Items Enabled: {_enemiesCount}");
            itemsSection.AddButton("Show All", () => { Debug.Log("Kill Player"); });
            itemsSection.AddToggle("Debug Items", v => _canFly = v, () => _canFly);
            
            IDebugActionsSection missionsSection = UDebugPanel.AddSection("Missions");
            missionsSection.AddToggle("Add Mission", v => _godMode = v, () => _godMode);
            missionsSection.AddIntSelector("Missions Count", v => _health = v, () => _health);
            missionsSection.AddInfoDynamic(() => $"Remote Missions Enabled: {_gameMode}");
            missionsSection.AddInfoDynamic(() => $"Local Missions Enabled: {_enemiesCount}");
            missionsSection.AddButton("Show All", () => { Debug.Log("Kill Player"); });
            missionsSection.AddToggle("Debug Missions", v => _canFly = v, () => _canFly);
            
            IDebugActionsSection menusSection = UDebugPanel.AddSection("Menus");
            IDebugActionsSection offersSection = UDebugPanel.AddSection("Offers");
            IDebugActionsSection shopSection = UDebugPanel.AddSection("Shop");
            IDebugActionsSection serverSection = UDebugPanel.AddSection("Server");
            IDebugActionsSection saveGameSection = UDebugPanel.AddSection("Save Game");
            IDebugActionsSection deviceInfoSection = UDebugPanel.AddSection("Device Info");
        }
    }
}