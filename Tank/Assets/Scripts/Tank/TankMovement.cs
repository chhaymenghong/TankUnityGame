using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         


	// When scene first starts
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


	// When script is turned on. Called afater Awake before any upate happens
    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
		// reset the value for input
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

	// 
    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true; // to stop external force from moving it
    }

	// 
    private void Start()
    {
		// to reference input
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    
	// running every frame used to get input
    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
		m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

		EngineAudio ();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
		if ( Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f )
		{
			// The tank is idling. Make sure that we are playing idling clip
			if (m_MovementAudio.clip == m_EngineDriving) {
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play (); // has to explicitly tell it to play whenever we change audio clip for audio source
			}
		}
		else {
			// The tank is driving. Make sure that we are playing driving clip
			if (m_MovementAudio.clip == m_EngineIdling) {
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play (); // has to explicitly tell it to play whenever we change audio clip for audio source
			}
		}
    }

	// This is where we should actually move objects. Instead of running every frame, it runs every physic engines step.
	// This is where we do physic change
    private void FixedUpdate()
    {
        // Move and turn the tank.
		Move();
		Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
		Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
		// move relative to its original position
		m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
		// Just a way to store rotation. We can create it using Vector3
		Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }
}