using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SatrancUI
{
    // Satranç oyununda kullanılacak imleçleri tanımlar.
    public static class SatrancImlecleri
    {
        public static readonly Cursor BeyazImlec = ImlecYukle("Assets/BeyazIsaretci.cur"); // Beyaz oyuncu için imleç.
        public static readonly Cursor SiyahImlec = ImlecYukle("Assets/SiyahIsaretci.cur"); // Siyah oyuncu için imleç.

        private static Cursor ImlecYukle(string dosyaYolu) // Verilen dosya yolundan bir Cursor nesnesi oluşturur.
        {
            Stream stream = Application.GetResourceStream(new Uri(dosyaYolu, UriKind.Relative)).Stream; // Dosyayı açar.
            return new Cursor(stream, true); // Cursor nesnesi oluşturur ve döndürür.
        }
    }
}