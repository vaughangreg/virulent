# Virulent

This repository was retrieved from a backup of our SVN server.  This represents the last commit, January 24, 2017. 
This commit had a note "Updating to the latest version of unity", upgrading from 5.6.4p2 to 2017.3.0f3, and it included what appear to be some basic sdk updates.  I have no idea if this project will load.

*The below was auto generated with Claude - expect some mistakes.* -Greg 

**An educational action-strategy game about viral infection and systems biology**

> *"We focused on integrating educational content into the game mechanics instead of adding a layer of content to an already defined game genre."*
> — Nathan Patterson, Lead Designer

---

## About

Virulent is a free educational game developed by a cross-disciplinary team at the [Morgridge Institute for Research](https://morgridge.org/) at the University of Wisconsin–Madison. It was the Morgridge Institute's first digital learning game and was released in March 2011.

The game is designed for players **age 13 and older** — particularly middle school and high school students — and teaches key concepts in **systems biology**, with a focus on the cycle of viral infection and reproduction. Rather than layering educational content on top of existing game mechanics, the Virulent team built the learning directly into the gameplay itself.

---

## Gameplay

Players take control of **"Raven virus" particles (virions)**, modeled after the real-world *vesicular stomatitis virus* (VSV). The goal is to infect a host cell, replicate inside it, and escape to infect other cells — all while defending your virions from the cell's defenses and the immune system.

Through play, users gain an intuitive understanding of:

- The lifecycle of a virus (infection → replication → escape)
- The strengths and weaknesses of the body's biological defenses
- Concepts in virology and systems biology as practiced by active researchers
- The interactions between a virus and cellular and immune system responses

The game was play-tested by **more than 100 individuals** from area schools and youth groups, and early results showed students were both enjoying the game and retaining the scientific concepts.

---

## Background & Development

Virulent was created by a team from the **Education Research Challenge Area (ERCA)** at the Morgridge Institute for Research, whose mission is to make scientific discovery accessible to the public through digital games.

### Core Team

| Name | Role |
|---|---|
| Nathan Patterson | Researcher & Lead Game Designer |
| John Yin | Systems Biology Theme Leader, Wisconsin Institute for Discovery; Professor, UW–Madison Chemical & Biological Engineering |
| Collin Timm | UW–Madison graduate student (Yin lab); VSV subject matter expert |
| Jenny Gumperz | Associate Professor, UW–Madison Dept. of Medical Microbiology & Immunology |
| Richard Halverson | Associate Professor, UW–Madison Education Leadership & Policy Analysis |
| Kevin Harris | UW–Madison graduate student |
| Mike Beall | Artist/Programmer, Morgridge Institute |
| Ted Lauterbach | Artist/Programmer, Morgridge Institute |
| David Mann | Artist/Programmer, Morgridge Institute |

The Raven virus in the game is directly informed by Yin and Timm's real research on the vesicular stomatitis virus, which they study as a potential tool for destroying certain types of cancer.

---

## Technology

The source code in this repository is primarily written in:

- **C#** (53.8%) — Unity scripting
- **Java** (36.0%) — likely Android/cross-platform components
- **Objective-C** (9.0%) — iOS/iPad components
- Other (1.2%)

The game was built for and released on multiple platforms including iPad (iOS), Android tablets, web browsers, and as a standalone Windows/Mac application.

---

## Source Code

This repository contains the Unity project source for Virulent.

```
virulent/
├── Assets/            # Unity assets (scripts, art, audio, scenes)
├── ProjectSettings/   # Unity project configuration
├── .gitignore
└── LICENSE            # GPL-3.0
```

### Getting Started

1. **Install Unity** — 2017.3.0f3
2. **Open the project** — Launch Unity Hub, click *Add*, and point it to the root of this repository.
3. **Build and Run** — Use Unity's Build Settings to target your desired platform (Windows, macOS, WebGL, etc.).

---

## License

This project is licensed under the **GNU General Public License v3.0**. See [LICENSE](LICENSE) for full details.

---

## Links & Further Reading

- 🎮 [Learning Games Network – Virulent Microsite](https://learninggamesnetwork.org/microsites/virulent/)
- 📰 [UW–Madison News: Morgridge Institute Releases First Educational Game](https://news.wisc.edu/morgridge-institute-researchers-release-first-educational-game/)
- 🥔 [Potato Interactive – Developer Notes on Virulent](https://rottentater.com/virulent/)

---

## Contact / Feedback

For questions or feedback related to the original game, the historical contact was:
📧 virulent@morgridgeinstitute.org

For questions about this source release, please open a [GitHub Issue](https://github.com/vaughangreg/virulent/issues).
