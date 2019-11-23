using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{     
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
    public float AimAngleFromBase { get { return m_angleOffsetFromBaseDir; } }

    //Camera will always look in this direction
    [SerializeField]
    private Transform       m_ballToFocus;

    #region AimHelper
    private List<GameObject> m_aimHelpers;

    //In max difficulty there is 0 helper
    //1 Ray is 1 direction until a cushion bounce
    private int         m_numberOfAimHelperRays;

    [SerializeField]
    private float       m_aimHelperthickness = 0.03f;
    [SerializeField]
    private Color       m_aimHelperColor = Color.blue;
    [SerializeField]
    private LayerMask   m_ballsLayer;
    #endregion

    [SerializeField]
    private InputManager m_inputManager;
    
    private PlayerPreferences.GameDifficulty m_gameDifficulty;
    public void SetGameDifficulty(PlayerPreferences.GameDifficulty _difficulty)
    {
        m_gameDifficulty = _difficulty;
        InitAimHelpers();
    }

    public delegate void CameraChangedAimDirection(Vector3 _direction);
    public event CameraChangedAimDirection CameraChangedAimDirectionEvent;

    private void Start()
    {      
        m_angleOffsetFromBaseDir = 0f;

        m_inputManager.InputCameraRotationEvent += RotateCameraByAngle;           
    }

    private void InitAimHelpers()
    {
        if (m_aimHelpers != null)
            DeleteAimHelpers();
        
        switch (m_gameDifficulty)
        {
            case PlayerPreferences.GameDifficulty.Easy:
                m_numberOfAimHelperRays = 4;
                break;
            case PlayerPreferences.GameDifficulty.Medium:
                m_numberOfAimHelperRays = 2;
                break;
            case PlayerPreferences.GameDifficulty.Hard:
                m_numberOfAimHelperRays = 0;
                break;
            default:
                break;
        }   

        m_aimHelpers = new List<GameObject>(m_numberOfAimHelperRays);
        for (int i = 0; i < m_numberOfAimHelperRays; i++)
        {
            m_aimHelpers.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            m_aimHelpers[i].SetActive(false);

            BoxCollider collider = m_aimHelpers[i].GetComponent<BoxCollider>();
            if (collider)
                Destroy(collider);

            Material material = new Material(Shader.Find("Unlit/Color"));
            material.SetColor("_Color", m_aimHelperColor);
            m_aimHelpers[i].GetComponent<MeshRenderer>().material = material;
        }
    }

    private void SetActiveAimHelpers(bool _b)
    {
        for (int i = 0; i < m_numberOfAimHelperRays; i++)
        {
            m_aimHelpers[i].SetActive(_b);
        }
    }

    private void DisableAimHelpers()
    {
        for (int i = 0; i < m_numberOfAimHelperRays; i++)
        {
            m_aimHelpers[i].SetActive(false);
        }
    }

    private void DeleteAimHelpers()
    {
        for (int i = 0; i < m_aimHelpers.Count; i++)
        {
            if(m_aimHelpers[i])
                GameObject.DestroyImmediate(m_aimHelpers[i]);
        }

        m_aimHelpers = new List<GameObject>();
    }

    //We update Aim Helpers objects depending of camera direction
    private void Update()
    {
        if (!m_ballToFocus)
        {
            Debug.LogError("CameraManager needs a 'ballToFocus'.");
            return;
        }

        if (m_aimHelpers == null)
        {
            //this case is triggered only if nobody called SetGameDifficulty()
            //should be done by GameManager
            InitAimHelpers();
        }
        else
        {
            if (m_numberOfAimHelperRays > 0)
            {
                if (GameManager.Instance && GameManager.Instance.CurrentGameState != GameManager.GameState.Shooting)
                {
                    if (m_aimHelpers[0].activeSelf)
                    {
                        SetActiveAimHelpers(false);
                    }
                }
                else  //If there is no GameManager (for debug / testing purpose ?) we activate AimHelpers anyway
                { 
                    SetActiveAimHelpers(true);

                    //AIMING HELPER
                    Vector3 lastPos = m_ballToFocus.position;
                    Vector3 lastDirection = new Vector3(this.transform.forward.x, 0, this.transform.forward.z).normalized;
                    RaycastHit hit;

                    for (int i = 0; i < m_numberOfAimHelperRays; i++)
                    {
                        if (Physics.Raycast(lastPos, lastDirection, out hit, 10))
                        {
                            //Debug.DrawRay(lastPos, lastDirection.normalized * hit.distance, Color.yellow);                   

                            m_aimHelpers[i].transform.localScale = new Vector3(m_aimHelperthickness, m_aimHelperthickness, hit.distance);
                            m_aimHelpers[i].transform.localPosition = (hit.point + lastPos) * 0.5f;
                            m_aimHelpers[i].transform.localRotation = Quaternion.FromToRotation(Vector3.forward, lastDirection);

                            //we don't show any helper after ball collision
                            //with physics we cannot predict
                            if (Mathf.Pow(2, hit.collider.gameObject.layer) == m_ballsLayer.value)
                            {
                                for (int j = i + 1; j < m_numberOfAimHelperRays; j++)
                                {
                                    m_aimHelpers[j].SetActive(false);
                                }
                                break;
                            }

                            Vector3 newPos = hit.point;
                            Vector3 newDir = Vector3.Reflect(lastDirection, hit.normal);
                            lastDirection = newDir;
                            lastPos = newPos;
                        }
                    }
                }
            }
        }        
    }

    private void RefreshCameraPosition()
    {
        //let's find Camera position in the 2D plan of the table (ball height)
        //I'll use Vector2's 'y' for real 'z'
        Vector2 ballPos = new Vector2(m_ballToFocus.position.x, m_ballToFocus.position.z);
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
        Vector3 newCameraPos = new Vector3(posCamera2D.x, m_ballToFocus.position.y + m_cameraHeight, posCamera2D.y);

        this.gameObject.transform.position = newCameraPos;
    }

    private void RefreshCameraOrientation()
    {
        //Find the direction of Camera with angle = 0 (horizontal plan)
        Vector3 baseDirPoint = new Vector3(m_ballToFocus.position.x, this.transform.position.y, m_ballToFocus.position.z);
        Vector3 baseDir = baseDirPoint - this.transform.position;

        //Calculate vertical distance offset between newDir and baseDir points
        float verticalDist = Mathf.Abs(Mathf.Tan(m_cameraPitchAngle * Mathf.Deg2Rad) * baseDir.magnitude);

        Vector3 newDirPoint = new Vector3(baseDirPoint.x, baseDirPoint.y - verticalDist, baseDirPoint.z);

        this.transform.LookAt(newDirPoint, Vector3.up);

         CameraChangedAimDirectionEvent?.Invoke(new Vector3(transform.forward.x, 0, transform.forward.z));
    }

    private void RotateCameraByAngle(float _angle)
    {
        m_angleOffsetFromBaseDir += _angle;

        RefreshCameraPosition();
        RefreshCameraOrientation();
    }

    public void SetCameraPositionWithAngleFromBase(float _angle)
    {
        m_angleOffsetFromBaseDir = _angle;

        RefreshCameraPosition();
        RefreshCameraOrientation();
    }
}
