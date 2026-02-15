namespace SmartController.Shared.Enums;

public enum CommandType
{
    AP,     // Request All Parameter - Tüm parametreleri iste
    WE,     // WDT Enable - ESP WDT Yetkili
    WD,     // WDT Disable - ESP WDT Yetkisiz
    AT,     // All Total - Tüm Pals ve Ödeme Toplamlarını Gönder
    CT      // Clear Total - Gün Sonu / Sıfırlama
}
