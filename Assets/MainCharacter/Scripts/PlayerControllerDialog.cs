/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerDialog : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 3f))
            {
                NPCInteraction npc = hit.collider.GetComponent<NPCInteraction>();
                if(npc != null) npc.OnInteract();
            }
        }
    }
}
*/