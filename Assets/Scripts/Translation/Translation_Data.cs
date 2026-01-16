using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TextToTranslate
{
    None,
    EntrerDate,
    Day,
    Month,
    Year,
    Confirm,
    Earth,
    Sun,
    Moon,
    Venus,
    Mars,
    Mercury,
    Saturn,
    Jupiter,
    TurnWheel
}

[System.Serializable]
public class Translation_Data
{
    public string fenchText = "";
    public string englishText = "";
    public string spanishText = "";
}
