using System.Collections.Generic;
using UnityEngine;

public interface IHumanoid
{
    bool AddResourceOnHands(Resource.Resources resource, GameObject resourceObj);

    void RemoveResourceFromHands(Resource.ResourceBase resource);

    void AnimationAdjustment();
}
