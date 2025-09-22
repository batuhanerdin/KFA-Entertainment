
Varsayımlar, Açıklamalar ve Notlar

-Kullanılan oyun motoru: unity 2020.3.62f1

Oynanış Kontrolleri

W / A / S / D → Oyuncu hareketi

E → Etkileşim (market açma, obeliskten sıradaki dalgayı çağırma vb.)

P → Sıradaki dalgayı çağırma kısayolu

ESC → Menü panelini açma/kapama

Oyun Akışı
-----

Oyun başladığında 1. dalga (Wave 1) başlar.

Düşmanlar portaldan doğar ve obeliske(korumamız gereken stun) doğru yürür.

Düşmanlar belli bir olasılıkla oyuncuya menzilli (projectile) ya da yakın dövüş (melee) saldırısı yapabilir.

Oyuncu düşmanlara yaklaştığında menziline göre otomatik saldırır.

ölen düşmanlar para bırakır ve ekranın sol üstündeki öldürme sayacını arttırır.

Düşmanlar obeliske ulaşırsa, obeliskin canı azalır.

Dalga tamamlandığında (tüm düşmanlar öldüğünde ya da obeliske girdiklerinde):

Haritanın soluna çitlerin olduğu yere bir merchant cart (pazar arabası) gelir.

Oyuncu, düşmanlardan düşen paralarla güçlendirmeler satın alabilir.

Marketin altındaki obeliskte E tuşuna basarak veya P kısayolunu kullanarak yeni dalgayı başlatabilir.

Yeni dalga başladığında tekrar düşmanlar doğar ve obeliske doğru yürürler.

5. 10. ve 15. dalgalarda golem bossu gelir diğer düşmanlarla beraber saldırırlar.

Bu döngü bu şekilde oyun boyunca devam eder.


Mimari Tercihler

Wave Sistemi (WaveConfig – Scriptable Object):
Düşman dalgaları WaveConfig isimli Scriptable Object’lerle tanımlanıyor. Bu sayede her dalgadaki düşman türleri, sayıları ve çıkış hızları Unity Inspector üzerinden kolayca düzenlenebiliyor. Dalga akışı WaveManager tarafından yönetiliyor.

Ses Yönetimi (AudioManager & MusicManager):

Tüm SFX (vurulma, ölüm, coin alma, market açma vs.) tek bir AudioManager üzerinden çağrılıyor. Bu merkezi yapı sayesinde ses seviyeleri ayrı ayrı kontrol edilebiliyor.

Arka plan müzikleri ise MusicManager tarafından yönetiliyor. Chill/Wave müzikleri arasında fade in – fade out geçişleri bulunuyor.
Bu iki sistem, oyunda hem net bir kontrol hem de kolay genişletilebilirlik sağlıyor.

Player Saldırı Sistemi (AttackSystem + Animation Events + VFX):
Oyuncunun saldırıları AttackSystem tarafından yönetiliyor.

Saldırı anında animator üzerinden Animation Event tetikleniyor ve hitbox aktif oluyor.

Vuruş anında VFX (parçacık efektleri) spawn ediliyor ve AudioManager ile özel saldırı sesi çalınıyor.
Böylece oyuncuya tatmin edici bir “vuruş hissiyatı” kazandırıldı.

Stat Sistemi (StatSystem):
Oyuncunun saldırı gücü, hız, menzil, can gibi tüm özellikleri StatSystem üzerinden kontrol ediliyor.
Marketten alınan upgrade’ler doğrudan bu sistemdeki metodlarla (IncreaseAttackPower, IncreaseMoveSpeed, IncreaseAttackRange, IncreaseMaxHealth) uygulanıyor.
Böylece upgrade’ler merkezi bir yerden kolayca yönetilebiliyor.

Kodların Modülerliği:

EnemyAttack tek bir script ama melee ve ranged olarak iki farklı saldırıyı yönetebiliyor. Inspector’dan tick ile seçilebilmesi designer dostu.

StatSystem upgrade metodlarıyla tamamen merkezi → yeni bir upgrade eklemek sadece bir fonksiyon eklemek kadar kolay.

Obelisk Koruma Mekaniği (ObeliskManager):
Oyuncunun koruması gereken bir Obelisk var. Düşmanlar obeliske ulaştığında ObeliskManager devreye giriyor, canı azaltıyor ve UI güncelleniyor. Bu sayede oyuna bir “koruma” boyutu ekleniyor.

Düşman Hareket Sistemi (PathFollower):
Düşmanlar, PathNode’lar ile tanımlanan yollardan PathFollower scripti sayesinde ilerliyor. Rastgele spread offset sayesinde düşmanlar tek sıra yerine hafif dağılarak daha doğal bir hareket yapıyor. Ayrıca stun ve saldırı gibi durumlarda hareket durdurulabiliyor.

Düşman Saldırı Sistemi (EnemyAttack):
Düşmanlar hem projectile saldırısı hem de melee dash saldırısı yapabiliyor.

Saldırı gerçekleşmeden önce animasyon tetikleniyor.

Projectile’ler Rigidbody ile hareket ediyor, çarpınca yok olup VFX oynatıyor.

Melee saldırılarında dash sonrası OverlapSphere ile menzil içindeki oyuncuya hasar veriliyor.
Bu yapı sayesinde farklı saldırı tipleri tek bir script üzerinden kontrol edilebiliyor.

Coin ve Market Sistemi (CoinManager, ObjectDropper, MerchantCartManager):

Ölen düşmanlar ObjectDropper sayesinde coin ve partikül efektleri bırakıyor. Coin pickup’ları CoinManager tarafından yönetiliyor ve UI anında güncelleniyor.

Wave’ler bittikten sonra MerchantCartManager devreye giriyor. Market animasyonla sahneye giriyor, AudioManager üzerinden ses efektleri çalıyor ve oyuncuya güçlendirmeler sunuyor.

UI Yönetimi (MainMenuUI, KillCounterUI, CoinUI, HealthUI):

UI’lar event system tabanlı çalışıyor. Örneğin, HealthSystem can değişince OnHealthChanged event’i fırlatıyor, HealthUI anında güncelliyor.

KillCounterUI yine eventlerle çalışıyor, düşman öldükçe otomatik artıyor.

Menü sistemi (MainMenuUI) sahneler arası kaybolmuyor, SceneManagerEx üzerinden sahneler async load ile yükleniyor bu sayede sahne yüklenirken oyun kasmıyor.

Enemy ölürken hem animasyon, hem ses, hem coin + partikül drop var. Üçlü geri bildirim sağlanıyor.

Player’a Özel Invincibility Frame ve Stun:

Player hasar aldığında can azalması + stun + iframe + ses + yanıp sönme var. Oyuncu hasar aldığını çok net hissediyor.

Player hasar aldığında sesefekti + kısa süreli stun alıyor (hareket ve saldırı disable ediliyor).

I-Frame süresince hasar almıyor, sprite yanıp sönüyor.

Bu efekt hem görsel hem de oynanış açısından net bir geri bildirim veriyor.