﻿--------------------
     FairyGUI
Copyright © 2014-2020 FairyGUI.com
Version 3.2.0
support@fairygui.com
--------------------
FairyGUI is a flexible UI framework for Unity, working with the professional FREE Game UI Editor: FairyGUI Editor. 
Download the editor from here: http://en.fairygui.com/

--------------------
Get Started
--------------------
Run demos in Assets/FairyGUI/Examples/Scenes.
The UI project is in Examples-UIProject.zip, unzip it anywhere. Use FairyGUI Editor to open it.

Using FairyGUI in Unity:
* Place a UIPanel in scene by using editor menu GameObject/FairyGUI/UIPanel.
* Or using UIPackage.CreateObject to create UI component dynamically, and then use GRoot.inst.AddChild to show it on screen.

-----------------
 Version History
-----------------
3.2.0
- NEW: Add DisplayObjectInfo component. Define script symbol FAIRYGUI_TEST to enable it.
- FIXED: A virtual list scrolling bug.
- FIXED: A BlendMode bug.

3.1.0
- NEW: Draw extra 8 directions instead of 4 directions to archive text outline effect. Toggle option is UIConfig.enhancedTextOutlineEffect.
- IMPROVED: Eexecution efficiency optimizations.
- IMPROVED: GoWrapper now supports multiple materials.
- FIXED: Correct cleanup action for RemovePackage.

3.0.0
From this version, we change package data format to binary. Editor version 3.9.0  with 'binary format' option checked in publish dialog is required to generating this kind of format. Old XML format is not supported anymore.

- NEW: Add UIPackage.UnloadAssets and UIPackage.ReloadAssets to allow unload package resources without removing the package. 
- NEW: Add TransitionActionType.Text and TransitionActionType.Icon.

2.4.0
- NEW: GTween is introduced, DOTween is no longer used inside FairyGUI.
- NEW: Transitions now support playing partially and pausing.
- IMPROVED: Change the way of registering bitmap font. 
- FIXED: A GButton pivot issue.
- FIXED: Correct text align behavior.

2.3.0
- NEW: Allow loader to load component.
- NEW: Add text template feature.
- FIXED: Exclude invisible object in FairyBatching.

2.2.0
- NEW: Modify shaders to fit linear color space.
- IMPROVED: Improve relation system to handle conditions that anchor is set.
- IMPROVED: Eliminate GC in transition.
- FIXED: Fixed a bug of unloading bundle in UIPackage.
- FIXED: Fixed issue that some blend mode(multiply, screen) works improperly.

2.1.0
- NEW: Add GGraph.DrawRoundRect.
- NEW: Add FillType.ScaleNoBorder.
- FIXED: Repair potential problems of virtual list.
- FIXED: Fixed a bug in handling shared materials.

2.0.0
- NEW: RTL Text rendering support. Define scripting symbols RTL_TEXT_SUPPORT to enabled it.
- NEW: Support for setting GObject.data in editor.
- NEW: Support for setting selectedIcon of list item in editor.
- IMPROVED: Add UIConfig.depthSupportForPaitingMode.
- IMPROVED: Set sorting order of popup automatically.
- FIXED: Fixed a text layout bug when line spacing is negative.
- FIXED: Reset ScrollPane.draggingPane when an active scrollPane is being disposed.
- FIXED: Fixed a bug of skew rendering.