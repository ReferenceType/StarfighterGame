/* DramaticStatusFill.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[AddComponentMenu( "UI/Ultimate Status Bar/Dramatic Status Fill" )]
public class DramaticStatusFill : MonoBehaviour
{
	// ----- < ULTIMATE STATUS BAR INFORMATION > ----- //
	public UltimateStatusBar ultimateStatusBar;
	UltimateStatusBar.UltimateStatus status;
	public int statusIndex = 0;
	Image ultimateStatusImage;

	// ----- < DRAMTIC STATUS INFORMATION > ----- //
	public Image statusImage;
	public Color statusColor = Color.white;
	public enum DramaticStyle
	{
		Increase,
		Decrease
	}
	public DramaticStyle dramaticStyle = DramaticStyle.Decrease;
	bool isUpdating = false;
	float _secondsDelay = 0.1f;
	public float secondsDelay = 0.1f;
	public float resetSensitivity = 0.1f;
	float previousFillAmt = 0.0f;
	public float fillSpeed = 0.5f;


	void OnEnable ()
	{
		// If the Ultimate Status Bar component is null, then return a error to the user.
		if( ultimateStatusBar == null )
		{
			Debug.LogError( "Dramatic Status Fill - The Ultimate Status Bar component has not been assigned." );
			return;
		}

		// Assign the Ultimate Status to follow.
		status = ultimateStatusBar.UltimateStatusList[ statusIndex ];

		// If the Ultimate Status variable is assigned...
		if( status != null )
		{
			// Store the Ultimate Status image for checks.
			ultimateStatusImage = status.statusImage;

			// Subscribe to the OnStatusUpdated Action.
			status.OnStatusUpdated += OnStatusUpdated;
		}

		if( statusImage != null )
			statusImage.color = statusColor;
	}

	void OnDisable ()
	{
		// If the Ultimate Status Bar component is null, then return.
		if( ultimateStatusBar == null )
			return;

		// If the Ultimate Status variable is not null, then unsubscribe from the event to prevent memory leaks.
		if( status != null )
			status.OnStatusUpdated -= OnStatusUpdated;
	}

	/// <summary>
	/// This function gets subscribed to the Ultimate Status OnStatusUpdated function.
	/// </summary>
	/// <param name="val">Useless Paramater. Needed to subscribe to OnStatusUpdated.</param>
	void OnStatusUpdated ( float val )
	{
		switch( dramaticStyle )
		{
			case DramaticStyle.Decrease:
			{
				// If the official status bar's fill amount is higher than this status...
				if( ultimateStatusImage.fillAmount > statusImage.fillAmount )
				{
					// Apply the same fill amount so that it will keep up visually, then return.
					statusImage.fillAmount = ultimateStatusImage.fillAmount;

					if( isUpdating == true )
						isUpdating = false;
					return;
				}

				// If the status is not currently updating, then start the update.
				if( isUpdating == false )
					StartCoroutine( "UpdateFillDecrease" );
				// Else if the status is currently udpating and the difference between fills is less than the reset sensitivity, then reset the wait seconds.
				else if( isUpdating == true && ( statusImage.fillAmount - previousFillAmt ) < resetSensitivity )
					_secondsDelay = secondsDelay;

				// Store the previous fill amount.
				previousFillAmt = ultimateStatusImage.fillAmount;
			}break;
			case DramaticStyle.Increase:
			default:
			{
				if( ultimateStatusImage.fillAmount < statusImage.fillAmount && ultimateStatusImage.fillAmount > status.GetTargetFill )
					statusImage.fillAmount = ultimateStatusImage.fillAmount;
				else if( statusImage.fillAmount != status.GetTargetFill )
					statusImage.fillAmount = status.GetTargetFill;
			}
			break;
		}
	}

	/// <summary>
	/// Coroutine to update the fill over time.
	/// </summary>
	IEnumerator UpdateFillDecrease ()
	{
		// Set isUpdating to true so that other functions can check if this is running.
		isUpdating = true;

		// Apply the wait time.
		_secondsDelay = secondsDelay;

		// This loop will continue while the local fill amount is greater than the target fill amount.
		while( statusImage.fillAmount > ultimateStatusImage.fillAmount && isUpdating == true )
		{
			// If the wait seconds are greater than zero, then decrease the time.
			if( _secondsDelay > 0 )
				_secondsDelay -= Time.deltaTime;
			// Else, reduce the fill amount by the configured fill speed.
			else
				statusImage.fillAmount -= fillSpeed * Time.deltaTime;

			yield return null;
		}

		// If isUpdating is true, then this coroutine finished, so apply the final amount.
		if( isUpdating == true )
			statusImage.fillAmount = ultimateStatusImage.fillAmount;

		// Set isUpdating to false so that other function will know that this function is not running now.
		isUpdating = false;
	}
}