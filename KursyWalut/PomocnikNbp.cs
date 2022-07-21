using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;

namespace KursyWalut
{
    public static class PomocnikNbp
    {
        private static KursyWalutNBP parsujPozycjęTabeliKursówWalutNBP(XElement elementPozycja,
                                                                       IFormatProvider formatProvider)
        {
            KursyWalutNBP pozycja = new KursyWalutNBP();
            pozycja.NazwaWaluty = elementPozycja.Element("nazwa_waluty").Value;
            pozycja.Przelicznik = double.Parse(elementPozycja.Element("przelicznik").Value);
            pozycja.KodWaluty = elementPozycja.Element("kod_waluty").Value;
            pozycja.KursKupna = decimal.Parse(elementPozycja.Element("kurs_kupna").Value);
            pozycja.KursSprzedaży = decimal.Parse(elementPozycja.Element("kurs_sprzedazy").Value);

            return pozycja;
        }

        private static string pobierzPlik(string adres)
        {
            using HttpClient klientSieciowy = new HttpClient();
            using Stream strumień = klientSieciowy.GetStreamAsync(adres).Result;
            return new StreamReader(strumień).ReadToEnd();
        }

        public static TabelaKursówWalutNBP PobierzAktualnąTabeleKursówNBP()
        {
            string adres = "https://www.nbp.pl/kursy/xml/LastC.xml";
            string zawartośćPlikuXml = pobierzPlik(adres);
            XDocument xml = XDocument.Parse(zawartośćPlikuXml);

            IFormatProvider formatProvider = new CultureInfo("pl");

            TabelaKursówWalutNBP tabela = new TabelaKursówWalutNBP();
            tabela.NumerTabeli = xml.Root.Element("numer_tabeli").Value;
            tabela.DataNotowania = DateTime.Parse(xml.Root.Element("data_notowania").Value,
                                                                   formatProvider);
            tabela.DataPublikacji = DateTime.Parse(xml.Root.Element("data_publikacji").Value,
                                                                    formatProvider);
            tabela.Pozycje = new Dictionary<string, KursyWalutNBP>();
            foreach (XElement elementPozycja in xml.Root.Elements("pozycja"))
            {
                KursyWalutNBP pozycja = parsujPozycjęTabeliKursówWalutNBP(elementPozycja, formatProvider);
                tabela.Pozycje.Add(pozycja.KodWaluty, pozycja);
            }
            return tabela;
        }
    }
    public struct KursyWalutNBP
    {
        public string NazwaWaluty;
        public string KodWaluty;
        public double Przelicznik;
        public decimal KursKupna;
        public decimal KursSprzedaży;

        public string ToString(IFormatProvider formatProvider)
        {
            return KodWaluty + " " + KursKupna.ToString(formatProvider) + " - " + KursSprzedaży.ToString(formatProvider);
        }

        public override string ToString()
        {
            return ToString(new CultureInfo("pl"));
        }
    }
    public struct TabelaKursówWalutNBP
    {
        public string NumerTabeli;
        public DateTime DataNotowania;
        public DateTime DataPublikacji;
        public Dictionary<string, KursyWalutNBP> Pozycje;
    }
}
