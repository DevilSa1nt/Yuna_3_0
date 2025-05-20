using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System.IO;

namespace Yuna_3dmodel_Core
{
    public class YunaModel : IYunaModel
    {
        public SceneNodeGroupModel3D ModelVisual { get; private set; }

        public void Load(string modelPath)
        {
            if (!File.Exists(modelPath))
                throw new FileNotFoundException("GLB-файл не найден", modelPath);

            //var importer = new SceneImporter(); // работает с .glb/.gltf
            //var scene = importer.Load(modelPath);

            ModelVisual = new SceneNodeGroupModel3D();
            //ModelVisual.AddNode(scene.Root);
        }

        public void PlayAnimation(string name) { }
        public void SetEmotion(string blendshape, float value) { }
        public void SetLipSync(float openness) { }
    }
}
