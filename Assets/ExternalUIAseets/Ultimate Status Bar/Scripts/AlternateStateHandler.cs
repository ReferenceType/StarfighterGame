/* AlternateStateHandler.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu( "UI/Ultimate Status Bar/Alternate State Handler" )]
public class AlternateStateHandler : MonoBehaviour
{
	// ----- < ULTIMATE STATUS BAR > ----- //
	public UltimateStatusBar ultimateStatusBar;

	// ----- < ALTERNATE STATES > ----- //
	[Serializable]
	public class AlternateState
	{
		public AlternateStateHandler alternateStateHandler;

		// ULTIMATE STATUS //
		public int statusIndex = 0;
		public string alternateStateName = "";
		public Color defaultStateColor = Color.white;
		public enum TriggerOption
		{
			Manual,
			Percentage
		}
		public TriggerOption triggerOption = TriggerOption.Manual;
		public Image alternateStateImage;
		public Color alternateStateColor = Color.white;
		public bool flashing = false;
		public Color flashingColor = Color.white;
		public float flashingSpeed = 1.0f;
		public float flashingDuration = 0.0f;
		public enum TriggerBy
		{
			LessThan,
			GreaterThan,
			EqualTo
		}
		public TriggerBy triggerBy = TriggerBy.LessThan;
		public float triggerValue = 0.0f;

		public enum StateType
		{
			Image,
			Text
		}
		public StateType stateType = StateType.Image;
		public Text alternateStateText;
		public bool CurrentState
		{
			get;
			private set;
		}


		/// <summary>
		/// Switches the current state of this Alternate State.
		/// </summary>
		/// <param name="state">The target state to switch to.</param>
		public void SwitchState ( bool state )
		{
			// If the targeted state is true, but the current state is false...
			if( state == true && CurrentState == false )
			{
				// Switch the current state to true.
				CurrentState = true;
				
				// If the user wants to flash this state and the alternateStateHandler is assigned, then start the flashing coroutine.
				if( flashing == true && alternateStateHandler != null )
					alternateStateHandler.StartCoroutine( alternateStateHandler.AlternateStateFlashing( this ) );
				// Else just apply the alternate state color.
				else
					ApplyColor( alternateStateColor );
			}
			// Else if the state is false, but the current state is true...
			else if( state == false && CurrentState == true )
			{
				// Set the current state to false.
				CurrentState = false;

				// If the user is not using flash, then just apply the color.
				if( flashing == false )
					ApplyColor( defaultStateColor );
			}
		}
		
		/// <summary>
		/// Applies the color variable to the alternate state image or text.
		/// </summary>
		/// <param name="col">The target color to apply.</param>
		public void ApplyColor ( Color col )
		{
			// If the state type is image and the image is assigned, then apply the color to the image.
			if( stateType == StateType.Image && alternateStateImage != null )
				alternateStateImage.color = col;
			// Else if the user is using text, then apply the color to the text.
			else if( stateType == StateType.Text && alternateStateText != null )
				alternateStateText.color = col;
		}
		
		/// <summary>
		/// This function is called from the Handler each time the targeted Ultimate Status is updated.
		/// </summary>
		/// <param name="amt">The percentage value of the targeted Ultimate Status.</param>
		public void OnStatusUpdated ( float amt )
		{
			switch( triggerBy )
			{
				default:
				case TriggerBy.LessThan:
				{
					if( amt < triggerValue && CurrentState == false )
						SwitchState( true );
					else if( amt > triggerValue && CurrentState == true )
						SwitchState( false );
				}
				break;
				case TriggerBy.GreaterThan:
				{
					if( amt > triggerValue && CurrentState == false )
						SwitchState( true );
					else if( amt < triggerValue && CurrentState == true )
						SwitchState( false );
				}
				break;
				case TriggerBy.EqualTo:
				{
					if( amt == triggerValue && CurrentState == false )
						SwitchState( true );
					else if( amt != triggerValue && CurrentState == true )
						SwitchState( false );
				}
				break;
			}
		}
	}
	public List<AlternateState> AlternateStateList = new List<AlternateState>();
	Dictionary<string, AlternateState> AlternateStateDict = new Dictionary<string, AlternateState>();
	static Dictionary<string,AlternateStateHandler> AlternateStatusHandlerDict = new Dictionary<string, AlternateStateHandler>();


	private void OnEnable ()
	{
		// Loop through each alternate state in the list...
		for( int i = 0; i < AlternateStateList.Count; i++ )
		{
			// If the trigger option is set to percentage, then subscribe to the OnStatusUpdated function so that the percentage can be calculated.
			if( AlternateStateList[ i ].triggerOption == AlternateState.TriggerOption.Percentage )
				ultimateStatusBar.UltimateStatusList[ AlternateStateList[ i ].statusIndex ].OnStatusUpdated += AlternateStateList[ i ].OnStatusUpdated;
		}
	}

	private void OnDisable ()
	{
		// Loop through each alternate state in the list...
		for( int i = 0; i < AlternateStateList.Count; i++ )
		{
			// If the trigger option is set to percentage, then unsubscribe from the OnStatusUpdated function.
			if( AlternateStateList[ i ].triggerOption == AlternateState.TriggerOption.Percentage )
				ultimateStatusBar.UltimateStatusList[ AlternateStateList[ i ].statusIndex ].OnStatusUpdated -= AlternateStateList[ i ].OnStatusUpdated;
		}
	}

	void Awake ()
	{
		// If the status bar is null...
		if( ultimateStatusBar == null )
		{
			// Log an error, disable the component, then return.
			Debug.LogError( "Alternate State Handler - The Ultimate Status Bar component has not been assigned. Disabling this component to avoid errors." );
			enabled = false;
			return;
		}

		// If the assigned status bar name is not empty...
		if( ultimateStatusBar.statusBarName != string.Empty )
		{
			// If the dictionary contains this key already, then remove the key before adding another. This is important to avoid referencing an alternate state handler that does not exist anymore.
			if( AlternateStatusHandlerDict.ContainsKey( ultimateStatusBar.statusBarName ) )
				AlternateStatusHandlerDict.Remove( ultimateStatusBar.statusBarName );

			// Add this to the dictionary with the status bar name.
			AlternateStatusHandlerDict.Add( ultimateStatusBar.statusBarName, this );
		}

		// Loop through each alternate state in the list...
		for( int i = 0; i < AlternateStateList.Count; i++ )
		{
			// If the alternate state name is assigned, then add it to the dictionary.
			if( AlternateStateList[ i ].alternateStateName != string.Empty )
				AlternateStateDict.Add( AlternateStateList[ i ].alternateStateName, AlternateStateList[ i ] );

			// Assign the alternateStateHandler variable to this.
			AlternateStateList[ i ].alternateStateHandler = this;

			// If the trigger option is set to percentage, then subscribe to the OnStatusUpdated function so that the percentage can be calculated.
			if( AlternateStateList[ i ].triggerOption == AlternateState.TriggerOption.Percentage )
				ultimateStatusBar.UltimateStatusList[ AlternateStateList[ i ].statusIndex ].OnStatusUpdated += AlternateStateList[ i ].OnStatusUpdated;
		}
	}

	IEnumerator AlternateStateFlashing ( AlternateState alternateState )
	{
		// Store the flashing duration for this state.
		float flashDuration = alternateState.flashingDuration;

		// Store a float to keep the step for the flashing.
		float step = -90.0f;
		
		// This is multiplying by 6 in order to represent a "per second" option.
		float flashingSpeed = alternateState.flashingSpeed * 6;

		// While the alternate state is active...
		while( alternateState.CurrentState == true )
		{
			// If the flashing duration is greater than zero...
			if( alternateState.flashingDuration > 0 )
			{
				// Reduce the flashDuration by time.
				flashDuration -= Time.deltaTime;

				// If the flashDuration is depleted, then switch the state off.
				if( flashDuration <= 0 )
					alternateState.SwitchState( false );
			}

			// Increase the step for the flash.
			step += Time.deltaTime * flashingSpeed;

			// If the step is greater than 270, then reduce the value by 360 to keep the value within -90 and 270. This just makes it not increase indefinitely.
			if( step > 270 )
				step -= 360;
			
			// Apply the color between the alternate state color and the flashing color by the step value.
			alternateState.ApplyColor( Color.Lerp( alternateState.alternateStateColor, alternateState.flashingColor, ( Mathf.Sin( step ) + 1 ) / 2 ) );
			yield return null;
		}

		// Apply the default color since the flashing is finished.
		alternateState.ApplyColor( alternateState.defaultStateColor );
	}

	/* ----------------------------------< PUBLIC FUNCTIONS >----------------------------------- */
	/// <summary>
	/// Switches the targeted Alternate State to the desired state.
	/// </summary>
	/// <param name="stateName">The name of the state.</param>
	/// <param name="state">The state to apply to the Alternate State.</param>
	public void SwitchState ( string stateName, bool state )
	{
		// If the targeted alternate state does not exist, then return.
		if( !ConfirmAlternateState( stateName ) )
			return;

		// Switch the state on the targeted alternate state.
		AlternateStateDict[ stateName ].SwitchState( state );
	}

	/// <summary>
	/// Returns the Alternate State that has been registered with the stateName.
	/// </summary>
	/// <param name="stateName">The name of the state.</param>
	public AlternateState GetAlternateState ( string stateName )
	{
		// If the targeted alternate state does not exist, then return.
		if( !ConfirmAlternateState( stateName ) )
			return null;

		// Return the registered alternate state.
		return AlternateStateDict[ stateName ];
	}

	bool ConfirmAlternateState ( string stateName )
	{
		// If the dictionary contains the targeted name, then return true.
		if( AlternateStateDict.ContainsKey( stateName ) )
			return true;

		// Log an error and return false.
		Debug.LogError( "Alternate State Handler - No State has been registered with the name: " + stateName + "." );
		return false;
	}
	/* ---------------------------------< END PUBLIC FUNCTIONS >-------------------------------- */

	/* -------------------------------< PUBLIC STATIC FUNCTIONS >------------------------------- */
	/// <summary>
	/// Switches the targeted Alternate State to the desired state.
	/// </summary>
	/// <param name="statusBarName">The name of the Ultimate Status Bar associated with the Alternate State Handler.</param>
	/// <param name="stateName">The name the desired state to update.</param>
	/// <param name="state">The targeted state.</param>
	public static void SwitchState ( string statusBarName, string stateName, bool state )
	{
		// If the targeted Alternate State Handler does not exist, then return.
		if( !ConfirmAlternateStateHandler( statusBarName ) )
			return;

		// Switch the state of the targeted Alternate State.
		AlternateStatusHandlerDict[ statusBarName ].SwitchState( stateName, state );
	}

	/// <summary>
	/// Returns the Alternate State that has been registered with the statusBarName.
	/// </summary>
	/// <param name="statusBarName">The name of the Ultimate Status Bar associated with the Alternate State Handler.</param>
	public static AlternateStateHandler GetAlternateStateHandler ( string statusBarName )
	{
		// If the targeted Alternate State Handler does not exist, then return null.
		if( !ConfirmAlternateStateHandler( statusBarName ) )
			return null;

		// Return the registered Alternate State Handler.
		return AlternateStatusHandlerDict[ statusBarName ];
	}

	static bool ConfirmAlternateStateHandler ( string statusBarName )
	{
		// If the dictionary contains the targeted key, then return true.
		if( AlternateStatusHandlerDict.ContainsKey( statusBarName ) )
			return true;

		// Log an error and return false.
		Debug.LogError( "Alternate State Handler - No Alternate State Handler has been registered with the Ultimate Status Bar name: " + statusBarName + "." );
		return false;
	}
	/* -----------------------------< END PUBLIC STATIC FUNCTIONS >----------------------------- */
}