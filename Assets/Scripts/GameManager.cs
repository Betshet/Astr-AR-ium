using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using CosineKitty;
using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UIElements;
using UnityEngine.XR.ARSubsystems;
using System.Security.Principal;

public class GameManager : MonoBehaviour
{
    List<MovingObject> Planets;

    [SerializeField]
    GameObject SolarSystemPrefab;

    [SerializeField]
    DateManager dateManager;

    [SerializeField]
    public GameObject DateCanvas;

    [SerializeField]
    ARTrackedImageManager trackedImageManager;

    public ARAnchorManager anchorManager;

    [HideInInspector]
    public Vector3 zero;

    [SerializeField]
    public Vector3 zeroOffset;

    public bool PlanetsDeployed = false;
    bool PlanetsSpawned = false;
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
            foreach (MovingObject planet in list)
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
                planet.MoveObjectIterate(positions, 2);
                //TODO : time in seconds changes depending on number of items in positions array
            }
        }
        else
        {
            GetComponent<MiniGearCountdown>().ResetFicelle();
            foreach (MovingObject planet in Planets)
            {
                planet.RegisterStartingPosition();
                Vector3 position = dateManager.GetVectorFromUserDate(TargetDateString, planet.astralBody);
                StartCoroutine(planet.MoveObjectTo(position,2));
                //TODO : parabole
            }
            PlanetsDeployed = true;
        }
        currentDate = TargetDate;
    }

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;

    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;

    void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if(!PlanetsSpawned)
            {
                Pose pose = new(updatedImage.transform.position, Quaternion.identity);
                var instance = Instantiate(SolarSystemPrefab, updatedImage.transform.position, Quaternion.identity);
                PlanetsSpawned = true;
                DateCanvas.SetActive(true);
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
        }
    }

    public void ResetPosition()
    {
        foreach(MovingObject planet in Planets)
        {
            planet.ResetPosition();
        }
    }
}
