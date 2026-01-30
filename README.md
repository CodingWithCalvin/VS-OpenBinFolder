<p align="center">
  <img src="https://raw.githubusercontent.com/CodingWithCalvin/VS-OpenBinFolder/main/resources/logo.png" alt="Open Bin Folder Logo" width="128" height="128">
</p>

<h1 align="center">Open Bin Folder</h1>

<p align="center">
  <strong>Instantly open your project's output directory in Windows File Explorer!</strong>
</p>

<p align="center">
  <a href="https://github.com/CodingWithCalvin/VS-OpenBinFolder/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/CodingWithCalvin/VS-OpenBinFolder?style=for-the-badge" alt="License">
  </a>
  <a href="https://github.com/CodingWithCalvin/VS-OpenBinFolder/actions/workflows/build.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/CodingWithCalvin/VS-OpenBinFolder/build.yml?style=for-the-badge" alt="Build Status">
  </a>
</p>

<p align="center">
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-OpenBinFolder">
    <img src="https://img.shields.io/visual-studio-marketplace/v/CodingWithCalvin.VS-OpenBinFolder?style=for-the-badge" alt="Marketplace Version">
  </a>
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-OpenBinFolder">
    <img src="https://img.shields.io/visual-studio-marketplace/i/CodingWithCalvin.VS-OpenBinFolder?style=for-the-badge" alt="Marketplace Installations">
  </a>
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-OpenBinFolder">
    <img src="https://img.shields.io/visual-studio-marketplace/d/CodingWithCalvin.VS-OpenBinFolder?style=for-the-badge" alt="Marketplace Downloads">
  </a>
  <a href="https://marketplace.visualstudio.com/items?itemName=CodingWithCalvin.VS-OpenBinFolder">
    <img src="https://img.shields.io/visual-studio-marketplace/r/CodingWithCalvin.VS-OpenBinFolder?style=for-the-badge" alt="Marketplace Rating">
  </a>
</p>

---

A Visual Studio 2022/2026 extension that adds a right-click context menu command to open the project's bin folder. It intelligently uses the currently active solution/project configuration to navigate to the correct output folder (Debug, Release, etc.).

## Features

- **Right-click to open** - Simple context menu integration in Solution Explorer
- **Configuration-aware** - Opens the correct bin folder based on your active build configuration
- **Lightning fast** - No settings, no dialogs, just works!

## Installation

### Visual Studio Marketplace

1. Open Visual Studio 2022 or 2026
2. Go to **Extensions > Manage Extensions**
3. Search for "Open Bin Folder"
4. Click **Download** and restart Visual Studio

### Manual Installation

Download the latest `.vsix` from the [Releases](https://github.com/CodingWithCalvin/VS-OpenBinFolder/releases) page and double-click to install.

## Usage

1. Right-click on any project in **Solution Explorer**
2. Select **Open Bin Folder**
3. Windows File Explorer opens to your project's output directory

That's it! The extension automatically detects your active configuration (Debug/Release) and opens the corresponding output folder.

## Requirements

- Visual Studio 2022 (17.0) or later
- .NET Framework 4.8

## Contributing

Contributions are welcome! Whether it's bug reports, feature requests, or pull requests - all feedback helps make this extension better.

### Development Setup

1. Clone the repository
2. Open `src/CodingWithCalvin.OpenBinFolder.slnx` in Visual Studio 2022 or 2026
3. Ensure you have the "Visual Studio extension development" workload installed
4. Press F5 to launch the experimental instance

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Contributors

<!-- readme: contributors -start -->
<!-- readme: contributors -end -->

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/CodingWithCalvin">Coding With Calvin</a>
</p>
