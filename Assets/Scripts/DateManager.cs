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

    // Update is called once per frame
    void Update()
    {
    }

    //Converts a DateTime object into a geocentric Vector3 position for the given astral body
    Vector3 ConvertDateToVector(DateTime date, Body astralBody)
    {
        AstroVector av = Astronomy.GeoVector(astralBody, new AstroTime(date.ToUniversalTime()),Aberration.None);
        return new Vector3( (float)(av.x), (float)(av.y), (float)(av.z));
    }

    DateTime ConvertTextToDate(string text)
    {
        // format = "16/12/2018";
        System.DateTime dateTime = System.DateTime.Parse(text);
        return dateTime;
    }

    public Vector3 GetVectorFromUserDate(string UserInput, Body astralBody)
    {
        return ConvertDateToVector(ConvertTextToDate(UserInput), astralBody);
    }

    public List<Vector3> GetVectorsBetweenDates(string startDate, string endDate, Body astralBody)
    {
        List<Vector3> result = new List<Vector3>();

        DateTime dt1 = DateTime.Parse(startDate);
        DateTime dt2 = DateTime.Parse(endDate);

        if (dt1 == dt2) return result;

        bool forward = dt1 < dt2;
        DateTime current = forward ? dt1 : dt2;
        DateTime goal = forward ? dt2 : dt1;

        while (current != goal)
        {
            result.Add(ConvertDateToVector(current, astralBody));
            current = current.AddDays(1);
        }

        return result;
    }
}

