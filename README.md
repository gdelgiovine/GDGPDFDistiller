GDGPDFDistiller è una Rest-API .NET che si occupa di:

Convertire files PostScript e PCL in files PDF usando GhostScript e GhostPCL
Convertire files HTML in files PDF usando IText

GDGPDFDistiller è strutturato in una Rest-API .NET che riceve le richieste di conversione sotto forma di files PS, PCL ed HTML accompagnati da un file Json che contiene i parametri di conversione
e restituisce i files in formato PDF. 
È logicamente strutturato in una libreria con i metodi di conversione ed una Rest-API che espone tali metodi.

Il servizio di conversione HTML è attualmente focalizzato sulla conversione in formato PDF dei reports FastReport Community Edition esportati in HTML.
La licenza è quella di GhostScript e Itext: AGPL.
