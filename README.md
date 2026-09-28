# TASK API - KoçSistem

Selamlar. Bu proje, stajyerlik sürecimde adým adým geliþtirdiðim, kod kalitesine ve mimari yapýya odaklandýðým bir Task API ve React arayüzü uygulamasýdýr. Projeyi geliþtirirken sadece kodun çalýþmasýna deðil, clean kod olmasýna ve benden sonra gelecek kiþi için detaylandýrýlmýþ açýklama metinleri barýndýrmasýna özen gösterdim.

## Hangi Mimariyi Kullandým?

Projeyi tek bir yýðýn halinde yazmak yerine Clean Architecture mantýðýyla 4 ayrý parçaya böldüm:
- **Domain:** Projenin kalbi. Veritabaný tablolarýmýzýn C# karþýlýklarý burada.
- **Application:** Ýþ kurallarýný yazdýðýmýz ve DTO'larýmýzý tuttuðumuz yer.
- **Infrastructure:** Veritabaný baðlantýlarýný ve altyapý iþlerini hallettiðimiz katman.
- **API:** Dýþarýya açýlan kapýmýz. Ýstekleri Controller'lar üzerinden burada karþýlýyoruz.

## Kullanýlan Teknolojiler

- **C# & .NET 8:** Temel dil ve iskelet.
- **Entity Framework Core:** Code-First yaklaþýmý ile SQL veritabaný yönetimi ve tablo iliþkileri için.
- **JWT (JSON Web Token):** Sisteme güvenli giriþ (Login), rol bazlý yetkilendirme (Admin/User) ve oturum kontrolü için.
- **Swagger:** Yazdýðýmýz API'yi arayüz üzerinden kolayca test edebilmek için.
- **React & Vanilla JS:** Kurumsal proxy kýsýtlamalarýný aþmak adýna npm kullanýlmadan, CDN üzerinden entegre edilen dinamik ön yüz (Frontend) için.

## Kod Yazarken Dikkat Ettiðim Standartlar

- **Ýsimlendirme (Naming):** Projedeki tüm sýnýflar, deðiþkenler ve klasörler evrensel kurallara uygun olarak tamamen Ýngilizce isimlendirildi.
- **Açýklamalar (Summary):** Yazdýðým metotlarýn tam olarak ne iþ yaptýðýný belirtmek için `/// <summary>` etiketleriyle Türkçe notlar düþtüm.
- **Düzen (Region):** Kod sayfalarýnda yüzlerce satýr içinde kaybolmamak için, kodlarý `#region` ve `#endregion` etiketleriyle mantýksal bloklara ayýrdým.
- **Gelecek Planlarý (TODO):** Geliþtirme sürecinde refactor edilecek veya geliþtirilecek alanlara `// TODO:` notlarý býrakarak ileriye dönük izler býraktým.

## Proje Nasýl Çalýþtýrýlýr?

Projeyi kendi bilgisayarýnda ayaða kaldýrmak istersen adýmlar çok basit:

### 1. Arka Uç (API) Kurulumu
1. Bu repoyu bilgisayarýna indir.
2. Klasördeki `TaskApi.sln` dosyasýna çift týklayarak projeyi Visual Studio ile aç.
3. **Paketleri Yükle:** Solution Explorer'da en üstteki Solution'a sað týklayýp *Restore NuGet Packages* seçeneðine týkla. Bu sayede projenin ihtiyaç duyduðu EF Core, JWT ve diðer kütüphaneler otomatik olarak indirilecektir.
4. **Veritabanýný Oluþtur:** Visual Studio üst menüsünden *Tools > NuGet Package Manager > Package Manager Console* yolunu izle. Açýlan konsolda üstteki "Default project" kýsmýný `TaskApi.Infrastructure` olarak seç. Ardýndan konsola `Update-Database` yazýp Enter'a bas. (Bu iþlem SQL tablolarýný fiziksel olarak oluþturacaktýr).
5. Saðdaki Solution Explorer'dan ana proje olan **TaskApi** üzerine sað týkla ve *Set as Startup Project* seçeneðini iþaretle.
6. Üstteki yeþil baþlat butonuna bas veya `F5` tuþunu kullan. API ayaða kalkacak ve tarayýcýda Swagger açýlacaktýr.

### 2. Ön Uç (React) Kurulumu
1. Reponun içindeki Frontend klasörünü (WebTask) Visual Studio Code ile aç.
2. VS Code eklentilerinden **Live Server** eklentisini kur (Ritwick Dey tarafýndan geliþtirilen).
3. `app.js` dosyasýnýn en üst satýrýnda bulunan `API_BASE` deðiþkenindeki port numarasýnýn, az önce Visual Studio'da açýlan API portu ile ayný olduðundan emin ol (Örn: `https://localhost:7142/api`).
4. `index.html` dosyasýna sað týklayýp *Open with Live Server* seçeneðine týkla. Arayüz tarayýcýda açýlacaktýr.

> **NOT:** Sistem Entity Framework üzerinden SQL Server'a (LocalDB) baðlanmýþtýr. Veritabaný ilk oluþtuðunda (Seed Data sayesinde) testleri tam yetkiyle yapabilmeniz için sisteme varsayýlan bir Admin atanýr. Login ekranýnda `Kullanýcý Adý: Admin`, `Þifre: 123456` bilgileriyle giriþ yaparak kullanýcý silme ve görev ekleme gibi full yetkileri test edebilirsin.