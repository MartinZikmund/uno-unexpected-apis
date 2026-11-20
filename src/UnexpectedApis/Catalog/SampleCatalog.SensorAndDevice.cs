using UnexpectedApis.Apis;
using UnexpectedApis.Core;

namespace UnexpectedApis.Catalog;

public static partial class SampleCatalog
{
    private const string SensorsDocs = "https://platform.uno/docs/articles/features/windows-devices-sensors.html";

    private const ApiPlatforms SensorPlatforms = ApiPlatforms.All & ~ApiPlatforms.Desktop & ~ApiPlatforms.TvOS;

    private static List<Sample> SensorAndDeviceApis() =>
    [
        new ApiSample("accelerometer", "Accelerometer", Sensors, Symbol.Rotate, "Reads how the device is tilted and moved along three axes.",
                SensorPlatforms, "https://platform.uno/docs/articles/features/accelerometer.html", () => new AccelerometerDemo())
            .Code("""
                var accelerometer = Accelerometer.GetDefault();
                if (accelerometer is not null)
                {
                    accelerometer.ReportInterval = Math.Max(accelerometer.MinimumReportInterval, 100);
                    accelerometer.ReadingChanged += (s, e) =>
                    {
                        var x = e.Reading.AccelerationX;
                        var y = e.Reading.AccelerationY;
                        var z = e.Reading.AccelerationZ;
                    };
                }
                """)
            .Tip("GetDefault returns null where there is no sensor, so check it. Unsubscribe from ReadingChanged when you're done to let the sensor power down.")
            .Find("tilt", "motion", "shake", "gravity"),

        new ApiSample("ambientlight", "Ambient light", Sensors, Symbol.View, "Measures how bright the room is, in lux.",
                ApiPlatforms.Android | ApiPlatforms.Windows | ApiPlatforms.WebAssembly, "https://platform.uno/docs/articles/features/lightsensor.html", () => new AmbientLightDemo())
            .Code("""
                var lightSensor = LightSensor.GetDefault();
                if (lightSensor is not null)
                {
                    lightSensor.ReadingChanged += (s, e) =>
                    {
                        var lux = e.Reading.IlluminanceInLux;
                    };
                }
                """)
            .Tip("iOS doesn't expose an ambient light sensor to apps, so GetDefault returns null there.")
            .Find("lux", "light sensor", "brightness", "illuminance"),

        new ApiSample("compass", "Compass", Sensors, Symbol.Map, "Points to magnetic north using the device's magnetometer.",
                SensorPlatforms, "https://platform.uno/docs/articles/features/compass.html", () => new CompassDemo())
            .Code("""
                var compass = Compass.GetDefault();
                if (compass is not null)
                {
                    compass.ReadingChanged += (s, e) =>
                    {
                        var heading = e.Reading.HeadingMagneticNorth;
                    };
                }
                """)
            .Tip("In the browser the magnetometer must be allowed in the site settings, and it needs a secure (HTTPS) page.")
            .Find("heading", "magnetometer", "north", "direction"),

        new ApiSample("geolocator", "Geolocator", Sensors, Symbol.Target, "Finds where the device is on the planet.",
                ApiPlatforms.All & ~ApiPlatforms.Desktop, "https://platform.uno/docs/articles/features/windows-devices-geolocation.html", () => new GeolocatorDemo())
            .Code("""
                if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
                {
                    Geolocator geolocator = new();
                    var geoposition = await geolocator.GetGeopositionAsync();
                    var latitude = geoposition.Coordinate.Point.Position.Latitude;
                    var longitude = geoposition.Coordinate.Point.Position.Longitude;
                }
                """)
            .Tip("Android needs ACCESS_FINE_LOCATION and location switched on; iOS needs NSLocationWhenInUseUsageDescription in Info.plist. Simulators report a fixed or fake position.")
            .Find("gps", "location", "latitude", "longitude", "position"),

        new ApiSample("hingeangle", "Hinge angle", Sensors, Symbol.Orientation, "Reads the angle of a foldable device's hinge.",
                ApiPlatforms.Android, SensorsDocs, () => new HingeAngleDemo())
            .Code("""
                var hinge = await HingeAngleSensor.GetDefaultAsync();
                if (hinge is not null)
                {
                    hinge.ReadingChanged += (s, e) =>
                    {
                        var angle = e.Reading.AngleInDegrees;
                    };
                }
                """)
            .Tip("Only foldables have a hinge sensor. On everything else GetDefaultAsync returns null.")
            .Find("foldable", "fold", "surface duo", "angle"),

        new ApiSample("flashlight", "Flashlight", Device, Symbol.Highlight, "Turns the camera flash on and off as a torch.",
                SensorPlatforms, "https://platform.uno/docs/articles/features/flashlight.html", () => new FlashlightDemo())
            .Code("""
                if (await Lamp.GetDefaultAsync() is Lamp lamp)
                {
                    lamp.BrightnessLevel = 1;
                    lamp.IsEnabled = true;

                    // When you're done:
                    lamp.IsEnabled = false;
                    lamp.Dispose();
                }
                """)
            .Tip("Dispose the Lamp when you're done, it holds native resources. Android needs the FLASHLIGHT and CAMERA permissions and can't dim the light.")
            .Find("torch", "lamp", "flash", "light"),

        new ApiSample("vibration", "Vibration", Device, Symbol.Phone, "Buzzes the device for a moment.",
                ApiPlatforms.Android | ApiPlatforms.iOS | ApiPlatforms.WebAssembly, "https://platform.uno/docs/articles/features/windows-phone-devices-notification-vibrationdevice.html", () => new VibrationDemo())
            .Code("""
                var vibrationDevice = VibrationDevice.GetDefault();
                if (vibrationDevice is not null)
                {
                    vibrationDevice.Vibrate(TimeSpan.FromMilliseconds(500));
                }
                """)
            .Tip("Android needs the VIBRATE permission. iOS ignores the duration and plays its default buzz. VibrationDevice isn't part of WinUI, use Windows.Devices.Haptics there.")
            .Find("haptic", "buzz", "rumble"),

        new ApiSample("power", "Battery", Device, Symbol.Important, "Shows the battery level, charging state and energy saver mode.",
                ApiPlatforms.All & ~ApiPlatforms.Desktop, "https://platform.uno/docs/articles/features/windows-system-power.html", () => new PowerDemo())
            .Code("""
                #if HAS_UNO
                // WebAssembly only: call before using PowerManager.
                var isAvailable = await PowerManager.InitializeAsync();
                #endif

                var percent = PowerManager.RemainingChargePercent;
                var charging = PowerManager.BatteryStatus == BatteryStatus.Charging;

                PowerManager.BatteryStatusChanged += (s, e) =>
                {
                    charging = PowerManager.BatteryStatus == BatteryStatus.Charging;
                };
                """)
            .Tip("Android needs the BATTERY_STATS permission and only raises the level event on Low and Ok, so poll RemainingChargePercent for live values. In the browser, call InitializeAsync first.")
            .Find("battery", "charging", "energy saver", "power"),

        new ApiSample("displayrequest", "Keep screen on", Device, Symbol.SlideShow, "Stops the screen from turning off while the device is idle.",
                ApiPlatforms.All & ~ApiPlatforms.Desktop, "https://platform.uno/docs/articles/features/windows-system-display.html", () => new DisplayRequestDemo())
            .Code("""
                DisplayRequest displayRequest = new();
                displayRequest.RequestActive();

                // Later, when the screen may sleep again:
                displayRequest.RequestRelease();
                """)
            .Tip("Always pair RequestActive with RequestRelease. In the browser it uses the Screen Wake Lock API, which some browsers lack, and rapid on/off toggling can race.")
            .Find("wake lock", "sleep", "screen", "idle", "awake"),
    ];
}
