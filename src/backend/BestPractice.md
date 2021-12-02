# ASP.NET SPA így csináld
## Használd a hívatalos VS template-et! 
És az aktuális .Net (Core) Framework verziót! release plan: https://devblogs.microsoft.com/dotnet/introducing-net-5/
## Frissíts!!! 
### VS
Mindig az utolsó updattel dolgozz!
### Nuget
Frissítsd le a templatetel bekerülő csomagokat! Ha lehet mindig minden csomagot frissíts!
### NPM
Frissítsd az NPM-ed az alábbi módon.

Run PowerShell as Administrator:
* Set-ExecutionPolicy Unrestricted -Scope CurrentUser -Force
* npm install -g npm-windows-upgrade
* npm-windows-upgrade

### Update Nod.js
https://nodejs.org/en/ (to recommanded)

(Az angular update-en előírt 10.13 minimal verzóval nekem nem voltak haljlandóak lemenni az ng updateek!)

### Angular refresh
Frissítsd az Angulart!

Verzió ellenőrzés:
`
ng version
`
A local verziód számít.
Utána az https://update.angular.io/ webhelyen válaszd ki a kiinduló verziód (nem feltétlen szerepel, a legnagyobb kisebbet válaszd, az azt jelenti, hogy a két verzó közt nem változott az update eljárás).

Válassz basic appot, és kövesd az utasításokat.
