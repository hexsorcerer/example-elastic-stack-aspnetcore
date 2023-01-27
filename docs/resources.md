# Elasticsearch
## Basic Configuration
[Discovery and cluster formation settings](https://www.elastic.co/guide/en/elasticsearch/reference/current/modules-discovery-settings.html#modules-discovery-settings)

You need to set the ```discovery.type``` setting to ```single-node``` as a basic
configuration step, and that setting is documented here. You'll also find here
all of the other settings to work with nodes/clusters.

[Configuring the Elasticsearch for a single instance](https://www.ibm.com/docs/en/product-master/12.0.0?topic=elasticsearch-configuring-single-instance)

Has a little bit more detail about some settings you might want to consider for
running a basic Elasticsearch instance, I found this helpful to fill in some
blanks from the official docs.

[Networking](https://www.elastic.co/guide/en/elasticsearch/reference/current/modules-network.html)

I would consider this a basic configuration item, because if you can't connect
then you can't log. The example is primarily concerned with how to control the
address and port you listen on, but this document covers every network seting
available if you need to know more than what the example shows.

[Config files location](https://www.elastic.co/guide/en/elasticsearch/reference/current/settings.html#config-files-location)

Shows the different configuration files available to use, and where they should
be located. I believe that all of the settings in the config file can also be
set via environment variables, but if you prefer to use the config files for
whatever reason this is some helpful info.

[elasticsearch.yml](https://github.com/elastic/elasticsearch/blob/main/distribution/src/config/elasticsearch.yml)

Here's an example config file you can copy/paste to get started if you don't
have one already.

## Security
[Security settings in Elasticsearch](https://www.elastic.co/guide/en/elasticsearch/reference/current/security-settings.html)

We set the ```xpack.security.enabled``` to ```false``` to avoid having to deal
with issues surrounding TLS/certificates in the example. All of the settings
related to the various security options can be found here.

[Start the Elasticstack with security enabled automatically](https://www.elastic.co/guide/en/elasticsearch/reference/current/configuring-stack-security.html)

Describes in more detail the security-related actions that occur when you start
Elasticsearch for the first time, and how to get Kibana configured to work with
security enabled. We don't use this in the example but it's important to know
for a real-world deployment.

## Advanced Configuration
[Important Elasticsearch configuration](https://www.elastic.co/guide/en/elasticsearch/reference/current/important-settings.html)

These are the settings that according to Elasticsearch "must be considered
before using your cluster in production".

[Advanced Configuration](https://www.elastic.co/guide/en/elasticsearch/reference/current/advanced-configuration.html)

This is mostly configuration of the JVM.

[Set the JVM heap size](https://www.elastic.co/guide/en/elasticsearch/reference/current/advanced-configuration.html#set-jvm-heap-size)

A subset of the above document, which I add here only because I used this option
in the example because I think you should know how to control the memory usage
on this when running locally because we don't always have 64GB+ RAM avaiable all
the time.

[Size your shards](https://www.elastic.co/guide/en/elasticsearch/reference/current/size-your-shards.html)

Sharding is an advanced topic that's not dealt with at all in this example, but
I included this link because it covers some best practices related to sizing
the JVM heap.

[Node](https://www.elastic.co/guide/en/elasticsearch/reference/current/modules-node.html)

Contains additional information about the different node roles, which I thought
was helpful to understand a little better what running as a single-node affects.

[Master-eligible nodes should have at least 1GB of heap per 3000 indices](https://www.elastic.co/guide/en/elasticsearch/reference/current/size-your-shards.html#shard-count-recommendation)

A subset of the above document, clearly states how big your JVM heap should be
relative to the amount of logs you're storing, removes the guesswork out of this
setting.

## Docker
[Install Elasticsearch with Docker](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html)

Shows what images to use, settings that need to be set, and has examples for
both CLI commands and docker-compose. Was very helpful in putting this example
together.

[Configuring Elasticsearch with Docker](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html#docker-configuration-methods)

A subset of the above document, goes into more detail about how you can use any
setting as an envorinment variable, which isn't obvious if you don't already
know you can do that. Also has some details on how to bind-mount config files
correctly.

## Elastic Common Schema (ECS)
[What is ECS?](https://www.elastic.co/guide/en/ecs/current/ecs-reference.html)

Covers the set of common fields that all messages stored in Elasticsearch must
have. Great for understanding all the other fields that just magically appear
on your logs.

[ECS Field Reference](https://www.elastic.co/guide/en/ecs/current/ecs-field-reference.html)

Details on all the ECS fields, if you need to understand a particular one.

[Metadata fields](https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping-fields.html)

Not exactly ECS, but Elasticsearch stores documents using these metadata fields
so I would consider it part of the schema. The most important field is
```_source```, which contains your actual event JSON object sent from logstash.
If you were wondering what all those other fields are and where they came from,
here you go!

[Strings are dead, long live strings!](https://www.elastic.co/blog/strings-are-dead-long-live-strings)

The best documentation I've found so far that explains what the ```fields```
object is, and why all those seemingly-duplicate values are stored in there
with a ```keyword``` suffix (it's for better searching).

[difference between a field and the field.keyword](https://stackoverflow.com/a/48875105)

This is the stackoverflow question that led me to discover the 'strings are
dead...' blog post.

# Logstash
## Basic Configuration
[Creating a Logstash pipeline](https://www.elastic.co/guide/en/logstash/current/configuration.html)

The most basic example of building a logstash pipeline, excellent starting point.

[Logstash configuration examples](https://www.elastic.co/guide/en/logstash/current/config-examples.html)

Has some really good examples of more realistic setups, gets into filters and
conditional statements.

[How to create multiple indexes in logstash.conf file](https://stackoverflow.com/a/33820688)

Where I learned how to control what index is being written to. I read somewhere
else later than it's better to try and keep the elasticsearch outputs to a
minimum because each one opens a new connection.

[How to use Logstash to parse and import JSON data into Elasticsearch](https://www.youtube.com/watch?v=_qgS1m6NTIE)

One of the best video walkthroughs I was able to find, explains things very
well and shows some realistic examples.

## Best Practices
[What are the rules for index names in Elasticsearch?](https://stackoverflow.com/a/41585755)

While this is technically an Elasticsearch thing and not a logstash thing, I put
it here because logstash is actually where we configure the index names in the
example. Worth pointing out the specific limitation that index names can't have
'.', which prevented me from using a variable to simplify the logstash config.

[[discuss] removal of @timestamp field](https://github.com/elastic/logstash/issues/10581)

Ran into this issue myself while working on the example, the Serilog formatter
adds the @t field with the timestamp, and @timestamp already existed, so when I
attempted to remove @timestamp and keep @t, logs just silently stopped working
and it wasn't clear why at first. I ended up removing the @t field instead, but
wanted to make sure to point out that you currently can't do this in case you
try.

[Tips and best practices](https://www.elastic.co/guide/en/logstash/current/tips.html)

Contains the helpful trick to use ```@metadata``` to test for the existence of
a field in the incoming message.

[logstash extract and move nested fields into new parent field](https://stackoverflow.com/a/50268552)

The stackoverflow question that showed me how to structure individual fields
into a new JSON object.

## Plugins
### Input
[Introducing the Logstash HTTP input plugin](https://www.elastic.co/blog/introducing-logstash-input-http-plugin)

Excellent introduction to the input plugin for noobs like me.

[Http input plugin](https://www.elastic.co/guide/en/logstash/current/plugins-inputs-http.html)

All the docs on the http input plugin.

### Filters
[Mutate filter plugin](https://www.elastic.co/guide/en/logstash/current/plugins-filters-mutate.html)

All the docs on the mutate filter.

[Using the Mutate Filter in Logstash](https://logz.io/blog/logstash-mutate-filter/)

An unofficial walkthrough for the mutate filter, helps to hear it described from
a different perspective than the official docs.

[Parsing Logs with Logstash](https://www.elastic.co/guide/en/logstash/current/advanced-pipeline.html)

The main draw here is a walkthrough of some grok usage.

### Output
[Elasticsearch output plugin](https://www.elastic.co/guide/en/logstash/current/plugins-outputs-elasticsearch.html)

Some excellent guidance on how to best use the output plugin can be found here.
Be sure to check out the optimization when writing to multiple indexes by using
```@metadata``` to create dynamic fields, this will allow you to write to
multiple indexes with a single ```elasticsearch``` block.

# Serilog
## Basic
[Serilog.Sinks.Http](https://github.com/FantasticFiasco/serilog-sinks-http)

Shows basic sink usage, good when getting setup and configured application-side.

[Serilog.Formatting.Compact](https://github.com/serilog/serilog-formatting-compact)

This is really helpful to get an idea of what your log messages are going to
look like. Might be worth pointing out that the fields you see here will be
on the log event, but there will be other fields added from other places also,
like the http input plugin to logstash.

## Advanced
[Writing Log Events](https://github.com/serilog/serilog/wiki/Writing-Log-Events)

Some really good advanced Serilog usage in here, I really need to come back to
this and dig into it some more.

[messagetemplates.org](https://messagetemplates.org/)

A formal description of the message template syntax used in Serilog.

## ECS

[Elastic Common Schema .NET library and integrations released](https://www.elastic.co/blog/elastic-common-schema-dotnet-library-and-integrations-released-for-elasticsearch)

A blog post introducing the package, which oddly enough contains WAY more info
than the documentation. Definitely recommend starting here.

[ecs-dotnet](https://github.com/elastic/ecs-dotnet)

The project for adding ECS support to .NET/Serilog. This does provide you with
a useable schema, but getting it into your logs the way you expect requires some
more effort.

[ECS Logging .NET Get Started](https://www.elastic.co/guide/en/ecs-logging/dotnet/master/setup.html)

Shows the absolute most basic setup for using the ECS text formatter for
Serilog. Good first step, but was missing some critical info on how to add
fields defined in the ECS, which I had to figure out elsewhere.

[[BUG] Using Elastic.CommonSchema.User, but still showing under _metadata
](https://github.com/elastic/ecs-dotnet/issues/133#issuecomment-777576172)

One of the most important links here, this contains the original solution I used
to get Serilog to output ECS-formatted JSON by using structured properties in
the log message. It's clunky but it does work!

# Kibana

[REST API](https://www.elastic.co/guide/en/kibana/current/api.html)

The base API docs.

[Export objects API](https://www.elastic.co/guide/en/kibana/current/saved-objects-api-export.html)

Provides the API documentation for how to export stuff from Kibana. Be careful
on those examples, they don't work as-is. At least for me, using curl 7.83.1 on
Windows required me to reformat the command a little bit.

[Import objects API](https://www.elastic.co/guide/en/kibana/current/saved-objects-api-import.html)

The counterpart of the export objects API. Things to look out for here are that
the file you import must be formatted as ndjson (which is should be if you
exported it). Also you must include the Content-Type header here, the call
failed when I omitted it.

[Unable to upload ndjson files to /api/saved_objects/_import](https://forum.opensearch.org/t/unable-to-upload-ndjson-files-to-api-saved-objects-import/3110/2)

Ran into this issue myself while trying to figure out how to do this, turns out
you "must" omit the Content-Type header when importing or else it doesn't work
(in contrast, you "must include" the Content-Type header for the export
command.) You also need to use ```--form file=@<path-to-file>``` and not the
```-d``` option that you see in many similar examples floating around out there.


