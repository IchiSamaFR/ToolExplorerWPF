using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ToolExplorerWPF.Services;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class NoiseVM : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private PerlinNoise2D _perlin;
        private CancellationTokenSource _cancellationTokenSource;

        [ObservableProperty]
        private BitmapSource _imageCreated;

        [ObservableProperty]
        private int _seed = 1;
        [ObservableProperty]
        private float _zoom = 100;

        partial void OnSeedChanged(int value)
        {
            _perlin = new PerlinNoise2D(Seed);
            GenerateNoise();
        }
        partial void OnZoomChanged(float value)
        {
            GenerateNoise();
        }


        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            _isInitialized = true;
            _perlin = new PerlinNoise2D(Seed);
            GenerateNoise();
        }

        [RelayCommand]
        public async void GenerateNoise()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            await GenerateNoiseAsync(_cancellationTokenSource.Token);
        }
        public async Task GenerateNoiseAsync(CancellationToken token)
        {
            int height = 1000;
            int width = 1000;
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.jpg");
            var pixelColors = new int[width, height];

            try
            {
                // Calculer les valeurs de Perlin en parallèle avec vérification du token d'annulation
                await Task.Run(() =>
                {
                    Parallel.For(0, width, (x, state) =>
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                state.Stop();
                                return;
                            }

                            var col = _perlin.NoiseClamped(x / Zoom, y / Zoom);
                            var grad = (int)MathF.Max(col * 255, 0);
                            pixelColors[x, y] = Color.FromArgb(255, grad, grad, grad).ToArgb();
                        }
                    });
                }, token);


                if (token.IsCancellationRequested)
                {
                    return;
                }

                using (var bmp = new Bitmap(width, height))
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bmp.SetPixel(x, y, Color.FromArgb(pixelColors[x, y]));
                        }
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        bmp.Save(memoryStream, ImageFormat.Jpeg);
                        memoryStream.Position = 0;
                        await File.WriteAllBytesAsync(imagePath, memoryStream.ToArray(), token);
                    }
                }

                await Task.Run(() =>
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = stream;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        ImageCreated = bitmapImage;
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                // Gestion de l'annulation
            }
        }

        public static BitmapSource BitmapImageFromFile(string filepath)
        {
            var bi = new BitmapImage();

            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                bi.BeginInit();
                bi.StreamSource = fs;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
            }

            bi.Freeze(); //Important to freeze it, otherwise it will still have minor leaks

            return bi;
        }
    }
}
