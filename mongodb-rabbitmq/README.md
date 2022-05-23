# Preparing the local env

```shell
    # Create instances of rabbitmq and mongodb and initialize the mongodb cluster
    docker compose up -d

    # check if the cluster is up and ok
    docker exec -it mongo1 mongosh --eval "rs.status()"
```

You can connnect to mongodb using this conn string:
`mongodb://localhost:27017/?authSource=admin&readPreference=primary&directConnection=true&ssl=false`

To access the rabbit admin go to:
http://localhost:15672/

```
    Username: guest
    Password: guest
```

Create a new exchange in rabbit:

    - Name: cap.default.router
    - Type: topic

Create a new queue in rabbit:

    - Type: Classic
    - Name: myapp2.paymentCondition.created

And bind it to a exchange:

    - From exchange: cap.default.router
    - Routing key: myapp.paymentCondition.created

Create a new queue in rabbit:

    - Type: Classic
    - Name: myapp3.paymentCondition

And bind it to a exchange:

    - From exchange: cap.default.router
    - Routing key: myapp.paymentCondition.*


# Test it

In VsCode (or Visual Studio) press F5


Access https://localhost:5001/swagger/index.html

