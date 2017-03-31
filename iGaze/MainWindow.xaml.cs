using iGaze.GazeSources;
using System;
using System.Linq;
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
		public static readonly DependencyProperty CurrentWordProperty = DependencyProperty.Register("CurrentWord", typeof(string), typeof(MainWindow));
		private readonly TimeSpan GazeTimeMilliseconds = TimeSpan.FromSeconds(0.9);
		private GazeSource GazeSource;
		private DispatcherTimer Timer;
		private Action TimerAction;
		private SpeechSynthesizer Text2Speech;
		private Button HighlightedButton;

		public MainWindow()
		{
			InitializeComponent();
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
				SetText("");
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
			HighlightedButton = (Button)sender;
			RepositionToolTip();
			switch ((string)HighlightedButton.Tag)
			{
				case "OK":
					if (InputText.Text.Length > 0)
						TimeOut(() => SayText());
					break;

				case "DELETE":
					TimeOut(() =>
					{
						if (InputText.Text.Length > 0) SetText(InputText.Text.Remove(InputText.Text.Length - 1));
					});
					break;

				default:
					TimeOut(() => SetText(InputText.Text + (string)HighlightedButton.Tag));
					break;
			}
		}

		private void RepositionToolTip()
		{
			var point = HighlightedButton.TransformToAncestor(this).Transform(new Point(0, 0));
			Canvas.SetLeft(CurrentWordToolTip, point.X + (HighlightedButton.ActualWidth / 2) - (CurrentWordToolTip.ActualWidth / 2));
			Canvas.SetTop(CurrentWordToolTip, point.Y + (HighlightedButton.ActualHeight / 2) - (CurrentWordToolTip.ActualHeight / 2) + 50);
		}

		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Timer.IsEnabled = false;
			TimerAction = null;
			HighlightedButton = null;
		}

		private void GazeSource_DataAvailable(object sender, EventArgs e)
		{
			System.Windows.Forms.Cursor.Position = GazeSource.DataPoint;
		}

		private void SetText(string value)
		{
			InputText.Text = value ?? "";
			string currentWord = InputText.Text.Split('_').Last();
			SetValue(CurrentWordProperty, currentWord);
			RepositionToolTip();
		}

	}
}
