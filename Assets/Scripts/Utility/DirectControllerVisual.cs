using UnityEngine;

public class DirectControllerVisual : MonoBehaviour
{
    public bool isOpen = true;

    private Renderer refRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen)
        {
            SetMaterialContactFloat(1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isOpen)
        {
            SetMaterialContactFloat(0f);
        }
    }

    public void SetMaterialContactFloat(float _f)
    {
        refRenderer.material.SetFloat("Vector1_1281dd58be3a41ea8c323a4d26769aa1", _f);
    }

    private void Start()
    {
        refRenderer = GetComponent<Renderer>();
    }
}
