FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["truckPRO_api.csproj", "."]
RUN dotnet restore "./truckPRO_api.csproj"
COPY . .
RUN dotnet build "./truckPRO_api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Use the official ASP.NET Core runtime image (Linux) to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 8080
EXPOSE 8081

# Final step to run the app
ENTRYPOINT ["dotnet", "truckPRO_api.dll"]


#push to azure registry
#docker login -u userName -p password truckproo.azurecr.io - login to azure 
#docker tag first:latest truckproo.azurecr.io/first:latest - tag dockerimage 
#docker push truckproo.azurecr.io/first:latest - push docker image 