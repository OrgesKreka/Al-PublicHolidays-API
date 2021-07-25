# API per festat zyrtare ne shqiperi

Projekti merr te dhenat e pushimeve zyrtare nga faqja e [Bankes se Shqiperise](https://www.bankofalbania.org/Shtypi/Kalendari_i_festave_zyrtare_per_vitin_2021/) dhe i formaton ne json.

---
### Ekzekutimi i programit:
Nese ke te instaluar .NET 5, programi mund te ekzekutohet me komanden e meposhtme:

    cd PublicHolidays.API/
    dotnet run
Programi mund te ekzekutohet dhe si container docker duke perdorur komandat meposhte:

    docker build -t al-public-holidays-api ./PublicHolidays.API/.
    docker run --rm -t -p 8000:80 al-public-holidays-api

### Dokumentimi i API-t:
Swagger Dokumentimi gjendet tek [linku](https://app.swaggerhub.com/apis/foo-hub/Al-PublicHolidays-API/v1).
Shkurtimisht api permban dy endpointe.
1. */api/v1/holidays* qe kthen listen e gjithe pushimeve per vitin.
   Me poshte nje shembull kerkese/pergjigje

Request:
GET  https://localhost:5001/api/v1/holidays

<details>
<summary>RESPONSE</summary>
<p>

  ```json
  [
   {
      "day":"01/01/2021",
      "name":"Festat e Vitit të Ri",
      "note":null,
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"02/01/2021",
      "name":"Festat e Vitit të Ri",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"14/03/2021",
      "name":"Dita e Verës",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"22/03/2021",
      "name":"Dita e Nevruzit",
      "note":null,
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"04/04/2021",
      "name":"E diela e Pashkëve Katolike",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"01/05/2021",
      "name":"Dita Ndërkombëtare e Punëtorëve",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"02/05/2021",
      "name":"E diela e Pashkëve Ortodokse",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"13/05/2021",
      "name":"Dita e Bajramit të Madh",
      "note":"**Shënim: Data e festave të Kurban Bajramit dhe Bajramit të Madh mund të ndryshojë sipas kalendarit hënor. Në rast se do të ketë ndryshime, do të viheni në dijeni me një njoftim të dytë.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"20/07/2021",
      "name":"Dita e Kurban Bajramit",
      "note":"**Shënim: Data e festave të Kurban Bajramit dhe Bajramit të Madh mund të ndryshojë sipas kalendarit hënor. Në rast se do të ketë ndryshime, do të viheni në dijeni me një njoftim të dytë.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"05/09/2021",
      "name":"Dita e Shenjtërimit të Shenjt Terezës",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"28/11/2021",
      "name":"Dita Flamurit dhe e Pavarësisë",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"29/11/2021",
      "name":"Dita e Çlirimit",
      "note":null,
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"08/12/2021",
      "name":"Dita Kombëtare e Rinisë",
      "note":null,
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"25/12/2021",
      "name":"Krishtlindjet",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   }
]
```

</p>
</details>

---
2. */api/v1/holidays/filter?fromDate=date&toDate=date* qe kthen listen e pushimeve per nje periudhe te caktuar kohore.
   Me poshte nje shembull kerkese/pergjigje

Request:
GET https://localhost:5001/api/v1/holidays/filterfromDate=27/07/2021&toDate=31/12/2021

**SHENIM:** Ne kete rast vlerat e fromDate dhe toDate duhet te jene sipas formatit dd/MM/yyyy
PSH: 01/01/2021
ku viti duhet te jete me vitin aktual!

<details>
<summary>RESPONSE</summary>
<p>

  ```json
  [
   {
      "day":"05/09/2021",
      "name":"Dita e Shenjtërimit të Shenjt Terezës",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"28/11/2021",
      "name":"Dita Flamurit dhe e Pavarësisë",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"29/11/2021",
      "name":"Dita e Çlirimit",
      "note":null,
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"08/12/2021",
      "name":"Dita Kombëtare e Rinisë",
      "note":null,
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   },
   {
      "day":"25/12/2021",
      "name":"Krishtlindjet",
      "note":"*Në rastet kur festat zyrtare bie në ditët e pushimit javor (e shtunë ose e diel), dita e hënë është ditë pushimi.",
      "lastUpdateDate":"Përditësuar më 14 tetor 2020"
   }
]
  ```

  </p>
  </details>
