using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ObjectToTranslate
{
    public TextToTranslate textToTranslate = TextToTranslate.None;
    public Translation_Data translations;
}

public enum Language
{
    French,
    English,
    Spanish
}

public class TranslationManager : MonoBehaviour
{
    [SerializeField]
    ObjectToTranslate[] texts;

    private static TranslationManager _instance;

    public static TranslationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("TranslationManager");
                go.AddComponent<TranslationManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TranslateAll(Language language)
    {
        Translatable[] gameObjects = Object.FindObjectsOfType<Translatable>();
        foreach (Translatable obj in gameObjects)
        {
            foreach(ObjectToTranslate text in texts)
            {
                if(obj.textToTranslate == text.textToTranslate)
                {
                    string translation;
                    switch (language)
                    {
                        case Language.English:
                            translation = text.translations.englishText;
                            break;
                        case Language.French:
                            translation = text.translations.fenchText;
                            break;
                        case Language.Spanish:
                            translation = text.translations.spanishText;
                            break;
                        default:
                            translation = text.translations.fenchText;
                            break;
                    }

                    obj.GetComponent<TextMeshProUGUI>().SetText(translation); 
                    break;
                }
            }
        }

    }
}
