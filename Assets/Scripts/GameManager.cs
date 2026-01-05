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
    public GameObject MiniGearIcon;

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

    public bool PlanetsMoving = false;

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

    void PlanetsFinishedMoving()
    {
        SoundManager.Instance.Play("planets_stop");
        SoundManager.Instance.Stop("gears_rotate_planets");
        PlanetsMoving = false;
        DateCanvas.SetActive(true);

        GetComponent<UIManager>().DisplayAstrology(currentDate);
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
        SoundManager.Instance.Play("gears_rotate_planets");
        SoundManager.Instance.Play("planets_move");

        PlanetsMoving = true;
        DateCanvas.SetActive(false);

        //Hide UIs
        DateTime TargetDate = System.DateTime.Parse(TargetDateString);
        //GetComponent<UIManager>().HideAstrology();


        if (PlanetsDeployed)
        {
            foreach (MovingObject planet in Planets)
            {
                List<Vector3> positions = dateManager.GetVectorsBetweenDates(currentDate, TargetDate, planet.astralBody);
                planet.MoveObjectIterate(positions, 10);
            }
            Invoke("PlanetsFinishedMoving", 10);
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
            Invoke("PlanetsFinishedMoving", 2);
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
                Vector3 offset = new Vector3(-.3f, -.3f, -.3f);

                var instance = Instantiate(SolarSystemPrefab, updatedImage.transform.position + offset, Quaternion.identity);
                PlanetsSpawned = true;
                DateCanvas.SetActive(true);
                MiniGearIcon.SetActive(true);
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
        }
    }

    public void ResetPosition()
    {
        SoundManager.Instance.Play("planets_reset");
        SoundManager.Instance.Play("system_break");
        foreach(MovingObject planet in Planets)
        {
            planet.ResetPosition();
        }
    }

    public void SetupRepairMechanism()
    {
        print("setup repair");
        GetComponent<UIManager>().HideAstrology();
        DateCanvas.SetActive(false);
        MiniGearIcon.SetActive(false);
        ResetPosition();
    }

    public void ResetAfterRepair()
    {
        print("reset after repair");
        MiniGearIcon.SetActive(true);
        GetComponent<MiniGearCountdown>().ResetFicelle();
        DateCanvas.SetActive(true);
        PlanetsDeployed = false;
        MoveAllPlanetsToDate(currentDate.ToString());
    }
}
