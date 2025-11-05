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

        GetVectorFromUserDate("10/12/1300", Body.Mars);
        GetVectorFromUserDate("10/12/1300", Body.Neptune);
        GetVectorFromUserDate("10/12/1300", Body.Sun);
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Converts a DateTime object into a geocentric Vector3 position for the given astral body
    Vector3 ConvertDateToVector(DateTime date, Body astralBody)
    {
        AstroVector av = Astronomy.GeoVector(astralBody, new AstroTime(date.ToUniversalTime()),Aberration.None);
        print("Mars pos at date : " + av.x + " " + av.y);
        return new Vector3( (float)(av.x), (float)(av.y), 0);
    }

    DateTime ConvertTextToDate(string text)
    {
        // format = "16/12/2018";
        System.DateTime dateTime = System.DateTime.Parse(text);
        print(dateTime.ToString());
        return dateTime;
    }

    Vector3 GetVectorFromUserDate(string UserInput, Body astralBody)
    {
        return ConvertDateToVector(ConvertTextToDate(UserInput), astralBody);
    }
}

