using LibreHardwareMonitor.Hardware;
using System.Threading;

class Program
{
    static void Main()
    {
        var computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMotherboardEnabled = true,
            IsMemoryEnabled = true,
            IsStorageEnabled = true,
            IsBatteryEnabled = true
        };

        computer.Open();

        // แสดงสเปกเครื่องแบบสรุป
        Console.WriteLine("================= Notebook Spec Overview =================");
        foreach (var hardware in computer.Hardware)
        {
            hardware.Update();
            switch (hardware.HardwareType)
            {
                case HardwareType.Cpu:
                    Console.WriteLine($"CPU: {hardware.Name}");
                    break;
                case HardwareType.GpuNvidia:
                case HardwareType.GpuAmd:
                case HardwareType.GpuIntel:
                    Console.WriteLine($"GPU: {hardware.Name}");
                    break;
                case HardwareType.Memory:
                    Console.WriteLine($"Memory: {hardware.Name}");
                    break;
                case HardwareType.Motherboard:
                    Console.WriteLine($"Motherboard: {hardware.Name}");
                    break;
                case HardwareType.Storage:
                    Console.WriteLine($"Storage: {hardware.Name}");
                    break;
                case HardwareType.Battery:
                    Console.WriteLine($"Battery: {hardware.Name}");
                    break;
            }
        }
        Console.WriteLine("==========================================================\n");
        Thread.Sleep(3000); // แสดง spec overview 3 วินาทีก่อนเริ่ม loop

        const float tempThreshold = 80.0f; // อุณหภูมิแจ้งเตือน (เช่น 80°C)

        // ====== ฟังก์ชันสำหรับวนซ้ำ hardware/subhardware ======
        void UpdateHardware(IHardware hardware)
        {
            hardware.Update();
            Console.WriteLine($"[{hardware.HardwareType}] {hardware.Name}");
            foreach (var sensor in hardware.Sensors)
            {
                // แสดงข้อมูล Temperature, Fan, Voltage, SSD (Storage)
                if (sensor.SensorType == SensorType.Temperature)
                {
                    Console.WriteLine($"  {sensor.Name}: {sensor.Value} °C");
                    if (sensor.Value.HasValue && sensor.Value.Value > tempThreshold)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  !!! WARNING: {sensor.Name} อุณหภูมิสูงเกิน {tempThreshold} °C !!!");
                        Console.ResetColor();
                    }
                }
                else if (sensor.SensorType == SensorType.Fan)
                {
                    Console.WriteLine($"  {sensor.Name}: {sensor.Value} RPM");
                }
                else if (sensor.SensorType == SensorType.Voltage)
                {
                    Console.WriteLine($"  {sensor.Name}: {sensor.Value} V");
                }
                else if (hardware.HardwareType == HardwareType.Storage)
                {
                    // แสดงข้อมูล Storage/SSD เช่นอุณหภูมิหรือสถานะ
                    Console.WriteLine($"  [SSD] {sensor.Name}: {sensor.Value}");
                }
            }
            foreach (var sub in hardware.SubHardware)
            {
                UpdateHardware(sub); // เรียกซ้ำสำหรับ sub-hardware
            }
        }
        // ...existing code...
        while (true)
        {
            Console.Clear();
            foreach (var hardware in computer.Hardware)
            {
                UpdateHardware(hardware);
            }
            Thread.Sleep(9000); // อัปเดตทุก 9 วินาที
        }
        // computer.Close(); // ไม่จำเป็นเพราะ loop ไม่จบ
    }
}
