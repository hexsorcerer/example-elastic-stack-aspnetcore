# Kibana config
## Environment variables
### [ELASTICSEARCH_HOSTS](https://www.elastic.co/guide/en/kibana/current/settings.html)
This is where you set the address to your Elasticsearch instance.

## Ports
### [5601](https://www.elastic.co/guide/en/kibana/current/settings.html)
The default port.

That's pretty much all you need to get it going, it's the simplest of the 3.

## Using Kibana
A guide on how to use Kibana is beyond the scope of this example, and I actually
don't know about a lot of things you can do with it anyways. But, a little
guidance...

First, if you load it before Elasticsearch is ready, you'll get prompted for
authentication. Assuming you have all the TLS settings disabled, you can just
wait a few momments and refresh and it should eventually take you in. The
healthcheck I'm using right now seems to work pretty well in this regard.

Once you get in there, use the hamburger menu over on the left, and under
```Analyze```, you'll be looking at ```Discover``` and ```Dashboards```.
```Discover``` is where you go to look through your logs, and ```Dashboards```
is, that's right you guessed it, where you go to look at your dashboards.

If you go under ```Discover``` and don't see anything there after your
application has logged something, then you did a oopsie somewhere and need to go
find it. If this is you, I hope the guide helps.
