using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    public class Pluie
    {
        private int mois;
        private int year;
        private float monthlyRain;

        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        public string Mois
        {
            get
            {
                string[] mo = new string[] { "", "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };
                return mo[mois];
            }
        }

        public float MonthlyRain
        {
            get { return monthlyRain; }
            set { monthlyRain = value; }
        }
        public Pluie(int mois, int year) { this.mois = mois; this.year = year; }
        public override string ToString()
        {
            return mois.ToString() + " " + monthlyRain.ToString();
        }
    }
}
