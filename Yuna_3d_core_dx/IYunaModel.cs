using System;

namespace Yuna_3d_core_dx
{
    public interface IYunaModel
    {
        void LoadModel(string path);
        void SetBlendshape(string name, float weight);
        void PlayAnimation(string name);
        void SetMouthOpen(float value);
        event Action OnFrameRendered;
    }
}
