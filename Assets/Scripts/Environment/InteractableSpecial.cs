using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpecial : EnvObject
{
    public InteractableType interactType;

    public short magazineCount;
    public short healthAmount;

    public AudioClip pickUpSound;

    void OnDisable()
    {
        AudioSource.PlayClipAtPoint(pickUpSound,transform.position);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
public enum InteractableType { healthPack, ammoBox}