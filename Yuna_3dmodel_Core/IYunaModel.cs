namespace Yuna_3dmodel_Core
{
    public interface IYunaModel
    {
        void Load(string modelPath);
        void PlayAnimation(string name);
        void SetEmotion(string blendshape, float value);
        void SetLipSync(float openness);
    }
}
