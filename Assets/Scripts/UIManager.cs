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
    Tab_SignAstro ZodiacInfo;



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
            print("heyyy");
            AstroCard.SetActive(false);
        }
        else
        {
            SoundManager.Instance.Play("clic_in");
            AstroCard.SetActive(true);
            UpdateAstroCard();
        }
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

        AstroCardSignImage.GetComponent<Image>().sprite = ZodiacSpriteListWhite[(int)sign];
        AstroCardSignText.GetComponent<TextMeshProUGUI>().text = sign.ToString();
        AstroCardMainText.GetComponent<TextMeshProUGUI>().text = ZodiacInfo.signastro[(int)sign].description;
    }
}
