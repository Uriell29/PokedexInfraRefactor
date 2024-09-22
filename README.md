# Pokedex API

This API provides information about Pokemons, including standard details and translated descriptions based on the Fun
Translation API.
It is built using .NET 8 and follows RESTful conventions.

## Requirements

- .NET SDK 8
- Docker (optional, for containerized execution)
- Any IDE or code editor (e.g., Visual Studio, Visual Studio Code)

## Run the app for local development

1. Clone the repository:

   `git clone `

   `cd PokedexApi`
2. Install the required dependencies:

   `   dotnet restore
   `
3. Run the API:

   `dotnet run
   `
4. The API will be available at http://localhost:5000.

## Running the tests

1. Restore the required packages:

   `dotnet restore`
2. Run all tests (unit and integration):

   `dotnet test`
3. The test results will be displayed in the console.

## Running the API with Docker

1. Make sure Docker is installed and running on your machine.
2. Build the Docker image:

   `docker build -t pokedexapi -f PokedexApi/Dockerfile .`
3. Run the Docker container:

   `docker run -d -p 8080:8080 -p 8081:8081 pokedexapi
   `
4. The API will be available at http://localhost:8080/

## API Endpoints

### Get Pokemon Information

- Endpoint: GET /api/v{version}/pokemon/{name}
- Description: Retrieves information about a Pokémon by its name.
- Responses:
    - 200 OK: Returns the Pokemon information.
    - 400 Bad Request: If the name is not provided or is invalid.
    - 404 Not Found: If no Pokemon is found with the given name.
    - 500 Internal Server Error: If an unexpected error occurs

#### Request

GET /api/v1/pokemon/pikachu

`curl -i -H 'Accept: application/json' http://localhost:5000/api/v1/pokemon/pikachu
`

#### Response

    HTTP/1.1 200 OK
    Date: Sun, 22 Sep 2024 15:02:24 GMT
    Content-Type: application/json; charset=utf-8
    Content-Length: 255

    {
        "name": "Pikachu",
        "description": "When several of these POKéMON gather, their electricity could build and cause lightning storms.",
        "habitat": "forest",
        "isLegendary": false
    }

### Get Pokemon Information with Translated Description

- Endpoint: GET /api/v{version}/pokemon/translated/{name}
- Description: Retrieves translated information about a Pokémon by its name.
- Responses:
    - 200 OK: Returns the Pokemon information with a translated description.
    - 400 Bad Request: If the name is not provided or is invalid.
    - 404 Not Found: If no Pokemon is found with the given name.
    - 500 Internal Server Error: If an unexpected error occurs.

#### Request

GET /api/v1/pokemon/translated/pikachu

`curl -i -H 'Accept: application/json' http://localhost:5000/api/v1/pokemon/translated/pikachu`

#### Response

HTTP/1.1 200 OK
Date: Sun, 22 Sep 2024 15:02:24 GMT
Content-Type: application/json; charset=utf-8
Content-Length: 255

    {
        "name": "Pikachu",
        "description": "At which hour several of these pokémon gather,  their electricity couldst buildeth and cause lightning storms.",
        "habitat": "forest",
        "isLegendary": false
    }

## Accessing Swagger Documentation

Swagger UI provides a visual interface to explore and test the API.

1. Run the API locally or in Docker.
2. Open your browser and navigate to https://localhost:5000/swagger (if running locally from IDE)
   or http://localhost:8080/swagger (if running from Docker container).


