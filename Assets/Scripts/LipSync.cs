using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

// code copied from the OVRLipSyncContextMorphTarget.cs, with unwanted stuff removed
public class LipSync : MonoBehaviour
{
    // Manually assign the skinned mesh renderer to this script
    [Tooltip("Skinned Mesh Rendered target to be driven by Oculus Lipsync")]
    public SkinnedMeshRenderer skinnedMeshRenderer = null;

    // Set the blendshape index to go to (-1 means there is not one assigned)
    [Tooltip("Blendshape index to trigger for each viseme.")]
    public int[] visemeToBlendTargets = Enumerable.Range(0, OVRLipSync.VisemeCount).ToArray();   

    // smoothing amount
    [Range(1, 100)]
    [Tooltip("Smoothing of 1 will yield only the current predicted viseme, 100 will yield an extremely smooth viseme response.")]
    public int smoothAmount = 70;

    // Look for a lip-sync Context (should be set at the same level as this component)
    private OVRLipSyncContextBase lipsyncContext = null;
        
    void Start()
    {
        string errorMessage = "LipSyncContextMorphTarget.Start Error: Please set the target Skinned Mesh Renderer to be controlled!";

        // morph target needs to be set manually;
        Assert.IsNotNull(skinnedMeshRenderer, errorMessage);

        // make sure there is a phoneme context assigned to this object
        errorMessage = "LipSyncContextMorphTarget.Start Error: No OVRLipSyncContext component on this object!";
        lipsyncContext = GetComponent<OVRLipSyncContextBase>();
        Assert.IsNotNull(lipsyncContext, errorMessage);

        // Send smoothing amount to context
        lipsyncContext.Smoothing = smoothAmount;
    }
   
    void Update()
    {
        if ((lipsyncContext != null) && (skinnedMeshRenderer != null))
        {
            // get the current viseme frame
            OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
            if (frame != null)
            {
                SetVisemeToMorphTarget(frame);
            }

            // Update smoothing value
            if (smoothAmount != lipsyncContext.Smoothing)
            {
                lipsyncContext.Smoothing = smoothAmount;
            }
        }
    }

    /// <summary>
    /// Sets the viseme to morph target.
    /// </summary>
    void SetVisemeToMorphTarget(OVRLipSync.Frame frame)
    {
        for (int i = 0; i < visemeToBlendTargets.Length; i++)
        {
            if (visemeToBlendTargets[i] != -1)
            {
                // Viseme blend weights are in range of 0->1.0
                skinnedMeshRenderer.SetBlendShapeWeight(visemeToBlendTargets[i], frame.Visemes[i]);
            }
        }
    }
}
