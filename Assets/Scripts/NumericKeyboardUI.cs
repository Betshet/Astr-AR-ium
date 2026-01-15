using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumericKeyboardUI : MonoBehaviour
{
    [Header("Keyboard Root")]
    public GameObject keyboardRoot;

    [Header("Input Fields")]
    public TMP_InputField dayInput;
    public TMP_InputField monthInput;
    public TMP_InputField yearInput;

    private TMP_InputField currentInput;

    void Start()
    {
        //Empecher clavier systeme
        dayInput.readOnly = true;
        monthInput.readOnly = true;
        yearInput.readOnly = true;

        keyboardRoot.SetActive(false);
    }
    public void SelectInput(TMP_InputField input)
    {
        currentInput = input;
        keyboardRoot.SetActive(true);
    }

    public void AddDigit(string digit)
    {
        SoundManager.Instance.Play("clic_in");
        if (currentInput == null) return;

        if (currentInput == dayInput && currentInput.text.Length >= 2) return;
        if (currentInput == monthInput && currentInput.text.Length >= 2) return;
        if (currentInput == yearInput && currentInput.text.Length >= 4) return;

        currentInput.text += digit;
    }

    public void Clear()
    {
        if (currentInput == null) return;
        currentInput.text = "";
    }

    public void Backspace()
    {
        if (currentInput == null) return;

        if (currentInput.text.Length > 0)
            currentInput.text = currentInput.text.Substring(0, currentInput.text.Length - 1);
    }
    public void CloseKeyboard()
    {
        SoundManager.Instance.Play("clic_out");
        currentInput = null;
        keyboardRoot.SetActive(false);
    }
}
