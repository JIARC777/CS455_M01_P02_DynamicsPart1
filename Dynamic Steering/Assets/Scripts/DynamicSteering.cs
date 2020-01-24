using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface dynamicSteering
{
    SteeringOutput GetSteering();
}
public class Seek: dynamicSteering
{
    public Kinematic ai;
    // Pseudocode has target as Kinematic, but it only needs to be a gameobject with transform.
    public GameObject target;
    float maxAcceleration = 5f;
    public bool seek = true;

    public SteeringOutput GetSteering()
    {
        SteeringOutput result = new SteeringOutput();

        //get direction to target
        if (seek)
            result.linear = target.transform.position - ai.transform.position;
        else
            result.linear = ai.transform.position - target.transform.position;
        result.linear.Normalize();
        // give full acceleration
        result.linear *= maxAcceleration;
        result.angular = 0;
        return result;
    }
}

public class Arrive: dynamicSteering
{
    public Kinematic ai;
    public GameObject target;
    float maxAcceleration = 100f;
    float targetRadius = 5f;
    float slowRadius = 20f;
    float timeToTarget = 0.1f;
    float maxSpeed = 5f;

    public SteeringOutput GetSteering()
    {
        SteeringOutput result = new SteeringOutput();
        result.linear = target.transform.position - ai.transform.position;
        float distance = result.linear.magnitude;
        float targetSpeed;
        Vector3 targetVelocity;
       // if (distance < targetRadius)
       //     return null;
        if (distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        } else
        {
            targetSpeed = maxSpeed * (distance - targetRadius) / targetRadius;
        }
        targetVelocity = result.linear;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        result.linear = targetVelocity - ai.linearVelocity;
        result.linear /= timeToTarget;

        if (result.linear.magnitude > maxAcceleration)
        {
            result.linear.Normalize();
            result.linear *= maxAcceleration;
        }
        result.angular = 0;
        return result;
        
    }
    
}

public class Align: dynamicSteering
{
    public Kinematic ai;
    public GameObject target;
    float maxAngularAcceleration = 10f;
    float maxRotation = 1f;
    float targetRadius = 10f;
    float slowRadius = 1f;
    float timeToTarget = 1f;

    public SteeringOutput GetSteering()
    {
        SteeringOutput result = new SteeringOutput();
        float rotation = Mathf.DeltaAngle(getTargetAngle(), ai.transform.rotation.eulerAngles.y);
        //Debug.Log(rotation);
        float rotationSize = Mathf.Abs(rotation);
        float targetRotation;
        //if (rotationSize < targetRadius)
        // {
        //     result.angular = 0;
        //     Debug.Log("Test");
        //     return null;
        // }    
        if (rotationSize > slowRadius)
        {
            targetRotation = maxRotation;
        }
        else
        {
         //   targetRotation = maxRotation * (rotationSize / targetRadius);
              targetRotation = maxRotation * (rotationSize - targetRadius) / targetRadius;
        }   
        targetRotation *= rotation / rotationSize;

        result.angular = targetRotation - rotation;
        
        result.angular /= timeToTarget;
        Debug.Log(result.angular);
        float angularAcceleration = Mathf.Abs(result.angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            result.angular /= angularAcceleration;
            result.angular *= maxAngularAcceleration;
        }
       // Debug.Log(result.angular);
        result.linear = Vector3.zero;
        return result;
    }
    public virtual float getTargetAngle()
    {
        return target.transform.rotation.eulerAngles.y;
    }
}

public class LookWhereGoing: Align {
    
    public override float getTargetAngle()
    {
        Vector3 velocity = ai.linearVelocity;
        if (velocity.magnitude == 0)
        {
            return 0;
        }
        Debug.Log("Correct Angle");
        return Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
    }
}


public class Face: Align
{
    public override float getTargetAngle()
    {
        Vector3 direction = (target.transform.position - ai.transform.position);
        if (direction.magnitude == 0)
        {
            return 0;
        }
        Debug.Log("Correct Angle");
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}
