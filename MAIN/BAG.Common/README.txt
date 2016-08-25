
schlüssel route: suche, search, portal, blog, admin, robots.txt, sitemap.xml



---------------------------------------------------------------
Google Analytics Conversion Events
---------------------------------------------------------------

Diese Events müssen in den unten angegeben Bereichen getriggert
werden.

- Merkliste

	-> ga('send', 'event', 'Merkliste', 'Add');
	-> ga('send', 'event', 'Merkliste', 'Remove');
	-> ga('send', 'event', 'Merkliste', 'Purchase');

- Anfrage (Dialog)

	-> ga('send', 'event', 'Anfrage', 'Purchase');
	-> ga('send', 'event', 'Anfrage', 'Abort');

- Suche

	-> ga('send', 'event', 'Tischlersuche', 'Search');
	-> ga('send', 'event', 'Tischlersuche', 'Selected');
	-> ga('send', 'event', 'Tischlersuche', 'Contacted');
	-> ga('send', 'event', 'Tischlersuche', 'MessageSent');

---------------------------------------------------------------
Verwendete NuGet Packages
---------------------------------------------------------------

-> FileContext
-> angularjs
-> gwc
-> _globals

bootstrap 3
font-awsome
http://ionicons.com/

---------------------------------------------------------------
Entitäten
---------------------------------------------------------------

Edition
Accesoire
Kollektion
 -> Editon
 -> Accessoire
Tischler

---------------------------------------------------------------
HowTo: Ein Widget anlegen
---------------------------------------------------------------

NAME := Widget%XY%

1. Partial unter ~/Views/Widgets anlegen (Namenskonvention NAME)
2. In Common unter ~/Data/Entities das "Modell" anlegen  NAME
3. Widget-Klasse mit Attributen versehen [XmlInclude(typeof(NAME))]

Hinweis: Bei nicht flachen Hierarchien die WidgetEdit anpassen.

---------------------------------------------------------------
ElFinder.NET: Hochgeladene Bilder einbinden
---------------------------------------------------------------

FILEHASH := v1_dm9yZ2FiZW5cZWRpdGlvbl90aXNjaGxlcl92NS5qcGc1

Einbettungslink: ~/file?cmd=file&target=FILEHASH


---------------------------------------------------------------
Image Selector einbinden
---------------------------------------------------------------

INSTANCENAME := {Beliebieger eindeutiger Name} bsp: selectImg1

<div class="img-select" data-name="INSTANCENAME">
    <button class="select-image">Bild auswählen</button>
    <button class="reset-image">Reset</button>
    <img class="preview image-responsive" style="width: 200px; max-height:200px;"/>
    <input class="selection" type="hidden" name="image"/>
</div>

Die Elemente müssen folgende Klassen haben:
	-> Oberelement    => img-select
	-> Auswahl-Button => select-image
	-> Reset-Button   => reset-image    (Optional)
	-> Vorschau-IMG   => preview
	-> Form-Control   => selection

Der beinhaltende Container muss das Data-Attribut name enthalten
um Kollisionen mit anderen Picker-Instanzen zu verhinden.

	Bsp.: data-name="selectImg1"

Das Form-Control kann type=text oder type=hidden sein.






TODO: 


Integration des QR-Code-Tools von Marcus und eines VCard-Generators in das CMS (Datenebene). 

(x) Link-shortener für alle Seiten! 

CMS mit Profilverwaltung verbinden, um Erfolgsgeschichten automatisiert mit der Website, den Word-Dokumenten und den PDF-Downloads zu syncronisieren. 

Beibehalten und Inszenierung des tl;dr. 

SEO-Felder: 

Veröffentlichungs-Datum 

Manueller URL-Eintrag (Vorschau) 

(/) Autor (rel="author"; auch mit G+ verknüpfen; User Profile [Drop down]) 

Alttexte für Bilder 

(x proirity) Sitemap Prio 

(x) Indiezierungsstaus bei der Sitemap 

(x) Indexierung auf der Seitensuche 

Canonical redirects (Verify that you can implement a proper canonical redirect (using 301s!). In addition, check out the way the CMS handles the default document. Are you going to end up with all your internal links pointing to www.youdomain.com/index.html (or something similar)? This is not desirable.) 

Default redirects (Hopefully, the default redirects are 301 redirects. If not, it must be simple to select a 301. Otherwise at some point in the future some well meaning developer will end up using a 302 by accident.) 

Ereignisse (Events) via Editor ermöglichen (für Analytics) 

Language-Tag 

Echtzeit-Wörterzähler im Editor einbinden (Textstatistiken). Keywordzähler. 

Scripte und Trackingcodes von Analytics und Webmastertools sollen im CMS pflegbar und änderbar sein. 

Verwaltung mehrerer URLs, Canonicals, manuelle Weiterleitungen bei Löschung 

Icons einbinden können (Bootstrap) 

Hilfe/FAQ bzw. Dashboard zu nützlichen Seiten oder mit Tutorials (Dokumentation, Wiki) 

Daten (wie Kontakte, etc.) 

Breadcrumbs 

Microformat Data/ Richsnippets/ Structured Data Support (https://developers.google.com/structured-data/) 

Newsbereich mit optionalem Abo (Autor, Datum, Tags, etc.), jede News mit einer eigenen URL 

CRM-Integration (Dokumenten-Download, Kontaktformular, mit Wiedervorlage) 

Blog-Modul (mit grundsätzlichen Features die auch Wordpress mit sich bringt) 

WebSpeed (Minifyer, Bilderkomprimierung) 

Blogmodul (Mit extra Rechten nur für den Blog, ohne Website) 

Drop-Down Navigation Menus Built in CSS 

No Frames / Iframes 

Tooltips (Mouse-Over) im Backend, damit man weiß wo man hin klicken soll. 

Multisite- und Multilanguage 

Vielleicht ein Eventmodul, für unsere Webcasts oder Veranstaltungen (Termine) 

Link zu einer internen Seite auswählen können 

Links zu PDF Dokumenten einbinden 

Sicherheistfrage beim verlassen von ungesicherten Inhalten (Editor) 

Modulabhängige Favicons 

Rollenverwaltung 

Bootstrap Funktionen sollen auch für nicht Techniker nutzbar sein. Entweder mittels des Editors oder dank passender Tutorials. 

Versioning und Autosave 

Desktop und Mobile Ansicht 

WYSIWYG editor 

Suchfunktion 

Social Media anbindung 

XML-Sitemap Generator mit "lastmod" 

Openthesaurus API für Synonyme integrieren 

Kontaktdaten der Mitarbeiter pflegen können. 

Responsive, nicht nur adaptive 

Mehr Fotos, auch vom Team auf der Seite (Fotogalerie) 