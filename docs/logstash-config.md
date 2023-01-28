# Logstash Configuration
## Environment Variables
### [LS_JAVA_OPTS](https://www.elastic.co/guide/en/logstash/current/jvm-settings.html#ls-java-opts): "-Xms512m -Xmx512m"

Works exactly the same as ES_JAVA_OPTS in Elasticsearch. Lets you configure how
much memory the JVM uses. I think the default is 1GB.

## Ports
### [5000](https://www.elastic.co/guide/en/logstash/current/config-examples.html#_processing_syslog_messages)
This is the default port for incoming log events.

### [5044](https://www.elastic.co/guide/en/logstash/current/plugins-inputs-beats.html)
This is the default port for the
[Beats input plugin](https://www.elastic.co/guide/en/logstash/current/plugins-inputs-beats.html).

### [9600](https://www.elastic.co/guide/en/logstash/current/monitoring-logstash.html)
This is used for the monitoring API so you can have some observability into the
logstash instance.

## Config files
### [logstash.conf](https://www.elastic.co/guide/en/logstash/current/configuration.html)
This file is your pipeline. It is where you configure inputs, filters, and
outputs. Probably the most important piece.

### [input](https://www.elastic.co/guide/en/logstash/current/input-plugins.html)
This is where you configure incoming messages. An example:
```
input {
    http {
        port => 5000
    }
}
```
Now you can send log events over http on port 5000 to logstash.

### [filter](https://www.elastic.co/guide/en/logstash/current/filter-plugins.html)
This is where you transform incoming log events to look like what you want. This
is where the bulk of your logstash magic happens, and there's a huge amount of
stuff you can do here. From our example:
```
filter {
    mutate {
        remove_field => [ "[metadata][file]" ]
    }
}
```
This will look at the incoming log event JSON, for a field matching the
following structure:
```
{
    "_source": {
        "metadata": {
            "file": {
                ...
            }
        }
    }
}
```
And remove it, leaving you with:
```
{
    "_source": {
        "metadata": {
            ...
        }
    }
}
```
This might be slightly not what you were expecting if you don't already know,
but your log event gets wrapped in an outer JSON object, and your original
payload is inside of the ```_source``` field. Your filters must be
written assuming you are alreading inside this field.

### [output](https://www.elastic.co/guide/en/logstash/current/output-plugins.html)
Once your log event has been filtered to your satisfaction, you'll send it here
where it can be sent out to whatever output you have configured (probably
Elasticsearch).
```
output {
    elasticsearch {
        hosts => ["elasticsearch:9200"]
        ssl_certificate_verification => false
        index => "elasticstack-api-1"
    }   
    stdout { codec => rubydebug }
}
```
This says write to an Elasticsearch instance, turn off SSL, and write everything
to the specified index. Also send it to ```stdout```.

An important detail on these: each one you specify here opens a separate
connection to the instance. When you first start out, it might feel like you
need lots of these to organize your indexes, but there are some things you can
do to work around this. Check out this
[best practices](https://www.elastic.co/guide/en/logstash/current/plugins-outputs-elasticsearch.html#_writing_to_different_indices_best_practices)
page for some info on how you can accomplish this.

### [logstash.yml](https://www.elastic.co/guide/en/logstash/current/logstash-settings-file.html)
Similar to the elasticsearch.yml file, this gives you a route to control the
configuration of the logstash instance. Also as with the other file, I think
most if not all of these settings can be controlled through environment
variables as well.

This example shows how to do it both ways.
