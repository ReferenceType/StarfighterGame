/* UltimateStatusBarReadme.cs */
/* Written by Kaz Crowe */
using UnityEngine;

//[CreateAssetMenu( fileName = "README", menuName = "ScriptableObjects/ReadmeScriptableObject", order = 1 )]
public class UltimateStatusBarReadme : ScriptableObject
{
	public static int ImportantChange = 4;// UPDATE ON IMPORTANT CHANGES // 4 >= 2.6.0 / 3 >= 2.5.0 / 2 >= 2.1.0 - 2.1.3 / 1 >= 2.0.0 - 2.0.3
	public Texture2D icon;
	public Texture2D scriptReference, statusInformation;

	public class VersionHistory
	{
		public string versionNumber = "";
		public string[] changes;
	}
	public VersionHistory[] versionHistory = new VersionHistory[]
	{
		// VERSION 2.6.0
		new VersionHistory ()
		{
			versionNumber = "2.6.0",
			changes = new string[]
			{
				"Simplified the editor script internally",
				"Removed AnimBool functionality from the inspector to avoid errors with Unity 2019+",
				"Added coding comments to the AlternateStateHandler.cs script since they were missing",
				"Added new script: UltimateStatusBarReadme.cs",
				"Added new script: UltimateStatusBarReadmeEditor.cs",
				"Added new file at the Ultimate Status Bar root folder: README. This file has all the documentation and how to information",
				"Removed the UltimateStatusBarWindow.cs file. All of that information and more is now located in the README file",
				"Removed the old README text file. All of that information and more is now located in the README file",
			},
		},
		// VERSION 2.5.0
		new VersionHistory ()
		{
			versionNumber = "2.5.0",
			changes = new string[]
			{
				"Restructured the folders to help conform with the Unity standard for Assets",
				"Made some pretty big changes and improvements to the Documentation Window",
				"Updated and fixed some minor things inside the Status Fill Follower script",
				"Removed the UltimateStatusBarUpdater class out of the UltimateStatusBar class as this would cause errors in some rare cases. A new script has been added to fix these errors. The script is named UltimateStatusBarScreenSizeUpdater",
			},
		},
	};
}