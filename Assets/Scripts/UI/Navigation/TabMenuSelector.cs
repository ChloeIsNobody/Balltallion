using System;
using System.Collections.Generic;
using UnityEngine;

namespace Balltallion.UI.Navigation
{
    public class TabMenuSelector : MonoBehaviour
    {
        public event Action OnTabSelected;
        
        [SerializeField] private List<MenuTab> tabs;
        
        private MenuTab selectedTab;

        private void Start()
        {
            if (tabs.Count > 0) SelectTab(tabs[0], false);   
            foreach (MenuTab tab in tabs)
            {
                tab.BindSelectionRequest(() => SelectTab(tab));
            }
        }
        
        public void SelectTabIdx(int idx) => SelectTab(tabs[idx]);

        private void SelectTab(MenuTab tabToSelect, bool invokeEvent = true)
        {
            if (tabToSelect == selectedTab) return;
            
            foreach (MenuTab tab in tabs)
            {
                if (tab == tabToSelect) tab.Select();
                else tab.Deselect();
            }
            selectedTab = tabToSelect;
            if (invokeEvent) OnTabSelected?.Invoke();
        }

        public CanvasGroup GetSelectedCanvasGroup() => selectedTab.GetCanvasGroup();
    }
}