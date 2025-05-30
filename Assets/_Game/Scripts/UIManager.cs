using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI HitCounterText;

    public void UpdateHitCounter(int hits)
    {
        HitCounterText.text = "Hits: " + hits;
    }
}

