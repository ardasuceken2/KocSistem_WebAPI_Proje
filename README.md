# TASK API - KoçSistem

Selamlar. Bu proje, stajyerlik sürecimde adým adým geliþtirdiðim, kod kalitesine ve mimari yapýya odaklandýðým bir Task API uygulamasýdýr. Projeyi geliþtirirken sadece kodun çalýþmasýna deðil, clean kod olmasýna ve benden sonra gelecek kiþi için detaylandýrýlmýþ açýklama metinleri barýndýrmasýna özen gösterdim.

## Hangi Mimariyi Kullandým?

Projeyi tek bir yýðýn halinde yazmak yerine Clean Architecture mantýðýyla 4 ayrý parçaya böldüm:
- **Domain:** Projenin kalbi. Veritabaný tablolarýmýzýn C# karþýlýklarý burada.
- **Application:** Ýþ kurallarýný yazdýðýmýz ve DTO'larýmýzý tuttuðumuz yer.
- **Infrastructure:** Veritabaný baðlantýlarýný ve altyapý iþlerini hallettiðimiz katman.
- **API:** Dýþarýya açýlan kapýmýz. Ýstekleri Controller'lar üzerinden burada karþýlýyoruz.

## Kullanýlan Teknolojiler

- **C# & .NET 8:** Temel dil ve iskelet.
- **JWT (JSON Web Token):** Sisteme güvenli giriþ (Login) ve yetki kontrolü için.
- **Swagger:** Yazdýðýmýz API'yi arayüz üzerinden kolayca test edebilmek için.

## Kod Yazarken Dikkat Ettiðim Standartlar

- **Ýsimlendirme (Naming):** Projedeki tüm sýnýflar, deðiþkenler ve klasörler evrensel kurallara uygun olarak tamamen Ýngilizce isimlendirildi.
- **Açýklamalar (Summary):** Yazdýðým metotlarýn tam olarak ne iþ yaptýðýný belirtmek için `/// <summary>` etiketleriyle Türkçe notlar düþtüm.
- **Düzen (Region):** Kod sayfalarýnda yüzlerce satýr içinde kaybolmamak için, kodlarý `#region` ve `#endregion` etiketleriyle mantýksal bloklara ayýrdým.
- **Gelecek Planlarý (TODO):** Geliþtirme yaparken þu an geçici olarak kullandýðým ama ileride gerçek veritabanýna baðlanacak kýsýmlara `// TODO:` notlarý býrakarak ileriye dönük notlar hazýrladým.

## Proje Nasýl Çalýþtýrýlýr?

Projeyi kendi bilgisayarýnda ayaða kaldýrmak istersen adýmlar çok basit:

1. Bu repoyu bilgisayarýna indir.
2. Klasördeki `TaskApi.sln` dosyasýna çift týklayarak projeyi Visual Studio ile aç.
3. **Paketleri Yükle:** Solution Explorer'da en üstteki Solution'a sað týklayýp *Restore NuGet Packages* seçeneðine týkla. Bu sayede projenin ihtiyaç duyduðu JWT ve veritabaný eklentileri (8.0.0) otomatik olarak indirilecektir.
4. Saðdaki Solution Explorer'dan ana proje olan **TaskApi** üzerine sað týkla ve *Set as Startup Project* seçeneðini iþaretle.
5. Üstteki yeþil baþlat butonuna bas veya `F5` tuþunu kullan.
6. Tarayýcýnda otomatik olarak Swagger arayüzü açýlacak. Tüm API isteklerini (GET, POST, DELETE) buradan test edebilirsin.

> **NOT:** Henüz gerçek veritabaný baðlanmadýðý için sistemi test etmek adýna kayýt (Register) olabilirsin. Eðer tam yetki (Admin) testleri yapmak istersen Login ekranýnda `Nick: Admin`, `Password: 123456` bilgileriyle giriþ yaparak full yetki alabilirsin.