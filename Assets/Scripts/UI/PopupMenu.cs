using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMenu : MonoBehaviour
{
	[SerializeField] private GameObject[] pages;
	private int currentPage = 0;

    private void Start()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (i == 0) pages[i].SetActive(true);
            else pages[i].SetActive(false);
        }
    }

    public void Close() {
		gameObject.SetActive(false);
	}

    // forward = true -> next page, forward = false -> previous page
    public void NextPage(bool forward)
    {
        currentPage = forward ? currentPage + 1 : currentPage - 1;
        currentPage = Mod(currentPage, pages.Length);

        for (int i = 0; i < pages.Length; i++)
        {
            if (i == currentPage) pages[i].SetActive(true);
            else pages[i].SetActive(false);
        }

    }

    private int Mod(int a, int b)
    {
        int c = a % b;
        return (c < 0) ? c + b : c;
    }
}
