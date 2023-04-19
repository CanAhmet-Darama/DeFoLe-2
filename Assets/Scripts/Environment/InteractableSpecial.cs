using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpecial : EnvObject
{
    public InteractableType interactType;

    public short magazineCount;
    public short healthAmount;
    public short interObjIndex;

    public static InteractableSpecial[] interactableObjects = new InteractableSpecial[0];

    public AudioClip pickUpSound;

    void Start()
    {
        AddInteractableToList(this);
        //if(gameObject.name == "Ammo Box" && transform.parent.name == "Start Point")
        //{
            StartCoroutine(AssignInteractObjIndexCoroutine());
        //}
    }

    void Update()
    {
        
    }

    void AddInteractableToList(InteractableSpecial interactObj)
    {
        GameManager.AddToArray(ref interactObj, ref interactableObjects);
    }
    public IEnumerator AssignInteractObjIndexCoroutine()
    {
        yield return null;
        Transform[] interactTransforms = new Transform[interactableObjects.Length];
        for (int index = interactableObjects.Length - 1; index >= 0; index--)
        {
            interactTransforms[index] = interactableObjects[index].transform;
        }
        GameManager.SortObjectArrayByDistance(ref interactableObjects, interactTransforms, Vector3.zero);
        for (int index = interactableObjects.Length - 1; index >= 0; index--)
        {
                interactableObjects[index].interObjIndex = (short)index;
        }
    }


    public static void SaveInteractableObjects(GameData gameDataToUse)
    {
        gameDataToUse.interactablesTaken = new bool[interactableObjects.Length];
        gameDataToUse.interactablePositions = new float[interactableObjects.Length][];
        bool[] takenObjArray = gameDataToUse.interactablesTaken;
        for(int i = interactableObjects.Length - 1; i >= 0; i--)
        {

            if (interactableObjects[i] == null)
            takenObjArray[interactableObjects[i].interObjIndex] = true;
            else
            {
                takenObjArray[interactableObjects[i].interObjIndex] = false;
                gameDataToUse.interactablePositions[interactableObjects[i].interObjIndex] = new float[3]
                {
                interactableObjects[i].transform.position.x,
                interactableObjects[i].transform.position.y,
                interactableObjects[i].transform.position.z
                };
            }
        }
    }
    public static void LoadInteractableObjects(GameData gameDataToUse)
    {
        for (int i = interactableObjects.Length - 1; i >= 0; i--)
        {
            if (gameDataToUse.interactablesTaken[interactableObjects[i].interObjIndex])
            {
                if(interactableObjects[i] != null)
                Destroy(interactableObjects[i].gameObject);
            }
            else
            {
                interactableObjects[i].transform.position = new Vector3(
                    gameDataToUse.interactablePositions[interactableObjects[i].interObjIndex][0],
                    gameDataToUse.interactablePositions[interactableObjects[i].interObjIndex][1],
                    gameDataToUse.interactablePositions[interactableObjects[i].interObjIndex][2]
                    );
            }
        }
    }

}
public enum InteractableType { healthPack, ammoBox}