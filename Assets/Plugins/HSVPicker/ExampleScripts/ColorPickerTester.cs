using HSVPicker;
using UnityEngine;
namespace HSVPickerExamples
{
    public class ColorPickerTester : MonoBehaviour 
    {

        public Renderer refRenderer = new Renderer();
        public ColorPicker picker;

        public Color Color = Color.red;
        public bool SetColorOnStart = false;

	    // Use this for initialization
	    void Start () 
        {
            picker.onValueChanged.AddListener(color =>
            {
                refRenderer.material.color = color;
                Color = color;
            });

            refRenderer.material.color = picker.CurrentColor;
            if(SetColorOnStart) 
            {
                picker.CurrentColor = Color;
            }
        }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
}