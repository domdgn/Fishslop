#nullable enable

using System;
using DebugPanel.Datas;
using DebugPanel.Popups;
using DebugPanel.Extensions;
using DebugPanel.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DebugPanel.Logic
{
    public sealed class PopupsLogic
    {
        readonly DebugPanelData _debugPanelData;
        readonly PopupsData _popupsData;
        readonly ControllersData _controllersData;
        readonly ControllerNavigationLogic _controllerNavigationLogic;

        public PopupsLogic(
            DebugPanelData debugPanelData, 
            PopupsData popupsData, 
            ControllersData controllersData, 
            ControllerNavigationLogic controllerNavigationLogic
            )
        {
            _debugPanelData = debugPanelData;
            _popupsData = popupsData;
            _controllersData = controllersData;
            _controllerNavigationLogic = controllerNavigationLogic;
        }

        public void AddPopup(Func<Transform, GameObject> spawnPopupAction)
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
         
            GameObject instance = spawnPopupAction.Invoke(_debugPanelData.DebugPanelView.PopupsParent);
            
            AddPopupToStack(instance);
            _controllerNavigationLogic.SetupControllerNavigation(instance);
        }
        
        public void AddPopup(Func<Transform, DebugPanelView, GameObject> spawnPopupAction)
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
         
            GameObject instance = spawnPopupAction.Invoke(_debugPanelData.DebugPanelView.PopupsParent, _debugPanelData.DebugPanelView);
            
            AddPopupToStack(instance);
            _controllerNavigationLogic.SetupControllerNavigation(instance);
        }

        public void RemovePopup(GameObject popup)
        {
            RemovePopupFromStack(popup);
            Object.Destroy(popup);
        }
        
        public void RemoveCurrentPopup()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            if (_popupsData.PopupsStack.Count == 0)
            {
                return;
            }

            GameObject currentPopup = _popupsData.PopupsStack[^1];
            
            RemovePopup(currentPopup);
        }

        public void AddPopupToStack(GameObject popup)
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            if (_controllersData.ControllerSupport)
            {
                if (_popupsData.PopupsStack.Count == 0)
                {
                    if (EventSystem.current != null)
                    {
                        _popupsData.PreviouslySelected = EventSystem.current.currentSelectedGameObject;
                    }
                    
                    _debugPanelData.DebugPanelView.MainPanelParent.gameObject.GetOrAddComponent<CanvasGroup>().interactable = false;
                }
                else
                {
                    GameObject currentPopup = _popupsData.PopupsStack[^1];
                    currentPopup.GetOrAddComponent<CanvasGroup>().interactable = false;
                    
                    DebugPanelPopup? currentDebugPanelPopup = currentPopup.GetComponent<DebugPanelPopup>();

                    if (currentDebugPanelPopup != null)
                    {
                        currentDebugPanelPopup.WhenUnfocusedFromStack();
                    }
                }         
            }
            
            _popupsData.PopupsStack.Add(popup);

            if (_controllersData.ControllerSupport)
            {
                Selectable? selectable = GetPopupSelectable(popup);

                if (selectable == null)
                {
                    return;
                }

                selectable.Select();
            }
            
            DebugPanelPopup? debugPanelPopup = popup.GetComponent<DebugPanelPopup>();

            if (debugPanelPopup != null)
            {
                debugPanelPopup.WhenFocusedOnStack();
            }
        }

        public void RemovePopupFromStack(GameObject popup)
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            if (_popupsData.PopupsStack.Count == 0)
            {
                return;
            }

            bool isCurrent = popup == _popupsData.PopupsStack[^1];

            if (!isCurrent)
            {
                _popupsData.PopupsStack.Remove(popup);
                return;
            }
            
            _popupsData.PopupsStack.RemoveAt(_popupsData.PopupsStack.Count - 1);
            
                        
            DebugPanelPopup? debugPanelPopup = popup.GetComponent<DebugPanelPopup>();

            if (debugPanelPopup != null)
            {
                debugPanelPopup.WhenUnfocusedFromStack();
            }
            
            if (_popupsData.PopupsStack.Count > 0)
            {
                GameObject newCurrentPopup = _popupsData.PopupsStack[^1];
                DebugPanelPopup? newDebugPanelPopup = newCurrentPopup.GetComponent<DebugPanelPopup>();
            
                if (newDebugPanelPopup != null)
                {
                    newDebugPanelPopup.WhenFocusedOnStack();
                }
            }
            
            if (!_controllersData.ControllerSupport)
            {
                return;
            }
            
            if (_popupsData.PopupsStack.Count == 0)
            {
                _debugPanelData.DebugPanelView.MainPanelParent.gameObject.GetOrAddComponent<CanvasGroup>().interactable = true;

                if (_popupsData.PreviouslySelected != null)
                {
                    bool isSelectable = _popupsData.PreviouslySelected.TryGetComponent(out Selectable selectable);

                    if (isSelectable)
                    {
                        selectable.Select();
                    }
                }
                else
                {
                    Selectable selectable = _debugPanelData.DebugPanelView.GetComponentInChildren<Selectable>();

                    if (selectable == null)
                    {
                        return;
                    }
            
                    selectable.Select();
                }
            }
            else
            {
                GameObject nextPopup = _popupsData.PopupsStack[^1];
                nextPopup.GetOrAddComponent<CanvasGroup>().interactable = true;   
                
                Selectable? selectable = GetPopupSelectable(nextPopup);

                if (selectable != null)
                {
                    selectable.Select();
                }
            }
        }

        public void ClearPopupsStack()
        {
            _popupsData.PopupsStack.Clear();
        }

        public Selectable? GetPopupSelectable(GameObject popup)
        {
            DebugPanelPopup? debugPanelPopup = popup.gameObject.GetComponent<DebugPanelPopup>();

            if (debugPanelPopup != null)
            {
                Selectable? firstSelectable = debugPanelPopup.GetFirstFocusedSelectable();

                if (firstSelectable != null)
                {
                    return firstSelectable;
                }
            }
            
            return popup.GetComponentInChildren<Selectable>();
        }
    }
}