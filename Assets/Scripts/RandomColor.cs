using UnityEngine;

public class RandomColor : MonoBehaviour
{
    void OnEnable()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetVector("_Color", new Color(1, 0, 1));
        renderer.SetPropertyBlock(block);
    }

    static Color GetRandomColor()
    {
        return Color.HSVToRGB(Random.value, 1, .9f);
    }
}