using UnityEngine;

/**
 *	Enables light component when parent particle system is playing,
 *	disables when particle system is not playing.
 *	
 *	(c) 2015, Jean Moreno
**/

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{
	private Light lightComponent;
	private ParticleSystem parentParticleSystem;
	
	void Start()
	{
		lightComponent = GetComponent<Light>();
		
		// Find the particle system component in parent objects
		parentParticleSystem = GetComponentInParent<ParticleSystem>();
		
		if (parentParticleSystem == null)
		{
			Debug.LogWarning("WFX_LightFlicker: No ParticleSystem found in parent objects. Light will remain disabled.");
			lightComponent.enabled = false;
		}
	}
	
	void Update()
	{
		if (parentParticleSystem != null)
		{
			// Enable light if particle system is playing, disable if not
			lightComponent.enabled = parentParticleSystem.isPlaying;
		}
	}
}
