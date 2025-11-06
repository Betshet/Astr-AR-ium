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


    public IEnumerator MoveObjectIterate(List<Vector3> posList, float totalTime)
    {
        float timeForEachMove = (totalTime/posList.Count) * 100 * Time.deltaTime;
        print(timeForEachMove);

        foreach (var pos in posList)
        {
            SetPosition(pos + GameManager.Instance.zero);
            yield return new WaitForSeconds(timeForEachMove);
        }
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
}
