using UnityEngine;

public class HintController : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI hint;
    [SerializeField] private TMPro.TextMeshProUGUI buttonToPress;

    public void SetHintText(string text, string button)
    {
        hint.text = text;
        buttonToPress.text = button;
    }

    public void HideHint()
    {
        Destroy(gameObject);
    }

}
