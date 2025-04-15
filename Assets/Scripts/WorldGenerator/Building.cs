
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private float hideTime;
    [SerializeField] private float showTime;
    [SerializeField] private SpriteRenderer buildingSprite;
    [SerializeField] private BoxCollider2D buildingCollider;
    [SerializeField] private Transform shadowTransform;

    private BuildingGenerator buildingGenerator;
    private List<SpawnedBuilding> dataList;
    private SpawnedBuilding buildingData;

    private bool isHiding;
    private bool isShowing;

    private Coroutine hideCoroutine;
    private Coroutine showCoroutine;

    public void RemoveSelfFromData()
    {
        dataList.Remove(buildingData);
        buildingGenerator.ReturnBuilding(this);
    }

    void OnValidate()
    {
        buildingSprite = GetComponent<SpriteRenderer>();
        buildingCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player"))
            return;
        
        if (isShowing)
            StopCoroutine(showCoroutine);
        hideCoroutine = StartCoroutine(HideBuilding());
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player"))
            return;
        
        if (isHiding)
            StopCoroutine(hideCoroutine);
        showCoroutine = StartCoroutine(ShowBuilding());
    }

    IEnumerator HideBuilding()
    {
        isHiding = true;
        float elapsed = 0;
        Color currentColor = buildingSprite.color;
        float currAlpha = currentColor.a;
        while (elapsed <= hideTime && isHiding)
        {
            elapsed += Time.deltaTime;
            buildingSprite.color = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currAlpha, 0, elapsed / hideTime));
            yield return null;
        }
    }

    IEnumerator ShowBuilding()
    {
        isShowing = true;
        float elapsed = 0;
        Color currentColor = buildingSprite.color;
        float currAlpha = currentColor.a;
        while (elapsed <= showTime && isShowing)
        {
            elapsed += Time.deltaTime;
            buildingSprite.color = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currAlpha, 1, elapsed / showTime));
            yield return null;
        }
    }

    public void SetBuildingData(BuildingSO buildingSO)
    {
        buildingSprite.sprite = buildingSO.BuildingSprite;
        Vector2 spriteSize = buildingSprite.sprite.bounds.size;
        buildingCollider.size = spriteSize;
        // Assume every sprite building is pivot at middle bottom
        buildingCollider.offset = new(0, spriteSize.y / 2);

        shadowTransform.localScale = buildingSO.ColliderSize;
        shadowTransform.localPosition = new(0, buildingSO.ColliderSize.y / 2, 0);
    }

    public void SetBuildingList(List<SpawnedBuilding> buildingDataList, SpawnedBuilding buildingData)
    {
        this.dataList = buildingDataList;
        this.buildingData = buildingData;
    }

    public void SetBuildingGenerator(BuildingGenerator buildingGenerator)
    {
        this.buildingGenerator = buildingGenerator;
    }
}