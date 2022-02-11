using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCrowd : MonoBehaviour
{
    public GameObject refprefab;

    void Start()
    {

        for (int i = -50; i < 50; i++)
        {
            for (int j = 1; j < 26; j++)
            {
                Instantiate(refprefab, new Vector3(i * 0.25f, 0.5f, j * 0.25f), Quaternion.identity);
            }
        }
    }
}
