using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Signastro
{Belier,
Taureau,
Gemeaux,
Cancer,
Lion,
Vierge,
Balance, 
Scorpion,
Sagittaire,
Capricorne,
Verseau,
Poisson

    
}


[System.Serializable]
public class Text_data
{
  public Signastro astro_sign = Signastro.Belier;
  public string date = "";
  public string element ="";
  public string planet="";
  public string description= "";
  
}
