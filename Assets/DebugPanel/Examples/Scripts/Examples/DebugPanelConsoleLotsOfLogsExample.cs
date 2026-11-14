using System;
using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using DebugPanel.Raiseables;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelConsoleLotsOfLogsExample : MonoBehaviour
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
            section.AddButtonConsole("Show Console", () => _logs, _logAddedRaiseable);

            const int logsCount = 99;
            
            for (int i = 0; i < logsCount; ++i)
            {
                AddLog($"This is the log number {i + 1}/{logsCount}");
            }
        }

        void AddLog(string log)
        {
            _logs.Add(log);
            _logAddedRaiseable.Raise(log);
        }
    }
}