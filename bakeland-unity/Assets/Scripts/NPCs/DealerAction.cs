using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerAction : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InteractedWithDealerStart()
    {
        anim.SetBool("IsInteracted", true);
    }

    public void InteractionWithDealerEnd()
    {
        anim.SetBool("IsInteracted", false);
    }
}
