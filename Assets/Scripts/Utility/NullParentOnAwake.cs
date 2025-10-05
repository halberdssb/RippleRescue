using UnityEngine;

/*
 *  Sets the object's transform parent to null on awake
 *
 *  Jeff Stevenson
 *  9.25.25
 */

public class NullParentOnAwake : MonoBehaviour
{
    private void Awake()
    {
        transform.parent = null;
    }
}
