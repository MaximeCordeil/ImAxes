#### What changed when updating to Unity 2018.2.1f1?

* TextMeshPro files shifted to Unity's built-in package manager system
* OVRLint.cs commented out deprecated PlayerSettings.mobileMTRendering (lines 120-122)
* Scripting Runtime Version changed from .NET 3.5 Equivalent to .NET 4.x Equivalent
* All ambiguous references of Tuple between Staxes and System preceeded with Staxes namespace
* Axis prefab updated with new TextMeshPro scripts (using in-built LiberationSans font instead)


#### What might be broken in this version?

* Support for VR devices other than the HTC Vive