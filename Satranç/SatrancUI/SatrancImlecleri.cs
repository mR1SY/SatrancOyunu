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
    public static class SatrancImlecleri
    {
        #region İmleç_Tanımlamaları

        public static readonly Cursor BeyazImlec = ImlecYukle("Assets/BeyazIsaretci.cur");
        public static readonly Cursor SiyahImlec = ImlecYukle("Assets/SiyahIsaretci.cur");
        private static Cursor ImlecYukle(string dosyaYolu)
        {
            Stream stream = Application.GetResourceStream(new Uri(dosyaYolu, UriKind.Relative)).Stream;
            return new Cursor(stream, true);
        }
        #endregion
    }
}
