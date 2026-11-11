using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Navigation
{
    public class TabMenuSelector : MonoBehaviour
    {
        [SerializeField] private List<MenuTab> tabs;

        private void Start()
        {
            if (tabs.Count > 0) SelectTab(tabs[0]);   
            foreach (MenuTab tab in tabs)
            {
                tab.BindSelectionRequest(() => SelectTab(tab));
            }
        }

        private void SelectTab(MenuTab tabToSelect)
        {
            foreach (MenuTab tab in tabs)
            {
                if (tab == tabToSelect) tab.Select();
                else tab.Deselect();
            }
        }
    }
}