using UnityEngine;

// *** Note: GameObject is like a container for a component.
public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


	// It just enter another collider
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
		// Get all the colliders within this radius
		Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
		for (int i = 0; i < colliders.Length; i++) {
			// Get the rigid body of this collider
			Rigidbody targetRigidBody = colliders [i].GetComponent<Rigidbody> ();
			if (!targetRigidBody) {
				continue;
			}
			// For animation stuffs using physic engine
			targetRigidBody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);
			// Get the TankHealth Script from this rigid body. We can use component reference to find another references on the same Game Object
			TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth> ();
			if (!targetHealth) {
				continue;
			}
			float damage = CalculateDamage (targetRigidBody.position);
			targetHealth.TakeDamage (damage);
		}

		m_ExplosionParticles.transform.parent = null;
		m_ExplosionParticles.Play ();
		m_ExplosionAudio.Play ();

		// wait for the explostion to finish and then delete it
		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
		Destroy (gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
		// Compare the target position with the shell position possibly
		Vector3 explositionToTarget = targetPosition - transform.position;
		float mag = explositionToTarget.magnitude;
		float relativeDistance = (m_ExplosionRadius - mag) / m_ExplosionRadius;
		float damage = relativeDistance * m_MaxDamage;
		return Mathf.Max( 0, damage );
    }
}