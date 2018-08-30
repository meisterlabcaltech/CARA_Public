using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginControl : MonoBehaviour {
	public ExperimentControl ExpScript;
	private AudioSource OriginSignal;
	public AudioSource StartHere;
	// Use this for initialization
	void Start () {
		OriginSignal = this.gameObject.GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		switch (ExpScript.TaskIndex) {
		case 0:
			{
				StartHere.mute = true;
				if (ExpScript.IsTrialOn && ExpScript.IsTrialFinished) {

					if (ExpScript.OriginDistance > ExpScript.OriginThreshold) {
						OriginSignal.mute = true;
					} else {
						OriginSignal.volume = Mathf.Max (0f, 0.5f - ExpScript.OriginDistance * 1f);
					}
				} else {
					OriginSignal.volume = Mathf.Max (0f, 0.5f - ExpScript.OriginDistance * 1f);
				}
				break;
			}
		case 1:
			{
				StartHere.mute = true;
				OriginSignal.mute = true;
				if (ExpScript.IsTrialOn && ExpScript.IsTrialFinished) {

					if (ExpScript.OriginDistance > ExpScript.OriginThreshold) {
						OriginSignal.mute = true;
					} else {
						OriginSignal.volume = Mathf.Max (0f, 0.5f - ExpScript.OriginDistance * 1f);
					}
				} else {
					OriginSignal.volume = Mathf.Max (0f, 0.5f - ExpScript.OriginDistance * 1f);
				}
				break;
			}
		case 2:
			{
				if (ExpScript.IsTrialOn && ExpScript.IsTrialFinished) {
					StartHere.mute = false;
				} else {
					StartHere.mute = true;
				}
				if (!ExpScript.IsTrialOn) {
					OriginSignal.mute = false;
					OriginSignal.volume = Mathf.Max (0f, 0.5f - ExpScript.OriginDistance * 1f);
				} else {
					OriginSignal.mute = true;
				}
				break;
			}
		case 3:
			{
				if (ExpScript.IsTrialOn && ExpScript.IsTrialFinished) {
					StartHere.mute = false;
				} else {
					StartHere.mute = true;
				}
				if (!ExpScript.IsTrialOn) {
					OriginSignal.mute = false;
					OriginSignal.volume = Mathf.Max (0f, 0.5f - ExpScript.OriginDistance * 1f);
				} else {
					OriginSignal.mute = true;
				}
				break;
			}
		default:
			{
				break;
			}
		}
	}
}
