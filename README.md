# Preparing the local env

```shell
    # Create instances of rabbitmq, sql server and mongodb and initialize the mongodb cluster
    docker compose up -d

    # check if the cluster is up and ok
    docker exec -it mongo1 mongosh --eval "rs.status()"

    # create initial tables in sql server
    dotnet ef migrations add Initial
    dotnet ef database update
```

You can connnect to mongodb using this conn string:
`mongodb://localhost:27017/?authSource=admin&readPreference=primary&directConnection=true&ssl=false`

To access the rabbit admin go to:
http://localhost:15672/

```
    Username: guest
    Password: guest
```

# Test it

In VsCode (or Visual Studio) press F5


Access https://localhost:5001/swagger/index.html


