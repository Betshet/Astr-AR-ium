using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using CosineKitty;
using UnityEngine;
using DG.Tweening;

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    public Body astralBody;

    public Vector3 StartingPosition;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void RegisterStartingPosition()
    {
        StartingPosition = transform.position - GameManager.Instance.zero;
    }


    public void MoveObjectIterate(List<Vector3> posList, float totalTime)
    {
        float timeForEachMove = (totalTime/posList.Count);
        print(posList.Count);
        transform.DOPath(posList.ToArray(), totalTime);
    }

    public IEnumerator MoveObjectTo(Vector3 end, float time) //in seconds
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, end + GameManager.Instance.zero, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = end + GameManager.Instance.zero;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void ResetPosition()
    {
        StartCoroutine(MoveObjectTo(StartingPosition, 2));
    }
}
