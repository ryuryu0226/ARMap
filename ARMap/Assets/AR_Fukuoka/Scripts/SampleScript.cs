using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//ARFoundationとARCoreExtensions関連を使用する
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using AR_Fukuoka;
    public class SampleScript : MonoBehaviour
    {
        public AREarthManager EarthManager; //GeospatialAPIを⽤いたトラッキング情報
        public VpsInitializer Initializer; // GeospatialAPIとARCoreの初期化と結果
        public Text OutputText; // 結果表⽰⽤のUI
        public double HeadingThreshold = 25; // ⽅位の許容精度
        public double HorizontalThreshold = 20; // ⽔平位置の許容精度
        public double Latitude; // オブジェクトを置く緯度
        public double Longitude; // オブジェクトを置く緯度
        public double Altitude; // オブジェクトを置く⾼さ
        public double Heading; // オブジェクトの⽅位 (北=0°)
        public GameObject ContentPrefab; //表⽰オブジェクトの元データ
        GameObject displayObject; //実際に表⽰するオブジェクト
        public ARAnchorManager AnchorManager; //アンカー作成に使⽤
        void Start()
        {
            // GameObject zero = Instantiate(ContentPrefab, Vector3.zero, Quaternion.identity);
        }
        // Update is called once per frame
        void Update()
        {
            string status = "";

            // 初期化失敗またはトラッキングができたいない場合は何もしないで戻る
            if ( ! Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
            {
                bool flag1 = ! Initializer.IsReady;
                bool flag2 = EarthManager.EarthTrackingState != TrackingState.Tracking;
                bool flag3 = ARSession.state == ARSessionState.SessionTracking;
                OutputText.text = string.Format(
                " \n\n" +
                " ! Initializer.IsReady: {0}\n" + 
                " EarthManager.EarthTrackingState != TrackingState.Tracking: {1}\n" + 
                " ARSession.state == ARSessionState.SessionTracking: {2}\n",
                flag1.ToString(),
                flag2.ToString(),
                flag3.ToString()
                );
                return;
            }
            // トラッキング結果を取得
            GeospatialPose pose = EarthManager.CameraGeospatialPose;

            // トラッキング精度がthresholdより悪い(値が⼤きい)場合
            if (pose.HeadingAccuracy > HeadingThreshold || pose.HorizontalAccuracy > HorizontalThreshold)
            {
                status = "低精度︓周辺を⾒回してください";
            }
            else // 許容誤差以内の場合
            {
                status = "⾼精度︓High Tracking Accuracy";

                // 初めて⾼精度計測できたタイミングでオブジェクトを⽣成
                if (displayObject == null)
                {
                    // Altitude = pose.Altitude - 1.5f; // スマホの⾼さ-1.5mでおよそ地⾯の⾼さとする (tentatively)
                    Altitude = pose.Altitude;
                   
                    Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);  // ⾓度の補正(このあと使うAnchor⽣成関数が、南=0を前提としているため)
                    ARGeospatialAnchor anchor = AnchorManager.AddAnchor(Latitude, Longitude, Altitude, quaternion); // 指定した位置・向きのアンカーを作成 (緯度・経度・⾼度をx,y,zに、⽅位を姿勢に変換)

                    // アンカーが正しく作られていればオブジェクトを実体化
                    if (anchor != null)
                    {
                        displayObject = Instantiate(ContentPrefab, anchor.transform);
                    }
                }
            }
            //結果を表⽰(statusはのちほど使う)
            ShowTrackingInfo(status, pose, displayObject);
            
        }
        void ShowTrackingInfo(string status, GeospatialPose pose, GameObject obj)
        {
            // 緯度・経度・⾼度やその精度、statusに代⼊された⽂字列を表⽰
            OutputText.text = string.Format(
                " \n\n" +
                " Latitude/Longitude: {0}°, {1}°\n" + // 緯度・経度
                " Horizontal Accuracy: {2}m\n" + // ⽔平⽅向の精度
                " Altitude: {3}m\n" + // ⾼度
                " Vertical Accuracy: {4}m\n" + // ⾼度の精度
                " Heading: {5}°\n" + // ⽅位
                " Heading Accuracy: {6} °\n" + // ⽅位の精度
                " {7} \n" + // statusに代⼊された⽂字列
                " Object Position: {8} \n" + 
                " Camera Position: {9} \n"
                ,
                pose.Latitude.ToString("F6"), // {0}
                pose.Longitude.ToString("F6"), // {1}
                pose.HorizontalAccuracy.ToString("F6"), // {2}
                pose.Altitude.ToString("F2"), // {3}
                pose.VerticalAccuracy.ToString("F2"), // {4}
                pose.Heading.ToString("F1"), // {5}
                pose.HeadingAccuracy.ToString("F1"), // {6}
                status, // {7},
                obj.transform.position.ToString(), // {8}
                Camera.main.transform.position.ToString() // {9}
            );
        }       
    }
