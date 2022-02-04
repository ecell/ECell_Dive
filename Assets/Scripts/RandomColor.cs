using UnityEngine;

public class RandomColor : MonoBehaviour
{
    void OnEnable()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetVector("_Color", GetRandomColor());
        renderer.SetPropertyBlock(block);
    }

    static Color GetRandomColor()
    {
        return Color.HSVToRGB(Random.value, 1, .9f);
    }
}