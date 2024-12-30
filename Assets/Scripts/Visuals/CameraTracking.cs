using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] float camSnapFloat;
    [SerializeField] Vector3 offset;
    [SerializeField] WorldGenerator worldGenerator;
    [SerializeField] int findAttempts = 20;

    Transform player;

    float minY;
    float maxY;

    Vector3 lerpPosition;

    void Start()
    {
        StartCoroutine(SearchForPlayer());
        this.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>().renderPostProcessing = true;
        Camera camera = Camera.main;
        float worldGeneratorYPos = worldGenerator.chunkSize / 2;
        float worldGeneratorVerticalExtend = worldGenerator.maxWidth;

        maxY = worldGeneratorYPos + worldGeneratorVerticalExtend + camera.orthographicSize / 2;
        minY = -worldGeneratorYPos - worldGeneratorVerticalExtend - camera.orthographicSize / 2;
    }

    IEnumerator SearchForPlayer()
    {
        yield return new WaitForSeconds(1.0f);
        NetworkObject localClientObj = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localClientObj != null)
            player = localClientObj.transform;
        findAttempts--;

        if (player == null && findAttempts > 0)
            StartCoroutine(SearchForPlayer());
    }

    float BetweenMinMax(float y)
    {
        return Mathf.Max(minY, MathF.Min(y, maxY));
    }

    // Update is called once per frame
    void LateUpdate()
    {
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
