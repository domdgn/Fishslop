#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using DebugPanel.Core;
using DebugPanel.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Popups
{
    public sealed class LargeTextPopup : DebugPanelPopup
    {
        [Header("References")] 
        public TextMeshProUGUI? TitleLabel;
        public Button? CloseButton;
        public TextMeshProUGUI? TextLabel;
        public GameObject? TextBlock;
        public GameObject? LoadingBlock;
        
        Func<string>? _infoAction;
        Func<Task<string>>? _infoAsyncAction;
        bool _refreshEveryFrame;

        bool _isRefreshing;

        readonly CancellationTokenSource _cancellationTokenSource = new();
        
        void Awake()
        {
            CloseButton!.onClick.AddListener(WhenClosed);
        }

        void Update()
        {
            if (!_refreshEveryFrame)
            {
                return;
            }

            TryRefresh();
        }

        void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }

        public override Selectable? GetFirstFocusedSelectable()
        {
            return CloseButton!.GetComponentInChildren<Selectable>();
        }
        
        public void Setup(string title, Func<string> infoAction, bool refreshEveryFrame)
        {
            TitleLabel!.text = title;
            
            TextBlock!.SetActive(false);
            LoadingBlock!.SetActive(true);
            
            _infoAction = infoAction;
            _refreshEveryFrame = refreshEveryFrame;
            
            RefreshAsync().FireTaskAndForget();
        }
        
        public void Setup(string title, Func<Task<string>> infoAsyncAction, bool refreshEveryFrame)
        {
            TitleLabel!.text = title;
            
            TextBlock!.SetActive(false);
            LoadingBlock!.SetActive(true);
            
            _infoAsyncAction = infoAsyncAction;
            _refreshEveryFrame = refreshEveryFrame;
            
            RefreshAsync().FireTaskAndForget();
        }

        public void SetText(string text)
        {
            TextLabel!.text = text;
            
            TextBlock!.SetActive(true);
            LoadingBlock!.SetActive(false);
        }

        void WhenClosed()
        {
            UDebugPanel.Popups.RemovePopup(gameObject);
        }

        void TryRefresh()
        {
            if (_isRefreshing)
            {
                return;
            }
            
            RefreshAsync().FireTaskAndForget();
        }

        async Task RefreshAsync()
        {
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;

            if (_infoAction != null)
            {
                string info = _infoAction.Invoke();
                SetText(info);
            }
            else if(_infoAsyncAction != null)
            {
                string info = await _infoAsyncAction.Invoke();
                if(_cancellationTokenSource.IsCancellationRequested) return;
                
                SetText(info);
            }

            _isRefreshing = false;
        }
    }
}