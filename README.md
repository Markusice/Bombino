# Bombino, a bomberman játék


## Ismertető
A játék célja egy bomberman típusú játék megvalósítása, ahol a játékosok egymás ellen játszva próbálnak egyedüliként életben maradni a pályán. A játékosok bombákat rakhatnak le, amellyel megölhetik egymást, de a pályán lévő dobozokat és szörnyeket is felrobbanthatják velük. A dobozokból különböző bónuszok kerülhetnek elő, a szörnyek képesek a játékosokat elkapni, megölni. A játék körökre van osztva, az nyer, aki az eléri az előre megadott nyert játszmák számát.

A játék Godot-ban lett fejlesztve, és Windows 10 operációs rendszeren tesztelve.

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
  * a játékos bombát helyezhet le üres mezőkre (maga alá)
  * a játékos irányíthatja a karakterét a játéktérképen (üres mezőkre)
  * a szörnyek mozognak, és mozgásuk közben elkaphatnak játékosokat (megölve őket ezáltal)
    * akadályba ütközéskor véletlenszerűen vált irányt
    * néha meglepetésszerűen is változtatnak a haladási irányukon (képkockánként 0,5% eséllyel)
  * a játékos felrobbanhat a saját vagy egy más játékos bombájától (meghalva ezáltal)
  * a játékos a játékot megállíthatja (Pause ablak előhozása Escape billentyű lenyomásával)
  * a játékos a játékot futás közben elmentheti (Pause ablakban a Save & Exit)
* **A játék vége**
  * egy játékos marad csak életben (a többi felrobbant és/vagy egy szörny elkapta)
  * ha egy játékos sem maradt életben
  * kiírásra kerül a nyertes játékos (vagy a döntetlenség ténye), az eddigi eredmények megjelenítésre kerülnek, utána egy új játszma indul (amennyiben vannak még játszmák hátra)
    * az utolsó játszma után a végeredmény is megjelenítésre kerül
    * a döntetlen mindkét játékos szempontjából vereségnek számít
* **A játéktérkép tulajdonságai**
  * N×M méretű négyzetrácsos (grid) pálya
  * a pálya átjuthatatlan fallal van körülvéve
  * minden mező a következő lehet:
    * üres (szabadon járható)
    * fal (nem járható, nem átléphető)
    * doboz (felrobbantható bombával, a bomba hatótávját megállítja)
    * „power up” bónusz, egy doboz felrobbantását követően  30%-os eséllyel megjelenhet:
      * egy bombák számát megnövelő bónusz (1-gyel megnöveli a lerakható bombák számát)
      * egy robbanás hatótávát megnövelő bónusz (mind a 4 irányba 1-1 mezővel megnöveli a bombák hatótávját)
    * szörny (elpusztítható bombával, a játéktérképen mozog, megölheti a játékosokat)
    * játékos (elpusztítható bombával, a játéktérképen mozog, bombákat rakhat le, bónuszokat vehet fel)
    * bomba (felrobban **_π_** idő alatt egy adott hatótávban, amely a pálya mind a 4 irányába terjed, kivéve, ha egy doboz megállítja)
      * bombák egymást is felrobbanthatják az időzítőjük lejárta előtt, és a bomba robbanásának pozíciójától a távolsággal arányos módon (**_e_**-vel osztva) fejtik ki a hatásukat
      * egy bomba alapértelmezett hatótávja 2 egység minden irányban (átlósan nem terjed)

## Videó a játékról
![](https://youtu.be/5Lj4rnR6-MQ)


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
* ingyenes és nyílt forráskódú játékmotor (Godot játékmotor)
* tervező eszköz (Figma, Visual Paradigm, StarUML)
* C# programozási nyelv
* Objektumelvű megvalósítás
* CI teszteléshez használt szoftver: [Chickensoft GoDotTest](https://github.com/chickensoft-games/GoDotTest)
* felhasznált modellek:
  * [Kaykit adventurers](https://kaylousberg.itch.io/kaykit-adventurers) (játékosok)
  * [Kaykit skeletons](https://kaylousberg.itch.io/kaykit-skeleton-pack) (ellenségek)

## Készítette
* Bugár Ádám
* Burus András
* Szekeres Márk
