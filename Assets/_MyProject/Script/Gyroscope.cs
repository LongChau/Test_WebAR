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
        public Canvas _canvasCompass;
        public Image _imgCompass;
        public TextMeshProUGUI _txtGyro;
        public TextMeshProUGUI _txtMotion;
        public TextMeshProUGUI _txtOrientation;
        public TextMeshProUGUI _txtInfo;
        public TextMeshProUGUI _txtDirection;
        public TextMeshProUGUI _txtNorthpoleValue;

        public Image _splashImg;

        public Transform _light;

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

        // Start is called before the first frame update
        [System.Obsolete]
        void Start()
        {
            Input.location.Start();
            Input.gyro.enabled = true;
            Input.compass.enabled = true;

            _txtGyro.SetText($"Waiting for checking gyro...");

#if UNITY_WEBGL
            Application.ExternalCall("registerDeviceMotion");
            Application.ExternalCall("registerDeviceOrientation");
            //Application.ExternalCall("registerOrientationEvents");
            _camCtrl.transform.SetParent(_camContainer);
            _camContainer.rotation = Quaternion.Euler(90f, 90f, 0f);
#endif

#if UNITY_IOS
            _camCtrl.transform.SetParent(_camContainer);
            _camContainer.rotation = Quaternion.Euler(90f, 90f, 0f);
            _light.gameObject.SetActive(true);
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

        bool _hasDeviceOrientation = false;
        Quaternion _orientationValues;
        public void Handle_DeviceOrientation(string infos)
        {
            _hasDeviceOrientation = true;
            //Debug.Log($"Handle_DeviceOrientation({infos})");
            string[] datas = infos.Split(","[0]);
            //Debug.Log($"datas: {datas}");
            float z = float.Parse(datas[0]);
            float y = float.Parse(datas[1]);
            float x = float.Parse(datas[2]);
            float w = float.Parse(datas[4]);
            string info = $" beta-x: {x}, gamma-y: {y}, alpha-z: {z}, w: {w}";
            //_txtGyro.SetText(info);

            _orientationValues = new Quaternion(x, y, z, w);
            //_orientationValues = Quaternion.Euler(x, y, z);
            // 80 <= gamma <= 120
            bool faceNorth = (z.IsBetweenInclusive(0f, 30f) || z.IsBetweenInclusive(330f, 360f)) &&
                y.IsBetweenInclusive(80f, 120f) && !x.IsBetweenInclusive(-5f, 5f);

            if (faceNorth)
            {
                OnFaceNorth();
                _txtDirection.SetText(_direction.ToString());
            }
            else
            {
                _txtDirection.SetText("Not facing north.");
            }
        }

        bool _isFirstTime = true;
        void OnFaceNorth()
        {
            _direction = Direction.North;
        }

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
            //_txtGyro.SetText(info);
            _accelerometorValues = new Vector3(posX, posY, posZ);
        }

        bool isPlayedFirework = false;
        bool isFacingNorth = false;
        // Update is called once per frame
        void Update()
        {
#if UNITY_IOS
            _camCtrl.transform.localRotation = Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
            _txtNorthpoleValue.SetText($"North pole value: {Input.compass.magneticHeading}");
            _txtGyro.SetText(Input.gyro.attitude.ToString());
            bool isFacingNorth = (Input.compass.magneticHeading.IsBetweenInclusive(0, 5) ||
                Input.compass.magneticHeading.IsBetweenInclusive(355, 360));
            if (isFacingNorth && _camCtrl.IsUpperCam(70) && !isPlayedFirework)
            {
                // Show firework there.
                _fireworkCtrl.transform.position = _camCtrl.transform.forward * 20f;
                _fireworkCtrl.PlayFirework();
                isPlayedFirework = true;
            }
            // For camera outside the camera container.
            //var magneticDegree = Input.compass.magneticHeading;
            //Quaternion newRotation = Quaternion.Euler(_canvasCompass.transform.eulerAngles.x, _camCtrl.transform.localEulerAngles.y, magneticDegree);
            //_imgCompass.transform.rotation = Quaternion.Lerp(_imgCompass.transform.rotation, newRotation, 5f * Time.deltaTime);

            // For camera in world parent, not in the camera container.
            var magneticDegree = Input.compass.magneticHeading;
            Quaternion newRotation = Quaternion.Euler(0, 0, magneticDegree);
            _imgCompass.transform.localRotation = Quaternion.Lerp(_imgCompass.transform.localRotation, newRotation, 5f * Time.deltaTime);
#else
            if (_hasDeviceOrientation)
            {
                //_camCtrl.transform.localRotation = _orientationValues;
                _camCtrl.transform.localRotation = _orientationValues * new Quaternion(0f, 0f, 1f, 0f);
                _txtGyro.SetText(_orientationValues.ToString());
            }
#endif
        }

        [ContextMenu("ToggleFacingNorth")]
        private void FacingNorth() => isFacingNorth = !isFacingNorth;

    }
}
