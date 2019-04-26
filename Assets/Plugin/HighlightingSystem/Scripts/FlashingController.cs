using UnityEngine;
using System.Collections;

public class FlashingController : HighlightingController
{
    public Color flashingStartColor = new Color(1, 0.6f, 0.1f);
	public Color flashingEndColor = new Color(1, 0.6f, 0.1f);
    public float flashingDelay = 2.5f;
	public float flashingFrequency = 2f;
	
	void Start()
	{
		StartCoroutine(DelayFlashing());
	}
	
	protected IEnumerator DelayFlashing()
	{
		yield return new WaitForSeconds(flashingDelay);
		
		// Start object flashing after delay
		ho.FlashingOn(flashingStartColor, flashingEndColor, flashingFrequency);
	}
}
