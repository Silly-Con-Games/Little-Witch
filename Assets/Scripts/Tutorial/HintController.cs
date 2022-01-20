using UnityEngine;

public class HintController : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI hint;
    [SerializeField] private TMPro.TextMeshProUGUI buttonToPress;

    private string button;
    private string buttonGamepad;

    private PlayerController playerController = null;

    public void SetHintText(string text, string button, string buttonGamepad)
    {
        hint.text = text;
        buttonToPress.text = button;

        this.button = button;
        this.buttonGamepad = buttonGamepad;

        playerController = FindObjectOfType<PlayerController>();
        playerController.controlSchemeChanged.AddListener(SwitchText);
    }

    private void SwitchText(bool gamepad)
    {
        buttonToPress.text = gamepad ? buttonGamepad : button;
    }

    public void HideHint()
    {
        playerController.controlSchemeChanged.RemoveListener(SwitchText);
        Destroy(gameObject);
    }

}
