# Kibana Backup/Restore
If you're like me, you probably said to yourself "wouldn't it be nice if we
could save all our kibana dashboards and stuff and then just automatically
import them whenever we re-create the system?".

I sure did. And I figured it out. Here's how:

## Export dashboard with all dependencies
While viewing your dashboard, you can get the ID from the URL, should be pretty
obvious. Once you have it, set the "id" field in the JSON payload below and
POST it to your kibana instance:
```
curl -X POST http://localhost:5601/api/saved_objects/_export -H 'Content-Type: application/json' -H 'kbn-xsrf:true' -d '{ "objects": [{ "type": "dashboard", "id": "<your-dashboard-id" }], "includeReferencesDeep": true }' > dashboards.ndjson
```
A couple of things to note:
- the Content-Type header is required
- includeReferencesDeep is set to true
- the file extension is ndjson

Omitting the Content-Type header will result in the following error response:
```
{"statusCode":400,"error":"Bad Request","message":"[request body.{ \"objects\": [{ \"type\": \"dashboard\", \"id\": \"2f3eb850-9de3-11ed-bc1f-b5cb7fdf7838\" }], \"includeReferencesDeep\": true }]: definition for this key is missing"}
```

```includeReferencesDeep``` will export all the dependencies of the dashboard
being exported, which is much simpler than trying to do them all individually.
Leaving this off resulted in just a silent failure, the import would respond
with a ```200 OK``` but then when I logged into Kibana it just wasn't there, and
I didn't see anything in the console logs for the service about it.

Apparently [ndjson](https://github.com/ndjson/ndjson-spec) is required for
saving this data. You shouldn't need to worry about that since the export
objects API will provide it for you, but if (like me) this is your first time
running into this particular flavor of JSON I thought I'd point it out.

## Import dashboard with all dependencies
```
curl -i -X POST http://kibana:5601/api/saved_objects/_import -H 'kbn-xsrf:true' --form file=@dashboards.ndjson
```
A few things to note:
- You must omit the Content-Type header
- You must pass the payload using ```--form file=``` and not ```-d```
- The file must be formatted as ndjson

If you include the Content-Type header here, or if you use the ```-d``` option
instead of ```--form file=```, you'll get the following error:
```
{"statusCode":415,"error":"Unsupported Media Type","message":"Unsupported Media Type"
```
Thanks to user samling on the opensearch forums for providing
[the solution](https://forum.opensearch.org/t/unable-to-upload-ndjson-files-to-api-saved-objects-import/3110/2)

## Importing in docker compose
All that's left is to automate the process, which I did like this:
```
kibana-init:
    image: alpine:latest
        container_name: kibana-init
        networks:
            elastic-stack-example:
        depends_on:
            kibana:
        condition: service_healthy
    volumes:
      - type: bind
        source: Logging/kibana/dashboards.ndjson
        target: /usr/share/kibana/data/dashboards.ndjson
        read_only: true
    working_dir: /usr/share/kibana/data
    command:
      - /bin/sh
      - -c
      - |
        apk --no-cache add curl
        curl -i -X POST http://kibana:5601/api/saved_objects/_import -H 'kbn-xsrf:true' --form file=@dashboards.ndjson
```

After you export the ndjson file, you can bind mount it into another container
and do the POST to your kibana instance.

You can test it by running a clean, and then clearing out all your docker images
and volumes:
```
docker system prune --all
docker volume prune --force
```
Now when you rebuild and run the system again, you'll see all your kibana stuff
already there. Neat huh?
