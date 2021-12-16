using UnityEngine;

public class TutorialTipController : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI tipText;

    public void SetTipText(string text)
    {
        tipText.text = text;
    }

    public void HideTip()
    {
        Destroy(gameObject);
    }

}
