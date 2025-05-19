namespace Vision_Core
{
    public class HandLandmark
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class HandLandmarkFrame
    {
        public List<HandLandmark> Points { get; set; } = new();
    }
}
