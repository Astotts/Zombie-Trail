using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] Transform[] bounds;
    Vector3 pos;
    [SerializeField] Transform player;

    float vertical, horizontal;
    Vector3 lerpPosition;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0f)) - player.position;
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero) - player.position;

        Debug.DrawRay(player.position, topRight, Color.green, 5f);
        Debug.DrawRay(player.position, bottomLeft, Color.green, 5f);
        
        
        horizontal = Vector3.Distance(new Vector3(topRight.x,0f,0f),new Vector3(bottomLeft.x,0f,0f)) / 2;
        vertical = Vector3.Distance(new Vector3(0f,topRight.y,0f),new Vector3(0f,bottomLeft.y,0f)) / 2;

        //Debug.Log(horizontal);
        //Debug.Log(vertical);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        pos = player.position;

        pos.x += Input.GetAxisRaw("Horizontal"); 
        pos.y += Input.GetAxisRaw("Vertical"); 

        pos.x = Mathf.Clamp(pos.x, bounds[1].position.x + horizontal, bounds[0].position.x - horizontal);
        pos.y = Mathf.Clamp(pos.y, bounds[1].position.y + vertical, bounds[0].position.y - vertical);    
        pos.z = Mathf.Clamp(pos.z, -10f, -10f);
        //mouseWheelLerpIncrement = Mathf.Lerp(mouseWheelLerpIncrement, Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, 10f * Time.deltaTime);
        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - mouseWheelLerpIncrement, zoomOutMin, zoomOutMax);
        lerpPosition = Vector3.Lerp(lerpPosition, pos, Time.deltaTime);
        transform.position = lerpPosition;
    }
}
