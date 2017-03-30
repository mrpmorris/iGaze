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
			GazeSource = new GazeSources.Tobii.TobiiGazeSource();
		}
	}
}
