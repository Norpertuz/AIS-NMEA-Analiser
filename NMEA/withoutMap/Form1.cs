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

namespace lab7gps
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
            listBox1.SelectionMode = SelectionMode.None;
            listBox2.SelectionMode = SelectionMode.None;
            listBox3.SelectionMode = SelectionMode.None;
            listBox4.SelectionMode = SelectionMode.None;
            listBox5.SelectionMode = SelectionMode.None;
            
        }

        private void button1_Click(object sender, EventArgs e)

        {
            button1.Enabled = false;
            int[] sv=new int [12];
            FileStream fs = new FileStream("dane.txt", FileMode.Open, FileAccess.Read);
          
            try
            {
                StreamReader sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                  


                    String kom = sr.ReadLine();
                    string[] cz = kom.Split(new char[] { ',', '*' });
                    int ile_kom = cz.Count();
                  
                   
                    if (suma_kontrolna(kom))
                    {
                        label5.ForeColor = Color.Green;
                        label5.Text = "Komunikaty prawidlowe";
                        if (cz[0] == "$GPRMC")
                        {
                            rmc_listbox.Items.Add(cz[1][0] + "" + cz[1][1] + ":" + cz[1][2] + "" + cz[1][3] + ":" + cz[1][4] + "" + cz[1][5] + "" + cz[1][6] + "" + cz[1][7] + "" + cz[1][8] + "" + cz[1][9]);
                            rmc_listbox.Items.Add("Valid");
                            String minutes_string = "0" + "" + cz[3][4] + "" + cz[3][5] + "" + cz[3][6] + "" + cz[3][7] + "" + cz[3][8];
                            float minutes = float.Parse(minutes_string) * 60; //konwersja minut ' 0.XXXX na sekundy ''
                            rmc_listbox.Items.Add(cz[3][0] + "" + cz[3][1] + "°" + cz[3][2] + "" + cz[3][3] + "'" + minutes.ToString() + "''" + cz[4]);
                            minutes_string = "0" + "" + cz[5][5] + "" + cz[5][6] + "" + cz[5][7] + "" + cz[5][8] + "" + cz[5][9];
                            minutes = float.Parse(minutes_string) * 60; //konwersja minut ' 0.XXXX na sekundy ''
                            rmc_listbox.Items.Add(cz[5][1] + "" + cz[5][2] + "°" + cz[5][3] + "" + cz[5][4] + "'" + minutes.ToString() + "''" + cz[6]);
                            rmc_listbox.Items.Add(cz[7]);
                            rmc_listbox.Items.Add(cz[8]);
                            rmc_listbox.Items.Add(cz[9][0] + "" + cz[9][1] + "/" + cz[9][2] + "" + cz[9][3] + "/" + cz[9][4] + "" + cz[9][5]);
                        }

                        else if (cz[0] == "$GPGGA")
                        {
                            gga_listbox.Items.Add(cz[1][0] + "" + cz[1][1] + ":" + cz[1][2] + "" + cz[1][3] + ":" + cz[1][4] + "" + cz[1][5] + "" + cz[1][6] + "" + cz[1][7] + "" + cz[1][8] + "" + cz[1][9]);
                            String minutes_string = "0" + "" + cz[2][4] + "" + cz[2][5] + "" + cz[2][6] + "" + cz[2][7] + "" + cz[2][8];
                            float minutes = float.Parse(minutes_string) * 60; //konwersja minut ' 0.XXXX na sekundy ''
                            gga_listbox.Items.Add(cz[2][0] + "" + cz[2][1] + "°" + cz[2][2] + "" + cz[2][3] + "'" + minutes.ToString() + "''" + cz[3]);
                            minutes_string = "0" + "" + cz[4][5] + "" + cz[4][6] + "" + cz[4][7] + "" + cz[4][8] + "" + cz[4][9];
                            minutes = float.Parse(minutes_string) * 60; //konwersja minut ' 0.XXXX na sekundy ''
                            gga_listbox.Items.Add(cz[4][1] + "" + cz[4][2] + "°" + cz[4][3] + "" + cz[4][4] + "'" + minutes.ToString() + "''" + cz[5]);
                            gga_listbox.Items.Add("SPS fix");
                            gga_listbox.Items.Add(cz[7]);
                            gga_listbox.Items.Add(cz[8]);
                            gga_listbox.Items.Add(cz[9] + "" + cz[10]);
                            gga_listbox.Items.Add(cz[11] + "" + cz[12]);

                        }
                        else if (cz[0] == "$GPGSA")
                        {
                            gsa_listbox.Items.Add("Auto");  //wartosc A
                            gsa_listbox.Items.Add("3D fix");//wartosc 3
                            for (int k = 3; k < ile_kom - 1; k++)
                            {
                                gsa_listbox.Items.Add(cz[k]);
                                
                                //Jezeli kolejna wartosc sv nie jest pusta to ja wrzucam do tablicy
                                //Wartosci sv moze byc max 12
                                if (cz[k] != ""&&((k-3)<12))
                                {
                                    sv[k - 3] = Int32.Parse(cz[k]);
                                    chart1.Series["Series1"].Points.AddY(sv[k-3]);
                                   // Console.WriteLine(sv[k - 3]);
                                }
                                else { Console.WriteLine("pusty"); }
                            }
                 
                        }
                        else if (cz[0] == "$GPVTG")
                        {

                            for (int k = 1; k < ile_kom - 2; k = k + 2)
                            {
                                vtg_listbox.Items.Add((cz[k] + cz[k + 1]).ToString());
                            }
                            if (cz[9] == "A") vtg_listbox.Items.Add("Auto");
                        }
                        else if (cz[0] == "$GPGSV")
                        {
                            int msg_num = 0;
                            ListBox [] aktualny = { gsv_listbox1, gsv_listbox2, gsv_listbox3 };
                            while (msg_num < 3) {
                              
                            for (int k = 1; k < ile_kom - 1; k++)
                            {
                                    if(msg_num+1 == Int32.Parse(cz[2]))
                                    aktualny[msg_num].Items.Add(cz[k]);
                            }

                                msg_num++;
                            }
                        }
                    }
                    else
                    {
                        label5.Text = "Błąd sumy kontrolnej";
                    }
                }
                sr.Close();
            }
            catch (Exception ko)
            {
                Console.WriteLine(ko.ToString());
            }




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

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
