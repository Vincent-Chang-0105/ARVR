using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class AnchorPlacer : MonoBehaviour
{
    private ARAnchorManager arAnchorManager;

    [SerializeField] private GameObject[] objectPrefabs;
    [SerializeField] private float placementDistance = 2f;
    [SerializeField] private Slider distanceControlSlider;
    [SerializeField] private GameObject pointCloudDisplay;
    [SerializeField] private Toggle pointCloudVisibilityToggle;
    [SerializeField] private Button removeAllAnchorsButton;
    [SerializeField] private Button[] selectPrefabButtons;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text slidertext;

    private List<GameObject> activeAnchors = new List<GameObject>();
    private GameObject currentPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        arAnchorManager = GetComponent<ARAnchorManager>();
        currentPrefab = objectPrefabs[0];

        SetupUIListeners();
    }

    private void SetupUIListeners()
    {
        pointCloudVisibilityToggle.onValueChanged.AddListener(TogglePointCloudDisplay);
        distanceControlSlider.onValueChanged.AddListener(UpdatePlacementDistance);

        for (int buttonIndex = 0; buttonIndex < selectPrefabButtons.Length; buttonIndex++)
        {
            int index = buttonIndex;
            selectPrefabButtons[buttonIndex].onClick.AddListener(() => SelectPrefabByIndex(index));
        }

        removeAllAnchorsButton.onClick.AddListener(ClearAllAnchors);
    }

    void Update()
    {

        slidertext.text = distanceControlSlider.value.ToString("F1");

        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            HandleTouchPlacement(touch.position);
        }
    }

    private void HandleTouchPlacement(Vector2 screenPosition)
    {
        Ray rayFromCamera = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(rayFromCamera, out RaycastHit hitInfo))
        {
            // If an anchored object is hit, remove it.
            if (hitInfo.collider.CompareTag("AnchoredObject"))
            {
                RemoveAnchor(hitInfo.collider.gameObject);
                return;
            }
        }

        Vector3 placementPosition = rayFromCamera.GetPoint(placementDistance);
        CreateAnchorAtPosition(placementPosition);
    }

    private void CreateAnchorAtPosition(Vector3 position)
    {
        GameObject anchorGameObject = new GameObject("AnchorObject");
        anchorGameObject.transform.position = position;
        ARAnchor anchor = anchorGameObject.AddComponent<ARAnchor>();

        GameObject instantiatedObject = Instantiate(currentPrefab, anchor.transform);
        instantiatedObject.transform.localPosition = Vector3.zero;
        instantiatedObject.tag = "AnchoredObject";

        activeAnchors.Add(anchorGameObject);
    }

    private void RemoveAnchor(GameObject anchorObject)
    {
        activeAnchors.Remove(anchorObject);
        Destroy(anchorObject);
    }

    private void ClearAllAnchors()
    {
        foreach (GameObject anchor in activeAnchors)
        {
            Destroy(anchor);
        }
        activeAnchors.Clear();
    }

    private void UpdatePlacementDistance(float newDistance)
    {
        placementDistance = newDistance;
    }

    private void TogglePointCloudDisplay(bool showPointCloud)
    {
        pointCloudDisplay.SetActive(showPointCloud);
    }

    private void SelectPrefabByIndex(int prefabIndex)
    {
        currentPrefab = objectPrefabs[prefabIndex];

        for (int i = 0; i < selectPrefabButtons.Length; i++)
        {
            selectPrefabButtons[i].interactable = i != prefabIndex;
        }
    }

    public void activateSettings(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}