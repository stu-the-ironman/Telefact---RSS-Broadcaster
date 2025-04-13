# Telefact - Teletext RSS Broadcaster

**Telefact** is a modern Teletext broadcaster that emulates classic Teletext services, displaying text-only news from RSS feeds and static pages. It uses a 40x24 Teletext format and allows for both dynamic and static page creation. You can customize the background, colors, and page behavior.

---

## Features

- **Dynamic Pages**: Display content from RSS feeds with automatic updates.
- **Static Pages**: Pages like index or information pages that don't change often.
- **Customizable RSS Feed Update Interval**: Set the interval for checking and updating the RSS feed (default: 15 minutes).
- **Caching**: Stores RSS feed data locally to prevent redundant fetching and improve performance.
- **Customizable Layout**: Customize font, page layout, and content format.
- **Background Music Support**: Stream local MP3 files from nested folders under `Assets/Music` with automatic shuffling and debug logging.
- **Music Toggle**: Disable music playback using a boolean setting in `config.json` (`"EnableMusic": false`).
- **Dynamic Subpages**: Paginated stories rotate automatically per user-defined interval.
- **Debug Logging**: Color-coded console output for easier troubleshooting.

---

## Installation

1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/stu-the-ironman/Telefact---RSS-Broadcaster.git
