using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpecial : EnvObject
{
    public InteractableType interactType;

    public short magazineCount;
    public short healthAmount;
    public short interObjIndex;
    public static short[] interObjIndexArray;

    public static InteractableSpecial[] interactableObjects = new InteractableSpecial[0];

    public AudioClip pickUpSound;

    void OnDestroy()
    {
        AudioSource.PlayClipAtPoint(pickUpSound,transform.position);
    }

    void Start()
    {
        AddInteractableToList(this);
    }

    void Update()
    {
        
    }

    void AddInteractableToList(InteractableSpecial interactObj)
    {
        GameManager.AddToArray(interactObj, ref interactableObjects);
        GameManager.AddToArray(interactObj.interObjIndex, ref interObjIndexArray);
    }

    public static void SaveInteractableObjects(GameData gameDataToUse)
    {
        gameDataToUse.interactablesTaken = new bool[interactableObjects.Length];
        bool[] takenObjArray = gameDataToUse.interactablesTaken;
        for(int i = interactableObjects.Length - 1; i >= 0; i--)
        {
            if (interactableObjects[i] == null)
            takenObjArray[interObjIndexArray[i]] = true;
            else
            {
                takenObjArray[interactableObjects[i].interObjIndex] = false;
            }
        }
    }
    public static void LoadInteractableObjects(GameData gameDataToUse)
    {
        for (int i = interactableObjects.Length - 1; i >= 0; i--)
        {
            if (gameDataToUse.interactablesTaken[interObjIndexArray[i]])
            {
                if(interactableObjects[i] != null)
                Destroy(interactableObjects[i].gameObject);
            }
        }
    }

}
public enum InteractableType { healthPack, ammoBox}