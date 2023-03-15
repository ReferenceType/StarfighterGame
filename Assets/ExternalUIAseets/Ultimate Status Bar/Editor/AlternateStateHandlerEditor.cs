/* AlternateStateHandlerEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor( typeof( AlternateStateHandler ) )]
public class AlternateStateHandlerEditor : Editor
{
	AlternateStateHandler targ;
	List<string> statusOptions, alternateStates;
	int selectionIndex = 0;

	SerializedProperty ultimateStatusBar;
	List<SerializedProperty> statusIndex, alternateStateName, triggerOption;
	List<SerializedProperty> alternateStateImage, alternateStateColor, flashing;
	List<SerializedProperty> flashingColor, flashingSpeed, triggerValue;
	List<SerializedProperty> triggerBy, stateType, alternateStateText;
	List<SerializedProperty> defaultStateColor, flashingDuration;
	
	List<bool> AlternateStateAdvanced, DefaultColorHelp;
	List<bool> AlternateStateNameError;
	

	void OnEnable ()
	{
		StoreReferences();
		
		AlternateStateAdvanced = new List<bool>();

		for( int i = 0; i < targ.AlternateStateList.Count; i++ )
			AlternateStateAdvanced.Add( EditorPrefs.GetBool( "UUI_USB_ALT_Advanced" + i.ToString() ) );

		Undo.undoRedoPerformed += UndoRedoCallback;
	}

	void OnDisable ()
	{
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	void UndoRedoCallback ()
	{
		StoreReferences();
	}
	
	void DisplayHeaderDropdown ( string headerName, string editorPref )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		if( GUILayout.Button( EditorPrefs.GetBool( editorPref ) == true ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			EditorPrefs.SetBool( editorPref, EditorPrefs.GetBool( editorPref ) == true ? false : true );

		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}
		
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( ultimateStatusBar );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			
			StoreStatusOptions();
		}
		
		if( targ.ultimateStatusBar == null )
		{
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.HelpBox( "Please assign the targeted Ultimate Status Bar before continuing.", MessageType.Warning );
			if( GUILayout.Button( "Find Status Bar" ) )
			{
				targ.ultimateStatusBar = targ.gameObject.GetComponentInParent<UltimateStatusBar>();
				EditorUtility.SetDirty( targ );
				
				StoreStatusOptions();

				if( targ.ultimateStatusBar == null )
					Debug.LogWarning( "Alternate State Handler - Could not find an Ultimate Status Bar component in any parent GameObjects." );
			}
			EditorGUILayout.EndVertical();
		}
		
		if( targ.ultimateStatusBar != null )
		{
			EditorGUILayout.Space();
			
			/* ---------------------------------- > ALTERNATE STATES < ---------------------------------- */
			DisplayHeaderDropdown( "Alternate States", "UUI_ASH_AlternateStates" );
			if( EditorPrefs.GetBool( "UUI_ASH_AlternateStates" ) )
			{
				for( int i = 0; i < targ.AlternateStateList.Count; i++ )
				{
					EditorGUILayout.BeginVertical( "Box" );
					GUILayout.Space( 1 );

					// ----- < STATE NAME > ----- //
					if( alternateStateName[ i ].stringValue == string.Empty && Event.current.type == EventType.Repaint )
					{
						GUIStyle style = new GUIStyle( GUI.skin.textField );
						style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
						EditorGUILayout.TextField( new GUIContent( "State Name", "The unique name to be used in reference to this state." ), "State Name", style );
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( alternateStateName[ i ], new GUIContent( "State Name", "The unique name to be used in reference to this state." ) );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();
							StoreAlternateStates();
							AlternateStateNameError[ i ] = DuplicateAlternateStateName( i );
						}
					}

					if( AlternateStateNameError[ i ] )
					{
						EditorGUILayout.HelpBox( "State Name is already in use.", MessageType.Error );
						EditorGUILayout.Space();
					}
					// ----- < END STATE NAME > ----- //

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( defaultStateColor[ i ], new GUIContent( "Default Color", "The default color for the state to return to." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();
						DefaultColorHelp[ i ] = targ.ultimateStatusBar != null && targ.AlternateStateList[ i ].triggerOption != AlternateStateHandler.AlternateState.TriggerOption.Manual && targ.AlternateStateList[ i ].defaultStateColor != targ.ultimateStatusBar.UltimateStatusList[ targ.AlternateStateList[ i ].statusIndex ].statusColor;
					}

					if( DefaultColorHelp[ i ] )
					{
						if( GUILayout.Button( "Copy Status Color", EditorStyles.miniButton ) )
						{
							defaultStateColor[ i ].colorValue = targ.ultimateStatusBar.UltimateStatusList[ targ.AlternateStateList[ i ].statusIndex ].statusColor;
							serializedObject.ApplyModifiedProperties();
							DefaultColorHelp[ i ] = targ.ultimateStatusBar != null && targ.AlternateStateList[ i ].triggerOption != AlternateStateHandler.AlternateState.TriggerOption.Manual && targ.AlternateStateList[ i ].defaultStateColor != targ.ultimateStatusBar.UltimateStatusList[ targ.AlternateStateList[ i ].statusIndex ].statusColor;
						}
					}

					if( AlternateStateAdvanced[ i ] )
					{
						EditorGUILayout.Space();

						EditorGUILayout.LabelField( alternateStateName[ i ].stringValue == string.Empty ? "Advanced Options" : alternateStateName[ i ].stringValue + " Options", EditorStyles.boldLabel );

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( triggerOption[ i ], new GUIContent( "Trigger Option", "Determines how the state will be switched." ) );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();
							StoreAlternateStates();
							DefaultColorHelp[ i ] = targ.ultimateStatusBar != null && targ.AlternateStateList[ i ].triggerOption != AlternateStateHandler.AlternateState.TriggerOption.Manual && targ.AlternateStateList[ i ].defaultStateColor != targ.ultimateStatusBar.UltimateStatusList[ targ.AlternateStateList[ i ].statusIndex ].statusColor;
						}

						if( targ.AlternateStateList[ i ].triggerOption == AlternateStateHandler.AlternateState.TriggerOption.Percentage )
						{
							EditorGUI.indentLevel = 1;
							for( int n = 0; n < targ.ultimateStatusBar.UltimateStatusList.Count; n++ )
							{
								if( targ.ultimateStatusBar.UltimateStatusList[ n ].statusName == string.Empty )
									continue;

								if( !statusOptions.Contains( targ.ultimateStatusBar.UltimateStatusList[ n ].statusName ) )
								{
									StoreStatusOptions();
									break;
								}
							}
							if( targ.ultimateStatusBar.UltimateStatusList.Count > 1 || targ.ultimateStatusBar.UltimateStatusList[ 0 ].statusName != string.Empty )
							{
								EditorGUI.BeginChangeCheck();
								statusIndex[ i ].intValue = EditorGUILayout.Popup( "Status Name", statusIndex[ i ].intValue, statusOptions.ToArray() );
								if( EditorGUI.EndChangeCheck() )
								{
									serializedObject.ApplyModifiedProperties();
									DefaultColorHelp[ i ] = targ.ultimateStatusBar != null && targ.AlternateStateList[ i ].triggerOption != AlternateStateHandler.AlternateState.TriggerOption.Manual && targ.AlternateStateList[ i ].defaultStateColor != targ.ultimateStatusBar.UltimateStatusList[ targ.AlternateStateList[ i ].statusIndex ].statusColor;
								}
							}

							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( triggerBy[ i ], new GUIContent( "Trigger By", "Determines how to compare the status amount to the trigger value." ) );
							EditorGUI.indentLevel = 2;
							EditorGUILayout.Slider( triggerValue[ i ], 0.0f, 1.0f, new GUIContent( "Value", "The value at which the state will trigger." ) );
							if( EditorGUI.EndChangeCheck() )
								serializedObject.ApplyModifiedProperties();
							EditorGUI.indentLevel = 0;
							EditorGUILayout.Space();
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( alternateStateColor[ i ], new GUIContent( "State Color", "The color to be applied for the state." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( stateType[ i ], new GUIContent( "State Type", "Determines what should be used to visually display the state." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();

						EditorGUI.BeginChangeCheck();
						EditorGUI.indentLevel = 1;
						if( targ.AlternateStateList[ i ].stateType == AlternateStateHandler.AlternateState.StateType.Image )
							EditorGUILayout.PropertyField( alternateStateImage[ i ], new GUIContent( "State Image", "The image to be used for the state." ) );

						if( targ.AlternateStateList[ i ].stateType == AlternateStateHandler.AlternateState.StateType.Text )
							EditorGUILayout.PropertyField( alternateStateText[ i ], new GUIContent( "State Text", "The Text component to be used for the state." ) );

						EditorGUI.indentLevel = 0;
						EditorGUILayout.Space();
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( flashing[ i ], new GUIContent( "Flashing", "Determines whether of not the state should flash between two colors or not." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();

						if( targ.AlternateStateList[ i ].flashing )
						{
							EditorGUI.indentLevel = 1;
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( flashingColor[ i ], new GUIContent( "Flashing Color", "The color of the flash." ) );
							if( EditorGUI.EndChangeCheck() )
								serializedObject.ApplyModifiedProperties();

							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( flashingSpeed[ i ], new GUIContent( "Flashing Speed", "Controls the speed of the flash." ) );
							EditorGUILayout.PropertyField( flashingDuration[ i ], new GUIContent( "Flashing Duration", "Controls how long the state should flash. Use 0 for no duration." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								if( flashingSpeed[ i ].floatValue < 0 )
									flashingSpeed[ i ].floatValue = 0;
								if( flashingDuration[ i ].floatValue < 0 )
									flashingDuration[ i ].floatValue = 0;
								serializedObject.ApplyModifiedProperties();
							}
							EditorGUI.indentLevel = 0;
						}

						EditorGUILayout.Space();
					}

					// ----- < EDIT TOOLBAR > ---- //
					EditorGUILayout.BeginHorizontal();
					if( GUILayout.Button( AlternateStateAdvanced[ i ] == true ? "Hide Options" : "Show Options", EditorStyles.miniButtonLeft ) )
					{
						AlternateStateAdvanced[ i ] = !AlternateStateAdvanced[ i ];
						EditorPrefs.SetBool( "UUI_USB_ALT_Advanced" + i.ToString(), AlternateStateAdvanced[ i ] );
					}
					EditorGUI.BeginDisabledGroup( Application.isPlaying == true );
					if( GUILayout.Button( "Create", EditorStyles.miniButtonMid ) )
					{
						AddNewState( i + 1 );
					}
					EditorGUI.BeginDisabledGroup( targ.AlternateStateList.Count == 1 );
					if( GUILayout.Button( "Delete", EditorStyles.miniButtonRight ) )
					{
						if( EditorUtility.DisplayDialog( "Alternate State Handler", "Warning!\n\nAre you sure that you want to delete " + ( alternateStateName[ i ].stringValue != string.Empty ? "the " + alternateStateName[ i ].stringValue : "this" ) + " state?", "Yes", "No" ) )
						{
							RemoveState( i );
							continue;
						}
					}
					EditorGUI.EndDisabledGroup();
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					// ----- < END EDIT TOOLBAR > ---- //

					GUILayout.Space( 1 );
					EditorGUILayout.EndVertical();
				}
			}
			/* -------------------------------- > END ALTERNATE STATES < -------------------------------- */
			
			EditorGUILayout.Space();

			/* ---------------------------------- > SCRIPT REFERENCE < ---------------------------------- */
			DisplayHeaderDropdown( "Script Reference", "UUI_ASH_ScriptReference" );
			if( EditorPrefs.GetBool( "UUI_ASH_ScriptReference" ) )
			{
				EditorGUILayout.Space();
				
				if( targ.ultimateStatusBar.statusBarName == string.Empty )
					EditorGUILayout.HelpBox( "The assigned Ultimate Status Bar has not been named.", MessageType.Error );

				if( alternateStates.Count == 0 )
					EditorGUILayout.HelpBox( "There are no named manual Alternate States to change through code.", MessageType.Warning );

				if( targ.ultimateStatusBar.statusBarName != string.Empty && alternateStates.Count > 0 )
				{
					EditorGUILayout.LabelField( "Ultimate Status Bar: " + ( targ.ultimateStatusBar.statusBarName == string.Empty ? "Unknown" : targ.ultimateStatusBar.statusBarName ) );
					
					if( selectionIndex > ( alternateStates.Count - 1 ) )
						selectionIndex = 0;

					EditorGUILayout.BeginVertical( "Box" );
					GUILayout.Space( 1 );
					EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );
					selectionIndex = EditorGUILayout.Popup( "Alternate State", selectionIndex, alternateStates.ToArray() );

					if( selectionIndex <= ( alternateStates.Count - 1 ) )
						EditorGUILayout.TextField( "AlternateStateHandler.SwitchState( \"" + targ.ultimateStatusBar.statusBarName + "\", \"" + alternateStates[ selectionIndex ] + "\", targetState );" );

					GUILayout.Space( 1 );
					EditorGUILayout.EndVertical();
				}
			}
			/* -------------------------------- > END SCRIPT REFERENCE < -------------------------------- */
		}

		EditorGUILayout.Space();
		Repaint();
	}

	void AddNewState ( int index )
	{
		serializedObject.FindProperty( "AlternateStateList" ).InsertArrayElementAtIndex( index );
		serializedObject.ApplyModifiedProperties();
		
		// Assign default values so that the previous index values are not copied.
		targ.AlternateStateList[ index ] = new AlternateStateHandler.AlternateState();
		AlternateStateAdvanced.Insert( index, false );

		EditorUtility.SetDirty( targ );

		// Store the references to get the information.
		StoreReferences();
	}

	void RemoveState ( int index )
	{
		serializedObject.FindProperty( "AlternateStateList" ).DeleteArrayElementAtIndex( index );
		serializedObject.ApplyModifiedProperties();
		
		AlternateStateAdvanced.RemoveAt( index );

		StoreReferences();
	}

	void StoreStatusOptions ()
	{
		statusOptions = new List<string>();
		if( targ.ultimateStatusBar != null )
		{
			for( int i = 0; i < targ.ultimateStatusBar.UltimateStatusList.Count; i++ )
			{
				if( targ.ultimateStatusBar.UltimateStatusList[ i ].statusName != string.Empty )
					statusOptions.Add( targ.ultimateStatusBar.UltimateStatusList[ i ].statusName );
			}
		}
	}

	void StoreAlternateStates ()
	{
		alternateStates = new List<string>();
		if( targ.AlternateStateList.Count > 0 )
		{
			for( int i = 0; i < targ.AlternateStateList.Count; i++ )
			{
				if( targ.AlternateStateList[ i ].triggerOption == AlternateStateHandler.AlternateState.TriggerOption.Manual && targ.AlternateStateList[ i ].alternateStateName != string.Empty )
					alternateStates.Add( targ.AlternateStateList[ i ].alternateStateName );
			}
		}
	}

	bool DuplicateAlternateStateName ( int index )
	{
		if( alternateStateName[ index ].stringValue == string.Empty )
			return false;

		for( int i = 0; i < alternateStateName.Count; i++ )
		{
			if( i == index )
				continue;

			if( alternateStateName[ i ].stringValue == alternateStateName[ index ].stringValue )
				return true;
		}
		return false;
	}

	void StoreReferences ()
	{
		targ = ( AlternateStateHandler )target;

		ultimateStatusBar = serializedObject.FindProperty( "ultimateStatusBar" );

		if( targ.ultimateStatusBar == null && targ.GetComponent<UltimateStatusBar>() )
		{
			ultimateStatusBar.objectReferenceValue = targ.GetComponent<UltimateStatusBar>();
			serializedObject.ApplyModifiedProperties();
		}

		// If the status bar has no alt state information registered, then create a new state.
		if( targ.AlternateStateList.Count == 0 )
		{
			serializedObject.FindProperty( "AlternateStateList" ).arraySize++;
			serializedObject.ApplyModifiedProperties();
			targ.AlternateStateList[ 0 ] = new AlternateStateHandler.AlternateState();
			EditorUtility.SetDirty( targ );
		}

		// Reset property lists
		defaultStateColor = new List<SerializedProperty>();
		alternateStateName = new List<SerializedProperty>();
		triggerOption = new List<SerializedProperty>();
		alternateStateImage = new List<SerializedProperty>();
		alternateStateColor = new List<SerializedProperty>();
		flashing = new List<SerializedProperty>();
		flashingColor = new List<SerializedProperty>();
		flashingSpeed = new List<SerializedProperty>();
		triggerValue = new List<SerializedProperty>();
		triggerBy = new List<SerializedProperty>();
		stateType = new List<SerializedProperty>();
		alternateStateText = new List<SerializedProperty>();
		statusIndex = new List<SerializedProperty>();
		flashingDuration = new List<SerializedProperty>();
		
		DefaultColorHelp = new List<bool>();
		AlternateStateNameError = new List<bool>();

		for( int i = 0; i < targ.AlternateStateList.Count; i++ )
		{
			// Add properties
			statusIndex.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].statusIndex", i ) ) );
			alternateStateName.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].alternateStateName", i ) ) );
			triggerOption.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].triggerOption", i ) ) );
			alternateStateImage.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].alternateStateImage", i ) ) );
			alternateStateColor.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].alternateStateColor", i ) ) );
			flashing.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].flashing", i ) ) );
			flashingColor.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].flashingColor", i ) ) );
			flashingSpeed.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].flashingSpeed", i ) ) );
			triggerValue.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].triggerValue", i ) ) );
			triggerBy.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].triggerBy", i ) ) );
			stateType.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].stateType", i ) ) );
			alternateStateText.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].alternateStateText", i ) ) );
			defaultStateColor.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].defaultStateColor", i ) ) );
			flashingDuration.Add( serializedObject.FindProperty( string.Format( "AlternateStateList.Array.data[{0}].flashingDuration", i ) ) );
			
			DefaultColorHelp.Add( targ.ultimateStatusBar != null && targ.AlternateStateList[ i ].triggerOption != AlternateStateHandler.AlternateState.TriggerOption.Manual && targ.AlternateStateList[ i ].defaultStateColor != targ.ultimateStatusBar.UltimateStatusList[ targ.AlternateStateList[ i ].statusIndex ].statusColor );

			AlternateStateNameError.Add( targ.AlternateStateList[ i ].alternateStateName != string.Empty && DuplicateAlternateStateName( i ) );
		}

		StoreStatusOptions();
		StoreAlternateStates();
	}
}