using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvObject : MonoBehaviour
{
    public EnvObjType objectType;
    public bool destroyable;
    public short healthOfObject;
}
public enum EnvObjType { general, dirt, metal, wood, concrete}