using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Core;

namespace Yuna_3dmodel_Core_WPF
{
    public class Yuna3DModel
    {
        public Element3D LoadObj(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"❌ OBJ-файл не найден:\n{path}");
                return null;
            }

            try
            {
                var reader = new ObjReader();
                IList<Object3D> geometryList = reader.Read(path);

                if (geometryList == null || geometryList.Count == 0)
                {
                    MessageBox.Show("❌ Файл OBJ загружен, но геометрия отсутствует.");
                    return null;
                }

                var obj = geometryList[0];
                if (obj.Geometry == null)
                {
                    MessageBox.Show("❌ Geometry в объекте отсутствует.");
                    return null;
                }

                var model = new MeshGeometryModel3D
                {
                    Geometry = obj.Geometry,
                    Material = new PhongMaterial
                    {
                        DiffuseColor = SharpDX.Color.Green,
                        SpecularColor = SharpDX.Color.White,
                        ReflectiveColor = SharpDX.Color.Gray
                    },
                    Transform = new System.Windows.Media.Media3D.Transform3DGroup
                    {
                        Children = new System.Windows.Media.Media3D.Transform3DCollection
                        {
                            new System.Windows.Media.Media3D.TranslateTransform3D(0, -1, 0),
                            new System.Windows.Media.Media3D.ScaleTransform3D(0.01, 0.01, 0.01)
                        }
                    }
                };

                return model;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка при загрузке модели:\n{ex.Message}");
                return null;
            }
        }
    }
}
