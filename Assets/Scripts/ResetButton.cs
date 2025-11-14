using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetButtonHandler : MonoBehaviour
{
    [Header("Settings")]
    public int requiredClicks = 5;
    public float timeWindow = 2f;

    private int clickCount = 0;
    private float firstClickTime = 0f;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        if (clickCount == 0)
            firstClickTime = Time.time;

        clickCount++;

        if (clickCount >= requiredClicks && (Time.time - firstClickTime) <= timeWindow)
        {
            GameManager.Instance.DestroySolarSystem();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if ((Time.time - firstClickTime) > timeWindow)
        {
            clickCount = 1;
            firstClickTime = Time.time;
        }
    }
}