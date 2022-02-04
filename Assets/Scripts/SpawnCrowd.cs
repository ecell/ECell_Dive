using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCrowd : MonoBehaviour
{
    public GameObject refprefab;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = -50; i < 50; i++)
        {
            for (int j = 1; j < 26; j++)
            {
                Instantiate(refprefab, new Vector3(i * 0.25f, 0.5f, j * 0.25f), Quaternion.identity);
                //GameObject go = Instantiate(refprefab, new Vector3(i * 0.25f, 0.5f, j * 0.25f), Quaternion.identity);

                //float r = Random.Range(0.0f, 1.0f);
                //float g = Random.Range(0.0f, 1.0f);
                //float b = Random.Range(0.0f, 1.0f);

                //renderer = go.GetComponent<MeshRenderer>();
                //renderer.GetPropertyBlock(props);
                //props.SetColor("_Color", new Color(r, g, b));

                //renderer.SetPropertyBlock(props);
            }
        }
    }
}
