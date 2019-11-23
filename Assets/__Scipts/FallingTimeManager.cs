using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class FallingTimeManager : MonoBehaviour
{

    public PlayableDirector pd;

    public void PauseTimeline()
    {
        pd.playableGraph.GetRootPlayable(0).SetSpeed(0f);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            pd.playableGraph.GetRootPlayable(0).SetSpeed(1f);
        }
    }

}
