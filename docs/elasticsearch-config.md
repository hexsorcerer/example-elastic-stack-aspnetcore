# Elasticsearch Configuration

## Environment Variables

### [discovery.type](https://www.elastic.co/guide/en/elasticsearch/reference/current/modules-discovery-settings.html#modules-discovery-settings)
This needs to be set to ```single-node``` to avoid timing out while looking for
other nodes in the cluster (because there won't be any).

### [xpack.security.enabled](https://www.elastic.co/guide/en/elasticsearch/reference/current/security-settings.html)
This needs to be set to ```false``` if you want to use elasticstack locally for
development without having to worry about security certificates (don't do this
in production!).

### [ES_JAVA_OPTS](https://www.elastic.co/guide/en/elasticsearch/reference/current/advanced-configuration.html)
Setting this to ```"-Xms512m -Xmx512m"``` sets the JVM heap size to 512MB. The
default is based on the node role, but I'm so far unable to find any
documentation stating what the values are for each role. I'm seeing 1GB
mentioned a lot though, so it may be that.

Either way, Elasticsearch provides this guideline for knowing how much heap size
you need:

[Master-eligible nodes should have at least 1GB of heap per 3000 indices](https://www.elastic.co/guide/en/elasticsearch/reference/current/size-your-shards.html#shard-count-recommendation)

## Ports

### [9200](https://www.elastic.co/guide/en/elasticsearch/reference/current/modules-network.html#common-network-settings)
This is the port used for client connections over HTTP.

### [9300](https://www.elastic.co/guide/en/elasticsearch/reference/current/modules-network.html#common-network-settings)
This is used for communication between nodes.

## Volumes

### [/usr/share/elasticsearch/data](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html#_always_bind_data_volumes)
This seems to be the default directory where Elasticsearch stores indexes.

### [/usr/share/elasticsearch/config](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html#docker-configuration-methods)
This is where the Elasticsearch configuration files get loaded from when you're
running in Docker.

## [elasticsearch.yml](https://github.com/elastic/elasticsearch/blob/main/distribution/src/config/elasticsearch.yml)
This file can also be used to configure Elasticsearch instead of environment
variables. This example project shows how to do it both ways.
