using OpenCvSharp;
using System.Threading.Tasks;

namespace Vision_Core
{
    public interface IGestureRecognizer
    {
        Task<string> RecognizeAsync(Mat frame);
    }
}
