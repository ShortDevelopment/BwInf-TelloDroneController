using System.Globalization;
using System.Reflection;

namespace TelloDroneControl
{
    public class TelloStats
    {
        private TelloStats() { }

        public decimal pitch { get; set; }
        public decimal roll { get; set; }
        public decimal yaw { get; set; }
        public decimal vgx { get; set; }
        public decimal vgy { get; set; }
        public decimal vgz { get; set; }
        public decimal templ { get; set; }
        public decimal temph { get; set; }
        public decimal tof { get; set; }
        public decimal h { get; set; }
        public decimal bat { get; set; }
        public decimal baro { get; set; }
        public decimal time { get; set; }
        public decimal agx { get; set; }
        public decimal agy { get; set; }
        public decimal agz { get; set; }

        public static TelloStats Parse(string data)
        {
            TelloStats stats = new TelloStats();
            foreach (string chunk in data.Split(';'))
            {
                string[] chunkData = chunk.Split(':');
                if (chunkData.Length == 2)
                {
                    string name = chunkData[0];
                    string valueStr = chunkData[1];
                    PropertyInfo prop = typeof(TelloStats).GetProperty(name);
                    if (prop != null)
                        prop.SetValue(stats, decimal.Parse(valueStr, CultureInfo.InvariantCulture));
                }
            }
            return stats;
        }
    }
}
