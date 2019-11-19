using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{ 
    //Camera will always look in this direction
    public Transform m_ballToFocusTransform;

    //By default, Camera will be aligned to this point
    //I choose the middle of the billiard table
    public Transform m_referencePointForOrientation;

    //the distance between Camera and focused ball
    //if we stay in table's plan
    [Range(0, 5)]
    public float m_horizontalDistanceToBall;

    //Vertical Height of Camera from focused ball
    [Range(0, 10)]
    public float m_cameraHeight;

    //Pitch Angle of the camera in Degrees
    [Range(0, 90)]
    public float m_cameraPitchAngle;

    //Base direction is alignment between White Ball and 'm_referencePointForOrientation' 
    //(I put the table's center). This angle simply move camera around the white ball
    private float m_angleOffsetFromBaseDir;

    void Start()
    {
        m_angleOffsetFromBaseDir = 0f;

        InputManager.Instance.InputCameraRotationEvent += RotateCameraByAngle;

        RefreshCameraPosition();
        RefreshCameraOrientation();
    }

    void Update()
    {
        RefreshCameraPosition();
        RefreshCameraOrientation();
    }

    private void RefreshCameraPosition()
    {
        //let's find Camera position in the 2D plan of the table (ball height)
        //I'll use Vector2's 'y' for real 'z'
        Vector2 ballPos = new Vector2(m_ballToFocusTransform.position.x, m_ballToFocusTransform.position.z);
        //baseOrientPoint is the point we use to align camera and ball with
        Vector2 baseOrientPoint = new Vector2(m_referencePointForOrientation.position.x, m_referencePointForOrientation.position.z);
        Vector2 ballToOrient = baseOrientPoint - ballPos;
        //we just need directions and angle, normalizing will simplify calculations
        Vector2 orientNorm = ballToOrient.normalized;

        //angle between X axis and 'ballToOrient' vector
        float baseAngle = 0;
        if (Vector2.Dot(new Vector2(0, 1), orientNorm) >= 0)
        {
            baseAngle = Mathf.Acos(orientNorm.x);
        }
        else
        {
            baseAngle = 2 * Mathf.PI - Mathf.Acos(orientNorm.x);
        }

        //I calculate a new "orientation point" from "base orientation point" with an offset angle
        Vector2 newOrientPoint = new Vector2(Mathf.Cos(baseAngle + m_angleOffsetFromBaseDir) + ballPos.x, Mathf.Sin(baseAngle + m_angleOffsetFromBaseDir) + ballPos.y);

        Vector2 posCamera2D = ballPos + (ballPos - newOrientPoint) * m_horizontalDistanceToBall;

        //now we have the new camera position in this 2D Plan, we just have to add the Height
        Vector3 newCameraPos = new Vector3(posCamera2D.x, m_ballToFocusTransform.position.y + m_cameraHeight, posCamera2D.y);

        this.gameObject.transform.position = newCameraPos;
    }

    private void RefreshCameraOrientation()
    {
        //Find the direction of Camera with angle = 0 (horizontal plan)
        Vector3 baseDirPoint = new Vector3(m_ballToFocusTransform.position.x, this.transform.position.y, m_ballToFocusTransform.position.z);
        Vector3 baseDir = baseDirPoint - this.transform.position;

        //Calculate vertical distance offset between newDir and baseDir points
        float verticalDist = Mathf.Abs(Mathf.Tan(m_cameraPitchAngle * Mathf.Deg2Rad) * baseDir.magnitude);

        Vector3 newDirPoint = new Vector3(baseDirPoint.x, baseDirPoint.y - verticalDist, baseDirPoint.z);

        this.transform.LookAt(newDirPoint, Vector3.up);
    }

    public void RotateCameraByAngle(float _angle)
    {
        m_angleOffsetFromBaseDir += _angle;
    }
}
