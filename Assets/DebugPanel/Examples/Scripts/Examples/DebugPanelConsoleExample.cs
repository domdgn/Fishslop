using System;
using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using DebugPanel.Raiseables;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelConsoleExample : MonoBehaviour
    {
        readonly List<string> _logs = new();
        readonly IRaiseable<string> _logAddedRaiseable = new Raiseable<string>();

        bool _addLogEverySecond = true;
        float _addLogTime;
        
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.Show();

            IDebugActionsSection section = UDebugPanel.AddSection("Section");
            section.AddButton("Add Console Log", () => AddLog($"Added log with button at {DateTime.Now}"));
            section.AddToggle("Add Log Every Second", v => _addLogEverySecond = v, () => _addLogEverySecond);
            section.AddButtonConsole("Show Console", () => _logs, _logAddedRaiseable);

            AddLog("This is a list of logs");
            AddLog("You can add more logs by pressing the Add Console Log button");
        }

        void Update()
        {
            if (_addLogEverySecond)
            {
                _addLogTime += Time.deltaTime;
            }

            if (_addLogTime > 1)
            {
                _addLogTime = 0;
                AddLog($"New log at {DateTime.Now}");
            }
        }

        void AddLog(string log)
        {
            _logs.Add(log);
            _logAddedRaiseable.Raise(log);
        }
    }
}