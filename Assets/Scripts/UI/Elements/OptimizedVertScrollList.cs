using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECellDive
{
    namespace UI
    {
        [System.Serializable]
        public struct Padding
        {
            public float left;
            public float right;
            public float top;
            public float bottom;
        }

        public class OptimizedVertScrollList : MonoBehaviour
        {
            public RectTransform refViewport;
            public RectTransform refContent;

            public Padding padding;

            private int firstVisibleChildIdx = 0;
            private int lastVisibleChildIdx = 0;

            // Start is called before the first frame update
            void Start()
            {
                //Updating content rect transform size in the case
                //there are already items in it.
                UpdateContentSize();

                GetBorderVisibleChildren();

                UpdateAllChildrenVisibility();
            }

            public GameObject AddItem(GameObject _item)
            {
                GameObject go;
                if (refContent.childCount > 0)
                {
                    RectTransform lastChild = refContent.GetChild(refContent.childCount - 1).GetComponent<RectTransform>();
                    go = Instantiate(_item, refContent);
                    RectTransform goRT = go.GetComponent<RectTransform>();
                    goRT.anchoredPosition = new Vector2(
                        0.5f * goRT.rect.width + padding.left,
                        lastChild.anchoredPosition.y - (0.5f * lastChild.rect.height + padding.bottom + padding.top + 0.5f * goRT.rect.height));
                }
                else
                {
                    go = Instantiate(_item, refContent);
                    RectTransform goRT = go.GetComponent<RectTransform>();
                    goRT.anchoredPosition = new Vector2(
                        0.5f * goRT.rect.width + padding.left,
                        -(padding.top + 0.5f * goRT.rect.height));

                }
                return go;
            }

            //public void DrawActiveChildren()
            //{
            //    Debug.Log("Drawing all active children");
            //    float total = 0f;
            //    foreach(RectTransform _child in refContent)
            //    {
            //        if (_child.gameObject.activeSelf)
            //        {
            //            total -= padding.top + 0.5f * _child.rect.height;
            //            _child.anchoredPosition = new Vector2(
            //                0.5f * _child.rect.width + padding.left, total);

            //            total -= 0.5f * _child.rect.height - padding.bottom;
            //        }
            //    }
            //}

            public void GetBorderVisibleChildren()
            {
                int childCounter = 0;
                bool firstChildFound = false;
                foreach(RectTransform _child in refContent)
                {
                    //if (_child.gameObject.activeSelf)
                    //{
                    if (!firstChildFound &&
                    _child.anchoredPosition.y - 0.5f * _child.rect.height +
                    refContent.anchoredPosition.y < 0)
                    {
                        firstChildFound = true;
                        firstVisibleChildIdx = childCounter;
                    }

                    //Debug.Log($"anch pos: {_child.anchoredPosition};" +
                    //    $" height {0.5f * _child.rect.height};" +
                    //    $"viewport anch: {refViewport.anchoredPosition.y}" +
                    //    $"viewport height: {refViewport.rect.height}" +
                    //    $" refContent.top {refContent.anchoredPosition.y}");
                    if (_child.anchoredPosition.y + 0.5f * _child.rect.height +
                        refContent.anchoredPosition.y > -refViewport.rect.height)
                    {
                        lastVisibleChildIdx = childCounter;
                    }
                    //}

                    childCounter++;
                }
                Debug.Log($"Final first:{firstVisibleChildIdx}; last:{lastVisibleChildIdx}");
            }

            //public void ShowAllChildren()
            //{
            //    foreach(RectTransform _child in refContent)
            //    {
            //        _child.gameObject.SetActive(true);
            //    }
            //}

            private void UpdateContentSize()
            {
                if (refContent.childCount > 0)
                {
                    RectTransform lastChild = refContent.GetChild(refContent.childCount - 1).GetComponent<RectTransform>();
                    refContent.sizeDelta = new Vector2(0,
                                                       Mathf.Abs(lastChild.anchoredPosition.y) + 0.5f * lastChild.rect.height);
                }
            }

            public void UpdateAllChildrenVisibility()
            {
                Debug.Log("Updating All children visibility");
                for (int i = 0; i < firstVisibleChildIdx; i++)
                {
                    refContent.GetChild(i).gameObject.SetActive(false);
                }
                for (int i = lastVisibleChildIdx + 1; i < refContent.childCount; i++)
                {
                    refContent.GetChild(i).gameObject.SetActive(false);
                }
            }

            public void UpdateBorderChildrenVisibility()
            {
                RectTransform firstVisibleChild = refContent.GetChild(firstVisibleChildIdx).GetComponent<RectTransform>();
                float refContentTop = refContent.anchoredPosition.y;

                if (firstVisibleChild.anchoredPosition.y - 0.5f * firstVisibleChild.rect.height + refContentTop > 0)
                {
                    firstVisibleChild.gameObject.SetActive(false);
                    firstVisibleChildIdx++;
                }

                else if (firstVisibleChild.anchoredPosition.y + 0.5f * firstVisibleChild.rect.height + refContentTop < 0)
                {
                    if (firstVisibleChildIdx >= 1)
                    {
                        firstVisibleChildIdx--;
                        firstVisibleChild = refContent.GetChild(firstVisibleChildIdx).GetComponent<RectTransform>();
                        firstVisibleChild.gameObject.SetActive(true);
                    }
                }

                RectTransform lastVisibleChild = refContent.GetChild(lastVisibleChildIdx).GetComponent<RectTransform>();

                if (lastVisibleChild.anchoredPosition.y + 0.5f * lastVisibleChild.rect.height + refContentTop <
                    -refViewport.rect.height)
                {
                    lastVisibleChild.gameObject.SetActive(false);
                    lastVisibleChildIdx--;
                }

                if (lastVisibleChild.anchoredPosition.y - 0.5f * lastVisibleChild.rect.height + refContentTop >
                    -refViewport.rect.height)
                {
                    if (lastVisibleChildIdx < refContent.childCount - 1)
                    {
                        lastVisibleChildIdx++;
                        lastVisibleChild = refContent.GetChild(lastVisibleChildIdx).GetComponent<RectTransform>();
                        lastVisibleChild.gameObject.SetActive(true);
                    }
                }
            }

            public void UpdateScrollList()
            {
                UpdateContentSize();
                GetBorderVisibleChildren();
                UpdateBorderChildrenVisibility();
            }
        }
    }
}