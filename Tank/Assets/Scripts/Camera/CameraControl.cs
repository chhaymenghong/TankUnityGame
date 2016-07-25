using UnityEngine;

public class CameraControl : MonoBehaviour
{
	// Time it takes to move the camera to the new position
    public float m_DampTime = 0.2f;                 
    // Buffer so that the the tank is not out of the screen
	public float m_ScreenEdgeBuffer = 4f;           
    // Don't want to zoom any more than this. Higher the value, the more zoom out
	public float m_MinSize = 6.5f;                  
	// transform for tanks
    /*[HideInInspector]*/ public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;     
	// center of the two tanks by taking the average of the two tanks
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
			// activateSelf: tag for tank that is still in play
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


	// Find the size for the camera. The bigger the camera, the more contents we can fit in the camera
    private float FindRequiredSize()
    {
		// Here we are trying to find the desired position for the camera within the camera rig space. That is why we are using transform of the camera rig
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);


        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

		// since we can have multiple tanks at different position, we want to take the biggest size to make sure the tank further away from the camera appear in the camera space
        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}