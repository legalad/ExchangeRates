using System;
using System.Collections.Generic;

namespace KursyWalut
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TabelaKursówWalutNBP tabela =
                PomocnikNbp.PobierzAktualnąTabeleKursówNBP();
                string s = "Tabela kursów walut";
                s += "\n\nNumer tabeli: " + tabela.NumerTabeli;
                s += "\nData notowania: " + tabela.DataNotowania.ToLongDateString();
                s += "\nData publikacji: " + tabela.DataPublikacji.ToLongDateString();
                foreach (KeyValuePair<string, KursyWalutNBP> pozycja in tabela.Pozycje)
                {
                    s += "\n" + pozycja.Value.ToString();
                }
                Console.WriteLine(s);
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(
                $"Błąd podczas pobierania kursów walut NBP: {exc.Message}");
            }
        }
    }
}
