using UnityEngine;
using ECellDive.Utility.Data.UI;

namespace ECellDive.UI
{
	/// <summary>
	/// A quick implementation of a vertical scroll list that
	/// can be used to display long lists of items without causing
	/// performance issues.
	/// </summary>
	/// <remarks>
	/// This should be refactored to use a pool of items instead of
	/// instantiating everything and deactivating the ones that are
	/// out of the viewport.
	/// </remarks>
	public class OptimizedVertScrollList : MonoBehaviour
	{
		/// <summary>
		/// The reference to the RectTransform of the viewport of the scroll list.
		/// </summary>
		public RectTransform refViewport;

		/// <summary>
		/// The reference to the RectTransform of the content of the scroll list
		/// (it is inside the viewport).
		/// </summary>
		public RectTransform refContent;

		/// <summary>
		/// A reference to the prefab of the item to be displayed in the scroll list.
		/// </summary>
		public GameObject itemPrefab;

		/// <summary>
		/// Padding information for the items in the scroll list.
		/// </summary>
		public Padding padding;

		/// <summary>
		/// The index of the first item to be displayed in the scroll list.
		/// </summary>
		private int firstVisibleChildIdx = 0;

		/// <summary>
		/// The index of the last item to be displayed in the scroll list.
		/// </summary>
		private int lastVisibleChildIdx = 0;

		// Start is called before the first frame update
		void Start()
		{
			UpdateAllChildrenPositions();

			//Updating content rect transform size in the case
			//there are already items in it.
			UpdateContentSize();

			GetBorderVisibleChildren();

			UpdateAllChildrenVisibility();
		}

		/// <summary>
		/// Adds an item (<see cref="itemPrefab"/>) as a child of the content
		/// (<see cref="refContent"/>).
		/// </summary>
		/// <returns>
		/// The newly added item.
		/// </returns>
		public GameObject AddItem()
		{
			GameObject go;
			if (refContent.childCount > 0)
			{
				RectTransform lastChild = refContent.GetChild(refContent.childCount - 1).GetComponent<RectTransform>();
				go = Instantiate(itemPrefab, refContent);
				RectTransform goRT = go.GetComponent<RectTransform>();
				goRT.anchoredPosition = new Vector2(
					0.5f * goRT.rect.width + padding.left,
					lastChild.anchoredPosition.y - (0.5f * lastChild.rect.height + padding.bottom + padding.top + 0.5f * goRT.rect.height));
			}
			else
			{
				go = Instantiate(itemPrefab, refContent);
				RectTransform goRT = go.GetComponent<RectTransform>();
				goRT.anchoredPosition = new Vector2(
					0.5f * goRT.rect.width + padding.left,
					-(padding.top + 0.5f * goRT.rect.height));

			}
			return go;
		}

		/// <summary>
		/// Clears the content of the scroll list.
		/// </summary>
		public void ClearScrollList()
		{
			while(refContent.childCount > 0)
			{
                Destroy(refContent.GetChild(0).gameObject);
            }
		}

		/// <summary>
		/// Performs the necessary updates when a child is destroyed.
		/// </summary>
		public void OnChildDestruction()
		{
			UpdateAllChildrenPositions();
			UpdateContentSize();
			GetBorderVisibleChildren();
			UpdateAllChildrenVisibility();
		}            

		/// <summary>
		/// Computes values for <see cref="firstVisibleChildIdx"/> and
		/// <see cref="lastVisibleChildIdx"/>.
		/// </summary>
		public void GetBorderVisibleChildren()
		{
			int childCounter = 0;
			bool firstChildFound = false;
			foreach(RectTransform _child in refContent)
			{
				if (!firstChildFound &&
				_child.anchoredPosition.y - 0.5f * _child.rect.height +
				refContent.anchoredPosition.y < 0)
				{
					firstChildFound = true;
					firstVisibleChildIdx = childCounter;
				}

				if (_child.anchoredPosition.y + 0.5f * _child.rect.height +
					refContent.anchoredPosition.y > -refViewport.rect.height)
				{
					lastVisibleChildIdx = childCounter;
				}
				childCounter++;
			}
		}

		/// <summary>
		/// Update the size of <see cref="refContent"/> to fit all its children.
		/// </summary>
		private void UpdateContentSize()
		{
			if (refContent.childCount > 0)
			{
				RectTransform lastChild = refContent.GetChild(refContent.childCount - 1).GetComponent<RectTransform>();
				refContent.sizeDelta = new Vector2(0,
													Mathf.Abs(lastChild.anchoredPosition.y) + 0.5f * lastChild.rect.height);
			}
		}

		/// <summary>
		/// Computes the positions of all the children of <see cref="refContent"/>.
		/// </summary>
		public void UpdateAllChildrenPositions()
		{
			float total = 0f;
			foreach (RectTransform _child in refContent)
			{
				total -= padding.top + 0.5f * _child.rect.height;
				_child.anchoredPosition = new Vector2(
					0.5f * _child.rect.width + padding.left, total);

				total -= 0.5f * _child.rect.height - padding.bottom;
			}
		}

		/// <summary>
		/// Computes the visibility of all the children of <see cref="refContent"/>
		/// based on their index respectively to <see cref="firstVisibleChildIdx"/>
		/// and <see cref="lastVisibleChildIdx"/> (i.e. visible if their index is
		/// between those two values).
		/// </summary>
		public void UpdateAllChildrenVisibility()
		{
			if (refContent.childCount > 0)
			{
				for (int i = 0; i < firstVisibleChildIdx; i++)
				{
					refContent.GetChild(i).gameObject.SetActive(false);
				}
				for (int i = firstVisibleChildIdx; i <= lastVisibleChildIdx; i++)
				{
					refContent.GetChild(i).gameObject.SetActive(true);
				}
				for (int i = lastVisibleChildIdx + 1; i < refContent.childCount; i++)
				{
					refContent.GetChild(i).gameObject.SetActive(false);
				}
			}
		}

		/// <summary>
		/// Updates the visibility of the children at the indeces <see cref="firstVisibleChildIdx"/> and
		/// <see cref="lastVisibleChildIdx"/>.
		/// </summary>
		public void UpdateBorderChildrenVisibility()
		{
			if (refContent.childCount > 0)
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
		}

		/// <summary>
		/// Updates the scroll list.
		/// </summary>
		public void UpdateScrollList()
		{
			UpdateContentSize();
			GetBorderVisibleChildren();
			UpdateBorderChildrenVisibility();
		}
	}
}