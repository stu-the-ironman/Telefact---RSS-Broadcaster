# Changelog

---

## [0.1.7.1] - 2025-04-03
### Added
- `EnableMusic` toggle support from `config.json` to allow testing without audio playback.
- Debug message confirms when music is disabled via config.

### Changed
- Updated `ConfigManager` to log warnings if `config.json` is missing, and fallback to defaults.

---

## [0.1.7] - 2025-04-03
### Added
- Background music system now plays MP3s from nested subdirectories inside `Assets/Music`.
- Expanded debug logs with clearer labels (e.g., "[MusicManager]") for easier diagnostics.

### Changed
- Timestamp in header now correctly appears fully in yellow.
- Improved variable naming in `MainForm` for better code readability.

### Fixed
- Subheader is now consistently displayed.
- Fixed previously incorrect timestamp coloring where the first character wasn't styled.

---

## [v0.1.6] - 2025-03-31

### Added
- Dynamic page handling with RSS feed integration.
- Static page handling for non-changing content (index, emergency contacts, etc.).
- Customizable RSS feed update interval.
- Caching for RSS feed data to avoid redundant fetches.
- Support for both dynamic and static pages in the Teletext UI.

### Changed
- Improved page rendering logic to accommodate both dynamic and static content.
- Refined overall UI structure for better user experience.

### Fixed
- Optimized performance when checking for RSS feed updates.
- Improved handling of invalid RSS feed data.

---

## [v0.1.5] - 2025-03-18

### Added
- Subpage cycling functionality with timer-based subpage navigation.
- Mock content and placeholder data for testing purposes.

### Changed
- Refined header layout and dynamic clock functionality.
- Added page number cycling for dynamic pages.

---

## [v0.1.4] - 2025-03-12

### Added
- Dynamic clock in the header with real-time updates.

### Changed
- Adjusted layout to improve readability of page numbers and service name.

---

## [v0.1.3] - 2025-03-05

### Added
- Basic Teletext window with background and color palette.
