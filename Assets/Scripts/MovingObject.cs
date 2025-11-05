using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using CosineKitty;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    public Body astralBody;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator MoveObjectIterate(List<Vector3> posList, float totalTime)
    {
        float timeForEachMove = totalTime/posList.Count;
        print("timeforeachmove : "+ timeForEachMove);

        foreach (var pos in posList)
        {
            StartCoroutine(MoveObjectTo(pos, timeForEachMove));
            yield return new WaitForSeconds(timeForEachMove);
        }
    }

    public IEnumerator MoveObjectTo(Vector3 end, float time) //in seconds
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = end;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
