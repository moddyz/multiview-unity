using UnityEngine;
using System.Collections;

public class MultiViewCamera : MonoBehaviour {
	
	private MultiViewController controller;
	private GameObject parent;
	private RenderTexture texture;
	
	private Camera cameraComponent;
	private int ID;
	
	public RenderTexture GetTexture()
	{
		return texture;
	}
	
	public void SetTexture(RenderTexture newTexture)
	{
		texture = newTexture;
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
	
	public int GetID()
	{
		return ID;
	}
	
	public void SetID(int newID)
	{
		ID = newID;
	}
	
	public void ResizeTexture(float aspectRatio)
	{
		cameraComponent.aspect = aspectRatio;
		texture.Release();
		texture.width = (int)(((float) texture.height) * aspectRatio);
		texture.Create();
	}
	
	public void ConstructRenderTexture(float aspectRatio)
	{	
		texture = new RenderTexture((int)(1000 * aspectRatio), 1000, 24);
		texture.Create();	
		cameraComponent.targetTexture = texture;
	}
	
	public void ConstructRenderCamera()
	{
		// Set-up Camera Component
		transform.parent = parent.transform;
		cameraComponent = gameObject.GetComponent<Camera>();
		AudioListener audioComponent = gameObject.GetComponent<AudioListener>();
		Destroy(audioComponent);
		
		// CUSTOM REMOVE COMPONENTS
		Component soldScript = gameObject.GetComponent("SoldierCamera");
		Destroy (soldScript);
		Component dofScript = gameObject.GetComponent("DepthOfField");
		Destroy (dofScript);
		
		Component anim = gameObject.GetComponent("Animation");
		Destroy (anim);
		
		Component camScript = gameObject.GetComponent("FreeCamera");
		Destroy (camScript);
		
		
		cameraComponent.depth = 1;
		cameraComponent.cullingMask = cameraComponent.cullingMask ^ (int) (1 << 31);
		
		// Set-up Transformation Attributes
		float interaxialDistance = controller.interaxialDistance;
		float focalLength = controller.focalLength;
		int numberOfViews = controller.numberOfViews;
		bool isParallel = controller.isParallel;
		
		Vector3 newPos = transform.localPosition;
		newPos.y = 0; newPos.z = 0;
		
		if (numberOfViews < 1)
			controller.numberOfViews = numberOfViews = 1;
		if (numberOfViews > 1)
			newPos.x = interaxialDistance / 2.0f - ((((float)ID - 1.0f) * interaxialDistance / (numberOfViews - 1.0f)));
		else if (numberOfViews == 1)
			newPos.x = 0;
			
		transform.localPosition = newPos;

		Quaternion newRot = transform.localRotation;
		newRot.eulerAngles = Vector3.zero;
		
		if (isParallel == false)
		{
			float angle = Mathf.Atan2(focalLength, transform.localPosition.x) * Mathf.Rad2Deg;
			Vector3 tempEuler = newRot.eulerAngles;
			tempEuler.y = angle - 90.0f;
			newRot.eulerAngles = tempEuler;
			transform.localRotation = newRot;
		}
		else
		{
			transform.localRotation = newRot;
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	
	void Update () {
		float interaxialDistance = controller.interaxialDistance;
		float focalLength = controller.focalLength;
		int numberOfViews = controller.numberOfViews;
		bool isParallel = controller.isParallel;
		
		Vector3 newPos = transform.localPosition;
		if (numberOfViews < 1)
			controller.numberOfViews = numberOfViews = 1;
		if (numberOfViews > 1)
			newPos.x = interaxialDistance / 2.0f - ((((float)ID - 1.0f) * interaxialDistance / (numberOfViews - 1.0f)));
		else if (numberOfViews == 1)
			newPos.x = 0;
		transform.localPosition = newPos;
		
		Quaternion newRot = transform.localRotation;
		newRot.eulerAngles = Vector3.zero;
		
		if (isParallel == false)
		{
			if (numberOfViews > 1)
			{
				float angle = Mathf.Atan2(focalLength, transform.localPosition.x) * Mathf.Rad2Deg;
				Vector3 tempEuler = newRot.eulerAngles;
				tempEuler.y = angle - 90.0f;
				newRot.eulerAngles = tempEuler;
				transform.localRotation = newRot;
			}
			else if (numberOfViews == 1)
				transform.localRotation = Quaternion.Euler(Vector3.zero);
		}
		else
		{
			transform.localRotation = newRot;
		}
	}
}
	
