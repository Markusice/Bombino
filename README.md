# Bombino, a bomberman játék


## Ismertető
A játék célja egy bomberman típusú játék megvalósítása, ahol a játékosok egymás ellen játszva próbálnak egyedüliként életben maradni a pályán. A játékosok bombákat rakhatnak le, amellyel megölhetik egymást, de a pályán lévő dobozokat és szörnyeket is felrobbanthatják velük. A dobozokból különböző bónuszok kerülhetnek elő, a szörnyek képesek a játékosokat elkapni, megölni. A játék körökre van osztva, az nyer, aki eléri az előre megadott nyert játszmák számát.

A játék Godot-ban lett fejlesztve, és Windows 10/11 operációs rendszeren lett tesztelve.

## Megvalósított feladatok
- Alapjáték [2 komplexitás]
- Három játékos [1 komplexitás]
- Perzisztencia [0.5 komplexitás] - (nem lett teljesen implementálva)
- Testreszabható vezérlés [0.5 komplexitás]
- Folyamatos mozgás [0.5 komplexitás]
- 3D-s grafika [1 komplexitás] (eredetileg 2.5D-s grafika 0.5 komplexitással)

## A játék pontokban
* **A főmenüben elérhető funkciók**
  * új játék indítása
  * mentett játék betöltése
  * mentett játék törlése
  * játékból való kilépés
* **Új játék indítása során szükséges**
  * 2 vagy 3 játékos mód kiválasztása
  * játéktérkép megadása a 3 megadott közül
  * játékkörök (játszmák) megadása
* **A játék közben**
  * a játékosok (2 vagy 3) ugyanazon az előre kiválasztott játéktérképen játszanak
  * a játék előre megadott mennyiségű nyert játszmákig tart
  * a játékos bombát helyezhet le az üres mezőkre (maga alá)
  * a játékos irányítja a karakterét a játéktérképen (üres mezőkre)
  * a szörnyek mozognak, és mozgásuk közben elkaphatnak játékosokat (megölve őket ezáltal)
    * akadályba ütközéskor véletlenszerűen váltanak irányt
    * néha meglepetésszerűen is változtatnak a haladási irányukon (képkockánként 0,5% eséllyel)
  * a játékos felrobbanhat a saját vagy egy más játékos bombájától
  * a játékos a játékot megállíthatja (Pause ablak előhozása Escape billentyű lenyomásával)
  * a játékos a játékot futás közben elmentheti (Pause ablakban a Save & Exit)
* **A játék vége**
  * egy játékos marad csak életben (a többi felrobbant és/vagy egy szörny elkapta)
  * ha egy játékos sem maradt életben
  * kiírásra kerül a nyertes játékos (vagy a döntetlenség ténye), az eddigi eredmények megjelenítésre kerülnek, utána egy új játszma indul (amennyiben vannak még nem érte el senki a megadott nyert játszmák számát)
    * az utolsó játszma után az összesített eredmény is megjelenítésre kerül
    * a döntetlen mindkét játékos szempontjából vereségnek számít
* **A játéktérkép tulajdonságai**
  * N×M méretű négyzetrácsos (grid) pálya
  * a pálya átjuthatatlan és felrobbanthatatlan fallal van körülvéve
  * minden mező a következő típusú lehet:
    * üres (szabadon járható)
    * fal (nem járható, nem átléphető)
    * doboz (felrobbantható bombával, a bomba hatótávját megállítja)
    * „power up” bónusz, egy doboz felrobbantását követően  30%-os eséllyel megjelenhet:
      * egy bombák számát megnövelő bónusz (1-gyel megnöveli a lerakható bombák számát)
      * egy robbanás hatótávát megnövelő bónusz (mind a 4 irányba 1-1 mezővel megnöveli a bombák hatótávját)
    * szörny (elpusztítható bombával, a játéktérképen mozog, megölheti a játékosokat)
    * játékos (elpusztítható bombával, a játéktérképen mozoghat, bombákat rakhat le, bónuszokat vehet fel)
    * bomba (felrobban **_π_** idő alatt egy adott hatótávban, amely a pálya mind a 4 irányába terjed, kivéve, ha egy doboz megállítja)
      * bombák egymást is felrobbanthatják az időzítőjük lejárta előtt, és a bomba robbanásának pozíciójától a távolsággal arányos módon (**_e_**-vel osztva) fejtik ki a hatásukat
      * egy bomba alapértelmezett hatótávja 2 egység minden irányban (átlósan nem terjed)

## Videó a játékról
[![](https://img.youtube.com/vi/5Lj4rnR6-MQ/0.jpg)](https://www.youtube.com/watch?v=5Lj4rnR6-MQ)


## Képernyőfotók a felhasználói felületről
![](https://i.imgur.com/rCC52IX.png)
![](https://i.imgur.com/CsVtvVA.png)
![](https://i.imgur.com/Qfo01gW.png)
![](https://i.imgur.com/FVCBeSG.png)
![](https://i.imgur.com/g8JbHzX.png)
![](https://i.imgur.com/FD8smsR.png)
![](https://i.imgur.com/6XI7QAe.png)

## Képernyőfotók a játékról
![](https://i.imgur.com/oTh41sH.png)
![](https://i.imgur.com/lVyXFwQ.png)
![](https://i.imgur.com/ds9pbwT.png)
![](https://i.imgur.com/7eNay00.png)
![](https://i.imgur.com/eW8AB3f.png)
![](https://i.imgur.com/Fm5HOkC.png)

## Feljesztés során használt eszközök
* integrált fejlesztőkörnyezet (JetBrains Rider)
* kódszerkesztő (Visual Studio Code)
* projektvezető szolgáltatás (Gitlab)
* verziókövető rendszer (Git)
  * használt Git client: https://git-fork.com/
* ingyenes és nyílt forráskódú játékmotor (Godot 4.2.1 játékmotor)
* tervező eszköz (Figma, PlantUML, StarUML)
* C# programozási nyelv
* objektumelvű megvalósítás
* CI teszteléshez használt szoftver: [Chickensoft GoDotTest](https://github.com/chickensoft-games/GoDotTest)
* felhasznált források:
    * [Kaykit adventurers](https://kaylousberg.itch.io/kaykit-adventurers) (játékosok)
    * [Kaykit skeletons](https://kaylousberg.itch.io/kaykit-skeleton-pack) (ellenségek)
    * [Analogue Buttons Pack I](https://prinbles.itch.io/analogue-buttons-pack-i?fbclid=IwZXh0bgNhZW0CMTAAAR0j0rQiFoxIIadyNnaww961VJgE50F9rKpG9fnOjuOb_GtsV4YGzWgl5lY_aem_ASiA1q0g6QXwSYIROLq_ugPNapExoeU7Sdo9QKuZrpuaTH8D2AuC8FjlZtFiQELosC_Bc7pZQim45iWfrhy4zmAU) (gombok)
    * [Wenrexa Game Assets: UI Sci-Fi Minimalism ](https://wenrexa.itch.io/nesia01?fbclid=IwZXh0bgNhZW0CMTAAAR3cud7iJxuqhpCjZN0Blq6EA54oEqtKFhtoY7TKKooCJLWiHc_w1IqSzhk_aem_AShpSI2FEtu9UE_Fnh-5ql3OU2Cc5L4LkyjMw_bsqzOy84PRMf-Wu0GvJcvMnlyLHLdtuWg7bF0m4-gZYuWW0j-w) #01 (UI)
    * [Godot VisualShaders Collection](https://github.com/daniel-ilett/shaders-godot?fbclid=IwZXh0bgNhZW0CMTAAAR3r7_8n2QASz6CfofABPqLDlRGmRX7d8c7OtF0WvyRG0S5dbk3kdOOW1zU_aem_AShernxKMq0xI8k4GtiOyxJTJDR54lhPKeeFK8OAkSlmYOoC1C6x72OAg-Ov83jULfO8Rn1buQpXfyYIWU4qF8tg) (shaderek)
    * [GODOT VFX - Easy Explosions](https://www.patreon.com/posts/godot-vfx-easy-93683992?fbclid=IwZXh0bgNhZW0CMTAAAR0iNWud4ZBcwBZIHT7Cj48f-ozxxy7WTtFLJDJNRovpe_gofpqbmXtniW8_aem_ASh_5z2PqQkkE1R6RBWW40OHs568gi9d4BzHUzVz3K20d5m3bXwJAgXPQ-nmwH3tso59c0S7SwGTbdfoPq10q9U-) (vfx)
    * [\[Godot/C#\] Tutorials](https://github.com/MinaPecheux/godot-tutorials?fbclid=IwZXh0bgNhZW0CMTAAAR26ub5lIsz9yXxGe8bmlf9BRZAmCgcP-kJdUyJxbQjE4zoMlvujGe9ZUuo_aem_ASgKi7l8AbHXYnRIpkR6qN7A2B0ccYeU3X_cmW83--4DzY5QCMiAqJ6ifffSocwhUniR7UGTd0DuofPsyiMy2XSg) (tutorialok)
    * [How to fix corrupt UIDs in a project (workaround inside)](https://github.com/godotengine/godot/issues/68661?fbclid=IwZXh0bgNhZW0CMTAAAR0Blij6nV1wCjQ5TxcTpTvUjssKOOTTar8036Vqwsf0162Ysff5x0vOWJk_aem_ASjY4ch4mvnTwTh2YeSB-YboAjF2CTSW26bQhQM4R2cPXoGgEudnOutmWoUwj6pVWJBHRymp8kjdoCmuvWcSGgp6) (korrupt UID-ra megoldás)

## Készítette
* Bugár Ádám
* Burus András
* Szekeres Márk
