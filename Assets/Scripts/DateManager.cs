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
                av = av / 3;
                break;
            case Body.Saturn:
                av = av / 4;
                break;
            case Body.Mars:
                av = av / 1.4;
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
        //List of vectors corresponding to the position of the given celestial body over the given period of time
        List<Vector3> result = new List<Vector3>();

        if (startDate == endDate) return result;

        //We can go forwards or backwards in time
        bool forward = startDate < endDate;

        DateTime current = startDate;

        //The steps between each selected day
        //By default, we select every day
        int daysSpacing = 1;

        double totalDays = Math.Abs((endDate - startDate).TotalDays);

        //If the two dates are more than 200 days apart, we adjust the steps between each day
        if (totalDays > 200d)
        {
            daysSpacing = (int)(totalDays / 200d);
        }

        print("totaldays : " + totalDays);
        print("daysSpacing " + daysSpacing);
        print("forward " + forward);

        bool continueLoop = true;
        while (continueLoop)
        {
            //Convert the date of the current day to a vector in space
            result.Add(ConvertDateToVector(current, astralBody) + GameManager.Instance.zero);

            //If going fowards, we advance the current time, else we go backwards
            if (forward) current = current.AddDays(daysSpacing);
            else current = current.AddDays(-daysSpacing);

            //If we have gone beyond the end date, stop the loop
            continueLoop = forward ? current < endDate : current > startDate;
        }

        return result;
    }
}

