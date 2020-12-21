using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace Directx_Textures
{
    public partial class Form1 : Form
    {
        private Device device = null;
        private VertexBuffer vb = null;
        private float angle = 0f;
        private CustomVertex.PositionNormalTextured[] vertices;
        private IndexBuffer ib = null;
        private int[] indices;
        private Bitmap b;
        private Texture tex1;

        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeDevice();
            VertexDeclaration();
            CameraPositioning();

            b = (Bitmap)Image.FromFile("Logo.bmp");
            tex1 = new Texture(device, b, 0, Pool.Managed);
        }

        public void InitializeDevice()
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
           // presentParams.AutoDepthStencilFormat = DepthFormat.D32SingleLockable;
           // presentParams.EnableAutoDepthStencil = true;
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            //включаем освещение
            device.RenderState.Lighting = true;

            //режим отбрасывания невидимых граней    
            device.RenderState.CullMode = Cull.CounterClockwise;
        }

        public void CameraPositioning()
        {
            device.Transform.Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4, (float)this.Width / (float)this.Height, 0.1f, 100f);
            device.Transform.View = Matrix.LookAtRH(new Vector3(10f, 10f, -5f),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 30));

            //включаем направленные источники света и настраиваем их
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(-1f, -1f, -1f);
            device.Lights[0].Enabled = true;
        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), 18, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            vertices = new CustomVertex.PositionNormalTextured[18];
            //дно
            float a = -1.5f; //координата Y первой точки
            float b = 3.0f; //Длина стороны
            float c = 0.0f; //смещение параллельной стороны относительно начальной стороны по Oy
            float d = 5.0f; //смещение параллельной стороны относительно начальной стороны по Ox


            vertices[0] = new CustomVertex.PositionNormalTextured(0f, a, 0f, 0f, 0f, 1f, 0f, 0f);
            vertices[1] = new CustomVertex.PositionNormalTextured(d, c + a, 0f, 0f, 0f, 1f, 1f, 0f);
            vertices[2] = new CustomVertex.PositionNormalTextured(0f, a + b, 0f, 0f, 0f, 1f, 0f, 1f);
            vertices[3] = new CustomVertex.PositionNormalTextured(0f, a + b, 0f, 0f, 0f, 1f, 0f, 1f);
            vertices[4] = new CustomVertex.PositionNormalTextured(d, c + a, 0f, 0f, 0f, 1f, 1f, 0f);
            vertices[5] = new CustomVertex.PositionNormalTextured(d, c + a + b, 0f, 0f, 0f, 1f, 1f, 1f);

            //передняя левая
            vertices[6] = new CustomVertex.PositionNormalTextured(0f, a, 0f, 1f, 0f, 0f, 1f, 0f);
            vertices[7] = new CustomVertex.PositionNormalTextured(0f, a + b, 0f, 1f, 0f, 0f, 0f, 0f);
            vertices[8] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, 1f, 0f, 0f, 0.5f, 0.5f);

            //боковая левая
            float norma1 = (float)Math.Sqrt(25 * c * c + 25 * d * d + d * d * (a + b) * (a + b));
            vertices[9] = new CustomVertex.PositionNormalTextured(0f, a + b, 0f, 5 * c / norma1, -5 * d / norma1, -d * (a + b) / norma1, 0f, 0f);
            vertices[10] = new CustomVertex.PositionNormalTextured(d, c + a + b, 0f, 5 * c / norma1, -5 * d / norma1, -d * (a + b) / norma1, 1f, 0f);
            vertices[11] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, 5 * c / norma1, -5 * d / norma1, -d * (a + b) / norma1, 0.5f, 0.5f);


            //(5c,-5d,-d(a+b))

            //задняя
            float norma2 = (float)Math.Sqrt(25 * b * b + b * b * d * d);
            vertices[12] = new CustomVertex.PositionNormalTextured(d, c + a + b, 0, -5 * b / norma2, 0f, -b * d / norma2, 1f, 0f);
            vertices[13] = new CustomVertex.PositionNormalTextured(d, c + a, 0f, -5 * b / norma2, 0f, -b * d / norma2, 0f, 0f);
            vertices[14] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, -5 * b / norma2, 0f, -b * d / norma2, 0.5f, 0.5f);

            //(-5b,0,-bd)

            //боковая правая
            float norma3 = (float)Math.Sqrt(25 * c * c + 25 * d * d + a * a * d * d);
            vertices[15] = new CustomVertex.PositionNormalTextured(d, c + a, 0f, -5 * c / norma3, -5 * d / norma3, a * d / norma3, 0f, 0f);
            vertices[16] = new CustomVertex.PositionNormalTextured(0f, a, 0f, -5 * c / norma3, -5 * d / norma3, a * d / norma3, 1f, 0f);
            vertices[17] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, -5 * c / norma3, -5 * d / norma3, a * d / norma3, 0.5f, 0.5f);
            //(-5c,5d,ad)
            vb.SetData(vertices, 0, LockFlags.None);

            //индексный буфер показывает, как вершины объединить в треугольники
            ib = new IndexBuffer(typeof(int), 18, device, Usage.WriteOnly, Pool.Default);
            indices = new int[18];

            //дно - по часовой стрелке
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 3;
            indices[4] = 4;
            indices[5] = 5;

            indices[6] = 6;
            indices[7] = 7;
            indices[8] = 8;

            indices[9] = 9;
            indices[10] = 10;
            indices[11] = 11;

            indices[12] = 12;
            indices[13] = 13;
            indices[14] = 14;

            indices[15] = 15;
            indices[16] = 16;
            indices[17] = 17;

            ib.SetData(indices, 0, LockFlags.None);

            ib = new IndexBuffer(typeof(int), indices.Length, device,
                     Usage.WriteOnly, Pool.Default);

            ib.SetData(indices, 0, LockFlags.None);


        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target, Color.DarkSlateBlue, 1.0f, 0);

            device.BeginScene();
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

            //установка вершин, индексов и текстуры
            device.SetStreamSource(0, vb, 0);
            device.Indices = ib;
            device.SetTexture(0, tex1);

            //материал показывает, как свет взаимодействует с поверхностью объекта
            Material M = new Material();
            M.Diffuse = Color.Green;
            M.Emissive = Color.Red;
            M.Ambient = Color.Moccasin;
            M.Specular = Color.WhiteSmoke;
            M.SpecularSharpness = 0.1f;
            device.Material = M;


            device.Transform.World = Matrix.RotationX(1*angle) * Matrix.RotationY(2*angle) * Matrix.RotationZ(3 * angle);
            //рисуем треугольники с использованием буфера индексов
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 18, 0, 6);

            device.EndScene();

            device.Present();

            this.Invalidate();
            angle += 0.01f;
           // Console.WriteLine(angle);
        }
    }
}
