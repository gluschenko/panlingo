# Instructions for Claude agents

This repository keeps agent skills in `.agents/skills/`.

For work involving language detection, language identification, MediaPipe, CLD2, CLD3, FastText, Whatlang, Lingua, or Panlingo language-code normalization, first read:

`.agents/skills/panlingo/SKILL.md`

Additional guidance is available in `.agents/skills/panlingo/references/`. Use `package-selection.md` when selecting a detector, the detector-specific reference for implementation details, `integration-patterns.md` for integration and tests, and `language-code.md` for code normalization.

The Panlingo skill is the repository's integration guide for .NET language-identification libraries. It describes model and package selection, explicit model loading, supported-platform checks, disposal of native resources, safe concurrency assumptions, adapter design, output normalization, and validation.

Treat `SKILL.md` and its references as the source of repository-specific agent guidance. Do not assume that general-purpose language-detection advice replaces them.
