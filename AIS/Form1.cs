using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.ToolTips;
using GMap.NET.MapProviders;

namespace ais13
{
    public partial class Form1 : Form
    {

        List<string> m1, m5a, m5b, mmsiList, m5List, m5IDs, areaList, areas, cities;
        List<double> longitudes, latitudes, cogs, toses;
        List<Placemark> areaData;
        Graphics drawMap;
        GMapOverlay overlay, markers;







        public Form1()
        {
        

            InitializeComponent();
            gMap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMap.MapScaleInfoEnabled = false;
            gMap.Position = new GMap.NET.PointLatLng(53.4284043, 14.5668269);
            gMap.ShowCenter = true;
            gMap.MaxZoom = 24;
            gMap.MinZoom = 0;
            gMap.Zoom = 12;
            gMap.AutoScroll = true;
            drawMap = Graphics.FromImage(gMap.ToImage());
            gMap.DragButton = MouseButtons.Left;

            /*
            GMapMarker markert;
            barwa = new Markery();
            PointLatLng pointt = new PointLatLng(53.4284043, 14.5668269);
            markert = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(pointt, barwa.brazowy);
            markert.ToolTipText = "Testowy";
            markers.Markers.Add(markert);

            gMap.Overlays.Add(markers);
            gMap.Refresh();
            */
         
        }

        public class Markery
        {

            public Bitmap zaznaczony = (Bitmap)Image.FromFile("img/zaznaczony.png");
            public Bitmap purpurowy = (Bitmap)Image.FromFile("img/purpurowy.png");
            public Bitmap niebieski = (Bitmap)Image.FromFile("img/niebieski.png");
            public Bitmap czerwony = (Bitmap)Image.FromFile("img/czerwony.png");
            public Bitmap pomaranczowy = (Bitmap)Image.FromFile("img/pomaranczowy.png");
            public Bitmap niebfiol = (Bitmap)Image.FromFile("img/niebiesko-fioletowy.png");
            public Bitmap rozowy = (Bitmap)Image.FromFile("img/rozowy.png");
            public Bitmap brazowy = (Bitmap)Image.FromFile("img/brazowy.png");
            public Bitmap zolty = (Bitmap)Image.FromFile("img/zolty.png");
            public Bitmap szary = (Bitmap)Image.FromFile("img/szary.png");
            public Bitmap bialy = (Bitmap)Image.FromFile("img/bialy.png");
        }

        Markery barwa;


        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            
            FileStream fs = new FileStream("ais.txt", FileMode.Open, FileAccess.Read);

            

            using (StreamReader reader = new StreamReader(fs))
            {
                var zawartosc = reader.ReadToEnd();
                komunikaty.Text = zawartosc;
                string[] all_messages = zawartosc.Split();
                all_messages = all_messages.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                m1 = new List<string>();
                m5a = new List<string>();
                m5b = new List<string>();
                for (int i = 0; i < all_messages.Count(); i++)
                {
                    if (all_messages[i].StartsWith("!AIVDM,1")) { m1.Add(all_messages[i]); }
                    if (all_messages[i].StartsWith("!AIVDM,2,1") && all_messages[i + 1].StartsWith("!AIVDM,2,2"))
                    {
                        m5a.Add(all_messages[i]);
                        m5b.Add(all_messages[i + 1]);
                    }
                }
            }

            
        }

        public void GetM5(List<string> m5_part1, List<string> m5_part2)
        {
            for (int i = 0; i < m5_part1.Count(); i++)
            {
                string msg = m5_part1[i].Substring(15, m5_part1[i].Count() - 20) + m5_part2[i].Substring(15, m5_part2[i].Count() - 20);
                m5List.Add(msg);
            }
        }

        public string GetIDs(string msg)
        {
            string temp = Dekoduj(msg);
            string value = String.Empty;
            for (int i = 8; i < 38; i++) value += temp[i];
            int id = Convert.ToInt32(BinToDec(value));
            return Convert.ToString(id);
        }

        public double GetTOS(string msg)
        {
            string bin = Dekoduj(msg);
            string typeofship = String.Empty;
            for (int i = 232; i < 240; i++) typeofship += bin[i];
            double tos = Convert.ToDouble(BinToDec(typeofship));
            return tos;
        }

        public string getMMSI(string data)
        {
            string msg = data.Substring(14, data.Count() - 19);
            string temp = Dekoduj(msg);
            string value = String.Empty;
            for (int i = 8; i < 37; i++) value += temp[i];
            int mmsiv = Convert.ToInt32(BinToDec(value)) * 2;
            return Convert.ToString(mmsiv);
        }

        public bool isClicked;
        double ivalue;

        GMapMarker oldmarker;
        private void gMap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            ivalue = item.Position.Lat;
            PointLatLng oldPosition = new PointLatLng(item.Position.Lat, item.Position.Lng);
            string id_data = item.ToolTipText.Substring(6);
            for (int x = 0; x < m5IDs.Count(); x++)
            {
                if (id_data == m5IDs[x])
                {
                    if (toses[x] == 70)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.purpurowy);
                        oldmarker = marker;
                    }
                    if (toses[x] == 52)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.niebieski);
                        oldmarker = marker;
                    }
                    if (toses[x] == 30)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.czerwony);
                        oldmarker = marker;
                    }
                    if (toses[x] == 79)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.pomaranczowy);
                        oldmarker = marker;
                    }
                    if (toses[x] == 80)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.niebfiol);
                        oldmarker = marker;
                    }
                    if (toses[x] == 84)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.rozowy);
                        oldmarker = marker;
                    }
                    if (toses[x] == 90)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.brazowy);
                        oldmarker = marker;
                    }
                    if (toses[x] == 83)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.zolty);
                        oldmarker = marker;
                    }
                    if (toses[x] == 60)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.szary);
                        oldmarker = marker;
                    }
                    if (toses[x] == 31)
                    {
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.bialy);
                        oldmarker = marker;
                    }
                }
            }
            lat_statku.Text = Convert.ToString(item.Position.Lat);
            lon_statku.Text = Convert.ToString(item.Position.Lng);
            GMapMarker isActive = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(oldPosition, barwa.zaznaczony);
            if (isClicked)
            {
                markers.Markers.Add(isActive);
                markers.Markers.Remove(oldmarker);
                isClicked = false;
            }
            else
            {
                markers.Markers.Add(oldmarker);
                markers.Markers.Remove(isActive);
                isClicked = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            gMap.Zoom = 14;
            latitudes = new List<double>();
            longitudes = new List<double>();
            m5List = new List<string>();
            m5IDs = new List<string>();
            areaList = new List<string>();
            areas = new List<string>();
            areaData = new List<Placemark>();
            mmsiList = new List<string>();
            cogs = new List<double>();
            cities = new List<string>();
            toses = new List<double>();
            foreach (string value in m1)
            {
                if (suma_kontrolna(value))
                {
                    Console.WriteLine(m1[6]);
                    Console.WriteLine(m5b[6]);
                    Console.WriteLine(m5a[6]);
                    Console.WriteLine(value);
                    latitudes.Add(getLatitude(value));
                    longitudes.Add(getLongitude(value));
                    mmsiList.Add(getMMSI(value));
                    cogs.Add(getCog(value));
                }
            }
            foreach (string userid in mmsiList) listBox2.Items.Add( "MMSI: " + userid + "\n");

            GetM5(m5a, m5b);

            foreach (string message in m5List)
            {
                m5IDs.Add(GetIDs(message));
                toses.Add(GetTOS(message));
                areaList.Add(getArea(message));
            }

            markers = new GMapOverlay("Markers");
            barwa = new Markery();
            GMapMarker marker;
            for (int j = 0; j < longitudes.Count(); j++)
            {
                PointLatLng point = new PointLatLng(latitudes[j], longitudes[j]);
                for (int k = 0; k < m5IDs.Count(); k++)
                {
                    if (mmsiList[j] == m5IDs[k])
                    {
                        if (toses[k] == 70)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.purpurowy);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 52)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.niebieski);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 30)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.czerwony);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 79)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.pomaranczowy);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 80)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.niebfiol);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 84)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.rozowy);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 90)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.brazowy);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 83)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.zolty);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 60)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.bialy);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                        if (toses[k] == 31)
                        {
                            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(point, barwa.szary);
                            marker.ToolTipText = $"MMSI: { mmsiList[j] }";
                            markers.Markers.Add(marker);
                        }
                    }
                }
                overlay = markers;
            }
            gMap.Overlays.Add(markers);
            gMap.Refresh();

           

            // Znajdowanie akwenów na podstawie komunikatów AIS
            areas = areaList.Distinct().ToList();
            List<string> trash = new List<string>();
            foreach (string area in areas)
            {
                string temp = area.Replace('@', ' ');
                cities.Add(temp);
                if (area.StartsWith("TEL") || area.StartsWith("TEL")) trash.Add(area);
            }

            foreach (string x in trash) cities.Remove(x);
            cities = cities.Distinct().ToList();
            cities.ForEach(s => s = s.Replace(" ", ""));
            for (int i = 0; i < cities.Count(); i++) { comboBox1.Items.Add(cities[i]); }
            comboBox1.SelectedItem = cities[0];
        }
        
        public string konwersja_6bit(string symbol)
        {
            string ascii = Convert.ToByte(Convert.ToChar(symbol)).ToString();
            int dec = Convert.ToInt32(ascii);

            if (dec < 48) { Console.WriteLine("Data Error Recovery"); }
            else
            {
                if (dec > 199) { Console.WriteLine("Data Error Recovery"); }
                else
                {
                    if (dec > 87)
                    {
                        if (dec < 96) { Console.WriteLine("Data Error Recovery"); }
                        else dec = dec + 40;
                    }
                    else dec = dec + 40;
                }
            }

            if (dec > 128) dec = dec + 32;
            else dec = dec + 40;

            string temp = Convert.ToString(dec, 2);
            return temp.Substring(temp.Length - 6);
        }


        public string Dekoduj(string kkom)
        {
            
             
            int length = kkom.Count();
  
            List<string> bity = new List<string>();
            List<char> chars = new List<char>();

            for (int i = 0; i < length; i++)
            {
                bity.Add(konwersja_6bit(Convert.ToString(kkom[i])));
            }

            for (int i = 0; i < bity.Count(); i++)
            {
                char[] temp = bity[i].ToCharArray();
                for (int j = 0; j < temp.Count(); j++)
                {
                    chars.Add(temp[j]);
                }
            }
            string wyjscie = "";

            for (int m = 0; m < chars.Count(); m++) wyjscie += chars[m];
            return wyjscie;

          
        }

        public double getLatitude(string data)
        {
            string msg = data.Substring(14, data.Count() - 19);
            string temp = Dekoduj(msg);
            string value = String.Empty;
            for (int i = 89; i < 115; i++)
                value += temp[i];
            double lat = Convert.ToDouble(BinToDec(value)) / 600000;
            return lat * 2;
        }

        public double getLongitude(string data)
        {
            string msg = data.Substring(14, data.Count() - 19);
            string temp = Dekoduj(msg);
            string value = String.Empty;
            for (int i = 61; i < 88; i++)
                value += temp[i];
            double lon = Convert.ToDouble(BinToDec(value)) / 600000;
            return lon * 2;
        }

        public string getArea(string msg)
        {
            string binary = Dekoduj(msg);
            string binary_6 = String.Empty;
            for (int i = 302; i < 421; i++) binary_6 += binary[i];

            string[] area_info = Podziel(binary_6, 6).Split(new char[] { ' ' });
            string area = KonwersjaNaText(area_info);
            return area;
        }

        public double getCog(string data)
        {
            string msg = data.Substring(14, data.Count() - 19);
            string temp = Dekoduj(msg);
            string value = String.Empty;
            for (int i = 116; i < 127; i++)
                value += temp[i];
            double cog = Convert.ToDouble(BinToDec(value)) / 10;
            return cog * 2;
        }







        public string KonwersjaNaText(string[] przekaz)
        {

            char[] znaki = { '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', ' ', '!', '”', '#', '$', '%', '&', '`', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?' };
            string[] reprezentacja_binarna = { "000000", "000001", "000010", "000011", "000100", "000101", "000110", "000111", "001000", "001001", "001010", "001011", "001100", "001101", "001110", "001111", "010000", "010001", "010010", "010011", "010100", "010101", "010110", "010111", "011000", "011001", "011010", "011011", "011100", "011101", "011110", "011111", "100000", "100001", "100010", "100011", "100100", "100101", "100110", "100111", "101000", "101001", "101010", "101011", "101100", "101101", "101110", "101111", "110000", "110001", "110010", "110011", "110100", "110101", "110110", "110111", "111000", "111001", "111010", "111011", "111100", "111101", "111110", "111111" };
            List<char> dane = new List<char>();
            int dlugosc = 0;
            for (int i = 0; i < przekaz.Count(); i++)
            {
                for (int j = 0; j < reprezentacja_binarna.Count(); j++)
                    if (przekaz[i] == reprezentacja_binarna[j])
                    {
                        dane.Add(znaki[j]);
                        dlugosc++;
                        break;
                    }
            }
            string text = String.Empty;
            for (int k = 0; k < dlugosc; k++) { text = text + dane[k]; }
            return text;
        }
        public string Podziel(string text, int liczba_bitow) { return string.Join(string.Empty, text.Select((x, i) => i > 0 && i % liczba_bitow == 0 ? string.Format(" {0}", x) : x.ToString())); }

        public string BinToDec(string bin)
        {
            return Convert.ToInt32(bin, 2).ToString();
        }

        public bool suma_kontrolna(string nap)
        {
            bool ok = false;
            int dl = nap.Count();
            string nap1 = nap.Substring(1, dl - 4);
            string nap2 = nap.Substring(dl - 2, 2);
            int sk = Convert.ToInt32(nap2, 16);
            int dl2 = nap1.Count();
            byte z = 0;
            for (int k = 0; k < dl2; k++)
            {
                byte zn = Convert.ToByte(nap1[k]);
                z ^= zn;
            }
            if (z == sk)
            {
                ok = true;
            }
            return ok;
        }
        //wybrana sytuacja
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            if (komunikaty.Text == String.Empty || (lat_statku.Text == String.Empty && lon_statku.Text == String.Empty))
            {
                MessageBox.Show("Prosze wybrac marker", "Blad", MessageBoxButtons.OK);
            }
            else
            {
                int indeks = latitudes.FindIndex(a => a == ivalue);
                string cog = Convert.ToString(cogs[indeks]) + "°";
                
                MessageBox.Show($"Latitude: { lat_statku.Text } \nLongitude: { lon_statku.Text } \nCOG: { cog } ", "Sytuacja", MessageBoxButtons.OK);
            }
        }
    }
}
