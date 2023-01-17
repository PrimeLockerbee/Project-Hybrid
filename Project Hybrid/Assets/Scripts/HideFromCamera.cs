using UnityEngine;

public class HideFromCamera : MonoBehaviour
{
    [HideInInspector] public bool isHidden;
    private new MeshRenderer renderer;
    private Material material;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();

/*        material.CopyPropertiesFromMaterial(renderer.materials[0]);
        //renderer.materials[0].CopyPropertiesFromMaterial(material);

        renderer.materials[0] = material;*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Hide();
        }
    }

    public void Hide()
    {
        isHidden = true;
        Color noAlphaColor = renderer.materials[0].color;
        noAlphaColor.a = 0;
        renderer.materials[0].color = noAlphaColor;
    }

    public void Show()
    {
        isHidden= false;
        Color noAlphaColor = renderer.materials[0].color;
        noAlphaColor.a = 1;
        renderer.materials[0].SetColor(0, noAlphaColor);
    }
}
