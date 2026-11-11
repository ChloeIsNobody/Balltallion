using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Navigation
{
    public class TabMenuSelector : MonoBehaviour
    {
        [SerializeField] private List<MenuTab> tabs;
        
        private MenuTab selectedTab;

        private void Start()
        {
            if (tabs.Count > 0) SelectTab(tabs[0]);   
            foreach (MenuTab tab in tabs)
            {
                tab.BindSelectionRequest(() => SelectTab(tab));
            }
        }
        
        public void SelectTabIdx(int idx) => SelectTab(tabs[idx]);

        private void SelectTab(MenuTab tabToSelect)
        {
            foreach (MenuTab tab in tabs)
            {
                if (tab == tabToSelect) tab.Select();
                else tab.Deselect();
            }
            selectedTab = tabToSelect;
        }

        public void GetSelectedCanvasGroup() => selectedTab.GetCanvasGroup();
    }
}