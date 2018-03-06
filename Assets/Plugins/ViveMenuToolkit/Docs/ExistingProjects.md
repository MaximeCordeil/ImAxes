# Importing the toolkit into existing projects

- Download and extract the zip file of this repository. 
- Copy the ViveMenuToolkit folder in the Assets folder.
- Paste the ViveMenuToolkit folder into the Assets folder of your existing project.

Note: You must have the SteamVR Plugin installed in the existing project. 

It is easiest to drag and drop the CameraRig prefab into the project. However if you are adapting an existing SteamVR CameraRig, then the following is required:
- Drop the VRInputModule prefab onto the existing CameraRig.
- Attach the following components to both controllers:
    1. ControllerEventSystem
    2. ViveUILaserPointer
    3. SteamVR_TrackedController
    
- When creating a ViveMenu (or dragging in a BlankViveMenu prefab), drag and drop the CameraRig into the Camera field of the ViveMenu script.