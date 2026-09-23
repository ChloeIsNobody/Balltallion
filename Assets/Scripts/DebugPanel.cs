using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public static DebugPanel Instance;
    
    [SerializeField] private List<TextMeshProUGUI> debugTexts;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogError("There can't be multiple instances of DebugPanel (singleton)", this);
            Destroy(gameObject);
        }
        
        foreach (TextMeshProUGUI text in debugTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    public void SetDebugLabel(int index, string title, string value)
    {
        debugTexts[index].text = $"{title}: {value}";
        debugTexts[index].gameObject.SetActive(true);
    }
}
