using iGaze.GazeSources;
using System;
using System.Speech.Synthesis;
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
		public static readonly DependencyProperty CurrentWordProperty = DependencyProperty.Register("CurrentWord", typeof(bool), typeof(MainWindow));
		private readonly TimeSpan GazeTimeMilliseconds = TimeSpan.FromSeconds(0.9);
		private GazeSource GazeSource;
		private DispatcherTimer Timer;
		private Action TimerAction;
		private SpeechSynthesizer Text2Speech;

		public MainWindow()
		{
			InitializeComponent();
			Calibrate();
			GazeSource = GazeSourceFactory.Create();
			GazeSource.UseCalibration = true;
			GazeSource.UseSmoothing = true;
			GazeSource.DataAvailable += GazeSource_DataAvailable;
			Timer = new DispatcherTimer();
			Timer.Interval = GazeTimeMilliseconds;
			Timer.IsEnabled = false;
			Timer.Tick += Timer_Tick;
			Text2Speech = new SpeechSynthesizer();
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
			if (InputText.Text.Length > 0)
			{
				string speech = InputText.Text.Replace('_', ' ');
				Text2Speech.SpeakAsync(speech);
				PreviousInputText.Text = "I said: " + speech;
				InputText.Text = "";
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (DateTime.UtcNow - GazeSource.DataTimeStamp < GazeTimeMilliseconds)
			{
				KeyHitAudio.Position = TimeSpan.Zero;
				KeyHitAudio.Play();
				TimerAction?.Invoke();
			}
		}


		private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.C)
				Calibrate();
		}

		private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Button control = (Button)sender;
			switch ((string)control.Tag)
			{
				case "OK":
					if (InputText.Text.Length > 0)
						TimeOut(() => SayText());
					break;

				case "DELETE":
					TimeOut(() =>
					{
						if (InputText.Text.Length > 0) InputText.Text = InputText.Text.Remove(InputText.Text.Length - 1);
					});
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

		private void GazeSource_DataAvailable(object sender, EventArgs e)
		{
			System.Windows.Forms.Cursor.Position = GazeSource.DataPoint;
		}

	}
}
