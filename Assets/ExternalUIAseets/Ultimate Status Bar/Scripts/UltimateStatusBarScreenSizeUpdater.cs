/* UltimateStatusBarUpdater.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UltimateStatusBarScreenSizeUpdater : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange ()
	{
		if( !gameObject.activeInHierarchy )
			return;

		StartCoroutine( "UpdatePositioning" );
	}

	IEnumerator UpdatePositioning ()
	{
		yield return new WaitForEndOfFrame();

		UltimateStatusBar[] allStatusBars = FindObjectsOfType<UltimateStatusBar>();
		for( int i = 0; i < allStatusBars.Length; i++ )
			allStatusBars[ i ].UpdatePositioning();
	}
}