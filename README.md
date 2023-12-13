
# Minimalist Biking App

This project is meant to create an app that makes itineraries using bikes from JCDecaux if available. 
The end user is asked to provide two adresses and will be given a list of directions from start to finish. The program will provide the user with the ability to use bikes from JCDecaux under three conditions : 

    A) There are stations in the areas of the start and end positions 
    B) Bikes are available at said stations 
    C) The distance including the directions to both stations is less than 1,5x of the walking distance


## Structure

The structure is the following : a java client makes requests to a routing SOAP C# server that will make requests to the Open Street Map API as well as a proxy C# server that will interact with the JCDecaux API.

![Picture of the structure of the program](https://github.com/nddav/Minimalist-Biking-App/blob/main/Strucutre.PNG)
