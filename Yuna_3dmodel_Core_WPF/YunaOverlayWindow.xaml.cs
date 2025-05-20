using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Model;
using SharpDX;

namespace Yuna_3dmodel_Core_WPF
{
    public partial class YunaOverlayWindow : Window
    {
        public YunaOverlayWindow()
        {
            InitializeComponent();
            Loaded += YunaOverlayWindow_Loaded;
        }

        private void YunaOverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetupScene();
            AddDebugCube();
        }

        private void SetupScene()
        {
            viewport.Camera = new PerspectiveCamera
            {
                Position = new System.Windows.Media.Media3D.Point3D(0, 2, 5),
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -2, -5),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
                NearPlaneDistance = 0.1f,
                FarPlaneDistance = 5000f
            };

            viewport.Items.Add(new DirectionalLight3D
            {
                Color = System.Windows.Media.Colors.White,
                Direction = new System.Windows.Media.Media3D.Vector3D(-1, -1, -1)
            });
        }

        private void AddDebugCube()
        {
            var builder = new MeshBuilder();
            builder.AddBox(center: new Vector3(0, 0, 0), xlength: 1f, ylength: 1f, zlength: 1f);

            var geometry = builder.ToMeshGeometry3D();

            var cube = new MeshGeometryModel3D
            {
                Geometry = geometry,
                Material = new PhongMaterial
                {
                    DiffuseColor = Color.Cyan
                }
            };

            viewport.Items.Add(cube);
        }
    }
}
