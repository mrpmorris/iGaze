using iGaze.GazeSources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace iGaze
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly TimeSpan GazeTimeMilliseconds = TimeSpan.FromSeconds(1);
		private GazeSource GazeSource;
		private DispatcherTimer Timer;
		private Action TimerAction;

		public MainWindow()
		{
			InitializeComponent();
			Calibrate();
			GazeSource = GazeSourceFactory.Create();
			GazeSource.UseCalibration = true;
			GazeSource.UseSmoothing = true;
			Timer = new DispatcherTimer();
			Timer.Interval = GazeTimeMilliseconds;
			Timer.IsEnabled = false;
			Timer.Tick += Timer_Tick;
		}

		private void Calibrate()
		{
			var calibrationWindow = new CalibrationWindow();
			calibrationWindow.ShowDialog();
		}

		private void TimeOut(Action action)
		{
			Timer.IsEnabled = false;
			TimerAction = action;
			Timer.IsEnabled = true;
		}

		private void SayText()
		{
			PreviousInputText.Text = "I said: " + InputText.Text;
			InputText.Text = "";
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			TimerAction?.Invoke();
		}


		private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.C)
				Calibrate();
		}

		private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Button control = (Button)sender;
			switch((string)control.Tag)
			{
				case "OK":
					if (InputText.Text.Length > 0)
						TimeOut(() => SayText());
					break;

				case "DELETE":
					if (InputText.Text.Length > 0)
						TimeOut(() => InputText.Text = InputText.Text.Remove(InputText.Text.Length - 1));
					break;

				case "_":
					TimeOut(() => InputText.Text += " ");
					break;

				default:
					TimeOut(() => InputText.Text += (string)control.Tag);
					break;
			}
		}

		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Timer.IsEnabled = false;
			TimerAction = null;
		}
	}
}
