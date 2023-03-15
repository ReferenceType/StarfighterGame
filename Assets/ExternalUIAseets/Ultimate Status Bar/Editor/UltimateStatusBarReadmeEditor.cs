/* UltimateStatusBarReadmeEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

[InitializeOnLoad]
[CustomEditor( typeof( UltimateStatusBarReadme ) )]
public class UltimateStatusBarReadmeEditor : Editor
{
	static UltimateStatusBarReadme readme;

	// LAYOUT STYLES //
	string Indent
	{
		get
		{
			return "    ";
		}
	}
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle titleStyle = new GUIStyle();
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();
	GUIStyle versionStyle = new GUIStyle();
	static string menuTitle = "Product Manual";

	class PageInformation
	{
		public string pageName = "";
		public Vector2 scrollPosition = Vector2.zero;
		public delegate void TargetMethod ();
		public TargetMethod targetMethod;
	}
	static PageInformation mainMenu = new PageInformation() { pageName = "Product Manual" };
	static PageInformation gettingStarted = new PageInformation() { pageName = "Getting Started" };
	static PageInformation overview = new PageInformation() { pageName = "Overview" };
	static PageInformation overview_USB = new PageInformation() { pageName = "Overview" };
	static PageInformation overview_ASH = new PageInformation() { pageName = "Overview" };
	static PageInformation overview_DSF = new PageInformation() { pageName = "Overview" };
	static PageInformation overview_SFF = new PageInformation() { pageName = "Overview" };
	static PageInformation documentation = new PageInformation() { pageName = "Documentation" };
	static PageInformation documentation_USB = new PageInformation() { pageName = "Documentation" };
	static PageInformation documentation_US = new PageInformation() { pageName = "Documentation" };
	static PageInformation documentation_ASH = new PageInformation() { pageName = "Documentation" };
	static PageInformation versionHistory = new PageInformation() { pageName = "Version History" };
	static PageInformation importantChange = new PageInformation() { pageName = "Important Change" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You!" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();

	class EndPageComment
	{
		public string comment = "";
		public string url = "";
	}
	EndPageComment[] endPageComments = new EndPageComment[]
	{
		new EndPageComment()
		{
			comment = "Enjoying the Ultimate Status Bar? Leave us a review on the <b><color=blue>Unity Asset Store</color></b>!",
			url = "https://assetstore.unity.com/packages/slug/48320"
		},
		new EndPageComment()
		{
			comment = "Looking for a mobile joystick for your game? Check out the <b><color=blue>Ultimate Joystick</color></b>!",
			url = "https://www.tankandhealerstudio.com/ultimate-joystick.html"
		},
		new EndPageComment()
		{
			comment = "Do you need a radial menu for your game? Check out the <b><color=blue>Ultimate Radial Menu</color></b>!",
			url = "https://www.tankandhealerstudio.com/ultimate-radial-menu.html"
		},
		new EndPageComment()
		{
			comment = "Check out our <b><color=blue>other products</color></b>!",
			url = "https://www.tankandhealerstudio.com/assets.html"
		},
	};
	int randomComment = 0;

	class DocumentationInfo
	{
		public string functionName = "";
		public AnimBool showMore = new AnimBool( false );
		public string[] parameter;
		public string returnType = "";
		public string description = "";
		public string codeExample = "";
	}
	/* ULTIMATE STATUS BAR DOCUMENTATION */
	DocumentationInfo[] UltimateStatusBar_PublicFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "UpdateStatus()",
			parameter = new string[]
			{
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status."
			},
			description = "This function will call the default status on the targeted Ultimate Status Bar. It updates the values of the status in order to display them to the user. This function has two parameters that need to be passed into it. The <i>currentValue</i> should be the current amount of the targeted status, whereas the <i>maxValue</i> should be the maximum amount that the status can be. These values must be passed into the function in order to correctly display them to the user.",
			codeExample = "statusBar.UpdateStatus( current, max );"
		},
		new DocumentationInfo()
		{
			functionName = "UpdateStatus() *",
			parameter = new string[]
			{
				"string statusName - The name of the targeted Ultimate Status.",
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status.",
			},
			description = "This function will call the targeted Ultimate Status that has been registered with the <i>statusName</i> parameter. It updates the values of the status in order to display them to the user. The <i>currentValue</i> should be the current amount of the targeted status, whereas the <i>maxValue</i> should be the maximum amount that the status can be. These values must be passed into the function in order to correctly display them to the user.",
			codeExample = "statusBar.UpdateStatus( \"Health\", current, max );"
		},
		new DocumentationInfo()
		{
			functionName = "GetStatusBarState",
			returnType = "bool",
			description = "Returns the current state of the Ultimate Status Bar visibility.",
			codeExample = "if( statusBar.GetStatusBarState )\n{\n	// The status bar is visible, so do something!\n}"
		},
		new DocumentationInfo()
		{
			functionName = "UpdatePositioning()",
			description = "This function updates the size and positioning of the Ultimate Status Bar on the screen. It's worth noting that this function will only work in the Screen Space option is selected for the Positioning setting.",
			codeExample = "statusBar.screenSpaceOptions.statusBarSize = 5.0f;\nstatusBar.UpdatePositioning();"
		},
		new DocumentationInfo()
		{
			functionName = "UpdateStatusBarIcon()",
			parameter = new string[]
			{
				"Sprite newIcon - The new icon to apply to the status bar icon.",
			},
			description = "This function updates the icon image associated with the Ultimate Status Bar with the <i>newIcon</i> sprite parameter.",
			codeExample = "statusBar.UpdateStatusBarIcon( newIcon );"
		},
		new DocumentationInfo()
		{
			functionName = "UpdateStatusBarText()",
			parameter = new string[]
			{
				"string newText - The new text to apply to the status bar.",
			},
			description = "This function will update the text component to display the <i>newText</i> string parameter.",
			codeExample = "statusBar.UpdateStatusBarText( \"Character Name\" );"
		},
		new DocumentationInfo()
		{
			functionName = "GetUltimateStatus()",
			parameter = new string[]
			{
				"string statusName - The name of the targeted Ultimate Status.",
			},
			returnType = "UltimateStatus",
			description = "This function returns the Ultimate Status class that has been registered with the <i>statusName</i> parameter.",
			codeExample = "UltimateStatusBar.UltimateStatus healthStatus = statusBar.GetUltimateStatus( \"Health\" );"
		},
	};
	DocumentationInfo[] UltimateStatusBar_StaticFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "UpdateStatus()",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
				"string statusName - The name of the targeted Ultimate Status.",
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status.",
			},
			description = "This function will update the targeted Ultimate Status that has been registered on the Ultimate Status Bar component. See the UpdateStatus() function inside the Ultimate Status documentation for more details.",
			codeExample = "UltimateStatusBar.UpdateStatus( \"Player\", \"Health\", current, max );"
		},
		new DocumentationInfo()
		{
			functionName = "UpdateStatusBarIcon()",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
				"Sprite newIcon - The new icon to apply to the status bar icon.",
			},
			description = "Calls the UpdateStatusBarIcon() function of the targeted Ultimate Status Bar. See the public function: UpdateStatusBarIcon(), for more details.",
			codeExample = "UltimateStatusBar.UpdateStatusBarIcon( \"Player\", newIcon );"
		},
		new DocumentationInfo()
		{
			functionName = "UpdateStatusBarText()",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
				"string newText - The new text to apply to the status bar.",
			},
			description = "Calls the UpdateStatusBarText() function of the targeted Ultimate Status Bar. See the public function: UpdateStatusBarText(), for more details.",
			codeExample = "UltimateStatusBar.UpdateStatusBarText( \"Player\", \"Character Name\" );"
		},
		new DocumentationInfo()
		{
			functionName = "GetUltimateStatusBar()",
			returnType = "UltimateStatusBar",
			parameter = new string[]
			{
				"string statusBarName - The name of the targeted Ultimate Status Bar.",
			},
			description = "This function will return the Ultimate Status Bar component that has been registered with the <i>statusBarName</i> parameter.",
			codeExample = "UltimateStatusBar playerStatusBar = UltimateStatusBar.GetUltimateStatusBar( \"Player\" );"
		},
	};
	DocumentationInfo[] UltimateStatus_PublicFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "UpdateStatus()",
			parameter = new string[]
			{
				"float currentValue - The current value of the status.",
				"float maxValue - The maximum value of the status."
			},
			description = "This function will update the values of the status in order to display them to the user. This function has two parameters that need to be passed into it. The <i>currentValue</i> should be the current amount of the targeted status, whereas the <i>maxValue</i> should be the maximum amount that the status can be. These values must be passed into the function in order to correctly display them to the user. Using these values, the Ultimate Status will calculate out the percentage values and then display this information to the user according to the options set in the inspector.",
			codeExample = "status.UpdateStatus( current, max );"
		},
		new DocumentationInfo()
		{
			functionName = "GetCurrentFraction",
			returnType = "float",
			description = "The GetCurrentFraction property will return the percentage value that was calculated when the status was updated. This number will not be current with the Smooth Fill option.",
			codeExample = ""
		},
		new DocumentationInfo()
		{
			functionName = "GetMaxValue",
			returnType = "float",
			description = "The GetMaxValue property will return the last known maximum value passed through the UpdateStatus function.",
			codeExample = ""
		},
		new DocumentationInfo()
		{
			functionName = "GetTargetFill",
			returnType = "float",
			description = "The GetTargetFill property will return the target amount of fill for the image. This is used by other classes, such as the Dramatic Status Fill.",
			codeExample = ""
		},
		new DocumentationInfo()
		{
			functionName = "GetCurrentCalculatedFraction",
			returnType = "float",
			description = "The GetCurrentCalculatedFraction property will return the current percentage of the status. This value is current with the Smooth Fill or Fill Constraint options.",
			codeExample = ""
		},
		new DocumentationInfo()
		{
			functionName = "UpdateStatusColor()",
			parameter = new string[]
			{
				"Color targetColor - The target color to apply to the status bar."
			},
			description = "This function will update the status color variable and apply the color immediately to the status image.",
			codeExample = "status.UpdateStatusColor( Color.red );"
		},
		new DocumentationInfo()
		{
			functionName = "UpdateStatusTextColor()",
			parameter = new string[]
			{
				"Color targetColor - The target color to apply to the status bar text component."
			},
			description = "This function will update the associated Text component's color value with the Color parameter.",
			codeExample = "status.UpdateStatusTextColor( Color.red );"
		},
	};

	/* ALTERNATE STATE HANDLER DOCUMENTATION */
	DocumentationInfo[] AlternateStateHandler_PublicFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "SwitchState()",
			parameter = new string[]
			{
				"string stateName - The name of the state.",
				"bool state - The targeted state."
			},
			description = "This function switches the targeted Alternate State to the desired state. The <i>stateName</i> parameter will allow the script to find that specific state in order to switch it.",
			codeExample = "altStateHandler.SwitchState( \"HealthCritical\", true );"
		},
		new DocumentationInfo()
		{
			functionName = "GetAlternateState()",
			parameter = new string[]
			{
				"string stateName - The name of the state.",
			},
			description = "This function will return the AlternateState class that has been registered with the <i>stateName</i> parameter.",
			codeExample = "AlternateStateHandler.AlternateState alternateState = altStateHandler.GetAlternateState( \"HealthCritical\" );"
		},
	};
	DocumentationInfo[] AlternateStateHandler_StaticFunctions = new DocumentationInfo[]
	{
		new DocumentationInfo()
		{
			functionName = "SwitchState()",
			parameter = new string[]
			{
				"string statusBarName - The name of the Ultimate Status Bar associated with the Alternate State Handler.",
				"string stateName - The name the desired state to update.",
				"bool state - The targeted state."
			},
			description = "This function switches the targeted Alternate State to the desired state. The <i>statusBarName</i> parameter will allow the script to find the specific Alternate State Handler that has been registered with the name of the Ultimate Status Bar that it is associated with. The <i>stateName</i> parameter will allow the script to find that specific state in order to switch it.",
			codeExample = "AlternateStateHandler.SwitchState( \"Player\", \"HealthCritical\", true );"
		},
		new DocumentationInfo()
		{
			functionName = "GetAlternateStateHandler()",
			parameter = new string[]
			{
				"string statusBarName - The name of the Ultimate Status Bar associated with the Alternate State Handler.",
			},
			description = "This function returns the AlternateStateHandler class that has been registered with the <i>statusBarName</i> parameter. It's worth noting that the Alternate State Handler will be registered with the name of the Ultimate Status Bar that it is associated with.",
			codeExample = "AlternateStateHandler altStateHandler = AlternateStateHandler.GetAlternateStateHandler( \"Player\" );"
		},
	};

	bool showStatusBarPositioning = false, showStatusBarSettings = false;
	bool showRadialButtonSettings = false, showScriptReference = false;
	bool showAlternateStates = false, altShowScriptRefernce = false;

	bool navigateToVersionHistory = false;


	static UltimateStatusBarReadmeEditor ()
	{
		EditorApplication.update += WaitForCompile;
	}

	static void WaitForCompile ()
	{
		if( EditorApplication.isCompiling )
			return;

		EditorApplication.update -= WaitForCompile;
		
		// If this is the first time that the user has downloaded this asset...
		if( !EditorPrefs.HasKey( "UltimateStatusBarVersion" ) )
		{
			NavigateForward( thankYou );
			EditorPrefs.SetInt( "UltimateStatusBarVersion", UltimateStatusBarReadme.ImportantChange );
			var ids = AssetDatabase.FindAssets( "README t:UltimateStatusBarReadme" );
			if( ids.Length == 1 )
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
				Selection.objects = new Object[] { readmeObject };
			}
		}
		// Else if the version has been updated and there are important changes to display to the user...
		else if( EditorPrefs.GetInt( "UltimateStatusBarVersion" ) < UltimateStatusBarReadme.ImportantChange )
		{
			NavigateForward( importantChange );
			EditorPrefs.SetInt( "UltimateStatusBarVersion", UltimateStatusBarReadme.ImportantChange );
			var ids = AssetDatabase.FindAssets( "README t:UltimateStatusBarReadme" );
			if( ids.Length == 1 )
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
				Selection.objects = new Object[] { readmeObject };
			}
		}
	}

	void OnEnable ()
	{
		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainPage;
		gettingStarted.targetMethod = GettingStarted;
		overview.targetMethod = Overview;
		overview_USB.targetMethod = Overview_UltimateStatusBar;
		overview_ASH.targetMethod = Overview_AlternateStateHandler;
		overview_DSF.targetMethod = Overview_DramaticStatusFill;
		overview_SFF.targetMethod = Overview_StatusFillFollower;
		documentation.targetMethod = Documentation;
		documentation_US.targetMethod = Documentation_UltimateStatus;
		documentation_USB.targetMethod = Documentation_UltimateStatusBar;
		documentation_ASH.targetMethod = Documentation_AlternateStateHandler;
		versionHistory.targetMethod = VersionHistory;
		importantChange.targetMethod = ImportantChange;
		thankYou.targetMethod = ThankYou;

		if( pageHistory.Count == 1 )
			currentPage = mainMenu;

		randomComment = Random.Range( 0, endPageComments.Length );

		readme = ( UltimateStatusBarReadme )target;
	}

	protected override void OnHeaderGUI ()
	{
		UltimateStatusBarReadme readme = ( UltimateStatusBarReadme )target;

		float iconWidth = Mathf.Min( EditorGUIUtility.currentViewWidth, 350f );

		Vector2 ratio = new Vector2( readme.icon.width, readme.icon.height ) / ( readme.icon.width > readme.icon.height ? readme.icon.width : readme.icon.height );

		GUILayout.BeginHorizontal( "In BigTitle" );
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label( readme.icon, GUILayout.Width( iconWidth * ratio.x ), GUILayout.Height( iconWidth * ratio.y ) );
			GUILayout.Space( -20 );
			GUILayout.Label( readme.versionHistory[ 0 ].versionNumber, versionStyle );
			var rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
			if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) && !pageHistory.Contains( versionHistory ) )
				navigateToVersionHistory = true;
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI();
		serializedObject.Update();

		paragraphStyle = new GUIStyle( EditorStyles.label ) { wordWrap = true, richText = true, fontSize = 12 };
		itemHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 12, fontStyle = FontStyle.Bold };
		sectionHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		titleStyle = new GUIStyle( paragraphStyle ) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		versionStyle = new GUIStyle( paragraphStyle ) { alignment = TextAnchor.MiddleCenter, fontSize = 10 };

		EditorGUILayout.BeginHorizontal();
		if( pageHistory.Count > 1 )
		{
			if( GUILayout.Button( "<", GUILayout.Width( 20 ) ) )
				NavigateBack();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( menuTitle, titleStyle );
		GUILayout.FlexibleSpace();
		if( pageHistory.Count > 1 )
			GUILayout.Space( 20 );

		EditorGUILayout.EndHorizontal();

		if( currentPage.targetMethod != null )
			currentPage.targetMethod();

		if( navigateToVersionHistory && Event.current.type == EventType.Repaint )
		{
			navigateToVersionHistory = false;
			NavigateForward( versionHistory );
		}

		Repaint();
	}

	void StartPage ( PageInformation pageInfo )
	{
		pageInfo.scrollPosition = EditorGUILayout.BeginScrollView( pageInfo.scrollPosition, false, false );
		GUILayout.Space( 15 );
	}

	void EndPage ()
	{
		EditorGUILayout.EndScrollView();
	}

	static void NavigateBack ()
	{
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		menuTitle = pageHistory[ pageHistory.Count - 1 ].pageName;
		currentPage = pageHistory[ pageHistory.Count - 1 ];
	}

	static void NavigateForward ( PageInformation menu )
	{
		pageHistory.Add( menu );
		menuTitle = menu.pageName;
		currentPage = menu;
	}

	void MainPage ()
	{
		EditorGUILayout.LabelField( "We hope that you are enjoying using the Ultimate Status Bar in your project! Here is a list of helpful resources for this asset:", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Read the <b><color=blue>Getting Started</color></b> section of this README!", paragraphStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( gettingStarted );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • To learn more about the options on the inspector, read the <b><color=blue>Overview</color></b> section!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( overview );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Check out the <b><color=blue>Documentation</color></b> section!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( documentation );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Watch our <b><color=blue>Video Tutorials</color></b> on the Ultimate Status Bar!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			Debug.Log( "Ultimate Status Bar\nOpening YouTube Tutorials" );
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9Tl0VRLpo3VoU2U-SbLgwB3-" );
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • <b><color=blue>Contact Us</color></b> directly with your issue! We'll try to help you out as much as we can.", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			Debug.Log( "Ultimate Status Bar\nOpening Online Contact Form" );
			Application.OpenURL( "https://www.tankandhealerstudio.com/contact-us.html" );
		}

		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField( "Now you have the tools you need to get the Ultimate Status Bar working in your project. Now get out there and make your awesome game!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		GUILayout.Space( sectionSpace );

		GUILayout.FlexibleSpace();

		EditorGUILayout.LabelField( endPageComments[ randomComment ].comment, paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			Application.OpenURL( endPageComments[ randomComment ].url );
	}

	void GettingStarted ()
	{
		StartPage( gettingStarted );

		EditorGUILayout.LabelField( "Introduction", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "The Ultimate Status Bar has been built from the ground up with being easy to use and customize to make it work the way that you want.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "To begin we'll look at how to simply create and customize an Ultimate Status Bar in your scene. After that we will go over how to reference the Ultimate Status Bar in your custom scripts.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "To create an Ultimate Status Bar in your scene, simply find the Ultimate Status Bar prefab that you would like to add and drag the prefab into the scene. The Ultimate Status Bar prefab will automatically find or create a canvas in your scene for you.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Customize", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "There are many ways to use the Ultimate Status Bar within your projects. The main Ultimate Status Bar component is used to display each status within your scene, while there are three subcomponents that are all used to enhance the visual display of the Ultimate Status Bar.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For more information about each of these scripts, please see the Overview and Documentation sections of this README. The Overview section explains how to get the components in your scene and describes what options you can find in each section of the inspector. The Documentation section explains each function available in these scripts.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "The Ultimate Status Bar is incredibly easy to get implemented into your custom scripts. There are a few ways that you can reference the Ultimate Status Bar through code, and it all depends on how many different status sections you have created on that particular Ultimate Status Bar. For more information on how to reference the Ultimate Status Bar, please see the Documentation section of this README, or the Script Reference section of the Ultimate Status Bar inspector.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For this example, we will create an Ultimate Status Bar for the Player of a simple game. Let's assume that the Player has several different status values that must be displayed. For this example, the Player will have a <i>Health</i> value, and a <i>Energy</i> value. These will need to be created inside the <b>Status Information</b> section in order to be referenced through code.", paragraphStyle );

		float imageWidth = Mathf.Min( EditorGUIUtility.currentViewWidth, 350f ) * 0.85f;
		Vector2 ratio = new Vector2( readme.statusInformation.width, readme.statusInformation.height ) / ( readme.statusInformation.width > readme.statusInformation.height ? readme.statusInformation.width : readme.statusInformation.height );
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( readme.statusInformation, GUILayout.Width( imageWidth * ratio.x ), GUILayout.Height( imageWidth * ratio.y ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField( "After these have been created, we need to give the Ultimate Status Bar a unique name to be referenced through code. This is done in the <b>Script Reference</b> section located within the Inspector window. For this example, we are creating this status bar for the <i>Player</i>, so that's what we will name it.", paragraphStyle );

		ratio = new Vector2( readme.scriptReference.width, readme.scriptReference.height ) / ( readme.scriptReference.width > readme.scriptReference.height ? readme.scriptReference.width : readme.scriptReference.height );
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( readme.scriptReference, GUILayout.Width( imageWidth * ratio.x ), GUILayout.Height( imageWidth * ratio.y ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField( "Now that each status has been named, and the Ultimate Status Bar has a unique name that can be referenced, simply copy the code provided inside the <b>Script Reference</b> section for the desired status. Make sure that the Function option is set to: Update Status.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "After copying the code that is provided, find the function in <i>your player's health script</i> where your player is receiving damage from and paste the example code into the function. Be sure to put it after the damage or healing has modified the health value. Of course, be sure to replace the currentValue and maxValue of the example code with your character's current and maximum health values. Whenever the character's health is updated, either by damage or healing done to the character, you will want to send the new information of the health's value.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "This process can be used for any status that you need to be displayed to the user. For more information about the individual functions available for the Ultimate Status Bar and other components, please refer to the Documentation section of this window.", paragraphStyle );

		EndPage();
	}

	void Overview ()
	{
		StartPage( overview );

		EditorGUILayout.LabelField( "Introduction", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "Welcome to the Ultimate Status Bar Overview section. This section will go over the different options that can be found on the inspector window for each script of the Ultimate Status Bar Asset Package. Please see each section below to learn more about each inspector individually.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		// UltimateStatusBar.cs
		EditorGUILayout.LabelField( "UltimateStatusBar.cs", itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( overview_USB );
			GUI.FocusControl( "" );
		}

		GUILayout.Space( paragraphSpace );

		// AlternateStateHandler.cs
		EditorGUILayout.LabelField( "AlternateStateHandler.cs", itemHeaderStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( overview_ASH );
			GUI.FocusControl( "" );
		}

		GUILayout.Space( paragraphSpace );

		// DramaticStatusFill.cs
		EditorGUILayout.LabelField( "DramaticStatusFill.cs", itemHeaderStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( overview_DSF );
			GUI.FocusControl( "" );
		}

		GUILayout.Space( paragraphSpace );

		// StatusFillFollower.cs
		EditorGUILayout.LabelField( "StatusFillFollower.cs", itemHeaderStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( overview_SFF );
			GUI.FocusControl( "" );
		}
		
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option on any of the Ultimate Status Bar inspectors, please select the component in your scene and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Overview_UltimateStatusBar ()
	{
		StartPage( overview_USB );

		EditorGUILayout.LabelField( "UltimateStatusBar.cs", sectionHeaderStyle );
		EditorGUILayout.LabelField( "Summary", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The <i>Ultimate Status Bar</i> can be used to display the status of anything from your Player and Enemies in your scene, to the Loading Bar when starting your game.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The display below is mimicking the Ultimate Status Bar inspector. Expand each section to learn what each one is designed for.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		/* //// --------------------------- < Status Bar Positioning > --------------------------- \\\\ */
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Status Bar Positioning", EditorStyles.boldLabel );
		if( GUILayout.Button( showStatusBarPositioning ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			showStatusBarPositioning = !showStatusBarPositioning;
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if( showStatusBarPositioning )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "This section handles the positioning of the Ultimate Status Bar on the screen or facing the camera if used in world space.", paragraphStyle );
		}
		/* \\\\ -------------------------- < End Status Bar Positioning > --------------------------- //// */

		EditorGUILayout.Space();

		/* //// ----------------------------- < Status Bar Options > ----------------------------- \\\\ */
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Status Bar Options", EditorStyles.boldLabel );
		if( GUILayout.Button( showStatusBarSettings ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			showStatusBarSettings = !showStatusBarSettings;
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if( showStatusBarSettings )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "This section has options that affect the status bar as a whole. Settings for icon, text and visibility.", paragraphStyle );
		}
		/* //// --------------------------- < End Status Bar Options > --------------------------- \\\\ */

		EditorGUILayout.Space();

		/* //// ----------------------------- < Status Information > ----------------------------- \\\\ */
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Status Information", EditorStyles.boldLabel );
		if( GUILayout.Button( showRadialButtonSettings ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			showRadialButtonSettings = !showRadialButtonSettings;
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if( showRadialButtonSettings )
		{
			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "Here is where you can customize and edit each status individually. This is where you will want to create each status that needs to be a part of this status bar.", paragraphStyle );
		}
		/* //// --------------------------- < End Status Information > --------------------------- \\\\ */

		EditorGUILayout.Space();

		/* //// ----------------------------- < Script Reference > ------------------------------ \\\\ */
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Script Reference", EditorStyles.boldLabel );
		if( GUILayout.Button( showScriptReference ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			showScriptReference = !showScriptReference;
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if( showScriptReference )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "In this section you will be able to setup the reference to this Ultimate Status Bar, and you will be provided with code examples to be able to copy and paste into your own scripts.", paragraphStyle );
		}
		/* //// --------------------------- < End Script Reference > ---------------------------- \\\\ */

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option on this component, please select the Ultimate Status Bar in your scene and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Overview_AlternateStateHandler ()
	{
		StartPage( overview_ASH );

		EditorGUILayout.LabelField( "AlternateStateHandler.cs", sectionHeaderStyle );
		EditorGUILayout.LabelField( "Summary", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The <i>Alternate State Handler</i> component allows you to display different states for each status in your scene. This can be done using images or text. An example for this would be making the Health bar of the player flash when it is at a critical amount. However, you can use this component in what ever way your project needs.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		/* HOW TO CREATE */
		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Alternate State Handler is very easy to get started. Simply add the Alternate State Handler component to the Ultimate Status Bar GameObject. After selecting the Ultimate Status Bar GameObject you can add this component in the Inspector by selecting: <i>Add Component / UI / Ultimate Status Bar / Alternate State Handler.</i>", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The display below is mimicking the Alternate State Handler inspector. Expand each section to learn what each one is designed for.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		/* //// ----------------------------- < Alternate States > ----------------------------- \\\\ */
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Alternate States", EditorStyles.boldLabel );
		if( GUILayout.Button( showAlternateStates ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			showAlternateStates = !showAlternateStates;
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if( showAlternateStates )
		{
			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "Here is where you can customize each state that you want for the status bar.", paragraphStyle );
		}
		/* //// --------------------------- < End Status Information > --------------------------- \\\\ */

		EditorGUILayout.Space();

		/* //// ----------------------------- < Script Reference > ------------------------------ \\\\ */
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Script Reference", EditorStyles.boldLabel );
		if( GUILayout.Button( altShowScriptRefernce ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			altShowScriptRefernce = !altShowScriptRefernce;
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if( altShowScriptRefernce )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "In this section you will be able to setup the reference to this Alternate State Handler, and you will be provided with code examples to be able to copy and paste into your own scripts.", paragraphStyle );
		}
		/* //// --------------------------- < End Script Reference > ---------------------------- \\\\ */

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option on this component, please select the Alternate State Handler in your scene and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Overview_DramaticStatusFill ()
	{
		StartPage( overview_DSF );

		EditorGUILayout.LabelField( "DramaticStatusFill.cs", sectionHeaderStyle );
		EditorGUILayout.LabelField( "Summary", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The <i>Dramatic Status Fill</i> component uses a second image to display a dramatic effect to the user. This component helps to draw attention to the change in status amount.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "In order to create a Dramatic Status Fill for a status on your Ultimate Status Bar you will need to select the GameObject that has the Image component for the status that you want. This can be found easily by clicking on the <b>Status Image</b> variable on the targeted status in the Ultimate Status Bar inspector. This will highlight the associated GameObject. Now that you know which GameObject has the image you want, duplicate that GameObject and add a Dramatic Status Fill component. This can be done by selecting: <i>Add Component / UI / Ultimate Status Bar / Dramatic Status Fill</i>.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option on this component, please select the Dramatic Status Fill in your scene and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Overview_StatusFillFollower ()
	{
		StartPage( overview_SFF );

		EditorGUILayout.LabelField( "StatusFillFollower.cs", sectionHeaderStyle );
		EditorGUILayout.LabelField( "Summary", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The <i>Status Fill Follower</i> component can be used to follow the current fill of an image to draw attention to the amount that it is at.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		/* HOW TO CREATE */
		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To create a Status Fill Follower for your Ultimate Status Bar you will need to create a new image that is a child of the Ultimate Status Bar. To do this, you can right click on the Ultimate Status Bar GameObject and select: <i>UI / Image</i>. Then simply add the Status Fill Follower component to the new GameObject. This can be done by selecting: <i>Add Component / UI / Ultimate Status Bar / Status Fill Follower.</i>", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Inspector Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option on this component, please select the Status Fill Follower in your scene and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Documentation ()
	{
		StartPage( documentation );

		EditorGUILayout.LabelField( "Introduction", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "Welcome to the Documentation section. This section will go over the various functions that you have available. Please click on the class to learn more about each function.", paragraphStyle );

		//EditorGUILayout.LabelField( "Please click on the class to learn more about the functions available.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		// UltimateStatusBar.cs
		EditorGUILayout.LabelField( "UltimateStatusBar.cs", itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( documentation_USB );
			GUI.FocusControl( "" );
		}
		
		// AlternateStateHandler.cs
		EditorGUILayout.LabelField( "AlternateStateHandler.cs", itemHeaderStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( documentation_ASH );
			GUI.FocusControl( "" );
		}

		EndPage();
	}

	void Documentation_UltimateStatusBar ()
	{
		StartPage( documentation_USB );
		
		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The following functions can be referenced from your scripts without the need for an assigned local Ultimate Status Bar variable. However, each function must have the targeted Ultimate Status Bar name in order to find the correct Ultimate Status Bar in the scene. Each example code provided uses the name <i>Player</i> as the status bar name.", paragraphStyle );
		
		Vector2 ratio = new Vector2( readme.scriptReference.width, readme.scriptReference.height ) / ( readme.scriptReference.width > readme.scriptReference.height ? readme.scriptReference.width : readme.scriptReference.height );

		float imageWidth = readme.scriptReference.width > Screen.width - 50 ? Screen.width - 50 : readme.scriptReference.width;

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( readme.scriptReference, GUILayout.Width( imageWidth ), GUILayout.Height( imageWidth * ratio.y ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatusBar_StaticFunctions.Length; i++ )
			ShowDocumentation( UltimateStatusBar_StaticFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Ultimate Status Bar class. Each example provided relies on having a Ultimate Status Bar variable named 'statusBar' stored inside your script. When using any of the example code provided, make sure that you have a Ultimate Status Bar variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place this in your public variables and assign it in the inspector. //\npublic UltimateStatusBar statusBar;", GUI.skin.textArea );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatusBar_PublicFunctions.Length; i++ )
			ShowDocumentation( UltimateStatusBar_PublicFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		/* //// --------------------------- < PUBLIC CLASSES > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Classes", sectionHeaderStyle );

		GUILayout.Space( itemHeaderSpace );

		// UltimateStatus
		EditorGUILayout.LabelField( "UltimateStatus", itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			NavigateForward( documentation_US );
			GUI.FocusControl( "" );
		}
		
		EndPage();
	}

	void Documentation_UltimateStatus ()
	{
		StartPage( documentation_US );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to an Ultimate Status class. Since this class is nested under the Ultimate Status Bar class, you will need to have a reference to an Ultimate Status Bar component first, like the code below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place these with your variables and assign the Ultimate Status Bar in the inspector. //\npublic UltimateStatusBar statusBar;\nUltimateStatusBar.UltimateStatus status;", GUI.skin.textArea );

		EditorGUILayout.LabelField( Indent + "After you have a reference to an Ultimate Status Bar you can get a reference to an Ultimate Status component. The examples provided rely on having an Ultimate Status variable named 'status' stored inside your script. When using any of the example code provided, make sure that you have a Ultimate Status variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place this in the Start() function. //\nstatus = statusBar.GetUltimateStatus( \"Status Name\" );", GUI.skin.textArea );

		EditorGUILayout.LabelField( "Be sure to change the \"Status Name\" to the name of your status that you are trying to access.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateStatus_PublicFunctions.Length; i++ )
			ShowDocumentation( UltimateStatus_PublicFunctions[ i ] );

		GUILayout.Space( itemHeaderSpace );

		EndPage();
	}

	void Documentation_AlternateStateHandler ()
	{
		StartPage( documentation_ASH );

		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All static functions require a string to be passed through the function first. Each Alternate State Handler is registered with the targeted Ultimate Status Bar's name. The <i>statusBarName</i> parameter is used to locate the targeted Alternate State Handler from a static list of Alternate State Handler classes that have been stored.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < AlternateStateHandler_StaticFunctions.Length; i++ )
			ShowDocumentation( AlternateStateHandler_StaticFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Alternate State Handler class. Each example provided relies on having a Alternate State Handler variable named 'altStateHandler' stored inside your script. When using any of the example code provided, make sure that you have a Alternate State Handler variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place this in your public variables and assign it in the inspector. //\npublic AlternateStateHandler altStateHandler;", GUI.skin.textArea );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < AlternateStateHandler_PublicFunctions.Length; i++ )
			ShowDocumentation( AlternateStateHandler_PublicFunctions[ i ] );
		
		EndPage();
	}

	void VersionHistory ()
	{
		StartPage( versionHistory );

		for( int i = 0; i < readme.versionHistory.Length; i++ )
		{
			EditorGUILayout.LabelField( "Version " + readme.versionHistory[ i ].versionNumber, itemHeaderStyle );

			for( int n = 0; n < readme.versionHistory[ i ].changes.Length; n++ )
				EditorGUILayout.LabelField( "  • " + readme.versionHistory[ i ].changes[ n ] + ".", paragraphStyle );

			if( i < ( readme.versionHistory.Length - 1 ) )
				GUILayout.Space( itemHeaderSpace );
		}

		EndPage();
	}

	void ImportantChange ()
	{
		StartPage( importantChange );

		EditorGUILayout.LabelField( Indent + "Thank you for downloading the most recent version of the Ultimate Status Bar. If you are experiencing any errors, please completely remove the Ultimate Status Bar from your project and re-import it. As always, if you run into any issues with the Ultimate Status Bar, please contact us at:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "NEW FILES", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  • UltimateStatusBarReadme.cs", paragraphStyle );
		EditorGUILayout.LabelField( "  • UltimateStatusBarReadmeEditor.cs", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "OLD FILES", sectionHeaderStyle );
		EditorGUILayout.LabelField( "The file listed below is no longer used, and should be removed from your project. All the information that was previously inside this script is now included in the Ultimate Status Bar README.", paragraphStyle );
		EditorGUILayout.LabelField( "  • UltimateStatusBarWindow.cs", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Got it!", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void ThankYou ()
	{
		StartPage( thankYou );

		EditorGUILayout.LabelField( "The two of us at Tank & Healer Studio would like to thank you for purchasing the Ultimate Status Bar asset package from the Unity Asset Store.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "We hope that the Ultimate Status Bar will be a great help to you in the development of your game. After clicking the <i>Continue</i> button below, you will be presented with information to assist you in getting to know the Ultimate Status Bar and getting it implementing into your project.", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "You can access this information at any time by clicking on the <b>README</b> file inside the Ultimate Status Bar folder.", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Again, thank you for downloading the Ultimate Status Bar. We hope that your project is a success!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		var rect2 = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect2, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}
	
	void ShowDocumentation ( DocumentationInfo info )
	{
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( info.functionName, itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) && ( info.showMore.faded == 0.0f || info.showMore.faded == 1.0f ) )
		{
			info.showMore.target = !info.showMore.target;
			GUI.FocusControl( "" );
		}

		if( EditorGUILayout.BeginFadeGroup( info.showMore.faded ) )
		{
			EditorGUILayout.LabelField( Indent + "<i>Description:</i> " + info.description, paragraphStyle );

			if( info.parameter != null )
			{
				for( int i = 0; i < info.parameter.Length; i++ )
					EditorGUILayout.LabelField( Indent + "<i>Parameter:</i> " + info.parameter[ i ], paragraphStyle );
			}
			if( info.returnType != string.Empty )
				EditorGUILayout.LabelField( Indent + "<i>Return type:</i> " + info.returnType, paragraphStyle );

			if( info.codeExample != string.Empty )
				EditorGUILayout.TextArea( info.codeExample, GUI.skin.textArea );

			GUILayout.Space( paragraphSpace );
		}
		EditorGUILayout.EndFadeGroup();
	}

	[MenuItem( "Window/Tank and Healer Studio/Ultimate Status Bar", false, 10 )]
	public static void SelectReadmeFile ()
	{
		var ids = AssetDatabase.FindAssets( "README t:UltimateStatusBarReadme" );
		if( ids.Length == 1 )
		{
			var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
			Selection.objects = new Object[] { readmeObject };
			readme = ( UltimateStatusBarReadme )readmeObject;
		}
		else
		{
			Debug.LogError( "There is no README object in the Ultimate Status Bar folder." );
		}
	}

	public static void OpenReadmeDocumentation ()
	{
		var ids = AssetDatabase.FindAssets( "README t:UltimateStatusBarReadme" );
		if( ids.Length == 1 )
		{
			var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
			Selection.objects = new Object[] { readmeObject };
			readme = ( UltimateStatusBarReadme )readmeObject;

			if( !pageHistory.Contains( documentation ) )
				NavigateForward( documentation );

			if( !pageHistory.Contains( documentation_USB ) )
				NavigateForward( documentation_USB );
		}
		else
		{
			Debug.LogError( "There is no README object in the Ultimate Status Bar folder." );
		}
	}
}