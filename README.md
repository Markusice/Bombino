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

## Mit tehet a játékos a játék megkezdése előtt?
- testreszabhatja az irányítást
- egy mentett játékot törölhet (vagy betölthet)
- kiléphet a játékból
- kiválaszthatja a 2 vagy 3 játékos módot
- új játékot kezdhet vagy egy korábbit betölhet
- megadhatja a játéktérképet a 3 megadott közül
- megadhatja aj átékkörök (játszmák) számát


## Mit tehet a játékos a játék közben?
- bombát helyezhet le
- irányíthatja a karakterét a játéktérképen
- felrobbanhat a karaktere a saját vagy egy más játékos bombájától (meghalva ezáltal)
- a karaktere meghalhat, ha egy szörny elkapja
- a játékot megállíthatja
- kiléphet a játékból, elmentve az állapotát

## Videó a játékról
[Youtube Video](https://youtu.be/5Lj4rnR6-MQ)

## Felhasználói felület
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
