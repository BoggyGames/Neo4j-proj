# Napomene za pokretanje:

NeonServer je metadata & recommendation backend za music streaming service čiji je front end WPF aplikacija NeonPlayer. Da bi NeonPlayer radio, potrebno je prvo na localhostu pokrenuti NeonServer.



Da bi NeonServer bio uspešno povezan sa neo4j bazom, potrebno je da je ona podešena sa podacima koji se sadrže u serveru, tj. neo4j/neo4j na navedenom portu. Takođe, ako trenutna aktivna neo4j baza sadrži stvari nevezane za trenutni projekat, predloženo je da se one uklone pre nego što počnete sa kreacijom metadata podataka za pesme.

# VELIKA NAPOMENA!!

Pesme možete dodavati preko NeonServer swagger admin POST komande, i sledeća imena pesama su validna za testiranje jer NJIH IMAMO UPLOADED NA REMOTE HOSTU - NeonPlayer koristi imena pesama da bi ih našao online u našem Azure hosted sajtu. Ostatak detalja možete da podesite po želji, ali će recommendation query da radi mnogo bolje ako ispoštujete i ostale napomene o metapodacima ovih pesama.
```
╒══════════════════════════╤══════════════════════╤══════════════════════╤═══════╤═══════╕
│Naziv - BITNO!            │Artist - BITNO!       │Album                 │Mood   │Genre  │
╞══════════════════════════╪══════════════════════╪══════════════════════╪═══════╪═══════╡
│"Dress Codes"             │["Haywyre", "Mr Bill"]│"Collaborations 19"   │"Funky"│"EDM"  │
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"The Schism"              │["Haywyre"]           │"Twofold"             │"Moody"│"EDM"  │
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"Prologue"                │["Haywyre"]           │"Twofold"             │"Chill"│"EDM"  │
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"Dichotomy"               │["Haywyre"]           │"Twofold"             │"Chill"│"EDM"  │
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"JTRs Vibe"               │["Mr Bill"]           │"Apophenia"           │"Funky"│"EDM"  │
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"Ejecta"                  │["Mr Bill"]           │"Apophenia"           │"Moody"│"EDM"  │
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"Dead Mans Plate"         │["Pentakill"]         │"Grasp Of The Undying"│"Heavy"│"Metal"│
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"Blade Of The Ruined King"│["Pentakill"]         │"Grasp Of The Undying"│"Moody"│"Metal"│
├──────────────────────────┼──────────────────────┼──────────────────────┼───────┼───────┤
│"Infinity Edge"           │["Pentakill"]         │"Grasp Of The Undying"│"Heavy"│"Metal"│
└──────────────────────────┴──────────────────────┴──────────────────────┴───────┴───────┘
```
Nakon što je baza preko NeonServer admin POST komande ili preko direktne intervencije u neo4j browseru popunjena neophodnim podacima, reprodukciju možete da isprobate u NeonPlayer-u - uđite u neku od pesama sa liste i pustite da vam recommendation query napravi lepu playlistu koja će da bude streamovana sa interneta - bez potrebe da se audio fajlovi zadržavaju kod vas na hard disku!
