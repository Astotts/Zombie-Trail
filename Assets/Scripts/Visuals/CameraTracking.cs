using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.U2D;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] Transform[] bounds;
    Vector3 pos;
    [SerializeField] Transform player = null;
    [SerializeField] float camSnapFloat;

    float vertical, horizontal;
    Vector3 lerpPosition;

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0f)) - player.position;
        Vector3 bottomLeft = camera.ScreenToWorldPoint(Vector3.zero) - player.position;

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
        if (player == null) return;
        pos = player.position;

        pos.x += Input.GetAxisRaw("Horizontal") * camSnapFloat; 
        pos.y += Input.GetAxisRaw("Vertical") * camSnapFloat; 

        pos.x = Mathf.Clamp(pos.x, bounds[1].position.x + horizontal, bounds[0].position.x - horizontal);
        pos.y = Mathf.Clamp(pos.y, bounds[1].position.y + vertical, bounds[0].position.y - vertical);    
        pos.z = Mathf.Clamp(pos.z, -10f, -10f);
        //mouseWheelLerpIncrement = Mathf.Lerp(mouseWheelLerpIncrement, Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, 10f * Time.deltaTime);
        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - mouseWheelLerpIncrement, zoomOutMin, zoomOutMax);
        lerpPosition = Vector3.Lerp(transform.position, pos, Time.deltaTime * camSnapFloat);
        transform.position = lerpPosition;
    }
}
