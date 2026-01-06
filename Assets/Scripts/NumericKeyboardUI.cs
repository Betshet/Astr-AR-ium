using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumericKeyboardUI : MonoBehaviour
{
    [Header("Keyboard Root")]
    public GameObject keyboardRoot; // L'empty avec l'image de fond + boutons

    [Header("Input Fields")]
    public TMP_InputField dayInput;
    public TMP_InputField monthInput;
    public TMP_InputField yearInput;

    private TMP_InputField currentInput;

    void Start()
    {
        // Empeche le clavier système
        dayInput.readOnly = true;
        monthInput.readOnly = true;
        yearInput.readOnly = true;

        keyboardRoot.SetActive(false);
    }

    // Appelé quand on clique sur un input
    public void SelectInput(TMP_InputField input)
    {
        currentInput = input;
        keyboardRoot.SetActive(true);
    }

    // Appele par les boutons 0–9
    public void AddDigit(string digit)
    {
        if (currentInput == null) return;

        // Limites par champ
        if (currentInput == dayInput && currentInput.text.Length >= 2) return;
        if (currentInput == monthInput && currentInput.text.Length >= 2) return;
        if (currentInput == yearInput && currentInput.text.Length >= 4) return;

        currentInput.text += digit;
    }

    // Bouton Clear
    public void Clear()
    {
        if (currentInput == null) return;
        currentInput.text = "";
    }

    // Appele quand on clique ailleurs
    public void CloseKeyboard()
    {
        currentInput = null;
        keyboardRoot.SetActive(false);
    }
}
