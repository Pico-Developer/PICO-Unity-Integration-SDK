using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PXRSample_SpatialAnchor : MonoBehaviour
{
    [HideInInspector]
    public ulong anchorHandle;
    [SerializeField]
    private Text anchorID;
    [SerializeField]
    private GameObject savedIcon;
    [SerializeField]
    private GameObject uiCanvas;

    [SerializeField] private Button btnPersist;
    [SerializeField] private Button btnDestroyAnchor;
    [SerializeField] private Button btnDeleteAnchor;

    private void Awake()
    {
        //uiCanvas.SetActive(false);
        uiCanvas.SetActive(true);
        uiCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        btnPersist.onClick.AddListener(OnBtnPressedPersist);
        btnDestroyAnchor.onClick.AddListener(OnBtnPressedDestroy);
        btnDeleteAnchor.onClick.AddListener(OnBtnPressedUnPersist);
    }

    protected void OnEnable()
    {
    }

    protected void OnDisable()
    {

    }

    private void Start()
    {
        
    }


    private void Update()
    {
        if (uiCanvas.activeSelf)
        {
            uiCanvas.transform.LookAt(new Vector3(uiCanvas.transform.position.x * 2 - Camera.main.transform.position.x, uiCanvas.transform.position.y * 2 - Camera.main.transform.position.y, uiCanvas.transform.position.z * 2 - Camera.main.transform.position.z), Vector3.up);
        }
    }

    private void LateUpdate()
    {
        var result = PXR_MixedReality.LocateAnchor(anchorHandle, out var position, out var rotation);
        if (result == PxrResult.SUCCESS)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
        else
        {
            PXRSample_SpatialAnchorManager.Instance.SetLogInfo("LocateSpatialAnchor:" + result.ToString());
        }
    }

    private async void OnBtnPressedPersist()
    {
        var result = await PXR_MixedReality.PersistSpatialAnchorAsync(anchorHandle);
        PXRSample_SpatialAnchorManager.Instance.SetLogInfo("PersistSpatialAnchorAsync:" + result.ToString());
        if (result == PxrResult.SUCCESS)
        {
            ShowSaveIcon();
        }
    }

    private void OnBtnPressedDestroy()
    {
        PXRSample_SpatialAnchorManager.Instance.DestroySpatialAnchor(anchorHandle);
    }

    private async void OnBtnPressedUnPersist()
    {
        var result = await PXR_MixedReality.UnPersistSpatialAnchorAsync(anchorHandle);
        PXRSample_SpatialAnchorManager.Instance.SetLogInfo("UnPersistSpatialAnchorAsync:" + result.ToString());
        if (result == PxrResult.SUCCESS)
        {
            OnBtnPressedDestroy();
        }
    }

    public void SetAnchorHandle(ulong handle)
    {
        anchorHandle = handle;
        anchorID.text = "ID: " + anchorHandle;
    }

    public void ShowSaveIcon()
    {
        savedIcon.SetActive(true);
    }

}
