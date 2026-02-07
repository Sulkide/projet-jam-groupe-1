using System.Collections.Generic;
using UnityEngine;

public class PV : MonoBehaviour
{
    PlayerClass p;
    int pvSave;
    public List<GameObject> hearts;
    private void Start()
    {
        p = PlayerClass.instance;
    }
    void Update()
    {
        if (pvSave != p.PV) { pvSave = p.PV; UpdatePV(); }
    }

    void UpdatePV()
    {
        foreach(GameObject heart in hearts)
        {
            heart.SetActive(false);
        }
        for(int i = 0;i < pvSave; i++)
        {
            hearts[i].SetActive(true);
        }
    }
}
