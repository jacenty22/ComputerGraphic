using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ComputreGraphic
{
    /// <summary>
    /// Interaction logic for RGBCubeWindow.xaml
    /// </summary>
    public partial class RGBCubeWindow : Window, INotifyPropertyChanged
    {
        //public GeometryModel3D geometry { get; set; } = new GeometryModel3D();
        public event PropertyChangedEventHandler PropertyChanged;
        private bool dragging;
        private Point dragStart;
        private Point dragTotal;
        private double _rotation;
        public double Rotation
        {
            set
            {
                _rotation = value; OnPropertyRaised(nameof(Rotation));
            }
            get { return _rotation; }
        }
        private Vector3D _axis = new Vector3D();
        public Vector3D AxisVector
        {
            set
            {
                _axis = value;
                OnPropertyRaised(nameof(AxisVector));
            }
            get { return _axis; }
        }

        public RGBCubeWindow()
        {
            InitializeComponent();
            DataContext = this;
            ModelVisual3D mv3d = new ModelVisual3D();
            mv3d.Content = CreateCubeModel3DGroup(0, 0, 0, 2, 2, 2);
            RGBViewPort.Children.Add(mv3d);
        }

        private Model3DGroup CreateCubeModel3DGroup(double X, double Y, double Z, double sizeX, double sizeY, double sizeZ)
        {
            Model3DGroup cube = new Model3DGroup();
            Point3D p0 = new Point3D(X - sizeX / 2, Y - sizeY / 2, Z - sizeZ / 2);
            Point3D p1 = new Point3D(X + sizeX / 2, Y - sizeY / 2, Z - sizeZ / 2);
            Point3D p2 = new Point3D(X + sizeX / 2, Y - sizeY / 2, Z + sizeZ / 2);
            Point3D p3 = new Point3D(X - sizeX / 2, Y - sizeY / 2, Z + sizeZ / 2);
            Point3D p4 = new Point3D(X - sizeX / 2, Y + sizeY / 2, Z - sizeZ / 2);
            Point3D p5 = new Point3D(X + sizeX / 2, Y + sizeY / 2, Z - sizeZ / 2);
            Point3D p6 = new Point3D(X + sizeX / 2, Y + sizeY / 2, Z + sizeZ / 2);
            Point3D p7 = new Point3D(X - sizeX / 2, Y + sizeY / 2, Z + sizeZ / 2);

            //front side triangles
            cube.Children.Add(CreateTriangleModel(p3, p2, p6, Color.FromRgb(255, 255, 255)));
            cube.Children.Add(CreateTriangleModel(p3, p6, p7, Color.FromRgb(255, 255, 255)));
            //right side triangles                          
            cube.Children.Add(CreateTriangleModel(p2, p1, p5, Color.FromRgb(255, 255, 255)));
            cube.Children.Add(CreateTriangleModel(p2, p5, p6, Color.FromRgb(255, 255, 255)));
            //back side triangles                          
            cube.Children.Add(CreateTriangleModel(p1, p0, p4, Color.FromRgb(255, 255, 255)));
            cube.Children.Add(CreateTriangleModel(p1, p4, p5, Color.FromRgb(255, 255, 255)));
            //left side triangles                          
            cube.Children.Add(CreateTriangleModel(p0, p7, p4, Color.FromRgb(255, 255, 255)));
            cube.Children.Add(CreateTriangleModel(p0, p3, p7, Color.FromRgb(255, 255, 255)));
            //top side triangles                            
            cube.Children.Add(CreateTriangleModel(p7, p6, p5, Color.FromRgb(255, 255, 255)));
            cube.Children.Add(CreateTriangleModel(p7, p5, p4, Color.FromRgb(255, 255, 255)));
            //bottom side triangles                         ,
            cube.Children.Add(CreateTriangleModel(p2, p3, p0, Color.FromRgb(255, 255, 255)));
            cube.Children.Add(CreateTriangleModel(p2, p0, p1, Color.FromRgb(255, 255, 255)));

            // cube.Children.Add(geometry);
            return cube;
        }
        static private Model3DGroup CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2, Color color)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            Material material = new DiffuseMaterial(new SolidColorBrush(color));
            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }


        private void Viewport3DOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.dragging) return;

            var pos = e.GetPosition(this.RGBViewPort);

            var x = pos.X - this.dragStart.X;
            var y = pos.Y - this.dragStart.Y;

            this.Rotation = Math.Sqrt(x * x + y * y);

            this.AxisVector = new Vector3D(y, 0, -x);
        }

        private void Viewport3DOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.dragging = true;
            this.dragStart = e.GetPosition(this.RGBViewPort);
            this.dragStart.Offset(-this.dragTotal.X, -this.dragTotal.Y);
        }

        private void Viewport3DOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.dragging = false;

            var dragEnd = e.GetPosition(this.RGBViewPort);
            this.dragTotal.X = dragEnd.X - this.dragStart.X;
            this.dragTotal.Y = dragEnd.Y - this.dragStart.Y;
        }
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }


}