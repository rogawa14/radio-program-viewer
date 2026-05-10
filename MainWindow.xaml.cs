using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;

namespace RadioProgramViewer
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer =
            new DispatcherTimer();

        bool darkMode = false;

        List<XElement> currentPrograms =
            new List<XElement>();

        Dictionary<string, string> areaIds =
            new Dictionary<string, string>()
        {
            {"東京", "JP13"},
            {"大阪", "JP27"},
            {"福岡", "JP40"}
        };

        Dictionary<string, Dictionary<string, string>> areaStations =
            new Dictionary<string, Dictionary<string, string>>()
        {
            {
                "東京",
                new Dictionary<string, string>()
                {
                    {"TOKYO FM", "FMT"},
                    {"J-WAVE", "FMJ"},
                    {"interfm", "INT"},
                    {"BAYFM78", "BAYFM78"},
                    {"NACK5", "NACK5"},
                    {"FMヨコハマ", "YFM"},
                    {"TBSラジオ", "TBS"},
                    {"文化放送", "QRR"},
                    {"ニッポン放送", "LFR"}
                }
            },

            {
                "大阪",
                new Dictionary<string, string>()
                {
                    {"FM802", "802"},
                    {"FM COCOLO", "CCL"},
                    {"FM大阪", "FMO"}
                }
            },

            {
                "福岡",
                new Dictionary<string, string>()
                {
                    {"RKBラジオ", "RKB"},
                    {"KBCラジオ", "KBC"}
                }
            }
        };

        public MainWindow()
        {
            InitializeComponent();

            cmbArea.Items.Add("東京");
            cmbArea.Items.Add("大阪");
            cmbArea.Items.Add("福岡");

            cmbArea.SelectionChanged +=
                cmbArea_SelectionChanged;

            btnGet.Click +=
                btnGet_Click;

            btnPlay.Click +=
                btnPlay_Click;

            btnTheme.Click +=
                btnTheme_Click;

            lstPrograms.SelectionChanged +=
                lstPrograms_SelectionChanged;

            cmbArea.SelectedIndex = 0;

            timer.Interval =
                TimeSpan.FromSeconds(30);

            timer.Tick +=
                Timer_Tick;

            timer.Start();
        }

        private void Timer_Tick(
            object sender,
            EventArgs e)
        {
            btnGet_Click(null, null);
        }

        private void btnTheme_Click(
            object sender,
            RoutedEventArgs e)
        {
            darkMode = !darkMode;

            if (darkMode)
            {
                this.Background =
                    new SolidColorBrush(
                        Color.FromRgb(15, 23, 42));

                RootGrid.Background =
                    new SolidColorBrush(
                        Color.FromRgb(15, 23, 42));

                SetDarkCard(HeaderCard);
                SetDarkCard(ThumbCard);
                SetDarkCard(InfoCard);
                SetDarkCard(DescCard);
                SetDarkCard(ListCard);

                txtHeader.Foreground = Brushes.White;
                txtSub.Foreground = Brushes.LightGray;
                txtNow.Foreground = Brushes.DeepSkyBlue;

                txtDescTitle.Foreground = Brushes.White;
                txtListTitle.Foreground = Brushes.White;

                lblTitle.Foreground = Brushes.White;
                lblStation.Foreground = Brushes.White;
                lblTime.Foreground = Brushes.White;

                lblStart.Foreground = Brushes.LightGray;
                lblEnd.Foreground = Brushes.LightGray;
                lblProgress.Foreground = Brushes.White;

                txtDesc.Foreground = Brushes.White;

                lstPrograms.Foreground = Brushes.White;

                btnTheme.Content =
                    "ライトモード";
            }
            else
            {
                this.Background =
                    new SolidColorBrush(
                        Color.FromRgb(243, 245, 247));

                RootGrid.Background =
                    new SolidColorBrush(
                        Color.FromRgb(243, 245, 247));

                SetLightCard(HeaderCard);
                SetLightCard(ThumbCard);
                SetLightCard(InfoCard);
                SetLightCard(DescCard);
                SetLightCard(ListCard);

                txtHeader.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(17, 24, 39));

                txtSub.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(107, 114, 128));

                txtNow.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(37, 99, 235));

                txtDescTitle.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(17, 24, 39));

                txtListTitle.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(17, 24, 39));

                lblTitle.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(17, 24, 39));

                lblStation.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(55, 48, 163));

                lblTime.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(55, 65, 81));

                lblStart.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(107, 114, 128));

                lblEnd.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(107, 114, 128));

                lblProgress.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(17, 24, 39));

                txtDesc.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(55, 65, 81));

                lstPrograms.Foreground =
                    Brushes.Black;

                btnTheme.Content =
                    "ダークモード";
            }
        }

        private void SetDarkCard(Border border)
        {
            border.Background =
                new SolidColorBrush(
                    Color.FromRgb(30, 41, 59));
        }

        private void SetLightCard(Border border)
        {
            border.Background =
                Brushes.White;
        }

        private void cmbArea_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            cmbFreq.Items.Clear();

            string area =
                cmbArea.SelectedItem.ToString();

            foreach (var station in areaStations[area].Keys)
            {
                cmbFreq.Items.Add(station);
            }

            cmbFreq.SelectedIndex = 0;
        }

        private async void btnGet_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                lblLoading.Visibility =
                    Visibility.Visible;

                lstPrograms.Items.Clear();

                currentPrograms.Clear();

                string area =
                    cmbArea.SelectedItem.ToString();

                string stationName =
                    cmbFreq.SelectedItem.ToString();

                string stationId =
                    areaStations[area][stationName];

                string areaId =
                    areaIds[area];

                string today =
                    DateTime.Now.ToString("yyyyMMdd");

                string url =
                    $"https://api.radiko.jp/program/v3/date/{today}/area/{areaId}.xml";

                using (HttpClient client =
                    new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(
                        "User-Agent",
                        "Mozilla/5.0");

                    string xml =
                        await client.GetStringAsync(url);

                    XDocument doc =
                        XDocument.Parse(xml);

                    DateTime now =
                        DateTime.Now;

                    foreach (var station in
                        doc.Descendants("station"))
                    {
                        string id =
                            station.Attribute("id")?.Value;

                        if (id != stationId)
                        {
                            continue;
                        }

                        foreach (var prog in
                            station.Descendants("prog"))
                        {
                            currentPrograms.Add(prog);

                            string ft =
                                prog.Attribute("ft")?.Value ?? "";

                            string to =
                                prog.Attribute("to")?.Value ?? "";

                            DateTime start =
                                DateTime.ParseExact(
                                    ft,
                                    "yyyyMMddHHmmss",
                                    CultureInfo.InvariantCulture);

                            DateTime end =
                                DateTime.ParseExact(
                                    to,
                                    "yyyyMMddHHmmss",
                                    CultureInfo.InvariantCulture);

                            string title =
                                GetElementValue(
                                    prog,
                                    "title");

                            lstPrograms.Items.Add(
                                start.ToString("HH:mm")
                                + " ～ "
                                + end.ToString("HH:mm")
                                + "  "
                                + title);

                            if (now >= start &&
                                now <= end)
                            {
                                ShowProgram(
                                    prog,
                                    stationName);
                            }
                        }
                    }
                }

                lblLoading.Visibility =
                    Visibility.Collapsed;
            }
            catch
            {
                lblLoading.Visibility =
                    Visibility.Collapsed;

                MessageBox.Show(
                    "radiko APIに接続できませんでした。");
            }
        }

        private void lstPrograms_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            int index =
                lstPrograms.SelectedIndex;

            if (index < 0)
            {
                return;
            }

            if (index >= currentPrograms.Count)
            {
                return;
            }

            string stationName =
                cmbFreq.SelectedItem.ToString();

            ShowProgram(
                currentPrograms[index],
                stationName);
        }

        private void ShowProgram(
            XElement prog,
            string stationName)
        {
            string ft =
                prog.Attribute("ft")?.Value ?? "";

            string to =
                prog.Attribute("to")?.Value ?? "";

            DateTime start =
                DateTime.ParseExact(
                    ft,
                    "yyyyMMddHHmmss",
                    CultureInfo.InvariantCulture);

            DateTime end =
                DateTime.ParseExact(
                    to,
                    "yyyyMMddHHmmss",
                    CultureInfo.InvariantCulture);

            lblTitle.Text =
                GetElementValue(
                    prog,
                    "title");

            lblStation.Text =
                stationName;

            lblTime.Text =
                start.ToString("HH:mm")
                + " ～ "
                + end.ToString("HH:mm");

            string desc =
                GetElementValue(
                    prog,
                    "desc");

            if (string.IsNullOrWhiteSpace(desc))
            {
                desc =
                    GetElementValue(
                        prog,
                        "info");
            }

            if (string.IsNullOrWhiteSpace(desc))
            {
                desc =
                    GetElementValue(
                        prog,
                        "pfm");
            }

            if (string.IsNullOrWhiteSpace(desc))
            {
                desc =
                    "番組詳細なし";
            }

            txtDesc.Text =
                CleanHtml(desc);

            lblStart.Text =
                start.ToString("HH:mm");

            lblEnd.Text =
                end.ToString("HH:mm");

            double total =
                (end - start).TotalSeconds;

            double current =
                (DateTime.Now - start).TotalSeconds;

            double percent =
                current / total * 100;

            if (percent < 0)
                percent = 0;

            if (percent > 100)
                percent = 100;

            progressBar.Value =
                percent;

            lblProgress.Text =
                Math.Round(percent)
                + "%";

            string img =
                GetElementValue(
                    prog,
                    "img");

            if (!string.IsNullOrEmpty(img))
            {
                try
                {
                    imgThumb.Source =
                        new BitmapImage(
                            new Uri(img));
                }
                catch
                {
                    imgThumb.Source = null;
                }
            }
        }

        private void btnPlay_Click(
            object sender,
            RoutedEventArgs e)
        {
            string area =
                cmbArea.SelectedItem.ToString();

            string stationName =
                cmbFreq.SelectedItem.ToString();

            string stationId =
                areaStations[area][stationName];

            string url =
                $"https://radiko.jp/#!/live/{stationId}";

            Process.Start(
                new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
        }

        private string GetElementValue(
            XElement parent,
            string name)
        {
            return parent.Element(name)?.Value ?? "";
        }

        private string CleanHtml(
            string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            html = Regex.Replace(
                html,
                @"<br\s*/?>",
                "\n",
                RegexOptions.IgnoreCase);

            html = Regex.Replace(
                html,
                @"</p>",
                "\n\n",
                RegexOptions.IgnoreCase);

            html = Regex.Replace(
                html,
                "<.*?>",
                "");

            html =
                WebUtility.HtmlDecode(html);

            return html.Trim();
        }
    }
}
