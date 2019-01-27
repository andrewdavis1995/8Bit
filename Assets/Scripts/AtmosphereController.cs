using UnityEngine;

public class AtmosphereController : MonoBehaviour
{
    public Material SkyboxMaterial;
    public Color TopFloorColor;
    public Color BottomFloorColor;
    public Sprite TopFloorSprite;
    public Sprite BottomFloorSprite;

    // Use this for initialization
    void Start()
    {
        RenderSettings.skybox = SkyboxMaterial;
        SetupFloor(GameObject.FindGameObjectsWithTag("GroundTopLayer"), TopFloorColor, TopFloorSprite);
        SetupFloor(GameObject.FindGameObjectsWithTag("Ground"), BottomFloorColor, BottomFloorSprite);
    }

    void SetupFloor(GameObject[] objects, Color color, Sprite sprite)
    {
        foreach(var obj in objects)
        {
            var renderer = obj.GetComponentInChildren<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
        }
    }
}
