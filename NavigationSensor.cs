using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NavigationSensor 
{
    public Transform t;

    public ColisionSensor colisionSensor;

    public Vector3 lastDirection = Vector3.zero;

    #region
    [InspectorName("ground options")]

    public float minDistanceToBeGrouded = .6f;

    public float distanceAverage;
    public float distancesDifference;
    public Vector3 CenterDirectionNormal;
    public Vector3 NormalsAverage;

    public RaycastHit groundFoward;
    public RaycastHit groundBackward;
    public float distanceF;
    public float distanceB;

    public float offsetV = 1f;
    public float offsetF = 1f;
    

    public float[] groundDistances = new float[2];
    public Vector3[] groundNormals = new Vector3[2];

    
    #endregion

    public bool isGrounded = true;

    public LayerMask layerMask;

    public float edgethreshold = 0.05f;
    public float stepThreshold = 0.05f;
    public float stepDistance = 0;
    public bool isInEdge;
    public bool isInStep;
    public bool isInPlane;
    public bool isInUpSlope;
    public bool isInDownSlope;
    public bool isInPeak;


    public void CastMovementDirectionRay(Vector3 _orign,Vector3 dir, ref RaycastHit hit, float angle)
    {
        if (dir.magnitude == 0) {dir = lastDirection;};

        Transform characterTransform = t;

        Vector3 orign = _orign;

        dir.y = 0;

        Vector3 walkingDirection = characterTransform.forward;

        float rotationAngle = Vector3.SignedAngle(Vector3.forward, dir, characterTransform.up);

        rotationAngle += angle;

        walkingDirection = Quaternion.AngleAxis(rotationAngle, characterTransform.up) * characterTransform.forward;

        orign += walkingDirection * offsetF;
        orign.y += offsetV;

        Physics.Raycast(orign, -characterTransform.up, out hit, 500f, layerMask);

        if (angle == 0)
        {
            Debug.DrawRay(orign, -characterTransform.up, Color.blue);
        }
        else
        {
            Debug.DrawRay(orign, -characterTransform.up, Color.red);
        }

        dir.x *= -1;

        float localNormalAngle = Vector3.SignedAngle(characterTransform.forward, dir, characterTransform.up);

        hit.normal = Quaternion.AngleAxis(localNormalAngle, characterTransform.up) * hit.normal;

        lastDirection = dir;

    }

    public void FowardRaysAverages()
    {
        Transform characterTransform = t;

        distanceAverage = (groundFoward.distance + groundBackward.distance) / 2;

        distancesDifference = (groundFoward.distance - groundBackward.distance);

        NormalsAverage = (groundBackward.normal + groundFoward.normal) / 2;

        CenterDirectionNormal = new Vector3(groundFoward.normal.x, offsetF * 2, distancesDifference).normalized;

        distanceF = Vector3.Distance(characterTransform.position, groundFoward.point);

        distanceB = Vector3.Distance(characterTransform.position, groundBackward.point);

    }

    public void UpdateMovementSensors(Vector3 orign,Vector3 direction)
    { 
        Vector3 inputsDir = direction;

        CastMovementDirectionRay(orign,inputsDir, ref groundFoward, 0.0f);
        CastMovementDirectionRay(orign,inputsDir, ref groundBackward, 180.0f);

        FowardRaysAverages();

        UpdateIsGrounded();

        if(groundDistances.Length > 1){
        groundDistances[0] = groundFoward.distance;
        groundDistances[1] = groundBackward.distance;
        }


        if(groundNormals.Length > 1){
        groundNormals[0] = groundFoward.normal;
        groundNormals[1] = groundBackward.normal;
        }

    }

    public void UpdateIsGrounded()
    {
        float tempMinDistanceModifier = 0;

        if(isInPeak){tempMinDistanceModifier += Mathf.Abs(groundFoward.normal.z) + Mathf.Abs(groundBackward.normal.z);}

        if(isInEdge){tempMinDistanceModifier += Mathf.Abs(groundFoward.distance - offsetV);}

        float tempMinDistanceToBeGrounded = (minDistanceToBeGrouded + offsetV);

        tempMinDistanceModifier = 0;

        if ((groundBackward.distance <= tempMinDistanceToBeGrounded) || (groundFoward.distance <= tempMinDistanceToBeGrounded)) { isGrounded = true; } else { isGrounded = false; }

        TerrainDefinitions();

    }

    public void AdjustRaycastHorizontalOffset(Collider collider)
    {

        offsetF += collider.bounds.extents.x + 0.005f;

    }


    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

    public void TerrainDefinitions()
    {
        isInStep = false;
        isInEdge = false;
        isInDownSlope = false;
        isInUpSlope = false;
        isInPeak = false;


        if (CenterDirectionNormal.z > groundBackward.normal.z + edgethreshold)
        {
            isInEdge = true;
        }

        if (CenterDirectionNormal.z < groundBackward.normal.z - stepThreshold)
        {
            isInStep = true;

            stepDistance = Mathf.Abs(groundFoward.distance - offsetV);
        }

        if (CenterDirectionNormal.z > 0)
        {
            isInDownSlope = true;
        }

        if (CenterDirectionNormal.z < 0)
        {
            isInUpSlope = true;
        }

        if(groundFoward.normal.z > 0 && groundBackward.normal.z < 0){

            isInPeak =true;

        }
    }
}
