using System;
using System.Collections;
using System.Collections.Generic;
using CosineKitty;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    //Converts a DateTime object into a geocentric Vector3 position for the given astral body
    Vector3 ConvertDateToVector(DateTime date, Body astralBody)
    {
        AstroVector av = Astronomy.GeoVector(astralBody, new AstroTime(date.ToUniversalTime()),Aberration.None);
        switch (astralBody)
        {
            case Body.Jupiter:
                av = av / 2;
                break;
            case Body.Saturn:
                av = av / 3;
                break;
            case Body.Moon:
                av = av / .01f;
                break;
            case Body.Earth:
                break;

        }
        return new Vector3( (float)(av.x), (float)(av.y), (float)(av.z));
    }

    public Vector3 GetVectorFromUserDate(string UserInput, Body astralBody)
    {
        return ConvertDateToVector(System.DateTime.Parse(UserInput), astralBody);
    }

    public List<Vector3> GetVectorsBetweenDates(DateTime startDate, DateTime endDate, Body astralBody)
    {
        List<Vector3> result = new List<Vector3>();

        if (startDate == endDate) return result;

        bool forward = startDate < endDate;
        DateTime current = startDate;

        while (current != endDate)
        {
            result.Add(ConvertDateToVector(current, astralBody));

            if (forward) current = current.AddDays(1);
            else current = current.AddDays(-1);
        }

        return result;
    }
}

