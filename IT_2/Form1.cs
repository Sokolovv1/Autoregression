//Оценить границы фрагмента эталонного
//гармонического сигнала с известной частотой, расположенного внутри
//массива, содержащего гармонические сигналы с другими частотами.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IT_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series["Исходный сигнал"].Points.Clear();
            chart2.Series["График отклонения"].Points.Clear();
            chart2.Series["Сглаженный график отклонения"].Points.Clear();
            chart2.Series["Порог ошибки"].Points.Clear();
            //double pi = 3.1415926535;
            double F0 = Convert.ToDouble(textBoxf0.Text);
            double F1 = Convert.ToDouble(textBoxf1.Text);
            double F2 = Convert.ToDouble(textBoxf2.Text);
            double n1 = Convert.ToDouble(textBoxn1.Text);
            double n2 = Convert.ToDouble(textBoxn2.Text);
            double Fd = Convert.ToDouble(textBoxfd.Text);
            int Width = Convert.ToInt32(textBoxPoints.Text);
            double ErrorThreshold = Convert.ToDouble(textBoxLevelError.Text);
            int N = Convert.ToInt32(textBoxN.Text);
            double[] points = new double[N];
            
            double[] points1_Shum = new double[N];
            double[] ArrayNoise_old = new double[N];
            double[] ArrayNoise_new = new double[N];
            double[] xArrayNoised = new double[N];

            
            double dt = 1 / Fd;
            double fi = 0;
            int N1search = 0;
            int N2search = 0;
            #region График чистого сигнала
            for (int t = 1; t < N; t++)
            {

                if (t < n1)
                {
                    fi += 2 * Math.PI * F1 * dt;
                    points[t] = Math.Sin(fi);
                }

                else if (t >= n1 & t <= n2)
                {
                    fi += 2 * Math.PI * F0 * dt;
                    points[t] = Math.Sin(fi);
                }
                else if (t > n2)
                {
                    fi += 2 * Math.PI * F2 * dt;
                    points[t] = Math.Sin(fi);
                }
                chart1.Series["Исходный сигнал"].Points.AddXY(t, points[t]);
            }
            #endregion
            #region Прогнозируемый Сигнал
            double[] Signal2 = new double[N];//Прогназируемый сигнал
            double[] Errors = new double[N - 2];//Ошибки
            Signal2[0] = 1;
            Signal2[1] = 1;
            for (int i = 2; i < N; i++)
            {
                Signal2[i] = points[i - 1] * 2 * Math.Cos(2 * Math.PI * F0 * dt) - points[i - 2];


                Errors[i - 2] = Math.Pow((Signal2[i] - points[i]), 2);

                chart2.Series["График отклонения"].Points.AddXY(i, Errors[i - 2]);//График отклонения
                chart2.Series["Порог ошибки"].Points.AddXY(i, ErrorThreshold);//Порог
            }
            #endregion
            #region Search_N

            double[] FilteredErrors = new double[N - 2 - 2 * Width];//массив отфильтрованных ошибок
            for (int i = Width; i < N - 2 - Width; i++)
            {
                FilteredErrors[i - Width] = 0;
                for (int j = i - Width; j < i + Width; j++)
                {
                    FilteredErrors[i - Width] += Errors[j];
                }
                FilteredErrors[i - Width] /= (2 * Width + 1);
            }

            for (int i = Width; i < N - 2 - Width; i++)
            {
                chart2.Series["Сглаженный график отклонения"].Points.AddXY(i, FilteredErrors[i - Width]);
            }

            bool start = true;
            bool end = true;
            for (int i = 0; i < N - 2 - 2 * Width; i++)
            {
                if (FilteredErrors[i] <= ErrorThreshold && start)
                {
                    N1search = i + 2;
                    start = false;
                }

                if (FilteredErrors[i] >= ErrorThreshold && !start && end)
                {
                    N2search = i + 2;
                    end = false;
                }
            }
            textBoxFoundInterval1.Text = N1search.ToString();
            textBoxFoundInterval2.Text = N2search.ToString();

            #endregion



        }

        
    }
}
