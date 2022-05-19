using System.Collections;
using UnityEngine;

/// Этот скрипт управляет движением автобусов и первого вагона поезда. Чтобы переместить транспортные средства предоставляется список
/// координат, который содержит координаты Unity. Транспортное средство в основном движется из точки в точку. 
/// Если транспортное средство достигает точки, где находится станция, оно останавливается на 1 секунду. Когда транспортное средство
/// достигает конца списка, он уничтожает себя.

public class VehicleMover : MonoBehaviour
{
    int targetIndex = 0; 
    int maxIndex;  

    bool isWaiting = false;

    float VehicleMaxSpeed = 100f; 

    void Start()
    {       
        transform.position = SortWay.PathsInRightOrder[0][0]; 

        maxIndex = SortWay.MoveToTarget.Count - 1; 
    }

    void Update()
    {
        if (IngameMenu.DarkModeOn)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        if (isWaiting)
        {
            return; 
        }

        // Транспортные средства перемещаются в следующую точку из списка "MoveToTarget".
        transform.position = Vector3.MoveTowards(transform.position, SortWay.MoveToTarget[targetIndex], Time.deltaTime * VehicleMaxSpeed);

        if (transform.position == SortWay.MoveToTarget[targetIndex])
        {
            if (TranSportWayMarker.StationOrder.Contains(transform.position))
            {
                StartCoroutine(Waiting());
            }
            if(targetIndex + 1 > maxIndex) 
            {
                Destroy(gameObject);
                return;
            }
            else if (SortWay.PathLastNode.Contains(transform.position))
            {
                int index = SortWay.MoveToTarget.IndexOf(transform.position);
                transform.position = SortWay.MoveToTarget[index + 1];
            }
            transform.LookAt(SortWay.MoveToTarget[targetIndex + 1]);

            targetIndex += 1;           
        }
    }

    /// <returns></returns>
    IEnumerator Waiting()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2);
        isWaiting = false;
    }
}
