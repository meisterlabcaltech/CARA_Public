// Copyright Yang Liu @ Caltech
// 08/01/2018
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Windows.Speech;
public class ExperimentControl : MonoBehaviour {
	public bool IsTrialOn, IsTrialFinished, IsEarlyEnd, IsExperimentFinished, IsSceneReaderOn, IsTaskFinished, IsSpotlightOn;
	public AudioSource ChairSource, KeySource, ListSource, SpotlightSource, TargetSource, TrialStartSource, TrialFinishedSource, TimeOutSource, TargetAnnouncementSource, TaskFinishedSource, TargetConfirmedSource, NextTrialReadySource, TurnAroundSource,Task0Source,Task1Source,Task2Source,Task3Source;
	private GameObject[] TargetObjects,SpotlightObjects;
	private GameObject Studio, BedPiano, TVWindow, Chair, ChairParent, Key, KeyParent, TargetConfimedObj, TrialStartObj, TrialFinishedObj, TargetAnnouncementObj, TaskFinishedOBJ, LastSpotlightObject;
	private float[] ScanAngle;
	public Vector3 TargetPosition, OriginPosition, ObjFacing;
	public float OriginDistance, TargetDistance, SpotlightTimer, ExpTimer, RecTimer, TrialTimer;
	public float TargetThreshold, OriginThreshold, SpotlightThreshold, ScanThreshold, BackThreshold, SpotlightInterval, RecordingInterval, AimingError, TrialTimeLimit;
	public int ListIndex, TargetIndex, TaskIndex, TrialIndex, TotalTrialNum, TotalObjNum, FileIndex;
	public string FileName, FileName2, FolderPath, KeyPressed;
	private StreamWriter recording,summary;
	// Use this for initialization
	void Start () {
		TargetObjects = GameObject.FindGameObjectsWithTag ("Target");
		TotalObjNum = TargetObjects.Length;
		Studio = GameObject.Find ("Studio");
		BedPiano = GameObject.Find ("Bed_Piano");
		TVWindow = GameObject.Find ("TV_Window");
		Chair = GameObject.FindGameObjectWithTag ("Chair");
		ChairParent = GameObject.FindGameObjectWithTag ("ChairParent");
		Key = GameObject.FindGameObjectWithTag ("Key");
		KeyParent = GameObject.FindGameObjectWithTag ("KeyParent");
		FileIndex = 0;
		FolderPath = "D:/VR/Recording";
		IsSceneReaderOn = true;
		IsSpotlightOn = false;
		ExpTimer = 0f;
		ResetTask (0);
	}
	
	// Update is called once per frame
	void Update () {
		KeyPressed = "";
		ExpTimer += Time.deltaTime;
		OriginDistance = Vector2.Distance (new Vector2 (Camera.main.transform.position.x, Camera.main.transform.position.z), new Vector2 (OriginPosition.x,OriginPosition.z)); 
		if (TaskIndex != 3) {
			TargetDistance = Vector2.Distance (new Vector2 (Camera.main.transform.position.x, Camera.main.transform.position.z), new Vector2 (TargetPosition.x, TargetPosition.z)); 
		} else {
			TargetDistance = Vector3.Distance (Camera.main.transform.position, TargetPosition);
		}
		if (Input.GetKeyDown (KeyCode.Keypad0)) {
			SwitchTask (0);
		}
		if (Input.GetKeyDown (KeyCode.Keypad1)) {
			SwitchTask (1);
		}
		if (Input.GetKeyDown (KeyCode.Keypad2)) {
			SwitchTask (2);
		}
		if (Input.GetKeyDown (KeyCode.Keypad3)) {
			SwitchTask (3);
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			ResetTask(TaskIndex);
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			IsSceneReaderOn = false;
		}
		if (Input.GetKeyDown (KeyCode.U)) {
			IsSceneReaderOn = true;
		}
		AimingError = 0f;
		UpdateStatus ();
		if (IsSceneReaderOn) {
			if (TaskIndex == 0 || TaskIndex == 1) {
				if (IsTrialOn && !IsTrialFinished) {
					TrialTimer += Time.deltaTime;
					if (Input.GetKeyDown (KeyCode.I) || Input.GetButtonDown ("ToggleSpotlight")) {
						IsSpotlightOn = !IsSpotlightOn;
						KeyPressed += "I";
					}
					if (Input.GetKeyDown (KeyCode.T) || Input.GetButtonDown ("Sonify")) {
						SonifyCurrent ();
						KeyPressed += "T";
					}
					if (Input.GetKeyDown (KeyCode.J)) {
						PreviousOnList ();
						KeyPressed += "J";
					}
					if (Input.GetKeyDown (KeyCode.K)) {
						NextOnList ();
						KeyPressed += "K";
					}
					if (IsSpotlightOn) {
						if (SpotlightSource == null || !SpotlightSource.isPlaying) {
							Spotlight ();
						}
					}
				}
			} else if (TaskIndex == 2) {
				if (IsTrialOn && !IsTrialFinished) {
					TrialTimer += Time.deltaTime;
					if (Input.GetKeyDown (KeyCode.I) || Input.GetButtonDown ("ToggleSpotlight")) {
						IsSpotlightOn = !IsSpotlightOn;
						KeyPressed += "I";
					}
					if (Input.GetKeyDown (KeyCode.L) || Input.GetKeyDown (KeyCode.T) || Input.GetButtonDown ("Sonify")) {
						SonifyChair ();
						KeyPressed += "T";
					}
				}
			} else if (TaskIndex == 3) {
				if (IsTrialOn && !IsTrialFinished) {
					TrialTimer += Time.deltaTime;
					if (Input.GetKeyDown (KeyCode.I) || Input.GetButtonDown ("ToggleSpotlight")) {
						IsSpotlightOn = !IsSpotlightOn;
						KeyPressed += "I";
					}
					if (Input.GetKeyDown (KeyCode.L) || Input.GetKeyDown (KeyCode.T) || Input.GetButtonDown ("Sonify")) {
						SonifyKey ();
						KeyPressed += "T";
					}
					if (IsSpotlightOn) {
						if (SpotlightSource == null || !SpotlightSource.isPlaying) {
							Spotlight ();
						}
					}
				}
			}
		} else if (!IsSceneReaderOn) {
			if (IsTrialOn && !IsTrialFinished) {
				TrialTimer += Time.deltaTime;
			}
		}
		WriteToFile ();
	}
	// Check and Update Trial Status
	public void UpdateStatus(){
		if (TaskIndex == 0 || TaskIndex == 1) {
			if (IsTrialOn && !IsTrialFinished) {
				if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Confirm")) {
					IsTrialFinished = true;
					CalculateError ();
					TargetConfirmedSource.Play ();
					LogSummary ();
				}else if (TrialTimer >= TrialTimeLimit) {
					IsEarlyEnd = true;
					CalculateError ();
					LogSummary ();
					TrialTimer = 0f;
					IsTrialFinished = true;
					CalculateError ();
					TimeOutSource.Play ();
				}
			}else if (IsTrialOn && IsTrialFinished) {
				if (OriginDistance <= OriginThreshold) {
					IsTrialFinished = false;
					IsTrialOn = false;
					TrialIndex = TrialIndex + 1;
					if (TrialIndex == TotalTrialNum) {
						TaskFinishedSource.PlayDelayed (1.5f);
						IsTaskFinished = true;
					} else {
						NextTrialReadySource.PlayDelayed (1.5f);
					}
				} 
			} else if (!IsTrialOn && !IsTrialFinished) {
				if (!IsTaskFinished) {
					if (OriginDistance <= OriginThreshold) {
						if (!NextTrialReadySource.isPlaying) {
							NextTrialReadySource.Play ();
						}
					}
					if (Input.GetKeyDown (KeyCode.G) || Input.GetButtonDown("Start")) {
						ResetTrial ();
						IsTrialOn = true;
						TrialStartSource.Play ();
						TargetAnnouncementSource.PlayDelayed (1.2f);
					}
				}
			}
		} else if (TaskIndex == 2) {
			if (IsTrialOn && !IsTrialFinished) {
				if (TargetDistance <= TargetThreshold) {
					IsTrialFinished = true;
					TrialFinishedSource.Play ();
					LogSummary ();
				}else if (TrialTimer >= TrialTimeLimit || Input.GetKeyDown(KeyCode.E)) {
					IsEarlyEnd = true;
					LogSummary ();
					TrialTimer = 0f;
					IsTrialFinished = true;
					TimeOutSource.Play ();
				}
			} else if (IsTrialOn && IsTrialFinished) {
				if (OriginDistance <= OriginThreshold) {
					IsTrialFinished = false;
					IsTrialOn = false;
					TrialIndex = TrialIndex + 1;
					if (TrialIndex == TotalTrialNum) {
						TaskFinishedSource.PlayDelayed (0f);
						IsTaskFinished = true;
					} else {
						NextTrialReadySource.Play ();
					}
				}
			} else if (!IsTrialOn && !IsTrialFinished) {
				if (!IsTaskFinished) {
					if (OriginDistance <= OriginThreshold) {
						if (!NextTrialReadySource.isPlaying) {
							NextTrialReadySource.Play ();
						}
						if (Input.GetKeyDown (KeyCode.G) || Input.GetButtonDown("Start")) {
							ResetTrial ();
							IsTrialOn = true;
							TrialStartSource.Play ();
							TargetAnnouncementSource.PlayDelayed (1.2f);
						}
					}
				}
			}
		}else if (TaskIndex == 3) {
			if (IsTrialOn && !IsTrialFinished) {
				CalculateError ();
				if (TargetDistance <= TargetThreshold*2f && AimingError <= SpotlightThreshold) {
					IsTrialFinished = true;
					TrialFinishedSource.Play ();
					LogSummary ();
				}else if (TrialTimer >= TrialTimeLimit || Input.GetKeyDown(KeyCode.E)) {
					IsEarlyEnd = true;
					LogSummary ();
					TrialTimer = 0f;
					IsTrialFinished = true;
					TimeOutSource.Play ();
				}
			} else if (IsTrialOn && IsTrialFinished) {
				if (OriginDistance <= OriginThreshold) {
					IsTrialFinished = false;
					IsTrialOn = false;
					TrialIndex = TrialIndex + 1;
					if (TrialIndex == TotalTrialNum) {
						TaskFinishedSource.PlayDelayed (0f);
						IsTaskFinished = true;
					} else {
						NextTrialReadySource.Play ();
					}
				}
			} else if (!IsTrialOn && !IsTrialFinished) {
				if (!IsTaskFinished) {
					if (OriginDistance <= OriginThreshold) {
						if (!NextTrialReadySource.isPlaying) {
							NextTrialReadySource.Play ();
						}
						if (Input.GetKeyDown (KeyCode.G) || Input.GetButtonDown("Start")) {
							ResetTrial ();
							IsTrialOn = true;
							TrialStartSource.Play ();
							TargetAnnouncementSource.PlayDelayed (1.2f);
						}
					}
				}
			}
		}
	}

	// Reset current trial
	public void ResetTrial(){
		switch (TaskIndex) {
		case 0:
			{
				TrialTimer = 0;
				IsSpotlightOn = false;
				IsTrialOn = false;
				IsTrialFinished = false;
				IsEarlyEnd = false;
				TargetIndex = (int)UnityEngine.Random.Range (0, TotalObjNum);
				ListSource = TargetObjects [TargetIndex].GetComponent<AudioSource> ();
				TargetPosition = TargetObjects [TargetIndex].transform.position;
				TargetAnnouncementSource.clip = ListSource.clip;
				break;
			}
		case 1:
			{
				TrialTimer = 0;
				IsSpotlightOn = false;
				IsTrialOn = false;
				IsTrialFinished = false;
				IsEarlyEnd = false;
				TargetIndex = (int)UnityEngine.Random.Range (0, TotalObjNum);
				ListSource = TargetObjects [TargetIndex].GetComponent<AudioSource> ();
				TargetPosition = TargetObjects [TargetIndex].transform.position;
				TargetAnnouncementSource.clip = ListSource.clip;
				break;
			}
		case 2:
			{
				TrialTimer = 0;
				IsTrialOn = false;
				IsTrialFinished = false;
				IsEarlyEnd = false;
				var Angle = (int)UnityEngine.Random.Range (0, 4 - 0.0000001f) * 90f;
				ListSource = TargetObjects [TargetIndex].GetComponent<AudioSource> ();
				ChairParent.transform.eulerAngles = new Vector3 (0f, Angle, 0f);
				TargetPosition = Chair.transform.position;
				TargetAnnouncementSource.clip = ChairSource.clip;
				break;
			}
		case 3:
			{
				TrialTimer = 0;
				IsTrialOn = false;
				IsTrialFinished = false;
				IsEarlyEnd = false;
				var Angle = (int)UnityEngine.Random.Range (0, 4 - 0.0000001f) * 90f;
				ListSource = TargetObjects [TargetIndex].GetComponent<AudioSource> ();
				KeyParent.transform.eulerAngles = new Vector3 (0f, Angle, 0f);
				TargetPosition = Key.transform.position;
				TargetAnnouncementSource.clip = KeySource.clip;
				break;
			}
		default:
			{
				break;
			}
		}
	}
	//Reset current task
	public void ResetTask(int i){
		TaskIndex = i;
		TrialIndex = 0;
		ListIndex = 0;
		IsTrialOn = false;
		IsTrialFinished = false;
		IsExperimentFinished = false;
		IsTaskFinished = false;
		if (i == 0) {
			Studio.transform.localScale = new Vector3 (-1, 1, 1);
			BedPiano.transform.localScale = new Vector3 (-1, 1, 1);
			TVWindow.transform.localScale = new Vector3 (-1, 1, 1);
			Task0Source.Play ();
			TargetIndex = (int)UnityEngine.Random.Range (0, TotalObjNum);
			ListSource = TargetObjects [TargetIndex].GetComponent<AudioSource> ();
			TargetPosition = TargetObjects [TargetIndex].transform.position;
		} else if (i == 1) {
			Studio.transform.localScale = new Vector3 (1, 1, 1);
			BedPiano.transform.localScale = new Vector3 (1, 1, 1);
			TVWindow.transform.localScale = new Vector3 (1, 1, 1);
			Task1Source.Play ();
			TargetIndex = (int)UnityEngine.Random.Range (0, TotalObjNum);
			ListSource = TargetObjects [TargetIndex].GetComponent<AudioSource> ();
			TargetPosition = TargetObjects [TargetIndex].transform.position;
		} else if (i == 2) {
			Studio.transform.localScale = new Vector3 (1, 1, 1);
			BedPiano.transform.localScale = new Vector3 (1, 1, 1);
			TVWindow.transform.localScale = new Vector3 (1, 1, 1);
			var ChairRenderer = Chair.GetComponent<MeshRenderer> ();
			Chair.SetActive (true);
			Task2Source.Play ();
			ResetTrial ();
			TargetPosition = Chair.transform.position;
		}else if (i == 3) {
			Studio.transform.localScale = new Vector3 (1, 1, 1);
			BedPiano.transform.localScale = new Vector3 (1, 1, 1);
			TVWindow.transform.localScale = new Vector3 (1, 1, 1);
			IsSpotlightOn = true;
			var ChairRenderer = Chair.GetComponent<MeshRenderer> ();
			Chair.SetActive (false);
			Task3Source.Play ();
			ResetTrial ();
			TargetPosition = Key.transform.position;
		}
		FileName = FolderPath + System.DateTime.Now.ToString("yy-MM-dd")+ "-" + System.DateTime.Now.ToString("hh-mm-ss") + "_" + TaskIndex  + "_" + FileIndex.ToString() + ".txt";
		recording = new StreamWriter( new FileStream(@FileName,FileMode.Create));
		recording.AutoFlush = true;	
		FileName2 = FolderPath + "Summary" + System.DateTime.Now.ToString("yy-MM-dd")+ "-" + System.DateTime.Now.ToString("hh-mm-ss") + "_" + TaskIndex  + "_" + FileIndex.ToString() + ".txt";
		summary = new StreamWriter( new FileStream(@FileName2,FileMode.Create));
		summary.AutoFlush = true;	
	}
	// Switch task
	public void SwitchTask(int i){
		TaskIndex = i;
		FileIndex = FileIndex + 1;
		ResetTask (i);
	}
	// Sonify Chair
	public void SonifyChair(){
		// If chair is behind the subject, say "turn around"
		CalculateError ();
		if (Math.Abs (AimingError) > BackThreshold) { 
			TurnAroundSource.Play();
			AimingError = 0f;
		} else {
			ChairSource.pitch = Math.Max (1.5f - 0.5f * (float)Math.Log10 (TargetDistance + 1f), 0.5f);
			ChairSource.Play ();
		}
	}
	// Sonify Key
	public void SonifyKey(){
		// If chair is behind the subject, say "turn around"
		AimingError = Vector3.Angle(TargetPosition-Camera.main.transform.position, Camera.main.transform.forward);
		if (Math.Abs (AimingError) > BackThreshold) { 
			TurnAroundSource.Play();
			AimingError = 0f;
		} else {
			KeySource.pitch = Math.Max (1.5f - 0.5f * (float)Math.Log10 (TargetDistance + 1f), 0.5f);
			KeySource.Play ();
		}
	}
	// List Mode Functions 
	// Sonify current object
	public void SonifyCurrent (){
		ListSource.pitch = Math.Max (1.5f - 0.5f * (float)Math.Log10 (TargetDistance + 1f), 0.5f);
		ListSource.Play ();	
	}
	// Choose next object on list
	public void NextOnList (){
		if (TargetIndex == (TotalObjNum - 1)) {
			TargetIndex = -1;
		}
		TargetIndex = TargetIndex + 1;
		TargetPosition = TargetObjects [TargetIndex].transform.position;
		ListSource = TargetObjects [TargetIndex].gameObject.GetComponent<AudioSource> ();
		ListSource.pitch = Math.Max (1.5f - 0.5f * (float)Math.Log10 (TargetDistance + 1f), 0.5f);
		ListSource.Play ();	
	}
	// Choose previous object on list
	public void PreviousOnList (){
		if (TargetIndex == 0) {
			TargetIndex = TotalObjNum;
		}
		TargetIndex = TargetIndex - 1;
		TargetPosition = TargetObjects [TargetIndex].transform.position;
		ListSource = TargetObjects [TargetIndex].gameObject.GetComponent<AudioSource> ();
		ListSource.pitch = Math.Max (1.5f - 0.5f * (float)Math.Log10 (TargetDistance + 1f), 0.5f);
		ListSource.Play ();	
	}
	// Spotlight Mode Functions
	public void Spotlight(){
		if (TaskIndex != 3) {			
			SpotlightObjects = TargetObjects;
			ScanAngle = new float[SpotlightObjects.Length];
		} else {
			SpotlightObjects = new GameObject[1];
			SpotlightObjects[0] = Key;
			ScanAngle = new float[1];
		}
		for (int i = 0; i < SpotlightObjects.Length; i++) {
			ObjFacing = SpotlightObjects [i].transform.position - Camera.main.transform.position;
			ScanAngle [i] = Vector3.Angle (ObjFacing, Camera.main.transform.forward);
		}
		Array.Sort (ScanAngle, SpotlightObjects);
		if (ScanAngle [0] < SpotlightThreshold) {
			if (LastSpotlightObject == SpotlightObjects [0]) {
				SpotlightTimer += SpotlightInterval;
				if (SpotlightTimer > SpotlightInterval) {
					SpotlightTimer = 0f;
					SpotlightSource = SpotlightObjects [0].GetComponent<AudioSource> ();
					SpotlightSource.Play ();
				}
			} else {
				SpotlightTimer = 0f;
				LastSpotlightObject = SpotlightObjects [0];
				SpotlightSource = SpotlightObjects [0].GetComponent<AudioSource> ();
				SpotlightSource.Play ();
			}
		}
	}
	//Calculate Aiming Error
	public void CalculateError(){
		var TgtFacing = TargetPosition - Camera.main.transform.position;
		var TgtFacing2 = new Vector2 (TgtFacing.x, TgtFacing.z);
		AimingError = Vector2.Angle (new Vector2 (Camera.main.transform.forward.x, Camera.main.transform.forward.z), TgtFacing2);
	}
	// Log summary
	public void LogSummary()
	{
		summary.Write (TaskIndex);
		summary.Write (" , ");
		summary.Write (TrialIndex);
		summary.Write (" , ");
		summary.Write (TrialTimer);
		summary.Write (" , ");
		summary.Write (AimingError);
		summary.Write (" , ");
		summary.Write (IsEarlyEnd);
		summary.Write (" , ");
		summary.Write (IsSceneReaderOn);
		summary.WriteLine (" , ");
	}
	// log experiment variables
	public void WriteToFile(){
		RecTimer += Time.deltaTime;
		if (RecTimer > RecordingInterval) {
			recording.Write (Time.deltaTime);
			recording.Write (" , ");
			recording.Write (ExpTimer);
			recording.Write (" , ");
			recording.Write (TrialTimer);
			recording.Write (" , ");
			recording.Write (Camera.main.transform.position.x);
			recording.Write (" , ");
			recording.Write (Camera.main.transform.position.y);
			recording.Write (" , ");
			recording.Write (Camera.main.transform.position.z);
			recording.Write (" , ");
			recording.Write (Camera.main.transform.rotation.eulerAngles.x);
			recording.Write (" , ");
			recording.Write (Camera.main.transform.rotation.eulerAngles.y);
			recording.Write (" , ");
			recording.Write (Camera.main.transform.rotation.eulerAngles.z);
			recording.Write (" , ");
			recording.Write (TargetPosition.x);
			recording.Write (" , ");
			recording.Write (TargetPosition.y);
			recording.Write (" , ");
			recording.Write (TargetPosition.z);
			recording.Write (" , ");
			recording.Write (TaskIndex);
			recording.Write (" , ");
			recording.Write (TrialIndex);
			recording.Write (" , ");
			recording.Write (IsTrialOn);
			recording.Write (" , ");
			recording.Write (IsTrialFinished);
			recording.Write (" , ");
			recording.Write (IsEarlyEnd);
			recording.Write (" , ");
			recording.Write (IsTaskFinished);
			recording.Write (" , ");
			recording.Write (IsSceneReaderOn);
			recording.Write (" , ");
			recording.Write (IsSpotlightOn);
			recording.Write (" , ");
			recording.Write (AimingError);
			recording.Write (" , ");
			recording.Write (KeyPressed);	
			recording.WriteLine (" , ");
			RecTimer = RecTimer - RecordingInterval;
		}
	}
}
