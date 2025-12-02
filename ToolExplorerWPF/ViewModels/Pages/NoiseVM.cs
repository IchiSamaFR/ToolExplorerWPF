using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using ToolExplorerWPF.Helpers.Extensions;
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
        private int _zoom = 100;
		[ObservableProperty]
        private int _pixelSize = 1;

        partial void OnSeedChanged(int value)
		{
			_perlin = new PerlinNoise2D(Seed);
			_ = GenerateNoise();
		}
		partial void OnZoomChanged(int value)
		{
			_ = GenerateNoise();
		}
		partial void OnPixelSizeChanged(int value)
		{
			_ = GenerateNoise();
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
			_ = GenerateNoise();
		}

		[RelayCommand]
		public async Task GenerateNoise()
		{
			if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
			{
				_cancellationTokenSource.Cancel();
			}

			_cancellationTokenSource = new CancellationTokenSource();
			await Task.Delay(500, _cancellationTokenSource.Token);
			await GenerateNoiseAsync(_cancellationTokenSource.Token);
		}

		public async Task GenerateNoiseAsync(CancellationToken token)
		{
			const int finalSize = 1000;
			int noiseWidth = finalSize / PixelSize + 1;
			int noiseHeight = finalSize / PixelSize + 1;
			float finalZoom = Zoom / PixelSize;
			var pixelColors = new int[noiseWidth, noiseHeight];

			try
			{
				var watch = System.Diagnostics.Stopwatch.StartNew();
				// Calculer les valeurs de Perlin en parallèle avec vérification du token d'annulation
				await Task.Run(() =>
				{
					Parallel.For(0, noiseWidth, (x, state) =>
					{
						for (int y = 0; y < noiseHeight; y++)
						{
							if (token.IsCancellationRequested)
							{
								state.Stop();
								return;
							}

							var col = _perlin.NoiseClamped(x, y, finalZoom);
							var grad = (int)MathF.Max(col * 255, 0);
							pixelColors[x, y] = Color.FromArgb(255, grad, grad, grad).ToArgb();
						}
					});
				}, token);
				watch.Stop();
				var test = watch.ElapsedMilliseconds;

				if (token.IsCancellationRequested)
				{
					return;
				}

				string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.jpg");
				using (var bmp = new Bitmap(finalSize, finalSize))
				{
					// Remplir l'image finale en étirant chaque pixel selon PixelSize
					for (int x = 0; x < noiseWidth; x++)
					{
						for (int y = 0; y < noiseHeight; y++)
						{
							Color pixelColor = Color.FromArgb(pixelColors[x, y]);

							bmp.SetPixels(x * PixelSize, y * PixelSize, PixelSize, pixelColor);
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