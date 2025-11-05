using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> vector3s = new List<Vector3>();
        vector3s.Add(new Vector3(0, 0, 10));
        vector3s.Add(new Vector3(0, 10, 20));
        vector3s.Add(new Vector3(10, 10, 10));
        vector3s.Add(new Vector3(0, 0, 0));
        StartCoroutine(MoveObjectIterate(this.gameObject, vector3s, 15));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator MoveObjectIterate(GameObject objectToMove, List<Vector3> posList, float totalTime)
    {
        float timeForEachMove = totalTime/posList.Count;

        foreach (var pos in posList)
        {
            StartCoroutine(MoveObjectTo(objectToMove, pos, timeForEachMove));
            yield return new WaitForSeconds(timeForEachMove);
        }
    }

    IEnumerator MoveObjectTo(GameObject objectToMove, Vector3 end, float time) //in seconds
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < time)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }
}
