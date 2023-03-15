/* UltimateStatusBar.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu( "UI/Ultimate Status Bar/Ultimate Status Bar" )]
public class UltimateStatusBar : MonoBehaviour
{
	// ----- < STATUS BAR POSITIONING > ----- //
	RectTransform baseTransform;
	public enum PositioningOption
	{
		Disabled,
		ScreenSpace,
		WorldSpace
	}
	public PositioningOption positioningOption = PositioningOption.Disabled;
	// ----->>> SCREEN SPACE //
	[Serializable]
	public class ScreenSpaceOptions
	{
		public enum ScalingAxis{ Height, Width }
		public ScalingAxis scalingAxis = ScalingAxis.Width;
		public float statusBarSize = 1.75f;
		public enum ImageAspectRatio { Preserve, Custom }
		public ImageAspectRatio imageAspectRatio = ImageAspectRatio.Custom;
		public Image targetImage;
		[Range( 0.0f, 1.0f )]
		public float xRatio = 1.0f, yRatio = 1.0f;
		[Range( 0.0f, 100.0f )]
		public float xPosition = 50.0f, yPosition = 50.0f;

		/// <summary>
		/// This function will configure a Vector2 for the position of the image.
		/// </summary>
		public Vector2 ConfigureImagePosition ( Vector2 baseSize )
		{
			// Create a temporary Vector2 to modify and return.
			Vector2 tempPosVector;
		
			// Fix the custom spacing variables to something that is easy to work with.
			float fixedCSX = xPosition / 100;
			float fixedCSY = yPosition / 100;
			
			// Create two floats for applying our spacers according to our canvas size.
			float positionSpacerX = Screen.width * fixedCSX - ( baseSize.x * fixedCSX );
			float positionSpacerY = Screen.height * fixedCSY - ( baseSize.y * fixedCSY );
		
			// Apple the position spacers to the temporary Vector2.
			tempPosVector.x = positionSpacerX;
			tempPosVector.y = positionSpacerY;
		
			// Return the updated Vector2.
			return tempPosVector;
		}
	}
	public ScreenSpaceOptions screenSpaceOptions = new ScreenSpaceOptions();
	public event Action OnUpdatePositioning;
	// ----->>> WORLD SPACE //
	[Serializable]
	public class WorldSpaceOptions
	{
		public enum FindBy{ Transform, Name, Tag }
		public FindBy findBy = FindBy.Transform;
		public string targetName = "Main Camera";
		public Transform cameraTransform;
	}
	public WorldSpaceOptions worldSpaceOptions = new WorldSpaceOptions();

	// ----- < STATUS BAR OPTIONS > ----- //
	// ----->>> ICON //
	public Image statusBarIcon;
	// ----->>> TEXT //
	public Text statusBarText;
	// ----->>> VISIBILITY //
	public enum UpdateVisibility
	{
		Never,
		Manually,
		OnStatusUpdated
	}
	public UpdateVisibility updateVisibility = UpdateVisibility.Never;
	public enum UpdateUsing
	{
		Fade,
		Animation
	}
	public UpdateUsing updateUsing = UpdateUsing.Fade;
	public float idleSeconds = 2.0f;
	public float enableDuration = 1.0f, disableDuration = 1.0f;
	public float enabledAlpha = 1.0f, disabledAlpha = 0.0f;
	float enabledSpeed = 1.0f, disabledSpeed = 1.0f;
	bool isFading = false, isCountingDown = false;
	float countdownTime = 0.0f;
	CanvasGroup statusBarGroup;
	public Animator statusBarAnimator;
	bool forceVisible = false;
	public bool initialState = true;
	bool _currentState = true;
	public bool GetStatusBarState
	{
		get
		{
			return _currentState;
		}
	}

	// ----- < STATUS INFORMATION > ----- //
	[Serializable]
	public class UltimateStatus
	{
		public UltimateStatusBar statusBar;
		public string statusName = "";
		
		// COLOR OPTIONS //
		public enum ColorMode
		{
			Single,
			Gradient
		}
		public ColorMode colorMode;
		public Image statusImage;
		public Color statusColor = Color.white;
		public Gradient statusGradient = new Gradient();

		// TEXT OPTIONS //
		public enum DisplayText
		{
			Disabled,
			Percentage,
			CurrentValue,
			CurrentAndMaxValues
		}
		public DisplayText displayText;
		public Text statusText;
		public string additionalText = string.Empty;

		// SMOOTH FILL //
		public bool smoothFill = false;
		public float smoothFillDuration = 1.0f;
		public bool isSmoothing = false;
		bool _resetSmoothing = false;
		public bool resetSmoothing
		{
			get
			{
				if( _resetSmoothing == true )
				{
					_resetSmoothing = false;
					return true;
				}
				return _resetSmoothing;
			}
		}

		// FILL CONSTRAINT //
		public bool fillConstraint = false;
		public float fillConstraintMin = 0.0f;
		public float fillConstraintMax = 1.0f;

		// PRIVATE VARIABLES AND GET FUNCTIONS //
		/// <summary>
		/// Returns the percentage value that was calculated when the status was updated. This number will not be current with the Smooth Fill option.
		/// </summary>
		float _currentFraction = 1.0f;
		public float GetCurrentFraction
		{
			get
			{
				return _currentFraction;
			}
		}

		/// <summary>
		/// The stored max value that the user entered.
		/// </summary>
		float _maxValue = 0.0f;
		public float GetMaxValue
		{
			get
			{
				return _maxValue;
			}
		}

		/// <summary>
		/// This float stores the target amount of fill. This value is current with Fill Constraints.
		/// </summary>
		float _targetFill = 0.0f;
		public float GetTargetFill
		{
			get
			{
				return _targetFill;
			}
		}

		/// <summary>
		/// This value calculates the current value of the status. The number is current with the smooth fill option and fill constraints.
		/// </summary>
		public float GetCurrentCalculatedFraction
		{
			get
			{
				if( statusImage == null )
				{
					Debug.LogError( "Ultimate Status Bar - The Status Image component has not been assigned." );
					return 0.0f;
				}
				float tempFloat = 0.0f;
				if( fillConstraint == true )
					tempFloat = ( statusImage.fillAmount - fillConstraintMin ) / ( fillConstraintMax - fillConstraintMin );
				else
					tempFloat = statusImage.fillAmount;

				return tempFloat;
			}
		}

		// SUBSCRIPTIONS //
		public event Action<float> OnStatusUpdated;
		public event Action UpdateStatusBar;
		public event Action UpdateStatusBarVisibility;

		// VISIBILITY CONFIGURATION //
		public bool keepVisible = false;
		public float triggerValue = 0.25f;
		bool _forceVisible = false;
		public bool forceVisible
		{
			get
			{
				return _forceVisible;
			}
		}

		/// <summary>
		/// This function configures which function to update on the Ultimate Status Bar for visibility.
		/// </summary>
		void ConfigureVisibility ()
		{
			// If the application is not playing, then no visibility options should be applied.
			if( Application.isPlaying == false )
				return;

			// If the user does not want this Ultimate Status to keep the Ultimate Status Bar visible...
			if( keepVisible == false )
			{
				// Then just inform the Ultimate Status Bar that the status has simply been updated.
				if( UpdateStatusBar != null )
					UpdateStatusBar();
				return;
			}

			// If the current fraction is less than the trigger, and forceVisible is false...
			if( GetCurrentCalculatedFraction <= triggerValue && _forceVisible == false )
			{
				// Set forceVisible to true so the Ultimate Status Bar knows and then inform the Ultimate Status Bar that a change to the visibility may be needed.
				_forceVisible = true;
				if( UpdateStatusBarVisibility != null )
					UpdateStatusBarVisibility();
				return;
			}

			// If the current fraction is greater than the trigger and forceVisible is currently true...
			if( GetCurrentCalculatedFraction > triggerValue && _forceVisible == true )
			{
				// Set forceVisible to false so the Ultimate Status Bar knows and then inform the Ultimate Status Bar that a change may be needed.
				_forceVisible = false;
				if( UpdateStatusBarVisibility != null )
					UpdateStatusBarVisibility();
				return;
			}

			// If nothing else has triggered, then simply update the status bar.
			if( UpdateStatusBar != null )
				UpdateStatusBar();
		}

		/// <summary>
		/// Displays the text.
		/// </summary>
		void DisplayTextHandler ()
		{
			// If the user does not want text to be displayed, or the text component is null, then return.
			if( displayText == DisplayText.Disabled || statusText == null )
				return;

			// Switch statement for the displayText option. Each option will display the correct text for the set option.
			switch( displayText )
			{
				case DisplayText.Percentage:
				{
					statusText.text = additionalText + ( GetCurrentCalculatedFraction * 100 ).ToString( "F0" ) + "%";
				}break;
				case DisplayText.CurrentValue:
				{
					statusText.text = additionalText + ( GetCurrentCalculatedFraction * _maxValue ).ToString( "F0" );
				}break;
				case DisplayText.CurrentAndMaxValues:
				{
					statusText.text = additionalText + ( GetCurrentCalculatedFraction * _maxValue ).ToString( "F0" ) + " / " + _maxValue.ToString();
				}break;
			}
		}

		/// <summary>
		/// Update the color of the status according to the gradient.
		/// </summary>
		void UpdateStatusGradient ()
		{
			// If the color mode is set to Gradient, then apply the current gradient color.
			if( colorMode == ColorMode.Gradient )
				statusImage.color = statusGradient.Evaluate( GetCurrentCalculatedFraction );
		}

		/// <summary>
		/// Updates the status bar with the current and max values.
		/// </summary>
		/// <param name="currentValue">The current value of the status.</param>
		/// <param name="maxValue">The maximum value of the status.</param>
		public void UpdateStatus ( float currentValue, float maxValue )
		{
			// If the status bar is left unassigned, then return.
			if( statusImage == null )
				return;
			
			// Fix the value to be a percentage.
			_currentFraction = currentValue / maxValue;

			// If the value is greater than 1 or less than 0, then fix the values to being min/max.
			if( _currentFraction < 0 || _currentFraction > 1 )
				_currentFraction = _currentFraction < 0 ? 0 : 1;

			// Store the target amount of fill according to the users options.
			_targetFill = fillConstraint == true ? Mathf.Lerp( fillConstraintMin, fillConstraintMax, _currentFraction ) : _currentFraction;

			// Store the values so that other functions used can reference the maxValue.
			_maxValue = maxValue;

			// If the user is not using the Smooth Fill option, or this function is being called inside Edit Mode...
			if( smoothFill == false || Application.isPlaying == false )
			{
				// Then just apply the target fill amount.
				statusImage.fillAmount = _targetFill;

				// Call the functions for the options.
				UpdateStatusOptions();
			}
			// Else the user is using Smooth Fill while in Play Mode.
			else
			{
				// If the Smooth Fill function is already running, then just reset the values.
				if( isSmoothing == true )
					_resetSmoothing = true;
				// Else Start the Coroutine to show the smooth fill.
				else
					statusBar.StartCoroutine( statusBar.SmoothFill( this ) );
			}
		}

		/// <summary>
		/// Updates the color of the status with the target color.
		/// </summary>
		/// <param name="targetColor">The target color to apply to the status bar.</param>
		public void UpdateStatusColor ( Color targetColor )
		{
			// If the color is not set to single, then return.
			if( colorMode != ColorMode.Single || statusImage == null )
				return;

			// Set the status color to the new target color and apply it to the status bar.
			statusColor = targetColor;
			statusImage.color = statusColor;
		}

		/// <summary>
		/// Updates the color of the text associated with the status bar.
		/// </summary>
		/// <param name="targetColor">The target color to apply to the status text component.</param>
		public void UpdateStatusTextColor ( Color targetColor )
		{
			// If the user is not wanting the text to be displayed, or the text component is not assigned, then return.
			if( displayText == DisplayText.Disabled || statusText == null)
				return;

			// Set the text color to the new target color and apply it to the text component.
			statusText.color = targetColor;
		}

		/// <summary>
		/// Updates the status options.
		/// </summary>
		public void UpdateStatusOptions ()
		{
			UpdateStatusGradient();
			DisplayTextHandler();

			if( OnStatusUpdated != null )
				OnStatusUpdated( GetCurrentCalculatedFraction );

			ConfigureVisibility();
		}
	}
	public List<UltimateStatus> UltimateStatusList = new List<UltimateStatus>();
	public Dictionary<string,UltimateStatus> UltimateStatusDict = new Dictionary<string,UltimateStatus>();
	static Dictionary<string, UltimateStatus> GlobalUltimateStatus = new Dictionary<string, UltimateStatus>();

	// ----- < SCRIPT REFERENCE > ----- //
	static Dictionary<string, UltimateStatusBar> UltimateStatusBars = new Dictionary<string, UltimateStatusBar>();
	public string statusBarName = string.Empty;


	void Awake ()
	{
		// If the application is not playing, then return.
		if( Application.isPlaying == false )
			return;

		// If the statusBarName is assigned...
		if( statusBarName != string.Empty )
		{
			// Check to see if the UltimateStatusBars dictionary already contains this name, and if so, remove the current one.
			if( UltimateStatusBars.ContainsKey( statusBarName ) )
				UltimateStatusBars.Remove( statusBarName );

			// Register this UltimateStatusBar into the dictionary.
			UltimateStatusBars.Add( statusBarName, GetComponent<UltimateStatusBar>() );

			// This loops through each of the UltimateStatus created on this game object.
			for( int i = 0; i < UltimateStatusList.Count; i++ )
			{
				// If the targeted name is nothing, then continue the loop without registering it.
				if( UltimateStatusList[ i ].statusName == string.Empty )
					continue;

				// Check to see if the GlobalUltimateStatus dictionary already contains this name, and if so, remove the current one.
				if( GlobalUltimateStatus.ContainsKey( statusBarName + UltimateStatusList[ i ].statusName ) )
					GlobalUltimateStatus.Remove( statusBarName + UltimateStatusList[ i ].statusName );

				// Register the Ultimate Status variable into the dictionary.
				GlobalUltimateStatus.Add( statusBarName + UltimateStatusList[ i ].statusName, UltimateStatusList[ i ] );
			}
		}

		// This loops through each of the Ultimate Status that is in this component.
		for( int i = 0; i < UltimateStatusList.Count; i++ )
		{
			// Assign the statusBar variable to this component.
			UltimateStatusList[ i ].statusBar = this;
			
            // If the statusName variable is assigned, then register it to the local dictionary.
            if ( UltimateStatusList[ i ].statusName != string.Empty )
				UltimateStatusDict.Add( UltimateStatusList[ i ].statusName, UltimateStatusList[ i ] );

			// If the user wants to update the visibility using the OnStatusUpdated option...
			if( updateVisibility == UpdateVisibility.OnStatusUpdated )
			{
				// Subscribe to each of the needed events to update the visibility.
				UltimateStatusList[ i ].UpdateStatusBar += UpdateStatusBar;
				UltimateStatusList[ i ].UpdateStatusBarVisibility += UpdateStatusBarVisibility;
			}
		}

		// If the user is wanting to update the visibility of the Ultimate Status Bar using fade...
		if( updateVisibility != UpdateVisibility.Never && updateUsing == UpdateUsing.Fade )
		{
			// Get the CanvasGroup component
			statusBarGroup = GetComponent<CanvasGroup>();

			// If the Canvas Group is null, then add the component and assign the variable again.
			if( statusBarGroup == null )
			{
				gameObject.AddComponent( typeof( CanvasGroup ) );
				statusBarGroup = GetComponent<CanvasGroup>();
			}

			// Configure the different fade speeds.
			enabledSpeed = 1.0f / enableDuration;
			disabledSpeed = 1.0f / disableDuration;

			// Set the current state to the initial state value.
			_currentState = initialState;

			// If the current state is false, then set the canvas group to the disabled alpha.
			if( _currentState == false )
				statusBarGroup.alpha = disabledAlpha;
			// Else apply the enabled alpha.
			else
				statusBarGroup.alpha = enabledAlpha;
			
			// If the user wants to update the visibility when the status has been updated, and the initial state is true, then start the countdown.
			if( updateVisibility == UpdateVisibility.OnStatusUpdated && initialState == true )
				StartCoroutine( "ShowStatusBarCountdown" );
		}
		// Else if the user is wanting to update the visibility using animations...
		else if( updateVisibility != UpdateVisibility.Never && updateUsing == UpdateUsing.Animation )
		{
			// Show the status bar.
			ShowStatusBar();

			// If the user wants to update the visibility when the status has been updated, and the initial state is true, then start the countdown.
			if( updateVisibility == UpdateVisibility.OnStatusUpdated && initialState == true )
				StartCoroutine( "ShowStatusBarCountdown" );
		}
	}

	void Start ()
	{
		// If the game isn't running, then this is still within the editor, so return.
		if( Application.isPlaying == false )
			return;

		// If the user wants to use the positioning of this script...
		if( positioningOption == PositioningOption.ScreenSpace )
		{
			// If the parent canvas does not have an Updater component, then add one so that this will update when the screen size changes.
			if( !GetParentCanvas().GetComponent<UltimateStatusBarScreenSizeUpdater>() )
				GetParentCanvas().gameObject.AddComponent( typeof( UltimateStatusBarScreenSizeUpdater ) );
			
			// Call UpdatePositioning() to apply the users positioning options on Start().
			UpdatePositioning();
		}
		// Else if the user is wanting to follow the rotation of a camera...
		else if( positioningOption == PositioningOption.WorldSpace )
		{
			// Then set the baseTransform to the parent canvas component and start the coroutine.
			baseTransform = GetParentCanvas().GetComponent<RectTransform>();
			StartCoroutine( "FollowCameraRotation" );
		}
	}

	/// <summary>
	/// This function will only run when the application is not running.
	/// Additionally, it will never run when being used within a built
	/// application. The #if UNITY_EDITOR will not allow the Update
	/// function to run at all.
	/// </summary>
	#if UNITY_EDITOR
	void Update ()
	{
		if( Application.isPlaying == true )
			return;

		if( positioningOption == PositioningOption.ScreenSpace )
			UpdatePositioning();
	}
	#endif

	/// <summary>
	/// This function is subscribed to the UltimateStatus class function UpdateStatusBar. This function will be called each time that a status is updated. Therefore, even if the certain status will not keep the status bar visible, it can still update the visibility when it has been modified.
	/// </summary>
	void UpdateStatusBar ()
	{
		// If the user is not using the OnStatusUpdated option, or the Ultimate Status Bar is already forcing visible, then return.
		if( updateVisibility != UpdateVisibility.OnStatusUpdated || forceVisible == true )
			return;
		
		// If the countdown is already running, then reset the countdown time.
		if( isCountingDown == true )
			countdownTime = idleSeconds;
		// Else start the countdown.
		else
			StartCoroutine( "ShowStatusBarCountdown" );

		// If the status bar is currently fading out...
		if( isFading == true && _currentState == false )
		{
			// Then set isFading to false, and stop the coroutine.
			isFading = false;
			StopCoroutine( "FadeOutHandler" );
		}
		// Show the status bar.
		ShowStatusBar();
	}

	/// <summary>
	/// This function is subscribed to the UltimateStatus class function: UpdateStatusBarVisibility. This function is only called when the ultimate status has triggered it's keep visible value.
	/// </summary>
	void UpdateStatusBarVisibility ()
	{
		// If the user doesn't have the timeout option enabled, return.
		if( updateVisibility != UpdateVisibility.OnStatusUpdated )
			return;

		// Loop through each Ultimate Status...
		for( int i = 0; i < UltimateStatusList.Count; i++ )
		{
			// If the current status has the keepVisible option enabled, and it is currently wanted to force the visibility...
			if( UltimateStatusList[ i ].keepVisible == true && UltimateStatusList[ i ].forceVisible == true )
			{
				// Stop counting down.
				isCountingDown = false;
				StopCoroutine( "ShowStatusBarCountdown" );

				// Show the status bar.
				ShowStatusBar();

				// Set forceVisible to true and return.
				forceVisible = true;
				return;
			}
		}

		// Set force visible to false.
		forceVisible = false;

		// Start the countdown and show the status bar.
		StartCoroutine( "ShowStatusBarCountdown" );
		ShowStatusBar();
	}

	/// <summary>
	/// This function handles the fading in of the Ultimate Status Bar.
	/// </summary>
	IEnumerator FadeInHandler ()
	{
		// Set isFading to true so that other functions will know that this coroutine is running.
		isFading = true;

		// Store the current value of the Canvas Group's alpha.
		float currentAlpha = statusBarGroup.alpha;

		// Loop for the duration of the enabled duration variable.
		for( float t = 0.0f; t < 1.0f && isFading == true; t += Time.deltaTime * enabledSpeed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( enabledSpeed ) )
				break;

			// Apply the alpha to the CanvasGroup.
			statusBarGroup.alpha = Mathf.Lerp( currentAlpha, enabledAlpha, t );
			yield return null;
		}
		// If the coroutine was not interrupted, then apply the final value.
		if( isFading == true )
			statusBarGroup.alpha = enabledAlpha;

		// Set isFading to false so that other functions know that this coroutine is not running anymore.
		isFading = false;
	}
	
	/// <summary>
	/// For details on this coroutine, see the FadeInHandler() function above.
	/// </summary>
	IEnumerator FadeOutHandler ()
	{
		isFading = true;
		float currentAlpha = statusBarGroup.alpha;
		for( float t = 0.0f; t < 1.0f && isFading == true; t += Time.deltaTime * disabledSpeed )
		{
			if( float.IsInfinity( disabledSpeed ) )
				break;

			statusBarGroup.alpha = Mathf.Lerp( currentAlpha, disabledAlpha, t );

			yield return null;
		}
		if( isFading == true )
			statusBarGroup.alpha = disabledAlpha;

		isFading = false;
	}

	/// <summary>
	/// This function is used as a local controlled Update function for count down the time that this status bar has been idle.
	/// </summary>
	IEnumerator ShowStatusBarCountdown ()
	{
		// Set isCountingDown to true for checks.
		isCountingDown = true;

		// Set the starting time.
		countdownTime = idleSeconds;

		// If the current state is false, then add the duration of the enable to make idle time correct.
		if( _currentState == false )
			countdownTime += enableDuration;

		// While the countdownTime is greater than zero, continue counting down.
		while( countdownTime > 0 )
		{
			countdownTime -= Time.deltaTime;
			yield return null;
		}

		// Once the countdown is complete, set isCountingDown to false and hide the status bar.
		isCountingDown = false;
		HideStatusBar();
	}

	/// <summary>
	/// Shows the status bar.
	/// </summary>
	public void ShowStatusBar ()
	{
		// If the current state is already true, or the user does not want to update the visibility ever, then return.
		if( _currentState == true || updateVisibility == UpdateVisibility.Never )
			return;

		// Set the current state to true.
		_currentState = true;
		
		// If the user is wanting to update the visibility using a Canvas Group fade...
		if( updateUsing == UpdateUsing.Fade )
		{
			// If there is no CanvasGroup, then return.
			if( statusBarGroup == null )
				return;

			// If the status bar is currently fading, then stop the FadeOutHandler.
			if( isFading == true )
				StopCoroutine( "FadeOutHandler" );

			// Start the Fade In routine.
			StartCoroutine( "FadeInHandler" );
		}
		// Else, update the animator.
		else
		{
			if( statusBarAnimator != null )
				statusBarAnimator.SetBool( "BarActive", true );
		}
	}

	/// <summary>
	/// Hides the status bar.
	/// </summary>
	public void HideStatusBar ()
	{
		// If the current state is already false, or the user does not want to update the visibility ever, then return.
		if( _currentState == false || updateVisibility == UpdateVisibility.Never )
			return;

		// Set the current state to false.
		_currentState = false;

		// If the user is wanting to update the visibility using a Canvas Group fade...
		if( updateUsing == UpdateUsing.Fade )
		{
			// If the statusBarGroup isn't assigned, return.
			if( statusBarGroup == null )
				return;

			// If the status bar is currently fading, then stop the coroutine.
			if( isFading == true )
				StopCoroutine( "FadeInHandler" );

			// Start the Fade Out routine.
			StartCoroutine( "FadeOutHandler" );
		}
		// Else, update the animator.
		else
		{
			if( statusBarAnimator != null )
				statusBarAnimator.SetBool( "BarActive", false );
		}
	}

	/// <summary>
	/// Smoothly transitions from the current fill amount to the target fill amount.
	/// </summary>
	/// <param name="status">The Ultimate Status variables to use for the smooth fill.</param>
	public IEnumerator SmoothFill ( UltimateStatus status )
	{
		// Set the referenced Ultimate Status isSmoothing to true so that it knows.
		status.isSmoothing = true;

		// Configure the speed, as well as the current fill of the image.
		float speed = 1.0f / status.smoothFillDuration;
		float currentFill = status.statusImage.fillAmount;
		
		// Loop for the duration of the smooth fill.
		for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed )
		{
			// If the referenced Ultimate Status needs to be reset...
			if( status.resetSmoothing == true )
			{
				// Reconfigure the current fill and reset t.
				currentFill = status.statusImage.fillAmount;
				t = 0.0f;
			}

			// Lerp the fill amount from the current fill to the target.
			status.statusImage.fillAmount = Mathf.Lerp( currentFill, status.GetTargetFill, t );

			// Call the UpdateStatusOptions each frame that this status is updated to update options.
			status.UpdateStatusOptions();

			yield return null;
		}
		// If the Ultimate Status is not needing to reset the smoothing, then finalize the fill to be the stored target fill.
		if( status.resetSmoothing == false )
			status.statusImage.fillAmount = status.GetTargetFill;
		// Else, restart this coroutine.
		else
			StartCoroutine( SmoothFill( status ) );
		
		// Call the UpdateStatusOptions each frame that this status is updated to update options.
		status.UpdateStatusOptions();

		// Set isSmoothing to false so that the script knows that this coroutine is not running any more.
		status.isSmoothing = false;
	}

	/// <summary>
	/// This function acts as an Update function to follow the rotation of the targeted camera.
	/// </summary>
	IEnumerator FollowCameraRotation ()
	{
		// This loop will continue until specified to stop.
		while( true )
		{
			// If the camera transform is null, then find the camera according to the user's options.
			if( worldSpaceOptions.cameraTransform == null )
				worldSpaceOptions.cameraTransform = FindCamera();
			// Else, make the canvas look at the camera.
			else
				baseTransform.LookAt( baseTransform.position + worldSpaceOptions.cameraTransform.rotation * Vector3.forward, worldSpaceOptions.cameraTransform.rotation * Vector3.up );

			yield return null;
		}
	}

	/// <summary>
	/// This function will return the targeted camera according to the users options.
	/// </summary>
	Transform FindCamera ()
	{
		// Create a temporary transform component.
		Transform tempTrans = null;
		
		// If the user is wanting to find the camera by name, then use the GameObject.Find function.
		if( worldSpaceOptions.findBy == WorldSpaceOptions.FindBy.Name && GameObject.Find( worldSpaceOptions.targetName ) )
			tempTrans = GameObject.Find( worldSpaceOptions.targetName ).GetComponent<Transform>();
		// Else if the user is wanting to find the camera by tag, then use the GameObject.FindGameObjectWithTag function.
		else if( worldSpaceOptions.findBy == WorldSpaceOptions.FindBy.Tag && GameObject.FindGameObjectWithTag( worldSpaceOptions.targetName ) )
			tempTrans = GameObject.FindGameObjectWithTag( worldSpaceOptions.targetName ).GetComponent<Transform>();

		// If the temporary transform is still null, then inform the user.
		if( tempTrans == null )
			Debug.LogError( "Ultimate Status Bar - Could not locate the targeted camera for the gameObject: " + gameObject.name + ". Please make sure that the Transform/Name/Tag is correct for the FindBy type." );

		return tempTrans;
	}

	/// <summary>
	/// This function is used to find the Canvas component located on the parent game objects.
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
		Debug.LogError( "Ultimate Status Bar - There is no Canvas component attached to any of the parent gameObjects." );
		return null;
	}

	/* --------------------------------------------- <<< PUBLIC FUNCTIONS FOR THE USER >>> --------------------------------------------- */
	/// <summary>
	/// Updates the size and positioning of the Ultimate Status Bar on the screen.
	/// </summary>
	public void UpdatePositioning ()
	{
		// If the user is not using the Screen Space Positioning Option, then return.
		if( positioningOption != PositioningOption.ScreenSpace )
			return;

		// If the base transform is null...
		if( baseTransform == null )
		{
			// Then get the RectTransform component on this game object.
			baseTransform = GetComponent<RectTransform>();

			// If the base transform is still null...
			if( baseTransform == null )
			{
				// Then inform the user that this gameobject does not have a RectTransform component, and return.
				Debug.LogError( "Ultimate Status Bar - There is no RectTransform on this game object." );
				return;
			}
		}
		
		// If the user is wanting to preserve the aspect ratio of the selected image...
		if( screenSpaceOptions.imageAspectRatio == ScreenSpaceOptions.ImageAspectRatio.Preserve )
		{
			// If the targetImage variable has been left unassigned, then inform the user and return.
			if( screenSpaceOptions.targetImage == null )
			{
				if( Application.isPlaying == true )
					Debug.LogError( "Ultimate Status Bar - The Target Image component has not been assigned." );
				return;
			}
			
			// If the target image does not have a sprite, then return.
			if( screenSpaceOptions.targetImage.sprite == null )
				return;

			// Store the raw values of the sprites ratio so that a smaller value can be configured.
			Vector2 rawRatio = new Vector2( screenSpaceOptions.targetImage.sprite.rect.width, screenSpaceOptions.targetImage.sprite.rect.height );

			// Temporary float to store the largest side of the sprite.
			float maxValue = rawRatio.x > rawRatio.y ? rawRatio.x : rawRatio.y;

			// Now configure the ratio based on the above information.
			screenSpaceOptions.xRatio = rawRatio.x / maxValue;
			screenSpaceOptions.yRatio = rawRatio.y / maxValue;
		}

		// Store the calculation value of either Height or Width.
		float referenceSize = screenSpaceOptions.scalingAxis == ScreenSpaceOptions.ScalingAxis.Height ? Screen.height : Screen.width;

		// Configure a size for the image based on the Canvas's size and scale.
		float textureSize = referenceSize * ( screenSpaceOptions.statusBarSize / 10 );

		// Apply the configured size to the controllerTransform's sizeDelta.
		baseTransform.sizeDelta = new Vector2( textureSize * screenSpaceOptions.xRatio, textureSize * screenSpaceOptions.yRatio );

		// Configure the images pivot space so that the image will be in the correct position regardless of the pivot set by the user.
		Vector2 pivotSpacer = new Vector2( baseTransform.sizeDelta.x * baseTransform.pivot.x, baseTransform.sizeDelta.y * baseTransform.pivot.y );

		// Configure the position of the image according to the information that was gathered above.
		Vector2 imagePosition = screenSpaceOptions.ConfigureImagePosition( new Vector2( baseTransform.sizeDelta.x, baseTransform.sizeDelta.y ) );

		// Apply the positioning.
		baseTransform.position = imagePosition + pivotSpacer;

		if( OnUpdatePositioning != null )
			OnUpdatePositioning();
	}

	/// <summary>
	/// Updates the Ultimate Status Bar icon with the new icon sprite.
	/// </summary>
	/// <param name="newIcon">The new Sprite to be applied to the icon image.</param>
	public void UpdateStatusBarIcon ( Sprite newIcon )
	{
		// If the statusBarIcon component is not assigned...
		if( statusBarIcon == null )
		{
			// Inform the user of the error and return.
			Debug.LogError( "Ultimate Status Bar - The Status Bar Icon variable has not been assigned." );
			return;
		}

		// Apply the new sprite to the icon image.
		statusBarIcon.sprite = newIcon;
	}

	/// <summary>
	/// Updates the Ultimate Status Bar text component with the new text.
	/// </summary>
	/// <param name="newText">The new text to apply to the text component.</param>
	public void UpdateStatusBarText ( string newText )
	{
		// If the statusBarText component is not assigned...
		if( statusBarText == null )
		{
			// Inform the user of the error and return.
			Debug.LogError( "Ultimate Status Bar - The Status Bar Text variable has not been assigned." );
			return;
		}

		// Apply the new text to the status text.
		statusBarText.text = newText;
	}

	/// <summary>
	/// Updates the default status on the local Ultimate Status Bar.
	/// </summary>
	/// <param name="currentValue">The current value of the status.</param>
	/// <param name="maxValue">The max value of the status.</param>
	public void UpdateStatus ( float currentValue, float maxValue )
	{
		// Call the UpdateStatus function on the default UltimateStatus.
		UltimateStatusList[ 0 ].UpdateStatus( currentValue, maxValue );
	}

	/// <summary>
	/// Updates the Ultimate Status that is registered with the targeted status name.
	/// </summary>
	/// <param name="statusName">The name of the targeted Ultimate Status.</param>
	/// <param name="currentValue">The current value of the status.</param>
	/// <param name="maxValue">The max value of the status.</param>
	public void UpdateStatus ( string statusName, float currentValue, float maxValue )
	{
		// If an Ultimate Status has not been registered with the statusName parameter, then return.
		if( !UltimateStatusRegistered( statusName ) )
			return;

		// Update the Ultimate Status that has been registered with the statusName.
		UltimateStatusDict[ statusName ].UpdateStatus( currentValue, maxValue );
	}
	
	/// <summary>
	/// Returns the Ultimate Status registered with the targeted status name.
	/// </summary>
	/// <param name="statusName">The name of the targeted Ultimate Status.</param>
	public UltimateStatus GetUltimateStatus ( string statusName )
	{
		// If an Ultimate Status has not been registered with the statusName parameter, then return null.
		if( !UltimateStatusRegistered( statusName ) )
			return null;

		// Return the Ultimate Status that has been registered with the statusName parameter.
		return UltimateStatusDict[ statusName ];
	}

	/// <summary>
	/// Returns the result of the Ultimate Status being registered to the local dictionary.
	/// </summary>
	bool UltimateStatusRegistered ( string statusName )
	{
		// If the Ultimate Status Dictionary contains the statusName, return true.
		if( UltimateStatusDict.ContainsKey( statusName ) )
			return true;

		// Debug a warning to the user so they know that the status they are trying to reference has not been registered, and then return false.
		Debug.LogWarning( "Ultimate Status Bar - No status has been registered with the name: " + statusName + ". Please make sure that the reference is correct and that this status has actually been created on the targeted Ultimate Status Bar." );
		return false;
	}
	/* ------------------------------------------- <<< END PUBLIC FUNCTIONS FOR THE USER >>> ------------------------------------------- */
	
	/* --------------------------------------------- <<< STATIC FUNCTIONS FOR THE USER >>> --------------------------------------------- */
	/// <summary>
	/// Updates the targeted Ultimate Status registered on the targeted Ultimate Status Bar, with the current and max values.
	/// </summary>
	/// <param name="statusBarName">The name of the targeted Ultimate Status Bar.</param>
	/// <param name="statusName">The name of the targeted Ultimate Status.</param>
	/// <param name="currentValue">The current value of the status.</param>
	/// <param name="maxValue">The maximum value of the status.</param>
	static public void UpdateStatus ( string statusBarName, string statusName, float currentValue, float maxValue )
	{
		// If an Ultimate Status Bar + Ultimate Status has not been registered with the statusBarName + statusName parameter, then return.
		if( !GlobalUltimateStatusBarRegistered( statusBarName, statusName ) )
			return;

		// Update the Ultimate Status with the current and max values.
		GlobalUltimateStatus[ statusBarName + statusName ].UpdateStatus( currentValue, maxValue );
	}

	static bool GlobalUltimateStatusRegistered ( string statusName )
	{
		if( GlobalUltimateStatus.ContainsKey( statusName ) )
			return true;

		// Debug a warning to the user so they know that the status they are trying to reference has not been registered, and then return false.
		Debug.LogWarning( "The status name: " + statusName + " has not been registered. Please make sure that your references are correct before proceeding." );
		return false;
	}

	/// <summary>
	/// Returns the result of the Ultimate Status being registered with the Ultimate Status Bar name in the dictionary.
	/// </summary>
	static bool GlobalUltimateStatusBarRegistered ( string statusBarName, string statusName )
	{
		// If an Ultimate Status Bar + Ultimate Status has been registered with the statusBarName + statusName parameter, then return true.
		if( GlobalUltimateStatus.ContainsKey( statusBarName + statusName ) )
			return true;

		// Debug a warning to the user so they know that the status they are trying to reference has not been registered, and then return false.
		Debug.LogWarning( "The status name: " + statusName + " has not been registered inside the Ultimate Status Bar: " + statusBarName + ". Please make sure that your references are correct before proceeding." );
		return false;
	}

	/// <summary>
	/// Returns the Ultimate Status Bar registered with the status name.
	/// </summary>
	/// <param name="statusBarName">The name of the targeted Ultimate Status Bar.</param>
	static public UltimateStatusBar GetUltimateStatusBar ( string statusBarName )
	{
		// If an Ultimate Status Bar has not been registered with the statusBarName parameter, then return null.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return null;

		// Return the Ultimate Status Bar that has been registered with the statusBarName parameter.
		return UltimateStatusBars[ statusBarName ];
	}

	/// <summary>
	/// Updates the targeted Ultimate Status Bar icon with the new icon sprite.
	/// </summary>
	/// <param name="statusBarName">The name of the targeted Ultimate Status Bar.</param>
	/// <param name="newIcon">The new Sprite to be applied to the icon image.</param>
	static public void UpdateStatusBarIcon ( string statusBarName, Sprite newIcon )
	{
		// If an Ultimate Status Bar has not been registered with the statusBarName parameter, then return.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		// Update the status bar icon with the newIcon parameter.
		UltimateStatusBars[ statusBarName ].UpdateStatusBarIcon( newIcon );
	}

	/// <summary>
	/// Updates the targeted Ultimate Status Bar text component with the new text.
	/// </summary>
	/// <param name="statusBarName">The name of the targeted Ultimate Status Bar.</param>
	/// <param name="newText">The new text to apply to the text component.</param>
	static public void UpdateStatusBarText ( string statusBarName, string newText )
	{
		// If an Ultimate Status Bar has not been registered with the statusBarName parameter, then return.
		if( !UltimateStatusBarRegistered( statusBarName ) )
			return;

		// Update the status bar text with the newText.
		UltimateStatusBars[ statusBarName ].UpdateStatusBarText( newText );
	}

	/// <summary>
	/// Returns the results of the Ultimate Status Bar being registered to the dictionary.
	/// </summary>
	static bool UltimateStatusBarRegistered ( string statusBarName )
	{
		// If the Ultimate Status Bar Dictionary contains the statusName, return true.
		if( UltimateStatusBars.ContainsKey( statusBarName ) )
			return true;

		// Debug a warning to the user so they know that the status bar they are trying to reference has not been registered, and then return false.
		Debug.LogWarning( "Ultimate Status Bar - No Ultimate Status Bar has been registered with the name: " + statusBarName + "." );
		return false;
	}
	/* ------------------------------------------- <<< END STATIC FUNCTIONS FOR THE USER >>> ------------------------------------------- */
}