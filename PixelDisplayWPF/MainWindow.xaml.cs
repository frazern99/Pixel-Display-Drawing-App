using Microsoft.VisualBasic;
using System.Data.Common;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;
using System.IO;
using System.Runtime.CompilerServices;


namespace PixelDisplayWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public static class Global
    {
        public static WriteableBitmap writeableBitmap;
        public static int colorX, colorY;
        public static Rectangle[,] rectArray = new Rectangle[41, 24];
        //public static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //public static System.IO.DirectoryInfo CreateDirectory(path);


    }
    public partial class MainWindow : Window
    {
        public static double s =100;
        public static double br = 100;
        public static double h = 0;
        public static double x= 0;
        public static double y = 0;
        public static int mode = 0;
        public static SolidColorBrush brush = new SolidColorBrush();
        public static bool mouseDown = false;

        double mod(double x, double m)
        {
            return (x % m + m) % m;
        }

        public int dimCalc(int margin, int canvSize, int recNum)
        {
            int result;

            result = (canvSize - (margin * (recNum+1)))/recNum;




            return result;
        }
        public int rgbCalc(double Hue, double Sat, double Bright)
        {

            double c,  x,rs, gs, bs, m;
            c = x = rs = gs = bs = m = 0;
            int result;
            Sat = Sat / 100;
            Bright = Bright / 100;
           
            c = Bright * Sat;
            x = c * (1 - Math.Abs(((Hue / 60)%2) - 1));
            m = Bright - c;
            double[] rgb = new double[3];

            if (Hue < 60)
            {
                rs = c;
                gs = x;
                bs = 0;


            }
            else if (Hue >= 60 && Hue < 120)
            {
                rs = x;
                gs = c;
                bs = 0;
            }
            else if (Hue >= 120 && Hue < 180)
            {
                rs = 0;
                gs = c;
                bs = x;
            }
            else if (Hue >= 180 && Hue < 240)
            {
                rs = 0;
                gs = x;
                bs = c;
            }
            else if (Hue >= 240 && Hue < 300)
            {
                rs = x;
                gs = 0;
                bs = c;
            }
            else if (Hue >= 240 && Hue < 300)
            {
                rs = x;
                gs = 0;
                bs = c;
            }
            else if (Hue >= 300 && Hue < 360)
            {

                rs = c;
                gs = 0;
                bs = x;
            }
            
            rgb[0] = Math.Ceiling((rs + m) * 255);
            rgb[1] = Math.Ceiling((gs + m) * 255);
            rgb[2] = Math.Ceiling((bs + m) * 255);

            result = (int)rgb[0] << 16;
            result |= (int)rgb[1] << 8;
            result |= (int)rgb[2] << 0;

            return Math.Abs(result);



            

        }

        public void hsvCalc(string colorHex)
        {
            
            int r = Convert.ToInt32(colorHex.Substring(3, 2), 16);
            int g = Convert.ToInt32(colorHex.Substring(5, 2), 16);
            int b = Convert.ToInt32(colorHex.Substring(7, 2), 16);

            double H = 0;
            double S;


            
            
            double RS = (double)r/ 255;
           
            double GS = (double)g/ 255;
            double BS = (double)b / 255;

            double Cmax = Math.Max(RS, Math.Max(GS, BS));
            
            double Cmin = Math.Min(RS, Math.Min(GS, BS));
            double Delta = Cmax - Cmin;
            
            
            if (Cmax == RS)
            {
                H = GS - BS;
                
                H = H / Delta;
                
                H = 60 * mod(H, 6);
                
            }
            else if (Cmax == GS)
            {
                H = 60 * (((BS - RS) / Delta) + 2);
            }
            else if (Cmax == BS)
            {
                H = 60 * (((RS - GS) / Delta) + 4);

                
            }

            if(GS-BS == 0 && Delta == 0)
            {
                H = 0;
            }



            if (Cmax == 0)
            {
                S = 0;
                H = 0;
            }
            else
            {
                S = Delta / Cmax;
            }

            
            s = Cmax*100;
            br = S*100;
            h = H;
             
            


        }

        public void eyeDropper()
        {
            try
            {
                colorSlider.Value = h;

                updateColorPicker(Global.writeableBitmap, (int)this.ColorGradient.Width, (int)ColorGradient.Height, h);
                updateCurrentColor();



                Canvas.SetLeft(ColorCursor, s - 10);
                Canvas.SetTop(ColorCursor, br - 10);
            }
            catch { }




        }

        public int updateColorPicker(WriteableBitmap writeableBitmap, int width, int height, double hue)
        {
            IntPtr pBackBuffer;
            int color_data;

            for (int i = 0; i <= width; i++)
            {
                for(int j= 0; j <height; j++) { 



                try
                {
                    // Reserve the back buffer for updates.
                    writeableBitmap.Lock();

                    unsafe
                    {
                        // Get a pointer to the back buffer.
                        pBackBuffer = writeableBitmap.BackBuffer;

                        // Find the address of the pixel to draw.
                        pBackBuffer += i * writeableBitmap.BackBufferStride;
                        pBackBuffer += j * 4;

                        // Compute the pixel's color.
                        color_data = rgbCalc(hue, (double)i, (double)j);
                        

                        // Assign the color data to the pixel.
                        *((int*)pBackBuffer) = color_data;
                    }

                    // Specify the area of the bitmap that changed.
                    writeableBitmap.AddDirtyRect(new Int32Rect(j, i, 1, 1));

                }
                catch
                {
                    return 0;
                }
                finally
                {
                    // Release the back buffer and make it available for display.
                    writeableBitmap.Unlock();

                }

                }

            }


            return 0;

               





            
        }

        public void updateCurrentColor()
        {
            
            string colorHex =rgbCalc(h, br, s).ToString("X");
            while(colorHex.Length < 6)
            {
                colorHex = "0" + colorHex;
            }
              colorHex = "#" + colorHex;  
                    Color color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(colorHex);
                     brush = new SolidColorBrush(color);
                     selectedColor.Fill = brush;
                    
                
                
            

        }
        public MainWindow()
        {
            InitializeComponent();
            
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Directory.CreateDirectory(path + "\\Documents\\Pixel");
             
            int rowNum = 41;
            int colNum = 24;
            int margin = 10;
            brush.Color = Colors.Black;
            Global.writeableBitmap = new WriteableBitmap(
                (int)this.ColorGradient.Width,
                (int)this.ColorGradient.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null);


            
            
            
            
            this.ColorGradient.Source = Global.writeableBitmap;

            updateColorPicker(Global.writeableBitmap, (int)this.ColorGradient.Width, (int)this.ColorGradient.Height, 0);


            this.TestText.Text = dimCalc(margin, (int)this.mainCanvas.Width, 8).ToString();
            int rectWidth = dimCalc(margin, (int)this.mainCanvas.Width, rowNum);
            int rectHeight = dimCalc(margin, (int)this.mainCanvas.Height, colNum);
            SolidColorBrush newcolor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    Global.rectArray[i,j] = new Rectangle 
                    {
                        Width = rectWidth,
                        Height = rectHeight,
                        Fill = newcolor,
                        
                        

                    };

                    Canvas.SetLeft(Global.rectArray[i, j], (rectWidth*i + margin));
                    Canvas.SetTop(Global.rectArray[i, j], (rectHeight*j + margin));

                    this.mainCanvas.Children.Add(Global.rectArray[i, j]);

                }
            }
            this.TestText.Text = Global.rectArray[0, 1].Fill.ToString();

            

            
        }

        private void colorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            int intvalue = Convert.ToInt32(e.NewValue);
            
            updateColorPicker(Global.writeableBitmap, (int)this.ColorGradient.Width, (int)this.ColorGradient.Height, e.NewValue);
            h = e.NewValue;
            updateCurrentColor();
            textblock1.Text = e.NewValue.ToString();
            
        }

        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            mouseDown = true;
            /*SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);

            Rectangle activeRect = (Rectangle)e.OriginalSource;

            switch (mode) {
                case 0:
                    
                    activeRect.Fill = brush;
                    break;
                case 1:
                    if(activeRect.Fill == whiteBrush)
                    {

                    }
                    else
                    {

                    }
                        break;

                case 2:

                    
                    hsvCalc(activeRect.Fill.ToString());
                    eyeDropper();
                    break;
                case 3:
                    
                    activeRect.Fill = whiteBrush;
                    break;

            }
            */
        }

        private void ColorCursor_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point Dropposition = e.GetPosition(this.ColorCanvas);
                DragDrop.DoDragDrop(ColorCursor, ColorCursor, DragDropEffects.Move);
                
                
            }

        }

       
            

        private void ColorCanvas_DragOver(object sender, DragEventArgs e)
        {
            System.Windows.Point Dropposition = e.GetPosition(this.ColorCanvas);
            
            Canvas.SetLeft(ColorCursor, Math.Clamp(Dropposition.X, 0, 100)-10);
            s = Math.Clamp(Dropposition.X, 0, 100);
            Canvas.SetTop(ColorCursor, Math.Clamp(Dropposition.Y, 0, 100) - 10);
            br = Math.Clamp(Dropposition.Y, 0, 100);
            this.TestText.Text = Dropposition.X.ToString();
            
            updateCurrentColor();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
            switch (mode)
            {
                case 1:
                    Fill.IsEnabled = true;
                    break;
                case 2:
                    EyeD.IsEnabled = true;
                    break;
                case 3:
                    Erase.IsEnabled = true;
                    break;

            }
            mode = 0;
            
            Paint.IsEnabled = false;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            switch (mode)
            {
                case 0:
                    Paint.IsEnabled = true;
                    break;
                case 2:
                    EyeD.IsEnabled = true;
                    break;
                case 3:
                    Erase.IsEnabled = true;
                    break;

            }


            
            mode = 1;
            
            Fill.IsEnabled = false;

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            
            switch (mode)
            {
                case 1:
                    Fill.IsEnabled = true;
                    break;
                case 0:
                    Paint.IsEnabled = true;
                    break;
                case 3:
                    Erase.IsEnabled = true;
                    break;
            }

            mode = 2;
          
            EyeD.IsEnabled = false;
        }

        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            switch (mode)
            {
                case 1:
                    Fill.IsEnabled = true;
                    break;
                case 0:
                    Paint.IsEnabled = true;
                    break;
                case 2:
                    EyeD.IsEnabled = true;
                    break;

            }

            mode = 3;

            Erase.IsEnabled = false;
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "\\New folder (26)\\PixelDisplayWPF\\PixelDisplayWPF\\Save.txt"; 
            string content = "";
            int multiplier = 20;
            int count = 0;
            int oddCount = 0;
            string temp;
            for (int j = 0; j < 10; j++) {
                for (int i = 0; i < 41; i++)
                {
                    if (j % 2 == 1)
                    {
                        temp = Global.rectArray[i, j].Fill.ToString();
                        content = content + "leds[" + count.ToString() + "] = 0x";
                        content = content + temp.Substring(3,6) + ";\n";
                        count++;
                        multiplier = 20;
                    }
                    else
                    {
                        temp = Global.rectArray[i, j].Fill.ToString();
                        oddCount = count + 2 * multiplier;
                        content = content + "leds[" + oddCount.ToString() + "] = 0x";
                        content = content + temp.Substring(3, 6) + ";\n";
                        count++;
                        multiplier = multiplier - 1;
                        
                    }


                }

            }
            count = 0;
            for (int j = 0; j < 14; j++)
            {
                for (int i = 0; i < 41; i++)
                {
                    if (j % 2 == 1)
                    {
                        temp = Global.rectArray[i, j+10].Fill.ToString();
                        content = content + "leds2[" + count.ToString() + "] = 0x";
                        content = content + temp.Substring(3, 6) + ";\n";
                        count++;
                        multiplier = 20;
                    }
                    else
                    {
                        temp = Global.rectArray[i, j+10].Fill.ToString();
                        oddCount = count + 2 * multiplier;
                        content = content + "leds2[" + oddCount.ToString() + "] = 0x";
                        content = content + temp.Substring(3, 6) + ";\n";
                        count++;
                        multiplier = multiplier - 1;

                    }


                }

            }

            File.WriteAllText(filePath, content);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

           
                using (var sw = new StreamWriter(path + "\\Documents\\Pixel\\save.txt", true))
                {
                    sw.WriteLine(content);
                }
            

           // File.WriteAllText(path + "\\Documents\\Pixel\\save.txt", content);

            TestText.Text = "Done";

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "\\New folder (26)\\PixelDisplayWPF\\PixelDisplayWPF\\Save.txt"; // Replace with your file path
            int linecount = 0;
            string[] stringArray = new string[984];
            SolidColorBrush tempbrush = new SolidColorBrush();
            

            var lines = File.ReadLines(filePath);
            foreach (var line in lines)
            {
                stringArray[linecount] = line;
                stringArray[linecount] = stringArray[linecount].Substring(12 + stringArray[linecount].Length - 19, 6);
                stringArray[linecount] = "#" +stringArray[linecount];
                

                linecount++;

            }
            

               

            this.TestText.Text = stringArray[linecount-1].ToString();
            linecount = 0;
             for (int j = 0; j < 24; j++)
             {
                for (int i = 0; i < 41; i++)
                {

                    Color color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(stringArray[linecount]);
                   tempbrush = new SolidColorBrush(color);
                    Global.rectArray[i, j].Fill = tempbrush;
                   linecount++;
                }

             } 

            







        }

        private void NextFrame_Click(object sender, RoutedEventArgs e)
        {
            string fileLast = "D:\\New folder (26)\\PixelDisplayWPF\\PixelDisplayWPF\\Last Frame.txt";
            string fileNext = "D:\\New folder (26)\\PixelDisplayWPF\\PixelDisplayWPF\\Next Frame.txt";
            string finalContent = "";

            int linecount = 0;

            string[] stringArrayNext = new string[984];
            string[] stringArrayLast = new string[984];


            var lines = File.ReadLines(fileLast);
            foreach (var line in lines)
            {

                stringArrayLast[linecount] = line;
                linecount++;

            }




            string content = "";
            int multiplier = 20;
            int count = 0;
            int countTotal = 0;
            int oddCount = 0;
            string temp;
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 41; i++)
                {
                    if (j % 2 == 1)
                    {
                        temp = Global.rectArray[i, j].Fill.ToString();
                        content = "leds[" + count.ToString() + "] = 0x" + temp.Substring(3, 6) + ";";
                        stringArrayNext[countTotal] = content;
                        count++;
                        multiplier = 20;
                        countTotal++;
                    }
                    else
                    {
                        temp = Global.rectArray[i, j].Fill.ToString();
                        oddCount = count + 2 * multiplier;
                        content = "leds[" + oddCount.ToString() + "] = 0x" + temp.Substring(3, 6) + ";";
                        stringArrayNext[countTotal] = content;
                        count++;
                        multiplier = multiplier - 1;
                        countTotal++;

                    }


                }

            }

            count = 0;
            for (int j = 0; j < 14; j++)
            {
                for (int i = 0; i < 41; i++)
                {
                    if (j % 2 == 1)
                    {
                        temp = Global.rectArray[i, j + 10].Fill.ToString();
                        content = "leds2[" + count.ToString() + "] = 0x" + temp.Substring(3, 6) + ";";
                        stringArrayNext[countTotal] = content;
                        count++;
                        multiplier = 20;
                        countTotal++;
                    }
                    else
                    {
                        temp = Global.rectArray[i, j + 10].Fill.ToString();
                        oddCount = count + 2 * multiplier;
                        content = "leds2[" + oddCount.ToString() + "] = 0x" + temp.Substring(3, 6) + ";";
                        stringArrayNext[countTotal] = content;
                        count++;
                        multiplier = multiplier - 1;
                        countTotal++;

                    }


                }

            }



            for (int i = 0; i < 984; i++)
            {
                if (!string.Equals(stringArrayLast[i], stringArrayNext[i]))
                {
                    finalContent = finalContent + stringArrayNext[i] + "\n";


                }

            }
            
            File.WriteAllText(fileNext, finalContent);




        }

        private void LastFrame_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "D:\\New folder (26)\\PixelDisplayWPF\\PixelDisplayWPF\\Last Frame.txt";
            string content = "";
            int multiplier = 20;
            int count = 0;
            int oddCount = 0;
            string temp;
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 41; i++)
                {
                    if (j % 2 == 1)
                    {
                        temp = Global.rectArray[i, j].Fill.ToString();
                        content = content + "leds[" + count.ToString() + "] = 0x";
                        content = content + temp.Substring(3, 6) + ";\n";
                        count++;
                        multiplier = 20;
                    }
                    else
                    {
                        temp = Global.rectArray[i, j].Fill.ToString();
                        oddCount = count + 2 * multiplier;
                        content = content + "leds[" + oddCount.ToString() + "] = 0x";
                        content = content + temp.Substring(3, 6) + ";\n";
                        count++;
                        multiplier = multiplier - 1;

                    }


                }

            }
            count = 0;
            for (int j = 0; j < 14; j++)
            {
                for (int i = 0; i < 41; i++)
                {
                    if (j % 2 == 1)
                    {
                        temp = Global.rectArray[i, j + 10].Fill.ToString();
                        content = content + "leds2[" + count.ToString() + "] = 0x";
                        content = content + temp.Substring(3, 6) + ";\n";
                        count++;
                        multiplier = 20;
                    }
                    else
                    {
                        temp = Global.rectArray[i, j + 10].Fill.ToString();
                        oddCount = count + 2 * multiplier;
                        content = content + "leds2[" + oddCount.ToString() + "] = 0x";
                        content = content + temp.Substring(3, 6) + ";\n";
                        count++;
                        multiplier = multiplier - 1;

                    }


                }

            }
            File.WriteAllText(filePath, content);


        }

        private void mainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;

        }

        private void mainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);

                Rectangle activeRect = (Rectangle)e.OriginalSource;

                switch (mode)
                {
                    case 0:

                        activeRect.Fill = brush;
                        break;
                    case 1:
                        if (activeRect.Fill == whiteBrush)
                        {

                        }
                        else
                        {

                        }
                        break;

                    case 2:


                        hsvCalc(activeRect.Fill.ToString());
                        eyeDropper();
                        break;
                    case 3:

                        activeRect.Fill = whiteBrush;
                        break;

                }




            }
            



        }
    }
}