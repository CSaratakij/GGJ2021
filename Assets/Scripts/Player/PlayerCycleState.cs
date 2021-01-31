using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCycleState : MonoBehaviour
{
    [SerializeField]
    int workAtleast;

    [SerializeField]
    int relaxAtleast;

    [SerializeField]
    Animator anim;

    [SerializeField]
    InnerTime innerTime;

    bool previousState;
    bool isWorking = true;

    int workingCumulative;
    int relaxingCumulative;

    void Start()
    {
        innerTime.OnDayPass += OnDayPass;
    }

    void OnDestroy()
    {
        innerTime.OnDayPass -= OnDayPass;
    }

    void OnDayPass(DateTime date)
    {
        if (isWorking && (workingCumulative > workAtleast)) {
            isWorking = false;
            workingCumulative = 0;
            anim.SetBool("is_relex", true);
        }
        else if (!isWorking && (relaxingCumulative > relaxAtleast)) {
            isWorking = true;
            relaxingCumulative = 0;
            anim.SetBool("is_relex", false);
        }

        if (isWorking) {
            workingCumulative += 1;
        }
        else {
            relaxingCumulative += 1;
        }
    }
}

