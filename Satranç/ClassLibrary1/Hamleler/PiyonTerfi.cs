using SatrancMantigi.Taslar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class PiyonTerfi : Hamle // Piyonun terfi etmesini sağlayan hamle sınıfı.
    {
        #region Özellikler
        public override HamleTuru Tur => HamleTuru.PiyonTerfi; // Hamle türünü PiyonTerfi olarak tanımlar.
        public override Pozisyon FromPos { get; } // Hamlenin başlangıç pozisyonunu tutar.
        public override Pozisyon ToPos { get; } // Hamlenin bitiş pozisyonunu tutar.

        private readonly TasTuru yeniTur;//Piyonun terfi ettileceği taş türü // Piyonun terfi edeceği taş türünü tutar.
        #endregion

        #region Yapıcı Metod
        public PiyonTerfi(Pozisyon fromPos, Pozisyon to, TasTuru yeniTur) // PiyonTerfi nesnesini başlangıç pozisyonu, bitiş pozisyonu ve yeni taş türüyle oluşturan yapıcı metod.

        {
            FromPos = fromPos; // Başlangıç pozisyonunu fromPos parametresinden alır.
            ToPos = to; // Bitiş pozisyonunu to parametresinden alır.
            this.yeniTur = yeniTur; // Yeni taş türünü yeniTur parametresinden alır.
        }
        #endregion

        #region Terfi Edilecek Taşı Oluşturan Metod
        private Tas TerfiTasiOlusturma(Oyuncu renk) // Terfi edilecek taşı oluşturan metod.
        {
            return yeniTur switch // Yeni taş türüne göre yeni bir taş nesnesi oluşturur ve döndürür.
            {
                TasTuru.At => new At(renk), // At
                TasTuru.Fil => new Fil(renk), // Fil
                TasTuru.Kale => new Kale(renk), // Kale
                _ => new Vezir(renk) // Varsayılan olarak Vezir
            };
        }
        #endregion

        #region Piyon Terfi Yürütme Metodu
        public override bool Execute(Tahta tahta) // PiyonTerfi hamlesini tahta üzerinde uygulayan metod.
        {
            Tas piyon = tahta[FromPos]; // Başlangıç pozisyonundaki piyonu alır.
            tahta[FromPos] = null; // Piyonu başlangıç pozisyonundan kaldırır.

            Tas terfiTasi = TerfiTasiOlusturma(piyon.Renk); // Yeni taş türüne göre terfi taşını oluşturur.
            terfiTasi.Tasindi = true; // Terfi taşının hareket ettiğini işaretler.

            TasSilindi = !tahta.BosMu(ToPos); // Bitiş pozisyonunda taş varsa TasSilindi özelliğini true olarak ayarlar.

            tahta[ToPos] = terfiTasi; // Terfi taşını bitiş pozisyonuna yerleştirir.

            return true; // Hamlenin başarılı olduğunu belirtir.
        }
        #endregion
    }
}