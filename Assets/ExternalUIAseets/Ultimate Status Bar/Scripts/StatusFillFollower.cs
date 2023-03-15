/* StatusFillFollower.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu( "UI/Ultimate Status Bar/Status Fill Follower" )]
public class StatusFillFollower : MonoBehaviour
{
	// ----- < ULTIMATE STATUS BAR > ----- //
	public UltimateStatusBar ultimateStatusBar;
	public int statusIndex = 0;
	
	// ----- < SCALING AND RATIO > ----- //
	public RectTransform statusBarTransform;
	RectTransform baseTransform;
	public enum ImageAspectRatio
	{
		Preserve, Custom
	}
	public ImageAspectRatio imageAspectRatio = ImageAspectRatio.Custom;
	public Image targetImage;
	public float xRatio = 1.0f, yRatio = 1.0f;
	public enum ScaleDirection{ Height, Width }
	public ScaleDirection scaleDirection = ScaleDirection.Height;
	public float imageSize = 1.0f;

	// ----- < POSITIONS > ----- //
	public Vector2 minimumPosition = Vector3.zero;
	public Vector2 maximumPosition = Vector3.zero;
	Vector2 _minimumPosition = Vector3.zero;
	Vector2 _maximumPosition = Vector3.zero;

	
	void Start ()
	{
		// If the application is not running, then return.
		if( Application.isPlaying == false )
			return;

		// If the Ultimate Status Bar variable is unassigned, then warn the user and return.
		if( ultimateStatusBar == null )
		{
			Debug.LogError( "Status Fill Follower - The Ultimate Status Bar component has not been assigned." );
			return;
		}

		// Subscribe to the OnStatusUpdated function for the targeted Ultimate Status.
		ultimateStatusBar.UltimateStatusList[ statusIndex ].OnStatusUpdated += OnStatusUpdated;

		// Update the positioning.
		UpdatePositioning();

		// Subscribe to the positioning function of the Ultimate Status Bar.
		ultimateStatusBar.OnUpdatePositioning += UpdatePositioning;
	}
	
	/// <summary>
	/// This function gets subscribed to the Ultimate Status OnStatusUpdated function.
	/// </summary>
	/// <param name="currVal">The current percentage to apply to the position of the transform.</param>
	void OnStatusUpdated ( float currVal )
	{
		// If the application is only running in the editor...
		if( Application.isPlaying == false )
		{
			// Check the fill constraints for the Ultimate Status. Since the user can be changing values, if the fill constraints are the same it will throw an error, so return.
			if( ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraintMin == ultimateStatusBar.UltimateStatusList[ statusIndex ].fillConstraintMax )
				return;
		}
		// Lerp the position from the minimum to the maximum by the current value.
		baseTransform.position = Vector3.Lerp( _minimumPosition, _maximumPosition, currVal );
	}
	
	/// <summary>
	/// Returns the Canvas component that is a parent of this game object.
	/// </summary>
	Canvas GetParentCanvas ()
	{
		// Store the current parent.
		Transform parent = transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{ 
			// If there is a Canvas component, return the component.
			if( parent.transform.GetComponent<Canvas>() )
				return parent.transform.GetComponent<Canvas>();

			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}

		// If no Canvas was found on any parents, inform the user and return nothing.
		Debug.LogError( "Status Fill Follower - No Canvas component is attached to the parent gameObjects." );
		return null;
	}

#if UNITY_EDITOR
	void Update ()
	{
		if( Application.isPlaying == false )
			UpdatePositioning();
	}
#endif

	/// <summary>
	/// Updates the size and positioning in relation to the Ultimate Status Bar.
	/// </summary>
	void UpdatePositioning ()
	{
		// If the Ultimate Status Bar variable is unassigned, then return.
		if( ultimateStatusBar == null )
			return;

		// If the baseTransform variable is unassigned...
		if( baseTransform == null )
		{
			// Assign the transform component.
			baseTransform = GetComponent<RectTransform>();

			// If the baseTransform variable is still null...
			if( baseTransform == null )
			{
				// If the application is running, then inform the user.
				if( Application.isPlaying == true )
					Debug.LogError( "Status Fill Follower - There is no RectTransform component attached to this gameObject." );
				return;
			}
		}

		// Attempt to assign the transform.
		statusBarTransform = ultimateStatusBar.GetComponent<RectTransform>();

		// If the Ultimate Status Bar's transform is unassigned...
		if( statusBarTransform == null )
		{
			// If the application is running, then inform the user of the error.
			if( Application.isPlaying == true )
				Debug.LogError( "Status Fill Follower - The Ultimate Status Bar is either not assigned, or does not have a Rect Transform component." );
			return;
		}

		// If the user is wanting to preserve the aspect ratio of the selected image...
		if( imageAspectRatio == ImageAspectRatio.Preserve )
		{
			// If the targetImage variable has been left unassigned, then inform the user and return.
			if( targetImage == null )
			{
				// If the application is running, then inform the user of the error.
				if( Application.isPlaying == true )
					Debug.LogError( "Status Fill Follower - The Target Image has not been assigned." );
				return;
			}
			
			// If the target image does not have a sprite, then return.
			if( targetImage.sprite == null )
				return;

			// Store the raw values of the sprites ratio so that a smaller value can be configured.
			Vector2 rawRatio = new Vector2( targetImage.sprite.rect.width, targetImage.sprite.rect.height );

			// Temporary float to store the largest side of the sprite.
			float maxValue = rawRatio.x > rawRatio.y ? rawRatio.x : rawRatio.y;

			// Now configure the ratio based on the above information.
			xRatio = rawRatio.x / maxValue;
			yRatio = rawRatio.y / maxValue;
		}
		
		// Set the reference size according to the Scale Direction option.
		float referenceSize = scaleDirection == ScaleDirection.Height ? statusBarTransform.sizeDelta.y : statusBarTransform.sizeDelta.x;

		// Configure the size of the image.
		float textureSize = referenceSize * imageSize;

		// Apply the size to the image along with the ratio options.
		baseTransform.sizeDelta = new Vector2( textureSize * xRatio, textureSize * yRatio );

		// Configure the minimum position according to the size of the Ultimate Status Bar.
		Vector3 tempVec = statusBarTransform.position;
		tempVec.x -= ( minimumPosition.x * statusBarTransform.sizeDelta.x );
		tempVec.y -= ( minimumPosition.y * statusBarTransform.sizeDelta.y );
		_minimumPosition = tempVec;

		// Configure the maximum position according to the size of the Ultimate Status Bar.
		Vector3 tempVecFull = statusBarTransform.position;
		tempVecFull.x -= ( maximumPosition.x * statusBarTransform.sizeDelta.x );
		tempVecFull.y -= ( maximumPosition.y * statusBarTransform.sizeDelta.y );
		_maximumPosition = tempVecFull;
		
		#if UNITY_EDITOR
		if( Application.isPlaying == false && UnityEditor.Selection.activeGameObject != gameObject && ultimateStatusBar != null && ultimateStatusBar.UltimateStatusList[ statusIndex ].statusImage != null )
			OnStatusUpdated( ultimateStatusBar.UltimateStatusList[ statusIndex ].GetCurrentCalculatedFraction );
		#endif

		if( Application.isPlaying == true )
			OnStatusUpdated( ultimateStatusBar.UltimateStatusList[ statusIndex ].GetCurrentCalculatedFraction );
	}
}