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

        public Image _splashImg;

        [Space]
        public FireworkControl _fireworkCtrl;

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

        // Should be placed in Plugins folder...
        //#region JS functions
        //[DllImport("__Internal")]
        //private static extern void registerDeviceMotion();
        //[DllImport("__Internal")]
        //private static extern void registerDeviceOrientation();
        //#endregion

        // Start is called before the first frame update
        [System.Obsolete]
        void Start()
        {
            //if (!SystemInfo.supportsGyroscope)
            //{
            //    Debug.Log("This device does not support gyroscope");
            //    _txtGyro.SetText($"Gyroscope does not support.");
            //}

            _txtGyro.SetText($"Waiting for checking gyro...");

            Application.ExternalCall("registerDeviceMotion");
            Application.ExternalCall("registerDeviceOrientation");

            //registerDeviceMotion();
            //registerDeviceOrientation();
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

        public void Handle_DeviceOrientation(string infos)
        {
            //Debug.Log($"Handle_DeviceOrientation({infos})");
            string[] datas = infos.Split(","[0]);
            //Debug.Log($"datas: {datas}");
            float z = float.Parse(datas[0]);
            float x = float.Parse(datas[1]);
            float y = float.Parse(datas[2]);
            string info = $"alpha: {z}, beta: {x}, gamma: {y}";
            //Debug.Log(info);
            _txtOrientation.SetText(info);

            // 80 <= gamma <= 120
            bool faceNorth = (z.IsBetweenInclusive(0f, 30f) || z.IsBetweenInclusive(330f, 360f)) &&
                y.IsBetweenInclusive(80f, 120f) && !x.IsBetweenInclusive(-5f, 5f);

            //_splashImg.gameObject.SetActive(faceNorth);

            if (faceNorth)
            {
                OnFaceNorth();
            }
            else
            {

            }

            _txtDirection.SetText(_direction.ToString());
        }

        void OnFaceNorth()
        {
            _direction = Direction.North;
            //_fireworkCtrl.PlayFirework();

            //if (!_firstTime)
            //{
            //    if (Vector3.Distance(_fireworkCtrl.transform.position, transform.position) > positionThreshold)
            //    {
            //        transform.position = Vector3.Lerp(transform.position, _fireworkCtrl.transform.position, Time.deltaTime * updateSpeed);
            //    }
            //}
            //else
            //{
            //    transform.position = _fireworkCtrl.transform.position;
            //    _firstTime = false;
            //}

            //transform.rotation = _fireworkCtrl.transform.rotation;

            //Vector3 absScale = new Vector3(
            //    Mathf.Abs(_fireworkCtrl.transform.scale.x),
            //    Mathf.Abs(_fireworkCtrl.transform.scale.y),
            //    Mathf.Abs(_fireworkCtrl.transform.scale.z)
            //);

            //transform.localScale = absScale;
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
        }

        // Update is called once per frame
        void Update()
        {
            if (SystemInfo.supportsGyroscope)
            {
                Debug.Log("This device supports gyroscope");
                _txtGyro.SetText($"Attitude: {Input.gyro.attitude} gravity: {Input.gyro.gravity}");
            }
        }
    }
}
