using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PXRSample_SpatialAnchorManager : MonoBehaviour
{
    private static PXRSample_SpatialAnchorManager instance = null;
    public static PXRSample_SpatialAnchorManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PXRSample_SpatialAnchorManager>();
            }
            return instance;
        }
    }

    public GameObject anchorPrefab;
    private bool isCreateAnchorMode = false;
    public Dictionary<ulong, PXRSample_SpatialAnchor> anchorList = new Dictionary<ulong, PXRSample_SpatialAnchor>();
    public Dictionary<ulong, ulong> persistTaskList = new Dictionary<ulong, ulong>();
    public Dictionary<ulong, ulong> unPersistTaskList = new Dictionary<ulong, ulong>();
    private InputDevice rightController;
    public GameObject anchorPreview;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button btnCreateAnchor;
    [SerializeField] private Button btnLoadAnchors;


    private bool btnAClick = false;
    private bool aLock = false;
    private bool btnAState = false;
    private bool gripButton = false;


    public Text tipsText;
    private int maxLogCount = 5;
    private Queue<string> logQueue = new Queue<string>();
    void Start()
    {
        PXR_Manager.EnableVideoSeeThrough = true;

        StartSpatialAnchorProvider();

        btnCreateAnchor.onClick.AddListener(OnBtnPressedCreateAnchor);
        btnLoadAnchors.onClick.AddListener(OnBtnPressedLoadAllAnchors);

        btnCreateAnchor.gameObject.SetActive(true);
        btnLoadAnchors.gameObject.SetActive(true);

        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private async void StartSpatialAnchorProvider()
    {
        var result = await PXR_MixedReality.StartSenseDataProvider(PxrSenseDataProviderType.SpatialAnchor);
        SetLogInfo("StartSenseDataProvider:" + result);
    }

    void OnEnable()
    {
        PXR_Manager.SpatialAnchorDataUpdated += SpatialAnchorDataUpdated;
    }

    void OnDisable()
    {
        PXR_Manager.SpatialAnchorDataUpdated -= SpatialAnchorDataUpdated;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessKeyEvent();
        
        menuPanel.SetActive(gripButton);

        if (isCreateAnchorMode && btnAClick)
        {
            CreateSpatialAnchor(anchorPreview.transform);
        }
    }

    private void ProcessKeyEvent()
    {
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out btnAState);
        if (btnAState && !aLock)
        {
            btnAClick = true;
            aLock = true;
        }
        else
        {
            btnAClick = false;
        }
        if (!btnAState)
        {
            btnAClick = false;
            aLock = false;
        }

        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.gripButton, out gripButton);
    }

    private void SpatialAnchorDataUpdated()
    {
        SetLogInfo("SpatialAnchorDataUpdated:");
        OnBtnPressedLoadAllAnchors();
    }

    private void OnBtnPressedCreateAnchor()
    {
        isCreateAnchorMode = !isCreateAnchorMode;
        if (isCreateAnchorMode)
        {
            btnCreateAnchor.transform.Find("Text").GetComponent<Text>().text = "CancelCreate";
            anchorPreview.SetActive(true);
        }
        else
        {
            btnCreateAnchor.transform.Find("Text").GetComponent<Text>().text = "CreateAnchor";
            anchorPreview.SetActive(false);
        }
    }

    private async void OnBtnPressedLoadAllAnchors()
    {
        var result = await PXR_MixedReality.QuerySpatialAnchorAsync();
        SetLogInfo("LoadSpatialAnchorAsync:" + result.result.ToString());
        if (result.result == PxrResult.SUCCESS)
        {
            foreach (var key in result.anchorHandleList)
            {
                if (!anchorList.ContainsKey(key))
                {
                    GameObject anchorObject = Instantiate(anchorPrefab);
                    PXRSample_SpatialAnchor anchor = anchorObject.GetComponent<PXRSample_SpatialAnchor>();
                    anchor.SetAnchorHandle(key);

                    PXR_MixedReality.LocateAnchor(key, out var position, out var orientation);
                    anchor.transform.position = position;
                    anchor.transform.rotation = orientation;
                    anchorList.Add(key, anchor);
                    anchorList[key].ShowSaveIcon();
                }
            }
        }
    }

    private async void CreateSpatialAnchor(Transform transform)
    {
        var result = await PXR_MixedReality.CreateSpatialAnchorAsync(transform.position, transform.rotation);
        SetLogInfo("CreateSpatialAnchorAsync:" + result.ToString());
        if (result.result == PxrResult.SUCCESS)
        {
            GameObject anchorObject = Instantiate(anchorPrefab);
            PXRSample_SpatialAnchor anchor = anchorObject.GetComponent<PXRSample_SpatialAnchor>();
            if (anchor == null)
            {
                anchor = anchorObject.AddComponent<PXRSample_SpatialAnchor>();
            }
            anchor.SetAnchorHandle(result.anchorHandle);

            anchorList.Add(result.anchorHandle, anchor);

            var result1 = PXR_MixedReality.GetAnchorUuid(result.anchorHandle, out var uuid);
            SetLogInfo("GetUuid:" + result1.ToString() + "  " + (result.uuid.Equals(uuid)) + "Uuid:" + uuid);
        }
    }

    public void DestroySpatialAnchor(ulong anchorHandle)
    {
        var result = PXR_MixedReality.DestroyAnchor(anchorHandle);
        SetLogInfo("DestroySpatialAnchor:" + result.ToString());
        if (result == PxrResult.SUCCESS)
        {
            if (anchorList.ContainsKey(anchorHandle))
            {
                Destroy(anchorList[anchorHandle].gameObject);
                anchorList.Remove(anchorHandle);
            }
        }
    }

    public void SetLogInfo(string log)
    {
        if (logQueue.Count >= maxLogCount)
        {
            logQueue.Dequeue();
        }
        logQueue.Enqueue(log);
        
        tipsText.text = string.Join("\n", logQueue.ToArray());
    }
}
