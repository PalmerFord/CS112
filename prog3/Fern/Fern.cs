using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FernNamespace
{
    /*
     * this class draws a fractal fern when the constructor is called.
     * Written as sample C# code for a CS 212 assignment -- October 2011.
     * 
     * Bugs: WPF and shape objects are the wrong tool for the task 
     */   
    class Fern
    {
        private static int BERRYMIN = 10;
        private static int TENDRILS = 2;
        private static int TENDRILMIN = 10;
        private static double DELTATHETA = 0.02;
        private static double SEGLENGTH = 3.0;

        /* 
         * Fern constructor erases screen and draws a fern
         * 
         * Size: number of 3-pixel segments of tendrils
         * Redux: how much smaller children clusters are compared to parents
         * Turnbias: how likely to turn right vs. left (0=always left, 0.5 = 50/50, 1.0 = always right)
         * canvas: the canvas that the fern will be drawn on
         */
        public Fern(double size, double redux, double turnbias, Canvas canvas)
        {
            canvas.Children.Clear();                                // delete old canvas contents
            // draw a new fern at the center of the canvas with given parameters
            cluster((int) (canvas.Width / 7), (int)(canvas.Height / 7), size, redux, turnbias, canvas);       
        }

        /*
         * cluster draws a cluster at the given location and then draws a bunch of tendrils out in 
         * regularly-spaced directions out of the cluster.
         */
        private void cluster(int x, int y, double size, double redux, double turnbias, Canvas canvas)
        {
            for (int i = 0; i < TENDRILS; i++)
            {
                // compute the angle of the outgoing tendril
                double theta = (i * 0.7 * Math.PI / TENDRILS) + Math.PI * 2;
                tendril(x, y, size, redux, turnbias, theta, canvas);
                if (size > BERRYMIN)
                    berry(x, y, 5, canvas);
                    Rectangle myRect = new Rectangle();
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = Color.FromArgb(255, 255, 230, 0);
                    myRect.Fill = mySolidColorBrush;
                    myRect.StrokeThickness = 1;
                    myRect.Stroke = Brushes.Black;
                    myRect.HorizontalAlignment = HorizontalAlignment.Center;
                    myRect.VerticalAlignment = VerticalAlignment.Center;
                    myRect.Width = 50;
                    myRect.Height = 50;

                canvas.Children.Add(myRect);
            }
        }

        /*
         * tendril draws a tendril (a randomly-wavy line) in the given direction, for the given length, 
         * and draws a cluster at the other end if the line is big enough.
         */
        private void tendril(int x1, int y1, double size, double redux, double turnbias, double direction, Canvas canvas)
        {
            int x2=x1, y2=y1;
            Random random = new Random();

            for (int i = 0; i < size; i++)
            {
                direction += (random.NextDouble() > turnbias) ? -1 * DELTATHETA : DELTATHETA;
                x1 = x2; y1 = y2;
                x2 = x1 + (int)(SEGLENGTH * Math.Sin(direction));
                y2 = y1 + (int)(SEGLENGTH * Math.Cos(direction));
                byte red = (byte) (100+size/2);
                byte green = (byte)(220 - size / 3);
                //if (size>120) red = 138; green = 108;
                line(x1, y1, x2, y2, red, green, 0, 1+size/80, canvas);
                if ((i % 10) == 0 && size > 5)
                {
                    tendril(x1, y1, size * 0.3, redux, turnbias, 1.05 * direction, canvas);
                    tendril(x1, y1, size * 0.3, redux, (0.5 + turnbias - 0.5) * (-1), direction, canvas);

                }
                if ((i % 20) == 0 && size > 20)
                {
                    berry(x1, y1, random.Next(0,6), canvas);
                }
                if (i >= size-1 && size > 20)
                {
                    tendril(x1, y1, size * 0.4, redux, random.NextDouble(), direction, canvas);
                }
            }
//            if (size > TENDRILMIN)
//                cluster(x2, y2, size / redux, redux, turnbias, canvas);
        }

        /*
         * draw a red circle centered at (x,y), radius radius, with a black edge, onto canvas
         */
        private void berry(int x, int y, double radius, Canvas canvas)
        {
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 0, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 2 * radius;
            myEllipse.Height = 2 * radius;
            myEllipse.SetCenter(x, y);
            canvas.Children.Add(myEllipse);
        }

        /*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
        private void line(int x1, int y1, int x2, int y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }
    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
        Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
    }
}

