using TMPro;
using UnityEngine;

namespace Balltallion.UI
{
    public class VersionText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI versionText;

        private void Awake()
        {
            versionText.text = $"v{Application.version}";
        }
    }
}
