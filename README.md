Akıllı Belge Arşivleme Sistemi

Akıllı Belge Arşivleme Sistemi; fiziksel veya dijital belge görüntülerini işleyerek belge alanını tespit eden, OCR teknolojisi ile metinleri okuyan, belge içerisindeki bilgileri analiz eden ve belgeleri aranabilir şekilde dijital ortamda arşivleyen C# tabanlı bir masaüstü uygulamasıdır.

Proje özellikle belediye ve benzeri kurumlarda bulunan çok sayıdaki belgenin dijital ortama aktarılması ve daha kolay yönetilebilmesi amacıyla geliştirilmiştir.

Projenin Amacı:

Sistemin temel amacı belge arşivleme sürecini mümkün olduğunca otomatik hale getirmektir.
Kullanıcı tarafından sisteme yüklenen bir belge üzerinde sırasıyla:
1. Görüntü işleme
2. Belge alanının tespit edilmesi
3. OCR ile metinlerin okunması
4. OCR sonucunun temizlenmesi
5. Belge bilgilerinin analiz edilmesi
6. Kullanıcı tarafından bilgilerin kontrol edilmesi
7. Belgenin arşivlenmesi
8. Bilgilerin SQLite veritabanına kaydedilmesi
işlemleri gerçekleştirilmektedir.



Sistem Akışı:
Belge / Görsel
Görüntü İşleme
YOLO ile Belge Tespiti
Görüntü İyileştirme
OCR
OCR Metin Temizleme
Belge Analizi
Kullanıcı Kontrolü
Kayıt

 ↙     ↘

Dosya     SQLite

Arşivi    Veritabanı

           ↓

           Arama

-YOLO ile Belge Tespiti:

Görüntü içerisindeki belge alanının otomatik olarak tespit edilmesi için YOLO tabanlı bir nesne tespit modeli kullanılmıştır.

İlk çalışmalarda Roboflow üzerinden hazır bir veri seti kullanılmaya çalışılmıştır. Ancak hazır veri setinin proje kapsamında kullanılan belge görüntülerinde yeterli sonuç vermemesi üzerine projeye özel bir veri seti hazırlanmıştır.

Hazırlanan görüntüler etiketlenmiş ve veri seti Google Colab ortamında düzenlenerek YOLO modeli eğitilmiştir.

Eğitilen model ONNX formatına dönüştürülmüş ve C# uygulamasına entegre edilmiştir.

Model:
AI/best.onnx
dosyası içerisinde bulunmaktadır.

YOLO modelinin uygulamadaki kullanımından YoloService sorumludur.

Modelin belge bulamadığı durumlarda işlemin tamamen durmasını önlemek amacıyla görüntünün tamamını belge kabul eden bir fallback mekanizması da bulunmaktadır.

-OCR Sistemi

Belge içerisindeki yazıların okunması için OCR teknolojilerinden yararlanılmıştır.

İlk aşamada Tesseract OCR üzerinde çalışmalar yapılmış, daha sonra OCR performansını geliştirmek amacıyla PaddleOCR entegrasyonu üzerinde çalışılmıştır.

OCR sonucunda elde edilen ham metin doğrudan kullanılmamaktadır.

OcrPostProcessor, sınıfı ile gereksiz boşluklar, satır problemleri ve bazı OCR kaynaklı hatalar temizlenerek metin belge analiz aşamasına gönderilmektedir.

-Görüntü İşleme

OCR doğruluğunu artırmak amacıyla belge görüntülerine OCR öncesinde çeşitli görüntü işleme işlemleri uygulanmaktadır.

Projede bu işlemler için başlıca:

DocumentEnhancer

ImageEnhancementService

PerspectiveCorrection

PerspectiveCorrectionV2

ImageEditor
sınıfları kullanılmaktadır.

Özellikle düşük çözünürlüklü görüntüler OCR işleminden önce büyütülmektedir.

Belgenin perspektifinin bozuk olması durumunda perspektif düzeltme yöntemlerinden yararlanılmaktadır.

-Belge Analizi

OCR sonucunda yalnızca metnin okunması yeterli görülmemiştir. Okunan metinden belge hakkında anlamlı bilgilerin çıkarılması hedeflenmiştir.

Bu amaçla:
DocumentParser
DocumentAnalyzer
DocumentAnalysisResult
yapıları geliştirilmiştir.

Sistem OCR metninden mümkün olduğunca aşağıdaki bilgileri çıkarmaya çalışmaktadır:
Belge Adı
Belge Türü
Kişi / Kurum
Açıklama
OCR Metni
 
Otomatik olarak bulunan bilgiler kayıt işleminden önce kullanıcı tarafından kontrol edilebilir ve değiştirilebilir.

-Veritabanı

Projede veritabanı olarak SQLite kullanılmaktadır.

SQLite tercih edilmesinin temel nedenleri:
Ayrı bir veritabanı sunucusu gerektirmemesi
Masaüstü uygulamaları için kolay kullanılabilmesi
Veritabanının tek bir dosyada saklanabilmesi
Kurulum ve taşınabilirlik açısından pratik olması

Veritabanının oluşturulmasından DatabaseInitializer, temel veritabanı işlemlerinden ise DatabaseService sorumludur.

-Kaydedilen Belge Bilgileri

Belgeler için aşağıdaki bilgiler saklanmaktadır:
ID
Belge Adı
Belge Türü
Kişi / Kurum
Açıklama
Dosya Yolu
OCR Metni
Kayıt Tarihi

Belge dosyaları arşiv klasöründe saklanırken ilgili belge bilgileri SQLite veritabanında tutulmaktadır.

-Arşiv ve Arama

Kaydedilen belgelerin daha sonra bulunabilmesi için bir arama ekranı geliştirilmiştir.

Belgeler;
Belge adı
Belge türü
Kişi / kurum
Tarih
gibi kriterlere göre aranabilmektedir.

Arama sonuçları DataGridView üzerinde görüntülenmektedir.
Seçilen belgeler açılabilir, güncellenebilir veya arşivden silinebilir.


\*\*\*\*\*\*

Kullanılan Teknolojiler:

C#
.NET
Windows Forms
SQLite
OpenCV
YOLO
ONNX
PaddleOCR
Tesseract OCR
Roboflow
Google Colab

\*\*\*\*\*

Proje Yapısı:

BelgeArsivlemeSistemi

│

├── AI

│   ├── YoloService

│   ├── ImageEnhancementService

│   └── best.onnx

│

├── Database

│   ├── DatabaseInitializer

│   └── DatabaseService

│

├── Forms

│   ├── Form1

│   ├── FormAna

│   └── FormArama

│

├── ImageProcessing

│   ├── DocumentAnalyzer

│   ├── DocumentEnhancer

│   ├── ImageEditor

│   ├── PerspectiveCorrection

│   └── PerspectiveCorrectionV2

│

├── Models

│   ├── DocumentAnalysisResult

│   └── DocumentRecord

│

├── OCR

│   ├── DocumentParser

│   ├── OcrPostProcessor

│   └── OcrTextAnalyzer

│

└── Services

   └── PaddleOcrService
Projenin temel belge işleme ve arşivleme altyapısı geliştirilmiştir.
Sistem genel olarak:

Belge → YOLO → Görüntü İşleme → OCR → Belge Analizi → Kullanıcı Kontrolü → SQLite → Arşiv → Arama
akışı üzerinden çalışmaktadır.

Proje geliştirme ve test sürecindedir.

