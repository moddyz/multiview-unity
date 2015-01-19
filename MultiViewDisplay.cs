using UnityEngine;
using System.Collections;

public class MultiViewDisplay : MonoBehaviour {
	
	private MultiViewController controller;
	private GameObject parent;
	private GameObject textObject;
	
	private Camera cameraComponent;
	private Material materialComponent;
	public GameObject projectionPlane;
	private Rect textRect;
	
	
	public Material GetMaterialComponent()
	{
		return materialComponent;
	}
	
	public MultiViewController GetController()
	{
		return controller;
	}
	
	public void SetController(MultiViewController newController)
	{
		controller = newController;
	}
	
	public GameObject GetParent()
	{
		return parent;
	}
	
	public void SetParent(GameObject newParent)
	{
		parent = newParent;
	}
	
	void OnGUI()
	{
		if (controller.debugMode == true)
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 18;
			style.normal.textColor = Color.white;
			textRect = new Rect(Screen.width / 100.0f, Screen.width / 100.0f, Screen.width, Screen.height);
			string GUIString = "Number of Views: " + controller.numberOfViews + "\n" +
							   "Interaxial Distance: " + controller.interaxialDistance + "\n" +
							   "Focal Length: " + controller.focalLength + "\n" +
							   "Angle of Attenuator: " + controller.angleOfAttenuator + "\n" +
							   "Parallel Cameras: " + controller.isParallel + "\n" +
							   "Parallel Shift: " + controller.parallelShift;
			GUI.Label(textRect, GUIString, style);
		}
	}
	
	
	public void ConstructDisplayCamera()
	{
		gameObject.AddComponent("Camera");
		cameraComponent = gameObject.GetComponent<Camera>();
		cameraComponent.orthographic = true;
		cameraComponent.depth = 2;
		cameraComponent.orthographicSize = 1;
		cameraComponent.cullingMask = 1 << 31;
	}
	
	public void LinkRenderTexturesToShader()
	{
		ArrayList renderTextures = controller.GetRenderTextures();
		for (var t = 1; t <= controller.numberOfViews; t++)
		{
			Texture tex = (Texture) renderTextures[t - 1];
			materialComponent.SetTexture("_RenderTexture" + t, tex);
		}
	}
	
	public void ConstructProjectionPlane(float aspectRatio)
	{
		
		projectionPlane = new GameObject("ProjectionPlane");
		projectionPlane.AddComponent("MeshFilter");
		MeshFilter meshFilter = projectionPlane.GetComponent<MeshFilter>();
		meshFilter.mesh = QuadMesh.Create();
		projectionPlane.AddComponent("MeshRenderer");
		projectionPlane.layer = 31;
		projectionPlane.transform.parent = transform;
	
		Vector3 tempPos = projectionPlane.transform.localPosition;
		tempPos.z = cameraComponent.nearClipPlane;
		projectionPlane.transform.localPosition = tempPos;
		
		Vector3 tempScale = projectionPlane.transform.localScale;
		tempScale.y = 2.0f;
		tempScale.x = 2.0f * aspectRatio;
		projectionPlane.transform.localScale = tempScale;
	
		//projectionPlane.renderer.material = new Material(Shader.Find("Custom/MultiplexShader"));
		int views = controller.numberOfViews;
		float angle = controller.angleOfAttenuator;
		float hInterval = controller.horizontalInterval;
		Material multiplexer = new Material(Shader.Find("PixelMultiplexer"));
		projectionPlane.renderer.material = multiplexer;
		materialComponent = projectionPlane.renderer.material;
		LinkRenderTexturesToShader();
		UpdateShaderUniforms();
	}
	
	public void ResizePlane(float aspectRatio)
	{
		Vector3 tempScale = projectionPlane.transform.localScale;
		tempScale.y = 2.0f;
		tempScale.x = 2.0f * aspectRatio;
		projectionPlane.transform.localScale = tempScale;
	}
	
	public void Shutdown()
	{
		Destroy(projectionPlane);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	void UpdateShaderUniforms()
	{
		if (controller.numberOfViews <= 0)
			controller.numberOfViews = 1;
		
		if (materialComponent.GetFloat("_Angle") != controller.angleOfAttenuator)
		{
			materialComponent.SetFloat("_Angle", controller.angleOfAttenuator);
		}
		
		float isParallelFloat = 0.0f;
		if (controller.isParallel == true)
			isParallelFloat = 1.0f;
		if (materialComponent.GetFloat("_IsParallel") != isParallelFloat)
		{
			materialComponent.SetFloat("_IsParallel", isParallelFloat);
		}
		
		if (materialComponent.GetFloat("_ParallelShift") != controller.parallelShift)
		{
			materialComponent.SetFloat("_ParallelShift", controller.parallelShift);
		}
		
		if (materialComponent.GetFloat("_HInterval") != controller.horizontalInterval)
		{
			materialComponent.SetFloat("_HInterval", controller.horizontalInterval);
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateShaderUniforms();
	}
}
