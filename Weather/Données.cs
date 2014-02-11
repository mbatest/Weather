using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
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
        #region Attributes
        string[] wind = new string[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSO", "SO", "OSO", "O", "ONO", "NO", "NNO", "---" };
        public long positionInFile;
        public byte[] dat = new byte[8];
        private float pressionAbsolue;
        private float pressionRelative;
        private float vitesseVent;
        private float vitesseRafale;
        private float pluviométrieTotale;
        private float pluviométrieRécente;
        private float températureIntérieure;
        private float humiditéIntérieure;
        private float humiditéExtérieure;
        private DateTime date;
        private int directWind;
        private float températureExtérieure;
        private byte[] dummy = new byte[4];
        private double db;
        #endregion
        #region Properties
        public float PressionAbsolue
        {
            get { return pressionAbsolue; }
            set { pressionAbsolue = value; }
        }
        public float PressionRelative
        {
            get { return pressionRelative; }
            set { pressionRelative = value; }
        }
        public float VitesseVent
        {
            get { return vitesseVent; }
            set { vitesseVent = value; }
        }
        public float VitesseRafale
        {
            get { return vitesseRafale * 3.6f; }
        }
        public float PluviométrieTotale
        {
            get { return pluviométrieTotale; }
            set { pluviométrieTotale = value; }
        }
        public float PluviométrieRécente
        {
            get { return pluviométrieRécente; }
            set { pluviométrieRécente = value; }
        }
        public float TempératureIntérieure
        {
            get { return températureIntérieure; }
            set { températureIntérieure = value; }
        }
        public float TempératureExtérieure
        {
            get { return températureExtérieure; }
            set { températureExtérieure = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public string chaineDate;
        public float HumiditéIntérieure
        {
            get { return humiditéIntérieure; }
            set { humiditéIntérieure = value; }
        }
        public float HumiditéExtérieure
        {
            get { return humiditéExtérieure; }
            set { humiditéExtérieure = value; }
        }
        public int DirectionWind
        {
            get { return directWind; }
            set { directWind = value; }
        }
        public string DirectionVent
        {
            get
            {
                return wind[directWind];
            }
        }
        #endregion
        public Data(FileStream fs)
        {
            positionInFile = fs.Position;
            fs.Read(dat, 0, dat.Length);
            pressionAbsolue = ReadFloat(fs);
            pressionRelative = ReadFloat(fs);
            vitesseVent = ReadFloat(fs);
            fs.Read(dummy, 0, dummy.Length);
            directWind = System.BitConverter.ToInt32(dummy, 0);
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
            fs.Read(dummy, 0, dummy.Length);

        }
        /// <summary>
        /// Converts double to TimeSpan
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private TimeSpan timeSpan(double db)
        {
            TimeSpan ts = new TimeSpan((long)(db * 10000000));
            return ts;
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
        }
    }

}
