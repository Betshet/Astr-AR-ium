using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    List<MovingObject> Planets;

    [SerializeField]
    DateManager dateManager;

    // Start is called before the first frame update
    void Start()
    {
        foreach (MovingObject planet in Planets)
        {
            List<Vector3> positions = dateManager.GetVectorsBetweenDates("01/01/2025", "30/12/2025", planet.astralBody);
            StartCoroutine(planet.MoveObjectIterate(positions, 30));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
