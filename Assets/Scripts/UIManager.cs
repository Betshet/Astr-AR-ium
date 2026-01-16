using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /*-------Astrology button------*/
    [SerializeField]
    GameObject AstroButton;

    [SerializeField]
    GameObject AstroButtonSignImage;

    /*-------Astrology card------*/
    [SerializeField]
    GameObject AstroCard;

    [SerializeField]
    GameObject AstroCardSignImage;

    [SerializeField]
    GameObject AstroCardSignText;

    [SerializeField]
    GameObject AstroCardMainText;

    /*-------Sprite lists------*/
    [SerializeField]
    List<Sprite> ZodiacSpriteList;

    [SerializeField]
    List<Sprite> ZodiacSpriteListWhite;

    [SerializeField]
    Tab_SignAstro ZodiacInfoFrench;

    [SerializeField]
    Tab_SignAstro ZodiacInfoEnglish;

    [SerializeField]
    Tab_SignAstro ZodiacInfoSpanish;



    Language currentLanguage = Language.French;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayAstrology(DateTime date)
    {
        AstroButton.SetActive(true);
        UpdateZodiacSignButton(date);
    }

    public void HideAstrology()
    {
        AstroButton.SetActive(false);
        AstroCard.SetActive(false);
    }

    public void OnClick_AstroButton()
    {
        if (AstroCard.activeSelf)
        {
            SoundManager.Instance.Play("clic_out");
            AstroCard.SetActive(false);
        }
        else
        {
            SoundManager.Instance.Play("clic_in");
            AstroCard.SetActive(true);
            UpdateAstroCard();
        }
    }

    public void OnClick_CloseAstroCard()
    {
        AstroCard.SetActive(false);
    }

    void UpdateZodiacSignButton(DateTime date)
    {
        Signastro sign = Astrology.GetSignFromDate(date);
        AstroButtonSignImage.GetComponent<Image>().sprite = ZodiacSpriteList[(int)sign];
    }

    void UpdateAstroCard()
    {
        DateTime date = GameManager.Instance.currentDate;
        Signastro sign = Astrology.GetSignFromDate(date);
        Tab_SignAstro astroTexts;
        switch (currentLanguage)
        {
            case Language.English:
                astroTexts = ZodiacInfoEnglish;
                break;
            case Language.Spanish:
                astroTexts = ZodiacInfoSpanish;
                break;
            case Language.French:
            default:
                astroTexts = ZodiacInfoFrench;
                break;

        }
        AstroCardSignImage.GetComponent<Image>().sprite = ZodiacSpriteListWhite[(int)sign];
        AstroCardSignText.GetComponent<TextMeshProUGUI>().text = astroTexts.signastro[(int)sign].sign;
        AstroCardMainText.GetComponent<TextMeshProUGUI>().text = astroTexts.signastro[(int)sign].description;

        //GET TRANSLATION MANAGER
    }

    public void onClick_ChangeLanguageFrench()
    {
        TranslationManager.Instance.TranslateAll(Language.French);
        currentLanguage = Language.French;
        UpdateAstroCard();
    }
    public void onClick_ChangeLanguage_English()
    {
        TranslationManager.Instance.TranslateAll(Language.English);
        currentLanguage = Language.English;
        UpdateAstroCard();
    }
    public void onClick_ChangeLanguage_Spanish()
    {
        TranslationManager.Instance.TranslateAll(Language.Spanish);
        currentLanguage = Language.Spanish;
        UpdateAstroCard();
    }

}
