const unityInstance = UnityLoader.instantiate("unityContainer", "%UNITY_WEBGL_BUILD_URL%");  
let isCameraReady = false;
let isDetectionManagerReady = false;
let gl = null;
const navigator = window.navigator;

function cameraReady(){
    isCameraReady = true;
    gl = unityInstance.Module.ctx;
}

function detectionManagerReady(){
    isDetectionManagerReady = true;
}

function createUnityMatrix(el){
    const m = el.matrix.clone();
    const zFlipped = new THREE.Matrix4().makeScale(1, 1, -1).multiply(m);
    const rotated = zFlipped.multiply(new THREE.Matrix4().makeRotationX(-Math.PI/2));
    return rotated;
}

//---Register functions
function registerDeviceMotion() {
    window.addEventListener('devicemotion', function (event) {
        //console.log(event.acceleration.x + ' m/s2');
        console.log(JSON.stringify(event));
        const msg = `${event.acceleration.x.toString()},${event.acceleration.y.toString()},${event.acceleration.z.toString()}`;
        unityInstance.SendMessage("Gyroscope", "Handle_DeviceMotion", msg);
    });
}

function registerDeviceOrientation() {
    if (window.DeviceOrientationEvent) {
        window.addEventListener("deviceorientation", function (event) {
            // alpha: rotation around z-axis
            var rotateDegrees = event.alpha;
            // gamma: left to right
            var leftToRight = event.gamma;
            // beta: front back motion
            var frontToBack = event.beta;
            handleOrientationEvent(frontToBack, leftToRight, rotateDegrees);
        }, true);
    }

    var handleOrientationEvent = function (frontToBack, leftToRight, rotateDegrees) {
        // do something amazing
        const msg = `${rotateDegrees.toString()},${leftToRight.toString()},${frontToBack.toString()}`;
        unityInstance.SendMessage("Gyroscope", "Handle_DeviceOrientation", msg);
    };

}
//---Register functions

function checkPermission(permissionName, descriptor) {
    console.log("checkPermission(" + permissionName + ")");
    try {
        navigator.permissions.query(Object.assign({ name: permissionName }, descriptor))
            .then(function (permission) {
                permission.addEventListener('change', function (e) {
                    handlePermissionChange(permissionName, permission.state);
                });
            });
    } catch (e) {

    }
}

function handlePermissionChange(permissionName, newState) {
    console.log("handlePermissionChange(" + permissionName + ")");
    var timeBadge = new Date().toTimeString().split(' ')[0];
    var info = '' + timeBadge + ' State of ' + permissionName + ' permission status changed to ' + newState + '.';
    console.log(info);
}

//---Request permissions
function requestGeolocationPermission() {
    console.log("requestGeolocationPermission()");
    navigator.permissions.query({ name: 'geolocation' }).then(function (result) {
        console.log("geolocation permission is " + result.state);
        if (result.state === 'granted') {
            //showLocalNewsWithGeolocation();
            navigator.geolocation.getCurrentPosition(function (position) {
                //yourFunction(position.coords.latitude, position.coords.longitude);
                console.log("latitude: " + position.coords.latitude);
                console.log("longitude: " + position.coords.longitude);
            });
        } else if (result.state === 'prompt') {
            //showButtonToEnableLocalNews();
        }
        // Don't do anything if the permission was denied.
    });
}

function requestGeolocationPermission() {
    console.log("requestGeolocationPermission()");
    navigator.permissions.query({ name: 'geolocation' }).then(function (result) {
        console.log("geolocation permission is " + result.state);
        if (result.state === 'granted') {
            //showLocalNewsWithGeolocation();
            navigator.geolocation.getCurrentPosition(function (position) {
                //yourFunction(position.coords.latitude, position.coords.longitude);
                console.log("latitude: " + position.coords.latitude);
                console.log("longitude: " + position.coords.longitude);
            });
        } else if (result.state === 'prompt') {
            //showButtonToEnableLocalNews();
        }
        // Don't do anything if the permission was denied.
    });
}

function requestAccelerometerPermission() {
    console.log("requestAccelerometerPermission()");
    navigator.permissions.query({ name: 'accelerometer' }).then(function (result) {
        console.log("accelerometer permission is " + result.state);
        if (result.state === 'granted') {
            useAccelerometer();
        } else if (result.state === 'prompt') {
            console.log("Need to show button request for accelerometer permission");
        }
        // Don't do anything if the permission was denied.
    });
}

function useAccelerometer() {
    //Create Accelerometer
    console.log("useAccelerometer()");
    let acl = new Accelerometer({ frequency: 60 });
    acl.addEventListener('reading', () => {
        //console.log("Acceleration along the X-axis " + acl.x);
        //console.log("Acceleration along the Y-axis " + acl.y);
        //console.log("Acceleration along the Z-axis " + acl.z);
        const aclMsg = `${acl.x.toString()},${acl.y.toString()},${acl.z.toString()}`;
        unityInstance.SendMessage("Gyroscope", "Handle_Accelerometer", aclMsg);
    });
    acl.start();
}

function requestGyroscopePermission() {
    console.log("requestGyroscopePermission()");
    // Track device orientation.
    window.addEventListener("deviceorientation", handleOrientation);
    navigator.permissions.query({ name: 'gyroscope' }).then(function (result) {
        console.log("gyroscope permission is " + result.state);
        if (result.state === 'granted') {
            useGyroscope();
        } else if (result.state === 'prompt') {
            console.log("Need to show button request for gyroscope permission");
        }
        // Don't do anything if the permission was denied.
    });
}
//---Request permissions

function useGyroscope() {
    //Create gyroscope
    console.log("useGyroscope()");
    let gyroscope = new Gyroscope();
    gyroscope.addEventListener('reading', handleReading);
    gyroscope.start();
}

function handleReading(event) {
    console.log("handleReading()");
    console.log("event " + JSON.stringify(event));
    console.log("alpha-x " + gyroscope.x);
    console.log("beta-y " + gyroscope.y);
    console.log("gamma-z " + gyroscope.z);
}

// Work!
function handleOrientation(event) {
    console.log("handleOrientation");
    var absolute = event.absolute;
    var alpha = event.alpha;
    var beta = event.beta;
    var gamma = event.gamma;

    // Do stuff with the new orientation data
    console.log("absolute " + absolute);
    console.log("alpha " + alpha);
    console.log("beta " + beta);
    console.log("gamma " + gamma);
}

// Run in each frame.
AFRAME.registerComponent('markercontroller', {
    schema: {
        name : {type: 'string'}
    },
    tock: function(time, timeDelta){

        let position = new THREE.Vector3();
        let rotation = new THREE.Quaternion();
        let scale = new THREE.Vector3();

        createUnityMatrix(this.el.object3D).decompose(position, rotation, scale);

        const serializedInfos = `${this.data.name},${this.el.object3D.visible},${position.toArray()},${rotation.toArray()},${scale.toArray()}`;

        if(isDetectionManagerReady){
          unityInstance.SendMessage("DetectionManager", "markerInfos", serializedInfos);
        }
    } 
});

// Run in each frame.
AFRAME.registerComponent('cameratransform', {
    tock: function(time, timeDelta){

        // transform
        let camtr = new THREE.Vector3();
        // rotation
        let camro = new THREE.Quaternion();
        // scale
        let camsc = new THREE.Vector3();

        this.el.object3D.matrix.clone().decompose(camtr, camro, camsc);

        const projection = this.el.components.camera.camera.projectionMatrix.clone();
        const serializedProj = `${[...projection.elements]}`

        const posCam = `${[...camtr.toArray()]}`
        const rotCam = `${[...camro.toArray()]}`
 
        if (isCameraReady) {
            // Work!
            //console.log("Camera is ready.");
            unityInstance.SendMessage("Main Camera", "Handle_CameraReady", isCameraReady.toString());
            unityInstance.SendMessage("Main Camera", "setProjection", serializedProj);
            unityInstance.SendMessage("Main Camera", "setPosition", posCam);
            unityInstance.SendMessage("Main Camera", "setRotation", rotCam);

            let w = window.innerWidth;
            let h = window.innerHeight;

            const unityCanvas = document.getElementsByTagName('canvas')[0];

            const ratio = unityCanvas.height / h;

            w *= ratio
            h *= ratio

            const size = `${w},${h}`

            unityInstance.SendMessage("Canvas", "setSize", size);
        }

        if(gl != null){
            gl.dontClearOnFrameStart = true;
        }
    } 
});

// Run in each frame.
AFRAME.registerComponent('copycanvas', {
    tick: function(time, timeDelta){
        const unityCanvas = document.getElementsByTagName('canvas')[0];
        unityCanvas.width = this.el.canvas.width
        unityCanvas.height = this.el.canvas.height
    } 
});