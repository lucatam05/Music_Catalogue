# Music Catalogue 
Un microservizio in C# che gestisce le richieste all'API di Spotify

---

# Struttura
- Repository: gestisce la comunicazione con il DB
- Business: strato logico superiore a Repository, che rende l'accesso al DB controllato
- ClientHttp: il suo scopo è essere il pacchetto Nuget per la comunicazione sincrona con il microservizio User e Library
- SpotifyClient: cuore del progetto, comunica con l'API di Spotify per cercare le canzoni dato il nome, l'artista o l'album
- Web API: espone API crud


