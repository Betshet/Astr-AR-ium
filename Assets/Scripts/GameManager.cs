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
    ARTrackedImageManager trackedImageManager;

    public ARAnchorManager anchorManager;

    public List<ImagePrefabPair> imagePrefabPairs = new List<ImagePrefabPair>();

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private HashSet<string> spawnedMarkers = new HashSet<string>();


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

        // Crée le dictionnaire à partir de la liste assignée dans l’inspector
        foreach (var pair in imagePrefabPairs)
        {
            if (!prefabDictionary.ContainsKey(pair.imageName))
                prefabDictionary.Add(pair.imageName, pair.prefab);
        }
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

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;

    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;

    void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            TrySpawnPrefab(trackedImage);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if(!PlanetsSpawned)
            {
                Pose pose = new(updatedImage.transform.position, Quaternion.identity);
                ARAnchor anchor = anchorManager.AddAnchor(pose);
                var instance = Instantiate(SolarSystemPrefab, updatedImage.transform.position, Quaternion.identity);
                instance.transform.parent = anchor.transform;
                PlanetsSpawned = true;
            }

            //Si c’est un autre marqueur, on tente de spawner le prefab associé
            TrySpawnPrefab(updatedImage);
        }

        foreach (var removedImage in eventArgs.removed)
        {
        }
    }

    private void TrySpawnPrefab(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        // Empêche de réapparaître plusieurs fois
        if (spawnedMarkers.Contains(imageName)) return;
        if (trackedImage.trackingState != TrackingState.Tracking) return;

        // Vérifie si le marqueur correspond à un prefab dans la liste
        if (prefabDictionary.TryGetValue(imageName, out GameObject prefab))
        {
            Pose pose = new Pose(trackedImage.transform.position, trackedImage.transform.rotation);
            ARAnchor anchor = anchorManager.AddAnchor(pose);

            if (anchor != null)
            {
                Instantiate(prefab, pose.position, pose.rotation, anchor.transform);
                spawnedMarkers.Add(imageName);
                Debug.Log($"Prefab '{prefab.name}' instancié pour le marqueur '{imageName}'");
            }
        }
    }
}

[Serializable]
public class ImagePrefabPair
{
    public string imageName;
    public GameObject prefab;
}