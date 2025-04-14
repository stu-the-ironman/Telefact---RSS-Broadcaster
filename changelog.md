---

### `CHANGELOG.md`

```markdown
# Changelog

---

## [0.1.9] - 2025-04-14
### Added
- Configurable `cellWidth` and `cellHeight` for Teletext standards.
- New visual effects: static, scanlines, banding flicker, and rolling scanline.
- Support for dynamic font loading from `Assets/Fonts`.

### Changed
- Refactored `Renderer` to use configurable grid dimensions.
- Improved `Effects` class to modularize visual effects logic.

### Fixed
- Resolved duplicate `Compile` item issue in the `.csproj` file.
- Fixed missing methods in `Effects` and properties in `ConfigSettings`.

---

## [0.1.8] - 2025-04-13
### Added
- Modular `Renderer` with support for header/footer rendering.
- Grid-aligned layout for consistent Teletext rendering.

### Changed
- Improved layout logic for better alignment with Teletext standards.

### Fixed
- Resolved layout inconsistencies in the header/footer.

---

## [0.1.8] - 2025-04-14
### Added
- Configurable `cellWidth` and `cellHeight` for Teletext standards.
- New visual effects: static, scanlines, banding flicker, and rolling scanline.
- Support for dynamic font loading from `Assets/Fonts`.

### Changed
- Refactored `Renderer` to use configurable grid dimensions.
- Improved `Effects` class to modularize visual effects logic.

### Fixed
- Resolved duplicate `Compile` item issue in the `.csproj` file.
- Fixed missing methods in `Effects` and properties in `ConfigSettings`.

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
