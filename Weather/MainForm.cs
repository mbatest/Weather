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
    public partial class MainForm : Form
    {
        string basePath = @"E:\Mes Documents\HeavyWeather\History\";
        public SortedList<string, Data> données = new SortedList<string, Data>();
        public SortedList<DateTime, Data> donnéesDate = new SortedList<DateTime, Data>();
        public List<Data> dats = new List<Data>();
        public List<Pluie> pluviométrie = new List<Pluie>();
        public MainForm()
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
                    string key = (d.Date.ToShortDateString() + " " + d.Date.ToShortTimeString()).Replace(".", "/");
                    données.Add(key, d);
                    donnéesDate.Add(d.Date, d);
                    i++;
                }
                sr.Close();
                foreach (Data d in donnéesDate.Values)
                    dats.Add(d);
                int j = 6;
                int mois = dats[j].Date.Month;
                int year = dats[j].Date.Year;
                Pluie p = new Pluie(mois, year);
                while (j < dats.Count)
                {
                    int current = dats[j].Date.Month;
                    year = dats[j].Date.Year;
                    while (current == mois)
                    {
                        p.MonthlyRain += dats[j].PluviométrieRécente;
                        j++;
                        if (j == dats.Count)
                            break;
                        current = dats[j].Date.Month;
                    }
                    p.MonthlyRain /= 10;
                    pluviométrie.Add(p);
                    mois = current;
                    p = new Pluie(mois, year);
                }
                i = 0;
                foreach (Data d in dats)
                {
                    ListViewItem lw = new ListViewItem(i.ToString());
                    listView1.Items.Add(lw);
                    lw.SubItems.Add(d.Date.ToShortDateString() + " " + d.Date.ToShortTimeString());
                    lw.SubItems.Add(d.PressionRelative.ToString("f2"));
                    lw.SubItems.Add(d.TempératureExtérieure.ToString("f2"));
                    if (d.HumiditéExtérieure < 100)
                        lw.SubItems.Add(d.HumiditéExtérieure.ToString("f2"));
                    else
                        lw.SubItems.Add("---");

                    if (d.DirectionWind != 16)
                        lw.SubItems.Add((d.VitesseRafale).ToString("f2"));
                    else
                        lw.SubItems.Add("---");
                    i++;
                }
                foreach (Pluie pl in pluviométrie)
                {
                    ListViewItem lw = new ListViewItem(pl.Mois + " " + pl.Year.ToString());
                    lw.SubItems.Add(pl.MonthlyRain.ToString("f2"));
                    listPluie.Items.Add(lw);
                }
            }

        }


     }
}
