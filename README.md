## Overview

This project is an interactive web app built with `Blazor Server` that runs [yt-dlp](https://github.com/yt-dlp/yt-dlp) on the server to download and extract videos into a configured folder. It is designed for self-hosting and can be paired with a NAS or shared network drive to simplify media collection & storage from anywhere on the network.

Features

- Web interface for submitting yt-dlp URLs
- Automatic extraction and storage to the server’s media folder
- Paginated video list with embedded playback
- Download from any device with access on the local network

> [!NOTE]
> Currently, the app does not expose custom yt-dlp parameters or advanced options.

## Usage

1. Deploy the container (see below)
2. Open the site in your browser at `http://localhost:8080` (or your configured endpoint). 
3. Paste a supported video URL and click *Extract*
4. Once processed, the video appears in the list and can be viewed or downloaded directly.

## Hosting with Docker

> [!WARNING]
> Security is minimal, if just not considered at all. Run the container behind a firewall or reverse proxy that handles authentication and TLS.

The provided `Dockerfile` installs `yt-dlp` and all required dependencies. 
The `docker-compose` is minimal. 
You only need to configure the exposed port and the volume mount path if you want downloaded videos to persist outside the container.