using OpenCvSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision_Core
{
    public interface IGestureRecognizer
    {
        Task<List<Point3D>> RecognizeAsync(Mat frame);
    }
}