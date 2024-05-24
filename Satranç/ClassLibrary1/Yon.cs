using System.Reflection;

namespace SatrancMantigi
{
    public class Yon
    {
        #region Yönler
        public readonly static Yon Kuzey = new Yon(-1, 0);
        //Belirli bir konumdan yukarı doğru hareket etmek için kuzey yön ile başlarız bir satır çıkarırız ve sütunu değişmeden bırakırız. Bu yüzden alfa satırı -1 olmalı ve digerlide bu mantık ile ilerliyor
        public readonly static Yon Guney = new Yon(1, 0);
        public readonly static Yon Dogu = new Yon(0, 1);
        public readonly static Yon Bati = new Yon(0, -1);
        public readonly static Yon KuzeyDogu = Kuzey + Dogu;
        public readonly static Yon KuzeyBati = Kuzey + Bati;
        public readonly static Yon GuneyDogu = Guney + Dogu;
        public readonly static Yon GuneyBati = Guney + Bati;

        //Kuzey, Guney, Dogu, Bati: Bu değişkenler, önceden tanımlanmış Yon nesneleridir. readonly ve static anahtar sözcükleri ile tanımladık çünkü Bu, bu yönlerin yalnızca Yon sınıfı üzerinden erişilebilir ve değerleri değiştirilemez sabitler olduğunu belirtir.

        //KuzeyDogu, KuzeyBati, GuneyDogu, GuneyBati: Bu değişkenler de Yon nesneleridir ancak bunlar, yukarıdaki sabit yönleri kullanarak + operatörüyle hesaplanır. Örneğin: KuzeyDogu = Kuzey + Dogu.
        #endregion

        #region Yön_Özellikleri_Ve_Nesneleri
        public int SatirAlfa { get; }  //SatirAlfa: Satır konumunu temsil eden bir tamsayı.
        public int SutunAlfa { get; }  //SutunAlfa: Sütun konumunu temsil eden bir tamsayı.

        //Yon sınıfının kurucusu, iki parametre alır: satirAlfa ve sutunAlfa. Bu parametreler, yeni bir Yon nesnesi oluşturmak için kullanılır.
        public Yon(int satirAlfa, int sutunAlfa)
        {
            SatirAlfa = satirAlfa;
            SutunAlfa = sutunAlfa;
        }
        #endregion

        #region İki_Yönden_Yeni_Bir_Yön(+ ile)
        // + : İki Yon nesnesini toplar ve yeni bir Yon nesnesi döndürür.Yeni nesnenin satır konumu, ilk iki nesnenin satır konumlarının toplamıdır. Sütun konumu da aynı şekilde hesaplanır.
        public static Yon operator +(Yon yon1, Yon yon2)
        {
            return new Yon(yon1.SatirAlfa + yon2.SatirAlfa, yon1.SutunAlfa + yon2.SutunAlfa);
        }
        #endregion

        # region İki_Yönden_Yeni_Bir_Yön(* ile)  
        // * : Bir Yon nesnesini bir tamsayı ile çarpar ve yeni bir Yon nesnesi döndürür.Yeni nesnenin satır konumu, ilk nesnenin satır konumunun tamsayı ile çarpımıdır.Sütun konumu da aynı şekilde hesaplanır.
        public static Yon operator *(int skaler, Yon yon)
        {
            return new Yon(skaler * yon.SatirAlfa, skaler * yon.SutunAlfa);
        }
        #endregion

        #region Operatörlerle_Örnek_Yön_Oluşturma
        /*
        
        Örnek kod
        Yon yon1 = new Yon(2, 3);
        Yon yon2 = new Yon(4, 5);

        Yon toplamYon = yon1 + yon2; // SatirAlfa: 6, SutunAlfa: 8

        Yon carpimYon = yon1 * 3; // SatirAlfa: 6, SutunAlfa: 9

        */
        #endregion
    }
}
