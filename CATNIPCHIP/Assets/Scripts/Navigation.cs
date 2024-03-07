using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class Navigation
{
    [SerializeField]
    private SplineAnimate spline;

    [SerializeField]
    private bool usesSplineNavigation;

#if usesSplineNavigation == true

    [SerializeField]
    public Waypoint[] waypoints;

#endif

    private void AnimateOnSpline(SplineContainer splineContainer)
    {
        spline.Container = splineContainer;

    }
    
}