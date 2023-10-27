using UnityEngine;

public class Generator : MonoBehaviour
{
    public void Generate()
    {
        GetComponent<WorldRenderer>().GenerateWorld(null);
    }

    public void Clear()
    {
        GetComponent<WorldRenderer>().ClearWorld();
    }
}
