using UnityEngine;
using UnityEngine.UI;

/// ???? ????? ????????? ????????? ??????. ???????????? ????? ????????? ???????????? ????????? ? ???????
/// ???? ? ??????????? ? ??????? ?????? WASD. ???????????? ????? ????? ?????? ?? ??????? ? ???????????? ??.

public class CameraController : MonoBehaviour
{
	public GameObject crossHair;
    public GameObject FlightModeInformation;
    public GameObject MouseModeInformation;
    public GameObject MouseClickDisabler;
    public GameObject InputFieldSearchBar;
    public GameObject SearchDropDown;
	
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    float mainSpeed = 100.0f; 
    float shiftAdd = 250.0f; 
    float maxShift = 1000.0f; 
    
	private float totalRun = 1.0f;

	Camera cam;

    bool stopmoving = true; 

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            stopmoving = !stopmoving;
			
            if (stopmoving)
            {
                InputFieldSearchBar.gameObject.SetActive(true);
                if (!IngameMenu.MenuHidden)
                {
                    InputFieldSearchBar.transform.GetComponentInParent<InputField>().text = "";
                }
                Cursor.visible = true;
                crossHair.SetActive(false);
                FlightModeInformation.SetActive(false);
                MouseModeInformation.SetActive(true);
                MouseClickDisabler.SetActive(false);
            }

            if (!stopmoving)
            {
                InputFieldSearchBar.gameObject.SetActive(false);
                SearchDropDown.gameObject.SetActive(false);
                Cursor.visible = false;
                crossHair.SetActive(true);
                FlightModeInformation.SetActive(true);
                MouseModeInformation.SetActive(false);
                MouseClickDisabler.SetActive(true);
            }
        }

        if(stopmoving == false)
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

            Vector3 p = GetBaseInput();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(p);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else if (Input.GetKey(KeyCode.X))
            {
                transform.Translate(p);
                newPosition.y = transform.position.y;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(p);
            }
        }

        if(stopmoving == true)
        {         
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var canvas = hit.transform.GetChild(0).gameObject;
                    canvas.SetActive(true);
                    canvas.transform.GetChild(6).gameObject.SetActive(true);
                }
            }        
        }
    }


    /// <returns></returns>
    private Vector3 GetBaseInput()
    {
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}