using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Render;
using SharpDX.Direct3D11;
using System;

namespace Yuna_3d_core_dx
{
    public class Yuna3DRenderer
    {
        private DeviceManager deviceManager;
        private RenderCore renderCore;
        private SceneRenderer sceneRenderer;
        private SceneNodeGroupModel3D sceneRoot;

        public Yuna3DRenderer()
        {
            deviceManager = new DeviceManager();
            renderCore = new BasicRenderCore();
        }

        public void Initialize()
        {
            deviceManager.CreateDevice(); // SharpDX device
            renderCore.AttachDevice(deviceManager);

            sceneRoot = new SceneNodeGroupModel3D();
            sceneRenderer = new SceneRenderer(sceneRoot.SceneNode, renderCore);
        }

        public void AttachScene(SceneNode node)
        {
            sceneRoot.AddNode(node);
        }

        public void Render()
        {
            renderCore.Render();
        }
    }
}
