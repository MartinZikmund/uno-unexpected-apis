namespace UnexpectedApis.Catalog;

public static partial class SampleCatalog
{
    private static List<Sample> BuildApis() =>
    [
        .. SensorAndDeviceApis(),
        .. DeviceAndSystemApis(),
        .. SystemAndGraphicsApis(),
    ];
}
