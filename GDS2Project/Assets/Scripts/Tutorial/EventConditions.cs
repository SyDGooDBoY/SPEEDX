using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define event condition interface
public interface IEventCondition
{
    bool CheckSuccess(); // Interface for event conditions
}

// Default
public class DefaultEventCondition : IEventCondition
{
    public bool CheckSuccess()
    {
        return true;
    }
}

// Jump
public class JumpEventCondition : IEventCondition
{
    public bool CheckSuccess()
    {
        Debug.Log("Jump event succeeded.");
        return true; 
    }
}

// Grappling
public class GrapplingHookEventCondition : IEventCondition
{
    public bool CheckSuccess()
    {
        Debug.Log("Grappling hook event succeeded.");
        return true;
    }
}
