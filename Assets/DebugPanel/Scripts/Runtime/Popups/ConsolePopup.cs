#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DebugPanel.Core;
using DebugPanel.Raiseables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Popups
{
    public sealed class ConsolePopup : MonoBehaviour
    {
        [Header("References")] 
        public GameObject LoadingContent;
        public TextMeshProUGUI? TitleLabel;
        public Button? CloseButton;
        public Button? CopyButton;
        public ScrollRect? ScrollRect;
        public Button? ScrollToTopButton;
        public Button? ScrollToBottomButton;
        public Transform? LogsParent;

        [Header("Prefabs")] 
        public TextMeshProUGUI LogEntryViewPrefab;

        [Header("Values")] 
        [Min(1)] public int MaxLogs = 2000;

        IListenRaiseable<string>? _addLogRaiseable;

        bool _isSpawningLogs;
        Queue<string>? _logsToSpawn;

        readonly Queue<TextMeshProUGUI> _logsToEnable = new();
        readonly List<TextMeshProUGUI> _spawnedLogs = new();
        
        readonly Stopwatch _frameTimeStopwatch = new();
        readonly StringBuilder _stringBuilder = new();

        bool _automaticallyScrollToBottom = true;

        void Awake()
        {
            CopyButton!.onClick.AddListener(WhenCopy);
            ScrollToTopButton!.onClick.AddListener(WhenScrollToTopButtonClicked);
            ScrollToBottomButton!.onClick.AddListener(WhenScrollToBottomButtonClicked);
            
            ScrollToTopButton!.gameObject.SetActive(false);
            ScrollToBottomButton!.gameObject.SetActive(false);
        }

        void Update()
        {
            TickLogSpawning();
            RefreshScrollToButtons();
        }

        void OnDestroy()
        {
            if (_addLogRaiseable != null)
            {
                _addLogRaiseable.WhenRaised -= WhenAddLog;   
            }
        }

        public void Setup(string title, IReadOnlyList<string> logs, IListenRaiseable<string>? addLogRaiseable)
        {
            TitleLabel!.text = title;
            CloseButton!.onClick.AddListener(WhenClosed);
            _addLogRaiseable = addLogRaiseable;

            int logsToSpawnStart = logs.Count - MaxLogs;
            logsToSpawnStart = Math.Max(logsToSpawnStart, 0);
            int logsToSpawnCount = Math.Min(logs.Count, MaxLogs);
            
            _logsToSpawn = new Queue<string>(logsToSpawnCount);

            for (int i = logsToSpawnStart; i < logs.Count; i++)
            {
                string log = logs[i];
                _logsToSpawn.Enqueue(log); 
            }
            
            if (_addLogRaiseable != null)
            {
                _addLogRaiseable.WhenRaised += WhenAddLog;
            }

            TickLogSpawning();
        }

        void AddLog(string log)
        {
            _stringBuilder.Append(log);

            while (_spawnedLogs.Count > MaxLogs)
            {
                TextMeshProUGUI previousInstance = _spawnedLogs[0];
                _spawnedLogs.RemoveAt(0);
                Destroy(previousInstance.gameObject);
            }
            
            TextMeshProUGUI logInstance = Instantiate(LogEntryViewPrefab, LogsParent);
            logInstance.gameObject.SetActive(false);
            logInstance.text = log;
            
            _spawnedLogs.Add(logInstance);
            _logsToEnable.Enqueue(logInstance);
        }

        void WhenAddLog(string log)
        {
            _logsToSpawn?.Enqueue(log);
        }
        
        void WhenClosed()
        {
            UDebugPanel.Popups.RemovePopup(gameObject);
        }

        void WhenCopy()
        {
            GUIUtility.systemCopyBuffer = _stringBuilder.ToString();
        }

        void TickLogSpawning()
        {
            if (!_frameTimeStopwatch.IsRunning)
            {
                _frameTimeStopwatch.Start();
            }
            else
            {
                _frameTimeStopwatch.Restart();
            }
            
            if(_logsToSpawn == null) return;
            if(_logsToSpawn.Count == 0 && _logsToEnable.Count == 0) return;

            if (!_isSpawningLogs)
            {
                if (_logsToSpawn.Count > 20)
                {
                    LoadingContent.gameObject.SetActive(true);
                }
                
                _isSpawningLogs = true;
            }

            RefreshShouldAutomaticallyScrollToBottom();
            
            while (_logsToSpawn.Count > 0)
            {
                string log = _logsToSpawn.Dequeue();
                AddLog(log);
                
                if (_frameTimeStopwatch.ElapsedMilliseconds > 4)
                {
                    break;
                }
            }

            if (_logsToSpawn.Count == 0)
            {
                while (_logsToEnable.Count > 0)
                {
                    TextMeshProUGUI textMeshProUGUI = _logsToEnable.Dequeue();
                    textMeshProUGUI.gameObject.SetActive(true);
                    
                    if (_frameTimeStopwatch.ElapsedMilliseconds > 4)
                    {
                        break;
                    }
                }
            }

            if (_logsToSpawn.Count == 0 && _logsToEnable.Count == 0)
            {
                _isSpawningLogs = false;
                LoadingContent.gameObject.SetActive(false);
            }
            
            TryScrollToBottom();
        }
        
        void RefreshScrollToButtons()
        {
            bool canUse = ScrollRect!.verticalScrollbar.gameObject.activeInHierarchy;

            if (!canUse)
            {
                ScrollToTopButton!.gameObject.SetActive(false);
                ScrollToBottomButton!.gameObject.SetActive(false);
                return;
            }
            
            bool shouldGoTop = ScrollRect!.verticalNormalizedPosition < 0.5f;
            
            ScrollToTopButton!.gameObject.SetActive(shouldGoTop);
            ScrollToBottomButton!.gameObject.SetActive(!shouldGoTop);
        }

        void RefreshShouldAutomaticallyScrollToBottom()
        {
            _automaticallyScrollToBottom = IsBarAtBottom();
        }
        
        void TryScrollToBottom()
        {
            if (!_automaticallyScrollToBottom)
            {
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollRect!.transform as RectTransform);
            
            StartCoroutine(SnapToBottom());
        }
        
        IEnumerator SnapToBottom()
        {
            yield return new WaitForEndOfFrame();
            ScrollRect!.verticalNormalizedPosition = 0;
        }
        
        IEnumerator SnapToTop()
        {
            yield return new WaitForEndOfFrame();
            ScrollRect!.verticalNormalizedPosition = 1;
        }
        
        bool IsBarAtBottom()
        {
            return Mathf.Abs(ScrollRect!.verticalNormalizedPosition) < 0.01f;
        }

        void WhenScrollToBottomButtonClicked()
        {
            StartCoroutine(SnapToBottom());
        }
        
        void WhenScrollToTopButtonClicked()
        {
            StartCoroutine(SnapToTop());
        }
    }
}