using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astrology : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Signastro GetSignFromDate(DateTime date)
    {
        if ((date.Month == 3 && date.Day >= 21) || (date.Month == 4 && date.Day <= 19))
        {
            return Signastro.Belier;
        }
        if ((date.Month == 4 && date.Day >= 20) || (date.Month == 5 && date.Day <= 20))
        {
            return Signastro.Taureau;
        }
        if ((date.Month == 5 && date.Day >= 21) || (date.Month == 6 && date.Day <= 20))
        {
            return Signastro.Gemeaux;
        }
        if ((date.Month == 6 && date.Day >= 21) || (date.Month == 7 && date.Day <= 22))
        {
            return Signastro.Cancer;
        }
        if ((date.Month == 7 && date.Day >= 23) || (date.Month == 8 && date.Day <= 22))
        {
            return Signastro.Lion;
        }
        if ((date.Month == 8 && date.Day >= 23) || (date.Month == 9 && date.Day <= 22))
        {
            return Signastro.Vierge;
        }
        if ((date.Month == 9 && date.Day >= 23) || (date.Month == 10 && date.Day <= 22))
        {
            return Signastro.Balance;
        }
        if ((date.Month == 10 && date.Day >= 23) || (date.Month == 11 && date.Day <= 21))
        {
            return Signastro.Scorpion;
        }
        if ((date.Month == 11 && date.Day >= 22) || (date.Month == 12 && date.Day <= 21))
        {
            return Signastro.Sagittaire;
        }
        if ((date.Month == 12 && date.Day >= 22) || (date.Month == 1 && date.Day <= 22))
        {
            return Signastro.Capricorne;
        }
        if ((date.Month == 1 && date.Day >= 22) || (date.Month == 2 && date.Day <= 21))
        {
            return Signastro.Verseau;
        }
        if ((date.Month == 2 && date.Day >= 22) || (date.Month == 3 && date.Day <= 20))
        {
            return Signastro.Poisson;
        }
        return Signastro.Taureau;
    }
}
