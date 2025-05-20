using Newtonsoft.Json;
using System.Collections.Generic;

namespace Vision_Core
{
    public class PythonKeypointResponse
    {
        [JsonProperty("keypoints")]
        public List<Point3D> Keypoints { get; set; }
    }
}
