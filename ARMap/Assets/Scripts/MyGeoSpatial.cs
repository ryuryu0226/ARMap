using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;

public class MyGeoSpatial : MonoBehaviour
{
    [Header("AR Components")]

    /// <summary>
    /// The AREarthManager used in the sample.
    /// </summary>
    public AREarthManager EarthManager;

    /// <summary>
    /// The ARCoreExtensions used in the sample.
    /// </summary>
    public ARCoreExtensions ARCoreExtensions;

    [SerializeField] Text infoText;

    // Update is called once per frame
    void Update()
    {
        var featureSupport = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (featureSupport)
        {
            case FeatureSupported.Unknown:
                this.infoText.text = "FeatureSupported UnKnown";
                return;
            case FeatureSupported.Unsupported:
                this.infoText.text = "Geospatial API is not supported by this devices.";
                return;
            case FeatureSupported.Supported:
                if (ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode ==
                    GeospatialMode.Disabled)
                {
                    Debug.Log("Geospatial sample switched to GeospatialMode.Enabled.");
                    ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode =
                        GeospatialMode.Enabled;
                    return;
                }

                break;
        }

        var earthTrackingState = EarthManager.EarthTrackingState;
        var pose = earthTrackingState == TrackingState.Tracking ?
            EarthManager.CameraGeospatialPose : new GeospatialPose();
        if (earthTrackingState == TrackingState.Tracking)
        {
            this.infoText.text = string.Format(
            "Latitude/Longitude: {1}°, {2}°{0}" +
            "Horizontal Accuracy: {3}m{0}" +
            "Altitude: {4}m{0}" +
            "Vertical Accuracy: {5}m{0}" +
            "Heading: {6}°{0}" +
            "Heading Accuracy: {7}°",
            System.Environment.NewLine, // 改行
            pose.Latitude.ToString("F6"), // 緯度
            pose.Longitude.ToString("F6"), // 経度
            pose.HorizontalAccuracy.ToString("F6"), // 水平方向の精度
            pose.Altitude.ToString("F2"), // 高度
            pose.VerticalAccuracy.ToString("F2"), // 高度の精度
            pose.Heading.ToString("F1"), // 方位
            pose.HeadingAccuracy.ToString("F1")); // 方位の精度
        }
        else
        {
            this.infoText.text = "GEOSPATIAL POSE: not tracking";
        }
    }
}
