using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using CosineKitty;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    List<MovingObject> Planets;

    [SerializeField]
    DateManager dateManager;

    public Vector3 zero;

    [SerializeField]
    public Vector3 zeroOffset;

    public bool PlanetsDeployed = false;
    public DateTime currentDate = DateTime.Now;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
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
        Planets = new List<MovingObject>();
        zero = new Vector3(0,0,0);
    }

    public void MoveAllPlanetsToDate(string TargetDateString)
    {
        if (Planets.Count == 0)
        {
            UnityEngine.Object[] list = GameObject.FindObjectsOfType(typeof(MovingObject));
            foreach (var planet in list)
            {
                Planets.Add((MovingObject)planet);
                if (((MovingObject)planet).astralBody == Body.Earth)
                {
                    zero = ((MovingObject)planet).transform.position + zeroOffset;
                }
            }
        }

        DateTime TargetDate = System.DateTime.Parse(TargetDateString);
        if (PlanetsDeployed)
        {
            foreach (MovingObject planet in Planets)
            {
                List<Vector3> positions = dateManager.GetVectorsBetweenDates(currentDate, TargetDate, planet.astralBody);
                StartCoroutine(planet.MoveObjectIterate(positions, 2));
                //TODO : time in seconds changes depending on number of items in positions array
            }
        }
        else
        {
            foreach (MovingObject planet in Planets)
            {
                Vector3 position = dateManager.GetVectorFromUserDate(TargetDateString, planet.astralBody);
                StartCoroutine(planet.MoveObjectTo(position,2));
                //TODO : parabole
            }
            PlanetsDeployed = true;
        }
        currentDate = TargetDate;
    }
}
