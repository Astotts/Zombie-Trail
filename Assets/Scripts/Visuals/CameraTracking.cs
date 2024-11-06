using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.U2D;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] float camSnapFloat;
    [SerializeField] Transform player = null;
    [SerializeField] Vector3 offset;
    [SerializeField] WorldGenerator worldGenerator;
    [SerializeField] int findAttempts = 20;

    float minY;
    float maxY;

    Vector3 lerpPosition;

    void Start()
    {
        this.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>().renderPostProcessing = true;
        StartCoroutine(FindPlayer());
        Camera camera = Camera.main;
        float worldGeneratorYPos = worldGenerator.chunkSize / 2;
        float worldGeneratorVerticalExtend = worldGenerator.maxWidth;

        maxY = worldGeneratorYPos + worldGeneratorVerticalExtend + camera.orthographicSize / 2;
        minY = -worldGeneratorYPos - worldGeneratorVerticalExtend - camera.orthographicSize / 2;
    }

    IEnumerator FindPlayer()
    {
        bool found = false;
        for (int i = 0; i < findAttempts && !found; i++)
        {
            Debug.Log("Finding Owner, Atempt #" + i);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go.GetComponent<NetworkBehaviour>().IsLocalPlayer)
                {
                    player = go.transform;
                    found = true;
                    Debug.Log("Is local");
                    break;
                }
                Debug.Log("Is it?");
            }
            yield return new WaitForSecondsRealtime(1.0f);
        }
        if (!found)
            gameObject.SetActive(false);
    }

    float BetweenMinMax(float y)
    {
        return Mathf.Max(minY, MathF.Min(y, maxY));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
            return;

        // pos.x += Input.GetAxisRaw("Horizontal") * camSnapFloat;
        // pos.y += Input.GetAxisRaw("Vertical") * camSnapFloat;

        // pos.x = Mathf.Clamp(pos.x, bounds[1].position.x + horizontal, bounds[0].position.x - horizontal);
        // pos.y = Mathf.Clamp(pos.y, bounds[1].position.y + vertical, bounds[0].position.y - vertical);
        // pos.z = Mathf.Clamp(pos.z, -10f, -10f);
        //mouseWheelLerpIncrement = Mathf.Lerp(mouseWheelLerpIncrement, Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, 10f * Time.deltaTime);
        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - mouseWheelLerpIncrement, zoomOutMin, zoomOutMax);
        if (player == null)
            return;
        lerpPosition = Vector3.Lerp(transform.position, player.position + offset, Time.deltaTime * camSnapFloat);
        lerpPosition.y = BetweenMinMax(lerpPosition.y);
        transform.position = lerpPosition;
    }
}
