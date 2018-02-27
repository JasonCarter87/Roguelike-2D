using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
    public GameObject GameManager;
    public GameObject soundManager;


    void Awake()
    {
        if (gameManager.instance == null)
            Instantiate(GameManager);
        if (SoundManager.instance = null)
            Instantiate(soundManager);
    }
}
