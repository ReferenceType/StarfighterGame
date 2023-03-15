using UnityEditor;
using UnityEngine;

public class ExampleSceneScript : MonoBehaviour
{
	UltimateStatusBar[] ultimateStatusBars;


	void Start ()
	{
		ultimateStatusBars = FindObjectsOfType<UltimateStatusBar>();
	}

	public void UpdateBars ( float amount )
	{
		for( int i = 0; i < ultimateStatusBars.Length; i++ )
		{
			ultimateStatusBars[ i ].UpdateStatus( amount, 1.0f );
		}
	}

	#if UNITY_EDITOR
	public void SelectUltimateStatusBarObject ()
	{
		Selection.activeGameObject = FindObjectOfType<UltimateStatusBar>().gameObject;
	}
	#endif
}