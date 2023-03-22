using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektFIltrLaplaca
{
    public partial class MainWindow : Window
    {
        public byte[] array;
        public byte[] arrayWithBorder;
        public byte[] outArray;

        int height;
        int width;

        int heightWithBorder;
        int widthWitchBorder;

        Bitmap bitmap;

        bool algorythmInCppWasChoosed = true;
        string filePathToFiltredPhoto;
        string defaultFilePath = "testowe\\photos\\6.bmp";
        string fileName;

        Stopwatch stopwatch;

        [DllImport("Algorythm_cpp.dll", EntryPoint = "doAlgorythmInCpp", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void doAlgorythmInCpp(byte* outList,byte* inListStart, int size ,int height);

        //zmienic sciezke
        [DllImport("AlgorythmInAsm.dll", EntryPoint = "doAlgorythmInAsm", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int doAlgorythmInAsm(byte* outList, byte* inListStart, int size, int height);


        public MainWindow()
        {
            InitializeComponent();
        }

        void runBackend(string filePath, int numberOfThreads, bool algorythmInCppWasChoosed)
        {
            initialize(filePath);
            readImage();
            createArrayWithBorder();

            List<Thread> threads = new List<Thread>();
            int size = height * width;
            long sizeForOneThread = size / numberOfThreads;
            long reminder = (bitmap.Width * bitmap.Height) - sizeForOneThread * numberOfThreads;
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = numberOfThreads;
            
            stopwatch.Reset();
            stopwatch.Start();
            var zmienna = Parallel.For(0, numberOfThreads, parallelOptions, i =>
            {
                int startIndex = (int)(i * sizeForOneThread + i);
                int endIndex = (int)((i + 1) * sizeForOneThread + i);
                startIndex *= 3;
                endIndex = (endIndex * 3) + 3;
                if (i == numberOfThreads - 1)
                {
                    endIndex = arrayWithBorder.Length;
                }
                if (i == 0)
                {
                    startIndex = (height + 2) * 3 + 4;
                }

                int sizeOfTheArrayToProcess = endIndex - startIndex;

                if (algorythmInCppWasChoosed)
                {
                    doAlgorythmInCpp(startIndex, sizeOfTheArrayToProcess);
                   
                }
                else
                {
                    doAlgorythmInAsm(startIndex, sizeOfTheArrayToProcess);
                }
            });

            stopwatch.Stop();

            var newBitmap = createNewBitMapFromOutptArray();
            saveNewImage(newBitmap);

            var bitmapImage = new BitmapImage(new Uri(filePathToFiltredPhoto));
            image.Source = bitmapImage;
        }

        private bool IsFileLocked(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        stream.Close();
                    }
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
       
        void initialize(string filePath)
        {
            FileStream fs = null;
            Bitmap tmpBmp = null;
            try
            {
                fs = File.Open(filePath, FileMode.Open, FileAccess.Read);
                tmpBmp = new Bitmap(fs); //  if photo has 32 depth in bits, we can't create a bitmap. Usually we have photos in 24b;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("This photo is in wrong format. Another photo will be processed");
            }

            // if tmpBmp wasn't created, create use photo from default path 
            if (tmpBmp==null)
            {
                fs = File.Open(defaultFilePath, FileMode.Open, FileAccess.Read);
                tmpBmp = new Bitmap(fs);
            }
            var a = tmpBmp.Clone();
            bitmap = new Bitmap((Image)a);
            tmpBmp.Dispose();
            tmpBmp = null;
            fs.Close();
       
            // image
            height = bitmap.Height;
            width = bitmap.Width;
            int sizeArray = width * height * 3;
            array = new byte[sizeArray];

            // image with border
            heightWithBorder = bitmap.Height + 2;
            widthWitchBorder = bitmap.Width + 2;
            var newSize = (heightWithBorder * widthWitchBorder) * 3;
            arrayWithBorder = new byte[newSize];
            outArray = new byte[newSize];

            // initialize array with 255 
            for (int i =0; i < arrayWithBorder.Length;i++)
            {
                arrayWithBorder[i] = 255;
            }
           
            fileName = filePath.Substring(filePath.LastIndexOf("\\"));
            filePathToFiltredPhoto = "C:\\Users\\hania\\Desktop\\Projekt\\Plik_exe\\testowe\\photosFiltered" + fileName;

            stopwatch = new Stopwatch();
        }
        int convert2dTo1d(int i, int j, bool withBorder)
        {
            var newHeight = height;
            if (withBorder)
            {
                newHeight += 2;
            }
            return i * newHeight + j;
        }
        void readImage()
        {
            for (int i = 0; i < bitmap.Width; i++)//3 razy
            {
                for (int j = 0; j < bitmap.Height; j++) // 3 razy
                {
                    System.Drawing.Color pixelColor = bitmap.GetPixel(i, j);
                    var index = j + i * bitmap.Height;
                    index *= 3;
                    array[index] = pixelColor.R;
                    array[index + 1] = pixelColor.G;
                    array[index + 2] = pixelColor.B;
                }
            }
        }
        void createArrayWithBorder()
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var currentBorderIndex = convert2dTo1d(i + 1, j + 1, true) * 3;
                    var currentIndex = convert2dTo1d(i, j, false) * 3;
                    arrayWithBorder[currentBorderIndex] = array[currentIndex];
                    arrayWithBorder[currentBorderIndex + 1] = array[currentIndex + 1];
                    arrayWithBorder[currentBorderIndex + 2] = array[currentIndex + 2];
                }
            }
        }
        int getThreadsNumber()
        {
            int number = Int32.Parse(numbersOfThreads.Text);
            if (number > 64)
            {
                numbersOfThreads.Text = "64";
            }
            if (number < 0)
            {
                numbersOfThreads.Text = "1";
            }
            return number;
        }
        void showAlgorythmTime()
        {
            Time.Text =
                "Time: "
                + stopwatch.ElapsedMilliseconds
                + "ms\n"
                + "Ticks: "
                + stopwatch.ElapsedTicks;
        }
        
        void doAlgorythmInCpp(int startIndex, int sizeOfTheArrayToProcess)
        {
            unsafe
            {
                fixed (byte* bitmapPointerIn =&arrayWithBorder[startIndex])
                {
                    fixed (byte* bitmapPointerOut = &outArray [startIndex])
                    {
                        doAlgorythmInCpp(bitmapPointerIn, bitmapPointerOut, sizeOfTheArrayToProcess, heightWithBorder * 3); // mozliwa zmiana width i high jakby nie dzialalo 
                    }
                }
            }
        }
        void doAlgorythmInAsm(int startIndex, int sizeOfTheArrayToProcess)
        {
            unsafe
            {
                fixed (byte* bitmapPointerIn = &arrayWithBorder[startIndex])
                {
                    fixed (byte* bitmapPointerOut = &outArray[startIndex])
                    {
                        doAlgorythmInAsm(bitmapPointerIn, bitmapPointerOut, sizeOfTheArrayToProcess, heightWithBorder*3);
                    }
                }
            }
        }
       
        Bitmap createNewBitMapFromOutptArray()
        {
            Bitmap newBitmap = new Bitmap(widthWitchBorder, heightWithBorder);
            for (int i = 0; i < widthWitchBorder; i++)
            {
                for (int j = 0; j < heightWithBorder; j++)
                {
                    var index = (j + i * heightWithBorder) * 3;
                    var pixelColorR = outArray[index];
                    var pixelColorG = outArray[index + 1];
                    var pixelColorB = outArray[index + 2];

                    var pixelColor = System.Drawing.Color.FromArgb(255, pixelColorR, pixelColorG, pixelColorB);

                    newBitmap.SetPixel(i, j, pixelColor);
                }
            }
            return newBitmap;
        }
        void saveNewImage(Bitmap bitmap)
        {
            int i = 0;
            while (IsFileLocked(filePathToFiltredPhoto))
            {
                var newFileName = fileName.Replace(".", i + ".");
                filePathToFiltredPhoto = "C:\\Users\\hania\\Desktop\\Projekt\\Plik_exe\\testowe\\photosFiltered" + newFileName;
                i++;
            }
            Bitmap b = bitmap.Clone(new Rectangle(1,1,width, height), bitmap.PixelFormat);
            b.Save(filePathToFiltredPhoto);
        }


        //buttons 
        private void btn_openFolders_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image Files| *.jpg; *.jpeg; *.png; *.bmp; ...";


            //When the user select the file
            if (openFileDialog.ShowDialog() == true)
            {
                //Get the file's path
                var filePath = openFileDialog.FileName;
                text_searchbox.Text = filePath;
                text_searchbox.FontSize = 18;
            }
        }
        private void btn_filterAsm_Click(object sender, RoutedEventArgs e)
        {
            btn_filterAsm.Background = new SolidColorBrush(Colors.BlueViolet);
            btn_filterCpp.Background = new SolidColorBrush(Colors.White);
        }
        private void btn_filterCpp_Click(object sender, RoutedEventArgs e)
        {
            btn_filterCpp.Background = new SolidColorBrush(Colors.BlueViolet);
            btn_filterAsm.Background = new SolidColorBrush(Colors.White);
        }
        private void btn_restart_Click(object sender, RoutedEventArgs e)
        {
            image.Source = null;
            btn_runAlgorythm.IsEnabled = true;
            btn_filterCpp.IsEnabled = true;
            btn_filterAsm.IsEnabled = true;
            text_searchbox.IsEnabled = true;
            btn_openFolders.IsEnabled = true;
            btn_runAlgorythm.Background = new SolidColorBrush(Colors.White);
            text_searchbox.Text = " Select photo to filter";
            bitmap = null;
            filePathToFiltredPhoto = null;
            arrayWithBorder = null;
            array = null;
            outArray = null;

        }
        private void btn_runAlgorythm_Click(object sender, RoutedEventArgs e)
        {
            if (((SolidColorBrush)btn_filterCpp.Background).Color == Colors.BlueViolet)
            {
                Console.WriteLine("cpp");
                algorythmInCppWasChoosed = true;
            }
            else
            {
                Console.WriteLine("asm");
                algorythmInCppWasChoosed = false;
            }

            if (!File.Exists(text_searchbox.Text))
            {
                text_searchbox.Text = defaultFilePath;
            }

            btn_runAlgorythm.Background = new SolidColorBrush(Colors.BlueViolet);

            btn_runAlgorythm.IsEnabled = false;
            btn_filterCpp.IsEnabled = false;
            btn_filterAsm.IsEnabled = false;
            btn_openFolders.IsEnabled = false;
            text_searchbox.IsEnabled = false;

            runBackend(text_searchbox.Text, getThreadsNumber(), algorythmInCppWasChoosed);
            showAlgorythmTime();

        }
    }
}