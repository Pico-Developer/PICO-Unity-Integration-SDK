namespace Unity.XR.PICO.TOBSupport
{
    public enum SystemInfoEnum
    {
        //电量查看
        ELECTRIC_QUANTITY = 0,

        //PUI版本号查看
        PUI_VERSION = 1,

        //设备型号查看
        EQUIPMENT_MODEL = 2,

        // 设备SN查看
        EQUIPMENT_SN = 3,

        // 客户SN查看
        CUSTOMER_SN = 4,

        // 设备内部存储空间查看
        INTERNAL_STORAGE_SPACE_OF_THE_DEVICE = 5,

        // 设备蓝牙状态查看
        DEVICE_BLUETOOTH_STATUS = 6,

        // 已连接蓝牙名称查看
        BLUETOOTH_NAME_CONNECTED = 7,

        // 蓝牙MAC地址查看
        BLUETOOTH_MAC_ADDRESS = 8,

        // 设备WiFi状态查看
        DEVICE_WIFI_STATUS = 9,

        // 已连接WiFi名称查看
        WIFI_NAME_CONNECTED = 10,

        // WLAN MAC地址查看
        WLAN_MAC_ADDRESS = 11,

        // 设备IP查看
        DEVICE_IP = 12,

        // 设备是否充电
        CHARGING_STATUS = 13,

        //Neo3设备新旧Key
        DEVICE_KEY = 14,

        //设备本身蓝牙信息
        //返回值格式：Name|Address
        //返回值示例：PICO 4|08:16:D5:70:20:11
        BLUETOOTH_INFO_DEVICE = 15,

        //已连接的蓝牙设备信息
        //返回值格式：[蓝牙设备1名称|蓝牙设备1地址, 蓝牙设备2名称|蓝牙设备2地址, . . .]
        //返回值示例：[PICO 4|08:16:D5:70:20:11, PICO Neo 3|21:23:D5:7A:2C:DE]
        BLUETOOTH_INFO_CONNECTED = 16,

        //相机温度单位摄氏度
        CAMERA_TEMPERATURE_CELSIUS = 17,

        //相机温度单位华氏度
        CAMERA_TEMPERATURE_FAHRENHEIT = 18,
        //大空间地图信息：
        LARGESPACE_MAP_INFO=19,
    }
}