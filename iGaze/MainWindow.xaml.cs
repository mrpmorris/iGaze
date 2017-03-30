using iGaze.GazeSources;
using System.Windows;

namespace iGaze
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private GazeSource GazeSource;

		public MainWindow()
		{
			InitializeComponent();
			Calibrate();
			GazeSource = GazeSourceFactory.Create();
		}

		private static void Calibrate()
		{
			var calibrationWindow = new CalibrationWindow();
			calibrationWindow.ShowDialog();
		}
	}
}
