# Web Api For Movies #

A REST api that fetches data from omdb. 

## Technologies ##

- ASP.Net Core
- Serilog (for logging)

## To Do LIst ##
- [ ] Add db support for storing fetched data 

- [ ] Add more methods for fetching data and storing them in local db

- [X] Add users and authorization

***Important: You must change the value of ```OMDB_Key``` in ```OMDBConf.json``` file for the app to run.***

**Note: OAuth authentication has been added. Since it is a project for demo purposes, the values for the OAuth token are these:**

|Key|Value|
|:--|--:|
|Grant Type|```Client Credentials```|
|Client Id|```client```|
|Client Secret|```secret```|
|Scope|```Api1```