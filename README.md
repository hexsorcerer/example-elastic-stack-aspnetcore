# Elastic Stack example with ASP.NET Core
An example project showing how to setup an elastic stack with an aspnetcore
application. The main things shown here are:

- Configure the Elastic Stack
- Log in ECS format from Serilog
- Automatically apply dashboards to kibana in docker-compose

# Getting Started
```
git clone https://github.com/hexsorcerer/example-elastic-stack-aspnetcore.git && cd example-elastic-stack-aspnetcore && cp src/.env.example src/.env
```
This will clone the repo and setup your .env file for you, just open in vs2022
and run the docker-compose configuration to start the system.

# Docs

[Configuring Elasticsearch](./docs/elasticsearch-config.md)

[Configuring Logstash](./docs/logstash-config.md)

[Configuring Kibana](./docs/kibana-config.md)

These explain the options required to get an elastic stack up and running.

[Kibana Backup/Restore](./docs/kibana-backup-restore.md)

This shows how to use the export/import APIs in Kibana to backup your
dashboards, and how to use docker-compose to automate the process.

[Serilog EcsTextFormatter Configuration](./docs/serilog-ecs-formatter.md)

This shows how to configure Serilog so that you can log ECS-formatted messages
using normal structured logging syntax (which oddly enough is not available
out of the box).

[Resources](./docs/resources.md)
