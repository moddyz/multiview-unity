using UnityEngine;
using System.Collections;

/*
 * Multi-view Rendering Control Class 
 **/

public class MultiViewController : MonoBehaviour {

	public GameObject rootCamera;
	public int numberOfViews = 8;
    public float angleOfAttenuator = 18.435f;
	public float horizontalInterval = 8.0f;
	public float interaxialDistance = 0.3f;
	public float focalLength = 10.0f;
	public bool isParallel = false;
	public float parallelShift = 0.0f;
	public bool debugMode = true;
	
	public float optimizedConvergedInteraxial = 0.059f;
	public float optimizedConvergedFocal = 1.89f;
	
	public float optimizedParallelInteraxial = 0.1f;
	public float optimizedParallelShift = 0.02f;
	
	
	// Private Members
	private float aspectRatio;
	private GameObject displayCamera;
	private GameObject projectionPlane;
	private ArrayList renderCameras;
	private ArrayList renderTextures;
	private bool resetSystem = false;
	private bool keyDown;
	
	public GameObject GetDisplayCamera()
	{
		return displayCamera;
	}
	
	public ArrayList GetRenderTextures()
	{
		return renderTextures;
	}
	
	void ConstructRenderSystem ()
	{
		if (numberOfViews < 1)
			numberOfViews = 1;
		
		aspectRatio = (float) Screen.width / (float) Screen.height;
		renderCameras = new ArrayList();
		renderTextures = new ArrayList();
		
		for (int v = 1; v <= numberOfViews; v++)
		{
			GameObject newRenderCam = (GameObject) Instantiate(rootCamera);
			newRenderCam.name = "RenderCam" + v;
			newRenderCam.AddComponent("MultiViewCamera");
			MultiViewCamera mvComponent = newRenderCam.GetComponent<MultiViewCamera>();
			
			// Camera Configuration
			mvComponent.SetController(gameObject.GetComponent<MultiViewController>());
			mvComponent.SetParent(rootCamera);
			mvComponent.SetID(v);
			mvComponent.enabled = true;
			
			// Add to array lists
			renderCameras.Add(newRenderCam);
			print ("Hello WOrld" + numberOfViews);
		}
		
		for (int v = 1; v <= numberOfViews; v++)
		{
			GameObject newRenderCam = (GameObject) renderCameras[v - 1];
			gameObject.SetActive(true);

			// Camera and Texture Construction
			MultiViewCamera mvComponent = newRenderCam.GetComponent<MultiViewCamera>();
			mvComponent.ConstructRenderCamera();
			mvComponent.ConstructRenderTexture(aspectRatio);
			renderTextures.Add(mvComponent.GetTexture());
		}
		
	}
	
	void ConstructDisplaySystem ()
	{
		// Construct Display Camera
		displayCamera = new GameObject("DisplayCam");
		displayCamera.AddComponent("MultiViewDisplay");
		
		MultiViewDisplay displayComponent = displayCamera.GetComponent<MultiViewDisplay>();
		displayComponent.SetController(gameObject.GetComponent<MultiViewController>());
		displayComponent.SetParent(rootCamera);
		displayComponent.ConstructDisplayCamera();
		displayComponent.ConstructProjectionPlane(aspectRatio);
		projectionPlane = displayComponent.projectionPlane;
	}
	
	
	
	void ProcessInput()
	{
		// PROCESS INPUT
		if (keyDown == false)
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				keyDown = true;
				if (debugMode == false)
					debugMode = true;
				else
					debugMode = false;
			}
		}
		if (keyDown == true)
		{
			if (Input.GetKeyUp(KeyCode.Backspace))
			{
				keyDown = false;
			}
		}
		
		if (debugMode == true)
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				if (Input.GetKey(KeyCode.Equals))
					interaxialDistance += 0.1f;
				else if (Input.GetKey(KeyCode.Minus))
					interaxialDistance -= 0.1f;
				else if (Input.GetKey(KeyCode.RightBracket))
					focalLength += 0.1f;
				else if (Input.GetKey(KeyCode.LeftBracket))
					focalLength -= 0.1f;
				else if (Input.GetKey(KeyCode.Quote))
					angleOfAttenuator += 0.05f;
				else if (Input.GetKey(KeyCode.Semicolon))
					angleOfAttenuator -= 0.05f;
				else if (Input.GetKey(KeyCode.Period))
					parallelShift += 0.05f;
				else if (Input.GetKey(KeyCode.Comma))
					parallelShift -= 0.05f;
				else if (Input.GetKey(KeyCode.Alpha9))
					OptimizeConverged();
				else if (Input.GetKey(KeyCode.Alpha0))
					OptimizeParallel();
			}
			else if (Input.GetKey(KeyCode.RightAlt))
			{
				if (Input.GetKey(KeyCode.Equals))
					interaxialDistance += 0.0001f;
				else if (Input.GetKey(KeyCode.Minus))
					interaxialDistance -= 0.0001f;
				else if (Input.GetKey(KeyCode.RightBracket))
					focalLength += 0.0001f;
				else if (Input.GetKey(KeyCode.LeftBracket))
					focalLength -= 0.0001f;
				else if (Input.GetKey(KeyCode.Quote))
					angleOfAttenuator += 0.0001f;
				else if (Input.GetKey(KeyCode.Semicolon))
					angleOfAttenuator -= 0.0001f;
				else if (Input.GetKey(KeyCode.Period))
					parallelShift += 0.0001f;
				else if (Input.GetKey(KeyCode.Comma))
					parallelShift -= 0.0001f;
			}
			else
			{
				if (Input.GetKey(KeyCode.Equals))
					interaxialDistance += 0.001f;
				else if (Input.GetKey(KeyCode.Minus))
					interaxialDistance -= 0.001f;
				else if (Input.GetKey(KeyCode.RightBracket))
					focalLength += 0.001f;
				else if (Input.GetKey(KeyCode.LeftBracket))
					focalLength -= 0.001f;
				else if (Input.GetKey(KeyCode.Quote))
					angleOfAttenuator += 0.001f;
				else if (Input.GetKey(KeyCode.Semicolon))
					angleOfAttenuator -= 0.001f;
				else if (Input.GetKey(KeyCode.Period))
					parallelShift += 0.001f;
				else if (Input.GetKey(KeyCode.Comma))
					parallelShift -= 0.001f;
			}
			
			if (keyDown == false)
			{
				if (Input.GetKeyDown(KeyCode.Slash))
				{
					if (isParallel == false)
						isParallel = true;
					else
						isParallel = false;
				}
				if (Input.GetKeyDown(KeyCode.L))
				{
					numberOfViews += 1;
				}
				if (Input.GetKeyDown(KeyCode.K))
				{
					numberOfViews -= 1;
				}
			}
		}
	}
	
	void OptimizeConverged()
	{
		isParallel = false;
		interaxialDistance = optimizedConvergedInteraxial;
		focalLength = optimizedConvergedFocal;
	}
	
	void OptimizeParallel()
	{
		isParallel = true;
		interaxialDistance = optimizedParallelInteraxial;
		parallelShift = optimizedParallelShift;
	}
	
	void Startup()
	{
		ConstructRenderSystem();
		ConstructDisplaySystem();
		OptimizeConverged();
	}
	
	void Shutdown()
	{	
		// Destroy Camera System
		foreach (GameObject cam in renderCameras)		
		{
			Destroy(cam);
		}
		renderCameras.Clear();
		
		
		foreach (RenderTexture texture in renderTextures)		
		{
			texture.Release();
			Destroy(texture);
		}
		renderTextures.Clear();

		// Destroy Display System
		displayCamera.GetComponent<MultiViewDisplay>().Shutdown();
		Destroy(displayCamera);
	}
	
	public void Reboot()
	{
		Shutdown();
		resetSystem = true;
	}
	
	// Use this for initialization
	void Start () {
		Startup();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (resetSystem == true)
		{
			resetSystem = false;
			Startup();
		}
		if (numberOfViews <= 0)
			numberOfViews = 1;
		
		float newAspectRatio = (float) Screen.width / (float) Screen.height;
		if (newAspectRatio != aspectRatio)
		{
			aspectRatio = newAspectRatio;
			// Update projection plane
			MultiViewDisplay displayComponent = displayCamera.GetComponent<MultiViewDisplay>();
			displayComponent.ResizePlane(aspectRatio);
			for (int c = 0; c < numberOfViews; c++)
			{
				GameObject cam = (GameObject) renderCameras[c];
				MultiViewCamera cameraUnit = cam.GetComponent<MultiViewCamera>();
				cameraUnit.ResizeTexture(aspectRatio);
			}
		}
		
		ProcessInput();
		if (projectionPlane.renderer.material.GetFloat("_Views") != numberOfViews)
		{
			Reboot();
		}
	}
}
