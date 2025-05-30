namespace Unity.XR.PICO.TOBSupport
{
    public enum SystemInfoEnum
    {
        /// <summary>
        /// Check the battery level.
        /// </summary>
        ELECTRIC_QUANTITY = 0,

        /// <summary>
        /// Check the PUI version number.
        /// </summary>
        PUI_VERSION = 1,

        /// <summary>
        /// Check the device model.
        /// </summary>
        EQUIPMENT_MODEL = 2,

        /// <summary>
        /// Check the device serial number (SN).
        /// </summary>
        EQUIPMENT_SN = 3,

        /// <summary>
        /// Check the customer serial number (SN).
        /// </summary>
        CUSTOMER_SN = 4,

        /// <summary>
        /// Check the internal storage space of the device.
        /// </summary>
        INTERNAL_STORAGE_SPACE_OF_THE_DEVICE = 5,

        /// <summary>
        /// Check the Bluetooth status of the device.
        /// </summary>
        DEVICE_BLUETOOTH_STATUS = 6,

        /// <summary>
        /// Check the name of the connected Bluetooth device.
        /// </summary>
        BLUETOOTH_NAME_CONNECTED = 7,

        /// <summary>
        /// Check the Bluetooth MAC address.
        /// </summary>
        BLUETOOTH_MAC_ADDRESS = 8,

        /// <summary>
        /// Check the Wi-Fi status of the device.
        /// </summary>
        DEVICE_WIFI_STATUS = 9,

        /// <summary>
        /// Check the name of the connected Wi-Fi network.
        /// </summary>
        WIFI_NAME_CONNECTED = 10,

        /// <summary>
        /// Check the WLAN MAC address.
        /// </summary>
        WLAN_MAC_ADDRESS = 11,

        /// <summary>
        /// Check the device IP address.
        /// </summary>
        DEVICE_IP = 12,

        /// <summary>
        /// Check if the device is charging.
        /// </summary>
        CHARGING_STATUS = 13,

        /// <summary>
        /// Neo3 device old and new keys.
        /// </summary>
        DEVICE_KEY = 14,

        /// <summary>
        /// Bluetooth information of the device itself.
        /// Return value format: Name|Address
        /// Return value example: PICO 4|08:16:D5:70:20:11
        /// </summary>
        BLUETOOTH_INFO_DEVICE = 15,

        /// <summary>
        /// Information of the connected Bluetooth devices.
        /// Return value format: [Bluetooth device 1 name|Bluetooth device 1 address, Bluetooth device 2 name|Bluetooth device 2 address, ...]
        /// Return value example: [PICO 4|08:16:D5:70:20:11, PICO Neo 3|21:23:D5:7A:2C:DE]
        /// </summary>
        BLUETOOTH_INFO_CONNECTED = 16,

        /// <summary>
        /// Camera temperature in Celsius.
        /// </summary>
        CAMERA_TEMPERATURE_CELSIUS = 17,

        /// <summary>
        /// Camera temperature in Fahrenheit.
        /// </summary>
        CAMERA_TEMPERATURE_FAHRENHEIT = 18,

        /// <summary>
        /// Large space map information.
        /// </summary>
        LARGESPACE_MAP_INFO = 19,

        /// <summary>
        /// Current charging protocol.
        /// </summary>
        CURRENT_CHARGING_PROTOCOL = 20,

        /// <summary>
        /// Maximum USB charging power.
        /// </summary>
        USB_PWOER_MAX = 21,

        /// <summary>
        /// Head-mounted display (HMD) position and orientation.
        /// </summary>
        HMD_POSITION_AND_ORIENTATION = 22
    }
}