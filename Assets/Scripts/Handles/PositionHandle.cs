using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PositionHandle : MonoBehaviour
{
    [Range(1,5)] public float sensibility;

    [Serializable] public enum XRControllerID { Left, Right }
    XRControllerID controllerID;
    private bool isGrabedByHand = false;
    
    private Vector3 controllerStartPosition = Vector3.zero;
    private Vector3 controllerPosition = Vector3.zero;

    private DirectControllerVisual directControllerVisual;

    Vector3 GetControllerPosition()
    {
        switch (controllerID)
        {
            case XRControllerID.Left:
                return XRController.leftHand.devicePosition.ReadValue();

            case XRControllerID.Right:
                return XRController.rightHand.devicePosition.ReadValue();
        }
        return Vector3.zero;
    }

    public void HandleDeselected()
    {
        isGrabedByHand = false;
        directControllerVisual.isOpen = true;
        directControllerVisual.SetMaterialContactFloat(0f);
    }

    public void HandleSelected()
    {
        if (!isGrabedByHand)
        {
            isGrabedByHand = true;
            controllerStartPosition = GetControllerPosition();
            directControllerVisual.isOpen = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isGrabedByHand)
        {
            if (other.gameObject.tag == "Left Controller")
            {
                controllerID = XRControllerID.Left;
                directControllerVisual = other.gameObject.GetComponent<DirectControllerVisual>();
            }

            else if (other.gameObject.tag == "Right Controller")
            {
                controllerID = XRControllerID.Right;
                directControllerVisual = other.gameObject.GetComponent<DirectControllerVisual>();
            }
        }
    }

    void TranslateMap()
    {
        controllerPosition = GetControllerPosition();
        Vector3 Delta = (controllerPosition - controllerStartPosition);

        transform.localPosition += Delta * sensibility;

        controllerStartPosition = controllerPosition;
    }

    void Update()
    {
        if (isGrabedByHand)
        {
            TranslateMap();
        }
    }
}
