namespace SatrancMantigi
{
    // Olası hamle türlerini tanımlar.
    public enum HamleTuru
    {
        Normal, // Normal taş hareketi.
        RokSahKanadi, // Şah kanadı rok.
        RokVezirKanadi, // Vezir kanadı rok.
        CiftPiyon, // Piyonun iki kare ilerlemesi.
        EnPassant, // Geçerken alma hamlesi.
        PiyonTerfi // Piyonun başka bir taşa terfi etmesi.
    }
}