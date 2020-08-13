# COM3D2.VyperCam.Plugin

![Logo](https://imgur.com/nnxqkYm.png)

A plugin dedicated to see a webcam inside COM3D2 VR Mode. Useful to check your room's door for your lewd needs

# Features
- Using unity built-in "WebcamTexture" for USB connected cameras
- Selectable camera through ini
- IP Cameras with HTTP authentication
- Enable/Disable button

# Pre-compiled DLL
To download the pre-compiled dll you can grab the latest version in the [Releases Page](https://github.com/Rex109/COM3D2.VyperCam.Plugin/releases/)

# IP Camera configuration
To configure an ip camera with VyperCam you will need its JPEG screenshot URL. This is usually the local ip address of the webcam and some GET variables defined by the webcam's vendor, the most common variable is ?action=snapshot. In addition if you set a user/password combination on your camera you will need to include them in the URL.

Example: http://user:password@192.168.1.127/cam/feed.cgi?action=snapshot
