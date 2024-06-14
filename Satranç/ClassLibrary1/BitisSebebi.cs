namespace SatrancMantigi
{
    // Oyunun bitiş sebeplerini temsil eden enum.
    public enum BitisSebebi
    {
        SahMat, // Şah mat ile oyun bitişi.
        Pat, // Pat ile oyun bitişi.
        ElliHamleKurali, // 50 hamle kuralı ile oyun bitişi.
        YetersizTas, // Yetersiz taş ile oyun bitişi.
        UcKatliTekrar, // Üç katlı tekrar ile oyun bitişi.
        SureDoldu // Süre dolması ile oyun bitişi
    }
}