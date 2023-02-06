//2016 Spyblood Games

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


[System.Serializable]
public class DayColors
{
	public Color skyColor;
	public Color equatorColor;
	public Color horizonColor;
}

public class DayAndNightControl : MonoBehaviourPunCallbacks {
	public bool StartDay; 
	public string state;
	public GameObject moonState;
	public GameObject moon;
	public GameObject roomLights;
	public DayColors dawnColors;
	public DayColors dayColors;
	public DayColors nightColors;
	public Light directionalLight; 
	public float SecondsInAFullDay = 120f; 
	[Range(0,1)]
	public float currentTime = 0; 
	[HideInInspector]
	public float timeMultiplier = 1f; 
	float lightIntensity; 

	// Use this for initialization
	void Start () {
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
		lightIntensity = directionalLight.intensity; //what's the current intensity of the light
		if (StartDay) {
			currentTime = 0.3f; //start at morning
		}
	}
	
	void Update () {
		if(photonView.IsMine){
			currentTime += (Time.deltaTime / SecondsInAFullDay) * timeMultiplier;
			if (currentTime >= 1) currentTime = 0;
			photonView.RPC("UpdateLight", RpcTarget.All, currentTime);
		}
	}
	[PunRPC]
	void UpdateLight(float phoTime)
	{
		currentTime = phoTime;
		directionalLight.transform.localRotation = Quaternion.Euler ((phoTime * 360f) - 90, 170, 0);
		moonState.transform.localRotation = Quaternion.Euler ((phoTime * 360f) - 100, 170, 0);

		float intensityMultiplier = 1;

		if (phoTime <= 0.23f || phoTime >= 0.75f) intensityMultiplier = 0; 
		else if (phoTime <= 0.25f) intensityMultiplier = Mathf.Clamp01((phoTime - 0.23f) * (1 / 0.02f));
		else if (phoTime <= 0.73f) intensityMultiplier = Mathf.Clamp01(1 - ((phoTime - 0.73f) * (1 / 0.02f)));

		if (phoTime <= 0.2f) {
			state = "midnight";
			RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, nightColors.skyColor,Time.deltaTime);
			RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, nightColors.equatorColor, Time.deltaTime);
			RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, nightColors.horizonColor, Time.deltaTime);
		}
		if (phoTime > 0.2f && phoTime < 0.4f) {
			state = "morning";
			RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, dawnColors.skyColor, Time.deltaTime);
			RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, dawnColors.equatorColor, Time.deltaTime);
			RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, dawnColors.horizonColor, Time.deltaTime);
			for (int i = 0; i< roomLights.transform.childCount; i++){
				roomLights.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		if (phoTime > 0.4f && phoTime < 0.75f) {
			state = "afternoon";
			RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, dayColors.skyColor, Time.deltaTime);
			RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, dayColors.equatorColor, Time.deltaTime);
			RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, dayColors.horizonColor, Time.deltaTime);
		}
		if (phoTime > 0.75f) {
			state = "night";
			RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, dayColors.skyColor, Time.deltaTime);
			RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, dayColors.equatorColor, Time.deltaTime);
			RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, dayColors.horizonColor, Time.deltaTime);
			for (int i = 0; i< roomLights.transform.childCount; i++){
				roomLights.transform.GetChild(i).gameObject.SetActive(true);
			}
		}

		directionalLight.intensity = lightIntensity * intensityMultiplier;
	}

}
