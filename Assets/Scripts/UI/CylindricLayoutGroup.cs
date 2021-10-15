using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ECellDive.Utility;

public class CylindricLayoutGroup : HorizontalOrVerticalLayoutGroup
{
    [Header("Cylindric Properties")]
    public float startRotation = 90f;
    public float radius = 1f;

    //public float height = 1f;

    protected CylindricLayoutGroup()
    { }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, false);
    }

    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1, false);
    }

    private Vector3 CylindricPosition(float _rad)
    {
        float onCylinderX = radius * Mathf.Cos(_rad);
        float onCylinderY = _rad % (2 * Mathf.PI);
        float onCylinderZ = radius * Mathf.Sin(_rad);

        return new Vector3(onCylinderX, onCylinderY, onCylinderZ);
    }

    public override void SetLayoutHorizontal()
    {
        float rad = startRotation * Mathf.Deg2Rad;
        float arc = rad * radius;

        if (transform.childCount > 0)
        {
            transform.GetChild(0).localPosition = CylindricPosition(rad);
            Vector3 target = new Vector3(Camera.main.transform.position.x,
                                         transform.GetChild(0).position.y,
                                         Camera.main.transform.position.z);
            Positioning.UIFaceTarget(transform.GetChild(0).gameObject, target);
            arc -= 0.5f * transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

            for (int i = 1; i < transform.childCount; i++)
            {
                arc -= 0.5f * transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
                rad = arc / radius;

                transform.GetChild(i).localPosition = CylindricPosition(rad);

                target = new Vector3(Camera.main.transform.position.x,
                                     transform.GetChild(i).position.y,
                                     Camera.main.transform.position.z);
                Positioning.UIFaceTarget(transform.GetChild(i).gameObject, target);
                arc -= 0.5f * transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
            }
        }
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, false);
    }
}
