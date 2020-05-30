using System;
using UnityInjector;
using UnityEngine;
using UnityInjector.Attributes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExIni;
using System.Linq;

namespace COM3D2.VyperCam.Plugin
{
    [PluginFilter("COM3D2x64"), PluginFilter("COM3D2VRx64"), PluginFilter("COM3D2OHx64"), PluginFilter("COM3D2OHVRx64"), PluginName("VyperCam"), PluginVersion("1.0.0.0")]
    public class VyperCam : PluginBase
    {
        private WebCamDevice[] devices = WebCamTexture.devices;
        private static WebCamTexture webcamTex;

        private bool objectcreated = false;

        private GameObject panel;

        KeyCode togglekey = KeyCode.F6;
        private int webcamid = 0;

        private bool vr;

        public void Start()
        {
            LoadConfig();
            SceneManager.sceneLoaded += OnLevelFinishedLoading;

            vr = Environment.GetCommandLineArgs().Any(s => s.ToLower().Contains("/vr"));

            if (!vr)
            {
                Console.WriteLine("[VyperCam] Game is not in vr mode, disabling plugin...");
            }
            else
            {
                if (devices.Length > 0)
                {
                    webcamTex = new WebCamTexture(WebCamTexture.devices[webcamid].name);
                    Console.WriteLine("[VyperCam] Webcam loaded: " + webcamTex.deviceName);
                }
                else
                    Console.WriteLine("[VyperCam] Can't find any camera");
            }
        }

        private void LoadConfig()
        {
            try
            {
                IniKey key = Preferences["Config"]["ToggleKey"];
                IniKey id = Preferences["Config"]["WebcamID"];

                togglekey = (KeyCode)Enum.Parse(typeof(KeyCode), key.Value, true);
                webcamid = int.Parse(id.Value);
                Console.WriteLine("[VyperCam] Configuration loaded correctly!");
            }
            catch (Exception e)
            {
                Console.WriteLine("[VyperCam] There was an error during the configuration loading: " +  e.ToString());
            }
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (vr)
            {
                Console.WriteLine("[VyperCam] Updating camera UI...");

                if (!objectcreated)
                {
                    var uiRoot = GameObject.Find("SystemUI Root");

                    panel = new GameObject();
                    panel.name = "VyperCam UI";
                    panel.transform.parent = uiRoot.transform;
                    panel.transform.localPosition = Vector3.zero;
                    panel.transform.localRotation = Quaternion.identity;
                    panel.transform.localScale = Vector3.one;
                    panel.layer = uiRoot.layer;

                    var canvas = panel.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.renderMode = RenderMode.WorldSpace;

                    var rect = panel.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(500f, 500f);
                    rect.pivot = new Vector2(1, 0);
                    rect.localPosition = new Vector2(960, -540);

                    var image = panel.AddComponent<RawImage>();
                    image.transform.SetParent(panel.transform, false);
                    image.material.mainTexture = webcamTex;
                    image.enabled = false;
                    objectcreated = true;
                }

                webcamTex.Play();

                Console.WriteLine("[VyperCam] Done!");
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(togglekey) && vr)
            {
                Console.WriteLine("[VyperCam] Hide/Show Camera");
                panel.GetComponent<RawImage>().enabled = !panel.GetComponent<RawImage>().enabled;
            }
        }
    }
}