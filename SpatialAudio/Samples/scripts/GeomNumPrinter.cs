using UnityEngine;

public class GeomNumPrinter : MonoBehaviour
{
    public PXR_Audio_Spatializer_Context spatialAudioContext = null;

    // Update is called once per frame
    void Update()
    {
        if (spatialAudioContext)
        {
            Debug.Log("GetNumOfGeometries() == " + spatialAudioContext.GetNumOfGeometries());
        }
    }
}
