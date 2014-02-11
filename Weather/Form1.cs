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

namespace Weather
{
    public partial class Form1 : Form
    {
        string basePath = @"E:\Mes Documents\HeavyWeather\History\";
        public SortedList<string, Data> données = new SortedList<string, Data>();
        public SortedList<DateTime, Data> donnéesDate = new SortedList<DateTime, Data>();
        public List<Data> dats = new List<Data>();
        public List<Pluie> pluviométrie = new List<Pluie>();
        public Form1()
        {
            InitializeComponent();
            listView1.Columns.Add("Numéro");
            listView1.Columns.Add("Date", 100);
            listView1.Columns.Add("Pression", 50, HorizontalAlignment.Right);
            listView1.Columns.Add("Température", 50, HorizontalAlignment.Right);
            listView1.Columns.Add("Humidité", 50, HorizontalAlignment.Right);
            listView1.Columns.Add("Rafales", 50, HorizontalAlignment.Right);
        }
        private void ouvrirToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.InitialDirectory = basePath;
            opf.Filter = "History (*.dat)|*.dat";
            if (opf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream sr = new FileStream(opf.FileName, FileMode.Open);
                int i = 0;
                while (sr.Position < sr.Length)
                {
                    Data d = new Data(sr);
                    string key = (d.date.ToShortDateString() + " " + d.date.ToShortTimeString()).Replace(".", "/");
                    données.Add(key, d);
                    donnéesDate.Add(d.date, d);
                    i++;
                }
                sr.Close();
                foreach (Data d in donnéesDate.Values)
                    dats.Add(d);
                int j = 6;
                int mois = dats[j].date.Month;
                Pluie p = new Pluie(mois);

                while (j < dats.Count)
                {
                    int current = dats[j].date.Month;
                    while (current == mois)
                    {
                        p.haut += dats[j].pluviométrieRécente;
                        j++;
                        if (j == dats.Count)
                            break;
                        current = dats[j].date.Month;
                    }
                    p.haut /= 10;
                    pluviométrie.Add(p);
                    mois = current;
                    p = new Pluie(mois);
                }
                i = 0;
                foreach (Data d in dats)
                {
                    ListViewItem lw = new ListViewItem(i.ToString());
                    listView1.Items.Add(lw);
                    lw.SubItems.Add(d.date.ToShortDateString() + " " + d.date.ToShortTimeString());
                    lw.SubItems.Add(d.pressionRelative.ToString("f2"));
                    lw.SubItems.Add(d.températureExtérieure.ToString("f2"));
                    if (d.humiditéExtérieure < 100)
                        lw.SubItems.Add(d.humiditéExtérieure.ToString("f2"));
                    else
                        lw.SubItems.Add("---");

                    if (d.directWind != 16)
                        lw.SubItems.Add((d.VitesseRafale).ToString("f2"));
                    else
                        lw.SubItems.Add("---");

                    i++;
                }
                foreach (Pluie pl in pluviométrie)
                {
                    ListViewItem lw = new ListViewItem(pl.Mois);
                    lw.SubItems.Add(pl.haut.ToString("f2"));
                    listPluie.Items.Add(lw);
                }
            }

        }


     }

    public class Pluie
    {

        private int mois;

        public string Mois
        {
            get
            {
                string[] mo = new string[] { "", "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };
                return mo[mois]; }
        }
        public float haut;
        public Pluie(int mois) { this.mois = mois; }
        public override string ToString()
        {
            return mois.ToString() + " " + haut.ToString();
        }
    }
    public class Data
    {
        /*Each row of data is stored in 56 byte chunks starting from the beginning of the file (no header).

 ROW
 OFFSET Type Name Unit
------ --------- ---------------- -----
00 Double [8] Timestamp days from 12/30/1899 00:00:00 (GMT) 
 08 Float [4] Abs Pressure hectopascals (millibars)
 12 Float [4] Relative Pressure hectopascals (millibars)
 16 Float [4] Wind Speed meters/second
 20 ULong [4] Wind Direction see below
 24 Float [4] Wind Gust meters/second
 28 Float [4] Total Rainfall millimeters
 32 Float [4] New Rainfall millimeters
 36 Float [4] Indoor Temp celsius
 40 Float [4] Outdoor Temp celsius
 44 Float [4] Indoor Humidity %
 48 Float [4] Outdoor Humidity %
 52 ULong [4] unknown - (Value is always 0)
 
         */
        /*Temp_Rosee:=
      (237.7*((17.27*Temp)/(237.7+Temp)+ln(Hum)))/(17.27-((17.27*Temp)/(237.7+Temp)+ln(Hum))); 
      (où Temp est la température extérieure en °C et Hum le taux d'humidité extérieure (attention entre 0 et 1))
      Pour la Température Ressentie le calcul est :
      Temp_Ress:= 13.12+0.6215*Temp+(0.3965*Temp-11.37)*(Vent^(0.16));
      (où Temp est la Température extérieure en °C et Vent la vitesse du vent en km/h))*/
        string[] wind = new string[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSO", "SO", "OSO", "O", "ONO", "NO", "NNO", "---" };
        public long position;
        public byte[] dat = new byte[8];
        public float pressionAbsolue ;
        public float pressionRelative ;
        public float vitesseVent ;
        public int directWind ;
        private float vitesseRafale;
        public float VitesseRafale
        {
            get { return vitesseRafale * 3.6f; }
        }
        public float pluviométrieTotale ;
        public float pluviométrieRécente ;
        public float températureIntérieure ;
        public float températureExtérieure ;
        public float humiditéIntérieure ;
        public float humiditéExtérieure ;
        public byte[] w = new byte[4];
        public double db;
        public DateTime date;
        public string chaineDate;
        public string DirectionVent
        {
            get
            {
                return wind[directWind];
            }
        }
        public Data(FileStream fs)
        {
            position = fs.Position;
            fs.Read(dat, 0, dat.Length);
            pressionAbsolue = ReadFloat(fs);
            pressionRelative = ReadFloat(fs);
            vitesseVent = ReadFloat(fs);
            fs.Read(w, 0, w.Length);
            directWind = System.BitConverter.ToInt32(w, 0);
            vitesseRafale = ReadFloat(fs); 
            pluviométrieTotale = ReadFloat(fs);
            pluviométrieRécente = ReadFloat(fs);
            températureIntérieure = ReadFloat(fs);
            températureExtérieure = ReadFloat(fs);
            humiditéIntérieure = ReadFloat(fs);
            humiditéExtérieure = ReadFloat(fs);
            db = System.BitConverter.ToDouble(dat, 0);// *86400 * 10000000;
            //           DateTime startDate = new DateTime(1900, 1,1, 0, 0, 0);
            DateTime startDate = new DateTime(1899, 12, 30, 0, 0, 0);
            TimeSpan ts = timeSpan(db * 86400);
            date = startDate + ts;
            fs.Read(w, 0, w.Length);

        }
        private TimeSpan timeSpan(double db)
        {
            TimeSpan ts = new TimeSpan((long)(db * 10000000));
            return ts;

        }

        public Data(string line)
        {
            line = line.Replace("\"", "");
            string[] data = line.Split(';');
            chaineDate = data[1];
        }
        float ReadFloat(FileStream fs)
        {
            byte[] w = new byte[4];
            fs.Read(w, 0, w.Length);
            return System.BitConverter.ToSingle(w, 0);

        }
        public override string ToString()
        {
            return date.ToShortDateString() + " " + date.ToShortTimeString();
            //+" " + dat[7].ToString("x") + dat[6].ToString("x") + dat[5].ToString("x") + " " + dat[4].ToString("x") + dat[3].ToString("x") + dat[2].ToString("x") + dat[1].ToString("x") + dat[0].ToString("x");
        }
    }

}
