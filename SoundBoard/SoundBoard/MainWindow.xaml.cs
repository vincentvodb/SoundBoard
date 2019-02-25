using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isSoundPlaying = false;
        private string _path = "Sounds/{0}.wav";
        private string _fileName = "soundtracks.json";
        private SoundPlayer _player;
        private Brush _background;
        private Button _prevClick;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSoundBoard();
        }

        public void InitializeSoundBoard()
        {
            _isSoundPlaying = false;
            var tracks = ReadTrackNames();

            var rows = 4;
            var columns = 7;

            for(var i = 0; i < columns; i++)
            {
                var column = new StackPanel();

                for (var j = 0; j < rows; j++)
                {
                    var name = tracks[string.Format("{0}{1}", i, j)];

                    if (name.Length > 13)
                        name = name.Substring(0, 13) + "...";

                    var label = new Label {
                        Content = name,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold
                    };
                    var button = new Button {
                        Height = 100,
                        Width = 100,
                        Margin = new Thickness(5, 0, 5, 0),
                        Background = new RadialGradientBrush
                        {
                            RadiusX = 0.5,
                            GradientStops = new GradientStopCollection
                            {
                                new GradientStop
                                {
                                    Color = Color.FromRgb(250, 250, 210),
                                    Offset = 0.2
                                },
                                new GradientStop
                                {
                                    Color = Color.FromRgb(255, 255, 0),
                                    Offset = 1.0
                                },
                            }
                        },
                        Tag = string.Format("{0},{1}", i, j)
                    };

                    if (string.IsNullOrEmpty(name))
                    {
                        button.Background = new RadialGradientBrush
                        {
                            RadiusX = 0.5,
                            GradientStops = new GradientStopCollection
                            {
                                new GradientStop
                                {
                                    Color = Color.FromRgb(148, 148, 148),
                                    Offset = 0.2
                                },
                                new GradientStop
                                {
                                    Color = Color.FromRgb(128, 128, 128),
                                    Offset = 1.0
                                },
                            }
                        };
                        column.Children.Add(label);
                        column.Children.Add(button);
                        continue;
                    }

                    button.Click += new RoutedEventHandler(BtnClicked);
                    column.Children.Add(label);
                    column.Children.Add(button);
                }

                Soundboard.Children.Add(column);
            }
        }

        public void BtnClicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var tag = button.Tag.ToString().Replace(",", string.Empty);
            
            if(_prevClick != null)
                _prevClick.Background = _background;

            if (_isSoundPlaying && _prevClick.Tag.ToString().Replace(",", string.Empty) == tag)
            {
                _isSoundPlaying = false;
                _player.Stop();
                button.Background = _background;
                return;
            }

            _background = button.Background;
            button.Background = new RadialGradientBrush
            {
                RadiusX = 0.5,
                GradientStops = new GradientStopCollection
                            {
                                new GradientStop
                                {
                                    Color = Color.FromRgb(250, 10, 110),
                                    Offset = 0.2
                                },
                                new GradientStop
                                {
                                    Color = Color.FromRgb(255, 0, 0),
                                    Offset = 1.0
                                },
                            }
            };

            if (string.IsNullOrEmpty(tag))
                return;
            try
            {
                _player = new SoundPlayer
                {
                    SoundLocation = string.Format(_path, tag)
                };

                _isSoundPlaying = true;
                _player.Play();
            }
            catch
            {
                _isSoundPlaying = false;
                button.Background = _background;
                _player.Stop();
            }

            _prevClick = button;
        }

        private Dictionary<string, string> ReadTrackNames()
        {
            using (var r = new StreamReader(_fileName))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }
    }
}
