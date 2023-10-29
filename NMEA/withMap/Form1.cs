
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
using System.Windows.Forms.DataVisualization.Charting;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace lab8mapa
{
    public partial class Form1 : Form
    {
        Graphics gmap_g;
        GMapOverlay markersOverlay = new GMapOverlay("markers");
        GMapOverlay routesOverlay = new GMapOverlay("routes");


        long ile_gga = 0;
        double sr_lo = 0;
        double sr_la = 0;
        double suma_lo = 0, suma_la = 0;
      

        public Form1()
        {
            InitializeComponent();
            chart1.Titles.Add(new Title("Pozycja geograficzna"));
            chart1.Titles[0].BackColor = Color.GreenYellow;
            chart1.Titles[0].BorderColor = Color.Green;
            chart1.ChartAreas[0].AxisY.Minimum = 53.37975 ;
            gMap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMap.MapScaleInfoEnabled = true;
            gMap.Position = new PointLatLng(+53,+14);
            gMap.ShowCenter = true;
            gMap.MinZoom = 0;
            gMap.MaxZoom = 24;
            gMap.Zoom = 14;
            gMap.AutoScroll = true;
            gmap_g = Graphics.FromImage(gMap.ToImage());
            gMap.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            FileStream fs = new FileStream("dane.txt", FileMode.Open, FileAccess.Read);
            double lo=0, la=0;
            List<PointLatLng> points = new List<PointLatLng>();
            try
            {
                StreamReader sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {

                    // listBox1.Items.Add(sr.ReadLine());

                    String kom = sr.ReadLine();
                    string[] cz = kom.Split(new char[] { ',', '*' });


                    if (suma_kontrolna(kom))
                    {
                        label1.Text = "Komunikaty prawidlowe";
                        if (cz[0] == "$GPGGA")
                        {

                            listBox1.Items.Add(kom);
                            ile_gga++;
                            lo = konwersja_DDM_do_DD(Double.Parse(cz[4]));
                            la = konwersja_DDM_do_DD(Double.Parse(cz[2]));
                            chart1.Series["Series1"].Points.AddXY(lo, la);
                            points.Add(new PointLatLng(la, lo));
                            suma_lo = suma_lo + Double.Parse(cz[2]);
                            suma_la = suma_la + Double.Parse(cz[4]);
                            listBox2.Items.Add(lo + " " +la);

                        }



                    }
                    else
                    {
                        label1.Text = "Błąd sumy kontrolnej";
                    }
                }
                sr.Close();


            }
            catch (Exception ko)
            {
                Console.WriteLine(ko.ToString());
            }
            //obliczanie wartosci srednich
            sr_la = suma_la / ile_gga; 
            sr_lo = suma_lo / ile_gga;
      
            //konwersja wartosci srednich z DDM do DD
            sr_la = konwersja_DDM_do_DD(sr_la);
            sr_lo = konwersja_DDM_do_DD(sr_lo);


            String ile_gga_s = "Liczba kom = " + "" + ile_gga.ToString();
            String sr_la_s = "Sr lat = " + "" + sr_la.ToString();
            String sr_lo_s = "Sr lon = " + "" + sr_lo.ToString();

            listBox2.Items.Insert(0, ile_gga_s);
            listBox2.Items.Insert(1, sr_la_s);
            listBox2.Items.Insert(2, sr_lo_s);
           
            GMapRoute route = new GMapRoute(points,"Dane");
            route.Stroke = new Pen(Color.Red, 3);
            routesOverlay.Routes.Add(route);
            gMap.Overlays.Add(routesOverlay);
            
            GMap.NET.WindowsForms.GMapMarker marker =new GMap.NET.WindowsForms.Markers.GMarkerGoogle(points[0],GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);
            markersOverlay.Markers.Add(marker);
            gMap.Overlays.Add(markersOverlay);
            gMap.Position = points[0];
            gMap.ShowCenter = true;
            gMap.Zoom=20;
            gMap.Invalidate();
            gMap.Refresh();
        }

        //funckja konwertujaca DDM do DD
        public double konwersja_DDM_do_DD(double konwertowana){
            double przekonwertowana=0;
            przekonwertowana = konwertowana / 100; //by uzyskac degree nalezy podzielic przez 100. Dwie pierwsze cyfry sa degree
            int przekonwertowana_D = Convert.ToInt32(przekonwertowana); // wyciagniecie degree i pozostawienie minut 0,XXXX
            przekonwertowana = (przekonwertowana - przekonwertowana_D) * 100;  //uzyskanie minut 
            przekonwertowana = przekonwertowana / 60; // minuty dzielimy na 60
            przekonwertowana = przekonwertowana + przekonwertowana_D; // do uzyskanego wyniku dodajemy degree
          // Console.WriteLine(przekonwertowana);
            return przekonwertowana;
        }
        //funckja sprawdzajaca poprawnosc sumy kontrolnej
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



    }
}
