docker build -t factsgreet -f .\Web\FactsGreet.Web\Dockerfile . &&
docker tag factsgreet registry.heroku.com/factsgreet/web &&
docker push registry.heroku.com/factsgreet/web &&
heroku container:release web -a factsgreet