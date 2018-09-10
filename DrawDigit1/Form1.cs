using MnistModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawDigit1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private Bitmap digitImage;//用来保存手写数字
        private Point startPoint;//用于绘制线段，作为线段的初始端点坐标
        private Mnist model;//用于识别手写数字
        private const int MnistImageSize = 28;//Mnist模型所需的输入图片大小

        private void Form1_Load(object sender, EventArgs e)
        {
            //当窗口加载时，绘制一个白色方框
            model = new Mnist();
            digitImage = new Bitmap(picturebox1.Width, picturebox1.Height);
            Graphics g = Graphics.FromImage(digitImage);
            g.Clear(Color.White);
            picturebox1.Image = digitImage;
        }

        private void clean_click(object sender, EventArgs e)
        {
            //当点击清除时，重新绘制一个白色方框，同时清除label1显示的文本
            digitImage = new Bitmap(picturebox1.Width, picturebox1.Height);
            Graphics g = Graphics.FromImage(digitImage);
            g.Clear(Color.White);
            picturebox1.Image = digitImage;
            label1.Text = "";
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //当鼠标左键被按下时，记录下需要绘制的线段的起始坐标
            startPoint = (e.Button == MouseButtons.Left) ? e.Location : startPoint;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //当鼠标在移动，且当前处于绘制状态时，根据鼠标的实时位置与记录的起始坐标绘制线段，同时更新需要绘制的线段的起始坐标
            if (e.Button == MouseButtons.Left)
            {
                Graphics g = Graphics.FromImage(digitImage);
                Pen myPen = new Pen(Color.Black, 40);
                myPen.StartCap = LineCap.Round;
                myPen.EndCap = LineCap.Round;
                g.DrawLine(myPen, startPoint, e.Location);
                picturebox1.Image = digitImage;
                g.Dispose();
                startPoint = e.Location;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //当鼠标左键释放时
            //同时开始处理图片进行推理
            //暂时不处理这里的代码
            //当鼠标左键释放时
            //开始处理图片进行推理
            if (e.Button == MouseButtons.Left)
            {
                Bitmap digitTmp = (Bitmap)digitImage.Clone();//复制digitImage
                                                             //调整图片大小为Mnist模型可接收的大小：28×28
                using (Graphics g = Graphics.FromImage(digitTmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(digitTmp, 0, 0, MnistImageSize, MnistImageSize);
                }
                //将图片转为灰阶图，并将图片的像素信息保存在list中
                var image = new List<float>(MnistImageSize * MnistImageSize);
                for (var x = 0; x < MnistImageSize; x++)
                {
                    for (var y = 0; y < MnistImageSize; y++)
                    {
                        var color = digitTmp.GetPixel(y, x);
                        var a = (float)(0.5 - (color.R + color.G + color.B) / (3.0 * 255));
                        image.Add(a);
                    }
                }
                //将图片信息包装为mnist模型规定的输入格式
                var batch = new List<IEnumerable<float>>();
                batch.Add(image);
                //将图片传送给mnist模型进行推理
                var result = model.Infer(batch);
                //将推理结果输出
                label1.Text = result.First().First().ToString();
            }

        }

    }
}
