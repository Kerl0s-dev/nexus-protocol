using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class AutoVolume : MonoBehaviour
{
    Volume volume;
    DepthOfField dof;

    Ray Ray;
    RaycastHit hit;

    float maxFocusDistance = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume = GetComponent<Volume>();
        if (volume.profile.TryGet(out dof))
        {
            dof.active = true;
        }
        else
        {
            Debug.LogError("DepthOfField not found in Volume profile.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(Ray, out hit, maxFocusDistance))
        {
            float targetFocusDistance = hit.distance;
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, targetFocusDistance, Time.deltaTime * 5f);
        }
        else
        {
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, maxFocusDistance, Time.deltaTime * 5f);
        }
    }
}
