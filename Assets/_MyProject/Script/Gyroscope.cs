using ARWT.Core;
using ARWT.Marker;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebAR.Effect;

namespace WebAR.Gyroscope
{
    public class Gyroscope : MonoBehaviour
    {
        public TextMeshProUGUI _txtGyro;
        public TextMeshProUGUI _txtMotion;
        public TextMeshProUGUI _txtOrientation;
        public TextMeshProUGUI _txtInfo;
        public TextMeshProUGUI _txtDirection;
        public TextMeshProUGUI _txtNorthpoleValue;

        public Image _splashImg;

        public Vector3 fakePosition = new Vector3(0f, 0f, -5f);

        [Space]
        public FireworkControl _fireworkCtrl;

        public GenericController genericController;

        public CameraController _camCtrl;
        public Transform _camContainer;

        private enum Direction
        {
            North,
            South,
            East,
            West
        }

        private Direction _direction;

        bool _firstTime = true;
        public float updateSpeed = 10;
        public float positionThreshold = 0;

        bool isGetOrientation;

        // Should be placed in Plugins folder...
        #region JS functions
        //[DllImport("__Internal")]
        //private static extern void registerDeviceMotion();
        //[DllImport("__Internal")]
        //private static extern void registerDeviceOrientation();
        #endregion

        //public void AddX() => fakePosition.Add(new Vector3(1f, 0f, 0f));
        //public void AddY() => fakePosition.Add(new Vector3(0f, 1f, 0f));
        //public void AddZ() => fakePosition.Add(new Vector3(0f, 0f, 1f));
        //public void SubtractX() => fakePosition.Add(new Vector3(0f, 0f, -1f));
        //public void SubtractY() => fakePosition.Add(new Vector3(0f, -1f, 0f));
        //public void SubtractZ() => fakePosition.Add(new Vector3(-1f, 0f, 0f));

        // Start is called before the first frame update
        [System.Obsolete]
        void Start()
        {
            //if (!SystemInfo.supportsGyroscope)
            //{
            //    Debug.Log("This device does not support gyroscope");
            //    _txtGyro.SetText($"Gyroscope does not support.");
            //}

            Input.location.Start();
            Input.gyro.enabled = true;
            Input.compass.enabled = true;

            _txtGyro.SetText($"Waiting for checking gyro...");

#if UNITY_WEBGL
            Application.ExternalCall("registerDeviceMotion");
            Application.ExternalCall("registerDeviceOrientation");
            //Application.ExternalCall("requestGyroscopePermission");
            //Application.ExternalCall("createSimpleCube");
#endif

            //registerDeviceMotion();
            //registerDeviceOrientation();
#if UNITY_IOS
            _camCtrl.transform.SetParent(_camContainer);
            _camContainer.rotation = Quaternion.Euler(90f, 90f, 0f);
            isGetOrientation = true;
#endif
        }

        public void Handle_DeviceMotion(string infos)
        {
            //Debug.Log($"Handle_DeviceMotion({infos})");
            string[] datas = infos.Split(","[0]);
            //Debug.Log($"datas: {datas}");
            float x = float.Parse(datas[0]);
            float y = float.Parse(datas[1]);
            float z = float.Parse(datas[2]);
            string info = $"x: {x}, y: {y}, z: {z}";
            //Debug.Log(info);
            _txtMotion.SetText(info);
        }

        Quaternion _orientationValues;
        public void Handle_DeviceOrientation(string infos)
        {
            isGetOrientation = true;

            //Debug.Log($"Handle_DeviceOrientation({infos})");
            string[] datas = infos.Split(","[0]);
            //Debug.Log($"datas: {datas}");
            float z = float.Parse(datas[0]);
            float y = float.Parse(datas[1]);
            float x = float.Parse(datas[2]);
            //float absolute = float.Parse(datas[3]);
            //float webkitCompassHeading = float.Parse(datas[4]);
            //float webkitCompassAccuracy = float.Parse(datas[5]);
            //float quaternionX = float.Parse(datas[6]);
            //float quaternionY = float.Parse(datas[7]);
            //float quaternionZ = float.Parse(datas[8]);
            //float quaternionW = float.Parse(datas[9]);
            //Quaternion quaternion = new Quaternion(quaternionX, quaternionY, quaternionZ, quaternionW);
            string info = $" beta: {x}, gamma: {y}, alpha: {z}";
                //$"absolute: {absolute}" +
                //$"webkitCompassHeading: {webkitCompassHeading}" +
                //$"webkitCompassAccuracy: {webkitCompassAccuracy}";
                //$"quaternionX: {quaternionX}" +
                //$"quaternionY: {quaternionY}" +
                //$"quaternionZ: {quaternionZ}" +
                //$"quaternionW: {quaternionW}";
            //Debug.Log(info);
            _txtOrientation.SetText(info);

            //_orientationValues = new Vector3(x, y, z);
            //_orientationValues = quaternion;
            _orientationValues = Quaternion.Euler(x, y, z);
            // 80 <= gamma <= 120
            bool faceNorth = (z.IsBetweenInclusive(0f, 30f) || z.IsBetweenInclusive(330f, 360f)) &&
                y.IsBetweenInclusive(80f, 120f) && !x.IsBetweenInclusive(-5f, 5f);

            //_splashImg.gameObject.SetActive(faceNorth);

            if (faceNorth)
            {
                OnFaceNorth();
                _txtDirection.SetText(_direction.ToString());
            }
            else
            {
                _txtDirection.SetText("Not facing north.");
                //MarkerInfo fakeMarker = new MarkerInfo(
                //    "Nutty", true,
                //    _fireworkCtrl.transform.position,
                //    _fireworkCtrl.transform.rotation,
                //    _fireworkCtrl.transform.localScale);
                //genericController.FakeMarkerLost(fakeMarker);
            }
        }

        bool _isFirstTime = true;
        void OnFaceNorth()
        {
            _direction = Direction.North;

            //MarkerInfo fakeMarker = new MarkerInfo(
            //    "Nutty", true,
            //    _fireworkCtrl.transform.position,
            //    _fireworkCtrl.transform.rotation,
            //    _fireworkCtrl.transform.localScale);
            //genericController.FakeMarkerVisible(fakeMarker);
            //if (_isFirstTime)
            //{
            //    //var newPos = _camCtrl.transform.position;
            //    //newPos.z = 20f;
            //    //_fireworkCtrl.transform.position = newPos;
            //    _fireworkCtrl.transform.position = new Vector3(
            //        _fireworkCtrl.transform.position.x,
            //        Input.acceleration.y + 5f, 
            //        _fireworkCtrl.transform.position.z);
            //    _fireworkCtrl.PlayFirework();
            //    _isFirstTime = false;
            //}
        }

        //void OnFaceAnotherDirection()
        //{
        //    if (m.name == markerToListen)
        //    {
        //        child?.SetActive(false);
        //        uiHelper?.SetActive(true);
        //        gameUI?.SetActive(false);
        //        _firstTime = true;
        //    }
        //}

        Vector3 _accelerometorValues;
        public void Handle_Accelerometer(string infos)
        {
            Debug.Log($"Handle_Accelerometer({infos})");
            string[] datas = infos.Split(","[0]);
            //Debug.Log($"datas: {datas}");
            float posX = float.Parse(datas[0]);
            float posY = float.Parse(datas[1]);
            float posZ = float.Parse(datas[2]);
            string info = $"posX: {posX}, posY: {posY}, posZ: {posZ}";
            //Debug.Log(info);
            _txtGyro.SetText(info);
            _accelerometorValues = new Vector3(posX, posY, posZ);
        }

        // Update is called once per frame
        void Update()
        {
            //_camCtrl.transform.Translate(Input.acceleration.x, Input.acceleration.y, -Input.acceleration.z);
            //_camCtrl.transform.Translate(_accelerometorValues.x, _accelerometorValues.y, -_accelerometorValues.z);
            //_camCtrl.transform.eulerAngles = _orientationValues;
            //Input.gyro.enabled = true;

#if UNITY_IOS
            _camCtrl.transform.localRotation = Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
            _txtNorthpoleValue.SetText($"North pole value: {Input.compass.magneticHeading}");
            _txtGyro.SetText(Input.gyro.attitude.ToString());
#else
            if (isGetOrientation)
            {
                _camCtrl.transform.localRotation = _orientationValues;
                //_camCtrl.transform.localRotation = _orientationValues * new Quaternion(0f, 0f, 1f, 0f);
            }
#endif
        }
    }
}
