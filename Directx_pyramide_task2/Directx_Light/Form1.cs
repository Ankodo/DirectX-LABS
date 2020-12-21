using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using Microsoft.VisualC;

namespace Directx_Light
{
    public partial class Form1 : Form
    {
        private Device device = null;
        private VertexBuffer vb = null;
        private float angle = 0f;
        private double time = 0;
        private CustomVertex.PositionNormalColored[] vertices;
        private IndexBuffer ib = null;
        private int[] indices;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeDevice();
            VertexDeclaration();
            CameraPositioning();
        }

        public void InitializeDevice()
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            //включаем режим освещения и обработку невидимых граней
            device.RenderState.Lighting = true;

            device.RenderState.CullMode = Cull.CounterClockwise;
        }

        public void CameraPositioning()
        {
            
            device.Transform.Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4, (float)this.Width / (float)this.Height, 0.1f, 100f);
            device.Transform.View = Matrix.LookAtRH(new Vector3(10f, 10f, 10f),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 30));

            //включаем направленные источники света
        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 5, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);

            //вершины содержат координаты, нормаль и цвет
            float a = 0.0f; //координата Y первой точки
            float b = 3.0f; //Длина стороны
            float c = 0.0f; //смещение параллельной стороны относительно начальной стороны по Oy
            float d = 5.0f; //смещение параллельной стороны относительно начальной стороны по Ox
            vertices = new CustomVertex.PositionNormalColored[5];
            vertices[0] = new CustomVertex.PositionNormalColored(0f, a, 0f, 0f, 0f, 1f, Color.Cyan.ToArgb());
            vertices[1] = new CustomVertex.PositionNormalColored(0f, a + b, 0f, 0f, 0f, 1f, Color.Red.ToArgb());
            vertices[2] = new CustomVertex.PositionNormalColored(d, c + a, 0f, 0f, 0f, 1f, Color.Blue.ToArgb());
            vertices[3] = new CustomVertex.PositionNormalColored(d, c + a + b, 0f, 0f, 0f, 1f, Color.Magenta.ToArgb());
            vertices[4] = new CustomVertex.PositionNormalColored(0f, 0f, 5f, 0f, 0f, 1f, Color.Green.ToArgb());

            vb.SetData(vertices, 0, LockFlags.None);

            //индексный буфер показывает, как вершины объединить в треугольники
            ib = new IndexBuffer(typeof(int), 18, device, Usage.WriteOnly, Pool.Default);
            indices = new int[18];

            //дно - по часовой стрелке
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 1;
            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 3;

            indices[6] = 0;
            indices[7] = 1;
            indices[8] = 4;

            indices[9] = 1;
            indices[10] = 3;
            indices[11] = 4;

            indices[12] = 3;
            indices[13] = 2;
            indices[14] = 4;

            indices[15] = 2;
            indices[16] = 0;
            indices[17] = 4;



            ib.SetData(indices, 0, LockFlags.None);

            ib = new IndexBuffer(typeof(int), indices.Length, device,
                     Usage.WriteOnly, Pool.Default);

            ib.SetData(indices, 0, LockFlags.None);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            
            device.Clear(ClearFlags.Target, Color.DarkSlateBlue, 1.0f, 0);

            device.BeginScene();
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;

            //установка вершин и индексов, показывающих как из них построить поверхность
            device.SetStreamSource(0, vb, 0);
            device.Indices = ib;


            device.Transform.World = Matrix.RotationX(1*angle) * Matrix.RotationY(2 * angle) * Matrix.RotationZ(3 * angle);

            //отрисовка индексированных фигур
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 5, 0, 6);

            Vector3 ncStart = new Vector3(20.0f, 0.0f, 0.0f);
            Vector3 NormalToPlane = new Vector3(-2.0f, 1.0f, -1.0f);
            double lambda = NormalToPlane.X * NormalToPlane.X + NormalToPlane.Y * NormalToPlane.Y + NormalToPlane.Z * NormalToPlane.Z;
            lambda = 1 / Math.Sqrt(lambda);
            NormalToPlane.X *= (float)lambda;
            NormalToPlane.Y *= (float)lambda;
            NormalToPlane.Z *= (float)lambda;
            Vector3 ncZ = NormalToPlane;
            Vector3 vec2 = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 ncX = Vector3.Cross(ncZ, vec2);
            lambda = ncX.X * ncX.X + ncX.Y * ncX.Y + ncX.Z * ncX.Z;
            lambda = 1 / Math.Sqrt(lambda);
            ncX.X *= (float)lambda;
            ncX.Y *= (float)lambda;
            ncX.Z *= (float)lambda;

            Vector3 ncY = Vector3.Cross(ncZ, ncX);
            lambda = ncY.X * ncY.X + ncY.Y * ncY.Y + ncY.Z * ncY.Z;
            lambda = 1 / Math.Sqrt(lambda);
            ncY.X *= (float)lambda;
            ncY.Y *= (float)lambda;
            ncY.Z *= (float)lambda;

           // float[,] trans = new float[,] { { ncX.X, ncY.X, ncZ.X }, { ncX.Y, ncY.Y, ncZ.Y }, { ncX.Z, ncY.Z, ncZ.Z } };

            ncStart.X = NormalToPlane.X * 20.0f / (float)Math.Sqrt(6.0);
            ncStart.Y = NormalToPlane.Y * 20.0f / (float)Math.Sqrt(6.0);
            ncStart.Z = NormalToPlane.Z * 20.0f / (float)Math.Sqrt(6.0);
            Vector3 lightTarget = new Vector3(0.0f, 0.0f, 0.0f);

                double radA = 20.0;
                double radB = 10.0;
                double x = Math.Cos(time) * radA;
                double y = Math.Sin(time) * radB;
                Vector3 lightPos = new Vector3(ncX.X * (float)x + ncY.X * (float)y + ncStart.X, ncX.Y * (float)x + ncY.Y * (float)y + ncStart.Y, ncX.Z * (float)x + ncY.Z * (float)y + ncStart.Z);
                Vector3 lightDir = new Vector3(lightTarget.X - lightPos.X, lightTarget.Y - lightPos.Y, lightTarget.Z - lightPos.Z);
                lambda = lightDir.X * lightDir.X + lightDir.Y * lightDir.Y + lightDir.Z * lightDir.Z;
                lambda = 1 / Math.Sqrt(lambda);
                lightDir.X *= (float)lambda;
                lightDir.Y *= (float)lambda;
                lightDir.Z *= (float)lambda;

                Console.WriteLine(lightPos.X-lightPos.Y+lightPos.Z-20);
              
                device.Lights[0].Type = LightType.Directional;
                device.Lights[0].Diffuse = Color.White;
                device.Lights[0].Direction = new Vector3(lightDir.X, lightDir.Y, lightDir.Z);
                device.Lights[0].Enabled = true;

            device.EndScene();

            device.Present();

            this.Invalidate();
            time += 0.1;
            angle += 0.005f;
        }
    }
}