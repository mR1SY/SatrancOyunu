namespace SatrancMantigi
{
    //Enum: bir grup sabit değeri temsil eden bir veri türüdür. Genellikle birbiriyle ilişkili olan ve birbiriyle değiştirilebilen değerleri tanımlamak için kullanılır. Oyuncu numaralandırılması yapacağımız için bunu kullanıyoruz. (Örneğin: Beyaz:1 Siyah:2 Boş:0)

    // Bunu hangi oyuncuları temsil ettiğini ve oyunu kimin kazandığını saklamak için kullanacağız ama ek olarak bunu satranç taşlarının rengini temsil etmek için de kullanacağız.

    //Boş koymamızın bize sağladığı kolaylık şudur: Beraberlik durumunda sıfıra endeksleyebiliriz.
    public enum Oyuncu
    {
        Bos,     
        Beyaz,   
        Siyah 
    }

    #region Sıra_Değişimi(Ana_kısım)
    //Bu kısımda mevcut oyuncu oynadıktan sonra sıranın bir diğer oyuncuya geçtiğini belirtiyoruz.
    public static class OyuncuUzantıları
    {
        public static Oyuncu Rakip(this Oyuncu oyuncu)
        {
            return oyuncu switch
            {
                Oyuncu.Beyaz => Oyuncu.Siyah,
                Oyuncu.Siyah => Oyuncu.Beyaz,
                _ => Oyuncu.Bos,
            };
        }
    }
    #endregion
}
