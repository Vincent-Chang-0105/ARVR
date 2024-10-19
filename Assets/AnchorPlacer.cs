using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class AnchorPlacer : MonoBehaviour
{
    ARAnchorManager anchorManager;
    ARPointCloudManager pointCloudManager;
    bool isPointCloudActive = false;

    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private GameObject capsulePrefab;
    [SerializeField] private float forwardOffset;

    [SerializeField] private Slider slider;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private TextMeshProUGUI sliderValueText;
    [SerializeField] private Toggle pointCloudToggle;

    [SerializeField] private Button cubeButton;
    [SerializeField] private Button sphereButton;
    [SerializeField] private Button capsuleButton;

    private float spawnOffSet;

    private int activePrefab;

    void Start()
    {
        anchorManager = GetComponent<ARAnchorManager>();
        pointCloudManager = FindAnyObjectByType<ARPointCloudManager>();
        pointCloudToggle.onValueChanged.AddListener(OnTogglePointCloud);

        activePrefab = 3;
    }

    void Update()
    {
        spawnOffSet = slider.value + forwardOffset;
        sliderValueText.text = slider.value.ToString();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ARAnchor anchor = hit.collider.GetComponentInParent<ARAnchor>();

                if (anchor != null)
                {
                    Destroy(anchor.gameObject);
                    return;
                }
            }

            Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * spawnOffSet;
            if (activePrefab != 3)
            {
                AnchorObject(spawnPos);
            }
        }
    }

    private bool IsTouchOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.position.x, touch.position.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }


    public void AnchorObject(Vector3 worldPos)
    {
        GameObject newAnchor = new GameObject("NewAnchor");
        newAnchor.transform.parent = null;
        newAnchor.transform.position = worldPos;
        newAnchor.AddComponent<ARAnchor>();

        if (activePrefab == 0)
        {
            GameObject obj = Instantiate(cubePrefab, newAnchor.transform);
            obj.transform.localPosition = Vector3.zero;
            activePrefab = 3;
        }
        else if (activePrefab == 1)
        {
            GameObject obj = Instantiate(spherePrefab, newAnchor.transform);
            obj.transform.localPosition = Vector3.zero;
            activePrefab = 3;
        }
        else if (activePrefab == 2)
        {
            GameObject obj = Instantiate(capsulePrefab, newAnchor.transform);
            obj.transform.localPosition = Vector3.zero;
            activePrefab = 3;
        }

    }

    public void OnTogglePointCloud(bool pointActive)
    {
        pointCloudManager.SetTrackablesActive(pointActive);
    }

    public void OnMazdaButtonPressed()
    {
        activePrefab = 0;
    }

    public void OnCubeButtonPressed()
    {
        activePrefab = 1;
    }

    public void OnPortalButtonPressed()
    {
        activePrefab = 2;
    }

    public void OnDestroyButtonPressed()
    {
        foreach (ARAnchor anchor in anchorManager.trackables)
        {
            Destroy(anchor.gameObject);
        }
    }

    public void OnOptionsButtonPressed()
    {
        if (optionsPanel.activeInHierarchy == false)
        {eqeqeqeqeqeqeqeqeqeq
            optionsPanel.SetActive(true);
        }
        else
        {
            optionsPanel.SetActive(false);
        }
    }

}