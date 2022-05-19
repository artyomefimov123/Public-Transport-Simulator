using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Этот скрипт работает аналогично скрипту "VehicleMover". Он предназначен только для железнодорожных вагонов, которые не являются
/// ведущим вагоном. Каждый вагон следует за вагоном впереди него.
public class WagonMover : MonoBehaviour
{
    int targetIndex = 0; 

    bool isWaiting = false;

    float VehicleMaxSpeed = 100f; 

    GameObject WagonToFollow; 

    Vector3 lastWagonPos = Vector3.zero;
    Vector3 curWagonPos = new Vector3(0, 0, 1);

    float velocity = 1f; 

    void Start()
    {
        transform.position = SortWay.PathsInRightOrder[0][0]; 

        List<GameObject> allGameObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(allGameObjects);

        WagonToFollow = allGameObjects[allGameObjects.Count - 2];
    }

    void Update()
    {   
        if(WagonToFollow == null)
        {
            gameObject.Destroy(); 
            return;
        }

        float distance = Vector3.Distance(WagonToFollow.transform.GetChild(0).GetChild(1).position, transform.GetChild(0).GetChild(0).position);
        if(Vector3.Distance(WagonToFollow.transform.position, transform.position) < 30)
        {
            velocity = .1f;
        }
        else if(distance > 20)
        {
            velocity = 1.2f;
        }
        else if(distance < 10)
        {

            velocity = .8f;
        }
        else
        {
            velocity = 1f;
        }

        // Здесь мы измеряем, остановилась ли следующая повозка. Если это так, то следующий вагон также остановится
        // и двигайтесь, как только следующий фургон снова начнет двигаться.
        curWagonPos = WagonToFollow.transform.position;
        if( curWagonPos == lastWagonPos)
        {
            isWaiting = true;
        }
        else
        {
            isWaiting = false;
        }
        lastWagonPos = curWagonPos;
        if (isWaiting)
        {
            return; 
        }

        // Транспортное средство перемещается в следующую точку списка "MoveToTarget" .
        transform.position = Vector3.MoveTowards(transform.position, SortWay.MoveToTarget[targetIndex], Time.deltaTime * VehicleMaxSpeed * velocity);
        if (transform.position == SortWay.MoveToTarget[targetIndex])
        {
            if (SortWay.PathLastNode.Contains(transform.position))
            {
                // Если дорога/железная дорога состоит из более чем одной части, мы должны телепортировать транспортное средство в другую часть по достижении
                // конца одной части.
                int index = SortWay.MoveToTarget.IndexOf(transform.position);
                transform.position = SortWay.MoveToTarget[index + 1];
            }
            transform.LookAt(SortWay.MoveToTarget[targetIndex + 1]); 

            targetIndex += 1;
        }
    }
}
