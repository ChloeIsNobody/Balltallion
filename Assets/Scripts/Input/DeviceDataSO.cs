using System;
using System.Collections.Generic;
using UnityEngine;

namespace Balltallion.Input
{
    [CreateAssetMenu(menuName = "Balltallion/Input/DeviceData", fileName = "DeviceData")]
    public class DeviceDataSO : ScriptableObject
    {
        [SerializeField] public string displayName;
        [SerializeField] public Sprite icon;
        [SerializeField] public Sprite missingControlIcon;

        [SerializeField] private List<IconMapping> iconMappingsList;
        private Dictionary<string, Sprite> iconMappings = new();
        
        [Serializable]
        private struct IconMapping
        {
            public string controlName;
            public Sprite icon;
        }

        private void OnValidate()
        {
            iconMappings = new Dictionary<string, Sprite>();
            foreach (var mapping in iconMappingsList) iconMappings.Add(mapping.controlName, mapping.icon);
        }
        
        public Sprite GetIconForControl(string actionName) => iconMappings.ContainsKey(actionName) ? iconMappings[actionName] : missingControlIcon;
    }
    
    
}