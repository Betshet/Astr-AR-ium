using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SingleMarkerDetection : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject objectToShow;

    private bool hasSpawned = false;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        if (hasSpawned) return; // On ignore les redétections

        foreach (var trackedImage in args.added)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                objectToShow.SetActive(true);
                objectToShow.transform.position = trackedImage.transform.position;
                objectToShow.transform.rotation = trackedImage.transform.rotation;

                hasSpawned = true;

                // Désactive la détection pour éviter toute redétection
                trackedImageManager.enabled = false;

                break;
            }
        }
    }
}
