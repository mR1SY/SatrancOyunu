using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancUI
{
    // Oyuncuların menüde yapabileceği seçenekleri tanımlar.
    public enum Secenek
    {
        YenidenBaslat, // Oyunu yeniden başlatma seçeneği.
        Cikis, // Oyundan çıkış seçeneği.
        DevamEt, // Oyunu devam ettirme seçeneği.
        AnaMenu // Ana menüye dönme seçeneği.
    }
}