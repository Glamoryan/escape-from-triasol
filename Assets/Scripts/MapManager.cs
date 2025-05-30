using UnityEngine;

public class MapManager : MonoBehaviour
{
    void Start()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            map.transform.position = new Vector3(-0.3586f, -0.7172f, -10);
            map.transform.localScale = new Vector3(10, 10, 1);
        }
        else
        {
            Debug.LogWarning("Map objesi bulunamadı!");
        }
    }
}
