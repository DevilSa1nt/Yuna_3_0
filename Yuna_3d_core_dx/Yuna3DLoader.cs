using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using System.IO;

namespace Yuna_3d_core_dx
{
    public class Yuna3DLoader
    {
        public SceneNode RootNode { get; private set; }

        public void LoadGlb(string path)
        {
            var importer = new Importer();
            var scene = importer.Load(path);

            if (scene == null || scene.Root == null)
                throw new FileNotFoundException("GLB модель не загружена");

            RootNode = scene.Root;
        }
    }
}
