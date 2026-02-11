# SmartController - IoT Cihaz Yönetim Sistemi

Otoyıkama istasyonlarında kullanılan IoT kontrollü su & köpük cihazlarının merkezi yönetim sistemi.

## Proje Yapısı

```
SmartController.sln
├── SmartController.Api      → REST API (Cihazların konuştuğu)
├── SmartController.Web      → MVC Yönetim Paneli
├── SmartController.Shared   → DTO / Enum / Ortak modeller
└── SmartController.Db       → EF Core / Migration katmanı
```

## Gereksinimler

- .NET 8 SDK
- PostgreSQL

## Docker ile Çalıştırma

```bash
docker-compose up --build
```

Servisler:
- Web UI: http://localhost:5001
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- PostgreSQL: localhost:5432

Migration ve seed otomatik çalışır.

## Manuel Kurulum

1. PostgreSQL'de veritabanı oluşturun:
```sql
CREATE DATABASE smartcontroller;
```

2. Migration uygulayın:
```bash
dotnet ef database update --project SmartController.Db --startup-project SmartController.Api
```

3. Çalıştırın:
```bash
dotnet run --project SmartController.Api
dotnet run --project SmartController.Web
```

## API Endpoints

### Cihaz Veri Gönderimi
- `POST /esp/data/{customerSuffix}` - Ham veri gönderimi
- `POST /device/status/{customerSuffix}` - Durum bilgisi

### Komut Sistemi
- `GET /commands/{customerSuffix}?peron_id=1&sube=1` - Bekleyen komut sorgula
- `POST /commands/{customerSuffix}/ack` - Komut alındı bildirimi

### Firmware
- `GET /check_firmware?customer=xxx&device=yyy&current_version=2.1.1`

### Yönetim API
- `GET /api/devices` - Cihaz listesi
- `GET /api/devices/{id}` - Cihaz detay
- `GET /api/devices/{id}/counters` - Sayaçlar
- `GET /api/devices/{id}/logs` - Durum logları
- `POST /api/commands/send` - Komut gönder
- `GET /api/dashboard` - Dashboard verileri
