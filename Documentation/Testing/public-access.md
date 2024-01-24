# Public access

For testing purposes, a locally-ran instance of Genfic can be exposed to the outside world.
Two popular ways would be using Ngrok or Cloudflare Tunnel

## Cloudflare Tunnel

1. Download [Cloudflared](https://github.com/cloudflare/cloudflared) CLI
2. Authenticate with `cloudflared tunnel login`
3. Create a new tunnel with `cloudflared tunnel create <NAME>`
   * This will display the tunnel's ID and the location of the config JSON file
4. Create a `config.yml` file in the `.cloudflared` directory
5. Add the following code:
   * ```yml
     tunnel: <TUNNEL ID> # tunnel ID from step 3
     credentials-file: <PATH TO CREDENTIALS FILE> # config file from step 3
     warp-routing:
       enabled: true
     ingress:
       - hostname: <HOSTNAME> # domain that will point to the Genfic instance, for example beta.genfic.net
         service: https://localhost:5001 # address of the running instance
       - service: http_status:404 # Required
     ```
6. Create a DNS record on Cloudflare with `cloudflared tunnel route dns <TUNNEL ID OR NAME> <HOSTNAME>`
7. Start the tunnel with `cloudflared tunnel run <TUNNEL ID OR NAME>`
8. Go to `<HOSTNAME>` to verify it works