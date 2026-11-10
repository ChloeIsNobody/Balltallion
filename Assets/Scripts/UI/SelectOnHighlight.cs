using System;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI
{
    public class SelectOnHighlight : MonoBehaviour
    {
        private Selectable selectable;

        private void Awake()
        {
            selectable = GetComponent<Selectable>();
        }

        private void Update()
        {
            if (selectable.IsHighlighted()) selectable.Select();
        }
    }
}